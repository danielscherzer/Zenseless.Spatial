using System;
using System.Collections.Generic;

namespace Example
{
	internal class Scene
	{
		private readonly List<GameObject> _gameObjects = new();

		public Scene(int count)
		{
			var random = new Random(12);
			for (int i = 0; i < count; ++i)
			{
				var x = random.Next(-1f, 1f);
				var y = random.Next(-1f, 1f);
				_gameObjects.Add(new GameObject(x, y) { Velocity = 0.01f * random.Vec2() });
			}
		}

		public IReadOnlyList<GameObject> GameObjects => _gameObjects;

		public void Update(float elapsedTime)
		{
			foreach (var gameObject in GameObjects)
			{
				gameObject.Update(elapsedTime);
			}
		}

		internal void Add(GameObject gameObject)
		{
			_gameObjects.Add(gameObject);
		}
	}
}
