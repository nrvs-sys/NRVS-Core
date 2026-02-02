using FishNet.CodeGenerating;
using FishNet.Serializing;
using FishNet.Serializing.Helping;
using FishNet.Utility.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, UseGlobalCustomSerializer]
public struct TransformState : IEquatable<TransformState>
{
    [ExcludeSerializationAttribute]
    public Vector3 position;
    [ExcludeSerializationAttribute]
    public Quaternion rotation;

    [ExcludeSerializationAttribute]
    public AutoPackType rotationPacking;

    public TransformState(Vector3 position)
    {
        this.position = position;
        rotation = Quaternion.identity;

        rotationPacking = AutoPackType.Packed;
    }

    public TransformState(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;

        rotationPacking = AutoPackType.Packed;
    }

    public TransformState(Vector3 position, Quaternion rotation, AutoPackType rotationPacking) 
    {
        this.position = position;
        this.rotation = rotation;
        this.rotationPacking = rotationPacking;
    }

    public bool Equals(TransformState other)
    {
        return position.Equals(other.position)
            && rotation.Equals(other.rotation)
            && rotationPacking == other.rotationPacking;
    }

    public override bool Equals(object obj)
    {
        return obj is TransformState other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = (hash * 23) + position.GetHashCode();
            hash = (hash * 23) + rotation.GetHashCode();
            hash = (hash * 23) + (int)rotationPacking;
            return hash;
        }
    }

    public static bool operator ==(TransformState left, TransformState right) => left.Equals(right);
    public static bool operator !=(TransformState left, TransformState right) => !left.Equals(right);

    public override string ToString() => $"Position: {position}, Rotation: {rotation}";
}

public static class TransformStateExtensions
{
    public static void SetTransform(this TransformState transformState, Transform transform) => transform.SetPositionAndRotation(transformState.position, transformState.rotation);

    public static void SetLocalTransform(this TransformState transformState, Transform transform) => transform.SetLocalPositionAndRotation(transformState.position, transformState.rotation);

    public static void SetFromTransformState(this Transform transform, TransformState transformState) => transform.SetPositionAndRotation(transformState.position, transformState.rotation);

    public static void SetLocalFromTransformState(this Transform transform, TransformState transformState) => transform.SetLocalPositionAndRotation(transformState.position, transformState.rotation);

    public static void SetTransformState(this Transform transform, TransformState transformState)
    {
        transformState.position = transform.position;
        transformState.rotation = transform.rotation;
    }

    public static void SetLocalTransformState(this Transform transform, TransformState transformState)
    {
        transformState.position = transform.localPosition;
        transformState.rotation = transform.localRotation;
    }

    public static void WriteTransformState(this Writer writer, TransformState value)
    {
        writer.Write(value.position);

        writer.Write((byte)value.rotationPacking);
        switch (value.rotationPacking)
        {
            case AutoPackType.Packed:
                writer.WriteQuaternion32(value.rotation);
                break;
            case AutoPackType.PackedLess:
                writer.WriteQuaternion64(value.rotation);
                break;
            case AutoPackType.Unpacked:
                writer.WriteQuaternionUnpacked(value.rotation);
                break;
        }
    }

    public static TransformState ReadTransformState(this Reader reader)
    {
        TransformState transformState = new TransformState();

        transformState.position = reader.Read<Vector3>();

        transformState.rotationPacking = (AutoPackType)reader.Read<byte>();
        switch (transformState.rotationPacking)
        {
            case AutoPackType.Packed:
                transformState.rotation = reader.ReadQuaternion32();
                break;
            case AutoPackType.PackedLess:
                transformState.rotation = reader.ReadQuaternion64();
                break;
            case AutoPackType.Unpacked:
                transformState.rotation = reader.ReadQuaternionUnpacked();
                break;
        }

        return transformState;
    }

    [CustomComparer]
    public static bool CompareTransformState(TransformState a, TransformState b) => a.position == b.position && a.rotation == b.rotation && a.rotationPacking == b.rotationPacking;
}
