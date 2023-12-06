using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text.Json;

namespace Zenseless.Spatial.Tests
{
	[TestClass()]
	public class GridTests
	{
		[TestMethod()]
		public void EmptyGridTest()
		{
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Grid<int>(0, 1));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Grid<int>(1, 0));
		}

		[TestMethod()]
		public void GridSizeTest()
		{
			Grid<int> grid = new(13, 17);
			Assert.AreEqual(grid.AsReadOnly().Count, 13 * 17);
			Assert.AreEqual(grid.AsReadOnly().Count, grid.Columns * grid.Rows);
			grid[grid.Columns - 1, grid.Rows - 1] = 4;
			Assert.AreEqual(4, grid.AsReadOnly().Last());
		}

		[TestMethod()]
		public void ValidTest()
		{
			Grid<int> grid = new(7, 5);
			Assert.IsTrue(grid.Valid(grid.Columns - 1, grid.Rows - 1));
			Assert.IsTrue(grid.Valid(0, 0));
			Assert.IsFalse(grid.Valid(-1, 0));
			Assert.IsFalse(grid.Valid(0, -1));
			Assert.IsFalse(grid.Valid(0, grid.Rows));
			Assert.IsFalse(grid.Valid(grid.Columns, 0));
		}

		[TestMethod()]
		public void FillTest()
		{
			Grid<int> grid = new(127, 543);
			grid.Fill(78);
			Assert.IsTrue(grid.AsReadOnly().All(value => 78 == value));
		}

		[TestMethod()]
		public void ForeachTest()
		{
			Grid<int> grid = new(127, 543);
			grid.ForEach(() => 78);
			Assert.IsTrue(grid.AsReadOnly().All(value => 78 == value));
		}

		[TestMethod()]
		public void Foreach2Test()
		{
			Grid<int> grid = new(127, 543);
			grid.ForEach((col, row, value) => col + (row * grid.Columns));
			for (int i = 0; i < grid.Cells.Length; ++i)
			{
				Assert.AreEqual(grid.Cells[i], i);
			}
		}

		[TestMethod()]
		public void CellEnumerationTest()
		{
			Grid<int> grid = new(543, 127);
			int count = 0;
			grid.Fill(78);
			foreach (var cell in grid.AsReadOnly())
			{
				++count;
				Assert.AreEqual(78, cell);
			}
			Assert.AreEqual(grid.Columns * grid.Rows, count);
		}

		[TestMethod()]
		public void CellsTest()
		{
			Grid<int> grid = new(2, 3);
			grid.Cells = new int[] { 1, 2, 3, 4, 5, 6 };
			for (int i = 0; i < grid.Cells.Length; ++i)
			{
				Assert.AreEqual(i + 1, grid.Cells[i]);
			}
			Assert.ThrowsException<ArgumentException>(() => grid.Cells = new int[] { 1, 2, 3 });
		}

		[TestMethod()]
		public void CopyFromTest()
		{
			Grid<int> grid = new(543, 127);
			var arr = Enumerable.Repeat(45, grid.AsReadOnly().Count * 10).ToArray();
			grid.CopyFrom(arr, grid.AsReadOnly().Count);
			Assert.ThrowsException<ArgumentException>(() => grid.CopyFrom(arr, arr.Length));
			Assert.IsTrue(grid.AsReadOnly().All(value => 45 == value));
			var arr2 = Enumerable.Repeat(67, 10).ToArray();
			grid.CopyFrom(arr2, arr2.Length);
			Assert.IsTrue(grid.AsReadOnly().Take(10).All(value => 67 == value));
			Assert.IsTrue(grid.AsReadOnly().Skip(10).All(value => 45 == value));
		}

		[TestMethod()]
		public void SerializeAndDeserializeTest()
		{
			Grid<int> grid = new(5, 3);
			grid.Fill(78);
			string jsonString = JsonSerializer.Serialize(grid);
			var grid2 = JsonSerializer.Deserialize<Grid<int>>(jsonString);
			Assert.IsTrue(grid.Equals(grid2));
		}

		[TestMethod()]
		public void SerializeInterfaceAndDeserializeTest()
		{
			Grid<int> grid = new(5, 3);
			grid.Fill(78);
			IReadOnlyGrid<int> ro = grid;
			string jsonString = JsonSerializer.Serialize(ro);
			var grid2 = JsonSerializer.Deserialize<Grid<int>>(jsonString);
			Assert.IsTrue(grid.Equals(grid2));
		}

		[TestMethod()]
		public void EqualsTest()
		{
			Assert.IsTrue(new Grid<int>(5, 3).Equals(new Grid<int>(5, 3)));
			Assert.IsFalse(new Grid<int>(5, 3).Equals(new Grid<int>(3, 5)));
			Grid<int> grid = new(5, 3);
			grid.Fill(78);
			Grid<int> grid2 = new(5, 3);
			grid2.Fill(78);
			Assert.IsTrue(grid.Equals(grid2));
			grid[grid.Columns - 1, grid.Rows - 1] = 4;
		}

		[DataTestMethod()]
		[DataRow(0, 0)]
		[DataRow(8, 2)]
		[DataRow(0, 1)]
		[DataRow(9, 2)]
		public void GetIDTest(int column, int row)
		{
			Grid<int> grid = new(10, 3);
			var id = grid.GetID(column, row);
			var (actualColumn, actualRow) = grid.GetColRow(id);
			Assert.AreEqual(actualColumn, column);
			Assert.AreEqual(actualRow, row);
		}
	}
}