using OpenTK.Mathematics;

namespace Example.Services
{
	public struct Material
	{
		public Material(Color4 color, bool filled) : this()
		{
			Color = color;
			Filled = filled;
		}

		public Color4 Color { get; set; }
		public bool Filled { get; set; }
	}
}
