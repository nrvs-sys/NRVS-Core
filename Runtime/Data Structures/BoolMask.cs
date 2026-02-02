using System.Collections;
using System.Collections.Generic;
using System;


/// <summary>
/// Represents a boolean mask for enumerations with up to 8 distinct values.
/// </summary>
/// <typeparam name="T">An enumeration type.</typeparam>
public struct BoolMask<T> : IEquatable<BoolMask<T>> where T : Enum
{
    public byte mask;

    public BoolMask(byte mask)
    {
        this.mask = mask;
    }

    /// <summary>
    /// Implicitly converts the BoolMask to a boolean. Returns true if all bits are unset.
    /// </summary>
    /// <param name="m">The BoolMask instance.</param>
    public static implicit operator bool(BoolMask<T> m)
    {
        return m.IsTrue();
    }

    /// <summary>
    /// Sets or unsets the bit corresponding to the given enum value.
    /// </summary>
    /// <param name="enumValue">The enumeration value.</param>
    /// <param name="value">True to set the bit, false to unset it.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the enum value is outside the range 0-7.</exception>
    public void Set(T enumValue, bool value)
    {
        int enumIntValue = Convert.ToInt32(enumValue);

        if (enumIntValue < 0 || enumIntValue > 7)
            throw new ArgumentOutOfRangeException(nameof(enumValue), "Enum value must be between 0 and 7 to fit in a byte mask.");

        byte bit = (byte)(1 << enumIntValue);

        if (value)
            mask |= bit;
        else
            mask &= (byte)~bit;
    }

    /// <summary>
    /// Checks if the bit corresponding to the given enum value is set.
    /// </summary>
    /// <param name="enumValue">The enumeration value.</param>
    /// <returns>True if the bit is set; otherwise, false.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the enum value is outside the range 0-7.</exception>
    public bool IsSet(T enumValue)
    {
        int enumIntValue = Convert.ToInt32(enumValue);

        if (enumIntValue < 0 || enumIntValue > 7)
            throw new ArgumentOutOfRangeException(nameof(enumValue), "Enum value must be between 0 and 7 to fit in a byte mask.");

        byte bit = (byte)(1 << enumIntValue);

        return (mask & bit) != 0;
    }

    /// <summary>
    /// Returns true if all bits are unset in the mask.
    /// </summary>
    public bool IsTrue()
    {
        return mask == 0;
    }

    /// <summary>
    /// Clears all bits in the mask.
    /// </summary>
    public void Clear()
    {
        mask = 0;
    }

    public bool Equals(BoolMask<T> other)
    {
        return mask == other.mask;
    }

    public override bool Equals(object obj)
    {
        if (obj is BoolMask<T> other)
            return Equals(other);
        return false;
    }

    public override int GetHashCode()
    {
        return mask.GetHashCode();
    }

    public static bool operator ==(BoolMask<T> left, BoolMask<T> right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(BoolMask<T> left, BoolMask<T> right)
    {
        return !left.Equals(right);
    }
}