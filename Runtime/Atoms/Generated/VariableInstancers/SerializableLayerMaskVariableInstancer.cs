using UnityEngine;
using UnityAtoms.BaseAtoms;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Variable Instancer of type `SerializableLayerMask`. Inherits from `AtomVariableInstancer&lt;SerializableLayerMaskVariable, SerializableLayerMaskPair, SerializableLayerMask, SerializableLayerMaskEvent, SerializableLayerMaskPairEvent, SerializableLayerMaskSerializableLayerMaskFunction&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-hotpink")]
    [AddComponentMenu("Unity Atoms/Variable Instancers/SerializableLayerMask Variable Instancer")]
    public class SerializableLayerMaskVariableInstancer : AtomVariableInstancer<
        SerializableLayerMaskVariable,
        SerializableLayerMaskPair,
        SerializableLayerMask,
        SerializableLayerMaskEvent,
        SerializableLayerMaskPairEvent,
        SerializableLayerMaskSerializableLayerMaskFunction>
    { }
}
