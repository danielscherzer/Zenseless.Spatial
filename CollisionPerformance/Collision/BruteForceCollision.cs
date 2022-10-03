using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using Zenseless.OpenTK;

namespace Example.Collision
{
	internal class BruteForceCollision : ICollisionAlgo
	{
		public void FindCollisions(ICollection<int> colliding, IReadOnlyList<Box2> bounds)
		{
			colliding.Clear();
			for (int i = 0; i < bounds.Count - 1; ++i)
			{
				var a = bounds[i];
				for (int j = i + 1; j < bounds.Count; ++j)
				{
					var b = bounds[j];
					if (a.Overlaps(b))
					{
						colliding.Add(i);
						colliding.Add(j);
					}
				}
			}
		}

		internal static void AddCollisions(ICollection<int> colliding, IReadOnlyList<Box2> bounds, IReadOnlyList<int> ids)
		{
			for (int i = 0; i < ids.Count - 1; ++i)
			{
				var idA = ids[i];
				var a = bounds[idA];
				for (int j = i + 1; j < ids.Count; ++j)
				{
					var idB = ids[j];
					var b = bounds[idB];
					if (a.Overlaps(b))
					{
						colliding.Add(idA);
						colliding.Add(idB);
					}
				}
			}
		}
	}
}
