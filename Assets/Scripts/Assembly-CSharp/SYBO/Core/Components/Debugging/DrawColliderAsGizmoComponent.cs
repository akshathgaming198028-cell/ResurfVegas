using UnityEngine;

namespace SYBO.Core.Components.Debugging
{
	public class DrawColliderAsGizmoComponent : MonoBehaviour
	{
		[SerializeField]
		public SphereCollider _collider;

		protected void Awake()
		{
		}

		protected void Start()
		{
		}

		protected void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(_collider.transform.position, _collider.radius);
		}
	}
}
