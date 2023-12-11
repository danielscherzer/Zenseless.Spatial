using OpenTK.Mathematics;
using System.Collections.Generic;
using Zenseless.Spatial;

namespace Example.Rendering;

internal sealed class GridVisual : IVisual
{
	private readonly IReadOnlyGrid<List<int>> grid;

	public GridVisual(IReadOnlyGrid<List<int>> grid, Material material)
	{
		this.grid = grid;
		Material = material;
	}

	public Material Material { get; }

	public Vector2[] CalcPoints()
	{
		var colSize = 2f / grid.Columns;
		var rowSize = 2f / grid.Rows;
		Box2 baseCell = new(0f, 0f, colSize, rowSize);
		List<Box2> cells = new(grid.Columns * grid.Rows);
		for (float x = -1f; x < 1f; x += colSize)
		{
			for (float y = -1f; y < 1f; y += rowSize)
			{
				cells.Add(baseCell.Translated(new Vector2(x, y)));
			}
		}
		return Rendering.CalcPoints(cells, cells.Count);
	}
}
