using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event of type `SerializableLayerMaskPair`. Inherits from `AtomEvent&lt;SerializableLayerMaskPair&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/SerializableLayerMaskPair", fileName = "SerializableLayerMaskPairEvent")]
    public sealed class SerializableLayerMaskPairEvent : AtomEvent<SerializableLayerMaskPair>
    {
    }
}
