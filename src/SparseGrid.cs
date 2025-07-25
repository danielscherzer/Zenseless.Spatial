﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Zenseless.Spatial;

/// <summary>
/// A two dimensional sparse grid, internally represented by a dictionary and a morton code key.
/// </summary>
/// <typeparam name="CellType">Data type of each cell</typeparam>
[DebuggerDisplay("Count = {Count}")]
[Serializable]
public class SparseGrid<CellType> : IEnumerable<CellType>
{
	/// <summary>
	/// For a given column and row returns the value of a cell if it exists or sets the value of the cell 
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
	/// Clear all cells of the grid
	/// </summary>
	public void Clear() => _cells.Clear();

	/// <summary>
	/// Checks if the given cell exists
	/// </summary>
	/// <param name="column">column of the cell</param>
	/// <param name="row">row of the cell</param>
	/// <returns></returns>
	public bool Contains(int column, int row) => _cells.ContainsKey(Id(column, row));

	/// <summary>
	/// Gets the value of elements in the grid
	/// </summary>
	public int Count => _cells.Count;

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

	/// <summary>
	/// Returns an enumerator that iterates through the cell items.
	/// </summary>
	/// <returns></returns>
	public IEnumerator<CellType> GetEnumerator() => _cells.Values.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => _cells.Values.GetEnumerator();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Id(int column, int row) => (int)Morton.Interleave((uint)(column + Half), (uint)(row + Half));

	private readonly Dictionary<int, CellType> _cells = new();
	private const int Half = ushort.MaxValue / 2;
}
