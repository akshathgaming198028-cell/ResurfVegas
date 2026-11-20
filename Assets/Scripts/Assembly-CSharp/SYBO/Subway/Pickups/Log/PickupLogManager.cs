using System;
using System.Collections.Generic;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities;
using UnityEngine;

namespace SYBO.Subway.Pickups.Log
{
	public class PickupLogManager : SingletonMonobehavior<PickupLogManager>
	{
		public delegate void OnPickupLogManagerDelegate();

		private static bool _willBeInstantiated;

		public List<PickupLogData> _pickupLogDatas;

		[Obsolete("Use instance.IsInstantiated() instead.")]
		public static bool WillBeInstantiated
		{
			get
			{
				return _willBeInstantiated && Application.isEditor;
			}
		}

		public List<PickupLogData> PickupLogDatas
		{
			get
			{
				return _pickupLogDatas;
			}
		}

		public event OnPickupLogManagerDelegate OnPickupLogDatasChanged;

		public event OnPickupLogManagerDelegate OnPickupLogDatasCleared;

		public void Clear()
		{
			_pickupLogDatas = new List<PickupLogData>();
			if (this.OnPickupLogDatasCleared != null)
			{
				this.OnPickupLogDatasCleared();
			}
		}

		public void Log(PickupLogData pickupLogData)
		{
			_pickupLogDatas.Add(pickupLogData);
			if (this.OnPickupLogDatasChanged != null)
			{
				this.OnPickupLogDatasChanged();
			}
		}

		protected override void SingletonMonobehavior_OnInstantiateCompleted()
		{
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.DynamicManagers);
			Clear();
		}

		private void _DebugLog(string message)
		{
			if (false)
			{
				Debug.Log(message);
			}
		}

		public void Game_OnGameAwake()
		{
			if (SingletonMonobehavior<PickupLogManager>.IsInstantiated())
			{
				SingletonMonobehavior<PickupLogManager>.Instance.Log(new PickupLogData(PickupLogType.SESSION_START));
			}
		}

		public void Game_OnGameStarted()
		{
			_DebugLog("2Game_OnGameStarted");
			if (SingletonMonobehavior<PickupLogManager>.IsInstantiated())
			{
				SingletonMonobehavior<PickupLogManager>.Instance.Log(new PickupLogData(PickupLogType.GAME_START));
			}
		}

		public void Game_OnRunStarted()
		{
			_DebugLog("Game_OnRunStarted");
			if (SingletonMonobehavior<PickupLogManager>.IsInstantiated())
			{
				SingletonMonobehavior<PickupLogManager>.Instance.Log(new PickupLogData(PickupLogType.RUN_START));
			}
		}

		public void Game_OnRunEnded()
		{
			_DebugLog("Game_OnRunEnded");
			if (SingletonMonobehavior<PickupLogManager>.IsInstantiated())
			{
				SingletonMonobehavior<PickupLogManager>.Instance.Log(new PickupLogData(PickupLogType.RUN_STOP));
			}
		}

		public void Game_OnMenuScreenShown()
		{
			_DebugLog("3Game_OnMenuScreenShown  i " + Game.Instance.IsInGame.Value + " m " + Game.Instance.IsInTopMenu.Value);
		}

		public void Game_OnGameEnded()
		{
			if (SingletonMonobehavior<PickupLogManager>.IsInstantiated())
			{
				_DebugLog("4Game_OnGameEnded");
				SingletonMonobehavior<PickupLogManager>.Instance.Log(new PickupLogData(PickupLogType.GAME_STOP));
			}
		}

		public void Game_OnGameDestroyed()
		{
			_DebugLog("Game_OnGameDestroyed");
			if (SingletonMonobehavior<PickupLogManager>.IsInstantiated())
			{
				SingletonMonobehavior<PickupLogManager>.Instance.Log(new PickupLogData(PickupLogType.SESSION_STOP));
			}
		}
	}
}
