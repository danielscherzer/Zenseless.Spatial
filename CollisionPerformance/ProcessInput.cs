using Example.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Generic;
using System.Linq;
using Zenseless.OpenTK;

namespace Example;

internal sealed class ProcessInput
{
	internal static bool Keyboard(Keys key, Observable<List<GameObject>> gameObjects, HashSet<int> collisions)
	{
		switch (key)
		{
			case Keys.Escape: return true;
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
		}
		return false;
	}

	internal static void CreateGameObject(Observable<List<GameObject>> gameObjects, Vector2 pixelPosition, Matrix4 invViewportMatrix)
	{
		var p = pixelPosition.Transform(invViewportMatrix);
		var gos = gameObjects.Get();
		var go = GameObjects.Create(p.X, p.Y);
		gos.Add(go);
		gameObjects.Set(gos);
	}
}