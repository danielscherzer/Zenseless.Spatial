using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Collections.Specialized;
using Zenseless.OpenTK;

namespace Example.Spatial
{
	public abstract class QuadtreeBase<TItem> where TItem : IPosition
	{
		protected QuadtreeBase(in Box2 bounds) => Bounds = bounds;
		public Box2 Bounds { get; }
		public abstract bool NeedSplit { get; }
		public abstract void Insert(TItem item);
		public abstract void Query(Box2 area, ICollection<TItem> results);
	}

	public class QuadtreeLeaf<TItem> : QuadtreeBase<TItem> where TItem : IPosition
	{
		public List<TItem> Items { get; } = new();

		public QuadtreeLeaf(in Box2 bounds) : base(bounds) { }

		public override bool NeedSplit => Items.Count >= 9;

		public override void Insert(TItem item) => Items.Add(item);

		public override void Query(Box2 area, ICollection<TItem> results)
		{
			//if (Box2Extensions.Overlaps(area, Bounds))
			{
				foreach (var item in Items)
				{
					if (area.Contains(item.Position)) results.Add(item);
				}
			}
			//else { }
		}
	}

	public class QuadtreeNode<TItem> : QuadtreeBase<TItem> where TItem : IPosition
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

		public override void Insert(TItem item)
		{
			int index = 0;
			if (item.Position.X > center.X) index += 1;
			if (item.Position.Y > center.Y) index += 2;
			if (Children[index].NeedSplit)
			{
				Split(index);
			}
			Children[index].Insert(item);
		}

		public override void Query(Box2 area, ICollection<TItem> results)
		{
			int whichChildren = 0b1111;
			if (center.X < area.Min.X) whichChildren &= 0b1010;
			else if (center.X > area.Max.X) whichChildren &= 0b0101;

			if (center.Y < area.Min.Y) whichChildren &= 0b1100;
			else if (center.Y > area.Max.Y) whichChildren &= 0b0011;
			for (int i = 0; i < 4; ++i, whichChildren >>= 1)
			{
				if (0 != (whichChildren & 0b0001)) Children[i].Query(area, results);
			}
			//TODO: check loop unrolling in C#
			//if (0 != (whichChildren & 0b0001)) Children[0].Query(area, results);
			//if (0 != (whichChildren & 0b0010)) Children[1].Query(area, results);
			//if (0 != (whichChildren & 0b0100)) Children[2].Query(area, results);
			//if (0 != (whichChildren & 0b1000)) Children[3].Query(area, results);
		}

		private void Split(int i)
		{
			QuadtreeLeaf<TItem> leaf = (QuadtreeLeaf<TItem>)Children[i];
			QuadtreeNode<TItem> node = new(leaf.Bounds);
			// move local items to children
			foreach (var item in leaf.Items)
			{
				node.Insert(item);
			}
			Children[i] = node;
		}
	}
}
