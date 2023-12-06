using Example.Spatial;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using Zenseless.OpenTK;

namespace Example.Collision;

internal sealed class RectQuadtreeCollision : ICollisionAlgo
{
	public RectQuadtreeCollision()
	{
		_quadTree = new RectQuadtreeNode<int>(new Box2(-1f, -1f, 1f, 1f));
	}

	public void FindCollisions(ICollection<int> colliding, IReadOnlyList<Box2> boundsList)
	{
		_quadTree.Clear();
		for (int id = 0; id < boundsList.Count; id++)
		{
			_quadTree.Insert(boundsList[id], id);
		}
		colliding.Clear();
		_quadTree.Traverse(null, leaf =>
		{
			AddCollisions(colliding, leaf.BoundItems);
		});
	}

	public IQuadtree<int> Tree => _quadTree;

	private readonly RectQuadtreeNode<int> _quadTree;

	internal static void AddCollisions(ICollection<int> colliding, IReadOnlyList<(Box2 bounds, int id)> items)
	{
		for (int i = 0; i < items.Count - 1; ++i)
		{
			var (aBounds, aId) = items[i];
			for (int j = i + 1; j < items.Count; ++j)
			{
				var (bBounds, bId) = items[j];
				if (aBounds.Overlaps(bBounds))
				{
					colliding.Add(aId);
					colliding.Add(bId);
				}
			}
		}
	}
}
