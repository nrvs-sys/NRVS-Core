using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ContactInfo : IEquatable<ContactInfo>
{
	/// <summary>
	/// The point of contact.
	/// </summary>
	public Vector3 point;
	/// <summary>
	/// The normal of the contact.
	/// </summary>
	public Vector3 normal;
	/// <summary>
	/// The velocity of the contact. May not be set in all cases. TODO - document when this is set.
	/// </summary>
	public Vector3 velocity;
	/// <summary>
	/// Normalized velocity
	/// </summary>
	public Vector3 direction => velocity.normalized;

	private Collider _contactingCollider;
	/// <summary>
	/// The collider that is contacting the other collider.
	/// </summary>
	public Collider contactingCollider
	{
		get => _contactingCollider;
		set
		{
			_contactingCollider = value;

			if (contactingCollider != null)
				contactingGameObject = contactingCollider.attachedRigidbody != null ? contactingCollider.attachedRigidbody.gameObject : contactingCollider.gameObject;
			else
				contactingGameObject = null;
		}
	}

	private Collider _contactedCollider;
	/// <summary>
	/// The "other" collider that is being contacted.
	/// </summary>
	public Collider contactedCollider
	{
		get => _contactedCollider;
		set
		{
			_contactedCollider = value;

			if (contactedCollider != null)
				contactedGameObject = contactedCollider.attachedRigidbody != null ? contactedCollider.attachedRigidbody.gameObject : contactedCollider.gameObject;
			else
				contactedGameObject = null;
		}
	}


    /// <summary>
    /// The originating object of the contact. This object must have the <see cref="FishNet.Object.NetworkObject"/> component attached. Sometimes, such as in the case of projectiles, the contact owner is not the same as the contacting game object.
    /// </summary>
    public GameObject contactOwner;

	/// <summary>
	/// The game object that is contacting the other game object.
	/// </summary>
	public GameObject contactingGameObject
	{
		get;
		set;
	}

	/// <summary>
	/// The "other" game object that is being contacted.
	/// </summary>
	public GameObject contactedGameObject
	{
		get;
		private set;
	}


	public static ContactInfo FromContactPoint(ContactPoint contactPoint, GameObject contactOwner)
	{
		return new ContactInfo()
		{
			point = contactPoint.point,
			normal = contactPoint.normal,
			contactingCollider = contactPoint.thisCollider,
			contactedCollider = contactPoint.otherCollider,
			contactOwner = contactOwner,
		};
	}

	public static ContactInfo FromContactPointWithVelocity(ContactPoint contactPoint, Vector3 velocity, GameObject contactOwner)
	{
		ContactInfo contact = FromContactPoint(contactPoint, contactOwner);

		contact.velocity = velocity;

		return contact;
	}

	public static ContactInfo FromCollider(Collider collider, Collider other, GameObject contactOwner)
	{
        Vector3 contactPoint = new();
        Vector3 contactNormal = new();

        if (Physics.ComputePenetration(
        collider, collider.transform.position, collider.transform.rotation,
        other, other.transform.position, other.transform.rotation,
            out Vector3 direction, out float distance))
        {
            contactPoint = collider.transform.position + direction * distance;
            contactNormal = -direction;
        }

        return new ContactInfo()
        {
            point = contactPoint,
            normal = contactNormal,
            contactingCollider = collider,
            contactedCollider = other,
            contactOwner = contactOwner,
        };
    }

	public static ContactInfo FromTrigger(Collider trigger, Collider other, GameObject contactOwner)
	{
		Vector3 contactPoint = new();
		Vector3 contactNormal = new();

        if (Physics.ComputePenetration(
			trigger, trigger.transform.position, trigger.transform.rotation,
            other, other.transform.position, other.transform.rotation,
            out Vector3 direction, out float distance))
        {
            contactPoint = trigger.transform.position + direction * distance;
            contactNormal = -direction;
        }

        return new ContactInfo()
		{
            point = contactPoint,
            normal = contactNormal,
            contactingCollider = trigger,
            contactedCollider = other,
            contactOwner = contactOwner,
        };
    }

	public static ContactInfo FromTriggerWithVelocity(Collider trigger, Collider other, Vector3 velocity, GameObject contactOwner)
	{
        ContactInfo contact = FromTrigger(trigger, other, contactOwner);

        contact.velocity = velocity;

        return contact;
    }

	public static ContactInfo FromRaycastHit(RaycastHit raycastHit, GameObject contactOwner)
	{
		return new ContactInfo()
		{
			point = raycastHit.point,
			normal = raycastHit.normal,
			contactedCollider = raycastHit.collider,
			contactOwner = contactOwner,
		};
	}

	public InteractionInfo GetInteractionInfo() => new InteractionInfo(contactOwner ?? contactingGameObject, contactedGameObject);

    public bool Equals(ContactInfo other)
    {
        return point.Equals(other.point)
            && normal.Equals(other.normal)
            && velocity.Equals(other.velocity)
            && ReferenceEquals(_contactingCollider, other._contactingCollider)
            && ReferenceEquals(_contactedCollider, other._contactedCollider)
            && ReferenceEquals(contactOwner, other.contactOwner)
            && ReferenceEquals(contactingGameObject, other.contactingGameObject)
            && ReferenceEquals(contactedGameObject, other.contactedGameObject);
    }

    public override bool Equals(object obj)
    {
        return obj is ContactInfo other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = (hash * 23) + point.GetHashCode();
            hash = (hash * 23) + normal.GetHashCode();
            hash = (hash * 23) + velocity.GetHashCode();
            hash = (hash * 23) + (_contactingCollider != null ? _contactingCollider.GetHashCode() : 0);
            hash = (hash * 23) + (_contactedCollider != null ? _contactedCollider.GetHashCode() : 0);
            hash = (hash * 23) + (contactOwner != null ? contactOwner.GetHashCode() : 0);
            hash = (hash * 23) + (contactingGameObject != null ? contactingGameObject.GetHashCode() : 0);
            hash = (hash * 23) + (contactedGameObject != null ? contactedGameObject.GetHashCode() : 0);
            return hash;
        }
    }

    public static bool operator ==(ContactInfo left, ContactInfo right)
        => left.Equals(right);

    public static bool operator !=(ContactInfo left, ContactInfo right)
        => !left.Equals(right);


    public override string ToString()
	{
		return string.Format("ContactInfo:\n" +
							 "Point: {0}\n" +
							 "Normal: {1}\n" +
							 "Velocity: {2}\n" +
							 "Direction: {3}\n" +
							 "Contacting Collider: {4}\n" +
							 "Contacting Game Object: {5}\n" +
							 "Contacted Collider: {6}\n" +
							 "Contacted Game Object: {7}\n" +
							 "Contact Owner: {8}\n",
			point,
			normal,
			velocity,
			direction,
			contactingCollider != null ? contactingCollider.name : "null",
			contactingGameObject != null ? contactingGameObject.name : "null",
			contactedCollider != null ? contactedCollider.name : "null",
			contactedGameObject != null ? contactedGameObject.name : "null",
			contactOwner != null ? contactOwner.name : "null");
	}
}