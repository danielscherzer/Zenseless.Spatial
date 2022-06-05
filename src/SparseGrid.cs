using System;
using System.Collections.Generic;

namespace Zenseless.Spatial
{
	internal class SparseGrid<CellType>
	{
		/// <summary>
		/// Create an instance with the given dimensions
		/// </summary>
		/// <param name="columns">number of columns</param>
		/// <param name="rows">number of rows</param>
		public SparseGrid(int columns, int rows)
		{
			if (0 == columns) throw new ArgumentOutOfRangeException(nameof(columns));
			if (0 == rows) throw new ArgumentOutOfRangeException(nameof(rows));
			Columns = columns;
			Rows = rows;
		}

		/// <summary>
		/// Returns a cell for a given column and row
		/// </summary>
		/// <param name="column">address of the cell</param>
		/// <param name="row">address of the cell</param>
		/// <returns>contents of a cell</returns>
		public CellType this[int column, int row]
		{
			get { return _cells[Id(column, row)]; }
			set { _cells[Id(column, row)] = value; }
		}

		public int Columns { get; }

		public bool Contains(int column, int row) => _cells.ContainsKey(Id(column, row));

		public int Rows { get; }

		private int Id(int column, int row) => column + Columns * row;

		private readonly Dictionary<int, CellType> _cells = new();
	}
}
