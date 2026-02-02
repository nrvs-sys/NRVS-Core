#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine.UIElements;
using UnityAtoms.Editor;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Event property drawer of type `SerializableLayerMaskPair`. Inherits from `AtomEventEditor&lt;SerializableLayerMaskPair, SerializableLayerMaskPairEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomEditor(typeof(SerializableLayerMaskPairEvent))]
    public sealed class SerializableLayerMaskPairEventEditor : AtomEventEditor<SerializableLayerMaskPair, SerializableLayerMaskPairEvent> { }
}
#endif
