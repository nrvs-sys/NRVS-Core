using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event of type `SerializableLayerMask`. Inherits from `AtomEvent&lt;SerializableLayerMask&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/SerializableLayerMask", fileName = "SerializableLayerMaskEvent")]
    public sealed class SerializableLayerMaskEvent : AtomEvent<SerializableLayerMask>
    {
    }
}
