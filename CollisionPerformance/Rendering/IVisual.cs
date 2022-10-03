using OpenTK.Mathematics; //TODO: Change to system.numerics

namespace Example.Rendering
{
	internal interface IVisual
	{
		Material Material { get; }
		Vector2[] Points { get; }
	}
}