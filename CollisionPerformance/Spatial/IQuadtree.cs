using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Example.Spatial;

public interface IQuadtree<TItem>
{
	Box2 Bounds { get; }
	void Clear();
	void Query(Box2 area, ICollection<TItem> results);
}

public interface IQuadtreeLeaf<TItem> : IQuadtree<TItem>
{
	IEnumerable<TItem> Items { get; }
}

public interface IQuadtreeNode<TItem> : IQuadtree<TItem>
{
	IQuadtree<TItem>[] Children { get; }
}