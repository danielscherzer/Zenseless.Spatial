using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Zenseless.Spatial
{
	/// <summary>
	/// Interface for a grid with read only access
	/// </summary>
	/// <typeparam name="CellType"></typeparam>
	public interface IReadOnlyGrid<CellType> //: IEnumerable<CellType>
	{
		/// <summary>
		/// Returns a cell for a given column and row
		/// </summary>
		/// <param name="column">address of the cell</param>
		/// <param name="row">address of the cell</param>
		/// <returns>contents of a cell</returns>
		CellType this[int column, int row] { get; }

		/// <summary>
		/// Allows to iterate over all cells.
		/// </summary>
		IEnumerable<CellType> Cells { get; }

		/// <summary>
		/// A <seealso cref="ReadOnlyCollection{T}"/> of cells.
		/// </summary>
		ReadOnlyCollection<CellType> AsReadOnly();

		/// <summary>
		/// Number of columns in the grid.
		/// </summary>
		int Columns { get; }

		/// <summary>
		/// Number of rows in the grid.
		/// </summary>
		int Rows { get; }

		/// <summary>
		/// Is the given column and row number contained in the grid
		/// </summary>
		/// <param name="column">address of the cell</param>
		/// <param name="row">address of the cell</param>
		/// <returns><c>true</c> if the column and row combination is contained in the grid, <c>false</c> otherwise.</returns>
		bool Valid(int column, int row);
	}
}