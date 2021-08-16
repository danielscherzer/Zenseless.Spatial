using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Example.Services
{
	public class BoxRenderer
	{
		public Handle<Material> Add(Material material)
		{
			_materials.Add(material);
			_materialRects.Add(new List<Box2>());
			return new Handle<Material>(_materials.Count - 1);
		}

		public void Enqueue(Box2 rectangle, Handle<Material> materialHandle)
		{
			try
			{
				var rects = _materialRects[materialHandle.Id];
				rects.Add(rectangle);
			}
			catch (ArgumentOutOfRangeException)
			{
				throw new ApplicationException($"Invalid material id:{materialHandle.Id}.");
			}
		}

		public void Draw()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);

			for (int i = 0; i < _materialRects.Count; ++i)
			{
				var material = _materials[i];
				var rects = _materialRects[i];
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
		}

		private readonly List<List<Box2>> _materialRects = new();
		private readonly List<Material> _materials = new();
	}
}
