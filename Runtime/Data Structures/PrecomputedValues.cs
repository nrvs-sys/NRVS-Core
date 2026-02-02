using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PrecomputedValues<T>
{
    protected T[] precomputedRange;

    public bool isInitialized => precomputedRange != null;

    public PrecomputedValues() { }

    public PrecomputedValues(int count, int seed = 0)
    {
        Initialize(count, seed);
    }

    public PrecomputedValues(int count, Random.State state)
    {
        Initialize(count, state);
    }

    public void Initialize(int count, int seed = 0)
    {
        precomputedRange = new T[count];

        var currentState = Random.state;

        Random.InitState(seed);

        Initialize(count);

        Random.state = currentState;
    }

    public void Initialize(int count, Random.State state)
    {
        precomputedRange = new T[count];

        var currentState = Random.state;

        Random.state = state;

        Initialize(count);

        Random.state = currentState;
    }

    void Initialize(int count)
    {
        precomputedRange = new T[count];

        for (int i = 0; i < count; i++)
            precomputedRange[i] = InitializeValue();
    }

    protected abstract T InitializeValue();
}
