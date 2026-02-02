using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SwitchConditionCallbackBehavior<T> : ScriptableObject, ISwitchConditionCallback<T>
{
    [SerializeField]
    SwitchConditionCallback<T> switchConditionCallback;

    public void Switch(T value) => switchConditionCallback.Switch(value);
}
