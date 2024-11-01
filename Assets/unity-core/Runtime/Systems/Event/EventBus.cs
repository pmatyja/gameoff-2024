using System;
using System.Collections.Concurrent;
using UnityEngine;

public static class EventBus
{
    private static class EventHub<T>
    {
        public static readonly EventChannelSO DefaultChannel = ScriptableObject.CreateInstance<EventChannelSO>();
        public static readonly ConcurrentDictionary<Action<object, T>, (EventChannelSO Channel, Func<T, bool> Predicate)> Subscribers = new();
    }

    public static void Raise<T>(object sender, T parameters = default)
    {
        Raise<T>(sender, null, parameters);
    }

    public static void Raise<T>(object sender, EventChannelSO eventChannel, T parameters = default)
    {
        eventChannel ??= EventHub<T>.DefaultChannel;

        foreach (var subscriber in EventHub<T>.Subscribers)
        {
            if (subscriber.Value.Predicate?.Invoke(parameters) == false)
            {
                continue;
            }

            if (subscriber.Value.Channel == null || subscriber.Value.Channel == eventChannel)
            {
                subscriber.Key.Invoke(sender, parameters);
            }
        }

        Logger.Developer($"Channel: {eventChannel}, Sender: {sender} Paramerters: \n{JsonUtility.ToJson(parameters)}");
    }

    public static void AddListener<T>(Action<object, T> listener, Func<T, bool> predicate = null)
    {
        AddListener<T>(listener, EventHub<T>.DefaultChannel, predicate);
    }

    public static void AddListener<T>(Action<object, T> listener, EventChannelSO eventChannel, Func<T, bool> predicate = null)
    {
        EventHub<T>.Subscribers.TryAdd(listener, (eventChannel ?? EventHub<T>.DefaultChannel, predicate));
    }

    public static void RemoveListener<T>(Action<object, T> listener)
    {
        EventHub<T>.Subscribers.TryRemove(listener, out var _);
    }
}