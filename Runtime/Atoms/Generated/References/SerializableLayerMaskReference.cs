using System;
using UnityAtoms.BaseAtoms;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Reference of type `SerializableLayerMask`. Inherits from `AtomReference&lt;SerializableLayerMask, SerializableLayerMaskPair, SerializableLayerMaskConstant, SerializableLayerMaskVariable, SerializableLayerMaskEvent, SerializableLayerMaskPairEvent, SerializableLayerMaskSerializableLayerMaskFunction, SerializableLayerMaskVariableInstancer, AtomCollection, AtomList&gt;`.
    /// </summary>
    [Serializable]
    public sealed class SerializableLayerMaskReference : AtomReference<
        SerializableLayerMask,
        SerializableLayerMaskPair,
        SerializableLayerMaskConstant,
        SerializableLayerMaskVariable,
        SerializableLayerMaskEvent,
        SerializableLayerMaskPairEvent,
        SerializableLayerMaskSerializableLayerMaskFunction,
        SerializableLayerMaskVariableInstancer>, IEquatable<SerializableLayerMaskReference>
    {
        public SerializableLayerMaskReference() : base() { }
        public SerializableLayerMaskReference(SerializableLayerMask value) : base(value) { }
        public bool Equals(SerializableLayerMaskReference other) { return base.Equals(other); }
        protected override bool ValueEquals(SerializableLayerMask other)
        {
            throw new NotImplementedException();
        }
    }
}
