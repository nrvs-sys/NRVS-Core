using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition_ List_ New", menuName = "Behaviors/Conditions/List")]
public class ListConditionBehavior : ConditionBehavior
{
    public enum ListConditionComparisonMode
    {
        All,
        Any,
        None
    }

    [Space(10)]

    public ListConditionComparisonMode comparisonMode;

    [Space(10)]

    [SerializeField]
    List<ConditionBehavior> conditions = new();

    protected override bool Evaluate() 
    {
        if (conditions.Count == 0)
            return true;

        switch (comparisonMode)
        {
            case ListConditionComparisonMode.All:
                return conditions.TrueForAll(c => c.If());
            case ListConditionComparisonMode.Any:
                foreach (var condition in conditions)
                    if (condition.If()) 
                        return true;
                return false;
            case ListConditionComparisonMode.None:
                return conditions.TrueForAll(c => !c.If());
        }

        return false;
    }
}
