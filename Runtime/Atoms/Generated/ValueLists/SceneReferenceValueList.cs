using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Value List of type `SceneReference`. Inherits from `AtomValueList&lt;SceneReference, SceneReferenceEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-piglet")]
    [CreateAssetMenu(menuName = "Unity Atoms/Value Lists/SceneReference", fileName = "SceneReferenceValueList")]
    public sealed class SceneReferenceValueList : AtomValueList<SceneReference, SceneReferenceEvent> { }
}
