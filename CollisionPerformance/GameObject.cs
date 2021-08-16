using Example.Spatial;
using OpenTK.Mathematics;

namespace Example
{
	public class GameObject : IPosition
	{
		public GameObject(Vector2 center) => Position = center;

		public GameObject(float x, float y) => Position = new Vector2(x, y);

		public Vector2 Position { get; set; }
		public Vector2 Velocity { get; set; }
		public float Radius { get; } = 0.01f;

		public void Update(float deltaTime)
		{
			var center = Position + deltaTime * Velocity;
			//wrap movement at window edges
			if (center.X - Radius > 1) center.X = -1f - Radius;
			if (center.X + Radius < -1) center.X = 1f + Radius;
			if (center.Y - Radius > 1) center.Y = -1f - Radius;
			if (center.Y + Radius < -1) center.Y = 1f + Radius;
			Position = center;
		}
	}
}
