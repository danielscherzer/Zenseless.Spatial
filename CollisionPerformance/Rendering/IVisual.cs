using OpenTK.Mathematics;

namespace Example.Rendering;

internal interface IVisual //TODO: Can this be removed and record used?
{
	Material Material { get; }
	Vector2[] CalcPoints();
}