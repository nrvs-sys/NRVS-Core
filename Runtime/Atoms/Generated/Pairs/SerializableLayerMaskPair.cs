using System;
using UnityEngine;
namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// IPair of type `&lt;SerializableLayerMask&gt;`. Inherits from `IPair&lt;SerializableLayerMask&gt;`.
    /// </summary>
    [Serializable]
    public struct SerializableLayerMaskPair : IPair<SerializableLayerMask>
    {
        public SerializableLayerMask Item1 { get => _item1; set => _item1 = value; }
        public SerializableLayerMask Item2 { get => _item2; set => _item2 = value; }

        [SerializeField]
        private SerializableLayerMask _item1;
        [SerializeField]
        private SerializableLayerMask _item2;

        public void Deconstruct(out SerializableLayerMask item1, out SerializableLayerMask item2) { item1 = Item1; item2 = Item2; }
    }
}