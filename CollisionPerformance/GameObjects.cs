﻿using Example.Core;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using Zenseless.Patterns;

namespace Example;

internal sealed class GameObjects
{
	public static GameObject Create(float x, float y)
	{
		var size = random.NextFloat();
		return new GameObject(x, y, size < 0.99f ? 0.002f : 0.01f)
		{
			Velocity = new Vector2(random.NextFloat(-0.01f, 0.01f), random.NextFloat(-0.01f, 0.01f)),
		};
	}

	public static GameObject Create()
	{
		var x = random.NextFloat(-1f, 1f);
		var y = random.NextFloat(-1f, 1f);
		return Create(x, y);
	}

	internal static void Update(Observable<List<GameObject>> gameObjects, int count)
	{
		var go = gameObjects.HasValue ? gameObjects.Get() : [];
		var diff = go.Count - count;
		if (0 < diff)
		{
			go.RemoveRange(go.Count - diff, diff);
		}
		else
		{
			Add(go, -diff);
		}
		gameObjects.Set(go);
	}

	private static void Add(List<GameObject> gameObjects, int count)
	{
		for (int i = 0; i < count; ++i)
		{
			gameObjects.Add(Create());
		}
	}

	private static readonly Random random = new(12);
}
