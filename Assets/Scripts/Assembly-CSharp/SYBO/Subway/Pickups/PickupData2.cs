using System;
using SYBO.Core;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities.Flags;
using UnityEngine;

namespace SYBO.Subway.Pickups
{
	[Serializable]
	public class PickupData2 : SYBO.Core.ICloneable
	{
		[SerializeField]
		private PickupType _pickupType;

		[Tooltip("The pickup's geometry prefab with no code components ...")]
		[SerializeField]
		private GameObject _pickupIconPrefab;

		[Tooltip("Manually set to to true for most. Others are programatically set based on current Game Events.")]
		[SerializeField]
		private PickupEnabledState _pickupEnabledState;

		[EnumFlagProperty(typeof(SpawnPointDifficulty))]
		[Tooltip("SpawnPoints are manually flagged with one of these values. Where should this Pickup spawn?")]
		[SerializeField]
		private SpawnPointDifficulty _spawnPointDifficulty;

		[EnumFlagProperty(typeof(SpawnPointHeight))]
		[Tooltip("SpawnPoints are manually flagged with one of these values. Where should this Pickup spawn?")]
		[SerializeField]
		private SpawnPointHeight _spawnPointHeight;

		[SerializeField]
		[Tooltip("Spawning: Reset minimum distance?")]
		private SpawnDistanceMinToPreviousMode _spawnDistanceMinToPreviousMode;

		[Range(0f, 100000f)]
		[Tooltip("Spawning: Minimum distance between ex. 'CoinMagnet' and previous 'CoinMagnet'...")]
		[SerializeField]
		private float _spawnDistanceMinToPrevious = 400f;

		[Tooltip("Spawning: 0 = never spawn, 1 = always ATTEMPT to spawn (it may not spawn still).")]
		[SerializeField]
		[Range(0f, 1f)]
		private float _spawnWeight;

		public PickupType PickupType
		{
			get
			{
				return _pickupType;
			}
		}

		public GameObject PickupIconPrefab
		{
			get
			{
				return _pickupIconPrefab;
			}
		}

		public PickupEnabledState PickupEnabledState
		{
			get
			{
				return _pickupEnabledState;
			}
			set
			{
				_pickupEnabledState = value;
			}
		}

		public SpawnPointDifficulty SpawnPointDifficulty
		{
			get
			{
				return _spawnPointDifficulty;
			}
		}

		public SpawnPointHeight SpawnPointHeight
		{
			get
			{
				return _spawnPointHeight;
			}
		}

		public SpawnDistanceMinToPreviousMode SpawnDistanceMinToPreviousMode
		{
			get
			{
				return _spawnDistanceMinToPreviousMode;
			}
			set
			{
				_spawnDistanceMinToPreviousMode = value;
			}
		}

		public float SpawnDistanceMinToPrevious
		{
			get
			{
				return _spawnDistanceMinToPrevious;
			}
		}

		public float SpawnWeight
		{
			get
			{
				return _spawnWeight;
			}
			set
			{
				_spawnWeight = value;
			}
		}

		public PickupData2()
		{
			FlagsHelper.SetNothing(ref _spawnPointDifficulty);
			FlagsHelper.SetNothing(ref _spawnPointHeight);
			_pickupType = PickupType.CoinMagnet;
			_pickupEnabledState = PickupEnabledState.Enabled;
			_spawnWeight = 0f;
			_spawnDistanceMinToPrevious = 400f;
			_spawnDistanceMinToPreviousMode = SpawnDistanceMinToPreviousMode.ResetAtRunStart;
		}

		public static PickupData2 CreateNew(PickupType pickupType)
		{
			PickupData2 pickupData = new PickupData2();
			pickupData._pickupType = pickupType;
			return pickupData;
		}

		public static PickupData2 CreateNew(PickupType pickupType, SpawnPointHeight spawnPointHeight, SpawnPointDifficulty spawnPointDifficulty, SpawnDistanceMinToPreviousMode spawnDistanceMinToPreviousMode, float spawnDistanceMinToPrevious)
		{
			PickupData2 pickupData = new PickupData2();
			pickupData._pickupType = pickupType;
			pickupData._spawnPointHeight = spawnPointHeight;
			pickupData._spawnPointDifficulty = spawnPointDifficulty;
			pickupData._spawnDistanceMinToPrevious = spawnDistanceMinToPrevious;
			return pickupData;
		}

		public object Clone()
		{
			return CreateNew(PickupType, SpawnPointHeight, SpawnPointDifficulty, SpawnDistanceMinToPreviousMode, SpawnDistanceMinToPrevious);
		}

		public override string ToString()
		{
			string empty = string.Empty;
			return string.Format(arg1: (!(_pickupIconPrefab != null)) ? "NA" : _pickupIconPrefab.name, format: "[PickupData2 (PickupType={0},PickupIconPrefab={1})]", arg0: PickupType);
		}

		public bool IsCompatibleWithSpawnPoint(PickupSpawnPoint2 pickupSpawnPoint)
		{
			if (_IsCompatibleWithSpawnPointDifficulty(pickupSpawnPoint) && _IsCompatibleWithSpawnPointHeight(pickupSpawnPoint) && _IsCompatibleWithSpawnPointZPosition(pickupSpawnPoint))
			{
				return true;
			}
			return false;
		}

		private bool _IsCompatibleWithSpawnPointDifficulty(PickupSpawnPoint2 pickupSpawnPoint)
		{
			return FlagsHelper.IsSet(SpawnPointDifficulty, pickupSpawnPoint.SpawnPointDifficulty);
		}

		private bool _IsCompatibleWithSpawnPointHeight(PickupSpawnPoint2 pickupSpawnPoint)
		{
			return FlagsHelper.IsSet(SpawnPointHeight, pickupSpawnPoint.SpawnPointHeight);
		}

		private bool _IsCompatibleWithSpawnPointZPosition(PickupSpawnPoint2 pickupSpawnPoint)
		{
			bool result = false;
			if (SingletonMonobehavior<PickupManager2>.IsInstantiated())
			{
				if (SingletonMonobehavior<PickupManager2>.Instance.HasShortHistoryOfLastZPositionForPickupType(PickupType) && SingletonMonobehavior<PickupManager2>.Instance.GetShortHistoryOfLastZPositionForPickupType(PickupType) > 0f)
				{
					float num = Game.ConvertUnityUnitsToGameMeters(SingletonMonobehavior<PickupManager2>.Instance.GetShortHistoryOfLastZPositionForPickupType(PickupType));
					float num2 = Game.ConvertUnityUnitsToGameMeters(pickupSpawnPoint.transform.position.z) - num;
					if (num2 > SpawnDistanceMinToPrevious)
					{
						_DebugLog("----------------");
						_DebugLog("\tspawnz: type" + PickupType);
						_DebugLog("\tspawnz: min" + SpawnDistanceMinToPrevious);
						_DebugLog("\tspawnz: last" + num);
						_DebugLog("\tspawnz: curr" + pickupSpawnPoint.transform.position.z);
						_DebugLog("\tspawnz: delt" + num2);
						_DebugLog("\tYES we are far enough");
						result = true;
					}
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		private void _DebugLog(string message)
		{
			if (SingletonMonobehavior<PickupManager2>.IsInstantiated() && !SingletonMonobehavior<PickupManager2>.Instance.WillDebugLogPickupData)
			{
			}
		}
	}
}
