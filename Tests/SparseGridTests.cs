using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text.Json;

namespace Zenseless.Spatial.Tests
{
	[TestClass()]
	public class SparseGridTests
	{
		[TestMethod()]
		public void GridSizeTest()
		{
			SparseGrid<int> grid = new();
			Assert.AreEqual(grid.Count, 0);
			grid[0, 0] = 4;
			Assert.AreEqual(4, grid[0, 0]);
			Assert.AreEqual(grid.Count, 1);
		}

		[TestMethod()]
		public void ForeachTest()
		{
			SparseGrid<int> grid = new();
			for (int i = 0; i < 20; ++i)
			{
				grid[i / 10, i % 10] = 78;
			}
			Assert.AreEqual(grid.Count, 20);
			Assert.IsTrue(grid.All(value => 78 == value));
		}

		[TestMethod()]
		public void SerializeAndDeserializeTest()
		{
			SparseGrid<int> grid = new();
			for (int i = 0; i < 20; ++i)
			{
				grid[i / 10, i % 10] = 78;
			}
			string jsonString = JsonSerializer.Serialize(grid);
			var grid2 = JsonSerializer.Deserialize<SparseGrid<int>>(jsonString);
			Assert.IsTrue(grid.Equals(grid2));
		}

		//[TestMethod()]
		//public void SerializeInterfaceAndDeserializeTest()
		//{
		//	SparseGrid<int> grid = new(5, 3);
		//	grid.Fill(78);
		//	IReadOnlyGrid<int> ro = grid;
		//	string jsonString = JsonSerializer.Serialize(ro);
		//	var grid2 = JsonSerializer.Deserialize<Grid<int>>(jsonString);
		//	Assert.IsTrue(grid.Equals(grid2));
		//}

		//[TestMethod()]
		//public void EqualsTest()
		//{
		//	Assert.IsTrue(new SparseGrid<int>(5, 3).Equals(new SparseGrid<int>(5, 3)));
		//	Assert.IsFalse(new SparseGrid<int>(5, 3).Equals(new SparseGrid<int>(3, 5)));
		//	SparseGrid<int> grid = new(5, 3);
		//	grid.Fill(78);
		//	SparseGrid<int> grid2 = new(5, 3);
		//	grid2.Fill(78);
		//	Assert.IsTrue(grid.Equals(grid2));
		//	grid[grid.Columns - 1, grid.Rows - 1] = 4;
		//}

		//[DataTestMethod()]
		//[DataRow(0, 0)]
		//[DataRow(8, 2)]
		//[DataRow(0, 1)]
		//[DataRow(9, 2)]
		//public void GetIDTest(int column, int row)
		//{
		//	SparseGrid<int> grid = new(10, 3);
		//	var id = grid.GetID(column, row);
		//	var (actualColumn, actualRow) = grid.GetColRow(id);
		//	Assert.AreEqual(actualColumn, column);
		//	Assert.AreEqual(actualRow, row);
		//}
	}
}