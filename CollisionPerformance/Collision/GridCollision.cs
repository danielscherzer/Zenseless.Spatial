using ImGuiNET;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using Zenseless.OpenTK;
using Zenseless.Spatial;

namespace Example.Collision;

internal sealed class GridCollision : ICollisionAlgo
{
	public GridCollision(int columns, int rows)
	{
		// init grid
		grid = new(columns, rows);
		grid.ForEach(() => []);
	}

	internal void Add(int id, Box2 objectBounds)
	{
		var min = ToGrid(objectBounds.Min);
		var max = ToGrid(objectBounds.Max);
		for (int y = min.Y; y <= max.Y; ++y)
		{
			for (int x = min.X; x <= max.X; ++x)
			{
				grid[x, y].Add(id);
			}
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

	public IEnumerable<int> FindCollisions(Box2 objectBounds, IReadOnlyList<Box2> boundsList)
	{
		var min = ToGrid(objectBounds.Min);
		var max = ToGrid(objectBounds.Max);
		HashSet<int> collider = [];
		for (int y = min.Y; y <= max.Y; ++y)
		{
			for (int x = min.X; x <= max.X; ++x)
			{
				foreach (var id in grid[x, y])
				{
					collider.Add(id);
				}
			}
		}
		foreach (var id in collider)
		{
			var other = boundsList[id];
			if (other.Overlaps(objectBounds))
			{
				yield return id;
			}
		}
	}

	public void FindCollisions(ICollection<int> colliding, IReadOnlyList<Box2> boundsList)
	{
		grid.ForEach(cell => cell.Clear());

		// fill grid
		for (int id = 0; id < boundsList.Count; id++)
		{
			var objectBounds = boundsList[id];
			var min = ToGrid(objectBounds.Min);
			var max = ToGrid(objectBounds.Max);
			for (int y = min.Y; y <= max.Y; ++y)
			{
				for (int x = min.X; x <= max.X; ++x)
				{
					grid[x, y].Add(id);
				}
			}
		}
		var maximum = 0;
		var sum = 0;
		colliding.Clear();
		grid.ForEach(cell =>
		{
			maximum = Math.Max(maximum, cell.Count);
			sum += cell.Count;
			BruteForceCollision.AddCollisions(colliding, boundsList, cell);
		});
		ImGui.Text($"Cells:{grid.Cells.Length}");
		ImGui.Text($"Entries per cell Max={maximum} Avg={sum / (float)grid.Cells.Length:F2}");
	}

	public IReadOnlyGrid<List<int>> Grid => grid;

	private readonly Grid<List<int>> grid;
	private readonly Box2 gridBounds = new(-1f, -1f, 1f, 1f);
}