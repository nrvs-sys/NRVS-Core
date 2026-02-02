using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node<T>
{
    public T data;
    public List<Node<T>> children = new List<Node<T>>();
}
