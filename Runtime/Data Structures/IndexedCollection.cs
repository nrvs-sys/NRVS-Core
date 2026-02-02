using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexedCollection<T> : IEnumerable<T>
{
    readonly T[] items;
    readonly Stack<int> freeIndices;
    int nextIndex;
    readonly int capacity;

    /// <summary>
    /// Creates a new collection with the given capacity.
    /// </summary>
    /// <param name="capacity">The maximum number of items this collection can hold.</param>
    public IndexedCollection(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive.");

        this.capacity = capacity;
        items = new T[capacity];
        freeIndices = new Stack<int>(capacity);
        nextIndex = 0;
    }

    /// <summary>
    /// The maximum number of items that can be stored.
    /// </summary>
    public int MaxCapacity => capacity;

    /// <summary>
    /// The total number of indices that have been registered to. This includes freed indices. Useful for iteration.
    /// </summary>
    public int Count => nextIndex;

    /// <summary>
    /// The amount of items that are currently registered in the collection.
    /// </summary>
    public int RegisteredCount => nextIndex - freeIndices.Count;

    /// <summary>
    /// Indexer to access the item at a given index.
    /// </summary>
    public T this[int index] => items[index];

    /// <summary>
    /// Registers a new item into the collection.
    /// Returns the assigned index.
    /// The caller is responsible for storing the returned index if needed.
    /// </summary>
    /// <param name="item">The item to register.</param>
    /// <returns>The index assigned to this item.</returns>
    public int Register(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        int index;
        if (freeIndices.Count > 0)
        {
            // Reuse a freed index.
            index = freeIndices.Pop();
        }
        else
        {
            // No freed indices available, so use the next available index.
            if (nextIndex >= capacity)
            {
                Debug.LogError($"IndexedCollection is at max capacity of {capacity}.");
                return -1;
            }
            index = nextIndex++;
        }

        items[index] = item;
        return index;
    }

    /// <summary>
    /// Unregisters the item at the specified index.
    /// The index is then available for reuse.
    /// </summary>
    /// <param name="index">The index of the item to unregister.</param>
    public void UnregisterAt(int index)
    {
        if (index < 0 || index >= capacity)
            throw new ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {capacity - 1}.");

        items[index] = default;
        freeIndices.Push(index);
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < nextIndex; i++)
        {
            if (!freeIndices.Contains(i))
                yield return items[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
