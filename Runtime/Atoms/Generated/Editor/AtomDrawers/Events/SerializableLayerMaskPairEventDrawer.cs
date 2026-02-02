#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Event property drawer of type `SerializableLayerMaskPair`. Inherits from `AtomDrawer&lt;SerializableLayerMaskPairEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableLayerMaskPairEvent))]
    public class SerializableLayerMaskPairEventDrawer : AtomDrawer<SerializableLayerMaskPairEvent> { }
}
#endif
