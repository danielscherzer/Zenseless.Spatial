using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using Zenseless.Patterns;

namespace Example
{
	internal class Scene
	{
		public static GameObject CreateObject()
		{
			var x = random.NextFloat(-1f, 1f);
			var y = random.NextFloat(-1f, 1f);
			return new GameObject(x, y) { Velocity = new Vector2(random.NextFloat(-0.01f, 0.01f), random.NextFloat(-0.01f, 0.01f)) };
		}

		public static List<GameObject> CreateObjects(int count)
		{
			List<GameObject> gameObjects = new();
			for (int i = 0; i < count; ++i)
			{
				gameObjects.Add(CreateObject());
			}
			return gameObjects;
		}

		private static readonly Random random = new(12);
	}
}
