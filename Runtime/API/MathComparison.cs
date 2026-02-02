using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MathComparisonType
{
	LessThan,
	LessThanOrEqual,
	NotEqual,
	Equal,
	GreaterThanOrEqual,
	GreaterThan
}

public static class MathComparison
{
	public static bool Compare(MathComparisonType comparisonType, int int1, int int2)
	{
		switch (comparisonType)
		{
			case MathComparisonType.LessThan:
				return int1 < int2;

			case MathComparisonType.LessThanOrEqual:
				return int1 <= int2;

			case MathComparisonType.NotEqual:
				return int1 != int2;

			case MathComparisonType.Equal:
				return int1 == int2;

			case MathComparisonType.GreaterThanOrEqual:
				return int1 >= int2;

			case MathComparisonType.GreaterThan:
				return int1 > int2;

			default:
				return false;
		}
	}

	public static bool Compare(MathComparisonType comparisonType, float float1, float float2)
	{
		switch (comparisonType)
		{
			case MathComparisonType.LessThan:
				return float1 < float2;

			case MathComparisonType.LessThanOrEqual:
				return float1 <= float2;

			case MathComparisonType.NotEqual:
				return float1 != float2;

			case MathComparisonType.Equal:
				return float1 == float2;

			case MathComparisonType.GreaterThanOrEqual:
				return float1 >= float2;

			case MathComparisonType.GreaterThan:
				return float1 > float2;

			default:
				return false;
		}
	}
}