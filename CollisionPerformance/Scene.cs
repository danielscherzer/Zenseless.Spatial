using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using Zenseless.Patterns;

namespace Example
{
	internal class Scene
	{
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
