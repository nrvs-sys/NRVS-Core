#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Constant property drawer of type `SceneReference`. Inherits from `AtomDrawer&lt;SceneReferenceConstant&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneReferenceConstant))]
    public class SceneReferenceConstantDrawer : VariableDrawer<SceneReferenceConstant> { }
}
#endif
