using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Zenseless.Spatial
{
	/// <summary>
	/// A two dimensional grid, internally represented by an array.
	/// </summary>
	/// <typeparam name="CellType">Data type of each cell</typeparam>
	[DebuggerDisplay("Count = {Columns}x{Rows}")]
	[Serializable]
	public class Grid<CellType> : IReadOnlyGrid<CellType>, IEquatable<Grid<CellType>>
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
			get { return _cells[ID(column, row)]; }
			set { _cells[ID(column, row)] = value; }
		}

		/// <summary>
		/// A <seealso cref="ReadOnlyCollection{T}"/> of cells.
		/// </summary>
		public ReadOnlyCollection<CellType> AsReadOnly()
		{
			Contract.Ensures(Contract.Result<ReadOnlyCollection<CellType>>() != null);
			return Array.AsReadOnly(_cells);
		}

		/// <summary>
		/// Return the array of cells for direct manipulation
		/// </summary>
		public CellType[] Cells
		{
			get => _cells;
			set
			{
				if(value.Length != Columns * Rows) throw new ArgumentException("Given array does not match in size.");
				_cells = value;
			}
		}

		/// <summary>
		/// Copy the data given by an array starting at a given start index into the grid. No grid resize will take place. Uses <seealso cref="Array.Copy(Array, int, Array, int, int)"/>
		/// </summary>
		/// <param name="data">source data to be copied</param>
		/// <param name="length">length of the data to copy</param>
		/// <param name="dataStartIndex">start index inside the source data to start copying</param>
		public void CopyFrom(CellType[] data, int length, int dataStartIndex = 0)
		{
			Array.Copy(data, dataStartIndex, _cells, 0, length); // will throw exception if grid is too small
		}

		/// <summary>
		/// Assigns the given value of type <c>CellType</c> to each element of the grid.
		/// </summary>
		/// <param name="value">value to fill with</param>
		public void Fill(CellType value) => Array.Fill(_cells, value);

		/// <summary>
		/// Iterate over all cells and execute <paramref name="eval"/> for each
		/// </summary>
		/// <param name="eval">Functor that will be executed for each cell</param>
		public void ForEach(Func<CellType> eval)
		{
			for (int i = 0; i < _cells.Length; ++i)
			{
				_cells[i] = eval();
			}
		}

		/// <summary>
		/// Iterate over all cells and execute <paramref name="eval"/> for each
		/// </summary>
		/// <param name="eval">Functor that will be executed for each cell</param>
		public void ForEach(Func<int, int, CellType> eval)
		{
			for (int row = 0; row < Rows; row++)
			{
				for(int col = 0; col < Columns; col++)
				{
					_cells[ID(col, row)] = eval(col, row);
				}
			}
		}

		/// <summary>
		/// Iterate over all cells and execute <paramref name="eval"/> for each
		/// </summary>
		/// <param name="eval">Functor that will be executed for each cell</param>
		public void ForEach(Func<int, int, CellType, CellType> eval)
		{
			for (int row = 0; row < Rows; row++)
			{
				for (int col = 0; col < Columns; col++)
				{
					var id = ID(col, row);
					_cells[id] = eval(col, row, _cells[id]);
				}
			}
		}

		/// <summary>
		/// Number of columns in the grid.
		/// </summary>
		public int Columns { get; }

		/// <summary>
		/// Number of rows in the grid.
		/// </summary>
		public int Rows { get; }
		
		IEnumerable<CellType> IReadOnlyGrid<CellType>.Cells => _cells;

		/// <summary>
		/// Is the given column and row number contained in the grid
		/// </summary>
		/// <param name="column">address of the cell</param>
		/// <param name="row">address of the cell</param>
		/// <returns><c>true</c> if the column and row combination is contained in the grid, <c>false</c> otherwise.</returns>
		public bool Valid(int column, int row) => 0 <= column && column < Columns && 0 <= row && row < Rows;

		/// <summary>
		/// Determines whether the specified Grid instances are considered equal.
		/// </summary>
		/// <param name="other">The second object to compare.</param>
		/// <returns><c>true</c> if the objects are considered equal; otherwise, <c>false</c>. If both are null, the method returns true.</returns>
		public override bool Equals(object? other)
		{
			if (ReferenceEquals(this, other)) return true;
			if (other?.GetType() != GetType()) return false;
			return Equals(other as Grid<CellType>);
		}

		/// <summary>
		/// Determines whether the specified Grid instances are considered equal.
		/// </summary>
		/// <param name="other">The second object to compare.</param>
		/// <returns><c>true</c> if the objects are considered equal; otherwise, <c>false</c>. If both are null, the method returns true.</returns>
		public bool Equals(Grid<CellType>? other)
		{
			if (other is null) return false;
			if (Columns != other.Columns || Rows != other.Rows) return false;
			ReadOnlyCollection<CellType> self = AsReadOnly();
			ReadOnlyCollection<CellType> o = other.AsReadOnly();
			for(int i = 0; i < self.Count; ++i)
			{
				if(!Equals(self[i], o[i])) return false;
			}
			return true;
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return Columns.GetHashCode() ^ Rows.GetHashCode() ^ _cells.GetHashCode();
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string? ToString()
		{
			return $"Columns:{Columns} Rows:{Rows} Cells:[{_cells}]";
		}

		private CellType[] _cells;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int ID(int column, int row) => column + Columns * row;
	}
}
