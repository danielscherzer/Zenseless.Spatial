using Example.Services;
using Example.Spatial;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        private static RectQuadtreeNode<GameObject> Create()
        {
            return new RectQuadtreeNode<GameObject>(new Box2(-1f, -1f, 1f, 1f));
        }

        public HashSet<GameObject> Check(IReadOnlyList<GameObject> gameObjects)
        {
            _quadTree = Create();
            foreach (var gameObject in gameObjects)
            {
                _quadTree.Insert(gameObject.Bounds(), gameObject);
            }
            HashSet<GameObject> colliding = new();
            _quadTree.Traverse(null, leaf =>
            {
                var potential = leaf.Items.Select(pair => pair.item).ToList();
				BruteForceCollision.Check(colliding, potential);
			});
            return colliding;
        }

        public void Render() => Render(_renderer, _quadTree);

        private readonly Handle<Material> _materialQuadTree;
        private RectQuadtreeNode<GameObject> _quadTree;
        private readonly BoxRenderer _renderer;

        private void Render(BoxRenderer renderer, RectQuadtreeBase<GameObject> quadTree)
        {
            _renderer.Enqueue(quadTree.Bounds, _materialQuadTree);
            if (quadTree is RectQuadtreeNode<GameObject> node)
                foreach (var child in node.Children)
                {
                    Render(renderer, child);
                }
        }
    }
}
