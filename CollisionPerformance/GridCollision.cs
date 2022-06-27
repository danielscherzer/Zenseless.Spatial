using Example;
using Example.Services;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using Zenseless.Patterns;
using Zenseless.Spatial;

internal class GridCollision : ICollisionAlgo
{
	const int columns = 60;
	const int rows = 60;
	private readonly BoxRenderer _renderer;
	private readonly Handle<Material> _material;

	public GridCollision(BoxRenderer renderer)
	{
		_renderer = renderer;
		_material = renderer.Add(new Material(Color4.Green, false));
	}

	public HashSet<GameObject> Check(IReadOnlyList<GameObject> gameObjects)
	{
		// create grid
		Grid<List<GameObject>> grid = new(columns, rows);
		for(int i = 0; i < grid.Cells.Length; ++i)
		{
			grid.Cells[i] = new List<GameObject>();
		}
		// fill grid
		foreach (var gameObject in gameObjects)
		{
			var bounds = gameObject.Bounds();
			var boundsMin = new Vector2(0.5f) + 0.5f * bounds.Min;
			var boundsMax = new Vector2(0.5f) + 0.5f * bounds.Max;
			var minX = Math.Max(0, (int)MathF.Floor(boundsMin.X * grid.Columns));
			var minY = Math.Max(0, (int)MathF.Floor(boundsMin.Y) * grid.Rows);
			var maxX = Math.Min(grid.Columns - 1, (int)MathF.Floor(boundsMax.X * grid.Columns));
			var maxY = Math.Min(grid.Rows - 1, (int)MathF.Floor(boundsMax.Y) * grid.Rows);
			for (int y = minY; y <= maxY; ++y)
			{
				for(int x = minX; x <= maxX; ++x)
				{
					grid[x, y].Add(gameObject);
				}
			}
		}
		HashSet<GameObject> colliding = new();
		for (int i = 0; i < grid.Cells.Length; ++i)
		{
			var cell = grid.Cells[i];
			BruteForceCollision.Check(colliding, cell);
		}
		return colliding;
	}

	public void Render()
	{
		var colSize = 2f / columns;
		var rowSize = 2f / rows;
		Box2 cell = new(0f, 0f, colSize, rowSize);
		for(float x = -1f; x < 1f; x += colSize)
		{
			for (float y = -1f; y < 1f; y += rowSize)
			{
				_renderer.Enqueue(cell.Translated(new Vector2(x, y)), _material);
			}
		}
	}
}