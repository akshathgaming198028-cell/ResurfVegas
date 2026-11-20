using SYBO.Subway.Level.Placeholders.PrefabProviders;
using UnityEngine;

namespace SYBO.Subway.Level.Placeholders.Lifecycles
{
	public class MovingTrainPlaceholder : Placeholder
	{
		[SerializeField]
		private float _speed = 1f;

		public float Speed
		{
			get
			{
				return _speed;
			}
		}

		protected override void InitializeSpawnedInstance(Transform objectTransform, string newName)
		{
			base.InitializeSpawnedInstance(objectTransform, newName);
			MovingTrain[] componentsInChildren = _transform.GetComponentsInChildren<MovingTrain>(true);
			if (0 < componentsInChildren.Length)
			{
				componentsInChildren[0].Speed = _speed;
				if (1 < componentsInChildren.Length)
				{
					Debug.LogError("Moving train placeholder loaded multiple MovingTrain chidren - " + Utils.GetLongName(_transform));
				}
			}
			else
			{
				Debug.LogError("Utils could not find moving train in children of " + Utils.GetLongName(_transform));
			}
		}

		public void OnDrawGizmos()
		{
			PlaceholderPrefabProvider componentInChildren = GetComponentInChildren<PlaceholderPrefabProvider>();
			if (!(componentInChildren == null) && !(componentInChildren.Prefab == null))
			{
				BoxCollider[] componentsInChildren = componentInChildren.Prefab.GetComponentsInChildren<BoxCollider>(true);
				if (componentsInChildren != null || 0 >= componentsInChildren.Length)
				{
					Gizmos.color = Color.red;
					Vector3 vector = componentsInChildren[0].size * 0.5f;
					vector.x = 0f;
					Gizmos.DrawWireCube(base.transform.position + vector, componentsInChildren[0].size);
				}
			}
		}
	}
}
