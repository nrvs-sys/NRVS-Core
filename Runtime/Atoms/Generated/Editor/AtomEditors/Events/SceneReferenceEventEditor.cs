#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine.UIElements;
using UnityAtoms.Editor;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Event property drawer of type `SceneReference`. Inherits from `AtomEventEditor&lt;SceneReference, SceneReferenceEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomEditor(typeof(SceneReferenceEvent))]
    public sealed class SceneReferenceEventEditor : AtomEventEditor<SceneReference, SceneReferenceEvent> { }
}
#endif
