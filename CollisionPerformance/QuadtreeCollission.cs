﻿using Example.Services;
using Example.Spatial;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Linq;

namespace Example
{
	internal class QuadtreeCollission
	{
		public QuadtreeCollission(BoxRenderer renderer)
		{
			_materialQuadTree = renderer.Add(new Material(Color4.CornflowerBlue, false));
			_quadTree = new Quadtree<GameObject>(new Box2(-1f, -1f, 1f, 1f));
			_renderer = renderer;
		}

		public HashSet<GameObject> Check(IEnumerable<GameObject> gameObjects)
		{
			_quadTree = new Quadtree<GameObject>(new Box2(-1f, -1f, 1f, 1f));
			foreach (var gameObject in gameObjects)
			{
				QuadTree.Insert(gameObject);
			}
			HashSet<GameObject> colliding = new();
			foreach (var gameObject in gameObjects)
			{
				foreach (var go2 in QuadTree.Query(gameObject.Bounds()).Where(go => go != gameObject))
				{
					colliding.Add(go2);
				}
			}
			return colliding;
		}

		public Quadtree<GameObject> QuadTree => _quadTree;

		internal void Render() => Render(QuadTree);

		private readonly Handle<Material> _materialQuadTree;
		private Quadtree<GameObject> _quadTree;
		private readonly BoxRenderer _renderer;

		private void Render(Quadtree<GameObject> quadTree)
		{
			_renderer.Enqueue(quadTree.Bounds, _materialQuadTree);
			foreach (var child in quadTree.Children)
			{
				Render(child);
			}
		}
	}
}
