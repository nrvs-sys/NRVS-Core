using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Constant of type `SceneReference`. Inherits from `AtomBaseVariable&lt;SceneReference&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-teal")]
    [CreateAssetMenu(menuName = "Unity Atoms/Constants/SceneReference", fileName = "SceneReferenceConstant")]
    public sealed class SceneReferenceConstant : AtomBaseVariable<SceneReference> { }
}
