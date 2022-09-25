using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Zenseless.OpenTK;

namespace Example.Spatial
{
	public abstract class RectQuadtreeBase<TItem>
	{
		protected RectQuadtreeBase(in Box2 bounds) => Bounds = bounds;
		public Box2 Bounds { get; }
		public abstract bool NeedSplit { get; }
		public abstract void Insert(Box2 bounds, TItem item);
		public abstract void Query(Box2 area, ICollection<TItem> results);
		public abstract void Traverse(Action<RectQuadtreeNode<TItem>>? actionNode, Action<RectQuadtreeLeaf<TItem>>? actionLeaf);
	}

	public class RectQuadtreeLeaf<TItem> : RectQuadtreeBase<TItem>
	{
		private readonly List<(Box2 bounds, TItem item)> _items = new();

		public IReadOnlyList<(Box2 bounds, TItem item)> Items => _items;

		public RectQuadtreeLeaf(in Box2 bounds) : base(bounds) { }

		public override bool NeedSplit => _items.Count >= 8;

		public override void Insert(Box2 bounds, TItem item) => _items.Add((bounds, item));

		public override void Query(Box2 area, ICollection<TItem> results)
		{
			foreach ((Box2 bounds, TItem item) in Items)
			{
				if (area.Overlaps(bounds)) results.Add(item);
			}
		}

		public override void Traverse(Action<RectQuadtreeNode<TItem>>? actionNode
			, Action<RectQuadtreeLeaf<TItem>>? actionLeaf) => actionLeaf?.Invoke(this);
	}

	public class RectQuadtreeNode<TItem> : RectQuadtreeBase<TItem>
	{
		private readonly Vector2 center;

		public RectQuadtreeBase<TItem>[] Children { get; }

		public RectQuadtreeNode(in Box2 bounds) : base(bounds)
		{
			center = bounds.Center;
			Children = new RectQuadtreeBase<TItem>[]
			{
				new RectQuadtreeLeaf<TItem>(new Box2(bounds.Min, center)),
				new RectQuadtreeLeaf<TItem>(new Box2(center.X, bounds.Min.Y, bounds.Max.X, center.Y)),
				new RectQuadtreeLeaf<TItem>(new Box2(bounds.Min.X, center.Y, center.X, bounds.Max.Y)),
				new RectQuadtreeLeaf<TItem>(new Box2(center, bounds.Max)),
			};
		}

		public override bool NeedSplit => false;

		public override void Insert(Box2 bounds, TItem item)
		{
			void Process(int index)
			{
				var child = Children[index];
				if (child.NeedSplit)
				{
					RectQuadtreeLeaf<TItem> leaf = (RectQuadtreeLeaf<TItem>)child;
					child = new RectQuadtreeNode<TItem>(leaf.Bounds);
					// move local items to children
					foreach ((Box2 b, TItem i) in leaf.Items)
					{
						child.Insert(b, i);
					}
					Children[index] = child;
				}
				child.Insert(bounds, item);
			}

			int whichChildren = 0b1111;
			if (center.X < bounds.Min.X) whichChildren &= 0b1010;
			else if (center.X > bounds.Max.X) whichChildren &= 0b0101;

			if (center.Y < bounds.Min.Y) whichChildren &= 0b1100;
			else if (center.Y > bounds.Max.Y) whichChildren &= 0b0011;

			if (0 != (whichChildren & 0b0001)) Process(0);
			if (0 != (whichChildren & 0b0010)) Process(1);
			if (0 != (whichChildren & 0b0100)) Process(2);
			if (0 != (whichChildren & 0b1000)) Process(3);
		}

		public override void Query(Box2 area, ICollection<TItem> results)
		{
			int whichChildren = 0b1111;
			if (center.X < area.Min.X) whichChildren &= 0b1010;
			else if (center.X > area.Max.X) whichChildren &= 0b0101;

			if (center.Y < area.Min.Y) whichChildren &= 0b1100;
			else if (center.Y > area.Max.Y) whichChildren &= 0b0011;

			if (0 != (whichChildren & 0b0001)) Children[0].Query(area, results);
			if (0 != (whichChildren & 0b0010)) Children[1].Query(area, results);
			if (0 != (whichChildren & 0b0100)) Children[2].Query(area, results);
			if (0 != (whichChildren & 0b1000)) Children[3].Query(area, results);

			//TODO: Why is loop unrolling not done by compiler?
			//for (int i = 0; i < 4; ++i, whichChildren >>= 1)
			//{
			//	if (0 != (whichChildren & 0b0001)) Children[i].Query(area, results);
			//}
		}

		public override void Traverse(Action<RectQuadtreeNode<TItem>>? actionNode
			, Action<RectQuadtreeLeaf<TItem>>? actionLeaf)
		{
			foreach (var child in Children)
			{
				child.Traverse(actionNode, actionLeaf);
			}
		}
	}
}
