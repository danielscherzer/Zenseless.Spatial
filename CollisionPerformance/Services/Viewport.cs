using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;

namespace Example.Services
{
	public class Viewport
	{
		public Vector2 ToNDC(Vector2 windowCoord)
		{
			return (new Vector4(windowCoord.X, windowCoord.Y, 0f, 1f) * _invViewportMatrix).Xy;
		}

		internal void Resize(int width, int height)
		{
			GL.Viewport(0, 0, width, height);
			var translate = Matrix4.CreateTranslation(1f, -1f, 0f);
			var scale = Matrix4.CreateScale(width / 2f, height / -2f, 1f);
			var viewport = translate * scale;
			_invViewportMatrix = viewport.Inverted();
		}

		private Matrix4 _invViewportMatrix = Matrix4.Identity;
	}
}
