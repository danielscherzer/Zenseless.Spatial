using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Example.Collision;

internal interface ICollisionAlgo
{
	void FindCollisions(ICollection<int> result, IReadOnlyList<Box2> bounds);
}