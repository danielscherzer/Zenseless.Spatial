using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenseless.OpenTK;
using Zenseless.Patterns;

namespace Example
{
	internal class BruteForceCollision : ICollisionAlgo
	{
		public HashSet<GameObject> Check(IReadOnlyList<GameObject> gameObjects)
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

		public void Render()
		{
		}
	}
}
