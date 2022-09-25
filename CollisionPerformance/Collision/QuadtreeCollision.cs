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

        private static QuadtreeNode<GameObject> Create()
        {
            return new QuadtreeNode<GameObject>(new Box2(-1f, -1f, 1f, 1f));
        }

        public HashSet<GameObject> Check(IReadOnlyList<GameObject> gameObjects)
        {
            _quadTree = Create();
            foreach (var gameObject in gameObjects)
            {
                _quadTree.Insert(gameObject);
            }
            HashSet<GameObject> colliding = new();
            List<GameObject> potential = new();
            foreach (var gameObject in gameObjects)
            {
                potential.Clear();
                //TODO: only works if radius is >= all objects -> change to rect quad tree
                var bounds = gameObject.Bounds(gameObject.Radius); //point quad tree -> enlarge bounds by object size

                _quadTree.Query(bounds, potential);
                BruteForceCollision.Check(colliding, potential);
            }
            return colliding;
        }

        public void Render() => Render(_renderer, _quadTree);

        private readonly Handle<Material> _materialQuadTree;
        private QuadtreeNode<GameObject> _quadTree;
        private readonly BoxRenderer _renderer;

        private void Render(BoxRenderer renderer, QuadtreeBase<GameObject> quadTree)
        {
            _renderer.Enqueue(quadTree.Bounds, _materialQuadTree);
            if (quadTree is QuadtreeNode<GameObject> node)
                foreach (var child in node.Children)
                {
                    Render(renderer, child);
                }
        }
    }
}
