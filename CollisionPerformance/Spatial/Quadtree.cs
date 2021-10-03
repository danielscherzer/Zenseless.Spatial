using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Zenseless.OpenTK;

namespace Example.Spatial
{
	[DebuggerDisplay("Bounds={Bounds}")]
	public class Quadtree<DataType> where DataType : IPosition
	{
		public Quadtree(Box2 bounds) => Bounds = bounds;

		public Box2 Bounds { get; }

		public static uint Capacity { get; set; } = 3;

		public Quadtree<DataType>[] Children => _children;

		public IEnumerable<DataType> Item => _items;

		public bool Insert(DataType item)
		{
			if (!Bounds.Contains(item.Position)) return false;
			if (_items.Count < Capacity)
			{
				_items.Add(item);
			}
			else
			{
				if (0 == _children.Length) Subdivide();
				foreach (var child in _children)
				{
					if (child.Insert(item)) return true;
				}
			}
			return true;
		}

		public IEnumerable<DataType> Query(Box2 area)
		{
			if (Box2Extensions.Overlaps(area, Bounds))
			{
				foreach (var item in Item)
				{
					if (area.Contains(item.Position)) yield return item;
				}
				foreach (var child in Children)
				{
					foreach (var point in child.Query(area)) yield return point;
				}
			}
		}

		private readonly List<DataType> _items = new();
		private Quadtree<DataType>[] _children = Array.Empty<Quadtree<DataType>>();

		private void Subdivide()
		{
			var h = 0.5f * Vector2.One;
			_children = new Quadtree<DataType>[]
			{
				new Quadtree<DataType>(Bounds.Scaled(h, new Vector2(Bounds.Min.X, Bounds.Max.Y))),
				new Quadtree<DataType>(Bounds.Scaled(h, Bounds.Max)),
				new Quadtree<DataType>(Bounds.Scaled(h, Bounds.Min)),
				new Quadtree<DataType>(Bounds.Scaled(h, new Vector2(Bounds.Max.X, Bounds.Min.Y))),
			};
		}
	}
}
