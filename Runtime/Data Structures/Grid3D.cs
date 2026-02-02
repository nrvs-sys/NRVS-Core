using System.Collections;
using System.Collections.Generic;
using UnityEngine;



	public enum GridCellDirection
	{
		XPos,
		XNeg,
		YPos,
		YNeg,
		ZPos,
		ZNeg,
	}

	public class Grid3D<T>
	{
		public T[] data;

		/// <summary>
		/// The Grid Cell count on the X axis.
		/// </summary>
		public int CellsX
		{
			get { return dimensions.x; }
		}

		/// <summary>
		/// The Grid Cell count on the Y axis.
		/// </summary>
		public int CellsY
		{
			get { return dimensions.y; }
		}

		/// <summary>
		/// The Grid Cell count on the Z axis.
		/// </summary>
		public int CellsZ
		{
			get { return dimensions.z; }
		}

		public int Count
		{
			get { return data.Length; }
		}

		private Vector3Int dimensions;

		public Grid3D(Vector3Int dimensions, T initialValue)
		{
			Resize(dimensions, initialValue);
		}

		public void Resize(Vector3Int dimensions, T initialValue)
		{
			data = new T[dimensions.x * dimensions.y * dimensions.z];
			this.dimensions = dimensions;
			Fill(initialValue);
		}

		public void Fill(T value)
		{
			for (int i = 0; i < data.Length; ++i)
				data[i] = value;
		}

		/// <summary>
		/// Gets the Cell Coords (ie the Cell's relative Grid position) from the provided `index`.
		/// </summary>
		public Vector3Int GetIndexCoords(int index)
		{
			var x = index % CellsX;
			var z = (index - x) / CellsX;

			return new Vector3Int(x, Mathf.FloorToInt(z / CellsZ), z % CellsZ);
		}

		/// <summary>
		/// Gets the Cell Index (ie the Cell's index in the Grid's `data` array) from the provided `coords`.
		/// </summary>
		public int GetCoordsIndex(Vector3Int coords)
		{
			return coords.x + CellsX * (coords.z + CellsZ * coords.y);
		}

		public Bounds GetBounds(Vector3 origin, int cellSize)
		{
			return new Bounds(origin, dimensions * new Vector3Int(cellSize, cellSize, cellSize));
		}

		/// <summary>
		/// Gets the Neighboring Cell Index based on the direction parameter. 
		/// 
		/// Returns -1 if the requested index is out of bounds.
		/// </summary>
		public int GetNeighborIndex(int index, GridCellDirection direction)
		{
			var i = -1;

			switch (direction)
			{
				case GridCellDirection.XPos:
					if ((index + 1) % CellsX != 0)
						i = index + 1;
					break;
				case GridCellDirection.XNeg:
					if ((index - 1) % CellsX != CellsX - 1)
						i = index - 1;
					break;
				case GridCellDirection.YPos:
					i = index + CellsX * CellsZ;
					break;
				case GridCellDirection.YNeg:
					i = index - CellsX * CellsZ;
					break;
				case GridCellDirection.ZPos:
					if (Mathf.FloorToInt(((index + CellsX) / CellsX) % CellsX) != 0)
						i = index + CellsX;
					break;
				case GridCellDirection.ZNeg:
					if (Mathf.FloorToInt(((index - CellsX) / CellsX) % CellsX) != CellsZ - 1)
						i = index - CellsX;
					break;
			}

			return i > -1 && i < data.Length ? i : -1;
		}

		public Grid3D<T> Sub(Vector3Int startCoords, Vector3Int dimensions, T initialValue)
		{
			var sub = new Grid3D<T>(dimensions, initialValue);

			for (int x = startCoords.x; x < CellsX && x < startCoords.x + dimensions.x; x++)
			{
				for (int y = startCoords.y; y < CellsY && y < startCoords.y + dimensions.y; y++)
				{
					for (int z = startCoords.z; x < CellsZ && z < startCoords.z + dimensions.z; z++)
					{
						sub[x - startCoords.x, y - startCoords.y, z - startCoords.z] = this[x, y, z];
					}
				}
			}

			return sub;
		}

		public T this[int i]
		{
			get { return data[i]; }
			set { data[i] = value; }
		}

		public T this[int x, int y, int z]
		{
			get { return data[x + CellsX * (z + CellsZ * y)]; }
			set { data[x + CellsX * (z + CellsZ * y)] = value; }
		}

		public T this[Vector3Int dimensions]
		{
			get { return data[dimensions.x + CellsX * (dimensions.z + CellsZ * dimensions.y)]; }
			set { data[dimensions.x + CellsX * (dimensions.z + CellsZ * dimensions.y)] = value; }
		}
	}
