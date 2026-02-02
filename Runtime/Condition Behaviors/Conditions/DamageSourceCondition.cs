using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition_ Damage Source_ New", menuName = "Behaviors/Conditions/Damage Source")]
public class DamageSourceCondition : ConditionBehavior<DamageInfo>
{
	public List<StringConstant> damageSources;

	protected override bool Evaluate(DamageInfo damageInfo)
	{
		foreach (StringConstant damageSource in damageSources)
		{
			if (damageInfo.damageSource == damageSource)
				return true;
		}

		return false;
	}
}