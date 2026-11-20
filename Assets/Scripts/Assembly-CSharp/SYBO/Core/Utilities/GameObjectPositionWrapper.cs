using UnityEngine;

namespace SYBO.Core.Utilities
{
	public struct GameObjectPositionWrapper
	{
		public GameObject GameObject;

		public Vector3 Position;

		public GameObjectPositionWrapper(GameObject gameObject)
		{
			GameObject = gameObject;
			Position = gameObject.transform.position;
		}

		public GameObjectPositionWrapper(Component component)
		{
			GameObject = component.gameObject;
			Position = GameObject.transform.position;
		}
	}
}
