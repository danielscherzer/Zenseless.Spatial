using OpenTK.Mathematics;

namespace Example;

public class GameObject
{
	public GameObject(float x, float y, float radius)
	{
		var position = new Vector2(x, y);
		Vector2 radii = new(radius);
		Bounds = new Box2(position - radii, position + radii);
	}

	public Box2 Bounds { get; set; }
	public Vector2 Velocity { get; set; }
	public float Radius => Bounds.HalfSize.X;
}
