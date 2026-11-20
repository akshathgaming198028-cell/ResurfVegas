using System;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities;
using SYBO.Subway.Managers;
using UnityEngine;

namespace SYBO.Subway.Utilities
{
	[PrefabPath("Prefabs/DebugSettings")]
	public class DebugSettings : SingletonMonobehavior<DebugSettings>
	{
		[SerializeField]
		private bool _unlockedSceneCamera = true;

		[SerializeField]
		private bool _characterInvincible;

		[SerializeField]
		private bool _dontUseAssetBundlesInEditor = true;

		[SerializeField]
		private bool _dontUseIdles;

		public static bool IsAllowed
		{
			get
			{
				return (Application.isEditor && Application.isPlaying) || Debug.isDebugBuild;
			}
		}

		public static bool UnlockedSceneCamera
		{
			get
			{
				CheckIsAllowed();
				return SingletonMonobehavior<DebugSettings>.Instance._unlockedSceneCamera;
			}
		}

		public static bool CharacterInvincible
		{
			get
			{
				CheckIsAllowed();
				return SingletonMonobehavior<DebugSettings>.Instance._characterInvincible;
			}
			set
			{
				CheckIsAllowed();
				SingletonMonobehavior<DebugSettings>.Instance._characterInvincible = value;
			}
		}

		public static bool DontUseBaseBundleInEditor
		{
			get
			{
				CheckIsAllowed();
				return SingletonMonobehavior<DebugSettings>.Instance._dontUseAssetBundlesInEditor;
			}
			set
			{
				CheckIsAllowed();
				SingletonMonobehavior<DebugSettings>.Instance._dontUseAssetBundlesInEditor = value;
			}
		}

		public static bool DontUseIdles
		{
			get
			{
				CheckIsAllowed();
				return SingletonMonobehavior<DebugSettings>.Instance._dontUseIdles;
			}
			set
			{
				CheckIsAllowed();
				SingletonMonobehavior<DebugSettings>.Instance._dontUseIdles = value;
			}
		}

		protected override void SingletonMonobehavior_OnInstantiateCompleted()
		{
			base.name = "_DebugSettings";
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.DynamicManagers);
		}

		private static void CheckIsAllowed()
		{
			if (!IsAllowed)
			{
				throw new InvalidOperationException("Use of DebugSetting not allowed");
			}
		}

		protected void Awake()
		{
			if (DemoVideoModeManager.IsEnabled)
			{
				CharacterInvincible = true;
				Debug.LogWarning("[DebugSettings].Awake() CharacterInvincible: " + CharacterInvincible);
			}
		}
	}
}
