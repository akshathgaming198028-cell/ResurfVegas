using System;
using System.Collections.Generic;
using SYBO.Subway.Managers;
using UnityEngine;

namespace SYBO.Subway.Pickups
{
	public class DebugPickupSpawnPointPosition : MonoBehaviour
	{
		internal List<float> _normalEasyValidYPositions;

		internal List<float> _normalMediumValidYPositions;

		internal List<float> _despawnedValidYPositions;

		internal List<float> _unspawnedValidYPositions;

		internal List<float> _forcedValidYPositions;

		internal List<float> _pogoStickValidYPositions;

		private bool _isTestingYPositioning;

		private float _lastInvalidPositionFound = -1f;

		private bool _isInitialized;

		private PickupSpawnPoint2 _pickupSpawnPoint;

		private TrackObject _trackObject;

		protected void Awake()
		{
			_trackObject = GetComponent<TrackObject>();
			_trackObject.OnActivate += TrackObject_OnActivate;
			MaybeInitialize();
		}

		internal void Initialize()
		{
			if (!_isInitialized)
			{
				_isInitialized = true;
				_pickupSpawnPoint = GetComponent<PickupSpawnPoint2>();
				if (_pickupSpawnPoint == null)
				{
					Debug.LogError("_pickupSpawnPoint cannot be null: " + _pickupSpawnPoint);
				}
				_unspawnedValidYPositions = new List<float>();
				List<float> unspawnedValidYPositions = _unspawnedValidYPositions;
				Vector3 dEFAULT_LOCAL_POSITION = SpawnPoolManager.DEFAULT_LOCAL_POSITION;
				unspawnedValidYPositions.Add(dEFAULT_LOCAL_POSITION.y);
				List<float> unspawnedValidYPositions2 = _unspawnedValidYPositions;
				Vector3 dEFAULT_GROUP_LOCAL_POSITION = SpawnPoolManager.DEFAULT_GROUP_LOCAL_POSITION;
				unspawnedValidYPositions2.Add(dEFAULT_GROUP_LOCAL_POSITION.y);
				_despawnedValidYPositions = new List<float>();
				List<float> despawnedValidYPositions = _despawnedValidYPositions;
				Vector3 dEFAULT_LOCAL_POSITION2 = SpawnPoolManager.DEFAULT_LOCAL_POSITION;
				despawnedValidYPositions.Add(dEFAULT_LOCAL_POSITION2.y);
				_despawnedValidYPositions.Add(170f);
				_normalEasyValidYPositions = new List<float>();
				_normalEasyValidYPositions.Add(7f);
				_normalEasyValidYPositions.Add(41f);
				_normalEasyValidYPositions.Add(42f);
				_normalEasyValidYPositions.Add(52f);
				_normalEasyValidYPositions.Add(61f);
				_normalEasyValidYPositions.Add(62f);
				_normalMediumValidYPositions = new List<float>();
				_normalMediumValidYPositions.Add(7f);
				_normalMediumValidYPositions.Add(31.2f);
				_normalMediumValidYPositions.Add(40.8f);
				_normalMediumValidYPositions.Add(55.5f);
				_normalMediumValidYPositions.Add(60f);
				_normalMediumValidYPositions.Add(69.60001f);
				_forcedValidYPositions = new List<float>();
				List<float> forcedValidYPositions = _forcedValidYPositions;
				Vector3 dEFAULT_GROUP_LOCAL_POSITION2 = SpawnPoolManager.DEFAULT_GROUP_LOCAL_POSITION;
				forcedValidYPositions.Add(dEFAULT_GROUP_LOCAL_POSITION2.y);
				_forcedValidYPositions.Add(7f);
				_forcedValidYPositions.Add(100f);
				_forcedValidYPositions.Add(110f);
				_forcedValidYPositions.Add(125f);
				_forcedValidYPositions.Add(170f);
				_pogoStickValidYPositions = new List<float>();
				_pogoStickValidYPositions.Add(7f);
			}
		}

		protected void OnDestroy()
		{
			if (_trackObject != null)
			{
				_trackObject.OnActivate -= TrackObject_OnActivate;
			}
		}

		private void MaybeInitialize()
		{
			if (_isTestingYPositioning)
			{
				Initialize();
			}
			else
			{
				UnityEngine.Object.Destroy(this);
			}
		}

		protected void Update()
		{
			if (_isInitialized && !_isTestingYPositioning)
			{
			}
		}

		public bool ValidatePosition(string methodName)
		{
			bool flag = CheckIsAllowedPerList_FrequentCall(methodName);
			if (flag)
			{
			}
			bool flag2 = IsPositionAllowedPerBugCriteria(_pickupSpawnPoint.transform.position);
			if (!flag2)
			{
				LogPickupSpawnPoint(base.transform.position, _pickupSpawnPoint, methodName);
			}
			return flag && flag2;
		}

		public static void LogPickupSpawnPoint(Vector3 newPosition, PickupSpawnPoint2 pickupSpawnPoint, string methodName)
		{
			Debug.LogError("-----------------\n" + methodName + "() , original (x=" + pickupSpawnPoint.LocalPositionOriginal.x + ",y=" + pickupSpawnPoint.LocalPositionOriginal.y + ") and new (x=" + newPosition.x + ",y=" + newPosition.y + ")", pickupSpawnPoint.gameObject);
			Debug.LogError("  gameObject: " + Utils.GetLongName(pickupSpawnPoint.gameObject.transform), pickupSpawnPoint.gameObject);
			Debug.LogError("  ID: " + pickupSpawnPoint.transform.GetInstanceID(), pickupSpawnPoint.gameObject);
		}

		protected bool CheckIsAllowedPerList(Vector3 newPosition, CheckType checkType, string methodName)
		{
			bool result = false;
			if (!_isInitialized)
			{
				return result;
			}
			List<float> list = null;
			string text = checkType.ToString();
			switch (checkType)
			{
			case CheckType.Unspawned:
				list = _unspawnedValidYPositions;
				break;
			case CheckType.Despawned:
				list = _despawnedValidYPositions;
				break;
			case CheckType.Easy:
				list = _normalEasyValidYPositions;
				break;
			case CheckType.Forced:
				list = _forcedValidYPositions;
				break;
			case CheckType.PogoStick:
				list = _pogoStickValidYPositions;
				break;
			case CheckType.Medium:
				list = _normalMediumValidYPositions;
				break;
			default:
				throw new UnityException("SwitchDefaultException: No case for: " + checkType);
			}
			result = IsAllowedPerList(newPosition, list);
			if (!result && newPosition.y != _lastInvalidPositionFound)
			{
				_lastInvalidPositionFound = newPosition.y;
				Debug.LogError("-----------------", base.gameObject);
				LogPickupSpawnPoint(newPosition, _pickupSpawnPoint, methodName + ", [CheckIsAllowed() List: " + text + "]");
			}
			return result;
		}

		private bool IsAllowedPerList(Vector3 newPosition, List<float> validYPositionsList)
		{
			bool result = false;
			bool flag = false;
			bool flag2 = false;
			if (!_isInitialized)
			{
				return result;
			}
			foreach (float validYPositions in validYPositionsList)
			{
				float a = validYPositions;
				if (IsNearlyEqualThreshold(a, newPosition.y, 0.1f))
				{
					flag2 = true;
					break;
				}
			}
			if (newPosition.x % 20f == 0f)
			{
				flag = true;
			}
			result = flag && flag2;
			if (!result)
			{
			}
			return result;
		}

		public static bool IsPositionAllowedPerBugCriteria(Vector3 newPosition)
		{
			if (newPosition.y > 0.7f && newPosition.y < 0.9f)
			{
				Debug.LogError("newPosition.y: " + newPosition.y);
				return false;
			}
			return true;
		}

		public bool CheckIsAllowedPerList_FrequentCall(string methodName)
		{
			bool result = false;
			if (!_isInitialized)
			{
				return result;
			}
			if (_pickupSpawnPoint.SpawnPointMode == SpawnPointMode.Normal)
			{
				if (_pickupSpawnPoint.SpawnPointState != SpawnPointState.Despawned)
				{
					switch (_pickupSpawnPoint.SpawnPointDifficulty)
					{
					case SpawnPointDifficulty.Easy:
						result = CheckIsAllowedPerList(base.gameObject.transform.localPosition, CheckType.Easy, methodName);
						break;
					case SpawnPointDifficulty.Medium:
						result = CheckIsAllowedPerList(base.gameObject.transform.localPosition, CheckType.Medium, methodName);
						break;
					default:
						throw new UnityException("SwitchDefaultException: No case for: " + _pickupSpawnPoint.SpawnPointDifficulty);
					}
				}
			}
			else
			{
				bool flag = CheckIsAllowedPerList(base.gameObject.transform.localPosition, CheckType.Forced, methodName);
				bool flag2 = true;
				if (_pickupSpawnPoint.ForceSpawnPickupType == PickupType.PogoStick)
				{
					flag2 = CheckIsAllowedPerList(base.gameObject.transform.localPosition, CheckType.PogoStick, methodName);
				}
				result = flag && flag2;
			}
			return result;
		}

		public static bool IsNearlyEqualThreshold(float a, float b, float threshold)
		{
			float num = Math.Abs(a - b);
			if (a == b)
			{
				return true;
			}
			return num < threshold;
		}

		public void OnSpawned()
		{
			_lastInvalidPositionFound = -1f;
			MaybeInitialize();
			if (!_isInitialized && _isTestingYPositioning)
			{
				Debug.Log("3 >>>>>>>>>Unspawned() with _isInitialized: " + _isInitialized + " yet _isTestingYPositioning: " + _isTestingYPositioning, base.gameObject);
				bool flag = CheckIsAllowedPerList(base.gameObject.transform.localPosition, CheckType.Unspawned, "OnSpawned()");
				if (!flag)
				{
					Debug.Log("3 >>>>>>>>>Unspawned() isAllowed: " + flag, base.gameObject);
				}
			}
		}

		public void OnDespawned()
		{
		}

		public void OnDespawnedCompleted()
		{
			bool flag = CheckIsAllowedPerList(base.gameObject.transform.localPosition, CheckType.Despawned, "OnDespawnedCompleted()");
			if (!flag)
			{
				Debug.Log("4 >>>>>>>>>OnDespawnedCompleted() isAllowed: " + flag, base.gameObject);
			}
		}

		private void TrackObject_OnActivate()
		{
			if (_isInitialized && _isTestingYPositioning)
			{
				if (!CheckIsAllowedPerList_FrequentCall("TrackObject_OnActivate()"))
				{
				}
				if (!IsPositionAllowedPerBugCriteria(_pickupSpawnPoint.transform.position))
				{
					LogPickupSpawnPoint(_pickupSpawnPoint.transform.position, _pickupSpawnPoint, "TrackObject_OnActivate()");
				}
			}
		}
	}
}
