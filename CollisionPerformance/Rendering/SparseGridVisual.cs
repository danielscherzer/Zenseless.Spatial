using Example.Core;
using OpenTK.Mathematics;
using System.Collections.Generic;
using Zenseless.Spatial;

namespace Example.Rendering;

internal sealed class SparseGridVisual : IVisual
{
	public SparseGridVisual(SparseGrid<List<int>> grid, Observable<List<GameObject>> gameObjects, Material material)
	{
		this.grid = grid;
		this.gameObjects = gameObjects;
		Material = material;
	}

	public Material Material { get; }

	public Vector2[] CalcPoints()
	{
		var gos = gameObjects.Get();
		List<Box2> cells = new(grid.Count);
		foreach (var cell in grid)
		{
			var min = Vector2.PositiveInfinity;
			var max = Vector2.NegativeInfinity;
			foreach (var id in cell)
			{
				var bounds = gos[id].Bounds;
				min = Vector2.ComponentMin(min, bounds.Min);
				max = Vector2.ComponentMax(max, bounds.Max);
			}
			Box2 cellBounds = new(min, max);
			cells.Add(cellBounds);
		}
		return Rendering.CalcPoints(cells, cells.Count);
	}

	private readonly SparseGrid<List<int>> grid;
	private readonly Observable<List<GameObject>> gameObjects;
}