using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utility
{
    /// <summary>
    /// Implementation grabbed from: https://wiki.unity3d.com/index.php/SerializableDictionary
    /// </summary>
    public class SerializableDictionary { }

    [Serializable]
    public class SerializableDictionary<TKey, TValue> : SerializableDictionary, ISerializationCallbackReceiver, IDictionary<TKey, TValue>
    {

        [SerializeField]
        private List<SerializableKeyValuePair> list = new List<SerializableKeyValuePair>();

        [Serializable]
        public struct SerializableKeyValuePair
        {
            [HideInInspector]
            public string name;
            public TKey Key;
            public TValue Value;

            public SerializableKeyValuePair(TKey key, TValue value)
            {
                name = key.ToString();
                Key = key;
                Value = value;
            }

            public void SetValue(TValue value)
            {
                Value = value;
            }

            public override string ToString() => $"Key: {Key}, Value: {Value}";
        }


        private Dictionary<TKey, uint> KeyPositions => _keyPositions.Value;
        private Lazy<Dictionary<TKey, uint>> _keyPositions;

        public SerializableDictionary()
        {
            _keyPositions = new Lazy<Dictionary<TKey, uint>>(MakeKeyPositions);
        }

        private Dictionary<TKey, uint> MakeKeyPositions()
        {
            var numEntries = list.Count;
            var result = new Dictionary<TKey, uint>(numEntries);
            for (int i = 0; i < numEntries; i++)
                if (!result.ContainsKey(list[i].Key)) result[list[i].Key] = (uint)i;
            return result;
        }

        public void OnBeforeSerialize() 
        {
            for (var i = 0; i < list.Count; i++)
            {
                var kvp = list[i];
                kvp.name = (kvp.Key is StringReference stringRef) ? stringRef.Value : kvp.Key.ToString();
                list[i] = kvp;
            }
        }
        public void OnAfterDeserialize()
        {
            // After deserialization, the key positions might be changed
            _keyPositions = new Lazy<Dictionary<TKey, uint>>(MakeKeyPositions);
        }

        #region IDictionary<TKey, TValue>
        public TValue this[TKey key]
        {
            get => list[(int)KeyPositions[key]].Value;
            set
            {
                if (KeyPositions.TryGetValue(key, out uint index))
                    list[(int)index] = new SerializableKeyValuePair(key, value);
                else
                {
                    KeyPositions[key] = (uint)list.Count;
                    list.Add(new SerializableKeyValuePair(key, value));
                }
            }
        }

        public ICollection<TKey> Keys => list.Select(tuple => tuple.Key).ToArray();
        public ICollection<TValue> Values => list.Select(tuple => tuple.Value).ToArray();

        public void Add(TKey key, TValue value)
        {
            if (KeyPositions.ContainsKey(key))
                throw new ArgumentException("An element with the same key already exists in the dictionary.");
            else
            {
                KeyPositions[key] = (uint)list.Count;
                list.Add(new SerializableKeyValuePair(key, value));
            }
        }

        public bool ContainsKey(TKey key) => KeyPositions.ContainsKey(key);


        public bool Remove(TKey key)
        {
            if (KeyPositions.TryGetValue(key, out uint index))
            {
                var kp = KeyPositions;
                kp.Remove(key);

                // Remove the entry first so list.Count reflects the new size.
                list.RemoveAt((int)index);

                // Update positions for items that shifted left.
                for (int i = (int)index; i < list.Count; i++)
                {
                    kp[list[i].Key] = (uint)i;
                }

                return true;
            }
            else
                return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (KeyPositions.TryGetValue(key, out uint index))
            {
                value = list[(int)index].Value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        #endregion


        #region ICollection <KeyValuePair<TKey, TValue>>
        public int Count => list.Count;
        public bool IsReadOnly => false;

        public void Add(KeyValuePair<TKey, TValue> kvp) => Add(kvp.Key, kvp.Value);

        public void Clear() => list.Clear();
        public bool Contains(KeyValuePair<TKey, TValue> kvp) => KeyPositions.ContainsKey(kvp.Key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            var numKeys = list.Count;
            if (array.Length - arrayIndex < numKeys)
                throw new ArgumentException("arrayIndex");
            for (int i = 0; i < numKeys; i++, arrayIndex++)
            {
                var entry = list[i];
                array[arrayIndex] = new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> kvp) => Remove(kvp.Key);
        #endregion


        #region IEnumerable <KeyValuePair<TKey, TValue>>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return list.Select(ToKeyValuePair).GetEnumerator();

            KeyValuePair<TKey, TValue> ToKeyValuePair(SerializableKeyValuePair skvp)
            {
                return new KeyValuePair<TKey, TValue>(skvp.Key, skvp.Value);
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

    }
}