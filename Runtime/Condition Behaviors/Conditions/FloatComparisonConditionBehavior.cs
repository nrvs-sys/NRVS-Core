using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition_ Float Comparison_ New", menuName = "Behaviors/Conditions/Float Comparison")]
public class FloatComparisonConditionBehavior : ConditionBehavior<float>
{
    [SerializeField]
    MathComparisonType comparisonType;
    [SerializeField]
    FloatReference comparison;

    protected override bool Evaluate(float value) => MathComparison.Compare(comparisonType, value, comparison); 
}
