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
window.VSync = VSyncMode.On;
window.KeyDown += args => { if (args.Key == Keys.Escape) window.Close(); };

Viewport viewport = new();
window.Resize += args => viewport.Resize(args.Width, args.Height);

var renderer = new BoxRenderer();
window.RenderFrame += _ => renderer.Draw();
window.RenderFrame += _ => window.SwapBuffers();


Scene scene = new(1000);
window.UpdateFrame += args => scene.Update((float)args.Time);

var materialGameObject = renderer.Add(new Material(Color4.White, true));
void RenderGameObjects()
{
	foreach (var gameObject in scene.GameObjects)
	{
		renderer.Enqueue(gameObject.Bounds(), materialGameObject);
	}
}
window.UpdateFrame += _ => RenderGameObjects();

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

QuadtreeCollission quadTreeCollission = new(renderer);

Func<HashSet<GameObject>> qtColl = () => quadTreeCollission.Check(scene.GameObjects);
Func<HashSet<GameObject>> bfColl = () => CollissionBruteForce(scene.GameObjects);

Func<HashSet<GameObject>> currentCollisionAlgo = qtColl;
int iterationCount = 0;
window.UpdateFrame += _ => 
{
	if(60 < iterationCount)
	{
		currentCollisionAlgo = bfColl;
		window.Title = "Brute Force Collision...";
	}
	else
	{
		window.Title = "Quadtree Collision...";
		currentCollisionAlgo = qtColl;
	}
	if(200 < iterationCount) iterationCount = 0;
	++iterationCount;
};

var materialCollission = renderer.Add(new Material(Color4.Red, false));
void CheckCollision(Func<HashSet<GameObject>> currentCollisionAlgo)
{
	var stopwatch = Stopwatch.StartNew();
	var colliding = currentCollisionAlgo();
	Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms");

	foreach (var gameObject in colliding)
	{
		renderer.Enqueue(gameObject.Bounds().Scaled(new Vector2(2f), gameObject.Position), materialCollission);
	}
	if (currentCollisionAlgo == qtColl) quadTreeCollission.Render();
}
window.UpdateFrame += _ => CheckCollision(currentCollisionAlgo);

window.MouseDown += _ => SetMousePoint();
void SetMousePoint()
{
	var m = window.MouseState;
	{
		var p = viewport.ToNDC(m.Position);
		var radius = new Vector2(0.01f);
		var area = new Box2(p - radius, p + radius);
		// only insert if no other point is nearby
		if(!quadTreeCollission.QuadTree.Query(area).Any()) scene.Add(new GameObject(p));
	}
}

window.Run();

//void Iterate(Action work)
//{
//	if (!window.Exists) return;
//	for (int i = 0; i < 1000; ++i)
//	{
//		work();
//		window.SwapBuffers();
//		window.ProcessEvents();
//		if(!window.Exists) break;
//	}
//}

//do
//{
//	window.Title = "Quadtree Collision...";
//	Iterate();
//	window.Title = "Brute Force Collision...";
//	Iterate();
//} while (window.Exists);

