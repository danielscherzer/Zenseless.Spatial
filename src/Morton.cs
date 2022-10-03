namespace Zenseless.Spatial
{
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
		public static uint Interleave(uint x, uint y)
		{
			// http://graphics.stanford.edu/~seander/bithacks.html#InterleaveBMN
			// See this post for interleaving 32-bit numbers into 64-bit result, and interesting comparisons with SIMD versions
			// https://lemire.me/blog/2018/01/08/how-fast-can-you-bit-interleave-32-bit-integers/
			x = (x | (x << 8)) & 0x00FF00FF;
			x = (x | (x << 4)) & 0x0F0F0F0F;
			x = (x | (x << 2)) & 0x33333333;
			x = (x | (x << 1)) & 0x55555555;

			y = (y | (y << 8)) & 0x00FF00FF;
			y = (y | (y << 4)) & 0x0F0F0F0F;
			y = (y | (y << 2)) & 0x33333333;
			y = (y | (y << 1)) & 0x55555555;

			return x | (y << 1);
		}
	}
}
