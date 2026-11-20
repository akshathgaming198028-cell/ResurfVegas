using SYBO.Subway.Pickups.Pickers;

namespace SYBO.Subway.Pickups.Picker.Pickers
{
	public class TokenHuntPicker : BasePicker, IPicker
	{
		void IPicker.DoAdd(PickupManager2 pickupManager)
		{
			_DoAddProtected(pickupManager, PickerType.TokenHunt);
		}

		bool IPicker.DoUpdate()
		{
			_DoUpdateProtected();
			return true;
		}
	}
}
