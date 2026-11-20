using System;
using System.Collections;
using System.Collections.Generic;
using SYBO.Subway.Info;
using UnityEngine;

public class SSEventsPopup : UIScreen
{
	[Serializable]
	private class EventTypeGameObjectPair
	{
		public SSEventType _eventType;

		public GameObject _gameObject;
	}

	private const string ONLINE_SETTING_EVENT_ELEMENT_NEW_POSITION_OFFSET = "event_popup_element_new_position_offset";

	private const string ONLINE_SETTING_EVENT_ELEMENT_NEW_ROTATION = "event_popup_element_new_rotation";

	private const string ONLINE_SETTING_EVENT_ELEMENT_NEW_SCALE = "event_popup_element_new_scale";

	private const float COIN_SPRITE_OFFSET = 2f;

	[SerializeField]
	private List<EventTypeGameObjectPair> _stateGameObjects = new List<EventTypeGameObjectPair>();

	private static Vector3 SPECIAL_POWER_BOARD_POSITION_OFFSET = new Vector3(-3f, 5f, 0f);

	private static Vector3 SPECIAL_POWER_BOARD_ROTATION = new Vector3(17.5f, 124f, -52.6f);

	private static Vector3 SPECIAL_POWER_BOARD_SCALE = new Vector3(8f, 8f, 8f);

	private static Vector3 LIMITED_CHAR_POSITION_OFFSET = new Vector3(0f, -37f, 0f);

	private static Vector3 LIMITED_CHAR_ROTATION = new Vector3(13f, -157f, 3f);

	private static Vector3 LIMITED_CHAR_SCALE = new Vector3(8f, 8f, 8f);

	private static Vector3 CHARACTER_DAY_MODEL_POSITION_OFFSET = new Vector3(-2f, -37.5f, 0f);

	private static Vector3 CHARACTER_DAY_MODEL_ROTATION = new Vector3(13f, -157f, 3f);

	private static Vector3 CHARACTER_DAY_MODEL_SCALE = new Vector3(8f, 8f, 8f);

	private static Vector3 MYSTERY_POWERUP_POSITION_OFFSET = new Vector3(0f, -35f, 0f);

	private static Vector3 MYSTERY_POWERUP_ROTATION = new Vector3(10f, 186f, 0f);

	private static Vector3 MYSTERY_POWERUP_SCALE = new Vector3(4.5f, 4.5f, 4.5f);

	private GameObject _specialPowerBoard;

	private GameObject _limitedCharacter;

	private GameObject _mysteryPowerup;

	[SerializeField]
	private GameObject _widgetsContainer;

	[SerializeField]
	private UISprite _bgFill;

	[SerializeField]
	private UISprite _bgOutline;

	[SerializeField]
	private UILabel _eventTitleLine1;

	[SerializeField]
	private UILabel _eventTitleLine2;

	[SerializeField]
	private UILabel _clockLabel;

	[SerializeField]
	private UILabel _messageTitleLabel;

	[SerializeField]
	private UILabel _messageSecondTitleLabel;

	[SerializeField]
	private UILabel _checkItOutLabel;

	[SerializeField]
	private GameObject _twoLinesDescription;

	[SerializeField]
	private GameObject _jackpotBottomElements;

	[SerializeField]
	private GameObject _jackpotTopElements;

	[SerializeField]
	private GameObject _goToItemButton;

	[SerializeField]
	private UILabel _jackpotsDescriptionLabel;

	[SerializeField]
	private UILabel _MBJackpotLabel;

	[SerializeField]
	private UILabel _SMBJackpotLabel;

	[SerializeField]
	private UISprite _coinLeft;

	[SerializeField]
	private UISprite _coinRight;

	[SerializeField]
	private UISprite _bottomDescriptionBoxSprite;

	[SerializeField]
	private UISprite _topSplatSprite;

	[SerializeField]
	private UISprite _glowEffectSprite;

	[SerializeField]
	private GameObject _descriptionEffectsParent;

	[SerializeField]
	private List<UISprite> _listBgStripes = new List<UISprite>();

	[SerializeField]
	private GameObject _mysteryPowerupPrefab;

	private IEventManager _activeEventManager;

	private readonly Vector2 _dimensionsBgFillNormalEvent = new Vector2(270f, 213f);

	private readonly Vector2 _dimensionsBgOutlineNormalEvent = new Vector2(280f, 226f);

	private readonly Vector2 _dimensionsBgFillCheckOutItemEvent = new Vector2(270f, 280f);

	private readonly Vector2 _dimensionsBgOutlineCheckOutItemEvent = new Vector2(280f, 292f);

	private readonly Vector2 _dimensionsBgFillExtendedCheckOutItemEvent = new Vector2(270f, 305f);

	private readonly Vector2 _dimensionsBgOutlineExtendedCheckOutItemEvent = new Vector2(280f, 317f);

	private readonly Vector3 _buttonPosition = new Vector3(-3f, -131f, 0f);

	private readonly Vector3 _buttonPositionWhenExtended = new Vector3(-3f, -156f, 0f);

	private readonly int _descriptionBoxNormalHeight = 80;

	private readonly int _descriptionBoxExtendedHeight = 105;

	private readonly int _rayEffectNormalHeight = 43;

	private readonly int _rayEffectExtendedHeight = 55;

	private readonly Vector3 _effectsNormalPosition = new Vector3(-2f, -7f, 0f);

	private readonly Vector3 _effectsExtendedPosition = new Vector3(-2f, -19f, 0f);

	private void Awake()
	{
		if (_eventTitleLine1 == null || _eventTitleLine2 == null || _clockLabel == null || _messageTitleLabel == null || _messageSecondTitleLabel == null || _checkItOutLabel == null)
		{
			Debug.LogError("Not all the Serialized Fields in SSEventsPopup are set properly");
		}
	}

	private void Update()
	{
		SetTheClock();
	}

	public override void Show()
	{
		base.Show();
		_activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		UpdateUIElements();
		SetLabelsTexts();
		PresentCurrentEventAvailable();
	}

	private void SetTheClock()
	{
		long num = SSEventsManagerController.instance.TimeLeftInSeconds();
		if (num > 0)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(num);
			_clockLabel.text = string.Format(Strings.Get(StringID.EVENTS_POPUP_CLOCK), string.Format("{0:00}:{1:00}:{2:00}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds));
		}
		else
		{
			_clockLabel.text = Strings.Get(StringID.EVENT_POPUP_EVENT_EXPIRED);
		}
	}

	private void UpdateUIElements()
	{
		if (_activeEventManager == null)
		{
			return;
		}
		switch (_activeEventManager.GetEventType())
		{
		case SSEventType.BiggerJackpot:
		{
			_goToItemButton.SetActive(true);
			UISprite bgFill3 = _bgFill;
			Vector2 dimensionsBgFillExtendedCheckOutItemEvent = _dimensionsBgFillExtendedCheckOutItemEvent;
			bgFill3.width = Mathf.RoundToInt(dimensionsBgFillExtendedCheckOutItemEvent.x);
			UISprite bgFill4 = _bgFill;
			Vector2 dimensionsBgFillExtendedCheckOutItemEvent2 = _dimensionsBgFillExtendedCheckOutItemEvent;
			bgFill4.height = Mathf.RoundToInt(dimensionsBgFillExtendedCheckOutItemEvent2.y);
			UISprite bgOutline3 = _bgOutline;
			Vector2 dimensionsBgOutlineExtendedCheckOutItemEvent = _dimensionsBgOutlineExtendedCheckOutItemEvent;
			bgOutline3.width = Mathf.RoundToInt(dimensionsBgOutlineExtendedCheckOutItemEvent.x);
			UISprite bgOutline4 = _bgOutline;
			Vector2 dimensionsBgOutlineExtendedCheckOutItemEvent2 = _dimensionsBgOutlineExtendedCheckOutItemEvent;
			bgOutline4.height = Mathf.RoundToInt(dimensionsBgOutlineExtendedCheckOutItemEvent2.y);
			_goToItemButton.transform.localPosition = _buttonPositionWhenExtended;
			_bottomDescriptionBoxSprite.height = _descriptionBoxExtendedHeight;
			_descriptionEffectsParent.transform.localPosition = _effectsExtendedPosition;
			if (_listBgStripes[0].height == _rayEffectExtendedHeight)
			{
				break;
			}
			foreach (UISprite listBgStripe in _listBgStripes)
			{
				listBgStripe.height = _rayEffectExtendedHeight;
			}
			break;
		}
		case SSEventType.SpecialPowerBoard:
		case SSEventType.ExpiredLimitedCharacter:
		case SSEventType.ExpiredLimitedBoard:
		case SSEventType.CharacterDay:
		{
			_goToItemButton.SetActive(true);
			UISprite bgFill5 = _bgFill;
			Vector2 dimensionsBgFillCheckOutItemEvent = _dimensionsBgFillCheckOutItemEvent;
			bgFill5.width = Mathf.RoundToInt(dimensionsBgFillCheckOutItemEvent.x);
			UISprite bgFill6 = _bgFill;
			Vector2 dimensionsBgFillCheckOutItemEvent2 = _dimensionsBgFillCheckOutItemEvent;
			bgFill6.height = Mathf.RoundToInt(dimensionsBgFillCheckOutItemEvent2.y);
			UISprite bgOutline5 = _bgOutline;
			Vector2 dimensionsBgOutlineCheckOutItemEvent = _dimensionsBgOutlineCheckOutItemEvent;
			bgOutline5.width = Mathf.RoundToInt(dimensionsBgOutlineCheckOutItemEvent.x);
			UISprite bgOutline6 = _bgOutline;
			Vector2 dimensionsBgOutlineCheckOutItemEvent2 = _dimensionsBgOutlineCheckOutItemEvent;
			bgOutline6.height = Mathf.RoundToInt(dimensionsBgOutlineCheckOutItemEvent2.y);
			_goToItemButton.transform.localPosition = _buttonPosition;
			_bottomDescriptionBoxSprite.height = _descriptionBoxNormalHeight;
			_descriptionEffectsParent.transform.localPosition = _effectsNormalPosition;
			if (_listBgStripes[0].height == _rayEffectNormalHeight)
			{
				break;
			}
			foreach (UISprite listBgStripe2 in _listBgStripes)
			{
				listBgStripe2.height = _rayEffectNormalHeight;
			}
			break;
		}
		default:
		{
			_goToItemButton.SetActive(false);
			UISprite bgFill = _bgFill;
			Vector2 dimensionsBgFillNormalEvent = _dimensionsBgFillNormalEvent;
			bgFill.width = Mathf.RoundToInt(dimensionsBgFillNormalEvent.x);
			UISprite bgFill2 = _bgFill;
			Vector2 dimensionsBgFillNormalEvent2 = _dimensionsBgFillNormalEvent;
			bgFill2.height = Mathf.RoundToInt(dimensionsBgFillNormalEvent2.y);
			UISprite bgOutline = _bgOutline;
			Vector2 dimensionsBgOutlineNormalEvent = _dimensionsBgOutlineNormalEvent;
			bgOutline.width = Mathf.RoundToInt(dimensionsBgOutlineNormalEvent.x);
			UISprite bgOutline2 = _bgOutline;
			Vector2 dimensionsBgOutlineNormalEvent2 = _dimensionsBgOutlineNormalEvent;
			bgOutline2.height = Mathf.RoundToInt(dimensionsBgOutlineNormalEvent2.y);
			_bottomDescriptionBoxSprite.height = _descriptionBoxNormalHeight;
			_descriptionEffectsParent.transform.localPosition = _effectsNormalPosition;
			if (_listBgStripes[0].height == _rayEffectNormalHeight)
			{
				break;
			}
			foreach (UISprite listBgStripe3 in _listBgStripes)
			{
				listBgStripe3.height = _rayEffectNormalHeight;
			}
			break;
		}
		}
		foreach (UISprite listBgStripe4 in _listBgStripes)
		{
			if (_activeEventManager.GetEventType() == SSEventType.BiggerJackpot)
			{
				listBgStripe4.color = GlobalColors.EVENTS_POPUP_STRIPES_JACKPOT;
			}
			else
			{
				listBgStripe4.color = GlobalColors.EVENTS_POPUP_STRIPES_NORMAL;
			}
		}
	}

	private void SetLabelsTexts()
	{
		if (_activeEventManager != null)
		{
			string firstLine = string.Empty;
			string secondLine = string.Empty;
			string secondTitle = string.Empty;
			string firstTitle = string.Empty;
			if (_activeEventManager != null)
			{
				_activeEventManager.GetPopupTitle(out firstLine, out secondLine);
				_activeEventManager.GetPopupDescription(out firstTitle, out secondTitle);
			}
			_eventTitleLine1.text = firstLine;
			_eventTitleLine2.text = secondLine;
			if (_activeEventManager.GetEventType() == SSEventType.BiggerJackpot)
			{
				_twoLinesDescription.SetActive(false);
				_jackpotBottomElements.SetActive(true);
				_jackpotTopElements.SetActive(true);
				_jackpotsDescriptionLabel.text = firstTitle;
				_MBJackpotLabel.text = string.Format("{0:#,###0}", MysteryBox.GetMegaJackpotAmount(MysteryBox.Type.Normal));
				_SMBJackpotLabel.text = string.Format("{0:#,###0}", MysteryBox.GetMegaJackpotAmount(MysteryBox.Type.Super));
				_bottomDescriptionBoxSprite.color = GlobalColors.EVENTS_POPUP_DESCRIPTION_BOX_JACKPOT_EVENT_COLOR;
				_topSplatSprite.color = GlobalColors.EVENTS_POPUP_DESCRIPTION_BOX_NORMAL_COLOR;
				_glowEffectSprite.color = GlobalColors.EVENTS_POPUP_GLOW_JACKPOT_COLOR;
			}
			else
			{
				_twoLinesDescription.SetActive(true);
				_jackpotBottomElements.SetActive(false);
				_jackpotTopElements.SetActive(false);
				_messageTitleLabel.text = firstTitle;
				_messageSecondTitleLabel.text = secondTitle;
				_bottomDescriptionBoxSprite.color = GlobalColors.EVENTS_POPUP_DESCRIPTION_BOX_NORMAL_COLOR;
				_topSplatSprite.color = GlobalColors.EVENTS_POPUP_SPLASH_COLOR;
				_glowEffectSprite.color = GlobalColors.EVENTS_POPUP_GLOW_DEFAULT_COLOR;
				_messageSecondTitleLabel.fontSize = ((!(_messageSecondTitleLabel.localSize.x / _bottomDescriptionBoxSprite.localSize.x > 0.95f)) ? 16 : 15);
			}
			_checkItOutLabel.text = Strings.Get(StringID.EVENTS_POPUP_CHECKOUT_BUTTON_TITLE);
		}
	}

	private void PresentCurrentEventAvailable()
	{
		if (_activeEventManager == null)
		{
			return;
		}
		SSEventType activeEventType = _activeEventManager.GetEventType();
		DisableAllStates();
		if (activeEventType != SSEventType.None)
		{
			_stateGameObjects.Find((EventTypeGameObjectPair pair) => pair._eventType == activeEventType)._gameObject.SetActive(true);
		}
		else
		{
			_messageTitleLabel.text = Strings.Get(StringID.EVENT_POPUP_NOT_AVAILABLE_DESCRIPTION);
			_messageSecondTitleLabel.text = string.Empty;
		}
		SetExtraSettingsForState(activeEventType);
	}

	private void SetExtraSettingsForState(SSEventType eventType)
	{
		switch (eventType)
		{
		case SSEventType.SpecialPowerBoard:
			AddSpecialPowerBoardEvent3dElements();
			break;
		case SSEventType.ExpiredLimitedCharacter:
		case SSEventType.CharacterDay:
			AddLimitedCharacter3dElements(eventType);
			break;
		case SSEventType.ExpiredLimitedBoard:
			AddSpecialPowerBoardEvent3dElements();
			break;
		case SSEventType.BiggerJackpot:
			PositionCoinIcons();
			break;
		case SSEventType.MysteryPowerup:
			PositionMysteryPowerup3dElements();
			break;
		case SSEventType.SuperMysteryBox:
			break;
		}
	}

	private void PositionMysteryPowerup3dElements()
	{
		if (_mysteryPowerup == null)
		{
			_mysteryPowerup = PlacePrefabUnderAnchor(_mysteryPowerupPrefab, UIModelController.Instance.TutorialPopupAnchor.transform, 31);
		}
		if (_mysteryPowerup != null)
		{
			Vector3 positionOffset = VerifySettingOnline("event_popup_element_new_position_offset", MYSTERY_POWERUP_POSITION_OFFSET);
			Vector3 newRotation = VerifySettingOnline("event_popup_element_new_rotation", MYSTERY_POWERUP_ROTATION);
			Vector3 newScale = VerifySettingOnline("event_popup_element_new_scale", MYSTERY_POWERUP_SCALE);
			PositionModelOnSprite(_topSplatSprite, _mysteryPowerup, positionOffset, newRotation, newScale);
		}
	}

	private void PositionCoinIcons()
	{
		_coinLeft.transform.localPosition = new Vector3(_MBJackpotLabel.transform.localPosition.x + 2f, _MBJackpotLabel.transform.localPosition.y, 0f);
		_coinRight.transform.localPosition = new Vector3(_SMBJackpotLabel.transform.localPosition.x + 2f, _SMBJackpotLabel.transform.localPosition.y, 0f);
	}

	private void AddSpecialPowerBoardEvent3dElements()
	{
		bool found = false;
		Hoverboards.BoardType? boardType = null;
		if (_activeEventManager != null)
		{
			SSEventType eventType = _activeEventManager.GetEventType();
			if (eventType == SSEventType.ExpiredLimitedBoard || eventType == SSEventType.SpecialPowerBoard)
			{
				boardType = _activeEventManager.GetEventValue<Hoverboards.BoardType>(out found);
			}
		}
		if (!found || !(_specialPowerBoard == null))
		{
			return;
		}
		Hoverboards.Board board = Hoverboards.boardData[boardType.Value];
		_specialPowerBoard = HoverboardModelPreviewFactory.Instance.GetHoverboardModelPreview(board.boardModelName, boardType.Value);
		if (_specialPowerBoard == null)
		{
			Debug.LogWarning("Can not instantiate special power board from Online Settings");
			return;
		}
		GameObject gameObject = PlaceModelUnderAnchor(_specialPowerBoard, UIModelController.Instance.TutorialPopupAnchor.transform, 31).transform.GetChild(0).gameObject;
		if (gameObject != null)
		{
			Vector3 positionOffset = VerifySettingOnline("event_popup_element_new_position_offset", SPECIAL_POWER_BOARD_POSITION_OFFSET);
			Vector3 newRotation = VerifySettingOnline("event_popup_element_new_rotation", SPECIAL_POWER_BOARD_ROTATION);
			Vector3 newScale = VerifySettingOnline("event_popup_element_new_scale", SPECIAL_POWER_BOARD_SCALE);
			PositionModelOnSprite(_topSplatSprite, gameObject, positionOffset, newRotation, newScale);
		}
	}

	private void AddLimitedCharacter3dElements(SSEventType eventType)
	{
		bool found = false;
		Characters.CharacterType? characterType = null;
		if (_activeEventManager != null)
		{
			characterType = _activeEventManager.GetEventValue<Characters.CharacterType>(out found);
		}
		if (!found)
		{
			return;
		}
		try
		{
			if (!(_limitedCharacter == null))
			{
				return;
			}
			Characters.Model model = Characters.characterData[characterType.Value];
			CharacterModelPreviewFactory.Instance.GetEventScreenModel(model.modelName, 0, delegate(GameObject lC)
			{
				_limitedCharacter = lC;
				if (_limitedCharacter == null)
				{
					Debug.LogWarning("Can not instantiate limited character from Online Settings");
				}
				else
				{
					GameObject gameObject = PlaceModelUnderAnchor(_limitedCharacter, UIModelController.Instance.TutorialPopupAnchor.transform, 31);
					if (gameObject != null)
					{
						Vector3 positionOffset = Vector3.zero;
						Vector3 newRotation = Vector3.zero;
						Vector3 newScale = Vector3.zero;
						if (eventType == SSEventType.ExpiredLimitedCharacter)
						{
							positionOffset = VerifySettingOnline("event_popup_element_new_position_offset", LIMITED_CHAR_POSITION_OFFSET);
							newRotation = VerifySettingOnline("event_popup_element_new_rotation", LIMITED_CHAR_ROTATION);
							newScale = VerifySettingOnline("event_popup_element_new_scale", LIMITED_CHAR_SCALE);
						}
						else if (eventType == SSEventType.CharacterDay)
						{
							positionOffset = VerifySettingOnline("event_popup_element_new_position_offset", CHARACTER_DAY_MODEL_POSITION_OFFSET);
							newRotation = VerifySettingOnline("event_popup_element_new_rotation", CHARACTER_DAY_MODEL_ROTATION);
							newScale = VerifySettingOnline("event_popup_element_new_scale", CHARACTER_DAY_MODEL_SCALE);
						}
						PositionModelOnSprite(_topSplatSprite, gameObject, positionOffset, newRotation, newScale);
					}
				}
			});
		}
		catch (ArgumentException ex)
		{
			Debug.LogError("Limited character name is not accurate from Online Setting! : " + ex.StackTrace);
		}
	}

	private GameObject PlaceModelUnderAnchor(GameObject model, Transform anchor, int layer)
	{
		NGUITools.SetLayer(model, layer);
		model.transform.parent = anchor;
		model.transform.localPosition = Vector3.zero;
		model.transform.localEulerAngles = Vector3.zero;
		model.transform.localScale = Vector3.one;
		return model;
	}

	private GameObject PlacePrefabUnderAnchor(GameObject prefab, Transform anchor, int layer)
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
		NGUITools.SetLayer(gameObject, layer);
		gameObject.transform.parent = anchor;
		gameObject.transform.localScale = Vector3.one;
		return gameObject;
	}

	private Vector3 VerifySettingOnline(string key, Vector3 defaultVal)
	{
		string valueString = string.Empty;
		if (OnlineSettings.instance.TryGetValue(key, out valueString))
		{
			string[] array = valueString.Split(',');
			if (array.Length == 3)
			{
				try
				{
					return new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
				}
				catch (Exception ex)
				{
					Debug.LogWarning("Wrong pos, scale or rotation in Online Setting for the Event element: " + ex.StackTrace);
				}
			}
		}
		return defaultVal;
	}

	private void OnDisable()
	{
		if (_specialPowerBoard != null)
		{
			_specialPowerBoard.SetActive(false);
		}
		if (_limitedCharacter != null)
		{
			_limitedCharacter.SetActive(false);
		}
		if (_mysteryPowerup != null)
		{
			_mysteryPowerup.SetActive(false);
		}
		SSEventsManagerController.OnSSEventFinishedHandler -= HidePopupIfNeeded;
		UIScreenController.OnApplicationResumed -= HidePopupIfNeeded;
	}

	private void OnEnable()
	{
		_activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (_activeEventManager != null)
		{
			SSEventType eventType = _activeEventManager.GetEventType();
			if (_specialPowerBoard != null)
			{
				_specialPowerBoard.SetActive(eventType == SSEventType.SpecialPowerBoard || eventType == SSEventType.ExpiredLimitedBoard);
			}
			if (_limitedCharacter != null)
			{
				_limitedCharacter.SetActive(eventType == SSEventType.ExpiredLimitedCharacter || eventType == SSEventType.CharacterDay);
			}
			if (_mysteryPowerup != null)
			{
				_mysteryPowerup.SetActive(eventType == SSEventType.MysteryPowerup);
			}
		}
		SSEventsManagerController.OnSSEventFinishedHandler += HidePopupIfNeeded;
		UIScreenController.OnApplicationResumed += HidePopupIfNeeded;
	}

	private void DisableAllStates()
	{
		foreach (EventTypeGameObjectPair stateGameObject in _stateGameObjects)
		{
			stateGameObject._gameObject.SetActive(false);
		}
	}

	public static void ShowIfNeeded()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (activeEventManager == null || !PlayerInfo.Instance.IsTutorialCompleted)
		{
			return;
		}
		long lastTimeEventPopupHasBeenForcedShown = PlayerInfo.Instance.lastTimeEventPopupHasBeenForcedShown;
		long eventID = activeEventManager.GetEventID();
		if (eventID > 0 && (lastTimeEventPopupHasBeenForcedShown == 0L || eventID != lastTimeEventPopupHasBeenForcedShown))
		{
			if (activeEventManager.GetEventType() == SSEventType.CharacterName)
			{
				UIScreenController.Instance.QueuePopup("CharacterNameEventPopup");
			}
			else
			{
				UIScreenController.Instance.QueuePopup("EventsPopup");
			}
			PlayerInfo.Instance.lastTimeEventPopupHasBeenForcedShown = eventID;
			Flurry.LogEvent("Event Popup was shown");
		}
	}

	private void HidePopupIfNeeded()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (_activeEventManager == null || activeEventManager == null || activeEventManager.GetEventID() != _activeEventManager.GetEventID())
		{
			if (_limitedCharacter != null)
			{
				_limitedCharacter.SetActive(false);
			}
			if (_specialPowerBoard != null)
			{
				_specialPowerBoard.SetActive(false);
			}
			UIScreenController.Instance.CloseActivePopups(screenName);
		}
	}

	public void GoToEventItem()
	{
		if (_activeEventManager == null)
		{
			return;
		}
		switch (_activeEventManager.GetEventType())
		{
		case SSEventType.ExpiredLimitedCharacter:
		case SSEventType.CharacterDay:
			if (UIScreenController.IsInstanced)
			{
				if (_limitedCharacter != null)
				{
					_limitedCharacter.SetActive(false);
				}
				UIScreenController.Instance.ignoreLastPopupClosed = true;
				UIScreenController.Instance.doSaveAndDelayRemainingPopups = true;
				UIScreenController.Instance.CloseActivePopups(screenName);
				CharacterScreen.forceCenterOnEventCharacter = true;
				UIScreenController.Instance.PushScreen("CharacterScreen");
			}
			break;
		case SSEventType.SpecialPowerBoard:
		case SSEventType.ExpiredLimitedBoard:
			if (UIScreenController.IsInstanced)
			{
				if (_specialPowerBoard != null)
				{
					_specialPowerBoard.SetActive(false);
				}
				UIScreenController.Instance.ignoreLastPopupClosed = true;
				UIScreenController.Instance.doSaveAndDelayRemainingPopups = true;
				UIScreenController.Instance.CloseActivePopups(screenName);
				BoardScreenManager.Instance.forceCenterOnSpecificBoard = true;
				UIScreenController.Instance.PushScreen("BoardScreen");
			}
			break;
		case SSEventType.BiggerJackpot:
			if (UIScreenController.IsInstanced)
			{
				UIScreenController.Instance.ignoreLastPopupClosed = true;
				UIScreenController.Instance.doSaveAndDelayRemainingPopups = true;
				UIScreenController.Instance.CloseActivePopups(screenName);
				UpgradeScreen.forceScrollRearange = true;
				UIScreenController.Instance.PushScreen("UpgradesUI_shop");
			}
			break;
		}
	}

	private void PositionModelOnSprite(UISprite tracker, GameObject objectToPosition, Vector3 positionOffset, Vector3 newRotation, Vector3 newScale)
	{
		StartCoroutine(DelayPositioning(tracker, objectToPosition, positionOffset, newRotation, newScale));
	}

	private IEnumerator DelayPositioning(UISprite tracker, GameObject objectToPosition, Vector3 positionOffset, Vector3 newRotation, Vector3 newScale)
	{
		yield return new WaitForEndOfFrame();
		Vector3 trackerPos = tracker.transform.position;
		Vector3 pixelPos = UIScreenController.Instance.Camera2DNormal.WorldToScreenPoint(trackerPos);
		pixelPos = new Vector3(pixelPos.x, pixelPos.y, 1f);
		Vector3 worldPos = UIScreenController.Instance.Camera3D.ScreenToWorldPoint(pixelPos);
		objectToPosition.transform.position = worldPos;
		objectToPosition.transform.localPosition = objectToPosition.transform.localPosition + positionOffset;
		objectToPosition.transform.localRotation = Quaternion.Euler(newRotation);
		objectToPosition.transform.localScale = newScale;
	}
}
