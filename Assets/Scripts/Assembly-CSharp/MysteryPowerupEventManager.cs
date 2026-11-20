using UnityEngine;

public class MysteryPowerupEventManager : IEventManager
{
	private SSEventType _eventType = SSEventType.MysteryPowerup;

	private long _eventID;

	public MysteryPowerupEventManager(SSEventType eventType, long eventSignature)
	{
		_eventType = eventType;
		_eventID = eventSignature;
	}

	public SSEventType GetEventType()
	{
		return _eventType;
	}

	public long GetEventID()
	{
		return _eventID;
	}

	public bool IsEventActive()
	{
		return _eventID > 0;
	}

	public T GetEventValue<T>(out bool found)
	{
		T result = default(T);
		found = false;
		Debug.LogError("The Mystery Powerup Event does not contain a value of the specified type: " + typeof(T));
		return result;
	}

	public void GetPopupTitle(out string firstLine, out string secondLine)
	{
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			firstLine = Strings.Get(StringID.EVENTS_POPUP_MONDAY_EVENT_TITLE_LINE_1);
			secondLine = Strings.Get(StringID.EVENTS_POPUP_MONDAY_EVENT_TITLE_LINE_2);
		}
		else
		{
			firstLine = OnlineSettings.instance.GetValue("event_popup_title_first", Strings.Get(StringID.EVENTS_POPUP_MONDAY_EVENT_TITLE_LINE_1));
			secondLine = OnlineSettings.instance.GetValue("event_popup_title_second", Strings.Get(StringID.EVENTS_POPUP_MONDAY_EVENT_TITLE_LINE_2));
		}
	}

	public void GetPopupDescription(out string firstTitle, out string secondTitle)
	{
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			firstTitle = Strings.Get(StringID.MYSTERY_POWERUP_POPUP_DESCRIPTION_TITLE);
			secondTitle = Strings.Get(StringID.MYSTERY_POWERUP_POPUP_DESCRIPTION_SECOND_TITLE);
		}
		else
		{
			firstTitle = OnlineSettings.instance.GetValue("event_popup_description_first", Strings.Get(StringID.MYSTERY_POWERUP_POPUP_DESCRIPTION_TITLE));
			secondTitle = OnlineSettings.instance.GetValue("event_popup_description_second", Strings.Get(StringID.MYSTERY_POWERUP_POPUP_DESCRIPTION_SECOND_TITLE));
		}
	}

	public void GetRibbonDescription(out string firstLine, out string secondLine, out string thirdLine)
	{
		firstLine = string.Empty;
		secondLine = string.Empty;
		thirdLine = string.Empty;
		Debug.LogError("The Mystery Powerup Event does not contain information for a ribbon description");
	}

	public void UnHookFromGameEvents()
	{
	}
}
