using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition_ Contact Layer Mask_ New", menuName = "Behaviors/Conditions/Contact Layer Mask")]
public class ContactLayerMaskCondition : ConditionBehavior<ContactInfo>
{
	public LayerMask layerMask;

	protected override bool Evaluate(ContactInfo contactInfo) => layerMask.Contains(contactInfo.contactedGameObject);
}