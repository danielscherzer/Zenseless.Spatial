using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Zenseless.Spatial
{
	/// <summary>
	/// A two dimensional sparse grid, internally represented by a dictionary.
	/// </summary>
	/// <typeparam name="CellType">Data type of each cell</typeparam>
	[DebuggerDisplay("Count = {Count}")]
	[Serializable]
	public class SparseGrid<CellType>
	{
		/// <summary>
		/// Create an instance with the given maximum dimensions
		/// </summary>
		/// <param name="columns">maximum number of columns</param>
		/// <param name="rows">maximum number of rows</param>
		public SparseGrid(int columns, int rows)
		{
			if (0 == columns) throw new ArgumentOutOfRangeException(nameof(columns));
			if (0 == rows) throw new ArgumentOutOfRangeException(nameof(rows));
			Columns = columns;
			Rows = rows;
		}

		/// <summary>
		/// Returns the value of a cell if it exists for a given column and row
		/// </summary>
		/// <param name="column">column of the cell</param>
		/// <param name="row">row of the cell</param>
		/// <returns>contents of a cell</returns>
		public CellType this[int column, int row]
		{
			get { return _cells[Id(column, row)]; }
			set { _cells[Id(column, row)] = value; }
		}

		/// <summary>
		/// Number of cells
		/// </summary>
		public int Count => _cells.Count;

		/// <summary>
		/// Maximum number of columns
		/// </summary>
		public int Columns { get; }

		/// <summary>
		/// Checks if the given cell exists
		/// </summary>
		/// <param name="column">column of the cell</param>
		/// <param name="row">row of the cell</param>
		/// <returns></returns>
		public bool Contains(int column, int row) => _cells.ContainsKey(Id(column, row));

		/// <summary>
		/// Iterate over all cells and execute <paramref name="exec"/> for each
		/// </summary>
		/// <param name="exec">Action that will be executed for each cell</param>
		public void ForEach(Action<CellType> exec)
		{
			foreach (var val in _cells.Values) exec(val);
		}

		/// <summary>
		/// Iterate over all cells and execute <paramref name="eval"/> for each
		/// </summary>
		/// <param name="eval">Functor that will be executed for each cell</param>
		public void ForEach(Func<CellType> eval)
		{
			for (int i = 0; i < _cells.Count; ++i)
			{
				_cells[i] = eval();
			}
		}

		/// <summary>
		/// Maximum number of rows in the grid.
		/// </summary>
		public int Rows { get; }

		/// <summary>
		/// Returns the value of the cell or if the cell does not exists creates it with the given functor
		/// </summary>
		/// <param name="column">column of the cell</param>
		/// <param name="row">row of the cell</param>
		/// <param name="eval">Functor to call if creation of the cell is necessary</param>
		/// <returns></returns>
		public CellType CreateOrReturn(int column, int row, Func<CellType> eval)
		{
			var id = Id(column, row);
			if (!_cells.TryGetValue(id, out var cell))
			{
				cell = eval();
				_cells[id] = cell;
			}
			return cell;
		}

		private int Id(int column, int row) => column + Columns * row;

		private readonly Dictionary<int, CellType> _cells = new();
	}
}
