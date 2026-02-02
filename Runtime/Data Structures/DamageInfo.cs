using FishNet.CodeGenerating;
using FishNet.Serializing;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityAtoms.BaseAtoms;
using UnityEngine;

[UseGlobalCustomSerializer]
public struct DamageInfo : IEquatable<DamageInfo>
{
	public float damage;
    public StringConstant damageSource;

    public bool isCritical;
	public bool isVulnerablePoint;
    public bool isKillingBlow;

	public Vector3 point;
	public InteractionInfo interactionInfo;

	public DamageInfo(float damage, bool isCritical, bool isVulnerablePoint, StringConstant damageSource, Vector3 point, InteractionInfo interactionInfo, bool isKillingBlow = false)
	{
		this.damage = damage;
		this.isCritical = isCritical;
		this.isVulnerablePoint = isVulnerablePoint;
		this.damageSource = damageSource;
		this.point = point;
		this.interactionInfo = interactionInfo;
		this.isKillingBlow = isKillingBlow;
	}

	public bool Equals(DamageInfo other)
	{
		return damage.Equals(other.damage) &&
			   isCritical == other.isCritical &&
			   isVulnerablePoint == other.isVulnerablePoint &&
			   isKillingBlow == other.isKillingBlow &&
			   Equals(damageSource, other.damageSource) &&
			   point.Equals(other.point) &&
			   interactionInfo.Equals(other.interactionInfo);
    }

	public override bool Equals(object obj)
	{
		return obj is DamageInfo other && Equals(other);
    }

	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;
			hash = hash * 31 + damage.GetHashCode();
			hash = hash * 31 + isCritical.GetHashCode();
			hash = hash * 31 + isVulnerablePoint.GetHashCode();
			hash = hash * 31 + (damageSource != null ? damageSource.GetHashCode() : 0);
			hash = hash * 31 + point.GetHashCode();
			hash = hash * 31 + interactionInfo.GetHashCode();
			hash = hash * 31 + isKillingBlow.GetHashCode();
			return hash;
        }
    }

    public override string ToString()
	{
		return $"DamageInfo: " +
			   $"  Damage: {damage}," +
			   $"  Critical Hit: {isCritical}," +
			   $"  Vulnerable Point: {isVulnerablePoint}," +
			   $"  Damage Source: {damageSource}," +
			   $"  Point: {point}," +
			   $"  Interaction Info: {interactionInfo}," +
			   $"  Is Killing Blow: {isKillingBlow}";
	}
}

public static class DamageInfoExtensions
{
	public static void WriteDamageInfo(this Writer writer, DamageInfo damageInfo)
	{
		writer.Write(damageInfo.damage);

		int hash = 0;
		bool hasDamageSource = Ref.TryGet(out DamageSourceCache damageSourceCache) && damageSourceCache.TryGetHash(damageInfo.damageSource, out hash);

		var bools = new bool4(damageInfo.isCritical, damageInfo.isVulnerablePoint, damageInfo.isKillingBlow, hasDamageSource);

		//writer.Write(bools);
		// Temp workaround for bool4 serialization
		{
			byte b = 0;

			if (bools.x)
				b |= 1;
			if (bools.y)
				b |= 2;
			if (bools.z)
				b |= 4;
			if (bools.w)
				b |= 8;

			writer.Write(b);
		}

        if (hasDamageSource)
		{
			writer.Write(hash);
		}

		writer.Write(damageInfo.point);
		writer.Write(damageInfo.interactionInfo);
	}

	public static DamageInfo ReadDamageInfo(this Reader reader)
	{
		float damage = reader.Read<float>();

		bool4 bools; //= reader.Readbool4();

        // Temp workaround for bool4 deserialization
        {
            byte b = reader.Read<byte>();

			bools = new bool4
			{
				x = (b & 1) != 0,
				y = (b & 2) != 0,
				z = (b & 4) != 0,
				w = (b & 8) != 0
			};
		}

        bool isCritical = bools.x;
        bool isVulnerablePoint = bools.y;
        bool isKillingBlow = bools.z;
        bool hasDamageSource = bools.w;

        StringConstant damageSource = null;
		if (hasDamageSource && Ref.TryGet(out DamageSourceCache damageSourceCache))
		{
			int damageSourceHash = reader.Read<int>();
			if (!damageSourceCache.TryGetByHash(damageSourceHash, out damageSource))
				Debug.LogError("DamageSource not found. Please ensure it is registered in the Damage Source Cache.");
		}

		Vector3 point = reader.Read<Vector3>();
		InteractionInfo interactionInfo = reader.Read<InteractionInfo>();

		return new DamageInfo(damage, isCritical, isVulnerablePoint, damageSource, point, interactionInfo, isKillingBlow);
	}
}
