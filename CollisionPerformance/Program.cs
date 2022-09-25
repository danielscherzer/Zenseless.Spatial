using DefaultEcs;
using Example;
using Example.Collision;
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

Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
GameWindow window = new(GameWindowSettings.Default, new NativeWindowSettings { Profile = ContextProfile.Compatability }); // window with immediate mode rendering enabled
var monitor = Monitors.GetPrimaryMonitor();
window.Size = (Vector2i)new Vector2(monitor.HorizontalResolution / 1.5f, monitor.VerticalResolution / 1.5f);
window.VSync = VSyncMode.On;

using World world = new();

Observable<int> objectCount = new();
Observable<List<Vector2>> position = new();
Observable<List<GameObject>> gameObjects = new();
objectCount.Subscribe(count => GameObjects.Update(gameObjects, count));
objectCount.Set(20000);

Viewport viewport = new();
window.Resize += args => viewport.Resize(args.Width, args.Height);

var renderer = new BoxRenderer();
window.RenderFrame += _ => renderer.Draw();
window.RenderFrame += _ => window.SwapBuffers();

bool freeze = true;
window.UpdateFrame += args =>
{
	if (freeze) return;
	var deltaTime = (float)args.Time;
	var go = gameObjects.Get();
	go.ForEach(gameObject => gameObject.Update(deltaTime));
	gameObjects.Set(go);
};

var materialGameObject = renderer.Add(new Material(new Color4(1f, 1f, 1f, 0.5f), true));
void RenderGameObjects()
{
	foreach (var gameObject in gameObjects.Get())
	{
		renderer.Enqueue(gameObject.Bounds(), materialGameObject);
	}
}
window.RenderFrame += _ => RenderGameObjects();

int ColCount() => (int)MathF.Sqrt(objectCount);
int colCount = ColCount();

//ICollisionAlgo algo = new QuadtreeCollision(renderer);
ICollisionAlgo algo = new RectQuadtreeCollision(renderer);
//ICollisionAlgo algo = new IdGridCollision(renderer, ColCount(), ColCount());

window.KeyDown += args =>
{
	Reset();
	switch (args.Key)
	{
		case Keys.D1: algo = new BruteForceCollision(); break;
		case Keys.D2: algo = new GridCollision(renderer, ColCount(), ColCount()); break;
		case Keys.D3: algo = new IdGridCollision(renderer, ColCount(), ColCount()); break;
		case Keys.D4: algo = new QuadtreeCollision(renderer); break;
		case Keys.D5: algo = new RectQuadtreeCollision(renderer); break;
		case Keys.Escape: window.Close(); break;
		case Keys.Down: objectCount.Set(objectCount / 2); break;
		case Keys.Up: objectCount.Set(objectCount * 2); break;
		case Keys.Left:
			var gos = new List<GameObject>(gameObjects.Get());
			var coll = algo.Check(gos);
			foreach(var go in coll.Take(coll.Count / 2))
			{
				gos.Remove(go);
			}
			objectCount.Set(gos.Count);
			gameObjects.Set(gos);
			break;
		case Keys.Space: freeze = !freeze; break;
	}
};

var materialCollission = renderer.Add(new Material(Color4.Red, false));
double sum = 0;
int count = -50;
void Reset() { count = -5; sum = 0.0; }
void CheckCollision(ICollisionAlgo algo)
{
	var stopwatch = Stopwatch.StartNew();
	var gos = gameObjects.Get();
	var collisions = algo.Check(gos);
	foreach (var collider in collisions)
	{
		renderer.Enqueue(collider.Bounds(), materialCollission);
	}

	var message = $"Objects:{gos.Count} {algo.GetType().Name}:Collisions={collisions.Count}";
	++count;

	var current = stopwatch.Elapsed.TotalMilliseconds;
	if (count > 0)
	{
		sum += current;
		message += $" avg:{sum / count:F2}ms";
	}
	message += $" last:{current:F2}ms";

	window.Title = message;
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
		if (!gameObjects.Get().Any(go => go.Bounds().Overlaps(area)))
		{
			objectCount.Set(objectCount + 1);
			var gos = gameObjects.Get();
			gos.Last().Position = p;
			gameObjects.Set(gos);
		}
	}
}

window.Run();
