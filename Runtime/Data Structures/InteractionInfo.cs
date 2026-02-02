using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InteractionInfo : IEquatable<InteractionInfo>
{
	public GameObject interactingGameObject;
	public GameObject interactedGameObject;

	public InteractionInfo(GameObject interactingGameObject, GameObject interactedGameObject)
	{
		this.interactingGameObject = interactingGameObject;
		this.interactedGameObject = interactedGameObject;
	}

	public override string ToString()
	{
		return $"InteractionInfo: " +
			   $" Interacting GameObject: {interactingGameObject.name}," +
			   $" Interacted GameObject: {interactedGameObject.name}";
	}

	public override bool Equals(object obj) => obj is InteractionInfo other && Equals(other);

	public bool Equals(InteractionInfo other) =>
		interactingGameObject == other.interactingGameObject &&
		interactedGameObject == other.interactedGameObject;

	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;
			hash = hash * 23 + (interactingGameObject != null ? interactingGameObject.GetHashCode() : 0);
			hash = hash * 23 + (interactedGameObject != null ? interactedGameObject.GetHashCode() : 0);
			return hash;
		}
	}
}
