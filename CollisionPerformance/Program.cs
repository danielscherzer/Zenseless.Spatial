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

var materialGameObject = renderer.Add(new Material(Color4.White, true));
void RenderGameObjects()
{
	foreach (var gameObject in gameObjects)
	{
		renderer.Enqueue(gameObject.Bounds(), materialGameObject);
	}
}
window.UpdateFrame += _ => RenderGameObjects();

QuadtreeCollission quadTreeCollission = new(renderer);
Func<HashSet<GameObject>> qtColl = () => quadTreeCollission.Check(gameObjects);
Func<HashSet<GameObject>> bfColl = () => Scene.CollissionBruteForce(gameObjects);

Func<HashSet<GameObject>> currentCollisionAlgo = bfColl;
void ToggleAlgo()
{
	if(currentCollisionAlgo == bfColl)
	{
		window.Title = "Quadtree Collision";
		currentCollisionAlgo = qtColl;
	}
	else
	{
		currentCollisionAlgo = bfColl;
		window.Title = "Brute Force Collision";
	}
}
ToggleAlgo();
Stopwatch time = Stopwatch.StartNew();
window.UpdateFrame += _ => 
{
	if(time.Elapsed > TimeSpan.FromSeconds(2))
	{
		ToggleAlgo();
		time = Stopwatch.StartNew();
	}
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
		var p = m.Position.Transform(viewport.InvViewportMatrix);
		var radius = new Vector2(0.01f);
		var area = new Box2(p - radius, p + radius);
		// only insert if no other point is nearby
		if(!quadTreeCollission.QuadTree.Query(area).Any()) gameObjects.Add(new GameObject(p));
	}
}

window.Run();


