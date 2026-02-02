using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Condition_ Callback_ New", menuName = "Behaviors/Conditions/Callbacks/Callback")]
public class ConditionCallbackBehavior : ScriptableObject, IConditionCallback
{
    [SerializeField]
    ConditionCallback conditionCallback = new();

    public void If() => conditionCallback.If();
    public void IfElse() => conditionCallback.IfElse();
}

public abstract class ConditionCallbackBehavior<T> : ScriptableObject, IConditionCallback<T>
{
    [SerializeField]
    ConditionCallback<T> conditionCallback;

    public void If() => conditionCallback.If();

    public void IfElse() => conditionCallback.IfElse();

    public void If(T value) => conditionCallback.If(value);

    public void IfElse(T value) => conditionCallback.IfElse(value);
}
