using System;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities.Flags;
using UnityEngine;

namespace SYBO.Subway.Pickups.Pickers
{
	[Serializable]
	public class PickerData2
	{
		[SerializeField]
		internal PickerType _pickerType;

		[Tooltip("Manually set to to true for most. Others are programatically set based on (...user progress?).")]
		[SerializeField]
		private PickerEnabledState _pickerEnabledState;

		[Tooltip("The PickupTypes allowed to spawn")]
		[SerializeField]
		[EnumFlagProperty(typeof(PickupType))]
		internal PickupType _pickupType;

		[SerializeField]
		[Tooltip("Spawning: Reset minimum distance?")]
		private SpawnDistanceMinToPreviousMode _spawnDistanceMinToPreviousMode;

		[Tooltip("Spawning: Minimum distance between ex. 'AnythingThisPickerSpawns' and previous 'AnythingThisPickerSpawns'...")]
		[Range(0f, 100000f)]
		[SerializeField]
		private float _spawnDistanceMinToPrevious;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("0 = never ATTEMPT, 1 = always ATTEMPT, etc...")]
		internal float _probabilityToSpawn;

		[Tooltip("Relative amongst the CURRENTLY ACTIVE pickers. 0 = DoUpdate() called first, 1 = DoUpdate() called second, etc...")]
		[SerializeField]
		protected int _executionOrder;

		public PickerType PickerType
		{
			get
			{
				return _pickerType;
			}
		}

		public PickerEnabledState PickerEnabledState
		{
			get
			{
				return _pickerEnabledState;
			}
			set
			{
				_pickerEnabledState = value;
			}
		}

		public PickupType PickupType
		{
			get
			{
				return _pickupType;
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

		public float ProbabilityToSpawn
		{
			get
			{
				return _probabilityToSpawn;
			}
		}

		public int ExecutionOrder
		{
			get
			{
				return _executionOrder;
			}
		}

		public PickerData2()
		{
			FlagsHelper.SetEverything(ref _pickupType);
			_executionOrder = 0;
			_probabilityToSpawn = 1f;
			_pickerType = PickerType.Default;
			_spawnDistanceMinToPreviousMode = SpawnDistanceMinToPreviousMode.ResetAtRunStart;
			_spawnDistanceMinToPrevious = 0f;
		}

		public bool IsCompatibleWithSpawnPoint(PickupSpawnPoint2 pickupSpawnPoint)
		{
			if (_IsCompatibleWithSpawnPointZPosition(pickupSpawnPoint))
			{
				return true;
			}
			return false;
		}

		private bool _IsCompatibleWithSpawnPointZPosition(PickupSpawnPoint2 pickupSpawnPoint)
		{
			bool result = false;
			if (SingletonMonobehavior<PickupManager2>.IsInstantiated())
			{
				_DebugLog("----------------");
				_DebugLog("\tSpawnDistanceMinToPrevious" + SpawnDistanceMinToPrevious);
				if (SingletonMonobehavior<PickupManager2>.Instance.HasShortHistoryOfLastZPositionForPickerType(PickerType) && SingletonMonobehavior<PickupManager2>.Instance.GetShortHistoryOfLastZPositionForPickerType(PickerType) > 0f)
				{
					float num = Game.ConvertUnityUnitsToGameMeters(SingletonMonobehavior<PickupManager2>.Instance.GetShortHistoryOfLastZPositionForPickerType(PickerType));
					float num2 = Game.ConvertUnityUnitsToGameMeters(pickupSpawnPoint.transform.position.z) - num;
					_DebugLog("\tlastPositionInMeters: " + num + ", deltaPositionInMeters: " + num2);
					if (num2 > SpawnDistanceMinToPrevious)
					{
						_DebugLog("\tSUCCESS: " + num2 + ">" + SpawnDistanceMinToPrevious);
						result = true;
					}
				}
				else
				{
					_DebugLog("\tSUCCESS: no history yet");
					result = true;
				}
			}
			return result;
		}

		public override string ToString()
		{
			return string.Format("[PickerData2: PickerType={0}, PickupType={1}]", PickerType, PickupManager2.ConvertPickupTypeFlagsToString(PickupType));
		}

		private void _DebugLog(string message)
		{
			if (SingletonMonobehavior<PickupManager2>.IsInstantiated() && SingletonMonobehavior<PickupManager2>.Instance.WillDebugLogPickupData)
			{
				Debug.Log(message);
			}
		}
	}
}
