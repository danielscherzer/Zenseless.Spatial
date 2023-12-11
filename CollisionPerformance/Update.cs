using Example.Core;
using System;
using System.Collections.Generic;

namespace Example;

internal static class Update
{
	internal static int Count(this Observable<List<GameObject>> gameObjects) => gameObjects.Get().Count;

	internal static void SetCount(this Observable<List<GameObject>> gameObjects, int count) => GameObjects.Update(gameObjects, count);

	internal static void Movement(float deltaTime, Observable<List<GameObject>> gameObjects)
	{
		var gos = gameObjects.Get();
		foreach (var go in gos)
		{
			var velocity = go.Velocity;
			var bounds = go.Bounds;
			var center = bounds.Center + deltaTime * velocity;
			//reflect at window edges, so object are always fully visible, because quad tree bounds should stay [-1,1]
			for (int axe = 0; axe < 2; ++axe)
			{
				if (MathF.Abs(center[axe]) >= 1f - go.Radius)
				{
					center[axe] = (1f - go.Radius) * MathF.Sign(velocity[axe]);
					velocity[axe] = -velocity[axe];
				}
			}
			bounds.Center = center;
			go.Velocity = velocity;
			go.Bounds = bounds;

		}
		gameObjects.Set(gos);
	}
}