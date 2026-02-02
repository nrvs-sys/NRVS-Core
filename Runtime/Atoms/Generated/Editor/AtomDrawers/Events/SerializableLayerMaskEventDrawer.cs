#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Event property drawer of type `SerializableLayerMask`. Inherits from `AtomDrawer&lt;SerializableLayerMaskEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableLayerMaskEvent))]
    public class SerializableLayerMaskEventDrawer : AtomDrawer<SerializableLayerMaskEvent> { }
}
#endif
