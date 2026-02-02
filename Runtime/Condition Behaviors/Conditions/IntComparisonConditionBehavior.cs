using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition_ Int Comparison_ New", menuName = "Behaviors/Conditions/Int Comparison")]
public class IntComparisonConditionBehavior : ConditionBehavior<int>
{
    [SerializeField]
    MathComparisonType comparisonType;
    [SerializeField]
    IntReference comparison;
    protected override bool Evaluate(int value) => MathComparison.Compare(comparisonType, value, comparison);
}
