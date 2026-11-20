namespace SYBO.Subway.Pickups.Pickers
{
	public class PickerRemoveRequestEvent
	{
		public IPicker _iPicker;

		public IPicker IPicker
		{
			get
			{
				return _iPicker;
			}
		}

		public PickerRemoveRequestEvent(IPicker iPicker)
		{
			_iPicker = iPicker;
		}
	}
}
