#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Constant property drawer of type `SerializableLayerMask`. Inherits from `AtomDrawer&lt;SerializableLayerMaskConstant&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableLayerMaskConstant))]
    public class SerializableLayerMaskConstantDrawer : VariableDrawer<SerializableLayerMaskConstant> { }
}
#endif
