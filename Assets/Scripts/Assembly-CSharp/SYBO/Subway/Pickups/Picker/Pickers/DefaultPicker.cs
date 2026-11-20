using SYBO.Subway.Pickups.Pickers;

namespace SYBO.Subway.Pickups.Picker.Pickers
{
	public class DefaultPicker : BasePicker, IPicker
	{
		void IPicker.DoAdd(PickupManager2 pickupManager)
		{
			_DoAddProtected(pickupManager, PickerType.Default);
		}

		bool IPicker.DoUpdate()
		{
			_DebugLog("DefaultPicker.DoUpdate()");
			_DoUpdateProtected();
			return true;
		}
	}
}
