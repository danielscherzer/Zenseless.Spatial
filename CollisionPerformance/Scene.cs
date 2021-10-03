using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using Zenseless.OpenTK;
using Zenseless.Patterns;

namespace Example
{
	internal class Scene
	{
		public static HashSet<GameObject> CollissionBruteForce(IReadOnlyList<GameObject> gameObjects)
		{
			HashSet<GameObject> colliding = new();
			for (int i = 0; i < gameObjects.Count - 1; ++i)
			{
				for (int j = i + 1; j < gameObjects.Count; ++j)
				{
					var a = gameObjects[i];
					var b = gameObjects[j];
					if (Box2Extensions.Overlaps(a.Bounds(), b.Bounds()))
					{
						colliding.Add(a);
						colliding.Add(b);
					}
				}
			}
			return colliding;
		}

		public static List<GameObject> CreateObjects(int count)
		{
			var random = new Random(12);
			List<GameObject> gameObjects = new();
			for (int i = 0; i < count; ++i)
			{
				var x = random.NextFloat(-1f, 1f);
				var y = random.NextFloat(-1f, 1f);
				gameObjects.Add(new GameObject(x, y) { Velocity = new Vector2(random.NextFloat(-0.01f, 0.01f), random.NextFloat(-0.01f, 0.01f)) });
			}
			return gameObjects;
		}
	}
}
