using System;
using System.Collections.Generic;
using SYBO.Core.Components.Singleton;
using UnityEngine;

namespace SYBO.Subway.Pickups.Pickers
{
	public class PickerCollection
	{
		private List<IPicker> _iPickerList;

		private PickupManager2 _pickupManager;

		public event Action<PickerRemoveRequestEvent> OnPickerRemoveRequest;

		public PickerCollection(PickupManager2 pickupManager)
		{
			_pickupManager = pickupManager;
			_iPickerList = new List<IPicker>();
		}

		public void Add(IPicker iPicker)
		{
			if (!_iPickerList.Contains(iPicker))
			{
				_iPickerList.Add(iPicker);
				iPicker.OnRemoveRequest += _IPicker_OnRemoveRequest;
				iPicker.DoAdd(_pickupManager);
			}
		}

		public int Count()
		{
			return _iPickerList.Count;
		}

		public bool HasPickerOfType<T>()
		{
			foreach (IPicker iPicker in _iPickerList)
			{
				if (iPicker.GetType() == typeof(T))
				{
					return true;
				}
			}
			return false;
		}

		public IPicker GetItemAtIndex(int i)
		{
			return _iPickerList[i];
		}

		public void Remove(IPicker iPicker)
		{
			Debug.Log("Remove: " + iPicker);
			if (_iPickerList.Contains(iPicker))
			{
				iPicker.OnRemoveRequest -= _IPicker_OnRemoveRequest;
				_iPickerList.Remove(iPicker);
			}
		}

		public void DoUpdate()
		{
			_iPickerList.Sort((IPicker a, IPicker b) => b.ExecutionOrder.CompareTo(a.ExecutionOrder));
			bool flag = true;
			_DebugLog("-------------PickerCollection.DoUpdate()-------------");
			for (int num = _iPickerList.Count - 1; num >= 0; num--)
			{
				flag = _iPickerList[num].DoUpdate();
				_DebugLog(string.Concat("\t", _iPickerList[num], ".DoUpdate()"));
				if (!flag)
				{
					break;
				}
			}
		}

		protected void _DebugLog(string message)
		{
			if (SingletonMonobehavior<PickupManager2>.IsInstantiated() && SingletonMonobehavior<PickupManager2>.Instance.WillDebugLogPickers)
			{
				Debug.Log(message);
			}
		}

		protected void _TriggerOnPickerRemove(PickerRemoveRequestEvent pickerRemoveRequestEvent)
		{
			if (this.OnPickerRemoveRequest != null)
			{
				this.OnPickerRemoveRequest(pickerRemoveRequestEvent);
			}
		}

		private void _IPicker_OnRemoveRequest(PickerRemoveRequestEvent pickerRemoveRequestEvent)
		{
			_TriggerOnPickerRemove(pickerRemoveRequestEvent);
		}
	}
}
