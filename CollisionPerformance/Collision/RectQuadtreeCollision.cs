using Example.Services;
using Example.Spatial;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Zenseless.OpenTK;
using Zenseless.Patterns;

namespace Example.Collision
{
    internal class RectQuadtreeCollision : ICollisionAlgo
    {
        public RectQuadtreeCollision(BoxRenderer renderer)
        {
            _materialQuadTree = renderer.Add(new Material(Color4.CornflowerBlue, false));
            _quadTree = Create();
            _renderer = renderer;
        }

        private static RectQuadtreeNode<int> Create()
        {
            return new RectQuadtreeNode<int>(new Box2(-1f, -1f, 1f, 1f));
        }

        public List<GameObject> Check(IReadOnlyList<GameObject> gameObjects)
        {
            _quadTree = Create();
			for (int id = 0; id < gameObjects.Count; id++)
            {
                _quadTree.Insert(gameObjects[id].Bounds(), id);
            }
            HashSet<GameObject> colliding = new();
            _quadTree.Traverse(null, leaf =>
            {
                Check(colliding, gameObjects, leaf.Items);
			});
            return colliding;
        }

		public void Render() => Render(_renderer, _quadTree);

        private readonly Handle<Material> _materialQuadTree;
        private RectQuadtreeNode<int> _quadTree;
        private readonly BoxRenderer _renderer;

		private static void Check(HashSet<GameObject> colliding, IReadOnlyList<GameObject> gameObjects, IReadOnlyList<(Box2 bounds, int item)> items)
		{
			for (int i = 0; i < items.Count - 1; ++i)
			{
				var a = items[i];
				for (int j = i + 1; j < items.Count; ++j)
				{
					var b = items[j];
					if (a.bounds.Overlaps(b.bounds))
					{
						colliding.Add(gameObjects[a.item]);
						colliding.Add(gameObjects[b.item]);
					}
				}
			}
		}
		
        private void Render(BoxRenderer renderer, RectQuadtreeBase<int> quadTree)
        {
            _renderer.Enqueue(quadTree.Bounds, _materialQuadTree);
            if (quadTree is RectQuadtreeNode<int> node)
                foreach (var child in node.Children)
                {
                    Render(renderer, child);
                }
        }
    }
}
