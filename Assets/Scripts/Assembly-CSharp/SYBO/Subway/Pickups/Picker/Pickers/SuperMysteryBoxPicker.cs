using SYBO.Subway.Pickups.Pickers;

namespace SYBO.Subway.Pickups.Picker.Pickers
{
	public class SuperMysteryBoxPicker : BasePicker, IPicker
	{
		void IPicker.DoAdd(PickupManager2 pickupManager)
		{
			_DoAddProtected(pickupManager, PickerType.SuperMysteryBox);
		}

		bool IPicker.DoUpdate()
		{
			_DoUpdateProtected();
			return true;
		}
	}
}
