using System.Collections.Generic;
using SYBO.Subway.Pickups.Pickers;
using UnityEngine;

namespace SYBO.Subway.Pickups.Picker.Pickers
{
	public class XTestPicker : BasePicker, IPicker
	{
		private int _temp_totalSpawnByThisPicker;

		void IPicker.DoAdd(PickupManager2 pickupManager)
		{
			_DoAddProtected(pickupManager, PickerType.XTest);
		}

		bool IPicker.DoUpdate()
		{
			return MakeManyAndDontLetOtherPickersHaveATurn();
		}

		private bool MakeManyAndDontLetOtherPickersHaveATurn()
		{
			_DoUpdateProtected();
			return false;
		}

		private bool TestToJustMake5()
		{
			List<PickupSpawnPoint2> list = _DoUpdateProtected();
			foreach (PickupSpawnPoint2 item in list)
			{
				if (item.SpawnPointMode != SpawnPointMode.WillForcePickupType)
				{
					_temp_totalSpawnByThisPicker++;
					Debug.Log("\t#(" + _temp_totalSpawnByThisPicker + ")PostPogo SPAWNED a : " + item.PickupData.PickupType);
				}
			}
			return false;
		}
	}
}
