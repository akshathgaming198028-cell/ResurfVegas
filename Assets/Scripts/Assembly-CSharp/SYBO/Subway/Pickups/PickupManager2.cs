using System;
using System.Collections.Generic;
using PathologicalGames;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities;
using SYBO.Core.Utilities.Flags;
using SYBO.Subway.Managers;
using SYBO.Subway.Pickups.Migration;
using SYBO.Subway.Pickups.Picker.Pickers;
using SYBO.Subway.Pickups.Pickers;
using UnityEngine;

namespace SYBO.Subway.Pickups
{
	[PrefabPath("Prefabs/Managers/PickupManager")]
	public class PickupManager2 : SingletonMonobehavior<PickupManager2>
	{
		private enum ReasonToResetHistory
		{
			SESSION_STARTED = 0,
			GAME_STARTED = 1,
			RUN_STARTED = 2
		}

		public delegate void OnPickupDelegate(PickupData2 pickupData, SpawnPointMode spawnPointMode, PickupSpawnPoint2 spawnPoint);

		public delegate void OnPickupSpawnPointDelegate(PickupSpawnPoint2 pickupSpawnPoint);

		public const ActivePickupSystemVersion DEFAULT_PICKUP_SYSTEM_VERSION = ActivePickupSystemVersion.Version2015;

		private const int _PICKER_TYPE_SHORT_NAME_LENGTH = 8;

		private const int _PICKUP_TYPE_SHORT_NAME_LENGTH = 9;

		private const int _DELAY_BEFORE_UPDATING_PICKER_COLLECTION_COUNT_MAX = 60;

		[SerializeField]
		[Tooltip("Show Gizmos in SceneView for PickupSpawnPoint")]
		private bool _willShowPickupSpawnPointGizmos;

		[SerializeField]
		[Tooltip("Show Debug.Logs to Console")]
		private bool _willDebugLogPickers;

		[SerializeField]
		[Tooltip("Show Debug.Logs to Console")]
		private bool _willDebugLogPickupManager;

		[Tooltip("Show Debug.Logs to Console")]
		[SerializeField]
		private bool _willDebugLogPickupData;

		[Tooltip("Show Debug.Logs to Console")]
		[SerializeField]
		private bool _willDebugLogPickerData;

		[Tooltip("Show Debug.LogError to Console")]
		[SerializeField]
		private bool _willDebugLogBadCollisions;

		[Tooltip("True for production. Remove property for production")]
		[SerializeField]
		private bool _willRewardUponPickupCollected;

		[SerializeField]
		internal GameObject _pickupSpawnPointPrefab;

		[SerializeField]
		private PickupManager2ConfigurationSO _pickupManagerConfiguration;

		public static float SpawnDistanceMinToCharacter = 112.5f;

		[Tooltip("Spawning: Minimum distance between ex. CoinMagnet and the start of the game. 0 = Spawn more closely. 900 = spawn only further than the skyline plane.")]
		[SerializeField]
		[Range(0f, 10000f)]
		private float _spawnDistanceMinToGameStart = 360f;

		private Dictionary<PickupType, float> _shortHistoryOfLastZPositionPerPickupTypeDictionary;

		private Dictionary<PickerType, float> _shortHistoryOfLastZPositionPerPickerTypeDictionary;

		[SerializeField]
		[Tooltip("The list of all possible pickup items.")]
		internal List<PickupData2> _pickupDataList;

		[Tooltip("The list of pickers which choose the items to spawn.")]
		[SerializeField]
		public List<PickerData2> _pickerDataList;

		private List<PickupSpawnPoint2> _pickupSpawnPointList;

		internal PickerCollection _pickerCollection;

		private int _delayBeforeUpdatingPickerCollectionCount;

		public bool WillShowPickupSpawnPointGizmos
		{
			get
			{
				return _willShowPickupSpawnPointGizmos;
			}
		}

		public bool WillDebugLogPickers
		{
			get
			{
				return _willDebugLogPickers;
			}
		}

		public bool WillDebugLogPickupManager
		{
			get
			{
				return _willDebugLogPickupManager;
			}
		}

		public bool WillDebugLogPickupData
		{
			get
			{
				return _willDebugLogPickupData;
			}
		}

		public bool WillDebugLogPickerData
		{
			get
			{
				return _willDebugLogPickerData;
			}
		}

		public bool WillDebugLogBadCollisions
		{
			get
			{
				return _willDebugLogBadCollisions;
			}
		}

		public bool WillRewardUponPickupCollected
		{
			get
			{
				return _willRewardUponPickupCollected;
			}
		}

		public float SpawnDistanceMinToGameStart
		{
			get
			{
				return _spawnDistanceMinToGameStart;
			}
		}

		public static ActivePickupSystemVersion ActivePickupSystemVersion
		{
			get
			{
				ActivePickupSystemVersion result = PlayerInfo.Instance.CurrentPickupSystemVersion;
				if (Globals.DEBUG_OVERWRITE_PICKUP_SYSTEM_VERSION)
				{
					if (SingletonMonobehavior<PickupManager2>.IsInstantiated() && SingletonMonobehavior<PickupManager2>.Instance._pickupManagerConfiguration != null)
					{
						result = SingletonMonobehavior<PickupManager2>.Instance._pickupManagerConfiguration._activePickupSystemVersion;
					}
					else
					{
						PickupManager2ConfigurationSO pickupManager2ConfigurationSO = (PickupManager2ConfigurationSO)Resources.Load("Prefabs/Pickups/Refactored/PickupManager2ConfigurationSO");
						result = pickupManager2ConfigurationSO._activePickupSystemVersion;
					}
				}
				return result;
			}
		}

		public static bool WillBeInstantiatedByGameClass
		{
			get
			{
				return PickupsMigration.Migration_IsActivePickupSystemVersion2015;
			}
		}

		public Dictionary<PickupType, float> ShortHistoryOfLastZPositionPerPickupTypeDictionary
		{
			get
			{
				return _shortHistoryOfLastZPositionPerPickupTypeDictionary;
			}
		}

		public Dictionary<PickerType, float> ShortHistoryOfLastZPositionPerPickerTypeDictionary
		{
			get
			{
				return _shortHistoryOfLastZPositionPerPickerTypeDictionary;
			}
		}

		internal List<PickupSpawnPoint2> PickupSpawnPointList
		{
			get
			{
				return _pickupSpawnPointList;
			}
		}

		public PickerCollection PickerCollection
		{
			get
			{
				return _pickerCollection;
			}
		}

		public event OnPickupDelegate OnPickupSpawned;

		public event OnPickupDelegate OnPickupDespawned;

		public event OnPickupDelegate OnPickupCollected;

		public event OnPickupSpawnPointDelegate OnPickupSpawnPointRegistered;

		public event OnPickupSpawnPointDelegate OnPickupSpawnPointDeregistered;

		public event Action<PickerRemovedEvent> OnPickerRemoved;

		public bool HasShortHistoryOfLastZPositionForPickupType(PickupType pickupType)
		{
			return _shortHistoryOfLastZPositionPerPickupTypeDictionary != null && _shortHistoryOfLastZPositionPerPickupTypeDictionary.ContainsKey(pickupType);
		}

		public float GetShortHistoryOfLastZPositionForPickupType(PickupType pickupType)
		{
			return _shortHistoryOfLastZPositionPerPickupTypeDictionary[pickupType];
		}

		public void SetShortHistoryOfLastZPositionForPickupType(PickupType pickupType, float value)
		{
			_shortHistoryOfLastZPositionPerPickupTypeDictionary[pickupType] = value;
		}

		public void AddShortHistoryOfLastZPositionForPickupType(PickupType pickupType, float value)
		{
			_shortHistoryOfLastZPositionPerPickupTypeDictionary.Add(pickupType, value);
		}

		public static float GetCurrentTimestamp()
		{
			return Time.timeSinceLevelLoad;
		}

		public bool HasShortHistoryOfLastZPositionForPickerType(PickerType pickerType)
		{
			return _shortHistoryOfLastZPositionPerPickerTypeDictionary != null && _shortHistoryOfLastZPositionPerPickerTypeDictionary.ContainsKey(pickerType);
		}

		public float GetShortHistoryOfLastZPositionForPickerType(PickerType pickerType)
		{
			return _shortHistoryOfLastZPositionPerPickerTypeDictionary[pickerType];
		}

		public void SetShortHistoryOfLastZPositionForPickerType(PickerType pickerType, float value)
		{
			_shortHistoryOfLastZPositionPerPickerTypeDictionary[pickerType] = value;
		}

		public void AddShortHistoryOfLastZPositionForPickerType(PickerType pickerType, float value)
		{
			_shortHistoryOfLastZPositionPerPickerTypeDictionary.Add(pickerType, value);
		}

		public PickupData2 GetPickupDataForPickupType(PickupType pickupType)
		{
			if (HasPickupDataForPickupType(pickupType))
			{
				return _pickupDataList.Find((PickupData2 pd) => pd.PickupType == pickupType);
			}
			return null;
		}

		public bool HasPickupDataForPickupType(PickupType pickupType)
		{
			if (_pickupDataList != null && _pickupDataList.Find((PickupData2 pd) => pd.PickupType == pickupType) != null)
			{
				return true;
			}
			return false;
		}

		internal PickerData2 GetPickerDataForPickerType(PickerType pickerType)
		{
			PickerData2 result = null;
			foreach (PickerData2 pickerData in _pickerDataList)
			{
				if (pickerData.PickerType == pickerType)
				{
					result = pickerData;
					break;
				}
			}
			return result;
		}

		protected void FixedUpdate()
		{
			if (Game.IsInstantiated() && Game.Instance.IsInGame.Value && !Game.Instance.IsPaused)
			{
				CallPickerCollectionUpdate(false);
			}
		}

		private void CallPickerCollectionUpdate(bool willForceUpdate)
		{
			if (willForceUpdate)
			{
				_delayBeforeUpdatingPickerCollectionCount = 60;
			}
			if (++_delayBeforeUpdatingPickerCollectionCount < 60)
			{
				return;
			}
			_delayBeforeUpdatingPickerCollectionCount = 0;
			if (PickupsMigration.Migration_IsActivePickupSystemVersion2015)
			{
				_pickerCollection.DoUpdate();
			}
			foreach (PickupSpawnPoint2 pickupSpawnPoint in _pickupSpawnPointList)
			{
				if (pickupSpawnPoint.IsAvailableForSpawning())
				{
					pickupSpawnPoint.DoMarkAsUnavailable();
				}
			}
		}

		protected void OnDestroy()
		{
			if (_pickerCollection != null)
			{
				_pickerCollection.OnPickerRemoveRequest -= _PickerCollection_OnPickerRemoveRequest;
			}
		}

		private static int SortAfterAdd(PickupSpawnPoint2 pickupSpawnPoint1, PickupSpawnPoint2 pickupSpawnPoint2)
		{
			return pickupSpawnPoint1.transform.position.z.CompareTo(pickupSpawnPoint2.transform.position.z);
		}

		public static object ConvertPickupTypeFlagsToString(PickupType pickupType)
		{
			string empty = string.Empty;
			empty += "[";
			string text = empty;
			empty = text + ConvertPickupTypeToShortString(PickupType.CoinMagnet) + "=" + ConvertBoolTo01String(FlagsHelper.IsSet(pickupType, PickupType.CoinMagnet)) + ",";
			text = empty;
			empty = text + ConvertPickupTypeToShortString(PickupType.JetPack) + "=" + ConvertBoolTo01String(FlagsHelper.IsSet(pickupType, PickupType.JetPack)) + ",";
			text = empty;
			empty = text + ConvertPickupTypeToShortString(PickupType.Key) + "=" + ConvertBoolTo01String(FlagsHelper.IsSet(pickupType, PickupType.Key)) + ",";
			text = empty;
			empty = text + ConvertPickupTypeToShortString(PickupType.LetterHuntDaily) + "=" + ConvertBoolTo01String(FlagsHelper.IsSet(pickupType, PickupType.LetterHuntDaily)) + ",";
			text = empty;
			empty = text + ConvertPickupTypeToShortString(PickupType.MultiplierScore2x) + "=" + ConvertBoolTo01String(FlagsHelper.IsSet(pickupType, PickupType.MultiplierScore2x)) + ",";
			text = empty;
			empty = text + ConvertPickupTypeToShortString(PickupType.MysteryBox) + "=" + ConvertBoolTo01String(FlagsHelper.IsSet(pickupType, PickupType.MysteryBox)) + ",";
			text = empty;
			empty = text + ConvertPickupTypeToShortString(PickupType.MysteryPowerup) + "=" + ConvertBoolTo01String(FlagsHelper.IsSet(pickupType, PickupType.MysteryBox)) + ",";
			text = empty;
			empty = text + ConvertPickupTypeToShortString(PickupType.PogoStick) + "=" + ConvertBoolTo01String(FlagsHelper.IsSet(pickupType, PickupType.PogoStick)) + ",";
			text = empty;
			empty = text + ConvertPickupTypeToShortString(PickupType.SuperMysteryBox) + "=" + ConvertBoolTo01String(FlagsHelper.IsSet(pickupType, PickupType.SuperMysteryBox)) + ",";
			text = empty;
			empty = text + ConvertPickupTypeToShortString(PickupType.SuperSneakers) + "=" + ConvertBoolTo01String(FlagsHelper.IsSet(pickupType, PickupType.SuperSneakers)) + ",";
			text = empty;
			empty = text + ConvertPickupTypeToShortString(PickupType.TokenHunt) + "=" + ConvertBoolTo01String(FlagsHelper.IsSet(pickupType, PickupType.TokenHunt)) + ",";
			return empty + "]";
		}

		public static string ConvertPickupTypeToShortString(PickupType pickupType)
		{
			string text = string.Empty;
			switch (pickupType)
			{
			case PickupType.CoinMagnet:
				text = "CoinM";
				break;
			case PickupType.JetPack:
				text = "JetP";
				break;
			case PickupType.Key:
				text = "Key";
				break;
			case PickupType.LetterHuntDaily:
				text = "LettD";
				break;
			case PickupType.LetterHuntCharacter:
				text = "LettC";
				break;
			case PickupType.MultiplierScore2x:
				text = "Mult2x";
				break;
			case PickupType.MysteryBox:
				text = "MystB";
				break;
			case PickupType.MysteryPowerup:
				text = "MystP";
				break;
			case PickupType.PogoStick:
				text = "PogoSt";
				break;
			case PickupType.SuperMysteryBox:
				text = "SMystB";
				break;
			case PickupType.SuperSneakers:
				text = "SSneaks";
				break;
			case PickupType.TokenHunt:
				text = "TokenH";
				break;
			}
			if (text.Length > 9)
			{
				text = text.Substring(0, 9);
			}
			return text.PadRight(9, '_');
		}

		public static string ConvertPickerTypeToShortString(PickerType pickerType)
		{
			string text = string.Empty;
			switch (pickerType)
			{
			case PickerType.Default:
				text = "Default";
				break;
			case PickerType.LetterHuntDaily:
				text = "LetterHuntD";
				break;
			case PickerType.SuperMysteryBox:
				text = "SMBox";
				break;
			case PickerType.MysteryPowerUp:
				text = "MystPow";
				break;
			case PickerType.TokenHunt:
				text = "TokenHunt";
				break;
			case PickerType.XTest:
				text = "XTest";
				break;
			}
			if (text.Length > 8)
			{
				text = text.Substring(0, 8);
			}
			return text.PadRight(8, '_');
		}

		public static string ConvertBoolTo01String(bool b)
		{
			if (b)
			{
				return "1";
			}
			return "0";
		}

		public void CallPickerCollectionUpdate()
		{
			CallPickerCollectionUpdate(true);
		}

		public void RegisterPickupSpawnPoint(PickupSpawnPoint2 pickupSpawnPoint)
		{
			if (!_pickupSpawnPointList.Contains(pickupSpawnPoint))
			{
				_pickupSpawnPointList.Add(pickupSpawnPoint);
				_pickupSpawnPointList.Sort(SortAfterAdd);
				pickupSpawnPoint.OnPickupSpawned += _PickupSpawnPoint_OnPickupSpawned;
				pickupSpawnPoint.OnPickupCollected += _PickupSpawnPoint_OnPickupCollected;
				pickupSpawnPoint.OnPickupDespawned += _PickupSpawnPoint_OnPickupDespawned;
				pickupSpawnPoint.OnRegistered += _PickupSpawnPoint_OnRegistered;
				pickupSpawnPoint.OnDeregistered += _PickupSpawnPoint_OnDeregistered;
				pickupSpawnPoint.DoRegister();
			}
		}

		public void DeregisterPickupSpawnPoint(PickupSpawnPoint2 pickupSpawnPoint)
		{
			if (_pickupSpawnPointList.Contains(pickupSpawnPoint))
			{
				_pickupSpawnPointList.Remove(pickupSpawnPoint);
			}
			pickupSpawnPoint.DoDeregister();
		}

		public GameObject SpawnPickupSpawnPoint()
		{
			return Spawn(_pickupSpawnPointPrefab.transform, new Vector3(0f, 0f, 0f), Quaternion.identity);
		}

		public void Despawn(Transform transformToDespawn)
		{
			if (HasSpawnPool())
			{
				GetSpawnPool().Despawn(transformToDespawn);
			}
			else
			{
				Debug.LogError("Despawn() Failed. No pool exists. Fix immediately.");
			}
		}

		public bool IsSpawned(Transform transformToDespawn)
		{
			if (HasSpawnPool())
			{
				return GetSpawnPool().IsSpawned(transformToDespawn);
			}
			Debug.LogError("Despawn() Failed. No pool exists. Fix immediately.");
			return false;
		}

		public GameObject Spawn(Transform transformToSpawn, Vector3 position, Quaternion rotation)
		{
			GameObject result = null;
			if (HasSpawnPool())
			{
				result = GetSpawnPool().Spawn(transformToSpawn).gameObject;
			}
			else
			{
				Debug.LogError("Spawn() Failed. No pool exists. Fix immediately.");
			}
			return result;
		}

		private bool HasSpawnPool()
		{
			return SingletonMonobehavior<SpawnPoolManager>.IsInstantiated() && SingletonMonobehavior<SpawnPoolManager>.Instance.HasSpawnPool(SpawnPoolType.Pickups);
		}

		private SpawnPool GetSpawnPool()
		{
			return SingletonMonobehavior<SpawnPoolManager>.Instance.GetSpawnPool(SpawnPoolType.Pickups);
		}

		private void _ResetShortHistoryOfLastZPositionForBoth(ReasonToResetHistory reasonToResetHistory)
		{
			_ResetShortHistoryOfLastZPositionPerPickupTypeDictionary(reasonToResetHistory);
			_ResetShortHistoryOfLastZPositionPerPickerTypeDictionary(reasonToResetHistory);
		}

		private void _ResetShortHistoryOfLastZPositionPerPickupTypeDictionary(ReasonToResetHistory reasonToResetHistory)
		{
			if (_shortHistoryOfLastZPositionPerPickupTypeDictionary == null)
			{
				_shortHistoryOfLastZPositionPerPickupTypeDictionary = new Dictionary<PickupType, float>();
			}
			switch (reasonToResetHistory)
			{
			case ReasonToResetHistory.SESSION_STARTED:
			{
				foreach (PickupData2 pickupData in _pickupDataList)
				{
					if (pickupData.SpawnDistanceMinToPreviousMode == SpawnDistanceMinToPreviousMode.ResetAtSessionStart)
					{
						if (HasShortHistoryOfLastZPositionForPickupType(pickupData.PickupType))
						{
							SetShortHistoryOfLastZPositionForPickupType(pickupData.PickupType, 0f);
						}
						else
						{
							AddShortHistoryOfLastZPositionForPickupType(pickupData.PickupType, 0f);
						}
					}
				}
				break;
			}
			case ReasonToResetHistory.RUN_STARTED:
			{
				foreach (PickupData2 pickupData2 in _pickupDataList)
				{
					if (pickupData2.SpawnDistanceMinToPreviousMode == SpawnDistanceMinToPreviousMode.ResetAtRunStart)
					{
						if (HasShortHistoryOfLastZPositionForPickupType(pickupData2.PickupType))
						{
							SetShortHistoryOfLastZPositionForPickupType(pickupData2.PickupType, 0f);
						}
						else
						{
							AddShortHistoryOfLastZPositionForPickupType(pickupData2.PickupType, 0f);
						}
					}
				}
				break;
			}
			}
		}

		private void _ResetShortHistoryOfLastZPositionPerPickerTypeDictionary(ReasonToResetHistory reasonToResetHistory)
		{
			if (_shortHistoryOfLastZPositionPerPickerTypeDictionary == null)
			{
				_shortHistoryOfLastZPositionPerPickerTypeDictionary = new Dictionary<PickerType, float>();
			}
			switch (reasonToResetHistory)
			{
			case ReasonToResetHistory.SESSION_STARTED:
			{
				foreach (PickerData2 pickerData in _pickerDataList)
				{
					if (pickerData.SpawnDistanceMinToPreviousMode == SpawnDistanceMinToPreviousMode.ResetAtSessionStart)
					{
						if (HasShortHistoryOfLastZPositionForPickerType(pickerData.PickerType))
						{
							SetShortHistoryOfLastZPositionForPickerType(pickerData.PickerType, 0f);
						}
						else
						{
							AddShortHistoryOfLastZPositionForPickerType(pickerData.PickerType, 0f);
						}
					}
				}
				break;
			}
			case ReasonToResetHistory.RUN_STARTED:
			{
				foreach (PickerData2 pickerData2 in _pickerDataList)
				{
					if (pickerData2.SpawnDistanceMinToPreviousMode == SpawnDistanceMinToPreviousMode.ResetAtRunStart)
					{
						if (HasShortHistoryOfLastZPositionForPickerType(pickerData2.PickerType))
						{
							SetShortHistoryOfLastZPositionForPickerType(pickerData2.PickerType, 0f);
						}
						else
						{
							AddShortHistoryOfLastZPositionForPickerType(pickerData2.PickerType, 0f);
						}
					}
				}
				break;
			}
			}
		}

		protected override void SingletonMonobehavior_OnInstantiateCompleted()
		{
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.DynamicManagers);
			_ResetShortHistoryOfLastZPositionForBoth(ReasonToResetHistory.SESSION_STARTED);
			_pickupSpawnPointList = new List<PickupSpawnPoint2>();
			_pickerCollection = new PickerCollection(this);
			_pickerCollection.OnPickerRemoveRequest += _PickerCollection_OnPickerRemoveRequest;
			if (!PickupsMigration.Migration_IsActivePickupSystemVersion2015)
			{
				return;
			}
			foreach (PickerData2 pickerData in _pickerDataList)
			{
				if (pickerData.PickerEnabledState == PickerEnabledState.Enabled)
				{
					_DebugLog(string.Concat("PM Adding : ", pickerData.PickerType, " with ", pickerData.PickerEnabledState));
					switch (pickerData.PickerType)
					{
					case PickerType.Default:
						_pickerCollection.Add(new DefaultPicker());
						break;
					case PickerType.LetterHuntDaily:
						_pickerCollection.Add(new LetterHuntDailyPicker());
						break;
					case PickerType.TokenHunt:
						_pickerCollection.Add(new TokenHuntPicker());
						break;
					case PickerType.SuperMysteryBox:
						_pickerCollection.Add(new SuperMysteryBoxPicker());
						break;
					case PickerType.MysteryPowerUp:
						_pickerCollection.Add(new MysteryPowerupPicker());
						break;
					case PickerType.XTest:
						_pickerCollection.Add(new XTestPicker());
						break;
					default:
						throw new UnityException("Fix Immediately. Add switch case for " + pickerData.PickerType);
					}
				}
			}
		}

		private void _DebugLog(string message)
		{
			if (SingletonMonobehavior<PickupManager2>.IsInstantiated() && SingletonMonobehavior<PickupManager2>.Instance.WillDebugLogPickupManager)
			{
				Debug.Log(message);
			}
		}

		public void Game_OnGameStarted()
		{
			_ResetShortHistoryOfLastZPositionForBoth(ReasonToResetHistory.GAME_STARTED);
		}

		public void Game_OnRunStarted()
		{
			_ResetShortHistoryOfLastZPositionForBoth(ReasonToResetHistory.RUN_STARTED);
		}

		private void _PickerCollection_OnPickerRemoveRequest(PickerRemoveRequestEvent pickerRemoveRequestEvent)
		{
			_pickerCollection.Remove(pickerRemoveRequestEvent.IPicker);
			if (this.OnPickerRemoved != null)
			{
				this.OnPickerRemoved(new PickerRemovedEvent(pickerRemoveRequestEvent.IPicker));
			}
		}

		private void _PickupSpawnPoint_OnPickupSpawned(PickupSpawnPoint2 pickupSpawnPoint, PickerData2 pickerData)
		{
			if (pickupSpawnPoint.SpawnPointMode != SpawnPointMode.WillForcePickupType)
			{
				PickupType pickupType = pickupSpawnPoint.PickupData.PickupType;
				if (pickupSpawnPoint.PickupData.SpawnDistanceMinToPrevious > 0f)
				{
					if (_shortHistoryOfLastZPositionPerPickupTypeDictionary.ContainsKey(pickupType))
					{
						_shortHistoryOfLastZPositionPerPickupTypeDictionary.Remove(pickupType);
					}
					AddShortHistoryOfLastZPositionForPickupType(pickupType, pickupSpawnPoint.transform.position.z);
					_DebugLog(string.Concat("1 HISTORY ADD : ", pickupType, " z: ", GetShortHistoryOfLastZPositionForPickupType(pickupType)));
				}
				if (pickerData.SpawnDistanceMinToPrevious > 0f)
				{
					if (_shortHistoryOfLastZPositionPerPickerTypeDictionary.ContainsKey(pickerData.PickerType))
					{
						_shortHistoryOfLastZPositionPerPickerTypeDictionary.Remove(pickerData.PickerType);
					}
					AddShortHistoryOfLastZPositionForPickerType(pickerData.PickerType, pickupSpawnPoint.transform.position.z);
					_DebugLog(string.Concat("2 HISTORY ADD : ", pickupType, " z: ", GetShortHistoryOfLastZPositionForPickerType(pickerData.PickerType)));
				}
			}
			if (this.OnPickupSpawned != null)
			{
				this.OnPickupSpawned(pickupSpawnPoint.PickupData, pickupSpawnPoint.SpawnPointMode, pickupSpawnPoint);
			}
		}

		private void _PickupSpawnPoint_OnPickupCollected(PickupSpawnPoint2 pickupSpawnPoint)
		{
			if (this.OnPickupCollected != null)
			{
				this.OnPickupCollected(pickupSpawnPoint.PickupData, pickupSpawnPoint.SpawnPointMode, pickupSpawnPoint);
			}
		}

		private void _PickupSpawnPoint_OnPickupDespawned(PickupSpawnPoint2 pickupSpawnPoint)
		{
			if (this.OnPickupDespawned != null)
			{
				this.OnPickupDespawned(pickupSpawnPoint.PickupData, pickupSpawnPoint.SpawnPointMode, pickupSpawnPoint);
			}
		}

		private void _PickupSpawnPoint_OnRegistered(PickupSpawnPoint2 pickupSpawnPoint)
		{
			if (pickupSpawnPoint.SpawnPointMode == SpawnPointMode.WillForcePickupType)
			{
			}
			if (this.OnPickupSpawnPointRegistered != null)
			{
				this.OnPickupSpawnPointRegistered(pickupSpawnPoint);
			}
		}

		private void _PickupSpawnPoint_OnDeregistered(PickupSpawnPoint2 pickupSpawnPoint)
		{
			if (pickupSpawnPoint.SpawnPointMode == SpawnPointMode.WillForcePickupType)
			{
			}
			pickupSpawnPoint.OnPickupSpawned -= _PickupSpawnPoint_OnPickupSpawned;
			pickupSpawnPoint.OnPickupCollected -= _PickupSpawnPoint_OnPickupCollected;
			pickupSpawnPoint.OnPickupDespawned -= _PickupSpawnPoint_OnPickupDespawned;
			pickupSpawnPoint.OnRegistered -= _PickupSpawnPoint_OnRegistered;
			pickupSpawnPoint.OnDeregistered -= _PickupSpawnPoint_OnDeregistered;
			if (this.OnPickupSpawnPointDeregistered != null)
			{
				this.OnPickupSpawnPointDeregistered(pickupSpawnPoint);
			}
		}
	}
}
