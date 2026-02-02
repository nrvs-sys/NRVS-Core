using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Variable Inspector of type `SerializableLayerMask`. Inherits from `AtomVariableEditor`
    /// </summary>
    [CustomEditor(typeof(SerializableLayerMaskVariable))]
    public sealed class SerializableLayerMaskVariableEditor : AtomVariableEditor<SerializableLayerMask, SerializableLayerMaskPair> { }
}
