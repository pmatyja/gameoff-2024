using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainBehvaiour : MonoBehaviour
{
    [SerializeField]
    [Readonly]
    private BrainActivitySO currentActivity;

    [SerializeField]
    [Readonly]
    private Animator animator;

    private Coroutine coroutine;

    [SerializeField]
    [Expandable]
    private BrainActivitySO[] activities;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float interrupDetectionRate = 0.1f;

    [SerializeField]
    [Readonly]
    private List<CooldownActivity> cooldownActivites = new();

    private class CooldownActivity
    {
        public BrainActivitySO Activity;
        public float Time;
    }

    public bool IsRunningActivity(BrainActivitySO activity)
    {
        return this.currentActivity == activity;
    }

    public void SetActivity(BrainActivitySO activitySO)
    {
        this.currentActivity = activitySO;
    }

    private void OnEnable()
    {
        this.animator = this.GetComponentInChildren<Animator>();
        this.coroutine = this.StartCoroutine(this.OnTick());

        EventBus.AddListener<SensorEventParameters>(this.OnSensorEvent);
        EventBus.AddListener<NoiseDetectedEventParameters>(this.OnNoiseDetectedEvent);
        EventBus.AddListener<AttackEventParameters>(this.OnAttackEvent);
    }

    private void OnDisable()
    {
        this.StopCoroutine(this.coroutine);
        this.coroutine = null;

        EventBus.RemoveListener<SensorEventParameters>(this.OnSensorEvent);
        EventBus.RemoveListener<NoiseDetectedEventParameters>(this.OnNoiseDetectedEvent);
        EventBus.RemoveListener<AttackEventParameters>(this.OnAttackEvent);
    }

    private void Update()
    {
        for (var i = 0; i < this.cooldownActivites.Count; )
        {
            this.cooldownActivites[i].Time -= Time.deltaTime;

            if (this.cooldownActivites[i].Time < 0.0f)
            {
                this.cooldownActivites.RemoveAt(i);
                continue;
            }

            ++i;
        }
    }

    private void OnValidate()
    {
        this.animator = this.GetComponentInChildren<Animator>();
    }

    private void OnNoiseDetectedEvent(object sender, NoiseDetectedEventParameters parameters)
    {
        this.EventInterruptsActivit(sender as GameObject);
    }

    private void OnSensorEvent(object sender, SensorEventParameters parameters)
    {
        if (parameters.DetectionFactor > this.interrupDetectionRate)
            this.EventInterruptsActivit(sender as GameObject);
    }

    private void OnAttackEvent(object sender, AttackEventParameters parameters)
    {
        this.EventInterruptsActivit(sender as GameObject);
    }

    private void EventInterruptsActivit(GameObject sender)
    {
        if (this.gameObject != sender)
        {
            return;
        }

        var newActivity = this.FindNewActivity();

        if (this.currentActivity == newActivity)
        {
            return;
        }

        if (this.currentActivity != default)
        {
            if (newActivity.GetPriority(this) > this.currentActivity.GetPriority(this))
            {
                this.currentActivity = newActivity;                
            }
        }
    }

    private IEnumerator OnTick()
    {
        while (this.isActiveAndEnabled)
        {
            if (this.currentActivity != default)
            {
                if (string.IsNullOrWhiteSpace(this.currentActivity.AnimatorState) == false)
                {
                    this.animator?.Play(this.currentActivity.AnimatorState);
                }

                this.AddCooldown(this.currentActivity);

                yield return this.currentActivity.OnUpdate(this);
            }

            this.currentActivity = this.FindNewActivity();

            yield return null;
        }
    }

    private BrainActivitySO FindNewActivity()
    {
        var bestActivity = default(BrainActivitySO);
        var bestScore = float.MinValue;

        foreach (var activity in activities)
        {
            if (this.CanActivate(activity) == false)
            {
                continue;
            }

            var newScore = activity.GetPriority(this);

            if (newScore > bestScore)
            {
                bestActivity = activity;
                bestScore = newScore;
            }
            else if (newScore == bestScore)
            {
                if (Rng.Boolean(ref Rng.Seed))
                {
                    bestActivity = activity;
                    bestScore = newScore;
                }
            }
        }

        return bestActivity;
    }

    private bool CanActivate(BrainActivitySO activity)
    {
        if (activity.CanActivate(this) == false)
        {
            return false;
        }

        for (var i = 0; i < this.cooldownActivites.Count; i++)
        {
            if (this.cooldownActivites[i].Activity == activity)
            {
                if (this.cooldownActivites[i].Time > 0.0f)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void AddCooldown(BrainActivitySO activity)
    {
        if (this.currentActivity.Cooldown <= 0.0f)
        {
            return;
        }

        if (this.cooldownActivites.Any(x => x.Activity == activity))
        {
            return;
        }

        this.cooldownActivites.Add(new CooldownActivity
        {
            Activity = this.currentActivity,
            Time = this.currentActivity.Cooldown
        });
    }
}