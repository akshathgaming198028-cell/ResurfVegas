using System;
using SYBO.Subway.Info;
using UnityEngine;

public class ExpiredLimitedCharacterEventManager : IEventManager
{
	private SSEventType _eventType = SSEventType.ExpiredLimitedCharacter;

	private long _eventID;

	public ExpiredLimitedCharacterEventManager(SSEventType eventType, long eventSignature)
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
		return _eventID > 0 && ShouldEventBeAvailable();
	}

	private bool ShouldEventBeAvailable()
	{
		bool found = false;
		Characters.CharacterType eventValue = GetEventValue<Characters.CharacterType>(out found);
		if (found)
		{
			return !PlayerInfo.Instance.IsCollectionComplete(eventValue);
		}
		return false;
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
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			firstLine = Strings.Get(StringID.EVENTS_POPUP_WEDNESDAY_EVENT_TITLE_LINE_1);
			secondLine = Strings.Get(StringID.EVENTS_POPUP_WEDNESDAY_EVENT_TITLE_LINE_2);
		}
		else
		{
			firstLine = OnlineSettings.instance.GetValue("event_popup_title_first", Strings.Get(StringID.EVENTS_POPUP_WEDNESDAY_EVENT_TITLE_LINE_1));
			secondLine = OnlineSettings.instance.GetValue("event_popup_title_second", Strings.Get(StringID.EVENTS_POPUP_WEDNESDAY_EVENT_TITLE_LINE_2));
		}
	}

	public void GetPopupDescription(out string firstTitle, out string secondTitle)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		bool found = false;
		Characters.CharacterType eventValue = GetEventValue<Characters.CharacterType>(out found);
		if (found)
		{
			StringID name = Characters.characterData[eventValue].name;
			text = string.Format(Strings.Get(StringID.EVENTS_POPUP_DESCRIPTION_TITLE_LIMITED_CHAR), Strings.Get(name));
			text2 = string.Format(Strings.Get(StringID.EVENTS_POPUP_DESCRIPTION_SECOND_TITLE_LIMITED_CHAR), Strings.Get(name));
		}
		else
		{
			Debug.LogError("ExpiredLimitedCharacterEventManager - GetPopupDescription: The Character type does not exist.");
		}
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			firstTitle = text;
			secondTitle = text2;
		}
		else
		{
			firstTitle = OnlineSettings.instance.GetValue("event_popup_description_first", text);
			secondTitle = OnlineSettings.instance.GetValue("event_popup_description_second", text2);
		}
	}

	public void GetRibbonDescription(out string firstLine, out string secondLine, out string thirdLine)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		if (IsEventActive())
		{
			bool found = false;
			Characters.CharacterType eventValue = GetEventValue<Characters.CharacterType>(out found);
			if (found)
			{
				StringID name = Characters.characterData[eventValue].name;
				text = Strings.Get(StringID.EVENT_RIBBON_TEXT_WILD);
				text2 = Strings.Get(StringID.EVENT_RIBBON_TEXT_WEDNESDAY);
				text3 = string.Format(Strings.Get(StringID.EVENT_RIBBON_TEXT_COMEBACK), Strings.Get(name));
			}
			else
			{
				Debug.LogError("ExpiredLimitedCharacterEventManager - GetRibbonDescription: The Character type in the ExpiredLimitedCharacter Event does not exist.");
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
		}
		firstLine = text;
		secondLine = text2;
		thirdLine = text3;
	}

	public void UnHookFromGameEvents()
	{
	}
}
