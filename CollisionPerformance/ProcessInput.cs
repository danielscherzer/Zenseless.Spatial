using Example;
using Example.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenseless.OpenTK;

internal class ProcessInput
{
	internal static bool Keyboard(Keys key, Observable<BroadPhaseAlgo> broadPhaseAlgo, Observable<List<GameObject>> gameObjects, HashSet<int> collisions, ref bool freeze)
	{
		switch (key)
		{
			case Keys.D1: broadPhaseAlgo.Set(BroadPhaseAlgo.Grid); break;
			case Keys.D2: broadPhaseAlgo.Set(BroadPhaseAlgo.QuadtreePoint); break;
			case Keys.D3: broadPhaseAlgo.Set(BroadPhaseAlgo.QuadTreeRect); break;
			case Keys.D4: broadPhaseAlgo.Set(BroadPhaseAlgo.SparseGrid); break;
			case Keys.Escape: return true;
			case Keys.Down: gameObjects.SetCount(gameObjects.Count() / 2); break;
			case Keys.Up: gameObjects.SetCount(gameObjects.Count() * 2); break;
			case Keys.Left:
				var gos = new List<GameObject>(gameObjects.Get());
				// sort colliding ids and remove top to bottom
				var remove = collisions.OrderByDescending(i => i).Take(collisions.Count / 2);
				foreach (var id in remove)
				{
					gos.RemoveAt(id);
				}
				gameObjects.Set(gos);
				break;
			case Keys.Space: freeze = !freeze; break;
		}
		return false;
	}

	internal static void Mouse(Observable<List<GameObject>> gameObjects, MouseState mouseState, Matrix4 invViewportMatrix)
	{
		var p = mouseState.Position.Transform(invViewportMatrix);
		var radius = new Vector2(0.01f);
		var area = new Box2(p - radius, p + radius);

		// only insert if no other point is nearby
		if (!gameObjects.Get().Any(go => go.Bounds.Overlaps(area)))
		{
			var gos = gameObjects.Get();
			var go = GameObjects.Create(p.X, p.Y);
			gos.Add(go);
			gameObjects.Set(gos);
		}
	}
}