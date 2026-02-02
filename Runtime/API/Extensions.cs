// jlink, 10-10-2018

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
#if !UNITY_IOS
using System.Reflection;
#endif
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using FishNet.Component.Prediction;

public static class Extensions
{
    #region Generic Extensions

    /// <summary>
    /// Determines whether the specified value is null or equal to its default value. An efficient way to check for null or default without boxing.
    /// </summary>
    /// <typeparam name="T">The type of the value to evaluate.</typeparam>
    /// <param name="value">The value to check for null or default.</param>
    /// <returns><see langword="true"/> if the specified value is null or equal to its default value; otherwise, <see
    /// langword="false"/>.</returns>
    public static bool IsNull<T>(T value)
    {
        return value is null || EqualityComparer<T>.Default.Equals(value, default(T));
    }

	#endregion

	#region Enum Extensions
	/// <summary>
	///		Returns true if a bitmasked enum contains another enum (or int).
	/// </summary>
	/// <remarks>Throws an ArgumentException error in the case of 'T' not being an Enum or int.</remarks>
	/// <typeparam name="T">Either an enum or int.</typeparam>
	/// <param name="Total">Enum to check containing within.</param>
	/// <param name="Value">Enum to check if contained within Total.  Can also be a bitmasked enum.</param>
	/// <returns>bool; If enum or int, returns if the Total contains the Value.  Otherwise, returns false.</returns>
	[Obsolete("Use HasFlags instead")]
    public static bool Contains<T>(this T Total, T Value) where T : struct, IConvertible
    {
        // Avoid boxing by using type checks and switch for int/enum
        Type t = typeof(T);
        if (t == typeof(int))
        {
            int total = (int)(object)Total;
            int value = (int)(object)Value;
            return (value & total) == value;
        }
        else if (t.IsEnum)
        {
            // Use Convert.ToInt32 to avoid boxing for enums
            int total = Convert.ToInt32(Total);
            int value = Convert.ToInt32(Value);
            return (value & total) == value;
        }

        throw new ArgumentException("'" + t + "' is not an enum or int, although an attempt was made to check if " + Total + " contains " + Value + ".");
    }

    /// <summary>
    /// Returns true if <paramref name="value"/> has any of the bits set in <paramref name="flags"/>.
    /// In other words, it performs (value & flags) != 0.
    /// </summary>
    public static bool HasAnyFlag<T>(this T value, T flags) where T : Enum
    {
        // Convert both to UInt64, so we can do a bitwise AND regardless of the underlying enum type.
        ulong v = Convert.ToUInt64(value);
        ulong f = Convert.ToUInt64(flags);
        return (v & f) != 0UL;
    }

	/// <summary>
	///		Creates a mask that shares the layers of two masks.
	///		Syntactic sugar for Union.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="flag"></param>
	public static T Add<T>(this Enum value, T flag)
	{

		return value.Union(flag);
	}

	/// <summary>
	///		Creates a mask that shares the layers of two masks.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="flag"></param>
	public static T Union<T>(this Enum value, T flag)
	{

		try
		{
			return (T)(object)((int)(object)value | (int)(object)flag);
		}
		catch (Exception exc)
		{
			throw new ArgumentException("Could not append flagged value \"" + flag + "\" to enum \"" + typeof(T).Name + "\".", exc);
		}
	}

	/// <summary>
	///		Creates a mask without the indicated layers, if any are present.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="flag"></param>
	public static T Remove<T>(this Enum value, T flag)
	{

		try
		{
			return (T)(object)((int)(object)value & ~(int)(object)flag);
		}
		catch (Exception exc)
		{
			throw new ArgumentException("Could not remove flagged value \"" + flag + "\" from enum \"" + typeof(T).Name + "\".", exc);
		}
	}

	/// <summary>
	///		Creates a mask that contains only the layers shared between two masks.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="flag"></param>
	public static T Intersection<T>(this Enum value, T flag)
	{

		try
		{
			return (T)(object)((int)(object)value & (int)(object)flag);
		}
		catch (Exception exc)
		{
			throw new ArgumentException("Could not create an intersection with flagged value \"" + flag + "\" from enum \"" + typeof(T).Name + "\".", exc);
		}
	}

	/// <summary>
	///		Creates a mask that contains only the layers shared between two masks.
	///		Syntactic sugar for Intersection.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="flag"></param>
	public static T SharedBetween<T>(this Enum value, T flag)
	{

		return value.Intersection(flag);
	}

	/// <summary>
	///		Creates a mask that contains only the layers not shared between two masks.
	///		NOTE: Is not the same as Remove!
	/// </summary>
	/// <param name="value"></param>
	/// <param name="flag"></param>
	public static T Difference<T>(this Enum value, T flag)
	{

		try
		{
			return (T)(object)((int)(object)value ^ (int)(object)flag);
		}
		catch (Exception exc)
		{
			throw new ArgumentException("Could not create a difference with flagged value \"" + flag + "\" from enum \"" + typeof(T).Name + "\".", exc);
		}
	}

	/// <summary>
	///		Creates a mask that contains only the layers not shared between two masks.
	///		NOTE: Is not the same as Remove!
	///		Syntactic sugar for Difference.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="flag"></param>
	public static T NotInCommon<T>(this Enum value, T flag)
	{

		try
		{
			return (T)(object)((int)(object)value ^ (int)(object)flag);
		}
		catch (Exception exc)
		{
			throw new ArgumentException("Could not create a difference with flagged value \"" + flag + "\" from enum \"" + typeof(T).Name + "\".", exc);
		}
	}

	/// <summary>
	///		Returns true if a bitmasked enum contains another enum of the same type.  Mimics the 4.0 HasFlag method.
	/// </summary>
	/// <param name="variable">The tested enum.</param>
	/// <param name="value">The value to test.</param>
	/// <returns>True if the flag is set. Otherwise false.</returns>
	public static bool HasFlag(this Enum variable, Enum value)
	{

		// check if from the same type.
		if (variable.GetType() != value.GetType())
			throw new ArgumentException("The checked flag is not from the same type as the checked variable. (" + variable.GetType() + " and " + value.GetType() + ")");

		int num = Convert.ToInt32(value);
		return (Convert.ToInt32(variable) & num) == num;
	}

	/// <summary>
	///		Given a variable of an Enum type, returns a List of all values within its Type (not limited to within the passed enum).
	/// </summary>
	/// <param name="value">The variable of Enum type that is used to get all values of the enum.</param>
	/// <returns></returns>
	public static IEnumerable<Enum> GetCollection(this Enum value)
	{

		return Enum.GetValues(value.GetType()).Cast<Enum>();
	}

	/// <summary>
	///		Returns a collection of all of the enums within a flag enum.
	///		If empty, collection has no values.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public static IEnumerable<Enum> GetFlags(this Enum value)
	{

		//return Enum.GetValues(value.GetType()).Cast<Enum>().Where(m => value.HasFlag(m));

		foreach (Enum flag in Enum.GetValues(value.GetType()))
			if (value.HasFlag(flag))
				yield return flag;
	}

	/// <summary>
	///		Returns a collection of all of the enums within a flag enum.  Unlike GetFlags(), this will cast to the given enum type.
	///		If empty, collection has no values.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="value"></param>
	/// <returns></returns>
	public static IEnumerable<T> GetFlags<T>(this Enum value) where T : struct, IConvertible
	{

		return value.GetFlags().Cast<T>();
	}

	/// <summary>
	///		Given an enum, returns the first flag present.  If no flag is present, returns null.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public static Enum GetFirstFlag(this Enum value)
	{

		IEnumerable<Enum> flags = GetFlags(value);
		if (flags == null)
			return null;
		return flags.First();
	}

	/// <summary>
	///		Given an enum, returns the last flag present.  If no flag is present, returns null.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public static Enum GetLastFlag(this Enum value)
	{

		IEnumerable<Enum> flags = GetFlags(value);
		if (flags == null)
			return null;
		return flags.Max();
	}

	/// <summary>
	///		Gets the description from an enum (using System.ComponentModel to utilize the DescriptionAttribute and System.Reflection to get the Attribute's value).
	///		For iOS (or for the case of no Description attribute), defaults to the string value of the enum.
	///		NOTE: Is not able to differentiate between two enum entries that share the same value.
	///			If differentiation is required, use EnumUtility.GetDescriptions() instead.
	/// </summary>
	/// <param name="value">The enum to return a Description for.</param>
	/// <returns>string?, either null if no Description set or a string otherwise.</returns>
	public static string GetDescription(this Enum value)
	{

#if !UNITY_IOS
		Type type = value.GetType();
		string name = Enum.GetName(type, value);
		if (name != null)
		{
			FieldInfo field = type.GetField(name);
			if (field != null)
			{
				System.ComponentModel.DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(System.ComponentModel.DescriptionAttribute)) as System.ComponentModel.DescriptionAttribute;
				if (attr != null)
					return attr.Description;
			}
		}
#endif
			
		return value.ToString();
	}

	/// <summary>
	///		Returns custom attribute classes on enums.
	/// </summary>
	/// <typeparam name="TAttribute">The Attribute class to return.</typeparam>
	/// <param name="value">The enum to get the attribute from.</param>
	/// <returns></returns>
	public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
	{

		var type = value.GetType();
		var name = Enum.GetName(type, value);
		return type.GetField(name)
			.GetCustomAttributes(false)
			.OfType<TAttribute>()
			.SingleOrDefault();
	}
	#endregion

	#region LayerMask Extensions
	/// <summary>
	///		Determines if a mask contains any of the layers in another mask.
	/// </summary>
	/// <param name="mask1"></param>
	/// <param name="mask2"></param>
	/// <returns></returns>
	public static bool ContainsAny(this LayerMask mask1, LayerMask mask2) {

		return (mask1.value & mask2.value) != 0;
	}

	/// <summary>
	///		Determines if a mask contains any of the passed layers.
	/// </summary>
	/// <param name="mask1"></param>
	/// <param name="layers">Layers, ranging from 0 to 31.  Any values exceeding this range are discarded.</param>
	/// <returns></returns>
	public static bool ContainsAny(this LayerMask mask, params int[] layers) {

		return (mask.value & CreateMask(layers)) != 0;
	}

	/// <summary>
	///		Determines if a mask contains all of the layers in another mask.
	/// </summary>
	/// <param name="mask1"></param>
	/// <param name="mask2"></param>
	/// <returns></returns>
	public static bool ContainsAll(this LayerMask mask1, LayerMask mask2) {

		return (mask1.value & mask2.value) == mask2.value;
	}

	/// <summary>
	///		Determines if a mask contains all of the passed layers.
	/// </summary>
	/// <param name="mask"></param>
	/// <param name="layers">Layers, ranging from 0 to 31.  Any values exceeding this range are discarded.</param>
	/// <returns></returns>
	public static bool ContainsAll(this LayerMask mask, params int[] layers) {

		int layersMask = CreateMask(layers);
		return (mask.value & layersMask) == layersMask;
	}

    /// <summary>
    /// Determines if a mask contains a single layer.
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool ContainsLayer(this LayerMask mask, int layer)
    {
        return (mask.value & (1 << layer)) != 0;
    }


    /// <summary>
    ///		Determines if a mask contains a passed GameObject's layer.
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="testObject"></param>
    /// <returns></returns>
    public static bool Contains(this LayerMask mask, GameObject testObject) {

		if (testObject == null)
			return false;

		return mask.ContainsLayer(testObject.layer);
	}

	/// <summary>
	///		Determines if a mask contains a passed Collider's layer.
	/// </summary>
	/// <param name="mask"></param>
	/// <param name="testCollider"></param>
	/// <returns></returns>
	public static bool Contains(this LayerMask mask, Collider testCollider) {

		if (testCollider == null)
			return false;

		return mask.ContainsLayer(testCollider.gameObject.layer);
	}

	/// <summary>
	///		Creates a mask that shares the layers of two masks.
	///		Syntactic sugar for Union(LayerMask, LayerMask).
	/// </summary>
	/// <param name="mask"></param>
	/// <param name="add"></param>
	/// <returns></returns>
	public static LayerMask Add(this LayerMask mask, LayerMask add) {

		return mask.Union(add);
	}

	/// <summary>
	///		Creates a mask that shares the layers of two masks.
	/// </summary>
	/// <param name="mask"></param>
	/// <param name="add"></param>
	/// <returns></returns>
	public static LayerMask Union(this LayerMask mask, LayerMask add) {

		return new LayerMask() {
			value = mask.value | add.value,
		};
	}

	/// <summary>
	///		Creates a mask that shares the layers of a mask and layers.
	///		Syntactic sugar for Union(LayerMask, params int[]).
	/// </summary>
	/// <param name="mask"></param>
	/// <param name="layers">Layers, ranging from 0 to 31.  Any values exceeding this range are discarded.</param>
	/// <returns></returns>
	public static LayerMask Add(this LayerMask mask, params int[] layers) {

		return mask.Union(layers);
	}

	/// <summary>
	///		Creates a mask that shares the layers of a mask and layers.
	/// </summary>
	/// <param name="mask"></param>
	/// <param name="layers">Layers, ranging from 0 to 31.  Any values exceeding this range are discarded.</param>
	/// <returns></returns>
	public static LayerMask Union(this LayerMask mask, params int[] layers) {

		return new LayerMask() {
			value = mask.value | CreateMask(layers),
		};
	}

	/// <summary>
	///		Creates a mask without the indicated layers, if any are present.
	/// </summary>
	/// <param name="mask"></param>
	/// <param name="remove"></param>
	/// <returns></returns>
	public static LayerMask Remove(this LayerMask mask, LayerMask remove) {

		return new LayerMask() {
			value = mask.value & ~(mask.value & remove.value),
		};
	}

	/// <summary>
	///		Creates a mask without the indicated layers, if any are present.
	/// </summary>
	/// <param name="mask"></param>
	/// <param name="layers">Layers, ranging from 0 to 31.  Any values exceeding this range are discarded.</param>
	/// <returns></returns>
	public static LayerMask Remove(this LayerMask mask, params int[] layers) {

		return new LayerMask() {
			value = mask.value & ~(mask.value & CreateMask(layers)),
		};
	}

	/// <summary>
	///		Creates a mask that contains only the layers shared between two masks.
	/// </summary>
	/// <param name="mask1"></param>
	/// <param name="mask2"></param>
	/// <returns></returns>
	public static LayerMask Intersection(this LayerMask mask1, LayerMask mask2) {

		return new LayerMask() {
			value = mask1.value & mask2.value,
		};
	}

	/// <summary>
	///		Creates a mask that contains only the layers shared between two masks.
	///		Syntactic sugar for Intersection(LayerMask, LayerMask).
	/// </summary>
	/// <param name="mask1"></param>
	/// <param name="mask2"></param>
	/// <returns></returns>
	public static LayerMask SharedBetween(this LayerMask mask1, LayerMask mask2) {

		return mask1.Intersection(mask2);
	}

	/// <summary>
	///		Creates a mask that contains only the layers shared between a mask and layers.
	/// </summary>
	/// <param name="mask"></param>
	/// <param name="layers">Layers, ranging from 0 to 31.  Any values exceeding this range are discarded.</param>
	/// <returns></returns>
	public static LayerMask Intersection(this LayerMask mask, params int[] layers) {

		return new LayerMask() {
			value = mask.value & CreateMask(layers),
		};
	}

	/// <summary>
	///		Creates a mask that contains only the layers shared between a mask and layers.
	///		Syntactic sugar for Intersection(LayerMask, params int[]).
	/// </summary>
	/// <param name="mask"></param>
	/// <param name="layers">Layers, ranging from 0 to 31.  Any values exceeding this range are discarded.</param>
	/// <returns></returns>
	public static LayerMask SharedBetween(this LayerMask mask, params int[] layers) {

		return mask.Intersection(layers);
	}

	/// <summary>
	///		Creates a mask that contains only the layers not shared between two masks.
	///		NOTE: Is not the same as Remove!
	/// </summary>
	/// <param name="mask1"></param>
	/// <param name="mask2"></param>
	/// <returns></returns>
	public static LayerMask Difference(this LayerMask mask1, LayerMask mask2) {

		return new LayerMask() {
			value = mask1.value ^ mask2.value,
		};
	}

	/// <summary>
	///		Creates a mask that contains only the layers not shared between two masks.
	///		NOTE: Is not the same as Remove!
	///		Syntactic sugar for Difference(LayerMask, LayerMask).
	/// </summary>
	/// <param name="mask1"></param>
	/// <param name="mask2"></param>
	/// <returns></returns>
	public static LayerMask NotInCommon(this LayerMask mask1, LayerMask mask2) {

		return mask1.Difference(mask2);
	}

	/// <summary>
	///		Creates a mask that contains only the layers not shared between a mask and layers.
	/// </summary>
	/// <param name="mask"></param>
	/// <param name="layers">Layers, ranging from 0 to 31.  Any values exceeding this range are discarded.</param>
	/// <returns></returns>
	public static LayerMask Difference(this LayerMask mask, params int[] layers) {

		return new LayerMask() {
			value = mask.value ^ CreateMask(layers),
		};
	}

	/// <summary>
	///		Creates a mask that contains only the layers not shared between a mask and layers.
	///		Syntactic sugar for Difference(LayerMask, params int[]).
	/// </summary>
	/// <param name="mask"></param>
	/// <param name="layers">Layers, ranging from 0 to 31.  Any values exceeding this range are discarded.</param>
	/// <returns></returns>
	public static LayerMask NotInCommon(this LayerMask mask, params int[] layers) {

		return mask.Difference(layers);
	}

	/// <summary>
	///		Internally creates a mask from provided layers.
	/// </summary>
	/// <param name="layers">Layers, ranging from 0 to 31.  Any values exceeding this range are discarded.</param>
	/// <returns></returns>
	private static int CreateMask(params int[] layers) {

		int layersMask = 0;
		foreach (int layer in layers) {
			if (layer < 0 || layer > 31)
				continue;
			layersMask |= 1 << layer;
		}

		return layersMask;
	}

	/// <summary>
	///		Converts a mask to a list of layer values.
	/// </summary>
	/// <param name="mask"></param>
	/// <returns>A List of layers, ranging from 0 to 31.</returns>
	public static List<int> ToList(this LayerMask mask) {

		List<int> layers = new List<int>();

		int i = 0;
		int bitmask = mask.value;
		int val;

		while (bitmask != 0 && i < 32) {
			val = 1 << i;
			if (Contains(bitmask, val)) {
				//	Remove from the bitmask
				bitmask = bitmask & ~(bitmask & val);
				layers.Add(i);
			}
			i++;
		}

		return layers;
	}

	/// <summary>
	/// Gets all GameObjects that match the LayerMask. Should not be used often as this operation can be VERY slow.
	/// </summary>
	public static List<GameObject> FindGameObjects(this LayerMask mask)
	{
		var goArray = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		var goList = new List<GameObject>();
		for (int i = 0; i < goArray.Length; i++)
		{
			if (mask.Contains(goArray[i]))
			{
				goList.Add(goArray[i]);
			}
		}
		if (goList.Count == 0)
		{
			return new List<GameObject>();
		}
		return goList;
	}
	#endregion

	#region Camera Culling Int Mask Extensions
	/// <summary>
	///		Adds passed layer(s) to a Camera's cullingMask.  Throws an exception if any of the layer names in Layernames does not match a valid layer.
	/// </summary>
	/// <param name="Bitmask">The Camera's cullingMask.</param>
	/// <param name="Layernames"></param>
	/// <returns>int; an updated cullingMask.</returns>
	public static int Add(this int Bitmask, params string[] LayerNames) {

		int layerTotal = 0;
		foreach (string layerName in LayerNames) {
			int layer = 1 << LayerMask.NameToLayer(layerName);
			if (layer < 0)
				throw new ArgumentOutOfRangeException("Layernames", layerName, "Passed layer name was invalid (not contained within Unity's layers).");
			layerTotal |= layer;
		}

		return Bitmask | layerTotal;
	}

	/// <summary>
	///		Removes passed layer(s) from a Camera's cullingMask.  Throws an exception if any of the layer names in Layernames does not match a valid layer.
	/// </summary>
	/// <param name="Bitmask">The Camera's cullingMask.</param>
	/// <param name="Layernames">The name of the layer to remove.</param>
	/// <returns>int; an updated cullingMask.</returns>
	public static int Remove(this int Bitmask, params string[] LayerNames) {

		return Bitmask & ~(0).Add(LayerNames);
	}

	/// <summary>
	///		Toggles passed layer(s) on/off on a Camera's cullingMask.  Throws an exception if any of the layer names in Layernames does not match a valid layer.
	/// </summary>
	/// <param name="Bitmask">The Camera's cullingMask.</param>
	/// <param name="Layernames">The name of the layer to toggle on/off.</param>
	/// <returns>int; an updated cullingMask.</returns>
	public static int Toggle(this int Bitmask, params string[] LayerNames) {

		return Bitmask ^ (0).Add(LayerNames);
	}

	/// <summary>
	///		Returns true if a Camera's cullingMask contains the passed layer (or, if multiple, all of the passed layers).  Throws an exception if any of the layer names in Layernames does not match a valid layer.
	/// </summary>
	/// <param name="Bitmask">The Camera's cullingMask.</param>
	/// <param name="Layernames">The name of the layer to test the presence for.</param>
	/// <returns>bool; the presence of the passed layer in the Bitmask.</returns>
	public static bool Contains(this int Bitmask, params string[] LayerNames) {

		int layerTotal = (0).Add(LayerNames);
		return (Bitmask & layerTotal) == layerTotal;
	}
    #endregion

    #region Vector3 Extensions

    /// <summary>
    /// Adds a random rotation to a forward vector within a specified cone.
    /// </summary>
    /// <param name="forward"></param>
    /// <param name="maxAngleDegrees"></param>
    /// <param name="minAngleDegrees"></param>
    /// <returns></returns>
    public static Vector3 AddRandomCone(this Vector3 forward, float maxAngleDegrees, float minAngleDegrees = 0)
	{
		// Convert angle to radians
		float maxAngleRad = maxAngleDegrees * Mathf.Deg2Rad;
		float minAngleRad = minAngleDegrees * Mathf.Deg2Rad;

		// Generate a random rotation around some axis perpendicular to forward
		// pick a random orientation by randomizing a perpendicular axis
		Vector3 randomOrthogonal = Vector3.Cross(forward, UnityEngine.Random.onUnitSphere);
		if (randomOrthogonal == Vector3.zero)
		{
			// fallback if forward and random sphere coincide
			randomOrthogonal = Vector3.Cross(forward, Vector3.up);
		}
		randomOrthogonal.Normalize();

		float angle = UnityEngine.Random.Range(minAngleRad, maxAngleRad);

		// rotate 'forward' about random axis
		Quaternion rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, randomOrthogonal);
		return (rotation * forward).normalized;
	}

	public static Vector3 RotateAroundPivot(this Vector3 point, Vector3 pivot, Vector3 angles)
	{
		return Quaternion.Euler(angles) * (point - pivot) + pivot;
	}

	public static Vector3 FlattenX(this Vector3 vector)
	{
		return new Vector3(0, vector.y, vector.z);
	}

	public static Vector3 FlattenY(this Vector3 vector)
	{
		return new Vector3(vector.x, 0, vector.z);
	}

	public static Vector3 FlattenXZ(this Vector3 vector)
	{
		return new Vector3(0, vector.y, 0);
	}

	/// <summary>
	/// Framerate Independent Damping.
	/// 
	/// Source: http://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/
	/// </summary>
	public static Vector3 Damp(this Vector3 source, Vector3 target, float lambda, float dt)
	{
		return Vector3.Lerp(source, target, 1f - Mathf.Exp(-lambda * dt));
	}

	/// <summary>
	/// Framerate Independent Damping.
	/// </summary>
	public static Vector3 Damp(this Vector3 value, float lambda, float dt)
	{
		return value.Damp(Vector3.zero, lambda, dt);
	}

	public static Bounds GetGridBounds(this Vector3 center, Vector3Int dimensions, int cellSize)
	{
		return new Bounds(center + (Vector3)dimensions * (cellSize * 0.5f), dimensions * new Vector3Int(cellSize, cellSize, cellSize));
	}

	/// <summary>
	/// Returns the delta vector between two positions.
	/// </summary>
	/// <param name="source"></param>
	/// <param name="target"></param>
	/// <returns></returns>
	public static Vector3 Delta(this in Vector3 source, in Vector3 target)
    {
		return target - source;
    }

	/// <summary>
	/// Returns the normalized projection of the supplied `direction`. 
	/// Slightly more efficient method than `Vector3.ProjectOnPlane()`, as this assumes `direction` is normalized.
	/// </summary>
	/// <param name="direction"></param>
	/// <param name="normal"></param>
	/// <returns></returns>
	public static Vector3 ProjectDirectionOnPlane(this in Vector3 direction, in Vector3 normal)
	{
		return (direction - normal * Vector3.Dot(direction, normal)).normalized;
	}

	/// <summary>
	/// Checks if a position is within a specified range of another position. This uses the more efficient squared distance method of comparison.
	/// </summary>
	/// <param name="position"></param>
	/// <param name="targetPosition"></param>
	/// <param name="range"></param>
	/// <returns>True if the positions are within the specified range, otherwise false.</returns>
	public static bool IsWithinRange(this Vector3 position, Vector3 targetPosition, float range)
	{
		float sqrDistance = position.GetSqrDistance(targetPosition);
		float sqrRange = range * range;

		return sqrDistance <= sqrRange;
	}

	public static float GetSqrDistance(this Vector3 position, Vector3 targetPosition)
	{
        return (position - targetPosition).sqrMagnitude;
    }

	public static bool Approximately(this Vector3 a, Vector3 b, float tolerance = 0.0001f)
	{
		return (a - b).sqrMagnitude < tolerance * tolerance;
	}

	public static bool IsZero(this Vector3 value) => value.x == 0 && value.y == 0 && value.z == 0;
	public static bool IsNaN(this Vector3 value) => float.IsNaN(value.x) || float.IsNaN(value.y) || float.IsNaN(value.z);
	public static bool IsInfinity(this Vector3 value) => float.IsInfinity(value.x) || float.IsInfinity(value.y) || float.IsInfinity(value.z);
	public static bool IsFinite(this Vector3 value) => !value.IsNaN() && !value.IsInfinity();
	
	/// <summary>
	/// Multiplies each component of the vector by a scalar.
	/// </summary>
	/// <param name="input">The input vector.</param>
	/// <param name="scalar">The scalar to multiply by.</param>
	/// <returns>The resulting vector after multiplication.</returns>
	public static Vector3 ScalarMultiply(this Vector3 input, float scalar)
	{
		return new Vector3(input.x * scalar, input.y * scalar, input.z * scalar);
	}
	
	/// <summary>
	/// Adds the components of two vectors.
	/// </summary>
	/// <param name="p">The first vector.</param>
	/// <param name="q">The second vector.</param>
	/// <returns>The resulting vector after addition.</returns>
	public static Vector3 Add(this Vector3 p, Vector3 q)
	{
		return new Vector3(p.x + q.x, p.y + q.y, p.z + q.z);
    }

    #endregion

    #region Vector2 Extensions

    public static Vector3 FlattenZ(this Vector3 vector)
	{
		return new Vector3(vector.x, vector.y, 0);
	}

	public static Vector2 GetXZ(this Vector3 vector)
	{
		return new Vector2(vector.x, vector.z);
	}

	/// <summary>
	/// Takes a normalized Vector2 and deadzone threshold, then returns a new Vector2 describing the deadzone.
	/// </summary>
	/// <param name="raw">A normalized input vector.</param>
	/// <param name="threshold">A normalized threshold (expecting values ranging from 0-1). This determines the minumum value
	/// of the raw input vector's magnitude in order to pass the deadzone.</param>
	/// <param name="linear">Determines if the output vector's magnitude should follow a linear curve as it increases, or if it should follow an exponential curve. This setting is purely for "game feel".</param>
	/// <returns>a normalized vector that represents the raw input relative to the deadzone threshold.</returns>
	public static Vector2 GetDeadzone(this Vector2 raw, float threshold, bool linear = true)
	{
		var d2 = raw.sqrMagnitude;

		// If d2 is lower than the threshold squared, then we're inside the deadzone
		if (d2 < threshold * threshold)
			return new Vector2(0, 0);
		else
		{
			// get actual distance from zero
			var d = Mathf.Sqrt(d2);

			// Normalise raw input
			var nx = raw.x / d;
			var ny = raw.y / d;

			// Rescaled so (dead->1) becomes (0->1)
			d = (d - threshold) / (1 - threshold);

			// and clamped down so we get 0 <= d <= 1
			d = Mathf.Min(d, 1);

			// Apply a curve for a smoother feel
			if (!linear)
				d *= d;

			// Scale normalised input and return
			return new Vector2(nx * d, ny * d);
		}
	}

    #endregion

    #region Quaternion Extensions

    /// <summary>
    /// Framerate Independent Damping.
    /// 
    /// Source: http://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/
    /// </summary>
    public static Quaternion Damp(this Quaternion source, Quaternion target, float lambda, float dt)
	{
		return Quaternion.Lerp(source, target, 1f - Mathf.Exp(-lambda* dt));
	}

	/// <summary>
	/// Framerate Independent Damping.
	/// </summary>
	public static Quaternion Damp(this Quaternion value, float lambda, float dt)
	{
		return value.Damp(Quaternion.identity, lambda, dt);
	}

	/// <summary>
	/// Returns the delta rotation between two rotations.
	/// </summary>
	/// <param name="source"></param>
	/// <param name="target"></param>
	/// <returns></returns>
	public static Quaternion Delta(this in Quaternion source, in Quaternion target)
    {
        return Quaternion.Inverse(source) * target;
	}

	public static bool IsZero(this Quaternion value) => value.x == 0 && value.y == 0 && value.z == 0 && value.w == 0;


    /// <summary>
    /// Multiplies each component of the quaternion by a scalar. The result is not normalized.
    /// </summary>
    /// <param name="input">The input quaternion.</param>
    /// <param name="scalar">The scalar to multiply by.</param>
    /// <returns>The resulting quaternion after multiplication.</returns>
    public static Quaternion ScalarMultiply(this Quaternion input, float scalar)
    {
        return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
    }

    /// <summary>
    /// Adds the components of two quaternions. The result is not normalized.
    /// </summary>
    /// <param name="p">The first quaternion.</param>
    /// <param name="q">The second quaternion.</param>
    /// <returns>The resulting quaternion after addition.</returns>
    public static Quaternion Add(this Quaternion p, Quaternion q)
    {
        return new Quaternion(p.x + q.x, p.y + q.y, p.z + q.z, p.w + q.w);
    }

    /// <summary>
    /// Performs a Lerp between two quaternions without potentially inverting the rotation.
    /// This can happen with Unity's Lerp method.
    /// </summary>
    /// <param name="p">The first quaternion.</param>
    /// <param name="q">The second quaternion.</param>
    /// <param name="t">The interpolation parameter.</param>
    /// <param name="shortWay">If true, will interpolate along the shortest path.</param>
    /// <returns>The interpolated quaternion.</returns>
    public static Quaternion Lerp(this Quaternion p, Quaternion q, float t, bool shortWay)
    {
        if (shortWay && Quaternion.Dot(p, q) < 0.0f)
        {
            p = p.ScalarMultiply(-1.0f);
        }

        float t1 = 1f - t;
        return new Quaternion(
			p.x * t1 + q.x * t, 
			p.y * t1 + q.y * t, 
			p.z * t1 + q.z * t, 
			p.w * t1 + q.w * t
		).normalized;
    }

    /// <summary>
    /// Performs a Slerp between two quaternions without potentially inverting the rotation.
    /// This can happen with Unity's Slerp method.
    /// </summary>
    /// <param name="p">The first quaternion.</param>
    /// <param name="q">The second quaternion.</param>
    /// <param name="t">The interpolation parameter.</param>
    /// <param name="shortWay">If true, will interpolate along the shortest path.</param>
    /// <returns>The interpolated quaternion.</returns>
    public static Quaternion Slerp(this Quaternion p, Quaternion q, float t, bool shortWay)
    {
        float dot = Quaternion.Dot(p, q);

        if (shortWay && dot < 0.0f)
        {
            return q.Slerp(p.ScalarMultiply(-1.0f), t, false);
        }

        float angle = Mathf.Acos(dot);
        float division = 1f / Mathf.Sin(angle);
        float t0 = Mathf.Sin((1f - t) * angle) * division;
        float t1 = Mathf.Sin(t * angle) * division;
        return new Quaternion(
            p.x * t0 + q.x * t1,
            p.y * t0 + q.y * t1,
            p.z * t0 + q.z * t1,
            p.w * t0 + q.w * t1
        ).normalized;
    }

	public static Quaternion GetLocalRotation(this Quaternion rotation, Transform transform) => Quaternion.Inverse(transform.rotation) * rotation;

    #endregion

    #region LineRenderer Extensions

    public static void SetStartEnd(this LineRenderer lineRenderer, Vector3 start, Vector3 end, float interval)
	{
		var distance = Vector3.Distance(start, end);
		var normal = (end - start).normalized;
		var count = Mathf.CeilToInt(distance / interval);
		var remainder = distance % interval;

		lineRenderer.positionCount = count;

		for (var i = 0; i < count; i++)
			lineRenderer.SetPosition(i, start + normal * (i * interval));

		if (remainder > 0)
		{
			lineRenderer.positionCount++;
			lineRenderer.SetPosition(count, start + normal * ((count - 1) * interval + remainder));
		}
	}

    public static float GetLineLength(this LineRenderer lineRenderer)
    {
        if (lineRenderer.positionCount > 1)
        {
            float lineLength = 0;

            Vector3[] positions = new Vector3[lineRenderer.positionCount];

            lineRenderer.GetPositions(positions);

            for (int i = 1; i < positions.Length; i++)
                lineLength += (positions[i] - positions[i - 1]).magnitude;

            return lineLength;
        }
        else
            return 0;
    }

    #endregion

    #region AnimationCurve Extensions

    /// <summary>
    /// Evaluates an AnimationCurve in reverse to find the input time value that corresponds to a given output value.
    /// </summary>
    /// <param name="curve">The AnimationCurve to evaluate.</param>
    /// <param name="value">The desired output value.</param>
    /// <param name="samples">The number of samples to use for evaluation.</param>
    /// <returns>The input time value that corresponds to the given output value.</returns>
    public static float InverseEvaluate(this AnimationCurve curve, float value, int samples = 1000)
    {
        float curveDuration = curve.keys[curve.length - 1].time;

        List<float> values = new List<float>();

        for (int i = 0; i <= samples; i++)
        {
            float time = (i / (float)samples) * curveDuration;

            values.Add(curve.Evaluate(time));
        }

        float closest = values.OrderBy(v => Mathf.Abs(value - v)).First();

        return (values.IndexOf(closest) / (float)samples) * curveDuration;
    }

	/// <summary>
	/// Get the duration of the animation curve
	/// </summary>
	/// <param name="curve"></param>
	/// <returns>The time of the last key in the curve. When there are less than two keys, 0 is returned.</returns>
	public static float GetDuration(this AnimationCurve curve) => curve.length >= 2 ? curve.keys[curve.length - 1].time : 0f;

    #endregion

    #region Bounds Extensions

    public static void Encapsulate(this Bounds bounds, GameObject gameObject, bool useRenderers = false)
	{
		bounds.Encapsulate(useRenderers ? gameObject.GetRendererBounds() : gameObject.GetColliderBounds());
	}

	public static List<Bounds> GetCells(this Bounds bounds, int gridSize)
	{
		gridSize = Mathf.Max(0, gridSize);

		// Break arena into cells based on its bounds
		Vector3 cellSize = new Vector3(bounds.size.x / gridSize, bounds.size.y / gridSize, bounds.size.z / gridSize);
		List<Bounds> boundsCells = new List<Bounds>();

		for (int y = 0; y < gridSize; y++)
		{
			for (int x = 0; x < gridSize; x++)
			{
				for (int z = 0; z < gridSize; z++)
				{
					Vector3 cellPosition = new Vector3(
						(cellSize.x / 2) + cellSize.x * x,
						(cellSize.y / 2) + cellSize.y * y,
						(cellSize.z / 2) + cellSize.z * z);

					cellPosition -= bounds.extents - bounds.center;

					Bounds cellBounds = new Bounds(cellPosition, cellSize);

					boundsCells.Add(cellBounds);
				}
			}
		}

		return boundsCells;
	}

	#endregion

	#region GameObject Extensions

	public static bool IsDestroyed(this GameObject gameObject)
	{
		return gameObject == null && !ReferenceEquals(gameObject, null);
	}

	public static Bounds GetRendererBounds(this GameObject gameObject)
	{
		var renderers = gameObject.GetComponentsInChildren<MeshRenderer>();

		if (renderers.Length > 0)
		{
			Bounds bounds = renderers[0].bounds;

			for (int i = 1, ni = renderers.Length; i < ni; i++)
				bounds.Encapsulate(renderers[i].bounds);

			return bounds;
		}
		else
		{
			return new Bounds(gameObject.transform.position, Vector3.zero);
		}
	}

	public static Bounds GetColliderBounds(this GameObject gameObject)
	{
		var colliders = gameObject.GetComponentsInChildren<Collider>();

		if (colliders.Length > 0)
		{
			Bounds bounds = colliders[0].bounds;

			for (int i = 1, ni = colliders.Length; i < ni; i++)
				bounds.Encapsulate(colliders[i].bounds);

			return bounds;
		}
		else
		{
			return new Bounds(gameObject.transform.position, Vector3.zero);
		}
	}

	public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T component) where T: Component
	{
		component = gameObject?.GetComponentInParent<T>();

		if (component == null)
			return false;

		return true;
	}

    public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component) where T : Component
	{
        component = gameObject?.GetComponentInChildren<T>();
        if (component == null)
            return false;
        return true;
    }

    public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T component, bool includeInactive) where T : Component
    {
        component = gameObject?.GetComponentInParent<T>(includeInactive);
        if (component == null)
            return false;
        return true;
    }
    public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component, bool includeInactive) where T : Component
    {
        component = gameObject?.GetComponentInChildren<T>(includeInactive);

        if (component == null)
            return false;

        return true;
    }

    /// <summary>
    /// Sets the layer of the game object and optionally its children.
    /// </summary>
    /// <param name="gameObject">The game object to set the layer for.</param>
    /// <param name="layer">The layer to set.</param>
    /// <param name="recursive">If true, also sets the layer of the game object's children.</param>
    public static void SetLayer(this GameObject gameObject, int layer, bool recursive = false)
    {
        gameObject.layer = layer;

        if (recursive)
		{
			for (int i = 0, ni = gameObject.transform.childCount; i < ni; i++)
			{
				Transform child = gameObject.transform.GetChild(i);
				child.gameObject.SetLayer(layer, recursive);
            }
        }
    }


    /// <summary>
    /// Replaces the layer of the game object and optionally its children.
    /// </summary>
    /// <param name="gameObject">The game object whose layer is to be replaced.</param>
    /// <param name="from">The current layer of the game object.</param>
    /// <param name="to">The new layer for the game object.</param>
    /// <param name="recursive">If set to true, the layer of the children of the game object will also be replaced.</param>
    public static void ReplaceLayer(this GameObject gameObject, int from, int to, bool recursive = false)
    {
        if (gameObject.layer == from)
            gameObject.layer = to;

		if (recursive)
		{
			for (int i = 0, ni = gameObject.transform.childCount; i < ni; i++)
			{
				Transform child = gameObject.transform.GetChild(i);
				child.gameObject.ReplaceLayer(from, to, recursive);
			}
		}
    }

	#endregion

	#region Transform Extensions

	/// <summary>
	/// Sets <see cref="Transform.localPosition"/>, <see cref="Transform.localEulerAngles"/>, and <see cref="Transform.localScale"/> to zero
	/// </summary>
	/// <param name="transform"></param>
	public static void ResetTransform(this Transform transform)
	{
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.localScale = Vector3.one;
	}

    public enum Direction
    {
        Any,
		Horizontal,
        Vertical,
        Forward,
        Backward,
        Left,
        Right,
        Up,
        Down
    }

	/// <summary>
	/// Returns the squared magnitude of a Vector relative to the specified direction.
	/// </summary>
	/// <param name="transform"></param>
	/// <param name="vector"></param>
	/// <param name="direction"></param>
	/// <returns></returns>
	public static float GetSqrMagnitude(this Transform transform, Vector3 vector, Direction direction = Direction.Any)
	{
		switch (direction)
		{
			case Direction.Any:
				return vector.sqrMagnitude;
			case Direction.Horizontal:
				{
					Vector3 plane = Vector3.ProjectOnPlane(vector, transform.up);
					return plane.sqrMagnitude;
				}
			case Direction.Vertical:
				{
					float dot = Vector3.Dot(vector, transform.up);
					return dot * dot;
				}
			case Direction.Forward:
				{
					float dot = Vector3.Dot(vector, transform.forward);
					if (dot < 0f) dot = 0f;
					return dot * dot;
				}
			case Direction.Backward:
				{
					// measure movement along forward axis, counting negative as well
					float dot = Vector3.Dot(vector, transform.forward);
					if (dot > 0f) dot = 0f;
					return dot * dot;
				}
			case Direction.Right:
				{
					float dot = Vector3.Dot(vector, transform.right);
					if (dot < 0f) dot = 0f;
					return dot * dot;
				}
			case Direction.Left:
				{
					float dot = Vector3.Dot(vector, transform.right);
					if (dot > 0f) dot = 0f;
					return dot * dot;
				}
			case Direction.Up:
				{
					float dot = Vector3.Dot(vector, transform.up);
					if (dot < 0f) dot = 0f;
					return dot * dot;
				}
			case Direction.Down:
				{
					float dot = Vector3.Dot(vector, transform.up);
					if (dot > 0f) dot = 0f;
					dot = Mathf.Abs(dot);
					return dot * dot;
				}
		}
		return vector.sqrMagnitude;
	}

	#endregion

	#region List Extensions

	/// <summary>
	/// Shuffles the element order of the specified list.
	/// </summary>
	public static void Shuffle<T>(this IList<T> ts)
	{
		var count = ts.Count;
		var last = count - 1;
		for (var i = 0; i < last; ++i)
		{
			var r = UnityEngine.Random.Range(i, count);
			var tmp = ts[i];
			ts[i] = ts[r];
			ts[r] = tmp;
		}
	}

	public static T GetRandomItem<T>(this IList<T> ts) => GetRandomItem(ts, out _);

    public static T GetRandomItem<T>(this IList<T> ts, out int index) => ts[index = UnityEngine.Random.Range(0, ts.Count)];

    public static T GetRandomItem<T>(this IList<T> ts, List<int> validIndices) => GetRandomItem(ts, validIndices, out _);

    public static T GetRandomItem<T>(this IList<T> ts, List<int> validIndices, out int index)
	{
        if (validIndices == null || validIndices.Count == 0)
        {
            return GetRandomItem(ts, out index);
        }

        index = validIndices[UnityEngine.Random.Range(0, validIndices.Count)];

        return ts[index];
    }

	public static bool IsValidIndex<T>(this IList<T> ts, int index) => index >= 0 && index < ts.Count;

	public static bool TryGetElementAt<T>(this IList<T> list, int index, out T value)
	{
		if (index >= 0 && index < list.Count)
		{
			value = list[index];
			return true;
		}

		value = default;
		return false;
	}

	#endregion

	#region Renderer Extensions

	static MaterialPropertyBlock _materialPropertyBlock;
    static MaterialPropertyBlock materialPropertyBlock => _materialPropertyBlock ??= new MaterialPropertyBlock();

    /// <summary>
    /// Sets a property of a material using a PropertyBlock (i.e. without create new material instances)
    /// </summary>
    /// <param name="renderer">Renderer to set the property of</param>
    /// <param name="propertyHash">Hash of the property</param>
    /// <param name="value">New value of the property</param>
    /// <param name="materialIndex">Material index on the renderer to set the property of</param>
    public static void SetProperty(this Renderer renderer, int propertyHash, int value, int materialIndex = 0)
    {
        if (materialIndex >= renderer.materials.Length)
        {
            Debug.LogWarning(
                $"Renderer {renderer.name} has {renderer.materials.Length.ToString()} materials. You are trying to access {materialIndex}. material.");
            return;
        }

        renderer.GetPropertyBlock(materialPropertyBlock, materialIndex);

        materialPropertyBlock.SetInt(propertyHash, value);

        renderer.SetPropertyBlock(materialPropertyBlock);
    }

    /// <summary>
    /// Sets a property of a material using a PropertyBlock (i.e. without create new material instances)
    /// </summary>
    /// <param name="renderer">Renderer to set the property of</param>
    /// <param name="propertyName">Name of the property</param>
    /// <param name="value">New value of the property</param>
    /// <param name="materialIndex">Material index on the renderer to set the property of</param>
    public static void SetProperty(this Renderer renderer, string propertyName, int value, int materialIndex = 0)
    {
        renderer.SetProperty(Shader.PropertyToID(propertyName), value, materialIndex);
    }

    /// <summary>
    /// Sets a property of a material using a PropertyBlock (i.e. without create new material instances)
    /// </summary>
    /// <param name="renderer">Renderer to set the property of</param>
    /// <param name="propertyHash">Hash of the property</param>
    /// <param name="value">New value of the property</param>
    /// <param name="materialIndex">Material index on the renderer to set the property of</param>
    public static void SetProperty(this Renderer renderer, int propertyHash, float value, int materialIndex = 0)
    {
        if (materialIndex >= renderer.materials.Length)
        {
            Debug.LogWarning(
                $"Renderer {renderer.name} has {renderer.materials.Length.ToString()} materials. You are trying to access {materialIndex}. material.");
            return;
        }

        renderer.GetPropertyBlock(materialPropertyBlock, materialIndex);

        materialPropertyBlock.SetFloat(propertyHash, value);

        renderer.SetPropertyBlock(materialPropertyBlock);
    }

    /// <summary>
    /// Sets a property of a material using a PropertyBlock (i.e. without create new material instances)
    /// </summary>
    /// <param name="renderer">Renderer to set the property of</param>
    /// <param name="propertyName">Name of the property</param>
    /// <param name="value">New value of the property</param>
    /// <param name="materialIndex">Material index on the renderer to set the property of</param>
    public static void SetProperty(this Renderer renderer, string propertyName, float value, int materialIndex = 0)
    {
        renderer.SetProperty(Shader.PropertyToID(propertyName), value, materialIndex);
    }

    /// <summary>
    /// Sets a property of a material using a PropertyBlock (i.e. without create new material instances)
    /// </summary>
    /// <param name="renderer">Renderer to set the property of</param>
    /// <param name="propertyHash">Hash of the property</param>
    /// <param name="value">New value of the property</param>
    /// <param name="materialIndex">Material index on the renderer to set the property of</param>
    public static void SetProperty(this Renderer renderer, int propertyHash, Color value, int materialIndex = 0)
    {
        if (materialIndex >= renderer.materials.Length)
        {
            Debug.LogWarning(
                $"Renderer {renderer.name} has {renderer.materials.Length.ToString()} materials. You are trying to access {materialIndex}. material.");
            return;
        }

        renderer.GetPropertyBlock(materialPropertyBlock, materialIndex);

        materialPropertyBlock.SetColor(propertyHash, value);

        renderer.SetPropertyBlock(materialPropertyBlock);
    }

    /// <summary>
    /// Sets a property of a material using a PropertyBlock (i.e. without create new material instances)
    /// </summary>
    /// <param name="renderer">Renderer to set the property of</param>
    /// <param name="propertyName">Name of the property</param>
    /// <param name="value">New value of the property</param>
    /// <param name="materialIndex">Material index on the renderer to set the property of</param>
    public static void SetProperty(this Renderer renderer, string propertyName, Color value, int materialIndex = 0)
    {
        renderer.SetProperty(Shader.PropertyToID(propertyName), value, materialIndex);
    }

    #endregion

    #region String Extensions

    public static bool IsNullOrEmpty(this string value)
    {
        return value == null || value.Length == 0;
    }

    public static string AddSpacesToSentence(this string text, bool preserveAcronyms = true)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;
        StringBuilder newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0]);
        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]))
                if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                    (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                        i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                    newText.Append(' ');
            newText.Append(text[i]);
        }
        return newText.ToString();
    }

    #endregion

    public static string SceneName(this SceneReference sceneReference) => System.IO.Path.GetFileNameWithoutExtension(sceneReference.ScenePath);

    public static void SetColorAlpha(this Material material, int property, float alpha)
    {
        if (material.HasProperty(property))
        {
            Color currentColor = material.GetColor(property);

            material.SetColor(property, new Color(currentColor.r, currentColor.g, currentColor.b, alpha));
        }
        else
            Debug.LogWarning($"Property \"{property}\" does not exist on {material.name}");
    }

    public static void SetColorAlpha(this Material material, string property, float alpha) => material.SetColorAlpha(Shader.PropertyToID(property), alpha);

    public static string BuildString<T>(this IEnumerable<T> ts, string delimiter = "\n")
	{
		StringBuilder sb = new StringBuilder();
		foreach (T v in ts)
		{
			sb.Append(v.ToString() + delimiter);
		}

		// Remove the last delimiter
		sb.Length = sb.Length - delimiter.Length;

		return sb.ToString();
	}

    /// <summary>
    /// Maps a value from one range to another range.
    /// </summary>
    /// <param name="x">The value to map.</param>
    /// <param name="in_min">The minimum value of the input range.</param>
    /// <param name="in_max">The maximum value of the input range.</param>
    /// <param name="out_min">The minimum value of the output range.</param>
    /// <param name="out_max">The maximum value of the output range.</param>
    /// <param name="clamp">Indicates whether the mapped value should be clamped within the output range.</param>
    /// <returns>The mapped value.</returns>
    public static float Map(this float x, float in_min, float in_max, float out_min, float out_max, bool clamp = false)
    {
        if (clamp) x = Mathf.Max(in_min, Math.Min(x, in_max));
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
 
	public static void ApplyDrag(this in float value, in float drag, in float dt, out float newValue)
	{
		var d = drag * dt;
		if (value - d > 0) newValue = value - d;
		else if (value + d < 0) newValue = value + d;
		else newValue = 0;
	}

	public static string GetOrdinal(this int num)
	{
		if (num <= 0) return num.ToString();

		switch (num % 100)
		{
			case 11:
			case 12:
			case 13:
				return num + "th";
		}

		switch (num % 10)
		{
			case 1:
				return num + "st";
			case 2:
				return num + "nd";
			case 3:
				return num + "rd";
			default:
				return num + "th";
		}
	}

	public static int ToInt(this bool value)
	{
		return value == true ? 1 : 0;
	}

	public static bool ToBool(this int value)
	{
		return value > 0 ? true : false;
	}

    public static int EnsureCapacity<T>(this Queue<T> q)
    {
#if UNITY_EDITOR
        return q.GetType().GetField("_array",
              System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
              .GetValue(q) is T[] arr ? arr.Length : q.Count;
#else
        return q.Count; // release: fallback
#endif
    }

	public static T GetOrCreate<T>(this Dictionary<string, T> dict, string key, int capacity) where T : class
	{
		if (!dict.TryGetValue(key, out T val))
			dict[key] = val = (T)Activator.CreateInstance(typeof(T), capacity);
		return val;
	}

    public static RigidbodyPauser GetRigidbodyPauser(this OfflineRigidbody offlineRigidbody) => (RigidbodyPauser)typeof(OfflineRigidbody).GetField("_rigidbodyPauser", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(offlineRigidbody);
}
