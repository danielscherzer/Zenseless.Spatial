using ImGuiNET;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using Zenseless.OpenTK;
using Zenseless.Spatial;

namespace Example.Collision;

internal sealed class SparseGridCollision : ICollisionAlgo
{
	public SparseGridCollision(int columns, int rows)
	{
		var size = new Vector2(columns, rows);
		scale = size / gridBounds.Size;
		cellMax = new Vector2i(columns - 1, rows - 1);
	}

	public void Add(int id, Box2 objectBounds)
	{
		var min = ToGrid(objectBounds.Min);
		var max = ToGrid(objectBounds.Max);
		for (int y = min.Y; y <= max.Y; ++y)
		{
			for (int x = min.X; x <= max.X; ++x)
			{
				grid.CreateOrReturn(x, y, () => []).Add(id);
			}
		}
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
				if (grid.Contains(x,y))
				foreach (var id in grid[x, y])
				{
					collider.Add(id);
				}
			}
		}
		foreach (var id in collider)
		{
			var other = boundsList[id];
			if(other.Overlaps(objectBounds))
			{
				yield return id;
			}
		}
	}

	public Vector2i ToGrid(Vector2 point)
	{
		var p = scale * (point - gridBounds.Min);
		return Vector2i.Clamp((Vector2i)p, Vector2i.Zero, cellMax);
	}

	public void FindCollisions(ICollection<int> colliding, IReadOnlyList<Box2> boundsList)
	{
		grid.Clear();

		// fill grid
		for (int id = 0; id < boundsList.Count; id++)
		{
			var bounds = boundsList[id];
			Add(id, bounds);
		}
		var max = 0;
		var sum = 0;
		colliding.Clear();
		grid.ForEach(cell =>
		{
			max = Math.Max(max, cell.Count);
			sum += cell.Count;
			BruteForceCollision.AddCollisions(colliding, boundsList, cell);
		});
		ImGui.Text($"Cells:{grid.Count}");
		ImGui.Text($"Entries per cell Max={max} Avg={sum / (float)grid.Count:F2}");
	}
	public SparseGrid<List<int>> Grid => grid;

	private readonly SparseGrid<List<int>> grid = new();
	private readonly Box2 gridBounds = new(-1f, -1f, 1f, 1f);
	private readonly Vector2 scale;
	private readonly Vector2i cellMax;
}