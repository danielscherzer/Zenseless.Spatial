using Example;
using Example.Services;
using Example.Spatial;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

GameWindow window = new(GameWindowSettings.Default, new NativeWindowSettings { Profile = ContextProfile.Compatability }); // window with immediate mode rendering enabled
window.WindowState = WindowState.Maximized;

Scene scene = new(1000);
window.UpdateFrame += args => scene.Update((float)args.Time);

Viewport viewport = new();
window.Resize += args => viewport.Resize(args.Width, args.Height);

var renderer = new BoxRenderer();
window.RenderFrame += _ => renderer.Draw();
window.RenderFrame += _ => window.SwapBuffers();

QuadtreeCollission quadTreeCollission = new(renderer);


var materialGameObject = renderer.Add(new Material(Color4.White, true));
var materialCollission = renderer.Add(new Material(Color4.Red, false));

scene.AfterUpdate += SceneAfterUpdate;

window.KeyDown += args => { if (args.Key == Keys.Escape) window.Close(); };
window.MouseMove += _ => SetMousePoint();

window.Run();

void SetMousePoint()
{
	var m = window.MouseState;
	if (m.IsAnyButtonDown)
	{
		var p = viewport.ToNDC(m.Position);
		var radius = new Vector2(0.01f);
		var area = new Box2(p - radius, p + radius);
		// only insert if no other point is nearby
		if(!quadTreeCollission.QuadTree.Query(area).Any()) scene.Add(new GameObject(p));
	}
}

HashSet<GameObject> CollissionBruteForce(IReadOnlyList<GameObject> gameObjects)
{
	HashSet<GameObject> colliding = new();
	for (int i = 0; i < gameObjects.Count - 1; ++i)
	{
		for (int j = i + 1; j < gameObjects.Count; ++j)
		{
			var a = gameObjects[i];
			var b = gameObjects[j];
			if (a.Bounds().Intersects(b.Bounds()))
			{
				colliding.Add(a);
				colliding.Add(b);
			}
		}
	}
	return colliding;
}

void SceneAfterUpdate(object? sender, IReadOnlyList<GameObject> gameObjects)
{
	var stopwatch = Stopwatch.StartNew();
	// collission
	var colliding = quadTreeCollission.Check(gameObjects);
	//var colliding2 = CollissionBruteForce(gameObjects);
	window.Title = stopwatch.ElapsedMilliseconds.ToString();

	foreach (var gameObject in gameObjects)
	{
		renderer.Enqueue(gameObject.Bounds(), materialGameObject);
	}
	foreach (var gameObject in colliding)
	{
		renderer.Enqueue(gameObject.Bounds().Scaled(new Vector2(2f), gameObject.Position), materialCollission);
	}
	quadTreeCollission.Render();
}
