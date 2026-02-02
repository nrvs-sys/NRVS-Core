using FishNet.CodeGenerating;
using FishNet.Serializing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UseGlobalCustomSerializer]
public struct ByteBool : IEquatable<ByteBool>
{
    [Flags]
    public enum BoolState : byte
    {
        Off = 0,
        False = 1,
        True = 2
    }

    private BoolState state
    {
        get => (BoolState)byteValue;
        set => byteValue = (byte)value;
    }

    public ByteBool(bool value)
    {
        byteValue = 0; // Initialize to Off state
        state = value ? BoolState.True : BoolState.False;
    }

    public ByteBool(byte value)
    {
        byteValue = value;
    }

    public ByteBool(BoolState state)
    {
        byteValue = (byte)state;
    }

    public bool Value
    {
        get
        {
            if (state.HasFlag(BoolState.True))
                return true;
            if (state.HasFlag(BoolState.False))
                return false;

            throw new InvalidOperationException("Cannot get value when in 'Off' state.");
        }
        set
        {
            state = value ? BoolState.True : BoolState.False;
        }
    }

    public byte byteValue;

    public BoolState GetState() => state;

    public void SetState(BoolState state) => this.state = state;

    public bool IsTrue => state.HasFlag(BoolState.True);

    public bool IsFalse => state.HasFlag(BoolState.False);

    public bool HasValue => state != BoolState.Off;

    public bool IsOff => state == BoolState.Off;

    public static implicit operator bool(ByteBool byteBool) => byteBool.Value;

    public static implicit operator ByteBool(bool value) => new ByteBool(value);

    public static ByteBool operator &(ByteBool operand1, ByteBool operand2) => new ByteBool(operand1.GetState() & operand2.GetState());

    public static ByteBool operator |(ByteBool operand1, ByteBool operand2) => new ByteBool(operand1.GetState() | operand2.GetState());

    public static ByteBool operator !(ByteBool operand) => new ByteBool(!operand.Value);

    public static bool operator ==(ByteBool left, ByteBool right)
    => left.Equals(right);

    public static bool operator !=(ByteBool left, ByteBool right)
        => !left.Equals(right);

    public bool Equals(ByteBool other)
        => byteValue == other.byteValue;

    public override bool Equals(object obj)
        => obj is ByteBool other && Equals(other);

    public override int GetHashCode()
        => byteValue.GetHashCode();

    public override string ToString()
    {
        if (state.HasFlag(BoolState.True))
            return "True";
        if (state.HasFlag(BoolState.False))
            return "False";
        if (state.HasFlag(BoolState.Off))
            return "Off";

        return "None";
    }
}

public static class ByteBoolExtensions
{
    public static void WriteByteBool(this Writer writer, ByteBool value)
    {
        writer.Write(value.byteValue);
    }

    public static ByteBool ReadByteBool(this Reader reader)
    {
        return new ByteBool(reader.Read<byte>());
    }

    /// <summary>
    /// Write two ByteBools into a single byte (2 bits each).
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    public static void WriteByteBool2(this Writer writer, ByteBool value1, ByteBool value2)
    {
        byte combined = (byte)((value1.byteValue & 0b11) | ((value2.byteValue & 0b11) << 2));
        writer.Write(combined);
    }

    /// <summary>
    /// Read two ByteBools from a single byte (2 bits each).
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static void ReadByteBool2(this Reader reader, out ByteBool value1, out ByteBool value2)
    {
        byte combined = reader.Read<byte>();
        value1 = new ByteBool((byte)(combined & 0b11));
        value2 = new ByteBool((byte)((combined >> 2) & 0b11));
    }

    /// <summary>
    /// Write three ByteBools into a single byte (2 bits each).
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <param name="value3"></param>
    public static void WriteByteBool3(this Writer writer, ByteBool value1, ByteBool value2, ByteBool value3)
    {
        byte combined = (byte)((value1.byteValue & 0b11) | ((value2.byteValue & 0b11) << 2) | ((value3.byteValue & 0b11) << 4));
        writer.Write(combined);
    }


    /// <summary>
    /// Read three ByteBools from a single byte (2 bits each).
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static void ReadByteBool3(this Reader reader, out ByteBool value1, out ByteBool value2, out ByteBool value3)
    {
        byte combined = reader.Read<byte>();
        value1 = new ByteBool((byte)(combined & 0b11));
        value2 = new ByteBool((byte)((combined >> 2) & 0b11));
        value3 = new ByteBool((byte)((combined >> 4) & 0b11));
    }

    /// <summary>
    /// Write four ByteBools into a single byte (2 bits each).
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <param name="value3"></param>
    /// <param name="value4"></param>
    public static void WriteByteBool4(this Writer writer, ByteBool value1, ByteBool value2, ByteBool value3, ByteBool value4)
    {
        byte combined = (byte)((value1.byteValue & 0b11) | ((value2.byteValue & 0b11) << 2) | ((value3.byteValue & 0b11) << 4) | ((value4.byteValue & 0b11) << 6));
        writer.Write(combined);
    }


    /// <summary>
    /// Read four ByteBools from a single byte (2 bits each).
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static void ReadByteBool4(this Reader reader, out ByteBool value1, out ByteBool value2, out ByteBool value3, out ByteBool value4)
    {
        byte combined = reader.Read<byte>();
        value1 = new ByteBool((byte)(combined & 0b11));
        value2 = new ByteBool((byte)((combined >> 2) & 0b11));
        value3 = new ByteBool((byte)((combined >> 4) & 0b11));
        value4 = new ByteBool((byte)((combined >> 6) & 0b11));
    }

    [CustomComparer]
    public static bool CompareByteBool(ByteBool a, ByteBool b)
    {
        return a.Equals(b);
    }
}
