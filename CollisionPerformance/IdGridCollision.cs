using Example;
using Example.Services;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Zenseless.OpenTK;
using Zenseless.Patterns;
using Zenseless.Spatial;

internal class IdGridCollision : ICollisionAlgo
{
	public IdGridCollision(BoxRenderer renderer, int columns, int rows)
	{
		_renderer = renderer;
		_material = renderer.Add(new Material(Color4.Green, false));
		// init grid
		grid = new(columns, rows);
		grid.ForEach(() => new List<int>());
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
		grid.ForEach(cell => cell.Clear());

		// fill grid
		for (int id = 0; id < gameObjects.Count; id++)
		{
			var bounds = gameObjects[id].Bounds();
			gameObjectBounds.Add(bounds);
			var gridBounds = new Box2i(ToGrid(bounds.Min), ToGrid(bounds.Max));
			for (int y = gridBounds.Min.Y; y <= gridBounds.Max.Y; ++y)
			{
				for(int x = gridBounds.Min.X; x <= gridBounds.Max.X; ++x)
				{
					grid[x, y].Add(id);
				}
			}
		}
		var max = 0;
		HashSet<GameObject> colliding = new();
		grid.ForEach(cell =>
		{
			max = Math.Max(max, cell.Count);
			Check(colliding, gameObjects, gameObjectBounds, cell);
		});
		Debug.WriteLine($"Maximum entries per cell:{max}");
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

	private readonly Grid<List<int>> grid;
	private readonly List<Box2> gameObjectBounds = new();
	private readonly BoxRenderer _renderer;
	private readonly Handle<Material> _material;
	private readonly Box2 gridBounds = new(-1f, -1f, 1f, 1f);

	internal static void Check(HashSet<GameObject> colliding, IReadOnlyList<GameObject> gameObjects, List<Box2> gameObjectBounds, List<int> ids)
	{
		for (int i = 0; i < ids.Count - 1; ++i)
		{
			var idA = ids[i];
			var a = gameObjectBounds[ids[i]];
			for (int j = i + 1; j < ids.Count; ++j)
			{
				var idB = ids[j];
				var b = gameObjectBounds[idB];
				if (Box2Extensions.Overlaps(a, b))
				{
					colliding.Add(gameObjects[idA]);
					colliding.Add(gameObjects[idB]);
				}
			}
		}
	}
}