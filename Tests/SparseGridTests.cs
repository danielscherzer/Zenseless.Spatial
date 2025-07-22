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
		Assert.AreEqual(0, grid.Count);
		grid[0, 0] = 4;
		Assert.AreEqual(4, grid[0, 0]);
		Assert.AreEqual(1, grid.Count);
	}

	[TestMethod()]
	public void ForeachTest()
	{
		SparseGrid<int> grid = new();
		for (int i = 0; i < 20; ++i)
		{
			grid[i / 10, i % 10] = 78;
		}
		Assert.AreEqual(20, grid.Count);
		Assert.IsTrue(grid.All(value => 78 == value));
	}

	[TestMethod()]
	public void SignedRangeCollisionTest()
	{
		SparseGrid<int> grid = new();
		const int range = 257;
		int value = 0;
		for (int x = -range; x < range; ++x)
		{
			for(int y = -range; y < range; ++y)
			{
				grid[x, y] = value++;
			}
		}
		value = 0;
		for (int x = -range; x < range; ++x)
		{
			for (int y = -range; y < range; ++y)
			{
				Assert.AreEqual(value, grid[x, y]);
				value++;
			}
		}
	}
}