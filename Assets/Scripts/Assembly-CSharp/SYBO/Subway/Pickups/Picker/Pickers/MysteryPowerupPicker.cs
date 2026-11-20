using SYBO.Subway.Pickups.Pickers;

namespace SYBO.Subway.Pickups.Picker.Pickers
{
	public class MysteryPowerupPicker : BasePicker, IPicker
	{
		void IPicker.DoAdd(PickupManager2 pickupManager)
		{
			_DoAddProtected(pickupManager, PickerType.MysteryPowerUp);
		}

		bool IPicker.DoUpdate()
		{
			_DoUpdateProtected();
			return true;
		}
	}
}
