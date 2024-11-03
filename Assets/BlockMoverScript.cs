using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMoverScript : MonoBehaviour
{
    [SerializeField]
    [Range(1f, 20f)]
    private float speedPerUnit = 10f;

    [SerializeField]
    [Range(1, 10)]
    private int distance = 3;

    [SerializeField]
    private BlockDirection direction = BlockDirection.PositiveY;

    [Header("Debug")]
    [SerializeField]
    private bool drawGizmos;
    [SerializeField]
    private bool drawGizmosSelected = true;

    [SerializeField]
    private bool manualTrigger;

    private Coroutine coroutine;
    private Vector3 rootPosition;
    private Vector3 pointA;
    private Vector3 pointB;

    public void Activate()
    {
        if (this.coroutine != null)
        {
            return;
        }

        this.coroutine = this.StartCoroutine(this.OnUpdate());
    }

    private void Start()
    {
        this.rootPosition = this.transform.position;
        this.pointA = this.transform.position;
        this.pointB = this.GetDestination();
    }

    private void OnDrawGizmos()
    {
        if (this.drawGizmos)
        {
            this.DrawGizmos();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (this.drawGizmosSelected)
        {
            this.DrawGizmos();
        }
    }
    
    private void DrawGizmos()
    {
        Debug.DrawLine(this.pointA, this.pointB, Color.yellow);

        var offset = this.transform.position - this.rootPosition;

        foreach (Transform child in this.transform)
        {
            DebugExtension.DrawBounds(new Bounds(this.pointA + (child.transform.position - offset), Vector3.one), Color.cyan);
            DebugExtension.DrawBounds(new Bounds(this.pointB + (child.transform.position - offset), Vector3.one), Color.cyan);
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying == false)
        {
            this.rootPosition = this.transform.position;
            this.pointA = this.transform.position;
            this.pointB = this.GetDestination();
            return;
        }

        if (this.manualTrigger == false)
        {
            return;
        }

        this.Activate();
        this.manualTrigger = false;
    }

    private IEnumerator OnUpdate()
    {
        var destination = this.GetDestination();

        while (this.isActiveAndEnabled)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, destination, this.speedPerUnit * Time.deltaTime);

            if (Vector3.Distance(this.transform.position, destination) < 0.01f)
            {
                this.transform.position = destination;
                break;
            }

            yield return null;
        }

        this.direction = NegateDirection(this.direction);
        this.coroutine = null;
    }

    private Vector3 GetDestination()
    {
        return this.transform.position + GetDirection(this.direction) * this.distance;
    }

    private static Vector3 GetDirection(BlockDirection direction)
    {
        switch (direction)
        {
            case BlockDirection.PositiveX:  return Vector3.right;
            case BlockDirection.NegativeX:  return Vector3.left;
            case BlockDirection.PositiveY:  return Vector3.up;
            case BlockDirection.NegativeY:  return Vector3.down;
            case BlockDirection.PositiveZ:  return Vector3.forward;
            case BlockDirection.NegativeZ:  return Vector3.back;
        }

        return Vector3.zero;
    }

    private static BlockDirection NegateDirection(BlockDirection direction)
    {
        switch (direction)
        {
            case BlockDirection.PositiveX:  return BlockDirection.NegativeX;
            case BlockDirection.NegativeX:  return BlockDirection.PositiveX;
            case BlockDirection.PositiveY:  return BlockDirection.NegativeY;
            case BlockDirection.NegativeY:  return BlockDirection.PositiveY;
            case BlockDirection.PositiveZ:  return BlockDirection.NegativeZ;
            case BlockDirection.NegativeZ:  return BlockDirection.PositiveZ;
        }

        return direction;
    }
}
