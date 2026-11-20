using System;
using SYBO.Subway.Info;
using UnityEngine;

public class CharacterDayEventManager : IEventManager
{
	private SSEventType _eventType = SSEventType.CharacterDay;

	private long _eventID;

	public CharacterDayEventManager(SSEventType eventType, long eventSignature)
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
		bool found = false;
		GetEventValue<Characters.CharacterType>(out found);
		return _eventID > 0 && found;
	}

	public T GetEventValue<T>(out bool found)
	{
		T result = default(T);
		found = false;
		string value = ((!Globals.DEBUG_USE_EVENT_TESTER) ? OnlineSettings.instance.GetValue("event_value", string.Empty) : EventTesterManager.Instance.GetEventValue());
		if (!string.IsNullOrEmpty(value))
		{
			try
			{
				result = (T)Enum.Parse(typeof(T), value, true);
				found = true;
			}
			catch (Exception ex)
			{
				found = false;
				Debug.LogError(string.Concat("The current event does not contain a value of the specified type: ", typeof(T), ", stack trace: ", ex.StackTrace));
			}
		}
		return result;
	}

	public void GetPopupTitle(out string firstLine, out string secondLine)
	{
		firstLine = string.Empty;
		secondLine = string.Empty;
		bool found = false;
		Characters.CharacterType eventValue = GetEventValue<Characters.CharacterType>(out found);
		if (found)
		{
			firstLine = string.Format(Strings.Get(StringID.EVENTS_POPUP_THURSDAY_EVENT_TITLE_LINE_1), Strings.Get(Characters.characterData[eventValue].name).ToUpper());
			secondLine = Strings.Get(StringID.EVENTS_POPUP_THURSDAY_EVENT_TITLE_LINE_2);
			if (Globals.DEBUG_USE_EVENT_TESTER)
			{
				return;
			}
		}
		else
		{
			Debug.LogError("The CharacterDay Event does not contain a valid character type!");
		}
		firstLine = OnlineSettings.instance.GetValue("event_popup_title_first", firstLine);
		secondLine = OnlineSettings.instance.GetValue("event_popup_title_second", secondLine);
	}

	public virtual void GetPopupDescription(out string firstTitle, out string secondTitle)
	{
		string defaultValue = string.Empty;
		string text = string.Empty;
		bool found = false;
		Characters.CharacterType eventValue = GetEventValue<Characters.CharacterType>(out found);
		if (found)
		{
			StringID name = Characters.characterData[eventValue].name;
			defaultValue = Strings.Get(StringID.EVENTS_POPUP_DESCRIPTION_TITLE_CHARACTER_DAY);
			text = string.Format(Strings.Get(StringID.EVENTS_POPUP_DESCRIPTION_SECOND_TITLE_CHARACTER_DAY), Strings.Get(name));
		}
		else
		{
			Debug.LogError("CharacterDayEventManager - GetPopupDescription: The Character type in the Character Day Event does not exist.");
		}
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			firstTitle = text;
			secondTitle = text;
		}
		else
		{
			firstTitle = OnlineSettings.instance.GetValue("event_popup_description_first", defaultValue);
			secondTitle = OnlineSettings.instance.GetValue("event_popup_description_second", text);
		}
	}

	public void GetRibbonDescription(out string firstLine, out string secondLine, out string thirdLine)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		bool found = false;
		Characters.CharacterType eventValue = GetEventValue<Characters.CharacterType>(out found);
		if (found)
		{
			StringID name = Characters.characterData[eventValue].name;
			text = Strings.Get(name).ToUpper();
			text2 = Strings.Get(StringID.EVENT_RIBBON_TEXT_THURSDAY);
			text3 = Strings.Get(StringID.EVENT_RIBBON_TEXT_MULTIPLIER);
		}
		else
		{
			Debug.LogError("CharacterDayEventManager - GetRibbonDescription: The Character type in the CharacterDay Event does not exist.");
		}
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			firstLine = text;
			secondLine = text2;
			thirdLine = text3;
			return;
		}
		string valueString = string.Empty;
		if (OnlineSettings.instance.TryGetValue("event_ribbon_text", out valueString) && !string.IsNullOrEmpty(valueString))
		{
			string[] array = valueString.Split(';');
			text = array[0];
			text2 = array[1];
			text3 = array[2];
		}
		firstLine = text;
		secondLine = text2;
		thirdLine = text3;
	}

	public void UnHookFromGameEvents()
	{
	}
}
