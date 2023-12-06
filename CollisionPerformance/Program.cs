using Example;
using Example.Collision;
using Example.Core;
using Example.Rendering;
using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Zenseless.OpenTK;
using Zenseless.OpenTK.GUI;

// state
Observable<List<GameObject>> gameObjects = new();
Observable<BroadPhaseAlgo> broadPhaseAlgo = new();
bool freeze = true;
HashSet<int> collisions = new();
IVisual? broadPhaseVisual = null;
ICollisionAlgo? algo = null;

// log
Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

//window
GameWindow window = new(GameWindowSettings.Default, ImmediateMode.NativeWindowSettings); // window with immediate mode rendering enabled
var monitor = Monitors.GetMonitorFromWindow(window);
window.Size = (Vector2i)new Vector2(monitor.HorizontalResolution / 1.5f, monitor.VerticalResolution / 1.5f);
window.VSync = VSyncMode.On;
ImGuiFacade gui = new(window);
gui.LoadFontDroidSans(24f);

broadPhaseAlgo.Subscribe(algoType => algo = Setup(algoType));
broadPhaseAlgo.Subscribe(_ => Reset());
gameObjects.Subscribe(_ => Reset());

window.UpdateFrame += args => { if (!freeze) Update.Movement((float)args.Time, gameObjects); };

window.RenderFrame += _ => Rendering.NewFrame();
window.RenderFrame += _ => Rendering.Draw(gameObjects.Get().Select(go => go.Bounds), gameObjects.Count(), new Material(new Color4(1f, 1f, 1f, 0.5f), true));
window.RenderFrame += _ => Rendering.Draw(collisions.Select(id => gameObjects.Get()[id].Bounds), collisions.Count, new Material(Color4.Red, false));
window.RenderFrame += _ => Rendering.Draw(broadPhaseVisual);
window.RenderFrame += _ => Rendering.Reset();
window.RenderFrame += _ => gui.Render(window.ClientSize);
window.RenderFrame += _ => window.SwapBuffers();

Viewport viewport = new();
window.Resize += args => viewport.Resize(args.Width, args.Height);

gameObjects.SetCount(20000);
broadPhaseAlgo.Set(BroadPhaseAlgo.SparseGrid);

ICollisionAlgo Setup(BroadPhaseAlgo broadPhaseAlgo)
{
	switch (broadPhaseAlgo)
	{
		case BroadPhaseAlgo.Grid:
			int colCount = (int)MathF.Sqrt(gameObjects.Count());
			var collGrid = new GridCollision(colCount, colCount);
			broadPhaseVisual = new GridVisual(collGrid.Grid, new Material(Color4.LawnGreen, false));
			return collGrid;
		case BroadPhaseAlgo.SparseGrid:
			int colCount2 = (int)MathF.Sqrt(gameObjects.Count());
			broadPhaseVisual = null;
			return new SparseGridCollision(colCount2, colCount2);
		case BroadPhaseAlgo.QuadtreePoint:
			var collTree = new QuadtreeCollision();
			broadPhaseVisual = new TreeVisual(collTree.Tree, new Material(Color4.CornflowerBlue, false));
			return collTree;
		case BroadPhaseAlgo.QuadTreeRect:
			var collTree2 = new RectQuadtreeCollision();
			broadPhaseVisual = new TreeVisual(collTree2.Tree, new Material(Color4.Goldenrod, false));
			return collTree2;
		default: return new BruteForceCollision();
	}
}

window.KeyDown += args => { if (ProcessInput.Keyboard(args.Key, gameObjects, collisions)) window.Close(); };

double sum = 0;
int count = -50;
void Reset() { count = -5; sum = 0.0; }
void Gui()
{
	ImGui.NewFrame();
	ImGui.SetNextWindowPos(System.Numerics.Vector2.Zero);
	ImGui.Begin("stats", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoDecoration);

	if (algo is null) return;
	var stopwatch = Stopwatch.StartNew();

	var objectCount = gameObjects.Get().Count;
	if(ImGui.SliderInt("Objects", ref objectCount, 1, 100000, "%d", ImGuiSliderFlags.Logarithmic))
	{
		gameObjects.SetCount(objectCount);
	}

	var bounds = gameObjects.Get().Select(go => go.Bounds).ToList();
	algo.FindCollisions(collisions, bounds);
	ImGui.Text($"Collisions:{collisions.Count}");

	++count;
	var current = stopwatch.Elapsed.TotalMilliseconds;
	if (count > 0)
	{
		sum += current;
		ImGui.Text($"AVG:{sum / count:F2}ms");
	}
	ImGui.Text($"last:{current:F2}ms");
	ImGui.Checkbox("Freeze", ref freeze);

	var algoNames = Enum.GetNames(typeof(BroadPhaseAlgo));
	var currentAlgo = (int)broadPhaseAlgo.Get();
	if (ImGui.Combo("Algorithm type", ref currentAlgo, algoNames, algoNames.Length))
	{
		broadPhaseAlgo.Set((BroadPhaseAlgo)currentAlgo);
	}

	ImGui.End();
}

window.UpdateFrame += _ => Gui();

window.MouseDown += _ => ProcessInput.Mouse(gameObjects, window.MouseState, viewport.InvViewportMatrix);

window.Run();
