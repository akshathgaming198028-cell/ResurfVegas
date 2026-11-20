using UnityEngine;

public class BiggerJackpotEventManager : IEventManager
{
	private SSEventType _eventType = SSEventType.BiggerJackpot;

	private long _eventID;

	public BiggerJackpotEventManager(SSEventType eventType, long signature)
	{
		_eventType = eventType;
		_eventID = signature;
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
		Debug.LogError("The BiggerJackpot Event does not contain a value of the specified type: " + typeof(T));
		return result;
	}

	public void GetPopupTitle(out string firstLine, out string secondLine)
	{
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			firstLine = Strings.Get(StringID.EVENTS_POPUP_TUESDAY_EVENT_TITLE_LINE_1);
			secondLine = Strings.Get(StringID.EVENTS_POPUP_TUESDAY_EVENT_TITLE_LINE_2);
		}
		else
		{
			firstLine = OnlineSettings.instance.GetValue("event_popup_title_first", Strings.Get(StringID.EVENTS_POPUP_TUESDAY_EVENT_TITLE_LINE_1));
			secondLine = OnlineSettings.instance.GetValue("event_popup_title_second", Strings.Get(StringID.EVENTS_POPUP_TUESDAY_EVENT_TITLE_LINE_2));
		}
	}

	public void GetPopupDescription(out string firstTitle, out string secondTitle)
	{
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			firstTitle = Strings.Get(StringID.EVENTS_POPUP_DESCRIPTION_TITLE_JACKPOT);
			secondTitle = string.Empty;
		}
		else
		{
			firstTitle = OnlineSettings.instance.GetValue("event_popup_description_first", Strings.Get(StringID.EVENTS_POPUP_DESCRIPTION_TITLE_JACKPOT));
			secondTitle = OnlineSettings.instance.GetValue("event_popup_description_second", string.Empty);
		}
	}

	public void GetRibbonDescription(out string firstLine, out string secondLine, out string thirdLine)
	{
		firstLine = string.Empty;
		secondLine = string.Empty;
		thirdLine = string.Empty;
		Debug.LogError("The Bigger Jackpot Event does not contain information for a ribbon description");
	}

	public void UnHookFromGameEvents()
	{
	}
}
