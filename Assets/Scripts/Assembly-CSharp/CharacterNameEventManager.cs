using System;
using System.Collections.Generic;
using MiniJSON;
using SYBO.Subway.Info;
using UnityEngine;
using com.kiloo.awards;

public class CharacterNameEventManager : IEventManager
{
	public class LetterCollectedEventArgs : EventArgs
	{
		private char _characterCollected;

		public char CharacterCollected
		{
			get
			{
				return _characterCollected;
			}
		}

		public LetterCollectedEventArgs(char characterCollected)
		{
			_characterCollected = characterCollected;
		}

		public override string ToString()
		{
			return string.Format("[LetterCollectedEventArgs: l={0}", _characterCollected);
		}
	}

	public const string ONLINE_SETTING_DESCRIPTION_FIRST_UNLOCK_CHARATER_NAME = "event_popup_description_first_unlock_character_name";

	public const string ONLINE_SETTING_DESCRIPTION_SECOND_UNLOCK_CHARACTER_NAME = "event_popup_description_second_unlock_character_name";

	public const string ONLINE_SETTING_DESCRIPTION_FIRST_SELECT_CHARACTER_NAME = "event_popup_description_first_select_character_name";

	public const string ONLINE_SETTING_DESCRIPTION_SECOND_SELECT_CHARACTER_NAME = "event_popup_description_second_select_character_name";

	private const int DEFAULT_VALUE_FOR_COLLECT = -1;

	public static readonly char[] FILTERED_CHARACTERS = new char[1] { ' ' };

	private SSEventType _eventType = SSEventType.CharacterName;

	private long _eventID;

	private CharacterNameEventData _eventProgressData;

	private Characters.CharacterType?[][] normalCharacters = new Characters.CharacterType?[12][]
	{
		new Characters.CharacterType?[36]
		{
			Characters.CharacterType.slick,
			null,
			Characters.CharacterType.facebookRewardCharacter01,
			Characters.CharacterType.tasha,
			null,
			null,
			Characters.CharacterType.fresh,
			Characters.CharacterType.frizzy,
			null,
			null,
			Characters.CharacterType.spike,
			Characters.CharacterType.lucy,
			Characters.CharacterType.brody,
			Characters.CharacterType.tricky,
			null,
			null,
			Characters.CharacterType.king,
			Characters.CharacterType.zoe,
			null,
			null,
			Characters.CharacterType.frank,
			Characters.CharacterType.yutani,
			null,
			null,
			Characters.CharacterType.princek,
			Characters.CharacterType.ninja,
			null,
			null,
			Characters.CharacterType.tag,
			null,
			null,
			null,
			null,
			null,
			null,
			null
		},
		new Characters.CharacterType?[36]
		{
			Characters.CharacterType.lucy,
			Characters.CharacterType.facebookRewardCharacter01,
			null,
			Characters.CharacterType.spike,
			Characters.CharacterType.frizzy,
			null,
			Characters.CharacterType.zoe,
			Characters.CharacterType.fresh,
			null,
			Characters.CharacterType.princek,
			Characters.CharacterType.tricky,
			Characters.CharacterType.ninja,
			Characters.CharacterType.tasha,
			null,
			Characters.CharacterType.yutani,
			Characters.CharacterType.tag,
			null,
			Characters.CharacterType.slick,
			null,
			Characters.CharacterType.king,
			null,
			Characters.CharacterType.brody,
			null,
			Characters.CharacterType.frank,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null
		},
		new Characters.CharacterType?[36]
		{
			Characters.CharacterType.ninja,
			null,
			Characters.CharacterType.facebookRewardCharacter01,
			Characters.CharacterType.tasha,
			null,
			Characters.CharacterType.yutani,
			Characters.CharacterType.brody,
			null,
			Characters.CharacterType.fresh,
			Characters.CharacterType.zoe,
			null,
			null,
			Characters.CharacterType.princek,
			Characters.CharacterType.frizzy,
			null,
			Characters.CharacterType.tricky,
			Characters.CharacterType.frank,
			null,
			Characters.CharacterType.spike,
			Characters.CharacterType.lucy,
			null,
			null,
			Characters.CharacterType.slick,
			Characters.CharacterType.tag,
			null,
			null,
			Characters.CharacterType.king,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null
		},
		new Characters.CharacterType?[36]
		{
			Characters.CharacterType.tag,
			null,
			null,
			Characters.CharacterType.tricky,
			Characters.CharacterType.facebookRewardCharacter01,
			null,
			null,
			Characters.CharacterType.lucy,
			Characters.CharacterType.spike,
			null,
			Characters.CharacterType.princek,
			Characters.CharacterType.tasha,
			null,
			Characters.CharacterType.zoe,
			Characters.CharacterType.fresh,
			null,
			Characters.CharacterType.slick,
			Characters.CharacterType.frizzy,
			null,
			null,
			Characters.CharacterType.brody,
			Characters.CharacterType.yutani,
			null,
			null,
			Characters.CharacterType.king,
			Characters.CharacterType.frank,
			null,
			null,
			Characters.CharacterType.ninja,
			null,
			null,
			null,
			null,
			null,
			null,
			null
		},
		new Characters.CharacterType?[37]
		{
			Characters.CharacterType.king,
			null,
			Characters.CharacterType.facebookRewardCharacter01,
			Characters.CharacterType.frizzy,
			null,
			null,
			Characters.CharacterType.spike,
			Characters.CharacterType.zoe,
			null,
			null,
			null,
			Characters.CharacterType.yutani,
			Characters.CharacterType.princek,
			null,
			null,
			Characters.CharacterType.lucy,
			Characters.CharacterType.frank,
			null,
			null,
			Characters.CharacterType.tricky,
			Characters.CharacterType.ninja,
			null,
			Characters.CharacterType.fresh,
			Characters.CharacterType.tag,
			null,
			null,
			Characters.CharacterType.brody,
			null,
			null,
			Characters.CharacterType.slick,
			null,
			Characters.CharacterType.tasha,
			null,
			null,
			null,
			null,
			null
		},
		new Characters.CharacterType?[37]
		{
			Characters.CharacterType.tricky,
			Characters.CharacterType.princek,
			null,
			Characters.CharacterType.facebookRewardCharacter01,
			Characters.CharacterType.zoe,
			null,
			null,
			null,
			Characters.CharacterType.yutani,
			Characters.CharacterType.slick,
			null,
			Characters.CharacterType.king,
			Characters.CharacterType.tasha,
			null,
			null,
			null,
			Characters.CharacterType.frizzy,
			Characters.CharacterType.fresh,
			null,
			null,
			null,
			Characters.CharacterType.frank,
			Characters.CharacterType.lucy,
			null,
			null,
			Characters.CharacterType.spike,
			null,
			null,
			Characters.CharacterType.tag,
			null,
			Characters.CharacterType.ninja,
			null,
			Characters.CharacterType.brody,
			null,
			null,
			null,
			null
		},
		new Characters.CharacterType?[37]
		{
			Characters.CharacterType.tasha,
			Characters.CharacterType.facebookRewardCharacter01,
			null,
			null,
			Characters.CharacterType.yutani,
			Characters.CharacterType.tag,
			null,
			null,
			null,
			Characters.CharacterType.frank,
			Characters.CharacterType.tricky,
			null,
			null,
			Characters.CharacterType.brody,
			Characters.CharacterType.frizzy,
			Characters.CharacterType.spike,
			Characters.CharacterType.zoe,
			null,
			null,
			null,
			null,
			Characters.CharacterType.fresh,
			Characters.CharacterType.lucy,
			Characters.CharacterType.king,
			Characters.CharacterType.ninja,
			null,
			null,
			Characters.CharacterType.princek,
			null,
			null,
			Characters.CharacterType.slick,
			null,
			null,
			null,
			null,
			null,
			null
		},
		new Characters.CharacterType?[37]
		{
			Characters.CharacterType.facebookRewardCharacter01,
			null,
			Characters.CharacterType.slick,
			Characters.CharacterType.zoe,
			null,
			null,
			Characters.CharacterType.fresh,
			Characters.CharacterType.ninja,
			null,
			null,
			Characters.CharacterType.spike,
			Characters.CharacterType.tasha,
			null,
			null,
			null,
			null,
			Characters.CharacterType.princek,
			Characters.CharacterType.yutani,
			null,
			null,
			Characters.CharacterType.frank,
			Characters.CharacterType.tag,
			null,
			null,
			Characters.CharacterType.king,
			Characters.CharacterType.tricky,
			null,
			null,
			Characters.CharacterType.brody,
			Characters.CharacterType.frizzy,
			null,
			Characters.CharacterType.lucy,
			null,
			null,
			null,
			null,
			null
		},
		new Characters.CharacterType?[38]
		{
			Characters.CharacterType.zoe,
			Characters.CharacterType.zombiejake,
			Characters.CharacterType.yutani,
			Characters.CharacterType.facebookRewardCharacter01,
			null,
			Characters.CharacterType.ninja,
			Characters.CharacterType.tasha,
			null,
			Characters.CharacterType.tricky,
			Characters.CharacterType.princek,
			null,
			null,
			Characters.CharacterType.lucy,
			Characters.CharacterType.tag,
			null,
			null,
			Characters.CharacterType.frizzy,
			Characters.CharacterType.spike,
			null,
			null,
			null,
			Characters.CharacterType.fresh,
			Characters.CharacterType.frank,
			null,
			null,
			Characters.CharacterType.king,
			null,
			null,
			Characters.CharacterType.brody,
			null,
			null,
			Characters.CharacterType.slick,
			null,
			null,
			null,
			null,
			null,
			null
		},
		new Characters.CharacterType?[38]
		{
			Characters.CharacterType.zombiejake,
			null,
			null,
			Characters.CharacterType.zoe,
			Characters.CharacterType.facebookRewardCharacter01,
			null,
			null,
			Characters.CharacterType.yutani,
			Characters.CharacterType.frank,
			null,
			null,
			Characters.CharacterType.ninja,
			Characters.CharacterType.fresh,
			null,
			null,
			Characters.CharacterType.frizzy,
			Characters.CharacterType.slick,
			null,
			null,
			Characters.CharacterType.tricky,
			Characters.CharacterType.princek,
			null,
			null,
			Characters.CharacterType.lucy,
			Characters.CharacterType.king,
			null,
			null,
			Characters.CharacterType.tasha,
			Characters.CharacterType.brody,
			null,
			null,
			Characters.CharacterType.tag,
			Characters.CharacterType.spike,
			null,
			null,
			null,
			null,
			null
		},
		new Characters.CharacterType?[39]
		{
			Characters.CharacterType.elftricky,
			Characters.CharacterType.facebookRewardCharacter01,
			null,
			null,
			null,
			Characters.CharacterType.londonCharacter02,
			Characters.CharacterType.frizzy,
			null,
			null,
			Characters.CharacterType.spike,
			Characters.CharacterType.lucy,
			null,
			Characters.CharacterType.yutani,
			Characters.CharacterType.slick,
			null,
			null,
			null,
			Characters.CharacterType.princek,
			Characters.CharacterType.tricky,
			null,
			null,
			Characters.CharacterType.brody,
			Characters.CharacterType.zoe,
			null,
			null,
			Characters.CharacterType.fresh,
			Characters.CharacterType.tasha,
			null,
			null,
			Characters.CharacterType.ninja,
			null,
			Characters.CharacterType.frank,
			null,
			null,
			Characters.CharacterType.tag,
			null,
			null,
			Characters.CharacterType.king,
			null
		},
		new Characters.CharacterType?[39]
		{
			Characters.CharacterType.slick,
			Characters.CharacterType.elftricky,
			Characters.CharacterType.londonCharacter02,
			null,
			null,
			Characters.CharacterType.yutani,
			Characters.CharacterType.facebookRewardCharacter01,
			null,
			null,
			Characters.CharacterType.tasha,
			Characters.CharacterType.fresh,
			null,
			null,
			Characters.CharacterType.zoe,
			Characters.CharacterType.spike,
			null,
			null,
			Characters.CharacterType.frizzy,
			Characters.CharacterType.king,
			null,
			null,
			Characters.CharacterType.lucy,
			Characters.CharacterType.princek,
			null,
			null,
			Characters.CharacterType.tricky,
			Characters.CharacterType.frank,
			null,
			null,
			Characters.CharacterType.brody,
			Characters.CharacterType.tag,
			null,
			Characters.CharacterType.ninja,
			null,
			null,
			null,
			null,
			null,
			null
		}
	};

	private Characters.CharacterType[] extraCharacters = new Characters.CharacterType[28]
	{
		Characters.CharacterType.arabiaCharacter01,
		Characters.CharacterType.losangelesCharacter01,
		Characters.CharacterType.carmen,
		Characters.CharacterType.roberto,
		Characters.CharacterType.kim,
		Characters.CharacterType.hawaiiCharacter01,
		Characters.CharacterType.vancouverCharacter01,
		Characters.CharacterType.tony,
		Characters.CharacterType.bangkokCharacter01,
		Characters.CharacterType.nick,
		Characters.CharacterType.cairoCharacter01,
		Characters.CharacterType.halloweenCharacter01,
		Characters.CharacterType.seoulCharacter01,
		Characters.CharacterType.saopauloCharacter01,
		Characters.CharacterType.mexicoCharacter01,
		Characters.CharacterType.lasVegasCharacter01,
		Characters.CharacterType.harumi,
		Characters.CharacterType.monkeyking,
		Characters.CharacterType.coco,
		Characters.CharacterType.mumbaiCharacter01,
		Characters.CharacterType.moscowCharacter01,
		Characters.CharacterType.veniceCharacter01,
		Characters.CharacterType.kenyaCharacter01,
		Characters.CharacterType.greeceCharacter01,
		Characters.CharacterType.sanfranciscoCharacter01,
		Characters.CharacterType.transylvaniaCharacter01,
		Characters.CharacterType.madagascarCharacter01,
		Characters.CharacterType.pragueCharacter01
	};

	public static event EventHandler<LetterCollectedEventArgs> OnLetterCollected;

	public CharacterNameEventManager(SSEventType eventType, long eventID)
	{
		_eventType = eventType;
		_eventID = eventID;
		if (eventID <= 0)
		{
			return;
		}
		if (PlayerInfo.Instance.characterNameEventData == string.Empty)
		{
			_eventProgressData = InitCharacterNameEventData();
		}
		else
		{
			_eventProgressData = DeserializeProgressFromJson();
			if (_eventProgressData.eventID != eventID)
			{
				_eventProgressData = InitCharacterNameEventData();
				AwardsData.ResetWordyWeekendChallengeOneWeekend();
			}
		}
		if (HasCompletedCollectionForSelectedCharacter())
		{
			SkipCurrentCharacter();
		}
	}

	private string SerializeProgressToJson()
	{
		string result = string.Empty;
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		if (_eventProgressData != null)
		{
			List<int> list = new List<int>();
			if (_eventProgressData.charactersList != null)
			{
				for (int i = 0; i < _eventProgressData.charactersList.Count; i++)
				{
					list.Add((int)_eventProgressData.charactersList[i]);
				}
			}
			dictionary.Add("charactersList", list);
			dictionary.Add("currentCharacter", (int)_eventProgressData.currentCharacter);
			dictionary.Add("collectedLetterIndex", _eventProgressData.collectedLetterIndex);
			dictionary.Add("completedCharacters", _eventProgressData.completedCharacters);
			dictionary.Add("skippedCharacters", _eventProgressData.skippedCharacters);
			dictionary.Add("wasLastCharacterCompleted", _eventProgressData.wasLastCharacterCompleted);
			dictionary.Add("eventID", _eventProgressData.eventID);
			result = Json.Serialize(dictionary);
		}
		return result;
	}

	private CharacterNameEventData DeserializeProgressFromJson()
	{
		CharacterNameEventData characterNameEventData = InitCharacterNameEventData();
		IDictionary<string, object> dictionary = Json.Deserialize(PlayerInfo.Instance.characterNameEventData) as IDictionary<string, object>;
		if (dictionary != null)
		{
			if (dictionary.ContainsKey("charactersList") && dictionary["charactersList"] != null)
			{
				List<object> list = dictionary["charactersList"] as List<object>;
				if (list != null && list.Count > 0)
				{
					List<Characters.CharacterType> list2 = new List<Characters.CharacterType>();
					for (int i = 0; i < list.Count; i++)
					{
						int num = Convert.ToInt32(list[i]);
						Characters.CharacterType item = (Characters.CharacterType)num;
						list2.Add(item);
					}
					if (list2.Count > 0)
					{
						characterNameEventData.charactersList = list2;
					}
				}
			}
			if (dictionary.ContainsKey("currentCharacter") && dictionary["currentCharacter"] != null)
			{
				int currentCharacter = Convert.ToInt32(dictionary["currentCharacter"]);
				characterNameEventData.currentCharacter = (Characters.CharacterType)currentCharacter;
			}
			if (dictionary.ContainsKey("collectedLetterIndex") && dictionary["collectedLetterIndex"] != null)
			{
				characterNameEventData.collectedLetterIndex = Convert.ToInt32(dictionary["collectedLetterIndex"]);
			}
			if (dictionary.ContainsKey("completedCharacters") && dictionary["completedCharacters"] != null)
			{
				characterNameEventData.completedCharacters = Convert.ToInt32(dictionary["completedCharacters"]);
			}
			if (dictionary.ContainsKey("skippedCharacters") && dictionary["skippedCharacters"] != null)
			{
				characterNameEventData.skippedCharacters = Convert.ToInt32(dictionary["skippedCharacters"]);
			}
			if (dictionary.ContainsKey("wasLastCharacterCompleted") && dictionary["wasLastCharacterCompleted"] != null)
			{
				characterNameEventData.wasLastCharacterCompleted = Convert.ToBoolean(dictionary["wasLastCharacterCompleted"]);
			}
			if (dictionary.ContainsKey("eventID") && dictionary["eventID"] != null)
			{
				characterNameEventData.eventID = Convert.ToInt64(dictionary["eventID"]);
			}
		}
		return characterNameEventData;
	}

	private CharacterNameEventData InitCharacterNameEventData()
	{
		List<Characters.CharacterType> list = CreateNewCharacterList();
		int index = 0;
		bool wasLastCharacterCompleted = false;
		int completedCharacters = 0;
		int skippedCharacters = 0;
		int collectedLetterIndex = -1;
		return new CharacterNameEventData(list, list[index], collectedLetterIndex, completedCharacters, skippedCharacters, wasLastCharacterCompleted, _eventID);
	}

	public List<Characters.CharacterType> GetCharacterList()
	{
		if (_eventProgressData != null && _eventProgressData.charactersList != null)
		{
			return _eventProgressData.charactersList;
		}
		return null;
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
		return _eventID > 0 && _eventProgressData != null && _eventProgressData.charactersList.Count > 0 && !HasLastCharacterBeenCompleted();
	}

	public T GetEventValue<T>(out bool found)
	{
		T result = default(T);
		found = false;
		string text = ((!Globals.DEBUG_USE_EVENT_TESTER) ? OnlineSettings.instance.GetValue("event_value", string.Empty) : EventTesterManager.Instance.GetEventValue());
		if (!string.IsNullOrEmpty(text) && typeof(T).IsArray)
		{
			try
			{
				string[] array = text.Split(';');
				Type elementType = typeof(T).GetElementType();
				Array array2 = Array.CreateInstance(elementType, array.Length);
				object[] array3 = Array.ConvertAll(array, (string item) => Convert.ChangeType(item, elementType));
				array3.CopyTo(array2, 0);
				result = (T)(object)array2;
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
			firstLine = Strings.Get(StringID.EVENTS_POPUP_WEEKEND_EVENT_TITLE_LINE_1);
			secondLine = Strings.Get(StringID.EVENTS_POPUP_WEEKEND_EVENT_TITLE_LINE_2);
		}
		else
		{
			firstLine = OnlineSettings.instance.GetValue("event_popup_title_first", Strings.Get(StringID.EVENTS_POPUP_WEEKEND_EVENT_TITLE_LINE_1));
			secondLine = OnlineSettings.instance.GetValue("event_popup_title_second", Strings.Get(StringID.EVENTS_POPUP_WEEKEND_EVENT_TITLE_LINE_2));
		}
	}

	public void GetPopupDescription(out string firstTitle, out string secondTitle)
	{
		string empty = string.Empty;
		string empty2 = string.Empty;
		if (SelectedCharacterMatchesFocusedCharacter())
		{
			empty = Strings.Get(StringID.EVENTS_POPUP_DESCRIPTION_TITLE_SELECT_CHARACTER_NAME);
			string text = string.Empty;
			if (_eventProgressData != null)
			{
				text = GetCharacterDisplayName(_eventProgressData.currentCharacter);
			}
			empty2 = text;
			if (Globals.DEBUG_USE_EVENT_TESTER)
			{
				firstTitle = empty;
				secondTitle = empty2;
			}
			else
			{
				firstTitle = OnlineSettings.instance.GetValue("event_popup_description_first_select_character_name", empty);
				secondTitle = OnlineSettings.instance.GetValue("event_popup_description_second_select_character_name", empty2);
			}
		}
		else
		{
			empty = Strings.Get(StringID.EVENTS_POPUP_DESCRIPTION_TITLE_UNLOCK_CHARACTER_NAME);
			string arg = string.Empty;
			if (_eventProgressData != null)
			{
				arg = Strings.Get(Characters.characterData[_eventProgressData.currentCharacter].name);
			}
			empty2 = string.Format(Strings.Get(StringID.EVENTS_POPUP_DESCRIPTION_SECOND_TITLE_UNLOCK_CHARACTER_NAME), arg);
			if (Globals.DEBUG_USE_EVENT_TESTER)
			{
				firstTitle = empty;
				secondTitle = empty2;
			}
			else
			{
				firstTitle = OnlineSettings.instance.GetValue("event_popup_description_first_unlock_character_name", empty);
				secondTitle = OnlineSettings.instance.GetValue("event_popup_description_second_unlock_character_name", empty2);
			}
		}
	}

	public void GetRibbonDescription(out string firstLine, out string secondLine, out string thirdLine)
	{
		firstLine = string.Empty;
		secondLine = string.Empty;
		thirdLine = string.Empty;
		Debug.LogError("The Character Name Event does not contain information for a ribbon description");
	}

	private void SaveProgress()
	{
		if (_eventProgressData != null)
		{
			PlayerInfo.Instance.characterNameEventData = SerializeProgressToJson();
		}
	}

	private List<Characters.CharacterType> CreateNewCharacterList()
	{
		List<Characters.CharacterType> list = new List<Characters.CharacterType>();
		bool found = false;
		int[] eventValue = GetEventValue<int[]>(out found);
		if (found && eventValue.Length >= 2)
		{
			int num = eventValue[0];
			int num2 = eventValue[1];
			if (num >= normalCharacters.Length || num < 0)
			{
				num = 0;
				Debug.LogError("CharacterEventManager - Specified standard list was negative or does not exist! Reverting to default.");
			}
			if (num2 >= extraCharacters.Length || num2 < 0)
			{
				num2 = 0;
				Debug.LogError("CharacterEventManager - ExtraCharacters list index number was negative or does not exist! Reverting to start.");
			}
			Characters.CharacterType?[] array = normalCharacters[num];
			for (int i = 0; i < array.Length; i++)
			{
				Characters.CharacterType? characterType = array[i];
				if (characterType.HasValue)
				{
					list.Add(characterType.Value);
					continue;
				}
				if (num2 < extraCharacters.Length)
				{
					list.Add(extraCharacters[num2]);
				}
				else
				{
					num2 = 0;
					list.Add(extraCharacters[num2]);
				}
				num2++;
			}
		}
		return list;
	}

	public List<Characters.CharacterType> GetCharactersToDisplay()
	{
		List<Characters.CharacterType> result = new List<Characters.CharacterType>();
		if (_eventProgressData != null)
		{
			result = TrimEventCharactersList(_eventProgressData.charactersList);
		}
		return result;
	}

	private List<Characters.CharacterType> TrimEventCharactersList(List<Characters.CharacterType> allEventCharacters)
	{
		List<Characters.CharacterType> list = new List<Characters.CharacterType>();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (_eventProgressData != null)
		{
			num3 = allEventCharacters.IndexOf(_eventProgressData.currentCharacter);
		}
		if (num3 == 0)
		{
			num = num3;
			num2 = num3 + 1;
		}
		else if (num3 == allEventCharacters.Count - 1)
		{
			num = num3 - 1;
			num2 = num3;
		}
		else
		{
			num = num3 - 1;
			num2 = num3 + 1;
		}
		int num4 = 0;
		foreach (Characters.CharacterType allEventCharacter in allEventCharacters)
		{
			if (num <= num4 && num4 <= num2)
			{
				list.Add(allEventCharacter);
			}
			num4++;
		}
		return list;
	}

	public int GetLastSelectedThemeForCharacterType(Characters.CharacterType charType)
	{
		int result = 0;
		if (CharacterThemes.GetThemeForCharacter(charType, PlayerInfo.Instance.GetIndexForLastSelectedTheme(charType)).HasValue)
		{
			result = PlayerInfo.Instance.GetIndexForLastSelectedTheme(charType);
		}
		return result;
	}

	public bool IsShowingFirstElement()
	{
		return _eventProgressData != null && _eventProgressData.charactersList.IndexOf(_eventProgressData.currentCharacter) == 0;
	}

	public bool IsShowingLastElement()
	{
		return _eventProgressData != null && _eventProgressData.charactersList.IndexOf(_eventProgressData.currentCharacter) == _eventProgressData.charactersList.Count - 1;
	}

	public void SkipCurrentCharacter()
	{
		if (_eventProgressData != null)
		{
			_eventProgressData.wasLastCharacterCompleted = HasCompletedCollectionForSelectedCharacter();
			if (_eventProgressData.wasLastCharacterCompleted)
			{
				_eventProgressData.completedCharacters++;
			}
			else
			{
				_eventProgressData.skippedCharacters++;
			}
			Characters.CharacterType? nextCharacterToDisplay = GetNextCharacterToDisplay();
			if (nextCharacterToDisplay.HasValue)
			{
				_eventProgressData.currentCharacter = nextCharacterToDisplay.Value;
				ResetCollectProgress();
			}
			SaveProgress();
		}
	}

	public Characters.CharacterType? GetNextCharacterToDisplay()
	{
		Characters.CharacterType? result = null;
		if (!IsLastCharacterSelected() && _eventProgressData != null)
		{
			int num = _eventProgressData.charactersList.IndexOf(_eventProgressData.currentCharacter);
			result = _eventProgressData.charactersList[num + 1];
		}
		return result;
	}

	public bool ShouldShowTheCompletedSign()
	{
		return _eventProgressData != null && _eventProgressData.wasLastCharacterCompleted;
	}

	public bool IsAbleToSkip()
	{
		bool flag = false;
		if (_eventProgressData != null)
		{
			Characters.CharacterType currentCharacter = _eventProgressData.currentCharacter;
			flag = ((Characters.characterData[currentCharacter].unlockType != Characters.UnlockType.facebook) ? PlayerInfo.Instance.IsCollectionComplete(currentCharacter) : (SocialManager.instance.facebookIsLoggedIn && PlayerInfo.Instance.IsCollectionComplete(currentCharacter)));
		}
		return (Globals.DEBUG_USE_EVENT_TESTER || !flag) && !IsLastCharacterSelected();
	}

	private bool HasLastCharacterBeenCompleted()
	{
		return IsLastCharacterSelected() && HasCompletedCollectionForSelectedCharacter();
	}

	public bool ShouldSpawnLetters()
	{
		return SelectedCharacterMatchesFocusedCharacter() && !HasCompletedCollectionForSelectedCharacter();
	}

	private void ResetCollectProgress()
	{
		if (_eventProgressData != null)
		{
			_eventProgressData.collectedLetterIndex = -1;
		}
	}

	public char? NextLetterToSpawn()
	{
		char? result = null;
		if (_eventProgressData != null)
		{
			char[] characterNameArray = GetCharacterNameArray(_eventProgressData.currentCharacter);
			if (characterNameArray.Length > 0 && !HasCompletedCollectionForSelectedCharacter())
			{
				result = ((_eventProgressData.collectedLetterIndex >= characterNameArray.Length - 1) ? new char?(characterNameArray[_eventProgressData.collectedLetterIndex]) : new char?(characterNameArray[_eventProgressData.collectedLetterIndex + 1]));
			}
		}
		return result;
	}

	public void NotifyLetterCollected(char collectedLetter)
	{
		if (_eventProgressData != null)
		{
			char[] characterNameArray = GetCharacterNameArray(_eventProgressData.currentCharacter);
			int num = -1;
			if (_eventProgressData.collectedLetterIndex < characterNameArray.Length - 1)
			{
				num = ((_eventProgressData.collectedLetterIndex < 0) ? Array.IndexOf(characterNameArray, collectedLetter) : Array.IndexOf(characterNameArray, collectedLetter, _eventProgressData.collectedLetterIndex + 1));
			}
			int num2 = num - _eventProgressData.collectedLetterIndex;
			if (num > -1 && num2 == 1)
			{
				_eventProgressData.collectedLetterIndex = num;
				UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.CharacterEventWord, _eventProgressData.collectedLetterIndex, new string(GetCharacterDisplayName(_eventProgressData.currentCharacter).ToCharArray())));
				if (HasCompletedCollectionForSelectedCharacter())
				{
					CelebrationReward celebrationReward = MysteryBox.Roll(MysteryBox.Type.Super);
					celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.CharacterEventSuperMysteryBox;
					celebrationReward.characterEventCompleted = _eventProgressData.completedCharacters + 1;
					celebrationReward.description = new string(characterNameArray);
					RewardManager.AddRewardToUnlock(celebrationReward, false);
					UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.CharacterEventWordReward, new string(GetCharacterDisplayName(_eventProgressData.currentCharacter).ToCharArray())));
					Statistics stats;
					Statistics statistics = (stats = PlayerInfo.Instance.stats);
					Stat stat2;
					Stat stat = (stat2 = Stat.WordyWeekendChallengesCompleted);
					int num3 = stats[stat2];
					statistics[stat] = num3 + 1;
					Statistics stats2;
					Statistics statistics2 = (stats2 = PlayerInfo.Instance.stats);
					Stat stat3 = (stat2 = Stat.WordyWeekendChallengesCompletedOneWeekend);
					num3 = stats2[stat2];
					statistics2[stat3] = num3 + 1;
				}
				SaveProgress();
			}
		}
		if (CharacterNameEventManager.OnLetterCollected != null)
		{
			CharacterNameEventManager.OnLetterCollected(this, new LetterCollectedEventArgs(collectedLetter));
		}
	}

	public int GetAmountOfCollectedLetters()
	{
		int result = 0;
		if (_eventProgressData != null)
		{
			result = _eventProgressData.collectedLetterIndex + 1;
		}
		return result;
	}

	public bool HasCompletedCollectionForSelectedCharacter()
	{
		bool result = false;
		if (_eventProgressData != null)
		{
			char[] characterNameArray = GetCharacterNameArray(_eventProgressData.currentCharacter);
			result = characterNameArray.Length > 0 && _eventProgressData.collectedLetterIndex == characterNameArray.Length - 1;
		}
		return result;
	}

	public bool IsFirstCharacterSelected()
	{
		return _eventProgressData != null && _eventProgressData.charactersList.Contains(_eventProgressData.currentCharacter) && _eventProgressData.charactersList.IndexOf(_eventProgressData.currentCharacter) == 0;
	}

	public bool IsLastCharacterSelected()
	{
		return _eventProgressData != null && _eventProgressData.charactersList.Contains(_eventProgressData.currentCharacter) && _eventProgressData.charactersList.IndexOf(_eventProgressData.currentCharacter) == _eventProgressData.charactersList.Count - 1;
	}

	private char[] GetCharacterNameArray(Characters.CharacterType characterType)
	{
		string text = FilterCharacterName(Strings.Get(Characters.characterData[characterType].name));
		return text.ToUpper().ToCharArray();
	}

	private string GetCharacterDisplayName(Characters.CharacterType characterType)
	{
		return Strings.Get(Characters.characterData[characterType].name).Trim().ToUpper();
	}

	private string FilterCharacterName(string characterName)
	{
		char[] fILTERED_CHARACTERS = FILTERED_CHARACTERS;
		foreach (char c in fILTERED_CHARACTERS)
		{
			characterName = characterName.Replace(c.ToString(), string.Empty);
		}
		return characterName;
	}

	public void UnHookFromGameEvents()
	{
		SaveProgress();
	}

	public int GetNumberOfKeysToSkip()
	{
		int result = 2;
		if (_eventProgressData != null)
		{
			result = _eventProgressData.skippedCharacters + 3;
		}
		return result;
	}

	public bool CanAffordToSkip()
	{
		return PlayerInfo.Instance.amountOfKeys - GetNumberOfKeysToSkip() >= 0;
	}

	public void UseKeysToSkip()
	{
		int numberOfKeysToSkip = GetNumberOfKeysToSkip();
		if (_eventProgressData != null)
		{
			Flurry.LogEventWithSeveralParameters("Skipped character for Wordy Weekend", "Character;Price", string.Concat(_eventProgressData.currentCharacter, ";", numberOfKeysToSkip));
		}
		MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendKeys, numberOfKeysToSkip);
		PlayerInfo.Instance.amountOfKeys -= numberOfKeysToSkip;
	}

	public bool SelectedCharacterMatchesFocusedCharacter()
	{
		return _eventProgressData != null && _eventProgressData.currentCharacter == (Characters.CharacterType)PlayerInfo.Instance.currentCharacter;
	}
}
