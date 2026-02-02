using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableObjectBehavior : ScriptableObject
{
	public ConditionBehavior condition;

    public virtual void Invoke()
    {
        if (condition != null && !condition.If())
            return;

        Execute();
    }

    protected abstract void Execute();
}

public abstract class ScriptableObjectBehavior<T> : ScriptableObjectBehavior
{
    [Tooltip("Default value to use when invoking without a parameter")]
    public T defaultValue;

    public override void Invoke() => Invoke(defaultValue);

    public void Invoke(T value)
    {
        if (condition != null)
        {
            if (condition is ConditionBehavior<T> conditionWithParam)
            {
                if (!conditionWithParam.If(value))
                    return;
            }
            else if (!condition.If())
                return;
        }

        Execute(value);
    }

    protected override void Execute() => Execute(defaultValue);
    protected abstract void Execute(T value);
}