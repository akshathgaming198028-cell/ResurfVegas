using UnityEngine;

namespace SYBO.Core.Components
{
	public class SetActiveComponent : MonoBehaviour
	{
		[SerializeField]
		private bool _willBeActiveOnAwake = true;

		protected void Awake()
		{
			base.gameObject.SetActive(_willBeActiveOnAwake);
			base.enabled = _willBeActiveOnAwake;
		}

		protected void Start()
		{
		}
	}
}
