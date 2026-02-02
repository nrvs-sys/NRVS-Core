using UnityEngine;
using System;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Variable of type `SerializableLayerMask`. Inherits from `AtomVariable&lt;SerializableLayerMask, SerializableLayerMaskPair, SerializableLayerMaskEvent, SerializableLayerMaskPairEvent, SerializableLayerMaskSerializableLayerMaskFunction&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-lush")]
    [CreateAssetMenu(menuName = "Unity Atoms/Variables/SerializableLayerMask", fileName = "SerializableLayerMaskVariable")]
    public sealed class SerializableLayerMaskVariable : AtomVariable<SerializableLayerMask, SerializableLayerMaskPair, SerializableLayerMaskEvent, SerializableLayerMaskPairEvent, SerializableLayerMaskSerializableLayerMaskFunction>
    {
        protected override bool ValueEquals(SerializableLayerMask other)
        {
            throw new NotImplementedException();
        }
    }
}
