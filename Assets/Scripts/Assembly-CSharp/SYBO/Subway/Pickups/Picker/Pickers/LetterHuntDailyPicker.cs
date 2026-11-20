using SYBO.Subway.Pickups.Pickers;

namespace SYBO.Subway.Pickups.Picker.Pickers
{
	public class LetterHuntDailyPicker : BasePicker, IPicker
	{
		void IPicker.DoAdd(PickupManager2 pickupManager)
		{
			_DoAddProtected(pickupManager, PickerType.LetterHuntDaily);
		}

		bool IPicker.DoUpdate()
		{
			PickupData2 pickupDataForPickupType = _pickupManager.GetPickupDataForPickupType(PickupType.LetterHuntDaily);
			if (pickupDataForPickupType.PickupEnabledState == PickupEnabledState.Enabled)
			{
				_DoUpdateProtected();
				return false;
			}
			return true;
		}
	}
}
