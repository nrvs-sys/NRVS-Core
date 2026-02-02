#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Event property drawer of type `SceneReference`. Inherits from `AtomDrawer&lt;SceneReferenceEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneReferenceEvent))]
    public class SceneReferenceEventDrawer : AtomDrawer<SceneReferenceEvent> { }
}
#endif
