using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class IntConditionCallbackUtility : ConditionCallbackUtility<int> 
{
    public void If(IntVariable value) => If(value.Value);
    public void IfElse(IntVariable value) => IfElse(value.Value);
}
