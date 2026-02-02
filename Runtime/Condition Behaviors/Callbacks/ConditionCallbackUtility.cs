using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionCallbackUtility : MonoBehaviour, IConditionCallback
{
    [SerializeField]
    ConditionCallback conditionCallback = new();

    public void If() => conditionCallback.If();
    public void IfElse() => conditionCallback.IfElse();
}

public abstract class ConditionCallbackUtility<T> : MonoBehaviour, IConditionCallback<T>
{
    // TODO - add custom editor that displays if the conditionBehavior is a ConditionBehavior<T> of the same type
    [SerializeField]
    ConditionCallback<T> conditionCallback;

    public void If() => conditionCallback.If();

    public void IfElse() => conditionCallback.IfElse();

    public void If(T value) => conditionCallback.If(value);

    public void IfElse(T value) => conditionCallback.IfElse(value);
}
