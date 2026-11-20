using System;
using System.Collections;
using System.Collections.Generic;
using Kiloo.Common;
using SYBO.Subway.Info;
using UnityEngine;

public class CharacterScreen : UIScreen
{
	private const float DEFAULT_SCALE_FACTOR = 7f;

	private const float FOCUSED_SCALE_FACTOR = 10f;

	public const int STICKER_SMALL_FONT_SIZE = 10;

	public const int STICKER_BIG_FONT_SIZE = 15;

	public const float SMALL_TOP_LABEL_POSITION_Y = 6f;

	public const float SMALL_BOTTOM_LABEL_POSITION_Y = -5f;

	public const float BIG_TOP_LABEL_POSITION_Y = 5.5f;

	public const float BIG_BOTTOM_LABEL_POSITION_Y = -7f;

	[SerializeField]
	private GameObject characterScrollAnchor;

	[SerializeField]
	private UIGrid characterGrid;

	[SerializeField]
	private GameObject overlayParent;

	[SerializeField]
	private UIPanel scrollPanel;

	[SerializeField]
	private GameObject char2dElements;

	[SerializeField]
	private GameObject dummyObject;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel themeLimitedLabel;

	[SerializeField]
	private GameObject eventRibbon;

	[SerializeField]
	private GameObject characterDayEventElements;

	[SerializeField]
	private UILabel characterDayEventTimer;

	[SerializeField]
	private UILabel[] ribbonLabels;

	[SerializeField]
	private AudioClip selectSound;

	[SerializeField]
	private UICharacterThemeShifter _characterThemeShifter;

	[SerializeField]
	private GameObject _characterDayParticleSystem;

	[SerializeField]
	private GameObject _characterSelectButtonPrefab;

	[SerializeField]
	private GameObject _centerAnchor;

	private bool _hasInited;

	private CenterOnChild _centerer;

	private float _cellWidth;

	private bool _hasShownModel;

	private SSEventType _currentActiveEventType;

	public static bool queueRefreshCharacter;

	public static bool forceRefreshOnCharacterScreen;

	public static bool forceCenterOnEventCharacter;

	public static bool forceCenterOnCurrentlySelectedCharacter;

	public static bool isReturningFromMysteryBox;

	private List<KeyValuePair<Characters.CharacterType, Characters.Model>> _characterList = new List<KeyValuePair<Characters.CharacterType, Characters.Model>>();

	private float _cachedScrollPosition;

	private CharacterScreenSelectButton _characterSelectButton;

	private bool _inappOverlayActive;

	private bool _popupActive;

	private bool _charactersEnabled;

	private bool _firstShow = true;

	private CharacterScreenManager _screenManagerInstance;

	private List<GameObject> characterModels = new List<GameObject>();

	private List<CharacterOverlayHelper> characterHelpers = new List<CharacterOverlayHelper>();

	private List<OverlayIndex> characterIndices = new List<OverlayIndex>();

	private bool _hasCenteredOnCharacter;

	public override void Init()
	{
		base.Init();
		_screenManagerInstance = CharacterScreenManager.Instance;
		_cellWidth = characterGrid.cellWidth;
		_centerer = characterGrid.GetComponent<CenterOnChild>();
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		_currentActiveEventType = ((activeEventManager != null) ? activeEventManager.GetEventType() : SSEventType.None);
		InitScrollWithCharacterModels();
		InitializeSelectButton();
		InitializeCoinbox(null, true, true, true);
		_hasInited = true;
	}

	private void CheckCenterOnCharacter()
	{
		if (!_hasCenteredOnCharacter)
		{
			_hasCenteredOnCharacter = true;
			if (CharacterScreenManager.Instance.GetCharacterTypeActiveInEvent().HasValue)
			{
				forceCenterOnEventCharacter = true;
			}
			else
			{
				forceCenterOnCurrentlySelectedCharacter = true;
			}
		}
	}

	private void InitializeSelectButton()
	{
		GameObject gameObject = NGUITools.AddChild(_centerAnchor, _characterSelectButtonPrefab);
		_characterSelectButton = gameObject.GetComponent<CharacterScreenSelectButton>();
		gameObject.transform.localPosition = new Vector3(62f, 14f, 0f);
		UpdateThemeShifter(_screenManagerInstance.currenlyShownCharacter, _screenManagerInstance.currentlyShownThemeIndex);
		_characterSelectButton.SetStickerColors(CharacterScreenSelectButton.StickerColors.Discount);
		_characterSelectButton.InitButton(CharacterScreenSelectButton.ButtonContext.characterScreen);
	}

	public override void Show()
	{
		if (_firstShow)
		{
			UIScreenController.Instance.Camera3D.enabled = false;
			UIScreenController.Instance.Camera3DClip.enabled = false;
		}
		base.Show();
		UIScreenController.Instance.OnPopupShown += OnPopupShown;
		UIScreenController.Instance.OnLastPopupClosed += OnLastPopupClosed;
		SSEventsManagerController.OnSSEventStartedHandler += LimitedCharacterEventStarted;
		SSEventsManagerController.OnSSEventFinishedHandler += LimitedCharacterEventFinished;
		UIScreenController.OnApplicationResumed += RefreshCharacterScreen;
		RefreshCharacterList();
		if (queueRefreshCharacter)
		{
			UpdateSelectedCharacter();
			queueRefreshCharacter = false;
		}
		_characterThemeShifter.AddOnChangeThemeListener(ThemeButtonClicked);
		CharacterScreenManager.Instance.AddOnCharacterUnlockedListener(OnCharacterUnlocked);
		CharacterScreenManager.OnCharacterSelected += UpdateSelectedCharacter;
		CharacterScreenManager instance = CharacterScreenManager.Instance;
		instance.OnFacebookStatusChanged = (Action)Delegate.Combine(instance.OnFacebookStatusChanged, new Action(RefreshCharacterScreen));
		CheckCenterOnCharacter();
		if (forceCenterOnEventCharacter)
		{
			forceCenterOnEventCharacter = false;
			forceCenterOnCurrentlySelectedCharacter = false;
			StartCoroutine(DelayedCenterOnEventCharacter());
		}
		else if (forceCenterOnCurrentlySelectedCharacter)
		{
			forceCenterOnCurrentlySelectedCharacter = false;
			CenterScrollOnCharacterType((Characters.CharacterType)PlayerInfo.Instance.currentCharacter);
			ChangeCurrentlyShownCharacter((Characters.CharacterType)PlayerInfo.Instance.currentCharacter, PlayerInfo.Instance.currentThemeIndex);
		}
		else if (_hasShownModel)
		{
			if (!isReturningFromMysteryBox)
			{
				ChangeCurrentlyShownCharacter(_screenManagerInstance.currenlyShownCharacter, _screenManagerInstance.currentlyShownThemeIndex);
			}
		}
		else
		{
			Update();
		}
		bool active = CharacterThemes.characterCustomThemes.ContainsKey(_screenManagerInstance.currenlyShownCharacter);
		_characterThemeShifter.gameObject.SetActive(active);
		if (_firstShow)
		{
			CoroutineUtil.StartCoroutine(EnableCamerasDelayed());
		}
	}

	private IEnumerator EnableCamerasDelayed()
	{
		_firstShow = false;
		yield return new WaitForEndOfFrame();
		UIScreenController.Instance.Camera3D.enabled = true;
		UIScreenController.Instance.Camera3DClip.enabled = true;
	}

	public override void Hide()
	{
		base.Hide();
		UIScreenController.Instance.OnPopupShown -= OnPopupShown;
		UIScreenController.Instance.OnLastPopupClosed -= OnLastPopupClosed;
		CharacterScreenManager.Instance.OnCharacterScreenHide();
		_characterThemeShifter.RemoveOnChangeThemeListener(ThemeButtonClicked);
		UIModelController.Instance.ClearModels();
		CharacterScreenManager.Instance.RemoveOnCharacterUnlockedListener(OnCharacterUnlocked);
		CharacterScreenManager.OnCharacterSelected -= UpdateSelectedCharacter;
		CharacterScreenManager instance = CharacterScreenManager.Instance;
		instance.OnFacebookStatusChanged = (Action)Delegate.Remove(instance.OnFacebookStatusChanged, new Action(RefreshCharacterScreen));
		SSEventsManagerController.OnSSEventStartedHandler -= LimitedCharacterEventStarted;
		SSEventsManagerController.OnSSEventFinishedHandler -= LimitedCharacterEventFinished;
		UIScreenController.OnApplicationResumed -= RefreshCharacterScreen;
	}

	private void InitScrollWithCharacterModels()
	{
		_screenManagerInstance.InitCharacters();
		Characters.CharacterType currentlyShownCharacter = _screenManagerInstance.currenlyShownCharacter;
		_characterList = _screenManagerInstance.GetCharacterList();
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		int i = 0;
		foreach (KeyValuePair<Characters.CharacterType, Characters.Model> character in _characterList)
		{
			Characters.CharacterType characterType = character.Key;
			int lastSelectedThemeForCharacterType = _screenManagerInstance.GetLastSelectedThemeForCharacterType(characterType);
			CharacterModelPreviewFactory.Instance.GetCharacterModelPreview(characterType.ToString(), lastSelectedThemeForCharacterType, delegate(GameObject currentCharacterModel)
			{
				if (currentCharacterModel == null)
				{
					Debug.LogError(string.Concat("Character Screen: GetCharacterModelPreview for character ", characterType, " has failed. Creating default model."));
				}
				currentCharacterModel.name = string.Format("{0:000}{1}", i, characterType.ToString());
				characterModels.Add(currentCharacterModel);
				Transform transform = currentCharacterModel.transform;
				transform.parent = characterScrollAnchor.transform;
				transform.localPosition = new Vector3((float)i * _cellWidth, 0f, 50f);
				transform.localScale = Vector3.one * 7f;
				transform.localEulerAngles = new Vector3(53f, 183f, 360f);
				GameObject gameObject = NGUITools.AddChild(overlayParent, char2dElements);
				gameObject.name = string.Format("{0:000}{1}", i, characterType.ToString());
				characterHelpers.Add(gameObject.GetComponent<CharacterOverlayHelper>());
				characterHelpers[i].Init(i, characterType, transform, currentlyShownCharacter != characterType, PlayerInfo.Instance.currentThemeIndex);
				GameObject gameObject2 = NGUITools.AddChild(characterGrid.gameObject, dummyObject);
				characterIndices.Add(gameObject2.AddComponent<OverlayIndex>());
				characterIndices[i].index = i;
				gameObject2.name = string.Format("{0:000}{1}", i, characterType.ToString());
				i++;
			});
		}
		float realtimeSinceStartup2 = Time.realtimeSinceStartup;
		Utility.SetLayerRecursively(characterScrollAnchor.transform, 29);
		Utility.SetLayerRecursively(overlayParent.transform, 26);
		characterGrid.SendMessage("Start");
		characterGrid.Reposition();
		StartCoroutine(DelayedInitCenterChar());
	}

	private IEnumerator DelayedInitCenterChar()
	{
		yield return new WaitForEndOfFrame();
		CenterScrollOnCharacterType(_screenManagerInstance.currenlyShownCharacter);
	}

	private void RefreshCharacterList()
	{
		if (_charactersEnabled && DetectChangeInCharacterList())
		{
			ClearArraysAndDestroyCachedGameObjects();
			InitScrollWithCharacterModels();
		}
	}

	private bool DetectChangeInCharacterList()
	{
		if (forceRefreshOnCharacterScreen)
		{
			forceRefreshOnCharacterScreen = false;
			return true;
		}
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		SSEventType sSEventType = ((activeEventManager != null) ? activeEventManager.GetEventType() : SSEventType.None);
		if (_currentActiveEventType != sSEventType)
		{
			_currentActiveEventType = sSEventType;
			return true;
		}
		for (int i = 0; i < characterHelpers.Count; i++)
		{
			if (_characterList[i].Key != characterHelpers[i].GetCharacterType())
			{
				return true;
			}
		}
		return false;
	}

	private void Update()
	{
		if (!_hasInited)
		{
			return;
		}
		if (_centerer.centeredObject != null)
		{
			int index = _centerer.centeredObject.GetComponent<OverlayIndex>().index;
			Characters.CharacterType characterType = characterHelpers[index].GetCharacterType();
			if (!_hasShownModel)
			{
				_hasShownModel = true;
				ChangeCurrentlyShownCharacter(characterType, PlayerInfo.Instance.GetIndexForLastSelectedTheme(characterType));
				characterHelpers[index].SelectedInMenu();
			}
			Characters.CharacterType currentlyShownCharacter = _screenManagerInstance.currenlyShownCharacter;
			if (currentlyShownCharacter != characterType)
			{
				CharacterOverlayHelper characterOverlayHelper = characterHelpers.Find((CharacterOverlayHelper cha) => cha.getCharacterType() == currentlyShownCharacter);
				if (characterOverlayHelper != null)
				{
					characterOverlayHelper.enableOverlay(false);
				}
				foreach (CharacterOverlayHelper characterHelper in characterHelpers)
				{
					if (characterHelper.GetIndex() != index)
					{
						characterHelper.RefreshOverlay();
					}
				}
				int index2 = PlayerInfo.Instance.GetIndexForLastSelectedTheme(characterType);
				if (characterType == (Characters.CharacterType)PlayerInfo.Instance.currentCharacter)
				{
					index2 = PlayerInfo.Instance.currentThemeIndex;
				}
				ChangeCurrentlyShownCharacter(characterType, index2);
				characterHelpers[index].SelectedInMenu();
			}
			if (characterDayEventElements.activeSelf)
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds(SSEventsManagerController.instance.TimeLeftInSeconds());
				if (timeSpan < TimeSpan.Zero)
				{
					timeSpan = TimeSpan.Zero;
				}
				characterDayEventTimer.text = string.Format("{0:00}:{1:00}:{2:00}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
			}
		}
		ScaleScrollModels();
		if (UIScreenController.Instance.isShowingPopup)
		{
			if (!_popupActive || !_inappOverlayActive)
			{
				_popupActive = true;
			}
		}
		else if (_popupActive && !_inappOverlayActive)
		{
			_popupActive = false;
		}
		if (UIScreenController.Instance.inAppPurchaseOverlay.activeInHierarchy)
		{
			if (!_inappOverlayActive)
			{
				_inappOverlayActive = true;
			}
		}
		else if (_inappOverlayActive)
		{
			_inappOverlayActive = false;
		}
		if (_inappOverlayActive || _popupActive)
		{
			if (_charactersEnabled)
			{
				characterScrollAnchor.SetActive(false);
				overlayParent.SetActive(false);
				_charactersEnabled = false;
			}
		}
		else if (!_charactersEnabled)
		{
			characterScrollAnchor.SetActive(true);
			overlayParent.SetActive(true);
			_charactersEnabled = true;
			RefreshCharacterScreen();
			if (queueRefreshCharacter)
			{
				UpdateSelectedCharacter();
				queueRefreshCharacter = false;
			}
		}
	}

	private void ChangeCurrentlyShownCharacter(Characters.CharacterType charType, int index)
	{
		_screenManagerInstance.currenlyShownCharacter = charType;
		_screenManagerInstance.currentlyShownThemeIndex = index;
		UIModelController.Instance.ShowMenuModel(charType, index);
		UpdateUIElements();
		UpdateThemeShifter(charType, index);
	}

	private void UpdateThemeShifter(Characters.CharacterType charType, int index)
	{
		bool flag = CharacterThemes.characterCustomThemes.ContainsKey(charType);
		_characterThemeShifter.gameObject.SetActive(flag);
		if (flag)
		{
			List<CharacterTheme> themes = CharacterThemes.TryGetCustomThemesForChar(charType);
			_characterThemeShifter.MakeCustomThemesAvailable(themes);
			_characterThemeShifter.UpdateUIForCharacter(charType, index);
		}
	}

	private void UpdateUIElements()
	{
		Characters.CharacterType currentlyShownModel = UIModelController.Instance.currentlyShownModel;
		Characters.Model model = Characters.characterData[currentlyShownModel];
		bool flag = PlayerInfo.Instance.IsCollectionComplete(currentlyShownModel);
		CharacterTheme? themeForCharacter = CharacterThemes.GetThemeForCharacter(currentlyShownModel, _screenManagerInstance.currentlyShownThemeIndex);
		bool flag2 = _characterSelectButton.CurrentlyShownCharacterMatchesEventItem();
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		bool flag3 = activeEventManager != null && (activeEventManager.GetEventType() == SSEventType.CharacterDay || activeEventManager.GetEventType() == SSEventType.ExpiredLimitedCharacter);
		_characterSelectButton.eventIcon.enabled = flag3 && flag2;
		SetEventRibbonActive();
		if (flag)
		{
			if (model.hasOnlineSettings && !string.IsNullOrEmpty(Strings.Get(model.characterLimitedDescription)))
			{
				themeLimitedLabel.text = Strings.Get(model.characterLimitedDescription);
				themeLimitedLabel.enabled = true;
			}
			else
			{
				themeLimitedLabel.text = string.Empty;
				themeLimitedLabel.enabled = false;
			}
			if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.CharacterDay)
			{
				_characterSelectButton.backgroundSticker.color = GlobalColors.GIFT_COLOR;
			}
		}
		else if (flag2 && !themeForCharacter.HasValue)
		{
			themeLimitedLabel.enabled = true;
			themeLimitedLabel.text = Strings.Get(model.characterLimitedDescription);
			_characterSelectButton.SetStickerColors(CharacterScreenSelectButton.StickerColors.Gift);
		}
		else if (model.hasOnlineSettings)
		{
			if (!themeForCharacter.HasValue)
			{
				_characterSelectButton.SetStickerColors(CharacterScreenSelectButton.StickerColors.Discount);
				TimeSpan timeSpan = PlayerInfo.Instance.CharacterExpirationTimespan(currentlyShownModel);
				if (timeSpan.Days < 1)
				{
					_characterSelectButton.stickerDayLabel.text = Strings.Get(StringID.CHARACTER_SCREEN_LAST);
					_characterSelectButton.stickerLeftLabel.text = Strings.Get(StringID.CHARACTER_SCREEN_DAY);
				}
				else
				{
					_characterSelectButton.stickerDayLabel.text = string.Format(Strings.Get(StringID.CHARACTER_SCREEN_DAYS_LEFT), timeSpan.Days, (timeSpan.Days == 1) ? string.Empty : "S");
					_characterSelectButton.stickerLeftLabel.text = Strings.Get(StringID.CHARACTER_SCREEN_LEFT);
				}
				_characterSelectButton.stickerDayLabel.fontSize = 10;
				ChangePositionOnYAxis(_characterSelectButton.stickerDayLabel.transform, 6f);
				ChangePositionOnYAxis(_characterSelectButton.stickerLeftLabel.transform, -5f);
			}
			if (!string.IsNullOrEmpty(Strings.Get(model.characterLimitedDescription)))
			{
				themeLimitedLabel.text = Strings.Get(model.characterLimitedDescription);
			}
			else
			{
				themeLimitedLabel.text = string.Empty;
			}
			themeLimitedLabel.enabled = true;
		}
		else
		{
			themeLimitedLabel.text = string.Empty;
			_characterSelectButton.stickerDayLabel.text = string.Empty;
			_characterSelectButton.stickerLeftLabel.text = string.Empty;
			_characterSelectButton.stickerDayLabel.enabled = false;
			themeLimitedLabel.enabled = false;
			_characterSelectButton.stickerLeftLabel.enabled = false;
			_characterSelectButton.backgroundSticker.enabled = false;
		}
		if (themeForCharacter.HasValue)
		{
			CharacterTheme value = themeForCharacter.Value;
			nameLabel.text = Strings.Get(value.title);
			string text = Strings.Get(value.themeDescription);
			if (!string.IsNullOrEmpty(text))
			{
				themeLimitedLabel.text = text;
			}
		}
		else
		{
			nameLabel.text = Strings.Get(model.name);
		}
		UpdateCharacterSticker(_screenManagerInstance.currenlyShownCharacter, _screenManagerInstance.currentlyShownThemeIndex);
		_characterSelectButton.ReloadButton();
	}

	private void UpdateSelectedCharacter()
	{
		Characters.CharacterType currentlyShownModel = UIModelController.Instance.currentlyShownModel;
		int currentlyShownThemeIndex = UIModelController.Instance.currentlyShownThemeIndex;
		bool flag = PlayerInfo.Instance.IsCollectionComplete(currentlyShownModel);
		bool flag2 = PlayerInfo.Instance.IsThemeUnlockedForCharacter(currentlyShownModel, currentlyShownThemeIndex);
		if (flag && flag2)
		{
			PlayerInfo.Instance.SetLastSelectedTheme(currentlyShownModel, currentlyShownThemeIndex);
			_characterThemeShifter.UpdateUIForCharacter(currentlyShownModel, currentlyShownThemeIndex);
			_characterSelectButton.ReloadButton();
			RefreshCharacterModelInScrollList(currentlyShownModel, currentlyShownThemeIndex);
		}
		else
		{
			Debug.LogWarning("Trying to select a character or theme we don't currently own.");
		}
	}

	private void RefreshCharacterModelInScrollList(Characters.CharacterType ctype, int themeIndex)
	{
		int charIndexOnScroll = 0;
		GameObject go = characterModels.Find((GameObject g) => g.name.Substring(3).Equals(ctype.ToString()));
		charIndexOnScroll = characterModels.IndexOf(go);
		characterModels.Remove(go);
		UnityEngine.Object.Destroy(go);
		foreach (CharacterOverlayHelper characterHelper in characterHelpers)
		{
			if (characterHelper.gameObject.name.Equals(go.name))
			{
				UnityEngine.Object.Destroy(characterHelper.gameObject);
				characterHelpers.Remove(characterHelper);
				break;
			}
		}
		CharacterModelPreviewFactory.Instance.GetCharacterModelPreview(ctype.ToString(), themeIndex, delegate(GameObject newGo)
		{
			characterModels.Insert(charIndexOnScroll, newGo);
			newGo.name = go.name;
			Transform transform = newGo.transform;
			Transform transform2 = go.transform;
			if (!characterScrollAnchor.activeSelf)
			{
				characterScrollAnchor.SetActive(true);
			}
			transform.parent = characterScrollAnchor.transform;
			transform.localPosition = transform2.localPosition;
			transform.localScale = transform2.localScale;
			transform.localEulerAngles = transform2.localEulerAngles;
			GameObject gameObject = NGUITools.AddChild(overlayParent, char2dElements);
			gameObject.name = string.Format("{0:000}{1}", charIndexOnScroll, ctype.ToString());
			CharacterOverlayHelper component = gameObject.GetComponent<CharacterOverlayHelper>();
			component.Init(charIndexOnScroll, ctype, transform, false, themeIndex);
			characterHelpers.Insert(charIndexOnScroll, component);
			Utility.SetLayerRecursively(transform, 29);
			Utility.SetLayerRecursively(gameObject.transform, 26);
		});
	}

	private void LimitedCharacterEventStarted()
	{
		Characters.CharacterType? characterType = null;
		bool found = false;
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (activeEventManager != null)
		{
			characterType = activeEventManager.GetEventValue<Characters.CharacterType>(out found);
		}
		if (found)
		{
			_screenManagerInstance.currenlyShownCharacter = characterType.Value;
			_screenManagerInstance.currentlyShownThemeIndex = PlayerInfo.Instance.GetIndexForLastSelectedTheme(characterType.Value);
			RefreshCharacterList();
			ChangeCurrentlyShownCharacter(_screenManagerInstance.currenlyShownCharacter, _screenManagerInstance.currentlyShownThemeIndex);
		}
	}

	private void LimitedCharacterEventFinished()
	{
		_screenManagerInstance.currenlyShownCharacter = (Characters.CharacterType)PlayerInfo.Instance.currentCharacter;
		_screenManagerInstance.currentlyShownThemeIndex = PlayerInfo.Instance.GetIndexForLastSelectedTheme(_screenManagerInstance.currenlyShownCharacter);
		RefreshCharacterList();
		ChangeCurrentlyShownCharacter(_screenManagerInstance.currenlyShownCharacter, _screenManagerInstance.currentlyShownThemeIndex);
	}

	private void RefreshCharacterScreen()
	{
		RefreshCharacterList();
		UpdateUIElements();
	}

	private void CenterScrollOnCharacterType(Characters.CharacterType charType)
	{
		int num = 0;
		using (List<KeyValuePair<Characters.CharacterType, Characters.Model>>.Enumerator enumerator = _characterList.GetEnumerator())
		{
			while (enumerator.MoveNext() && charType != enumerator.Current.Key)
			{
				num++;
			}
		}
		if (num >= characterIndices.Count)
		{
			Debug.LogWarning("CharacterScreen: Index of character: " + num + " - is bigger or equal than character indices count: " + characterIndices.Count);
			num = characterIndices.Count - 1;
		}
		bool flag = false;
		Characters.CharacterType characterType = Characters.CharacterType.slick;
		if (_centerer.centeredObject != null)
		{
			characterType = characterHelpers[num].GetCharacterType();
			flag = true;
		}
		if (flag && characterType == _screenManagerInstance.currenlyShownCharacter)
		{
			ChangeCurrentlyShownCharacter(_screenManagerInstance.currenlyShownCharacter, _screenManagerInstance.currentlyShownThemeIndex);
			return;
		}
		_centerer.CenterOnTransform(characterIndices[num].transform, true);
		_centerer.Recenter();
	}

	private IEnumerator DelayedCenterOnEventCharacter()
	{
		yield return new WaitForEndOfFrame();
		CenterScrollOnEventCharacter();
	}

	private void CenterScrollOnEventCharacter()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (activeEventManager != null && (activeEventManager.GetEventType() == SSEventType.CharacterDay || activeEventManager.GetEventType() == SSEventType.ExpiredLimitedCharacter))
		{
			bool found = false;
			Characters.CharacterType eventValue = activeEventManager.GetEventValue<Characters.CharacterType>(out found);
			if (found)
			{
				CenterScrollOnCharacterType(eventValue);
			}
		}
	}

	private void OnCharacterUnlocked(Characters.CharacterType charType, int version)
	{
		ChangeCurrentlyShownCharacter(charType, version);
		queueRefreshCharacter = true;
		if (version > 0)
		{
			CharacterTheme? themeForCharacter = CharacterThemes.GetThemeForCharacter(charType, version);
			if (themeForCharacter.HasValue)
			{
				Flurry.LogEventWithAParameter("Outfit bought", "Id", Strings.Get(themeForCharacter.Value.title));
			}
		}
	}

	public void ScrollClicked(Vector2 pos)
	{
		Characters.CharacterType currenlyShownCharacter = _screenManagerInstance.currenlyShownCharacter;
		Characters.Model model = Characters.characterData[currenlyShownCharacter];
		bool flag = _centerer.CenterOnClosestChildAtPosition(pos);
		bool flag2 = PlayerInfo.Instance.IsCollectionComplete(currenlyShownCharacter);
		int indexForLastSelectedTheme = PlayerInfo.Instance.GetIndexForLastSelectedTheme(currenlyShownCharacter);
		bool flag3 = indexForLastSelectedTheme == _screenManagerInstance.currentlyShownThemeIndex;
		bool flag4 = true;
		if (model.unlockType == Characters.UnlockType.facebook && (!SocialManager.instance.facebookIsLoggedIn || CharacterScreenManager.Instance.isFacebookLoginRunning()))
		{
			flag4 = false;
		}
		if (flag && flag2 && flag3 && flag4)
		{
			UIModelController.Instance.SelectThemeForCurrentModel(indexForLastSelectedTheme);
			UpdateSelectedCharacter();
			NGUITools.PlaySound(selectSound);
		}
	}

	private void ThemeButtonClicked(int index)
	{
		_screenManagerInstance.currentlyShownThemeIndex = index;
		UIModelController.Instance.ShowMenuModel(_screenManagerInstance.currenlyShownCharacter, index);
		UpdateUIElements();
		_characterThemeShifter.UpdateUIForCharacter(_screenManagerInstance.currenlyShownCharacter, index);
	}

	private void UpdateCharacterSticker(Characters.CharacterType charType, int index)
	{
		Characters.Model model = Characters.characterData[charType];
		CharacterTheme? themeForCharacter = CharacterThemes.GetThemeForCharacter(charType, index);
		bool flag = PlayerInfo.Instance.IsCollectionComplete(charType);
		bool hasValue = themeForCharacter.HasValue;
		bool flag2 = hasValue && themeForCharacter.Value.price == 0 && !CharacterStickerManager.isThemeUnlockedForCharacter(charType, index);
		bool flag3 = !hasValue && model.hasOnlineSettings && !flag;
		bool flag4 = _characterSelectButton.CurrentlyShownCharacterMatchesEventItem();
		_characterSelectButton.stickerDayLabel.enabled = (flag2 || flag3) && !flag4;
		_characterSelectButton.stickerLeftLabel.enabled = (flag2 || flag3) && !flag4;
		_characterSelectButton.backgroundSticker.enabled = flag2 || flag3 || flag4;
		_characterSelectButton.eventIcon.enabled = flag4;
		characterDayEventElements.SetActive(flag4 && _currentActiveEventType == SSEventType.CharacterDay);
		_characterDayParticleSystem.SetActive(ShouldEnableCharacterDayParticleSystem());
		if (flag2)
		{
			_characterSelectButton.stickerDayLabel.text = Strings.Get(StringID.BOARD_SCREEN_FREE);
			_characterSelectButton.stickerLeftLabel.text = Strings.Get(StringID.BOARD_SCREEN_GIFT);
			_characterSelectButton.SetStickerColors(CharacterScreenSelectButton.StickerColors.Gift);
			_characterSelectButton.stickerDayLabel.fontSize = 15;
			ChangePositionOnYAxis(_characterSelectButton.stickerDayLabel.transform, 5.5f);
			ChangePositionOnYAxis(_characterSelectButton.stickerLeftLabel.transform, -7f);
		}
		else if (characterDayEventElements.activeSelf)
		{
			_characterSelectButton.backgroundSticker.color = GlobalColors.GIFT_COLOR;
		}
	}

	private void SetEventRibbonActive()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		bool flag = activeEventManager != null && (activeEventManager.GetEventType() == SSEventType.CharacterDay || activeEventManager.GetEventType() == SSEventType.ExpiredLimitedCharacter);
		eventRibbon.SetActive(flag);
		if (!flag)
		{
			return;
		}
		string firstLine = string.Empty;
		string secondLine = string.Empty;
		string thirdLine = string.Empty;
		activeEventManager.GetRibbonDescription(out firstLine, out secondLine, out thirdLine);
		if (!string.IsNullOrEmpty(firstLine) && !string.IsNullOrEmpty(secondLine) && !string.IsNullOrEmpty(thirdLine))
		{
			ribbonLabels[0].text = firstLine;
			ribbonLabels[1].text = secondLine;
			ribbonLabels[2].text = thirdLine;
			Vector3 localPosition = ribbonLabels[2].transform.localPosition;
			float x = ribbonLabels[2].localSize.x;
			float num = 136f;
			if (x > num)
			{
				float num2 = Mathf.Abs(x - num);
				localPosition.x = num2 * 0.5f;
			}
			else
			{
				localPosition.x = 0f;
			}
			ribbonLabels[2].transform.localPosition = localPosition;
		}
	}

	private void OnPopupShown()
	{
		if (_characterDayParticleSystem.activeSelf)
		{
			_characterDayParticleSystem.SetActive(false);
		}
	}

	private void OnLastPopupClosed()
	{
		_characterDayParticleSystem.SetActive(ShouldEnableCharacterDayParticleSystem());
		if (_hasShownModel && isReturningFromMysteryBox)
		{
			ChangeCurrentlyShownCharacter(_screenManagerInstance.currenlyShownCharacter, _screenManagerInstance.currentlyShownThemeIndex);
			isReturningFromMysteryBox = false;
		}
	}

	public void SetupAfterMysteryBox()
	{
		isReturningFromMysteryBox = true;
	}

	private bool ShouldEnableCharacterDayParticleSystem()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.CharacterDay);
		return activeEventManager != null && !UIScreenController.Instance.isShowingPopup && _characterSelectButton.CurrentlyShownCharacterMatchesEventItem();
	}

	private void ScaleScrollModels()
	{
		float num = Mathf.Abs(scrollPanel.transform.localPosition.x);
		if (num != _cachedScrollPosition)
		{
			for (int i = 0; i < characterModels.Count; i++)
			{
				float num2 = Mathf.Abs(num - (float)i * _cellWidth);
				float num3 = 1.5f * _cellWidth;
				float num4 = Mathf.SmoothStep(10f, 7f, num2 / num3);
				characterModels[i].transform.localScale = Vector3.one * num4;
			}
			_cachedScrollPosition = num;
		}
	}

	private void ClearArraysAndDestroyCachedGameObjects()
	{
		foreach (GameObject characterModel in characterModels)
		{
			UnityEngine.Object.Destroy(characterModel);
		}
		characterModels.Clear();
		foreach (CharacterOverlayHelper characterHelper in characterHelpers)
		{
			UnityEngine.Object.Destroy(characterHelper.gameObject);
		}
		characterHelpers.Clear();
		foreach (OverlayIndex characterIndex in characterIndices)
		{
			UnityEngine.Object.Destroy(characterIndex.gameObject);
		}
		characterIndices.Clear();
		foreach (Transform item in characterGrid.transform)
		{
			item.gameObject.SetActive(false);
			UnityEngine.Object.Destroy(item);
		}
		overlayParent.transform.localPosition = Vector3.zero;
		if (!characterScrollAnchor.activeSelf)
		{
			characterScrollAnchor.SetActive(true);
		}
		characterScrollAnchor.transform.localPosition = new Vector3(0f, characterScrollAnchor.transform.localPosition.y, characterScrollAnchor.transform.localPosition.z);
		scrollPanel.cachedTransform.localPosition = Vector3.zero;
		scrollPanel.clipOffset = new Vector2(0f, 0f);
		_centerer.ClearCenterObject();
		_cachedScrollPosition = 0f;
		SpringPanel component = scrollPanel.GetComponent<SpringPanel>();
		if (component != null)
		{
			UnityEngine.Object.Destroy(component);
		}
	}

	private void ChangePositionOnYAxis(Transform objectTrasnform, float newYPosition)
	{
		Vector3 localPosition = objectTrasnform.localPosition;
		localPosition.y = newYPosition;
		objectTrasnform.localPosition = localPosition;
	}
}
