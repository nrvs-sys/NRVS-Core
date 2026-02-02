using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpawnFormationUtilities
{
	public static List<Vector3> GetUniformPointsOnSphere(int n)
	{
		List<Vector3> upts = new List<Vector3>();
		float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
		float off = 2.0f / n;
		float x = 0;
		float y = 0;
		float z = 0;
		float r = 0;
		float phi = 0;

		for (var k = 0; k < n; k++)
		{
			y = k * off - 1 + (off / 2);
			r = Mathf.Sqrt(1 - y * y);
			phi = k * inc;
			x = Mathf.Cos(phi) * r;
			z = Mathf.Sin(phi) * r;

			upts.Add(new Vector3(x, y, z));
		}

		return upts;
	}

	public static List<Vector3> GetRandomPointsInSphere(int n)
	{
		List<Vector3> points = new List<Vector3>();

		for (int i = 0; i < n; i++)
			points.Add(UnityEngine.Random.insideUnitSphere);

		return points;
	}

	public static List<Vector3> GetRandomPointsOnSphere(int n)
	{
		List<Vector3> points = new List<Vector3>();

		for (int i = 0; i < n; i++)
			points.Add(UnityEngine.Random.onUnitSphere);

		return points;
	}

	/// <summary>
	/// Get random points in a sphere with a minimum distance between points return to prevent overlap. Unlike the base method, this method returns scaled positions based on sphere size.
	/// </summary>
	/// <param name="n">The number of points to get</param>
	/// <param name="sphereSize">The scale of the sphere used to get points</param>
	/// <param name="minimumDistance">The minimum distance between points returned, used to prevent overlap</param>
	/// <returns></returns>
	public static List<Vector3> GetRandomPointsInSphere(int n, float sphereSize, float minimumDistance)
	{
		List<Vector3> points = new List<Vector3>();


		// Create a grid of points
		float sphereSizeRadius = sphereSize / 2f;
		List<Vector3> gridPoints = new List<Vector3>();
		int gridSize = Mathf.FloorToInt(sphereSize / minimumDistance);

		for (int i = 0; i < gridSize; i++)
		{
			List<Vector2> gridRows = GetUniformGrid(gridSize * gridSize);

			// Position and scale the points
			foreach (Vector2 row in gridRows)
				gridPoints.Add(new Vector3(row.x, row.y, (i / (float)gridSize) - 0.5f) * sphereSize);
		}

		// Remove ones outside sphere
		gridPoints.RemoveAll(p => p.magnitude > sphereSizeRadius);

		// Get random points from the grid, reusing points once they have all been used
		List<Vector3> availablePoints = new List<Vector3>(gridPoints);

		for (int i = 0; i < n; i++)
		{
			Vector3 point = availablePoints[UnityEngine.Random.Range(0, availablePoints.Count)];

			points.Add(point);
			availablePoints.Remove(point);

			// If there are no points left, reset the list
			if (availablePoints.Count == 0)
				availablePoints = new List<Vector3>(gridPoints);
		}


		return points;
	}

	public static List<Vector3> GetRandonPointsOnCircle(int n)
	{
		List<Vector3> points = new List<Vector3>();

		for (int i = 0; i < n; i++)
		{
			// Generate a random angle in radians
			float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
			float x = Mathf.Cos(angle);
			float y = Mathf.Sin(angle);

			// Create the point on the circle's perimeter (assuming unit circle in the X-Y plane)
			points.Add(new Vector3(x, y, 0));
		}

		return points;
	}

	public static List<Vector3> GetUniformPointsOnCircle(int n)
	{
		List<Vector3> points = new List<Vector3>();

		for (int i = 0; i < n; i++)
		{
			Vector2 point = new Vector2(
				Mathf.Cos(((float)i / n) * 360f * Mathf.Deg2Rad),
				Mathf.Sin(((float)i / n) * 360f * Mathf.Deg2Rad)
				);

			points.Add(point);
		}

		return points;
	}

	/// <summary>
	/// Gets the smallest uniform grid based on the number of requested points
	/// </summary>
	/// <param name="numberOfPoints">The number of required points available in the grid</param>
	/// <returns></returns>
	public static List<Vector2> GetUniformGrid(int numberOfPoints)
	{
		List<Vector2> points = new List<Vector2>();

		// Find a grid size to fit the number of points
		int gridLength = 1;

		while (true)
		{
			int cellCount = gridLength * gridLength;

			if (cellCount >= numberOfPoints)
				break;
			else
				gridLength++;
		}

		// Create a grid
		float cellSize = 1 / (float)gridLength;

		Vector2 gridOffset = new Vector2(-0.5f, -0.5f);

		for (int r = 0; r < gridLength; r++)
		{
			float yPosition = (cellSize / 2) + (r * cellSize);

			for (int c = 0; c < gridLength; c++)
			{
				float xPosition = (cellSize / 2) + (c * cellSize);

				points.Add(new Vector2(xPosition, yPosition) + gridOffset);
			}
		}

		return points;
	}

	/// <summary>
	/// Poisson disc sampling method
	/// </summary>
	/// <param name="radius">Minimum distance between points</param>
	/// <param name="sampleRegionSize"></param>
	/// <param name="numSamplesBeforeRejection"></param>
	/// <returns></returns>
	public static List<Vector2> GetRandomPointsOnPlane(float radius, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 30)
	{
		float cellSize = radius / Mathf.Sqrt(2);

		int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];
		List<Vector2> points = new List<Vector2>();
		List<Vector2> spawnPoints = new List<Vector2>();

		spawnPoints.Add(sampleRegionSize / 2);
		while (spawnPoints.Count > 0)
		{
			int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
			Vector2 spawnCentre = spawnPoints[spawnIndex];
			bool candidateAccepted = false;

			for (int i = 0; i < numSamplesBeforeRejection; i++)
			{
				float angle = UnityEngine.Random.value * Mathf.PI * 2;
				Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
				Vector2 candidate = spawnCentre + dir * UnityEngine.Random.Range(radius, 2 * radius);
				if (IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid))
				{
					points.Add(candidate);
					spawnPoints.Add(candidate);
					grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
					candidateAccepted = true;
					break;
				}
			}
			if (!candidateAccepted)
			{
				spawnPoints.RemoveAt(spawnIndex);
			}

		}

		return points;
	}

	public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
	{
		return point.RotateAroundPivot(pivot, angles);
	}

	private static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
	{
		if (candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y)
		{
			int cellX = (int)(candidate.x / cellSize);
			int cellY = (int)(candidate.y / cellSize);
			int searchStartX = Mathf.Max(0, cellX - 2);
			int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
			int searchStartY = Mathf.Max(0, cellY - 2);
			int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

			for (int x = searchStartX; x <= searchEndX; x++)
			{
				for (int y = searchStartY; y <= searchEndY; y++)
				{
					int pointIndex = grid[x, y] - 1;
					if (pointIndex != -1)
					{
						float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
						if (sqrDst < radius * radius)
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		return false;
	}
}
