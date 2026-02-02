using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Implements a Gaussian Distributed Number Generator as described here: https://www.alanzucconi.com/2015/09/09/understanding-the-gaussian-distribution/
/// </summary>
public class RandomGaussian : PrecomputedValues<float>
{
    uint _next = 0;
    public uint next
    {
        get => _next;
        set => _next = (uint)(!isInitialized ? 0 : value % precomputedRange.Length);
    }

    protected override float InitializeValue() => Random.Range(0f, 1f);

	public float Next()
	{
		int count = precomputedRange.Length;
		float v1, v2, s;
		do
		{
			v1 = 2.0f * precomputedRange[next++ % count] - 1.0f;
			v2 = 2.0f * precomputedRange[next++ % count] - 1.0f;
			s = v1 * v1 + v2 * v2;
		} while (s >= 1.0f || s == 0f);

		s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);

		return v1 * s;
	}

    public float Next(float mean, float standardDeviation)
    {
        return mean + Next() * standardDeviation;
    }

    public float Next(float mean, float standardDeviation, float min, float max)
    {
        float x;
        do
        {
            x = Next(mean, standardDeviation);
        } while (x < min || x > max);
        return x;
    }
}
