using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScriptableState<T> : ScriptableObject, IState<T>
{
    public UnityEvent<T> OnEnter;
    public UnityEvent<T> OnExecute;
    public UnityEvent<T> OnExit;

    public virtual void Enter(T owner) => OnEnter?.Invoke(owner);
    public virtual void Execute(T owner) => OnExecute?.Invoke(owner);
    public virtual void Exit(T owner) => OnExit?.Invoke(owner);
}
