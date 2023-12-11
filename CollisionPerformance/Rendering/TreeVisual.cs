using Example.Spatial;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Example.Rendering;

internal sealed class TreeVisual : IVisual
{
	private readonly IQuadtree<int> tree;

	public TreeVisual(IQuadtree<int> tree, Material material)
	{
		this.tree = tree;
		Material = material;
	}

	public Material Material { get; }
	public Vector2[] CalcPoints()
	{
		List<Box2> cells = new();

		void AddBounds(IQuadtree<int> quadTree)
		{
			cells.Add(quadTree.Bounds);
			if (quadTree is IQuadtreeNode<int> node)
				foreach (var child in node.Children)
				{
					AddBounds(child);
				}
		}
		AddBounds(tree);
		return Rendering.CalcPoints(cells, cells.Count);
	}
}
