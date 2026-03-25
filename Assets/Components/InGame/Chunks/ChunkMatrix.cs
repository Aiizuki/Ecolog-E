using Assets.Components.InGame.Chunks.Interactables;
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
	}
}
