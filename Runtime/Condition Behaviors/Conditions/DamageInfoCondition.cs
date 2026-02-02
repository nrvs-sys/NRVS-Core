using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition_ Damage Info_ New", menuName = "Behaviors/Conditions/Damage Info")]
public class DamageInfoCondition : ConditionBehavior<DamageInfo>
{
    [Header("Damage Value Condition")]
    public bool checkDamage;
    public float damage;
    public MathComparisonType damageComparison;

    [Header("Critical Hit Condition")]
    public bool checkCritical;
    public bool isCritical;

    [Header("Vulnerable Point Condition")]
    public bool checkVulnerablePoint;
    public bool isVulnerablePoint;

    [Header("Damage Source Condition")]
    public bool checkDamageSource;
    public StringConstant damageSource;

    [Header("Killing Blow Condition")]
    public bool checkKillingBlow;
    public bool isKillingBlow;

    protected override bool Evaluate(DamageInfo damageInfo)
    {
        // Check damage value condition
        if (checkDamage)
        {
            bool damagePass = MathComparison.Compare(damageComparison, damageInfo.damage, damage);

            if (!damagePass)
                return false;
        }

        // Check critical hit condition
        if (checkCritical && damageInfo.isCritical != isCritical)
            return false;

        // Check vulnerable point condition
        if (checkVulnerablePoint && damageInfo.isVulnerablePoint != isVulnerablePoint)
            return false;

        // Check damage source condition
        if (checkDamageSource && (damageSource == null || damageInfo.damageSource != damageSource))
            return false;

        // Check killing blow condition
        if (checkKillingBlow && damageInfo.isKillingBlow != isKillingBlow)
            return false;

        // All enabled conditions passed
        return true;
    }
}