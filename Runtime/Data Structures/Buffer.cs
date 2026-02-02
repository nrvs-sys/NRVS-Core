using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buffer<T>
{
    public int Count => buffer.Count;

    List<T> buffer = new List<T>();

    int max = -1;

    public Buffer() { }

    public Buffer(int max)
    {
        this.max = max;
    }

    public void Add(T obj)
    {
        buffer.Add(obj);

        while (max > 0 && buffer.Count > max)
        {
            buffer.RemoveAt(0);
        }
    }

    public T Latest()
    {
        if (buffer.Count > 0)
        {
            return buffer[buffer.Count - 1];
        }
        else
        {
            Debug.LogWarning("History is empty.");
            return default(T);
        }
    }

    public T Get(int index) => buffer[index];

    public List<T> All() => buffer;

    public void Clear() => buffer.Clear();
}
