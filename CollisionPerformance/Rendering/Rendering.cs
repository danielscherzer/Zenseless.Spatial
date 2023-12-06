using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Example.Rendering
{
	internal static class Rendering
	{
		internal static void Draw(IEnumerable<Box2> boxes, int count, Material material)
		{
			SetMaterial(material);
			var points = CalcPoints(boxes, count);
			Draw(PrimitiveType.Quads, points);
		}

		private static void SetMaterial(Material material)
		{
			GL.Color4(material.Color);
			GL.PolygonMode(MaterialFace.FrontAndBack, material.Filled ? PolygonMode.Fill : PolygonMode.Line);
			if (material.Filled) GL.Enable(EnableCap.Blend); else GL.Disable(EnableCap.Blend);
		}

		internal static void NewFrame()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
		}

		public static void Draw(PrimitiveType type, Vector2[] points)
		{
			GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, points);
			GL.EnableVertexAttribArray(0);
			GL.DrawArrays(type, 0, points.Length);
			GL.DisableVertexAttribArray(0);
		}

		public static Vector2[] CalcPoints(IEnumerable<Box2> boxes, int count)
		{
			Vector2[] points = new Vector2[4 * count];
			int i = 0;
			foreach (var box in boxes)
			{
				var min = box.Min;
				var max = box.Max;
				points[i++] = min;
				points[i++] = new Vector2(min.X, max.Y);
				points[i++] = max;
				points[i++] = new Vector2(max.X, min.Y);
			}
			return points;
		}

		internal static void Draw(IVisual? broadPhaseVisual)
		{
			if (broadPhaseVisual is null) return;
			SetMaterial(broadPhaseVisual.Material);
			Draw(PrimitiveType.Quads, broadPhaseVisual.Points);
			GL.Color4(1f, 1f, 1f, 1f);
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
		}

		internal static void Reset()
		{
			GL.Color4(1f, 1f, 1f, 1f);
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
			GL.Enable(EnableCap.Blend);
		}
	}
}
