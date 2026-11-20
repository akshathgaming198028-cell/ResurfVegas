using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities;
using UnityEngine;

namespace SYBO.Core.Managers
{
	public class UnityTimingManager : SingletonMonobehavior<UnityTimingManager>
	{
		[SerializeField]
		public object UnityTimingManagerEditor;

		private int _updateCount;

		protected override void SingletonMonobehavior_OnInstantiateCompleted()
		{
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.DynamicManagers);
		}

		protected void OnEnable()
		{
		}

		protected void OnDisable()
		{
		}

		protected void Awake()
		{
		}

		protected void Start()
		{
			_updateCount = 0;
		}

		protected void Update()
		{
			if (_updateCount++ >= 5)
			{
			}
		}

		protected void OnDestroy()
		{
		}
	}
}
