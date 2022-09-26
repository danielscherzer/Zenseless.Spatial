using Example.Services;
using Example.Spatial;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            _quadTree = Create();
			gameObjectBounds.Clear();
			for (int id = 0; id < gameObjects.Count; id++)
			{
				var bounds = gameObjects[id].Bounds();
				gameObjectBounds.Add(bounds);
				_quadTree.Insert(gameObjects[id].Position, id);
			}
            HashSet<GameObject> colliding = new();
            List<int> potential = new();
            foreach (var gameObject in gameObjects)
            {
                potential.Clear();
                //TODO: only works if radius is >= all objects -> change to rect quad tree
                var bounds = gameObject.Bounds(gameObject.Radius); //point quad tree -> enlarge bounds by object size

                _quadTree.Query(bounds, potential);
                IdGridCollision.Check(colliding, gameObjects, gameObjectBounds, potential);
            }
            return colliding;
        }

        public void Render() => Render(_renderer, _quadTree);

        private readonly Handle<Material> _materialQuadTree;
        private QuadtreeNode<int> _quadTree;
		private readonly List<Box2> gameObjectBounds = new();
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
