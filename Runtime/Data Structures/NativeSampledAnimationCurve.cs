using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System;
using GameKit.Dependencies.Utilities;

public interface ISampledAnimationCurve : IDisposable
{
    int Samples { get; }
    bool IsInitialized { get; }
    float Evaluate(float time);


}

public struct SampledAnimationCurve : ISampledAnimationCurve
{
    List<float> sampledFloat;

    public bool IsInitialized { get; private set; }

    public int Samples { get; private set; }

    /// <param name="samples">Must be 2 or higher</param>
    public SampledAnimationCurve(AnimationCurve ac, int samples)
    {
        sampledFloat = CollectionCaches<float>.RetrieveList();
        
        float timeFrom = ac.keys[0].time;
        float timeTo = ac.keys[ac.keys.Length - 1].time;
        float timeStep = (timeTo - timeFrom) / (samples - 1);

        for (int i = 0; i < samples; i++)
        {
            var s = ac.Evaluate(timeFrom + (i * timeStep));

            if (i < sampledFloat.Count)
                sampledFloat[i] = s;
            else
                sampledFloat.Add(s);
        }

        Samples = samples;

        IsInitialized = true;
    }

    public float Evaluate(float time)
    {
        float clamp01 = time < 0 ? 0 : (time > 1 ? 1 : time);
        float floatIndex = (clamp01 * Samples);
        int floorIndex = Mathf.FloorToInt(floatIndex);
        if (floorIndex == Samples)
        {
            return sampledFloat[Samples - 1];
        }
        float lowerValue = sampledFloat[floorIndex];
        float higherValue = sampledFloat[floorIndex + 1];
        return Mathf.Lerp(lowerValue, higherValue, clamp01);
    }

    public void Dispose()
    {
        CollectionCaches<float>.Store(sampledFloat);
        sampledFloat = null;

        IsInitialized = false;
    }
}

public struct NativeSampledAnimationCurve : ISampledAnimationCurve
{
    NativeArray<float> sampledFloat;

    public bool IsInitialized { get; private set; }

    public int Samples { get; private set; }

    /// <param name="samples">Must be 2 or higher</param>
    public NativeSampledAnimationCurve(AnimationCurve ac, int samples)
    {
        sampledFloat = new NativeArray<float>(samples, Allocator.Persistent);
        float timeFrom = ac.keys[0].time;
        float timeTo = ac.keys[ac.keys.Length - 1].time;
        float timeStep = (timeTo - timeFrom) / (samples - 1);

        for (int i = 0; i < samples; i++)
        {
            sampledFloat[i] = ac.Evaluate(timeFrom + (i * timeStep));
        }

        Samples = samples;

        IsInitialized = true;
    }

    public float Evaluate(float time)
    {
        float clamp01 = time < 0 ? 0 : (time > 1 ? 1 : time);
        float floatIndex = (clamp01 * Samples);
        int floorIndex = (int)math.floor(floatIndex);
        if (floorIndex == Samples)
        {
            return sampledFloat[Samples - 1];
        }
        float lowerValue = sampledFloat[floorIndex];
        float higherValue = sampledFloat[floorIndex + 1];
        return math.lerp(lowerValue, higherValue, clamp01);
    }

    public void Dispose()
    {
        sampledFloat.Dispose();

        IsInitialized = false;
    }
}