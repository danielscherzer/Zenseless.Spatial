using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using Zenseless.Patterns;

namespace Example.Services
{
	public class BoxRenderer
	{
		public Handle<Material> Add(Material material)
		{
			_materials.Add((material, new List<Box2>()));
			return new Handle<Material>(_materials.Count - 1);
		}

		public void Enqueue(Box2 rectangle, Handle<Material> materialHandle)
		{
			try
			{
				var rects = _materials[materialHandle].rects;
				rects.Add(rectangle);
			}
			catch (ArgumentOutOfRangeException e)
			{
				throw new ArgumentOutOfRangeException($"Invalid material id:{materialHandle}.", e);
			}
		}

		//TODO: Use vertex arrays
		public static void DrawQuads(Vector2[] points)
		{
			GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, points);
			GL.EnableVertexAttribArray(0);
			GL.DrawArrays(PrimitiveType.Quads, 0, points.Length);
			GL.DisableVertexAttribArray(0);
		}

		public void Draw()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);

			for (int i = 0; i < _materials.Count; ++i)
			{
				(Material material, List<Box2> rects) = _materials[i];
				GL.Color4(material.Color);
				GL.PolygonMode(MaterialFace.FrontAndBack, material.Filled ? PolygonMode.Fill : PolygonMode.Line);
				GL.Begin(PrimitiveType.Quads);
				foreach (var rect in rects)
				{
					var min = rect.Min;
					var max = rect.Max;
					GL.Vertex2(min);
					GL.Vertex2(min.X, max.Y);
					GL.Vertex2(max);
					GL.Vertex2(max.X, min.Y);
				}
				GL.End();
				rects.Clear();
			}
			GL.Disable(EnableCap.Blend);
		}

		private readonly List<(Material material, List<Box2> rects)> _materials = new();
	}
}
