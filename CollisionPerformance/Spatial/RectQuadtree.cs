using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenseless.OpenTK;

namespace Example.Spatial
{
	public abstract class RectQuadtreeBase<TItem> : IQuadtree<TItem>
	{
		protected RectQuadtreeBase(in Box2 bounds) => Bounds = bounds;
		public Box2 Bounds { get; }
		public abstract bool NeedSplit { get; }
		public abstract void Clear();
		public abstract void Insert(Box2 bounds, TItem item);
		public abstract void Query(Box2 area, ICollection<TItem> results);
		public abstract void Traverse(Action<RectQuadtreeNode<TItem>>? actionNode, Action<RectQuadtreeLeaf<TItem>>? actionLeaf);
	}

	public class RectQuadtreeLeaf<TItem> : RectQuadtreeBase<TItem>, IQuadtreeLeaf<TItem>
	{
		public RectQuadtreeLeaf(in Box2 bounds) : base(bounds) { }

		public IReadOnlyList<(Box2 bounds, TItem item)> BoundItems => _items;

		public override void Clear() => _items.Clear();

		public override bool NeedSplit => _items.Count >= 8;

		IEnumerable<TItem> IQuadtreeLeaf<TItem>.Items => BoundItems.Select(i => i.item);

		public override void Insert(Box2 bounds, TItem item) => _items.Add((bounds, item));

		public override void Query(Box2 area, ICollection<TItem> results)
		{
			foreach ((Box2 bounds, TItem item) in BoundItems)
			{
				if (area.Overlaps(bounds)) results.Add(item);
			}
		}

		public override void Traverse(Action<RectQuadtreeNode<TItem>>? actionNode
			, Action<RectQuadtreeLeaf<TItem>>? actionLeaf) => actionLeaf?.Invoke(this);

		private readonly List<(Box2 bounds, TItem item)> _items = new();
	}

	public class RectQuadtreeNode<TItem> : RectQuadtreeBase<TItem>, IQuadtreeNode<TItem>
	{
		private readonly Vector2 center;

		public RectQuadtreeBase<TItem>[] Children { get; }

		public RectQuadtreeNode(in Box2 bounds) : base(bounds)
		{
			center = bounds.Center;
			Children = new RectQuadtreeBase<TItem>[4];
			Clear();
		}

		public override void Clear()
		{
			Children[0] = new RectQuadtreeLeaf<TItem>(new Box2(Bounds.Min, center));
			Children[1] = new RectQuadtreeLeaf<TItem>(new Box2(center.X, Bounds.Min.Y, Bounds.Max.X, center.Y));
			Children[2] = new RectQuadtreeLeaf<TItem>(new Box2(Bounds.Min.X, center.Y, center.X, Bounds.Max.Y));
			Children[3] = new RectQuadtreeLeaf<TItem>(new Box2(center, Bounds.Max));
		}

		public override bool NeedSplit => false;

		IQuadtree<TItem>[] IQuadtreeNode<TItem>.Children => Children;

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
					foreach ((Box2 b, TItem i) in leaf.BoundItems)
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
			//TODO: Check loop unrolling
			foreach (var child in Children)
			{
				child.Traverse(actionNode, actionLeaf);
			}
		}
	}
}
