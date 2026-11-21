#define ENABLE_ERROR_LOGS
using System;
using System.Diagnostics;
using SYBO.Subway.Info;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CharacterScreenSelectButton : MonoBehaviour
{
	public enum ButtonStates
	{
		buy = 0,
		select = 1,
		trial = 2,
		buyWithInapp = 3,
		unselect = 4,
		facebook = 5
	}

	public enum ButtonContext
	{
		characterScreen = 0,
		characterNameEventPopup = 1,
		limitedCharacterPopup = 2
	}

	public enum StickerColors
	{
		Gift = 0,
		Discount = 1
	}

	private const float NO_OFFSET_DEFAULT = 0f;

	private const float TIMER_X_PIXEL_OFFSET_NORMAL = 2f;

	private const float TIMER_Y_PIXEL_OFFSET_NORMAL = 2f;

	private const float PRICE_COIN_Y_PIXEL_OFFSET_LIMITED_ELEMENT = -4f;

	private const float PRICE_COIN_Y_PIXEL_OFFSET_REGULAR_ELEMENT = 1f;

	private UIButtonOverlayOff uiButtonOverlayOff;

	[SerializeField]
	private UILabel labelSelected;

	[SerializeField]
	private UILabel labelUnselect;

	[SerializeField]
	private UILabel labelBuyWithInapp;

	[SerializeField]
	private UILabel priceLabel;

	[SerializeField]
	private UILabel progressLabel;

	[SerializeField]
	private UISprite tokenSprite;

	[SerializeField]
	private UILabel expirePriceLabel;

	[SerializeField]
	private UILabel expireTimeLabel;

	[SerializeField]
	private UISprite _keySprite;

	[SerializeField]
	private UISprite _coinSprite;

	[SerializeField]
	private UISprite _notAvailableThemeCoin;

	[SerializeField]
	private UISprite _notAvailableThemeKey;

	[SerializeField]
	private UILabel _notAvailableThemeAmount;

	[SerializeField]
	private UILabel _notAvailableThemeFeedback;

	[SerializeField]
	private UIAnchor _timeLeftAnchor;

	[SerializeField]
	private UIAnchor _priceCoinAnchor;

	[SerializeField]
	private GameObject buy;

	[SerializeField]
	private GameObject select;

	[SerializeField]
	private GameObject trial;

	[SerializeField]
	private GameObject buyWithInapp;

	[SerializeField]
	private GameObject unselect;

	[SerializeField]
	private GameObject facebook;

	[SerializeField]
	private GameObject lockedBoardGraphics;

	[SerializeField]
	private GameObject lockedThemeGraphics;

	[SerializeField]
	private GameObject lockedCharacterGraphics;

	[SerializeField]
	private GameObject facebookOffline;

	[SerializeField]
	private GameObject facebookLoading;

	[SerializeField]
	private UILabel facebookOfflineLabel;

	[SerializeField]
	private UILabel facebookLoadingLabel;

	[SerializeField]
	private GameObject stickerContents;

	public UISprite backgroundSticker;

	public UILabel stickerDayLabel;

	public UILabel stickerLeftLabel;

	public UISprite eventIcon;

	private Characters.CharacterType currentlyShownModelType;

	private Characters.Model modelData;

	private int currentlyShownThemeIndex;

	private CharacterTheme? modelTheme;

	private Characters.UnlockType unlockType;

	private bool isCharacterOwned;

	private bool isThemeOwned;

	private int unlockPrice;

	private string textProgress = "{0} / {1}";

	private BoxCollider col;

	private bool _hasInited;

	private bool buyCharacterWithIAPEventIsRunning;

	private ButtonStates _activeState;

	private ButtonContext _buttonContext;

	private CharacterScreenManager _managerInstance;

	private int _totalSeconds = -1;

	public void SetButtonState(ButtonStates state)
	{
		_activeState = state;
		switch (state)
		{
		case ButtonStates.buy:
			buy.SetActive(true);
			select.SetActive(!buy.activeSelf);
			trial.SetActive(!buy.activeSelf);
			buyWithInapp.SetActive(!buy.activeSelf);
			unselect.SetActive(!buy.activeSelf);
			facebook.SetActive(!buy.activeSelf);
			break;
		case ButtonStates.trial:
			trial.SetActive(true);
			buy.SetActive(!trial.activeSelf);
			select.SetActive(!trial.activeSelf);
			buyWithInapp.SetActive(!trial.activeSelf);
			unselect.SetActive(!trial.activeSelf);
			facebook.SetActive(!trial.activeSelf);
			break;
		case ButtonStates.select:
			select.SetActive(true);
			buy.SetActive(!select.activeSelf);
			trial.SetActive(!select.activeSelf);
			buyWithInapp.SetActive(!select.activeSelf);
			unselect.SetActive(!select.activeSelf);
			facebook.SetActive(!select.activeSelf);
			break;
		case ButtonStates.unselect:
			unselect.SetActive(true);
			buy.SetActive(!unselect.activeSelf);
			trial.SetActive(!unselect.activeSelf);
			buyWithInapp.SetActive(!unselect.activeSelf);
			select.SetActive(!unselect.activeSelf);
			facebook.SetActive(!unselect.activeSelf);
			break;
		case ButtonStates.buyWithInapp:
			buyWithInapp.SetActive(true);
			buy.SetActive(!buyWithInapp.activeSelf);
			select.SetActive(!buyWithInapp.activeSelf);
			trial.SetActive(!buyWithInapp.activeSelf);
			unselect.SetActive(!buyWithInapp.activeSelf);
			facebook.SetActive(!buyWithInapp.activeSelf);
			break;
		case ButtonStates.facebook:
			facebook.SetActive(true);
			buyWithInapp.SetActive(!facebook.activeSelf);
			buy.SetActive(!facebook.activeSelf);
			select.SetActive(!facebook.activeSelf);
			trial.SetActive(!facebook.activeSelf);
			unselect.SetActive(!facebook.activeSelf);
			break;
		default:
			LogError("No handler for button state: " + state);
			break;
		}
	}

	public void InitButton(ButtonContext newContext)
	{
		_hasInited = true;
		_buttonContext = newContext;
		_managerInstance = CharacterScreenManager.Instance;
		col = GetComponent<BoxCollider>();
		uiButtonOverlayOff = GetComponent<UIButtonOverlayOff>();
		ReloadButton();
	}

	public void SetStickerColors(StickerColors stickerType)
	{
		switch (stickerType)
		{
		case StickerColors.Gift:
			stickerDayLabel.color = GlobalColors.GIFT_LABEL_COLOR;
			stickerLeftLabel.color = GlobalColors.GIFT_LABEL_COLOR;
			backgroundSticker.color = GlobalColors.GIFT_COLOR;
			break;
		case StickerColors.Discount:
			backgroundSticker.color = GlobalColorsGenerated.DISCOUNT_STICKER_COLOR;
			stickerDayLabel.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
			stickerLeftLabel.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
			break;
		}
	}

	public bool CurrentlyShownCharacterMatchesEventItem()
	{
		bool result = false;
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (activeEventManager != null)
		{
			SSEventType eventType = activeEventManager.GetEventType();
			if (eventType == SSEventType.CharacterDay || eventType == SSEventType.ExpiredLimitedCharacter)
			{
				bool found = false;
				Characters.CharacterType? characterType = activeEventManager.GetEventValue<Characters.CharacterType>(out found);
				if (found && characterType.GetValueOrDefault() == UIModelController.Instance.currentlyShownModel && characterType.HasValue)
				{
					result = true;
				}
			}
		}
		return result;
	}

	public void ReloadButton(bool overrideCharacter = false, Characters.CharacterType newCharacter = Characters.CharacterType.slick)
	{
		if (!_hasInited)
		{
			LogWarning("CharacterSelectButton not initialized. Aborting reload.");
			return;
		}
		CharacterScreenManager instance = CharacterScreenManager.Instance;
		LimitedCharacterPopupManager instance2 = LimitedCharacterPopupManager.Instance;
		switch (_buttonContext)
		{
		case ButtonContext.characterScreen:
			currentlyShownModelType = instance.currenlyShownCharacter;
			currentlyShownThemeIndex = instance.currentlyShownThemeIndex;
			break;
		case ButtonContext.limitedCharacterPopup:
			currentlyShownModelType = instance2.currentCharacter;
			currentlyShownThemeIndex = instance2.currentThemeIndex;
			break;
		case ButtonContext.characterNameEventPopup:
			if (overrideCharacter)
			{
				currentlyShownModelType = newCharacter;
				if (CharacterThemes.GetThemeForCharacter(currentlyShownModelType, PlayerInfo.Instance.GetIndexForLastSelectedTheme(currentlyShownModelType)).HasValue)
				{
					currentlyShownThemeIndex = PlayerInfo.Instance.GetIndexForLastSelectedTheme(currentlyShownModelType);
				}
				else
				{
					currentlyShownThemeIndex = 0;
				}
			}
			stickerContents.SetActive(false);
			break;
		}
		modelData = Characters.characterData[currentlyShownModelType];
		if (_buttonContext == ButtonContext.characterScreen || _buttonContext == ButtonContext.limitedCharacterPopup)
		{
			modelTheme = CharacterThemes.GetThemeForCharacter(currentlyShownModelType, currentlyShownThemeIndex);
		}
		else
		{
			modelTheme = null;
		}
		unlockType = modelData.unlockType;
		isCharacterOwned = PlayerInfo.Instance.IsCollectionComplete(currentlyShownModelType);
		unlockPrice = modelData.Price;
		if (modelTheme.HasValue)
		{
			CharacterTheme value = modelTheme.Value;
			unlockType = value.unlockType;
			isThemeOwned = PlayerInfo.Instance.IsThemeUnlockedForCharacter(currentlyShownModelType, currentlyShownThemeIndex);
			unlockPrice = value.price;
		}
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.ExpiredLimitedCharacter);
		buyCharacterWithIAPEventIsRunning = CurrentlyShownCharacterMatchesEventItem() && (isCharacterOwned || !modelTheme.HasValue) && activeEventManager != null;
		if (unlockType == Characters.UnlockType.facebook && !Globals.DEBUG_ALL_CHARS)
		{
			if (!SocialManager.instance.facebookIsLoggedIn || !isCharacterOwned || CharacterScreenManager.Instance.isFacebookLoginRunning())
			{
				SetButtonState(ButtonStates.facebook);
				UpdateUIForFacebookState();
			}
			else if (isCharacterOwned && SocialManager.instance.facebookIsLoggedIn)
			{
				SetButtonState(ButtonStates.select);
				UpdateUIForSelectState();
			}
		}
		else if ((unlockType == Characters.UnlockType.free || unlockType == Characters.UnlockType.tokens || (isCharacterOwned && !modelTheme.HasValue) || (isCharacterOwned && isThemeOwned) || (!isCharacterOwned && modelTheme.HasValue)) && !buyCharacterWithIAPEventIsRunning)
		{
			SetButtonState(ButtonStates.select);
			UpdateUIForSelectState();
		}
		else if (buyCharacterWithIAPEventIsRunning)
		{
			SetButtonState(ButtonStates.buyWithInapp);
			UpdateUIForBuyWithInappState();
		}
		else
		{
			SetButtonState(ButtonStates.buy);
			UpdateUIForBuyState();
		}
	}

	private void UpdateUIForFacebookState()
	{
		DisableLimitedOfferElements();
		if (CharacterScreenManager.Instance.isFacebookLoginRunning())
		{
			SetToFacebookLoadingState();
		}
		else
		{
			SetToFacebookOfflineState();
		}
	}

	private void SetToFacebookLoadingState()
	{
		facebookLoading.SetActive(true);
		facebookOffline.SetActive(false);
		facebookLoadingLabel.text = Strings.Get(StringID.FACEBOOK_LOADING_CHARACTER_SCREEN);
		SetUIToSelectUnavailableState();
	}

	private void SetToFacebookOfflineState()
	{
		facebookLoading.SetActive(false);
		facebookOffline.SetActive(true);
		facebookOfflineLabel.text = Strings.Get(StringID.FACEBOOK_OFFLINE_CHARACTER_SCREEN);
		SetUIToSelectGreenState();
	}

	private void UpdateUIForBuyWithInappState()
	{
		_timeLeftAnchor.container = labelBuyWithInapp.gameObject;
		_timeLeftAnchor.side = UIAnchor.Side.Bottom;
		SetUIToSelectGreenState();
		_timeLeftAnchor.pixelOffset.x = 0f;
		_timeLeftAnchor.pixelOffset.y = 0f;
		expireTimeLabel.pivot = UIWidget.Pivot.Top;
		expireTimeLabel.color = GlobalColors.EXPIRE_TIME_COUNTER_TRY_NOT_SELECTED;
		_coinSprite.gameObject.SetActive(false);
		_keySprite.gameObject.SetActive(false);
		TimeSpan timeLeft = TimeSpan.FromSeconds(SSEventsManagerController.instance.TimeLeftInSeconds());
		EnableLimitedOfferElements(timeLeft);
		labelBuyWithInapp.text = Strings.Get(StringID.EVENT_LIMITED_CHAR_DEFAULT_BUTTON_BUY);
		string key = InAppData.ConvertCharacterTypeToDynamicPurchaseId(currentlyShownModelType);
		if (InAppData.inAppData.ContainsKey(key))
		{
			labelBuyWithInapp.text = InAppData.inAppData[key].LocalizedPrice;
		}
		else
		{
			labelBuyWithInapp.text = Strings.Get(StringID.BUY_INAPP_NOPRICE_DEFAULT);
		}
	}

	private void UpdateUIForSelectState()
	{
		DisableLimitedOfferElements();
		bool flag = PlayerInfo.Instance.currentCharacter == (int)currentlyShownModelType;
		if (flag && (_buttonContext == ButtonContext.characterScreen || _buttonContext == ButtonContext.limitedCharacterPopup))
		{
			flag = PlayerInfo.Instance.currentThemeIndex == currentlyShownThemeIndex;
		}
		if (flag)
		{
			SetUIToSelectedBlueState();
			return;
		}
		bool hasValue = modelTheme.HasValue;
		if ((isCharacterOwned && !hasValue) || (isCharacterOwned && isThemeOwned) || (unlockType == Characters.UnlockType.free && isCharacterOwned))
		{
			SetUIToSelectGreenState();
			return;
		}
		bool flag2 = unlockType == Characters.UnlockType.tokens;
		if (isCharacterOwned || (!flag2 && !hasValue))
		{
			return;
		}
		SetUIToSelectUnavailableState();
		SetUINotAvalibleVisibilityTokens(!hasValue);
		if (hasValue)
		{
			CharacterTheme value = modelTheme.Value;
			_notAvailableThemeAmount.text = value.price.ToString();
			_notAvailableThemeCoin.enabled = unlockType == Characters.UnlockType.coins;
			_notAvailableThemeKey.enabled = unlockType == Characters.UnlockType.keys;
			string text = Strings.Get(value.unlockDescription);
			if (!string.IsNullOrEmpty(text))
			{
				_notAvailableThemeFeedback.text = text.ToUpper();
			}
			lockedThemeGraphics.SetActive(true);
		}
		else
		{
			lockedThemeGraphics.SetActive(false);
			SetUIToSelectUnavailableState();
		}
	}

	private void UpdateUIForBuyState()
	{
		if (modelTheme.HasValue)
		{
			CharacterTheme value = modelTheme.Value;
			unlockType = value.unlockType;
			isThemeOwned = PlayerInfo.Instance.IsThemeUnlockedForCharacter(currentlyShownModelType, currentlyShownThemeIndex);
			unlockPrice = value.price;
		}
		if (unlockType == Characters.UnlockType.tokens)
		{
			SetUIToSelectUnavailableState();
		}
		else
		{
			SetUIToSelectGreenState();
		}
		priceLabel.text = unlockPrice.ToString();
		expirePriceLabel.text = unlockPrice.ToString();
		_coinSprite.gameObject.SetActive(unlockType == Characters.UnlockType.coins);
		_keySprite.gameObject.SetActive(unlockType == Characters.UnlockType.keys);
		DisableLimitedOfferElements();
	}

	private void EnableLimitedOfferElements(TimeSpan timeLeft)
	{
		if (timeLeft < TimeSpan.Zero)
		{
			timeLeft = TimeSpan.Zero;
		}
		expireTimeLabel.text = ExpireTimeLabelFormat(false, timeLeft);
		_priceCoinAnchor.container = expirePriceLabel.gameObject;
		_priceCoinAnchor.pixelOffset.y = -4f;
		priceLabel.gameObject.SetActive(false);
		expireTimeLabel.gameObject.SetActive(true);
		expirePriceLabel.gameObject.SetActive(true);
	}

	private void DisableLimitedOfferElements()
	{
		_priceCoinAnchor.container = priceLabel.gameObject;
		_priceCoinAnchor.pixelOffset.y = 1f;
		priceLabel.gameObject.SetActive(true);
		expirePriceLabel.gameObject.SetActive(false);
		expireTimeLabel.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (expireTimeLabel.gameObject.activeInHierarchy && expireTimeLabel.enabled)
		{
			TimeSpan zero = TimeSpan.Zero;
			zero = ((!buyCharacterWithIAPEventIsRunning) ? PlayerInfo.Instance.CharacterExpirationTimespan(currentlyShownModelType) : TimeSpan.FromSeconds(SSEventsManagerController.instance.TimeLeftInSeconds()));
			if (zero < TimeSpan.Zero)
			{
				zero = TimeSpan.Zero;
			}
			if (_totalSeconds != (int)zero.TotalSeconds)
			{
				expireTimeLabel.text = ExpireTimeLabelFormat(buyCharacterWithIAPEventIsRunning, zero);
				_totalSeconds = (int)zero.TotalSeconds;
			}
		}
	}

	private string ExpireTimeLabelFormat(bool eventActive, TimeSpan timeLeft)
	{
		if (eventActive)
		{
			return string.Format("{0:00}:{1:00}:{2:00} left", (int)timeLeft.TotalHours, timeLeft.Minutes, timeLeft.Seconds);
		}
		if (timeLeft.Days > 0)
		{
			return string.Format("{0:0} day{1} {2:00}:{3:00}:{4:00}", timeLeft.Days, (timeLeft.Days == 1) ? string.Empty : "s", timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
		}
		return string.Format("{0:00}:{1:00}:{2:00}", timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
	}

	private void SetUIToSelectGreenState()
	{
		lockedThemeGraphics.SetActive(false);
		SetUINotAvalibleVisibilityTokens(false);
		uiButtonOverlayOff.SetButtonType(UIButtonOverlayOff.ButtonType.Primary);
		TweenColor.Begin(uiButtonOverlayOff.fillWithPrimeColor, 0f, GlobalColors.PRIMARY_ACTION_COLOR);
		uiButtonOverlayOff.GoToNormalState();
		labelSelected.text = Strings.Get(StringID.UICHARACTER_SELECT_BUTTON_SELECT);
		col.enabled = true;
	}

	private void SetUIToSelectedBlueState()
	{
		lockedThemeGraphics.SetActive(false);
		SetUINotAvalibleVisibilityTokens(false);
		uiButtonOverlayOff.SetButtonType(UIButtonOverlayOff.ButtonType.Tertiary);
		TweenColor.Begin(uiButtonOverlayOff.fillWithPrimeColor, 0f, GlobalColors.TERTIARY_ACTION_COLOR);
		uiButtonOverlayOff.GoToSelectedState();
		labelSelected.text = Strings.Get(StringID.UICHARACTER_SELECT_BUTTON_SELECTED);
		col.enabled = false;
	}

	private void SetUIToUnselectBlueState()
	{
		lockedThemeGraphics.SetActive(false);
		SetUINotAvalibleVisibilityTokens(false);
		uiButtonOverlayOff.SetButtonType(UIButtonOverlayOff.ButtonType.Tertiary);
		TweenColor.Begin(uiButtonOverlayOff.fillWithPrimeColor, 0f, GlobalColors.TERTIARY_ACTION_COLOR);
		uiButtonOverlayOff.GoToNormalState();
		labelUnselect.text = Strings.Get(StringID.HOVERBOARD_THEME_BUTTON_UNSELECT);
		col.enabled = true;
	}

	private void SetUIToSelectUnavailableState()
	{
		uiButtonOverlayOff.SetButtonType(UIButtonOverlayOff.ButtonType.Tertiary_inactive);
		TweenColor.Begin(uiButtonOverlayOff.fillWithPrimeColor, 0f, GlobalColors.TERTIARY_ACTION_COLOR_UNAVAILABLE);
		uiButtonOverlayOff.GoToNotAvailableState();
		labelSelected.text = string.Empty;
		progressLabel.text = string.Format(textProgress, PlayerInfo.Instance.GetCollectedTokens(currentlyShownModelType), modelData.Price);
		tokenSprite.spriteName = Characters.characterData[currentlyShownModelType].tokenSprite2dName;
		tokenSprite.MakePixelPerfect();
		col.enabled = false;
	}

	private void SetUINotAvalibleVisibilityTokens(bool val)
	{
		lockedCharacterGraphics.SetActive(val);
		lockedBoardGraphics.SetActive(false);
	}

	public void OnClick()
	{
		if (_activeState == ButtonStates.buy)
		{
			PurchaseCharacter(false);
		}
		else if (_activeState == ButtonStates.buyWithInapp)
		{
			PurchaseCharacter(true);
		}
		else if (_activeState == ButtonStates.facebook)
		{
			SetToFacebookLoadingState();
			_managerInstance.StartLoginToFacebook();
		}
		else
		{
			_managerInstance.SelectCharacter(currentlyShownModelType, currentlyShownThemeIndex);
		}
	}

	private void PurchaseCharacter(bool withInAppPurchase)
	{
		switch (_buttonContext)
		{
		case ButtonContext.characterScreen:
			if (withInAppPurchase)
			{
				_managerInstance.PurchaseCharacterWithInapp(CharacterScreenManager.CharacterPurchaseInitiator.characterScreen, currentlyShownModelType);
			}
			else
			{
				_managerInstance.PurchaseCharacter(CharacterScreenManager.CharacterPurchaseInitiator.characterScreen, currentlyShownModelType, currentlyShownThemeIndex);
			}
			break;
		case ButtonContext.limitedCharacterPopup:
			if (withInAppPurchase)
			{
				_managerInstance.PurchaseCharacterWithInapp(CharacterScreenManager.CharacterPurchaseInitiator.limitedCharacterPopup, currentlyShownModelType);
			}
			else
			{
				_managerInstance.PurchaseCharacter(CharacterScreenManager.CharacterPurchaseInitiator.limitedCharacterPopup, currentlyShownModelType, currentlyShownThemeIndex);
			}
			CharacterScreen.forceCenterOnCurrentlySelectedCharacter = true;
			CharacterScreen.forceRefreshOnCharacterScreen = true;
			break;
		case ButtonContext.characterNameEventPopup:
			if (withInAppPurchase)
			{
				_managerInstance.PurchaseCharacterWithInapp(CharacterScreenManager.CharacterPurchaseInitiator.characterNameEvent, currentlyShownModelType);
			}
			else
			{
				_managerInstance.PurchaseCharacter(CharacterScreenManager.CharacterPurchaseInitiator.characterNameEvent, currentlyShownModelType, 0);
			}
			CharacterScreen.forceCenterOnCurrentlySelectedCharacter = true;
			CharacterScreen.forceRefreshOnCharacterScreen = true;
			break;
		default:
			LogWarning("No handler for context: " + _buttonContext);
			break;
		}
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, UnityEngine.Object context = null)
	{
		Debug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, UnityEngine.Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("ENABLE_CHARACTER_SELECT_BUTTON_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}
}
