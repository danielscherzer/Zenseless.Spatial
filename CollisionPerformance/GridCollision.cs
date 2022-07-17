using Example;
using Example.Services;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using Zenseless.OpenTK;
using Zenseless.Patterns;
using Zenseless.Spatial;

internal class GridCollision : ICollisionAlgo
{
	private readonly BoxRenderer _renderer;
	private readonly Handle<Material> _material;
	private readonly Box2 gridBounds = new(-1f, -1f, 1f, 1f);

	public GridCollision(BoxRenderer renderer, int columns, int rows)
	{
		_renderer = renderer;
		_material = renderer.Add(new Material(Color4.Green, false));
		// init grid
		grid = new(columns, rows);
		for (int i = 0; i < grid.Cells.Length; ++i)
		{
			grid.Cells[i] = new List<GameObject>();
		}
	}

	public Vector2i ToGrid(Vector2 point)
	{
		var size = new Vector2(grid.Columns, grid.Rows);
		var cellMax = new Vector2i(grid.Columns - 1, grid.Rows - 1);
		var scale = size / gridBounds.Size;

		var p = scale * (point - gridBounds.Min);
		return Vector2i.Clamp((Vector2i)p, Vector2i.Zero, cellMax);
	}

	public HashSet<GameObject> Check(IReadOnlyList<GameObject> gameObjects)
	{
		for (int i = 0; i < grid.Cells.Length; ++i)
		{
			grid.Cells[i].Clear();
		}
		// fill grid
		foreach (var gameObject in gameObjects)
		{
			var bounds = gameObject.Bounds();
			var gridBounds = new Box2i(ToGrid(bounds.Min), ToGrid(bounds.Max));
			for (int y = gridBounds.Min.Y; y <= gridBounds.Max.Y; ++y)
			{
				for(int x = gridBounds.Min.X; x <= gridBounds.Max.X; ++x)
				{
					grid[x, y].Add(gameObject);
				}
			}
		}
		var max = 0;
		HashSet<GameObject> colliding = new();
		for (int i = 0; i < grid.Cells.Length; ++i)
		{
			var cell = grid.Cells[i];
			max = Math.Max(max, cell.Count);
			BruteForceCollision.Check(colliding, cell);
		}
		Console.WriteLine(max);
		return colliding;
	}

	public void Render()
	{
		var colSize = 2f / grid.Columns;
		var rowSize = 2f / grid.Rows;
		Box2 cell = new(0f, 0f, colSize, rowSize);
		for(float x = -1f; x < 1f; x += colSize)
		{
			for (float y = -1f; y < 1f; y += rowSize)
			{
				_renderer.Enqueue(cell.Translated(new Vector2(x, y)), _material);
			}
		}
	}

	private readonly Grid<List<GameObject>> grid;
}