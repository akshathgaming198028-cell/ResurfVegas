using System;
using System.Collections.Generic;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities.Flags;
using SYBO.Subway.Pickups.Pickers;
using UnityEngine;

namespace SYBO.Subway.Pickups.Picker.Pickers
{
	public abstract class BasePicker : IPicker
	{
		protected PickupManager2 _pickupManager;

		protected PickerData2 _pickerData;

		public int ExecutionOrder
		{
			get
			{
				return _pickerData.ExecutionOrder;
			}
		}

		public float SpawnDistanceMinToPrevious
		{
			get
			{
				return _pickerData.SpawnDistanceMinToPrevious;
			}
		}

		public float ProbabilityToSpawn
		{
			get
			{
				return _pickerData.ProbabilityToSpawn;
			}
		}

		public string Name
		{
			get
			{
				return PickupManager2.ConvertPickerTypeToShortString(_pickerData.PickerType);
			}
		}

		protected event Action<PickerRemoveRequestEvent> _OnRemoveRequest;

		event Action<PickerRemoveRequestEvent> IPicker.OnRemoveRequest
		{
			add
			{
				this._OnRemoveRequest = (Action<PickerRemoveRequestEvent>)Delegate.Combine(this._OnRemoveRequest, value);
			}
			remove
			{
				this._OnRemoveRequest = (Action<PickerRemoveRequestEvent>)Delegate.Remove(this._OnRemoveRequest, value);
			}
		}

		public BasePicker()
		{
		}

		public void DoAdd(PickupManager2 pickupManager)
		{
		}

		public bool DoUpdate()
		{
			return true;
		}

		protected void _DebugLog(string message)
		{
			if (SingletonMonobehavior<PickupManager2>.IsInstantiated() && SingletonMonobehavior<PickupManager2>.Instance.WillDebugLogPickers)
			{
				Debug.Log(message);
			}
		}

		protected void _DoAddProtected(PickupManager2 pickupManager, PickerType pickerType)
		{
			_pickupManager = pickupManager;
			_pickerData = _pickupManager.GetPickerDataForPickerType(pickerType);
			if (_pickerData != null)
			{
				_DebugLog(GetType().Name + ".pickerData: " + _pickerData);
				return;
			}
			throw new UnityException(GetType().Name + ".pickerData must be not-null. Fix immediately");
		}

		protected List<PickupSpawnPoint2> _DoUpdateProtected()
		{
			List<PickupSpawnPoint2> list = new List<PickupSpawnPoint2>();
			return _MaybeChooseSpawnPoint();
		}

		protected List<PickupSpawnPoint2> _MaybeChooseSpawnPoint()
		{
			List<PickupSpawnPoint2> list = new List<PickupSpawnPoint2>();
			PickupData2 pickupData = null;
			foreach (PickupSpawnPoint2 pickupSpawnPoint in _pickupManager.PickupSpawnPointList)
			{
				if (pickupSpawnPoint.SpawnPointMode == SpawnPointMode.WillForcePickupType)
				{
				}
				if (!pickupSpawnPoint.IsAvailableForSpawning())
				{
					continue;
				}
				if (pickupSpawnPoint.SpawnPointMode == SpawnPointMode.WillForcePickupType)
				{
					if (_pickupManager.HasPickupDataForPickupType(pickupSpawnPoint.ForceSpawnPickupType))
					{
						pickupData = _pickupManager.GetPickupDataForPickupType(pickupSpawnPoint.ForceSpawnPickupType);
					}
				}
				else
				{
					pickupData = _MaybeChoosePickupDataForSpawnPoint(pickupSpawnPoint);
				}
				if (pickupData != null)
				{
					pickupSpawnPoint.DoSpawnAPickup(pickupData, _pickerData);
					list.Add(pickupSpawnPoint);
				}
			}
			return list;
		}

		private PickupData2 _MaybeChoosePickupDataForSpawnPoint(PickupSpawnPoint2 pickupSpawnPoint)
		{
			PickupData2 pickupData = null;
			float value = UnityEngine.Random.value;
			if (value < _pickerData.ProbabilityToSpawn)
			{
				_DebugLog("Probability SUCCESS (" + value + "<" + _pickerData.ProbabilityToSpawn + "). Now considering pickups...");
				List<PickupData2> list = new List<PickupData2>();
				if (_pickerData.IsCompatibleWithSpawnPoint(pickupSpawnPoint))
				{
					_DebugLog("PickerData.IsCompatible() SUCCESS ");
					foreach (PickupData2 pickupData2 in _pickupManager._pickupDataList)
					{
						if (FlagsHelper.IsSet(_pickerData.PickupType, pickupData2.PickupType) && pickupData2.SpawnWeight > 0f && pickupData2.PickupEnabledState == PickupEnabledState.Enabled && pickupData2.IsCompatibleWithSpawnPoint(pickupSpawnPoint))
						{
							list.Add(pickupData2);
						}
					}
				}
				if (list.Count > 0)
				{
					pickupData = _ChoosePickupDataBasedUponWeight(list);
				}
				_DebugLog(string.Concat("\t2. D CchosenPickupData: ", pickupData, " from total#  ", list.Count, ")"));
			}
			return pickupData;
		}

		protected PickupData2 _ChoosePickupDataBasedUponWeight(List<PickupData2> pickupDataList)
		{
			PickupData2 result = null;
			float num = 0f;
			foreach (PickupData2 pickupData in pickupDataList)
			{
				num += pickupData.SpawnWeight;
			}
			float num2 = UnityEngine.Random.value * num;
			float num3 = 0f;
			_DebugLog("\t3. roll (" + num2 + ") out of totalWeight (" + num + ")");
			foreach (PickupData2 pickupData2 in pickupDataList)
			{
				num3 += pickupData2.SpawnWeight;
				if (num2 <= num3)
				{
					_DebugLog("\t5. FOUND! (" + num2 + ") <= (" + num3 + ")");
					result = pickupData2;
					break;
				}
			}
			return result;
		}

		protected void _TriggerOnRequestRemoval(PickerRemoveRequestEvent requestRemovalEvent)
		{
			if (this._OnRemoveRequest != null)
			{
				this._OnRemoveRequest(requestRemovalEvent);
			}
		}
	}
}
