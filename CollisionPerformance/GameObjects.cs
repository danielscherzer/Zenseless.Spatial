using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using Zenseless.Patterns;

namespace Example
{
	internal class GameObjects
	{
		public static GameObject Create()
		{
			var x = random.NextFloat(-1f, 1f);
			var y = random.NextFloat(-1f, 1f);
			var size = random.NextFloat();
			return new GameObject(x, y)
			{
				Velocity = new Vector2(random.NextFloat(-0.01f, 0.01f), random.NextFloat(-0.01f, 0.01f)),
				Radius = size < 0.99f ? 0.002f : 0.01f,
			};
		}

		private static void Add(List<GameObject> gameObjects, int count)
		{
			for (int i = 0; i < count; ++i)
			{
				gameObjects.Add(Create());
			}
		}

		internal static void Update(Observable<List<GameObject>> gameObjects, int count)
		{
			var go = gameObjects.HasValue ? gameObjects.Get() : new List<GameObject>();
			var diff = go.Count - count;
			if (0 < diff)
			{
				go.RemoveRange(go.Count - diff, diff);
			}
			else
			{
				Add(go, -diff);
			}
			gameObjects.Set(go);
		}

		private static readonly Random random = new(12);
	}
}
