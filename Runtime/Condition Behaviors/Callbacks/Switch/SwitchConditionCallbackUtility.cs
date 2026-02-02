using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchConditionCallbackUtility<T> : MonoBehaviour, ISwitchConditionCallback<T>
{
    [SerializeField]
    SwitchConditionCallback<T> switchConditionCallback;

    public void Switch(T value) => switchConditionCallback.Switch(value);
}
