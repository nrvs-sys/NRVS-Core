#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Value List property drawer of type `SerializableLayerMask`. Inherits from `AtomDrawer&lt;SerializableLayerMaskValueList&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableLayerMaskValueList))]
    public class SerializableLayerMaskValueListDrawer : AtomDrawer<SerializableLayerMaskValueList> { }
}
#endif
