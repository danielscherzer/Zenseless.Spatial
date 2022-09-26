using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using Zenseless.OpenTK;

namespace Example.Spatial
{
	public abstract class QuadtreeBase<TItem>
	{
		protected QuadtreeBase(in Box2 bounds) => Bounds = bounds;
		public Box2 Bounds { get; }
		public abstract bool NeedSplit { get; }
		public abstract void Insert(Vector2 position, TItem item);
		public abstract void Query(Box2 area, ICollection<TItem> results);
	}

	public class QuadtreeLeaf<TItem> : QuadtreeBase<TItem>
	{
		public List<(Vector2 position, TItem item)> Items { get; } = new();

		public QuadtreeLeaf(in Box2 bounds) : base(bounds) { }

		public override bool NeedSplit => Items.Count >= 8;

		public override void Insert(Vector2 position, TItem item) => Items.Add((position, item));

		public override void Query(Box2 area, ICollection<TItem> results)
		{
			foreach ((Vector2 position, TItem item) in Items)
			{
				if (area.Contains(position)) results.Add(item);
			}
		}
	}

	public class QuadtreeNode<TItem> : QuadtreeBase<TItem>
	{
		private readonly Vector2 center;

		public QuadtreeBase<TItem>[] Children { get; }

		public QuadtreeNode(in Box2 bounds) : base(bounds)
		{
			center = bounds.Center;
			Children = new QuadtreeBase<TItem>[]
			{
				new QuadtreeLeaf<TItem>(new Box2(bounds.Min, center)),
				new QuadtreeLeaf<TItem>(new Box2(center.X, bounds.Min.Y, bounds.Max.X, center.Y)),
				new QuadtreeLeaf<TItem>(new Box2(bounds.Min.X, center.Y, center.X, bounds.Max.Y)),
				new QuadtreeLeaf<TItem>(new Box2(center, bounds.Max)),
			};
		}

		public override bool NeedSplit => false;

		public override void Insert(Vector2 position, TItem item)
		{
			int index = 0;
			if (position.X > center.X) index += 1;
			if (position.Y > center.Y) index += 2;
			var child = Children[index];
			if (child.NeedSplit)
			{
				QuadtreeLeaf<TItem> leaf = (QuadtreeLeaf<TItem>)child;
				child = new QuadtreeNode<TItem>(leaf.Bounds);
				// move local items to children
				foreach ((Vector2 pos, TItem it) in leaf.Items)
				{
						child.Insert(pos, it);
				}
				Children[index] = child;
			}
			child.Insert(position, item);
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
	}
}
