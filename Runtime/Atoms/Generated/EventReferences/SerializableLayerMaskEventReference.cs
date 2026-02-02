using System;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event Reference of type `SerializableLayerMask`. Inherits from `AtomEventReference&lt;SerializableLayerMask, SerializableLayerMaskVariable, SerializableLayerMaskEvent, SerializableLayerMaskVariableInstancer, SerializableLayerMaskEventInstancer&gt;`.
    /// </summary>
    [Serializable]
    public sealed class SerializableLayerMaskEventReference : AtomEventReference<
        SerializableLayerMask,
        SerializableLayerMaskVariable,
        SerializableLayerMaskEvent,
        SerializableLayerMaskVariableInstancer,
        SerializableLayerMaskEventInstancer>, IGetEvent 
    { }
}
