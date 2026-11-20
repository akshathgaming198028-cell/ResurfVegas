using System;
using System.Collections.Generic;
using System.Linq;
using SYBO.Subway.Info;
using UnityEngine;

public class EventTesterPopup : UIScreen
{
	[SerializeField]
	private SelectPopupButton _eventTypePopupButton;

	[SerializeField]
	private SelectPopupButton _charPopupButton;

	[SerializeField]
	private SelectPopupButton _boardPopupButton;

	[SerializeField]
	private SelectPopupButton _hoursLeftPopupButton;

	[SerializeField]
	private GameObject boardTypeButton;

	[SerializeField]
	private GameObject characterTypeButton;

	[SerializeField]
	private GameObject enableButton;

	[SerializeField]
	private GameObject disableButton;

	private string _hoursLeft;

	private string _charType;

	private string _boardType;

	private string _eventType;

	private void Start()
	{
		EventDelegate.Add(_eventTypePopupButton.onChange, EventTypeSelected);
		_eventTypePopupButton.SetItems(Enum.GetNames(typeof(SSEventType)).ToList());
		List<string> items = Enum.GetNames(typeof(Characters.CharacterType)).ToList();
		EventDelegate.Add(_charPopupButton.onChange, CharSelected);
		_charPopupButton.SetItems(items);
		List<string> items2 = Enum.GetNames(typeof(Hoverboards.BoardType)).ToList();
		EventDelegate.Add(_boardPopupButton.onChange, BoardSelected);
		_boardPopupButton.SetItems(items2);
		EventDelegate.Add(_hoursLeftPopupButton.onChange, NewTimeLimitSelected);
	}

	public void EventTypeSelected()
	{
		characterTypeButton.SetActive(false);
		boardTypeButton.SetActive(false);
		_eventType = _eventTypePopupButton.value;
		switch (_eventType)
		{
		case "CharacterDay":
		case "ExpiredLimitedCharacter":
			characterTypeButton.SetActive(true);
			break;
		case "ExpiredLimitedBoard":
		case "SpecialPowerBoard":
			boardTypeButton.SetActive(true);
			break;
		case "CharacterName":
			break;
		}
	}

	public void CharSelected()
	{
		_charType = _charPopupButton.value;
	}

	public void BoardSelected()
	{
		_boardType = _boardPopupButton.value;
	}

	public void NewTimeLimitSelected()
	{
		_hoursLeft = _hoursLeftPopupButton.value;
	}

	public void EnableEvent()
	{
		enableButton.SetActive(false);
		disableButton.SetActive(true);
		Globals.DEBUG_USE_EVENT_TESTER = true;
		EventTesterManager.Instance.UpdateInfo(_eventType, _charType, _boardType, _hoursLeft);
		BoardScreenManager.Instance.forceRefreshOnBoardScreen = true;
		CharacterScreen.forceRefreshOnCharacterScreen = true;
		OnlineSettings.instance.DownloadNow();
		UIScreenController.Instance.ClosePopup();
		UIScreenController.Instance.ClosePopup();
		UIScreenController.Instance.ClosePopup();
	}

	public void DisableEvent()
	{
		enableButton.SetActive(true);
		disableButton.SetActive(false);
		Globals.DEBUG_USE_EVENT_TESTER = false;
		BoardScreenManager.Instance.forceRefreshOnBoardScreen = true;
		CharacterScreen.forceRefreshOnCharacterScreen = true;
		OnlineSettings.instance.DownloadNow();
		UIScreenController.Instance.ClosePopup();
		UIScreenController.Instance.ClosePopup();
		UIScreenController.Instance.ClosePopup();
	}
}
