using System.Collections.Generic;
using Zenseless.OpenTK;

namespace Example
{
	internal class BruteForceCollision : ICollisionAlgo
	{
		public static void Check(HashSet<GameObject> colliding, IReadOnlyList<GameObject> gameObjects)
		{
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
		}

		public HashSet<GameObject> Check(IReadOnlyList<GameObject> gameObjects)
		{
			HashSet<GameObject> colliding = new();
			Check(colliding, gameObjects);
			return colliding;
		}

		public void Render()
		{
		}
	}
}
