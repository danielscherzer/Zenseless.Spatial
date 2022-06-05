using System.Collections.Generic;

namespace Example
{
	internal interface ICollisionAlgo
	{
		HashSet<GameObject> Check(IReadOnlyList<GameObject> gameObjects);
		void Render();
	}
}