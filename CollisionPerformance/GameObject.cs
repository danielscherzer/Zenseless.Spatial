using Example.Spatial;
using OpenTK.Mathematics;
using System;

namespace Example
{
	public class GameObject : IPosition
	{
		public GameObject(float x, float y) => Position = new Vector2(x, y);

		public Vector2 Position { get; set; }
		public Vector2 Velocity { get; set; }
		public float Radius { get; set; } = 0.002f;

		public void Update(float deltaTime)
		{
			var velocity = Velocity;
			var center = Position + deltaTime * velocity;
			//reflect at window edges, so object are always fully visible, because quad tree bounds should stay [-1,1]
			for (int axe = 0; axe < 2; ++axe)
			{
				if (MathF.Abs(center[axe]) >= 1f - Radius)
				{
					center[axe] = (1f - Radius) * MathF.Sign(velocity[axe]);
					velocity[axe] = -velocity[axe];
				}
			}
			Velocity = velocity;
			Position = center;
		}
	}
}
