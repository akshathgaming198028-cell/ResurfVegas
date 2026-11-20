using System;
using System.Collections.Generic;
using Kiloo.Common;

public class EventTesterManager
{
	private static EventTesterManager _instance;

	private Dictionary<string, string> _settings = new Dictionary<string, string>();

	public static EventTesterManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new EventTesterManager();
			}
			return _instance;
		}
	}

	public static bool isInitialized
	{
		get
		{
			return _instance != null;
		}
	}

	public static event Action OnEventTesterInfoChanged;

	private EventTesterManager()
	{
		UpdateTimeLimit(24);
		UpdateSettingValue("event_value", "Slick");
		UpdateSettingValue("event_type", SSEventType.None.ToString());
	}

	public long GetEventEndTime()
	{
		if (_settings.ContainsKey("event_end"))
		{
			return long.Parse(_settings["event_end"]);
		}
		return 0L;
	}

	public long GetEventStartTime()
	{
		if (_settings.ContainsKey("event_start"))
		{
			return long.Parse(_settings["event_start"]);
		}
		return 0L;
	}

	public long GetEventID()
	{
		if (_settings.ContainsKey("event_end"))
		{
			return long.Parse(_settings["event_end"]);
		}
		return 0L;
	}

	public SSEventType GetEventType()
	{
		if (_settings.ContainsKey("event_type"))
		{
			return (SSEventType)(int)Enum.Parse(typeof(SSEventType), _settings["event_type"], true);
		}
		return SSEventType.None;
	}

	public string GetEventValue()
	{
		if (_settings.ContainsKey("event_value"))
		{
			return _settings["event_value"];
		}
		return string.Empty;
	}

	public void UpdateInfo(string eventType, string charType, string boardType, string hoursLeft)
	{
		UpdateTimeLimit(int.Parse(hoursLeft));
		UpdateSettingValue("event_type", eventType);
		SSEventType sSEventType = (SSEventType)(int)Enum.Parse(typeof(SSEventType), eventType, true);
		string value = string.Empty;
		switch (sSEventType)
		{
		case SSEventType.ExpiredLimitedCharacter:
		case SSEventType.CharacterDay:
			value = charType;
			break;
		case SSEventType.SpecialPowerBoard:
		case SSEventType.ExpiredLimitedBoard:
			value = boardType;
			break;
		case SSEventType.CharacterName:
			value = "11;20";
			break;
		}
		UpdateSettingValue("event_value", value);
		Action onEventTesterInfoChanged = EventTesterManager.OnEventTesterInfoChanged;
		if (onEventTesterInfoChanged != null)
		{
			onEventTesterInfoChanged();
		}
	}

	private void UpdateTimeLimit(int hoursFromNow)
	{
		long num = DateTime.Now.ToUnixTime();
		long num2 = DateTime.Now.AddHours(hoursFromNow).ToUnixTime();
		UpdateSettingValue("event_start", num.ToString());
		UpdateSettingValue("event_end", num2.ToString());
		UpdateSettingValue("event_id", num2.ToString());
	}

	private void UpdateSettingValue(string key, string value)
	{
		if (_settings.ContainsKey(key))
		{
			_settings[key] = value;
		}
		else
		{
			_settings.Add(key, value);
		}
	}
}
