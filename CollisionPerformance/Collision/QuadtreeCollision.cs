using Example.Services;
using Example.Spatial;
using OpenTK.Mathematics;
using System.Collections.Generic;
using Zenseless.Patterns;

namespace Example.Collision
{
	internal class QuadtreeCollision : ICollisionAlgo
	{
		public QuadtreeCollision(BoxRenderer renderer)
		{
			_materialQuadTree = renderer.Add(new Material(Color4.CornflowerBlue, false));
			_quadTree = Create();
			_renderer = renderer;
		}

		private static QuadtreeNode<int> Create()
		{
			return new QuadtreeNode<int>(new Box2(-1f, -1f, 1f, 1f));
		}

		public HashSet<GameObject> Check(IReadOnlyList<GameObject> gameObjects)
		{
			static Box2 Inflate(GameObject gameObject)
			{
				Vector2 radii = new(2f * gameObject.Radius);
				var pos = gameObject.Position;
				return new Box2(pos - radii, pos + radii);
			}

			_quadTree = Create();
			for (int id = 0; id < gameObjects.Count; id++)
			{
				_quadTree.Insert(gameObjects[id].Position, id);
			}
			HashSet<GameObject> colliding = new();
			List<int> potential = new();
			foreach (var gameObject in gameObjects)
			{
				potential.Clear();
				//Only works because check (a coll b) and (b coll a) and only two categories should miss (big coll big)
				var bounds = Inflate(gameObject); //point quad tree -> enlarge bounds by object size

				_quadTree.Query(bounds, potential);
				IdGridCollision.Check(colliding, gameObjects, potential);
			}
			return colliding;
		}

		public void Render() => Render(_renderer, _quadTree);

		private readonly Handle<Material> _materialQuadTree;
		private QuadtreeNode<int> _quadTree;
		private readonly BoxRenderer _renderer;

		private void Render(BoxRenderer renderer, QuadtreeBase<int> quadTree)
		{
			_renderer.Enqueue(quadTree.Bounds, _materialQuadTree);
			if (quadTree is QuadtreeNode<int> node)
				foreach (var child in node.Children)
				{
					Render(renderer, child);
				}
		}
	}
}
