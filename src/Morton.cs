using System.Runtime.CompilerServices;

namespace Zenseless.Spatial;

/// <summary>
/// 
/// </summary>
public static class Morton
{
	/// <summary>
	/// Interleave lower 16 bits of x and y, so the bits of x are in the even positions and bits from y in the odd.
	/// </summary>
	/// <param name="x">A positive integer value between 0 and 65536</param>
	/// <param name="y">A positive integer value between 0 and 65536</param>
	/// <returns>A Morton Code with the values of X and Y interleaved into a single Integer value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint Interleave(uint x, uint y)
	{
		return SpreadLower16Bit(x) | (SpreadLower16Bit(y) << 1);
	}

	/// <summary>
	/// Put even bits of input into x and odd bits into y
	/// Undo <see cref="Interleave(uint, uint)"/>
	/// </summary>
	/// <param name="z">Morton number with interleaved bits</param>
	/// <returns>Two uint with the lower 16 bits containing the results.</returns>
	public static (uint x, uint y) DeInterleave(uint z)
	{
		return (CompactLower16Bit(z), CompactLower16Bit(z >> 1));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint CompactLower16Bit(uint x)
	{
		x &= 0x55555555;
		x = (x ^ ( x >> 1)) & 0x33333333;
		x = (x ^ ( x >> 2)) & 0x0F0F0F0F;
		x = (x ^ ( x >> 4)) & 0x00FF00FF;
		x = (x ^ ( x >> 8)) & 0x0000FFFF;
		return x;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint SpreadLower16Bit(uint x)
	{
		// http://graphics.stanford.edu/~seander/bithacks.html#InterleaveBMN
		// See this post for interleaving 32-bit numbers into 64-bit result, and interesting comparisons with SIMD versions
		// https://lemire.me/blog/2018/01/08/how-fast-can-you-bit-interleave-32-bit-integers/
		x = (x | (x << 8)) & 0x00FF00FF;
		x = (x | (x << 4)) & 0x0F0F0F0F;
		x = (x | (x << 2)) & 0x33333333;
		x = (x | (x << 1)) & 0x55555555;
		return x;
	}
}
