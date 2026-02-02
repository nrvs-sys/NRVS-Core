using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event of type `SceneReference`. Inherits from `AtomEvent&lt;SceneReference&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/SceneReference", fileName = "SceneReferenceEvent")]
    public sealed class SceneReferenceEvent : AtomEvent<SceneReference>
    {
    }
}
