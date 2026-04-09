using Assets.Components.InGame.Chunks.Interactables;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Components.InGame.Chunks
{
	/// <summary>
	/// This represents the composition of a chunk (each column represents a line, each row represents a position on the line, which is filled with obstacles)
	/// If _matrice[x,y] = 0 => empty position on the X position of the Y lane
	/// If _matrice[x,y] = 1 => obstacle on the X position of the Y lane
	/// If _matrice[x,y] = 2 => collectible on the X position of the Y lane
	/// If _matrice[x,y] = 3 => super collectible on the X position of the Y lane
	/// </summary>
	[System.Serializable]
	public class ChunkMatrix
	{
		[System.Serializable]
		private class Row
		{
			public int[] cols;

			public Row(int size) => cols = new int[size];
		}

		[SerializeField] private Row[] _rows;

		public int NbRows { get; set; }

		public int NbCols { get; set; }

		public void Init(int nbRows, int nbCols)
		{
			NbRows = nbRows;
			NbCols = nbCols;

			_rows = new Row[nbRows];
			for (int i = 0; i < nbRows; i++)
				_rows[i] = new Row(nbCols);
		}

		public int Get(int row, int col)
			=> _rows[row - 1].cols[col];

		public void Set(int row, int col, int value)
			=> _rows[row - 1].cols[col] = value;

		public bool IsFree(int row, int col)
			=> _rows[row - 1].cols[col] == 0;

		/// <summary>
		/// Check if we can place a <see cref="AInteractable"/> at a position, while respecting its constraints
		/// </summary>
		/// <param name="row">The position in the lane</param>
		/// <param name="col">The lane</param>
		/// <param name="minDistance">The distance constraint</param>
		/// <returns></returns>
		public bool IsFreeInRangeOfType(int row, int col, int minRows, int type)
		{
			int from = Mathf.Max(0, row - minRows);
			int to = Mathf.Min(NbRows - 1, row + minRows);

			for (int r = from; r <= to; r++)
				if (_rows[r].cols[col] == type)
					return false;

			return true;
		}

		public void Clear()
		{
			for (int r = 0; r < NbRows; r++)
				for (int c = 0; c < NbCols; c++)
					_rows[r].cols[c] = 0;
		}

		/// <summary>
		/// Construct a ditance map using BFS algorithm
		/// </summary>
		/// <remarks> Each collectible or component count as a starting point, as the goal is to identified the most isolated coordinates to spawn a <see cref="Interactables.Collectibles.Component"/></remarks>
		private int[,] BuildCollectibleDistanceMap(int minRow)
		{
			int[,] dist = new int[NbRows, NbCols];
			Queue<(int row, int col)> queue = new();

			for (int row = 0; row < NbRows; row++)
			{
				for (int col = 0; col < NbCols; col++)
				{
					// If we have an obstacle, we insert a infinite distance to ensure that the BFS algorithm will not return theese coords
					// We also insert an infinite distance for every row where we don't want to spawn anything
					if (_rows[row].cols[col] == 1 || row < minRow)
					{
						dist[row, col] = -2;
					}
					else if (_rows[row].cols[col] == 2 || _rows[row].cols[col] == 3)
					{
						// If we have either a component or collectible at these coords, we insert 0 as distance (starting point)
						dist[row, col] = 0;
						queue.Enqueue((row, col)); // We add the point to the BFS queue
					}
					else
					{
						dist[row, col] = -1;
					}
				}
			}

			// BFS alogrithm
			while (queue.Count > 0)
			{
				(int r, int c) = queue.Dequeue();
				foreach ((int nr, int nc) in new[] { (r + 1, c), (r - 1, c), (r, c + 1), (r, c - 1) })
				{
					if (nr < 0 || nr >= NbRows || nc < 0 || nc >= NbCols)
						continue;
					if (dist[nr, nc] != -1)
						continue;

					dist[nr, nc] = dist[r, c] + 1;
					queue.Enqueue((nr, nc));
				}
			}

			return dist;
		}

		public (int row, int col) GetFarthestPositionFromCollectibles(int minRow)
		{
			int[,] dist = BuildCollectibleDistanceMap(minRow);

			(int row, int col) best = (-1, -1);
			int bestDist = -1;

			for (int r = 0; r < NbRows; r++)
			{
				for (int c = 0; c < NbCols; c++)
				{
					if (_rows[r].cols[c] == 0 && dist[r, c] > bestDist)
					{
						bestDist = dist[r, c];
						best = (r + 1, c);
					}
				}
			}

			return best;
		}
	}
}
