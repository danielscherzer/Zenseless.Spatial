using OpenTK.Mathematics;

namespace Example.Rendering;

public readonly record struct Material(Color4 Color, bool Filled)
{
	public static Material Default => new(Color4.White, true);
}
