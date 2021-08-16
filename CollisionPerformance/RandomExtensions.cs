using OpenTK.Mathematics;
using System;

namespace Example
{
	public static class RandomExtensions
	{
		public static Vector2 Vec2(this Random random, float min = -1f, float max = 1f)
		{
			return new Vector2(random.Next(min, max), random.Next(min, max));
		}

		public static float Next(this Random random, float min = 0f, float max = 1f)
		{
			return min + (max - min) * (float)random.NextDouble();
		}

		public static float NextGauß(this Random random)
		{
			var r1 = (float)random.NextDouble();
			var r2 = (float)random.NextDouble();
			return MathF.Sqrt(-2f * MathF.Log(r1)) * MathF.Cos(2f * MathF.PI * r2);
		}
	}
}
