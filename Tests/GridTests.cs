using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

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
			Assert.AreEqual(grid.Cells.Count, 13 * 17);
			Assert.AreEqual(grid.Cells.Count, grid.Columns * grid.Rows);
			grid[grid.Columns - 1, grid.Rows - 1] = 4;
			Assert.AreEqual(4, grid.Last());
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
			Assert.IsTrue(grid.All(value => 78 == value));
		}

		[TestMethod()]
		public void GetEnumeratorTest()
		{
			Grid<int> grid = new(543, 127);
			int count = 0;
			grid.Fill(78);
			foreach (var cell in grid)
			{
				++count;
				Assert.AreEqual(78, cell);
			}
			Assert.AreEqual(grid.Columns * grid.Rows, count);
		}

		[TestMethod()]
		public void CopyFromTest()
		{
			Grid<int> grid = new(543, 127);
			var arr = Enumerable.Repeat(45, grid.Cells.Count * 10).ToArray();
			grid.CopyFrom(arr, grid.Cells.Count);
			Assert.ThrowsException<ArgumentException>(() => grid.CopyFrom(arr, arr.Length));
			Assert.IsTrue(grid.All(value => 45 == value));
			var arr2 = Enumerable.Repeat(67, 10).ToArray();
			grid.CopyFrom(arr2, arr2.Length);
			Assert.IsTrue(grid.Take(10).All(value => 67 == value));
			Assert.IsTrue(grid.Skip(10).All(value => 45 == value));
		}
	}
}