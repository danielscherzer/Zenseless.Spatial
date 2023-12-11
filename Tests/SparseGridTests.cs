using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Zenseless.Spatial.Tests;

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
}