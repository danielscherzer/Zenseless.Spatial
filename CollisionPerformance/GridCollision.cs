using Example;
using Example.Spatial;
using System.Collections.Generic;
using Zenseless.Spatial;

internal class GridCollision : ICollisionAlgo
{
	public HashSet<GameObject> Check(IReadOnlyList<GameObject> gameObjects)
	{
		Grid<List<GameObject>> grid = new(10, 10);
		for(int i = 0; i < grid.Cells.Length; ++i)
		{
			grid.Cells[i] = new List<GameObject>();
		}
		//foreach (var gameObject in gameObjects)
		//{
		//	grid[].Insert(gameObject);
		//}
		return new HashSet<GameObject>();
	}

	public void Render()
	{
	}
}