namespace SYBO.Subway.Pickups.Pickers
{
	public class PickerRemovedEvent : PickerRemoveRequestEvent
	{
		public PickerRemovedEvent(IPicker iPicker)
			: base(iPicker)
		{
		}
	}
}
