using Example;
using Example.Services;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Zenseless.OpenTK;

GameWindow window = new(GameWindowSettings.Default, new NativeWindowSettings { Profile = ContextProfile.Compatability }); // window with immediate mode rendering enabled
window.VSync = VSyncMode.On;
window.KeyDown += args => { if (args.Key == Keys.Escape) window.Close(); };

Viewport viewport = new();
window.Resize += args => viewport.Resize(args.Width, args.Height);

var renderer = new BoxRenderer();
window.RenderFrame += _ => renderer.Draw();
window.RenderFrame += _ => window.SwapBuffers();

List<GameObject> gameObjects = Scene.CreateObjects(1000);
window.UpdateFrame += args =>
{
	var deltaTime = (float)args.Time;
	gameObjects.ForEach(gameObject => gameObject.Update(deltaTime));
};

var materialGameObject = renderer.Add(new Material(new Color4(1f, 1f, 1f, 0.5f), true));
void RenderGameObjects()
{
	foreach (var gameObject in gameObjects)
	{
		renderer.Enqueue(gameObject.Bounds(), materialGameObject);
	}
}
window.RenderFrame += _ => RenderGameObjects();

ICollisionAlgo algo = new BruteForceCollision();

void ToggleAlgo()
{
	switch(algo)
	{
		case BruteForceCollision bf:
			algo = new QuadtreeCollision(renderer);
			break;
		case QuadtreeCollision qt:
			algo = new GridCollision();
			break;
		case GridCollision g:
			algo = new BruteForceCollision();
			break;
	}
}

double time = 0;
window.UpdateFrame += args => 
{
	time -= args.Time;
	if(time < 0)
	{
		ToggleAlgo();
		window.Title = algo.GetType().Name;
		time = 2.0;
	}
};

var materialCollission = renderer.Add(new Material(Color4.Red, false));
void CheckCollision(ICollisionAlgo algo)
{
	var stopwatch = Stopwatch.StartNew();

	foreach (var collider in algo.Check(gameObjects))
	{
		renderer.Enqueue(collider.Bounds(), materialCollission);
	}
	Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms");
	algo.Render();
}

window.UpdateFrame += _ => CheckCollision(algo);

window.MouseDown += _ => SetMousePoint();
void SetMousePoint()
{
	var m = window.MouseState;
	{
		var p = m.Position.Transform(viewport.InvViewportMatrix);
		var radius = new Vector2(0.01f);
		var area = new Box2(p - radius, p + radius);
		// only insert if no other point is nearby
		if(!gameObjects.Any(go => go.Bounds().Overlaps(area))) gameObjects.Add(new GameObject(p));
	}
}

window.Run();


