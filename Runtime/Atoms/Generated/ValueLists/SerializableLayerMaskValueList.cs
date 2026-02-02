using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Value List of type `SerializableLayerMask`. Inherits from `AtomValueList&lt;SerializableLayerMask, SerializableLayerMaskEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-piglet")]
    [CreateAssetMenu(menuName = "Unity Atoms/Value Lists/SerializableLayerMask", fileName = "SerializableLayerMaskValueList")]
    public sealed class SerializableLayerMaskValueList : AtomValueList<SerializableLayerMask, SerializableLayerMaskEvent> { }
}
