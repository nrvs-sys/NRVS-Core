#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Variable property drawer of type `SerializableLayerMask`. Inherits from `AtomDrawer&lt;SerializableLayerMaskVariable&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableLayerMaskVariable))]
    public class SerializableLayerMaskVariableDrawer : VariableDrawer<SerializableLayerMaskVariable> { }
}
#endif
