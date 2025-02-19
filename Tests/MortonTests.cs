using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Zenseless.Spatial.Tests;

[TestClass()]
public class MortonTests
{
	[TestMethod()]
	[DataRow(0b0u, 0b0u, 0b0u)]
	[DataRow(0b1u, 0b0u, 0b1u)]
	[DataRow(0b0u, 0b1u, 0b10u)]
	[DataRow(0b1u, 0b1u, 0b11u)]
	[DataRow(0xFFFFu, 0xFFFFu, 0xFFFFFFFFu)]
	[DataRow(0xFFFFu, 0x0u, 0x55555555u)]
	[DataRow(0x0u, 0xFFFFu, 0xAAAAAAAAu)]
	public void InterleaveTest(uint x, uint y, uint n)
	{
		var result = Morton.Interleave(x, y);
		AssertBinary(n, result);
	}

	[TestMethod()]
	[DataRow(0b0u, 0b0u, 0b0u)]
	[DataRow(0b1u, 0b0u, 0b1u)]
	[DataRow(0b0u, 0b1u, 0b10u)]
	[DataRow(0b1u, 0b1u, 0b11u)]
	[DataRow(0xFFFFu, 0xFFFFu, 0xFFFFFFFFu)]
	[DataRow(0xFFFFu, 0x0u, 0x55555555u)]
	[DataRow(0x0u, 0xFFFFu, 0xAAAAAAAAu)]
	public void DeInterleaveTest(uint x, uint y, uint n)
	{
		var (resultX, resultY) = Morton.DeInterleave(n);
		AssertBinary(x, resultX);
		AssertBinary(y, resultY);
	}

	[TestMethod()]
	public void InterleaveDeinterleaveTest()
	{
		Random random = new(12);
		for (int i = 0; i < 100000; i++)
		{
			var x = (ushort)random.Next();
			var y = (ushort)random.Next();
			var n = Morton.Interleave(x, y);
			var (newX, newY) = Morton.DeInterleave(n);
			Assert.AreEqual(x, newX);
			Assert.AreEqual(y, newY);
		}
	}

	private static void AssertBinary(uint expected, uint result)
	{
		Assert.AreEqual(expected, result, $"Expected:{expected:B}, actual:{result:B}");
	}
}