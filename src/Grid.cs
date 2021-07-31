using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Zenseless.Spatial
{
	/// <summary>
	/// A two dimensional grid, internally represented by an array.
	/// </summary>
	/// <typeparam name="CellType">Data type of each cell</typeparam>
	public class Grid<CellType> : IReadOnlyGrid<CellType>
	{
		/// <summary>
		/// Create an instance of a grid with the given dimensions
		/// </summary>
		/// <param name="columns">number of columns</param>
		/// <param name="rows">number of rows</param>
		public Grid(int columns, int rows)
		{
			if (0 == columns) throw new ArgumentOutOfRangeException(nameof(columns));
			if (0 == rows) throw new ArgumentOutOfRangeException(nameof(rows));
			Columns = columns;
			Rows = rows;
			_cells = new CellType[columns * rows];
		}

		/// <summary>
		/// Returns a cell for a given column and row
		/// </summary>
		/// <param name="column">address of the cell</param>
		/// <param name="row">address of the cell</param>
		/// <returns>contents of a cell</returns>
		public CellType this[int column, int row]
		{
			get { return _cells[column + Columns * row]; }
			set { _cells[column + Columns * row] = value; }
		}

		/// <summary>
		/// A <seealso cref="ReadOnlyCollection{T}"/> of cells.
		/// </summary>
		public ReadOnlyCollection<CellType> Cells => Array.AsReadOnly(_cells);

		/// <summary>
		/// Copy the data given by an array starting at a given start index into the grid
		/// </summary>
		/// <param name="data">source data to be copied</param>
		/// <param name="dataStartIndex">start index inside the source data to start copying</param>
		public void CopyFrom(CellType[] data, int dataStartIndex = 0)
		{
			Array.Copy(data, dataStartIndex, _cells, 0, _cells.Length); // will throw exception if data is too short
		}

		/// <summary>
		/// Assigns the given value of type <c>CellType</c> to each element of the grid.
		/// </summary>
		/// <param name="value">value to fill with</param>
		public void Fill(CellType value) => Array.Fill(_cells, value);

		/// <summary>
		/// Number of columns in the grid.
		/// </summary>
		public int Columns { get; set; }

		/// <summary>
		/// Number of rows in the grid.
		/// </summary>
		public int Rows { get; set; }

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		public IEnumerator<CellType> GetEnumerator()
		{
			foreach (var cell in _cells)
			{
				yield return cell;
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>
		/// Is the given column and row number contained in the grid
		/// </summary>
		/// <param name="column">address of the cell</param>
		/// <param name="row">address of the cell</param>
		/// <returns><c>true</c> if the column and row combination is contained in the grid, <c>false</c> otherwise.</returns>
		public bool Valid(int column, int row) => 0 <= column && column < Columns && 0 <= row && row < Rows;

		private readonly CellType[] _cells;
	}
}
