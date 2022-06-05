using System.Collections.Generic;

namespace Example
{
	internal interface ICollissionAlgo
	{
		HashSet<GameObject> Check(IReadOnlyList<GameObject> gameObjects);
		void Render();
	}
}