using Example.Spatial;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Example.Collision;

internal sealed class QuadtreeCollision : ICollisionAlgo
{
	public QuadtreeCollision()
	{
		_quadTree = new QuadtreeNode<int>(new Box2(-1f, -1f, 1f, 1f));
	}

	public void FindCollisions(ICollection<int> colliding, IReadOnlyList<Box2> boundsList)
	{
		static Box2 Inflate(Box2 bounds)
		{
			return new Box2(bounds.Min - bounds.HalfSize, bounds.Max + bounds.HalfSize);
		}

		_quadTree.Clear();
		for (int id = 0; id < boundsList.Count; id++)
		{
			_quadTree.Insert(boundsList[id].Center, id);
		}
		colliding.Clear();
		List<int> potential = new();
		foreach (var gameObject in boundsList)
		{
			potential.Clear();
			//Only works because check (a coll b) and (b coll a) and only two categories should miss (big coll big)
			var bounds = Inflate(gameObject); //point quad tree -> enlarge bounds by object size

			_quadTree.Query(bounds, potential);
			BruteForceCollision.AddCollisions(colliding, boundsList, potential);
		}
	}

	public IQuadtree<int> Tree => _quadTree;
	private readonly QuadtreeNode<int> _quadTree;
}
