using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IConditionCallback
{
    void If();

    void IfElse();
}

public interface IConditionCallback<T> : IConditionCallback
{
    void If(T value);

    void IfElse(T value);
}

[Serializable]
public class ConditionCallback : IConditionCallback
{
    [SerializeField, Expandable]
    ConditionBehavior condition;

    [Space(10)]

    [SerializeField, TextArea]
    string developerNote;

    [Header("Callbacks")]

    [SerializeField]
    UnityEvent OnIf;
    [SerializeField]
    UnityEvent OnElse;

    public void If()
    {
        if (condition.If())
            OnIf?.Invoke();
    }

    public void IfElse()
    {
        if (condition.If())
            OnIf?.Invoke();
        else
            OnElse?.Invoke();
    }
}

[Serializable]
public class ConditionCallback<T> : IConditionCallback<T>
{
    [SerializeField, Expandable]
    ConditionBehavior condition;

    [Space(10)]

    [SerializeField, TextArea]
    string developerNote;

    [Header("Callbacks")]

    [SerializeField]
    UnityEvent<T> OnIf;
    [SerializeField]
    UnityEvent<T> OnElse;

    public void If()
    {
        if (condition.If())
        {
            if (condition is ConditionBehavior<T> conditionBehavior)
                OnIf?.Invoke(conditionBehavior.defaultValue);
            else
                OnIf?.Invoke(default);
        }
    }

    public void IfElse()
    {
        if (condition.If())
        {
            if (condition is ConditionBehavior<T> conditionBehavior)
                OnIf?.Invoke(conditionBehavior.defaultValue);
            else
                OnIf?.Invoke(default);
        }
        else
        {
            if (condition is ConditionBehavior<T> conditionBehavior)
                OnElse?.Invoke(conditionBehavior.defaultValue);
            else
                OnElse?.Invoke(default);
        }
    }

    public void If(T value)
    {
        if (condition is ConditionBehavior<T> conditionBehavior)
        {
            if (conditionBehavior.If(value))
                OnIf?.Invoke(value);
        }
        else
        {
            if (condition.If())
                OnIf?.Invoke(value);
        }
    }

    public void IfElse(T value)
    {
        if (condition is ConditionBehavior<T> conditionBehavior)
        {
            if (conditionBehavior.If(value))
                OnIf?.Invoke(value);
            else
                OnElse?.Invoke(value);
        }
        else
        {
            if (condition.If())
                OnIf?.Invoke(value);
            else
                OnElse?.Invoke(value);
        }
    }
}
