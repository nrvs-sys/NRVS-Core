#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine.UIElements;
using UnityAtoms.Editor;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Event property drawer of type `SerializableLayerMask`. Inherits from `AtomEventEditor&lt;SerializableLayerMask, SerializableLayerMaskEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomEditor(typeof(SerializableLayerMaskEvent))]
    public sealed class SerializableLayerMaskEventEditor : AtomEventEditor<SerializableLayerMask, SerializableLayerMaskEvent> { }
}
#endif
