using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

[CreateAssetMenu(fileName = "Switch Condition_ Callback_ Int_ New", menuName = "Behaviors/Conditions/Callbacks/Switch/Int")]
public class IntSwitchConditionCallbackBehavior : SwitchConditionCallbackBehavior<int> 
{
    public void Switch(IntVariable value) => Switch(value.Value);
}