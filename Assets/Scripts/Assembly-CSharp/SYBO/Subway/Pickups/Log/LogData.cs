using System;

namespace SYBO.Subway.Pickups.Log
{
	[Serializable]
	public class LogData
	{
		protected string _category;

		protected float _timeStamp;

		protected string _type;

		protected string _message = string.Empty;

		public string Category
		{
			get
			{
				return _category;
			}
		}

		public float TimeStamp
		{
			get
			{
				return _timeStamp;
			}
		}

		public string Type
		{
			get
			{
				return _type;
			}
		}

		public string Message
		{
			get
			{
				return _message;
			}
		}

		public LogData(string category, string type, string message)
		{
			_timeStamp = PickupManager2.GetCurrentTimestamp();
			_category = category;
			_type = type;
			_message = message;
		}
	}
}
