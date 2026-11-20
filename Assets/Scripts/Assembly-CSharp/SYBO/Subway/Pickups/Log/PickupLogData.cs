using System;

namespace SYBO.Subway.Pickups.Log
{
	[Serializable]
	public class PickupLogData : LogData
	{
		private PickupData2 _pickupData;

		private PickupSpawnPoint2 _pickupSpawnPoint;

		private SpawnPointDifficulty _spawnPointDifficultyForRegisterAndDeregisterOnly = SpawnPointDifficulty.Easy;

		private PickupLogType _pickupLogType;

		private SpawnPointMode _spawnPointMode;

		public static string PickupCategory = "Pickup";

		public PickupData2 PickupData
		{
			get
			{
				return _pickupData;
			}
		}

		public PickupSpawnPoint2 PickupSpawnPoint
		{
			get
			{
				return _pickupSpawnPoint;
			}
		}

		public SpawnPointDifficulty SpawnPointDifficultyForRegisterAndDeregisterOnly
		{
			get
			{
				return _spawnPointDifficultyForRegisterAndDeregisterOnly;
			}
		}

		public PickupLogType PickupLogType
		{
			get
			{
				return _pickupLogType;
			}
		}

		public SpawnPointMode SpawnPointMode
		{
			get
			{
				return _spawnPointMode;
			}
		}

		public PickupLogData(PickupLogType pickupLogType)
			: base("a", "b", "c")
		{
			_pickupLogType = pickupLogType;
			_category = PickupCategory;
			_message = _pickupLogType.ToString() + "(" + _timeStamp + ")";
		}

		public PickupLogData(PickupLogType pickupLogType, SpawnPointDifficulty spawnPointDifficulty, SpawnPointMode spawnPointMode)
			: base("a", "b", "c")
		{
			_pickupLogType = pickupLogType;
			_category = PickupCategory;
			_spawnPointDifficultyForRegisterAndDeregisterOnly = spawnPointDifficulty;
			_spawnPointMode = spawnPointMode;
			_message = _pickupLogType.ToString() + "(" + _timeStamp + ")";
		}

		public PickupLogData(PickupLogType pickupLogType, SpawnPointDifficulty spawnPointDifficulty)
			: base("a", "b", "c")
		{
			_pickupLogType = pickupLogType;
			_category = PickupCategory;
			_spawnPointDifficultyForRegisterAndDeregisterOnly = spawnPointDifficulty;
			_message = _pickupLogType.ToString() + "(" + _timeStamp + ")";
		}

		public PickupLogData(PickupLogType pickupLogType, PickupData2 pickupData, SpawnPointMode spawnPointMode)
			: base("a", "b", "c")
		{
			_pickupLogType = pickupLogType;
			_spawnPointMode = spawnPointMode;
			_category = PickupCategory;
			if (pickupData != null)
			{
				_pickupData = (PickupData2)pickupData.Clone();
				_message = string.Concat(_pickupLogType, "=", Enum.GetName(typeof(PickupType), _pickupData.PickupType), "(", _timeStamp, ")");
			}
			else
			{
				_message = string.Concat(_pickupLogType, " (", _timeStamp, ")");
			}
		}

		public override string ToString()
		{
			return string.Format("[PickupLogData (PickupLogType={0},PickupData={1})]", _pickupLogType, _pickupData);
		}
	}
}
