using OpenTK.Mathematics;

namespace Example
{
	public static class GameObjectExtensions
	{
		public static Box2 Bounds(this GameObject gameObject)
		{
			Vector2 radii = new(gameObject.Radius);
			var pos = gameObject.Position;
			return new Box2(pos - radii, pos + radii);
		}

		public static Box2 Bounds(this GameObject gameObject, float inflateRadius)
		{
			Vector2 radii = new(gameObject.Radius + inflateRadius);
			var pos = gameObject.Position;
			return new Box2(pos - radii, pos + radii);
		}
	}
}
