using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities;
using SYBO.Subway.Characters.States;
using SYBO.Subway.Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace SYBO.Subway.Pickups.Migration
{
	public class PickupsMigration2014Component : MonoBehaviour
	{
		public static string TEMP_ROUTE_CHUNK_SUBSTRING = "routeChunk_default_bonus_short(Clone)/random/ground_run/randomPickup";

		[SerializeField]
		[FormerlySerializedAs("SpawnPointDifficulty")]
		private SpawnPointDifficulty _defaultSpawnPointDifficulty = SpawnPointDifficulty.Easy;

		private TrackObject _selfTrackObject;

		private bool _isSpawnPoint2014Registered;

		public SpawnPointDifficulty SpawnPointDifficulty
		{
			get
			{
				return _defaultSpawnPointDifficulty;
			}
		}

		private bool _isThisPogostickPickup
		{
			get
			{
				if (PogoStickState.IsAPogoStickPickup(base.gameObject))
				{
					return true;
				}
				return false;
			}
		}

		protected void Reset()
		{
			_defaultSpawnPointDifficulty = SpawnPointDifficulty.Easy;
		}

		protected void Awake()
		{
			if (DemoVideoModeManager.IsEnabled)
			{
				base.gameObject.UnparentAndDestroy();
				return;
			}
			if (PickupsMigration.Migration_IsActivePickupSystemVersion2015 && SingletonMonobehavior<SpawnPoolManager>.IsInstantiated())
			{
				GameObject gameObject = SingletonMonobehavior<PickupManager2>.Instance.SpawnPickupSpawnPoint();
				HierarchyUtility.Reparent(gameObject.transform, base.transform.parent, base.transform.localPosition);
				PickupSpawnPoint2 component = gameObject.GetComponent<PickupSpawnPoint2>();
				if (_isThisPogostickPickup)
				{
					component.Migration_Replace2014SpawnPointWith2015SpawnPointForced(base.gameObject, PickupType.PogoStick, SpawnPointDifficulty.Easy, true, false);
					SingletonMonobehavior<PickupManager2>.Instance.CallPickerCollectionUpdate();
				}
				else
				{
					component.Migration_Replace2014SpawnPointWith2015SpawnPoint(base.gameObject, SpawnPointDifficulty);
				}
				base.gameObject.UnparentAndDestroy();
			}
			if (!PickupsMigration.Migration_IsActivePickupSystemVersion2014)
			{
			}
		}

		protected void OnDestroy()
		{
		}

		private void DebugLogBeforeDestroy(string s, GameObject go)
		{
			string text = DebugGetGOName(go);
			Debug.LogWarning(s + text, go);
		}

		private string DebugGetGOName(GameObject go)
		{
			return Utils.GetLongName(go.transform) + "," + go.GetInstanceID();
		}
	}
}
