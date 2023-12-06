using ImGuiNET;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using Zenseless.Spatial;

namespace Example.Collision;

internal sealed class GridCollision : ICollisionAlgo
{
	public GridCollision(int columns, int rows)
	{
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

	public void FindCollisions(ICollection<int> colliding, IReadOnlyList<Box2> boundsList)
	{
		grid.ForEach(cell => cell.Clear());

		// fill grid
		for (int id = 0; id < boundsList.Count; id++)
		{
			var bounds = boundsList[id];
			var gridBounds = new Box2i(ToGrid(bounds.Min), ToGrid(bounds.Max));
			for (int y = gridBounds.Min.Y; y <= gridBounds.Max.Y; ++y)
			{
				for (int x = gridBounds.Min.X; x <= gridBounds.Max.X; ++x)
				{
					grid[x, y].Add(id);
					//grid.CreateOrReturn(x, y, () => new List<int>()).Add(id);
				}
			}
		}
		var max = 0;
		colliding.Clear();
		grid.ForEach(cell =>
		{
			max = Math.Max(max, cell.Count);
			BruteForceCollision.AddCollisions(colliding, boundsList, cell);
		});
		ImGui.Text($"Maximum entries per cell:{max}");
	}

	public IReadOnlyGrid<List<int>> Grid => grid;

	private readonly Grid<List<int>> grid;
	//private readonly SparseGrid<List<int>> grid;
	private readonly Box2 gridBounds = new(-1f, -1f, 1f, 1f);
}