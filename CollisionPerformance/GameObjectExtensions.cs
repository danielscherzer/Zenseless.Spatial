using OpenTK.Mathematics;

namespace Example
{
	public static class GameObjectExtensions
	{
		public static Box2 Bounds(this GameObject gameObject)
		{
			Vector2 radii = new(gameObject.Radius);
			return new Box2(gameObject.Position - radii, gameObject.Position + radii);
		}
	}
}
