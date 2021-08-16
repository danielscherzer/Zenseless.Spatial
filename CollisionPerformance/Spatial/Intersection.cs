using OpenTK.Mathematics;

namespace Example.Spatial
{
	public static class Intersection
	{
		public static bool Intersects(this Box2 a, Box2 b)
		{
			bool noXintersect = a.Max.X <= b.Min.X || a.Min.X >= b.Max.X;
			bool noYintersect = a.Max.Y <= b.Min.Y || a.Min.Y >= b.Max.Y;
			return !(noXintersect || noYintersect);
		}
	}
}
