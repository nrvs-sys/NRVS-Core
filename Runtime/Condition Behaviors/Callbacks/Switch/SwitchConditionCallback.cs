using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;

public interface ISwitchConditionCallback<T>
{
    void Switch(T value);
}

[Serializable]
public class SwitchConditionCallback<T> : ISwitchConditionCallback<T>
{
    [Serializable]
    public struct CallbackEntry
    {
        public T value;
        public UnityEvent<T> callback;
        public CallbackEntry(T value, UnityEvent<T> callback)
        {
            this.value = value;
            this.callback = callback;
        }
    }

    [SerializeField, TextArea]
    string developerNote;

    [Space(10)]

    [SerializeField]
    List<CallbackEntry> callbacks;

    public void Switch(T value)
    {
        for (int i = 0; i < callbacks.Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(callbacks[i].value, value))
            {
                callbacks[i].callback?.Invoke(value);
                return;
            }
        }
    }
}
