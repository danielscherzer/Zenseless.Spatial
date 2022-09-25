using System.Collections.Generic;

namespace Example.Collision
{
    internal interface ICollisionAlgo
    {
        //TODO: Remove GameObject dependency
        HashSet<GameObject> Check(IReadOnlyList<GameObject> gameObjects);
        void Render();
    }
}