using System;
using System.Diagnostics;
using UnityEngine;

public class HoverboardSelectButton : MonoBehaviour
{
	public enum ButtonStates
	{
		Buy = 0,
		Select = 1,
		Trial = 2,
		BuyWithInapp = 3,
		Unselect = 4
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

	private const float SMALL_BOTTOM_LABEL_POSITION_Y = -5f;

	private const float BIG_BOTTOM_LABEL_POSITION_Y = -7f;

	private const int SMALL_FONT_SIZE = 10;

	private const int BIG_FONT_SIZE = 15;

	[SerializeField]
	private UILabel _labelSelected;

	[SerializeField]
	private UILabel _labelUnselect;

	[SerializeField]
	private UILabel _labelTry;

	[SerializeField]
	private UILabel _labelBuyWithInapp;

	[SerializeField]
	private UILabel _priceLabel;

	[SerializeField]
	private UILabel _progressLabel;

	[SerializeField]
	private UISprite _tokenSprite;

	[SerializeField]
	private UILabel _expirePriceLabel;

	[SerializeField]
	private UILabel _expireTimeLabel;

	[SerializeField]
	private UISprite _keySprite;

	[SerializeField]
	private UISprite _coinSprite;

	[SerializeField]
	private UISprite _lockedKeySprite;

	[SerializeField]
	private UISprite _lockedCoinSprite;

	[SerializeField]
	private UILabel _notAvailableThemeAmount;

	[SerializeField]
	private UILabel _notAvailableThemeFeedback;

	[SerializeField]
	private UIAnchor _timeLeftAnchor;

	[SerializeField]
	private UIAnchor _priceCoinAnchor;

	[SerializeField]
	private GameObject _buy;

	[SerializeField]
	private GameObject _select;

	[SerializeField]
	private GameObject _trial;

	[SerializeField]
	private GameObject _buyWithInapp;

	[SerializeField]
	private GameObject _unselect;

	[SerializeField]
	private GameObject _lockedBoardGraphics;

	[SerializeField]
	private GameObject _lockedThemeGraphics;

	[SerializeField]
	private UISprite _backgroundSticker;

	[SerializeField]
	private UILabel _stickerTopLabel;

	[SerializeField]
	private UILabel _stickerBottomLabel;

	[SerializeField]
	private UISprite _eventIcon;

	private Hoverboards.BoardType _boardType;

	private Hoverboards.Board _board;

	private Hoverboards.UnlockType _boardUnlockType = Hoverboards.UnlockType.free;

	private int _currentThemeIndex;

	private int _unlockPrice;

	private int _expireTimeLeft = -1;

	private readonly string _textProgress = "{0} / {1}";

	private BoxCollider _buttonCollider;

	private UIButtonOverlayOff _uiButtonOverlayOff;

	private bool _purchaseInProgress;

	private bool _hasInited;

	private bool _shouldShowEventIndicatorsForTryBoardsEvent;

	private bool _shouldShowEventIndicatorsForBuyLimitedBoardsEvent;

	private HoverboardManager.ScreenContext _screenContext;

	private ButtonStates _activeState;

	public event Action OnClickSelectedButton;

	public void UpdateCurrentThemeIndex()
	{
		_currentThemeIndex = HoverboardManager.Instance.GetCurrentlyShownHoverboardTheme(_screenContext);
	}

	public void SetStickerColors(StickerColors stickerType)
	{
		switch (stickerType)
		{
		case StickerColors.Gift:
			_stickerTopLabel.color = GlobalColors.GIFT_LABEL_COLOR;
			_stickerBottomLabel.color = GlobalColors.GIFT_LABEL_COLOR;
			_backgroundSticker.color = GlobalColors.GIFT_COLOR;
			break;
		case StickerColors.Discount:
			_stickerTopLabel.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
			_stickerBottomLabel.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
			_backgroundSticker.color = GlobalColorsGenerated.DISCOUNT_STICKER_COLOR;
			break;
		}
	}

	public void UpdateSticker(Hoverboards.BoardType board, int themeIndex)
	{
		Hoverboards.Board board2 = Hoverboards.boardData[board];
		bool flag = PlayerInfo.Instance.isHoverboardUnlocked(board);
		Hoverboards.BoardType? eventBoardType = HoverboardManager.Instance.GetEventBoardType();
		bool flag2 = eventBoardType.HasValue && board == eventBoardType.Value;
		if (themeIndex == 0)
		{
			if (flag)
			{
				DisableStickers();
			}
			else if (flag2 && !SSEventsManagerController.instance.HasTheCurrentEventBeenTried())
			{
				_backgroundSticker.enabled = true;
				_eventIcon.enabled = true;
				_stickerTopLabel.enabled = false;
				_stickerBottomLabel.enabled = false;
				_backgroundSticker.color = GlobalColors.GIFT_COLOR;
			}
			else if (board2.unlockType == Hoverboards.UnlockType.free)
			{
				EnableFreeSticker();
				UpdateStickerLabel(-7f, 15);
			}
			else if (board2.hasOnlineTimeLimit && !flag2)
			{
				_stickerTopLabel.enabled = true;
				_stickerBottomLabel.enabled = true;
				_backgroundSticker.enabled = true;
				_eventIcon.enabled = false;
				TimeSpan timeSpan = PlayerInfo.Instance.BoardExpirationTimespan(board);
				if (timeSpan.Days < 1)
				{
					_stickerTopLabel.text = Strings.Get(StringID.BOARD_SCREEN_LAST);
					_stickerBottomLabel.text = Strings.Get(StringID.BOARD_SCREEN_DAY);
				}
				else
				{
					_stickerTopLabel.text = string.Format("{0:0} DAY{1}", timeSpan.Days, (timeSpan.Days == 1) ? string.Empty : "S");
					_stickerBottomLabel.text = Strings.Get(StringID.BOARD_SCREEN_LEFT);
				}
				SetStickerColors(StickerColors.Discount);
				UpdateStickerLabel(-5f, 10);
			}
			else
			{
				DisableStickers();
			}
		}
		else if (HoverboardThemes.hoverboardCustomThemes[board][themeIndex - 1].unlockType == Hoverboards.UnlockType.free && PlayerInfo.Instance.isHoverboardUnlocked(board) && !PlayerInfo.Instance.HoverboardCustomizationData.IsThemeOwned(board, HoverboardThemes.GetThemeIdentifier(board, themeIndex - 1)))
		{
			EnableFreeSticker();
			UpdateStickerLabel(-7f, 15);
		}
		else
		{
			DisableStickers();
		}
	}

	private void SetButtonState(ButtonStates state)
	{
		_activeState = state;
		_buy.SetActive(state == ButtonStates.Buy);
		_select.SetActive(state == ButtonStates.Select);
		_trial.SetActive(state == ButtonStates.Trial);
		_buyWithInapp.SetActive(state == ButtonStates.BuyWithInapp);
		_unselect.SetActive(state == ButtonStates.Unselect);
	}

	private void DisableStickers()
	{
		_stickerTopLabel.text = string.Empty;
		_stickerBottomLabel.text = string.Empty;
		_stickerTopLabel.enabled = false;
		_stickerBottomLabel.enabled = false;
		_backgroundSticker.enabled = false;
		_eventIcon.enabled = false;
	}

	private void EnableFreeSticker()
	{
		_stickerTopLabel.text = Strings.Get(StringID.BOARD_SCREEN_FREE);
		_stickerBottomLabel.text = Strings.Get(StringID.BOARD_SCREEN_GIFT);
		_stickerTopLabel.enabled = true;
		_stickerBottomLabel.enabled = true;
		_backgroundSticker.enabled = true;
		_eventIcon.enabled = false;
		SetStickerColors(StickerColors.Gift);
	}

	private void UpdateStickerLabel(float positionY, int fontSize)
	{
		Vector3 localPosition = _stickerBottomLabel.cachedTransform.localPosition;
		_stickerBottomLabel.cachedTransform.localPosition = new Vector3(localPosition.x, positionY, localPosition.z);
		_stickerTopLabel.fontSize = fontSize;
	}

	private void OnEnable()
	{
		SSEventsManagerController.OnSSEventStartedHandler += OnChangedCurrentlyShownHoverboardModel;
		SSEventsManagerController.OnSSEventFinishedHandler += OnChangedCurrentlyShownHoverboardModel;
		UIScreenController.OnApplicationResumed += OnChangedCurrentlyShownHoverboardModel;
		UIModelController.OnChangedCurrentlyShownHoverboard += OnChangedCurrentlyShownHoverboardModel;
	}

	private void OnDisable()
	{
		SSEventsManagerController.OnSSEventStartedHandler -= OnChangedCurrentlyShownHoverboardModel;
		SSEventsManagerController.OnSSEventFinishedHandler -= OnChangedCurrentlyShownHoverboardModel;
		UIScreenController.OnApplicationResumed -= OnChangedCurrentlyShownHoverboardModel;
		UIModelController.OnChangedCurrentlyShownHoverboard -= OnChangedCurrentlyShownHoverboardModel;
	}

	public void InitButton(HoverboardManager.ScreenContext newContext)
	{
		if (!_hasInited)
		{
			_screenContext = newContext;
			_buttonCollider = GetComponent<BoxCollider>();
			_uiButtonOverlayOff = GetComponent<UIButtonOverlayOff>();
			OnChangedCurrentlyShownHoverboardModel();
			_hasInited = true;
		}
	}

	private bool CurrentlyShownBoardMatchesTrySpecialPowerBoard()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.SpecialPowerBoard);
		bool result = false;
		if (activeEventManager != null)
		{
			bool found = false;
			Hoverboards.BoardType eventValue = activeEventManager.GetEventValue<Hoverboards.BoardType>(out found);
			if (found && eventValue == _boardType)
			{
				result = true;
			}
		}
		return result;
	}

	private bool CurrentlyShownBoardMatchesLimitedExpiredBoardEvent()
	{
		bool result = false;
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (activeEventManager != null)
		{
			SSEventType eventType = activeEventManager.GetEventType();
			if (eventType == SSEventType.ExpiredLimitedBoard)
			{
				bool found = false;
				Hoverboards.BoardType eventValue = activeEventManager.GetEventValue<Hoverboards.BoardType>(out found);
				if (found && eventValue == HoverboardManager.Instance.GetCurrentlyShownHoverboard(_screenContext))
				{
					result = true;
				}
			}
		}
		return result;
	}

	private void OnChangedCurrentlyShownHoverboardModel()
	{
		Hoverboards.BoardType currentlyShownHoverboard = HoverboardManager.Instance.GetCurrentlyShownHoverboard(_screenContext);
		if (_boardType != currentlyShownHoverboard)
		{
			_currentThemeIndex = 0;
			BoardScreenManager.Instance.shouldResetThemeSelection = true;
		}
		_boardType = currentlyShownHoverboard;
		_board = Hoverboards.boardData[_boardType];
		_boardUnlockType = _board.unlockType;
		_unlockPrice = _board.price;
		_shouldShowEventIndicatorsForTryBoardsEvent = CurrentlyShownBoardMatchesTrySpecialPowerBoard();
		_shouldShowEventIndicatorsForBuyLimitedBoardsEvent = CurrentlyShownBoardMatchesLimitedExpiredBoardEvent();
		if (_currentThemeIndex == 0)
		{
			if ((PlayerInfo.Instance.isHoverboardUnlocked(_boardType) || _boardUnlockType == Hoverboards.UnlockType.alwaysUnlocked || _boardUnlockType == Hoverboards.UnlockType.eggHunt) && !_shouldShowEventIndicatorsForTryBoardsEvent && !_shouldShowEventIndicatorsForBuyLimitedBoardsEvent)
			{
				SetButtonState(ButtonStates.Select);
				SetSelectHoverboardUI();
			}
			else if (_shouldShowEventIndicatorsForTryBoardsEvent)
			{
				SetButtonState(ButtonStates.Trial);
				SetTryHoverboardUI();
			}
			else if (_shouldShowEventIndicatorsForBuyLimitedBoardsEvent)
			{
				SetButtonState(ButtonStates.BuyWithInapp);
				BuyHoverboardWithInapp();
			}
			else
			{
				SetButtonState(ButtonStates.Buy);
				SetBuyHoverboardUI();
			}
			return;
		}
		HoverboardThemes.ThemeIdentifier themeIdentifier = HoverboardThemes.GetThemeIdentifier(_boardType, _currentThemeIndex - 1);
		HoverboardCustomizationData hoverboardCustomizationData = PlayerInfo.Instance.HoverboardCustomizationData;
		if (PlayerInfo.Instance.isHoverboardUnlocked(_boardType))
		{
			if (hoverboardCustomizationData.IsThemeOwned(_boardType, themeIdentifier))
			{
				if (hoverboardCustomizationData.IsThemeActive(_boardType, themeIdentifier))
				{
					SetButtonState(ButtonStates.Unselect);
					SetUnselectBoardThemeUI();
				}
				else
				{
					SetButtonState(ButtonStates.Select);
					SetSelectBoardThemeUI();
				}
			}
			else
			{
				SetButtonState(ButtonStates.Buy);
				SetBuyBoardThemeUI();
			}
		}
		else
		{
			SetNeedsToUnlockBoardUI();
		}
	}

	private void SetTryHoverboardUI()
	{
		_timeLeftAnchor.container = _labelTry.gameObject;
		_expireTimeLabel.pivot = UIWidget.Pivot.Top;
		_timeLeftAnchor.pixelOffset.x = 0f;
		_timeLeftAnchor.pixelOffset.y = 0f;
		_timeLeftAnchor.side = UIAnchor.Side.Bottom;
		if (!SSEventsManagerController.instance.HasTheCurrentEventBeenTried() || (_shouldShowEventIndicatorsForTryBoardsEvent && HoverboardManager.Instance.GetCurrentlyShownHoverboard(_screenContext) != PlayerInfo.Instance.currentHoverboard))
		{
			_expireTimeLabel.color = GlobalColors.EXPIRE_TIME_COUNTER_TRY_NOT_SELECTED;
			SetToSelectGreenState();
		}
		else
		{
			_expireTimeLabel.color = GlobalColors.EXPIRE_TIME_COUNTER_TRY_SELECTED;
			SetToSelectedBlueState();
		}
		_coinSprite.gameObject.SetActive(false);
		_keySprite.gameObject.SetActive(false);
		TimeSpan timeLeft = TimeSpan.FromSeconds(SSEventsManagerController.instance.TimeLeftInSeconds());
		EnableLimitedOffer(timeLeft);
	}

	private void SetSelectHoverboardUI()
	{
		DisableLimitedOffer();
		if (PlayerInfo.Instance.currentHoverboard == _boardType)
		{
			SetToSelectedBlueState();
		}
		else if (PlayerInfo.Instance.isHoverboardUnlocked(_boardType) || _boardUnlockType == Hoverboards.UnlockType.alwaysUnlocked)
		{
			SetToSelectGreenState();
		}
		else
		{
			SetToSelectUnavailableState();
		}
		_timeLeftAnchor.container = _expirePriceLabel.gameObject;
		_timeLeftAnchor.pixelOffset.y = 2f;
	}

	private void SetBuyHoverboardUI()
	{
		SetToSelectGreenState();
		_coinSprite.gameObject.SetActive(true);
		_keySprite.gameObject.SetActive(false);
		_priceLabel.text = _unlockPrice.ToString();
		_expirePriceLabel.text = _unlockPrice.ToString();
		DisableLimitedOffer();
	}

	private void SetBuyBoardThemeUI()
	{
		SetToSelectGreenState();
		if (_currentThemeIndex != 0)
		{
			HoverboardThemes.HoverboardTheme hoverboardTheme = HoverboardThemes.hoverboardCustomThemes[_boardType][_currentThemeIndex - 1];
			switch (hoverboardTheme.unlockType)
			{
			case Hoverboards.UnlockType.coins:
				_coinSprite.gameObject.SetActive(true);
				_keySprite.gameObject.SetActive(false);
				break;
			case Hoverboards.UnlockType.free:
			case Hoverboards.UnlockType.keys:
				_coinSprite.gameObject.SetActive(false);
				_keySprite.gameObject.SetActive(true);
				break;
			}
			_priceLabel.text = hoverboardTheme.price.ToString();
			_expirePriceLabel.text = _unlockPrice.ToString();
			DisableLimitedOffer();
		}
	}

	private void SetSelectBoardThemeUI()
	{
		DisableLimitedOffer();
		SetToSelectGreenState();
	}

	private void SetUnselectBoardThemeUI()
	{
		DisableLimitedOffer();
		SetToUnselectBlueState();
	}

	private void SetNeedsToUnlockBoardUI()
	{
		DisableLimitedOffer();
		SetToNeedsUnlockGreyState();
	}

	private void BuyHoverboardWithInapp()
	{
		_timeLeftAnchor.container = _labelBuyWithInapp.gameObject;
		_timeLeftAnchor.side = UIAnchor.Side.Bottom;
		SetToSelectGreenState();
		_timeLeftAnchor.pixelOffset.x = 0f;
		_timeLeftAnchor.pixelOffset.y = 0f;
		_expireTimeLabel.pivot = UIWidget.Pivot.Top;
		_expireTimeLabel.color = GlobalColors.EXPIRE_TIME_COUNTER_TRY_NOT_SELECTED;
		_coinSprite.gameObject.SetActive(false);
		_keySprite.gameObject.SetActive(false);
		TimeSpan timeLeft = TimeSpan.FromSeconds(SSEventsManagerController.instance.TimeLeftInSeconds());
		EnableLimitedOffer(timeLeft);
		_labelBuyWithInapp.text = Strings.Get(StringID.EVENT_LIMITED_BOARD_DEFAULT_BUTTON_BUY);
		string key = InAppData.ConvertBoardTypeToDynamicPurchaseId(_boardType);
		if (InAppData.inAppData.ContainsKey(key))
		{
			_labelBuyWithInapp.text = InAppData.inAppData[key].LocalizedPrice;
		}
		else
		{
			_labelBuyWithInapp.text = Strings.Get(StringID.BUY_INAPP_NOPRICE_DEFAULT);
		}
	}

	private void EnableLimitedOffer(TimeSpan timeLeft)
	{
		if (timeLeft < TimeSpan.Zero)
		{
			timeLeft = TimeSpan.Zero;
		}
		_expireTimeLabel.text = ExpireTimeLabelFormat(_shouldShowEventIndicatorsForTryBoardsEvent || _shouldShowEventIndicatorsForBuyLimitedBoardsEvent, timeLeft);
		_priceCoinAnchor.container = _expirePriceLabel.gameObject;
		_priceCoinAnchor.pixelOffset.y = -4f;
		_priceLabel.gameObject.SetActive(false);
		_expireTimeLabel.gameObject.SetActive(true);
		_expirePriceLabel.gameObject.SetActive(!_shouldShowEventIndicatorsForTryBoardsEvent || !_shouldShowEventIndicatorsForBuyLimitedBoardsEvent);
	}

	private void DisableLimitedOffer()
	{
		_priceCoinAnchor.container = _priceLabel.gameObject;
		_priceCoinAnchor.pixelOffset.y = 1f;
		_priceLabel.gameObject.SetActive(true);
		_expirePriceLabel.gameObject.SetActive(false);
		_expireTimeLabel.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (_expireTimeLabel.gameObject.activeInHierarchy && _expireTimeLabel.enabled)
		{
			TimeSpan zero = TimeSpan.Zero;
			zero = ((!_shouldShowEventIndicatorsForTryBoardsEvent && !_shouldShowEventIndicatorsForBuyLimitedBoardsEvent) ? PlayerInfo.Instance.BoardExpirationTimespan(_boardType) : TimeSpan.FromSeconds(SSEventsManagerController.instance.TimeLeftInSeconds()));
			if (zero < TimeSpan.Zero)
			{
				zero = TimeSpan.Zero;
			}
			if (_expireTimeLeft != (int)zero.TotalSeconds)
			{
				_expireTimeLabel.text = ExpireTimeLabelFormat(_shouldShowEventIndicatorsForTryBoardsEvent || _shouldShowEventIndicatorsForBuyLimitedBoardsEvent, zero);
				_expireTimeLeft = (int)zero.TotalSeconds;
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

	private void SetToSelectGreenState()
	{
		_lockedThemeGraphics.SetActive(false);
		SetNotAvalibleVisibilityTokens(false);
		_uiButtonOverlayOff.SetButtonType(UIButtonOverlayOff.ButtonType.Primary);
		TweenColor.Begin(_uiButtonOverlayOff.fillWithPrimeColor, 0f, GlobalColors.PRIMARY_ACTION_COLOR);
		_uiButtonOverlayOff.GoToNormalState();
		if (_currentThemeIndex == 0)
		{
			if (_shouldShowEventIndicatorsForTryBoardsEvent)
			{
				_labelTry.text = Strings.Get(StringID.HOVERBOARD_TRY_BUTTON_SELECT);
			}
			else
			{
				_labelSelected.text = Strings.Get(StringID.HOVERBOARD_SELCTECT_BUTTON_SELECT);
			}
		}
		else
		{
			_labelSelected.text = Strings.Get(StringID.HOVERBOARD_THEME_SELECT_BUTTON_TEXT);
		}
		_buttonCollider.enabled = true;
	}

	private void SetToSelectedBlueState()
	{
		_lockedThemeGraphics.SetActive(false);
		SetNotAvalibleVisibilityTokens(false);
		_uiButtonOverlayOff.SetButtonType(UIButtonOverlayOff.ButtonType.Tertiary);
		TweenColor.Begin(_uiButtonOverlayOff.fillWithPrimeColor, 0f, GlobalColors.TERTIARY_ACTION_COLOR);
		_uiButtonOverlayOff.GoToSelectedState();
		if (_shouldShowEventIndicatorsForTryBoardsEvent)
		{
			_labelTry.text = Strings.Get(StringID.HOVERBOARD_SELCTECT_BUTTON_SELECTED);
		}
		else
		{
			_labelSelected.text = Strings.Get(StringID.HOVERBOARD_SELCTECT_BUTTON_SELECTED);
		}
		_buttonCollider.enabled = false;
	}

	private void SetToUnselectBlueState()
	{
		_lockedThemeGraphics.SetActive(false);
		SetNotAvalibleVisibilityTokens(false);
		_uiButtonOverlayOff.SetButtonType(UIButtonOverlayOff.ButtonType.Tertiary);
		TweenColor.Begin(_uiButtonOverlayOff.fillWithPrimeColor, 0f, GlobalColors.TERTIARY_ACTION_COLOR);
		_uiButtonOverlayOff.GoToNormalState();
		_labelUnselect.text = Strings.Get(StringID.HOVERBOARD_THEME_BUTTON_UNSELECT);
		_buttonCollider.enabled = true;
	}

	private void SetToNeedsUnlockGreyState()
	{
		_uiButtonOverlayOff.SetButtonType(UIButtonOverlayOff.ButtonType.Tertiary_inactive);
		TweenColor.Begin(_uiButtonOverlayOff.fillWithPrimeColor, 0f, GlobalColors.TERTIARY_ACTION_COLOR_UNAVAILABLE);
		_uiButtonOverlayOff.GoToNotAvailableState();
		_select.SetActive(true);
		_lockedBoardGraphics.SetActive(false);
		_lockedThemeGraphics.SetActive(true);
		_buy.SetActive(false);
		_buyWithInapp.SetActive(false);
		_labelSelected.text = string.Empty;
		HoverboardThemes.HoverboardTheme hoverboardTheme = HoverboardThemes.hoverboardCustomThemes[_boardType][_currentThemeIndex - 1];
		_notAvailableThemeAmount.text = hoverboardTheme.price.ToString();
		switch (hoverboardTheme.unlockType)
		{
		case Hoverboards.UnlockType.coins:
			_lockedCoinSprite.gameObject.SetActive(true);
			_lockedKeySprite.gameObject.SetActive(false);
			break;
		case Hoverboards.UnlockType.free:
		case Hoverboards.UnlockType.keys:
			_lockedCoinSprite.gameObject.SetActive(false);
			_lockedKeySprite.gameObject.SetActive(true);
			break;
		}
		_notAvailableThemeFeedback.text = string.Format(Strings.Get(StringID.HOVERBOARD_THEME_UNLOCK_BOARD_FIRST), Strings.Get(Hoverboards.boardData[_boardType].name).ToUpper());
		_buttonCollider.enabled = false;
	}

	private void SetToSelectUnavailableState()
	{
		_uiButtonOverlayOff.SetButtonType(UIButtonOverlayOff.ButtonType.Tertiary_inactive);
		TweenColor.Begin(_uiButtonOverlayOff.fillWithPrimeColor, 0f, GlobalColors.TERTIARY_ACTION_COLOR_UNAVAILABLE);
		_uiButtonOverlayOff.GoToNotAvailableState();
		_labelSelected.text = string.Empty;
		if (_boardUnlockType == Hoverboards.UnlockType.eggHunt)
		{
			_progressLabel.text = string.Format(_textProgress, EggHuntManager.GetCurrentNumberOfEggs(), EggHuntManager.GetMaximumNumberOfEggs());
		}
		else if (_boardUnlockType == Hoverboards.UnlockType.itemHunt)
		{
			_progressLabel.text = string.Format(_textProgress, WeeklyHuntManager.ItemsCollectedForBoardType(_boardType), WeeklyHuntManager.ItemsToCollectForBoardType(_boardType));
		}
		_tokenSprite.spriteName = _board.tokenSprite2dName;
		SetNotAvalibleVisibilityTokens(true);
		_tokenSprite.MakePixelPerfect();
		_buttonCollider.enabled = false;
	}

	private void SetNotAvalibleVisibilityTokens(bool val)
	{
		_lockedBoardGraphics.SetActive(val);
		_lockedThemeGraphics.SetActive(false);
	}

	public void OnClick()
	{
		switch (_activeState)
		{
		case ButtonStates.Buy:
			if (!_purchaseInProgress)
			{
				_purchaseInProgress = true;
				if (_currentThemeIndex == 0)
				{
					PurchaseHandler.Instance.PurchaseHoverboardType(_boardType, this);
				}
				else
				{
					PurchaseHandler.Instance.PurchaseHoverboardTheme(_boardType, _currentThemeIndex - 1, this);
				}
			}
			break;
		case ButtonStates.Select:
			if (_currentThemeIndex != 0)
			{
				ActivateTheme(_boardType, _currentThemeIndex - 1, true);
				OnChangedCurrentlyShownHoverboardModel();
			}
			HoverboardManager.Instance.SelectCurrentlyShownBoard(_screenContext);
			break;
		case ButtonStates.Trial:
		{
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.SpecialPowerBoard);
			if (activeEventManager == null)
			{
				break;
			}
			PlayerInfo.Instance.previousHoverboard = PlayerInfo.Instance.currentHoverboard;
			PlayerInfo.Instance.shouldRestoreAfterEvent = true;
			PlayerInfo.Instance.lastEventID = activeEventManager.GetEventID();
			bool found = false;
			Hoverboards.BoardType eventValue = activeEventManager.GetEventValue<Hoverboards.BoardType>(out found);
			if (found)
			{
				PlayerInfo.Instance.lastEventItemID = eventValue.ToString();
			}
			OnChangedCurrentlyShownHoverboardModel();
			HoverboardManager.Instance.SelectCurrentlyShownBoard(_screenContext);
			if (!SSEventsManagerController.instance.HasTheCurrentEventBeenTried())
			{
				UIScreenController.Instance.ShowUnlockAnimationForHoverboard(PlayerInfo.Instance.currentHoverboard);
				SSEventsManagerController.instance.CurrentEventHasBeenTriedNow();
				if (found)
				{
					Flurry.LogEventWithAParameter("Event Popup was shown", "Id", eventValue.ToString().ToUpper());
				}
			}
			break;
		}
		case ButtonStates.BuyWithInapp:
			if (!_purchaseInProgress)
			{
				_purchaseInProgress = true;
				PurchaseHandler.Instance.StartPurchaseBoardWithInapp(HoverboardManager.Instance.GetCurrentlyShownHoverboard(_screenContext), this);
			}
			break;
		case ButtonStates.Unselect:
			DeactivateTheme(_boardType, _currentThemeIndex - 1);
			break;
		}
		Action onClickSelectedButton = this.OnClickSelectedButton;
		if (onClickSelectedButton != null)
		{
			onClickSelectedButton();
		}
	}

	private void ActivateTheme(Hoverboards.BoardType boardType, int themeIndex, bool allowSignatureAnimation)
	{
		PlayerInfo.Instance.HoverboardCustomizationData.SetThemeActive(boardType, HoverboardThemes.GetThemeIdentifier(boardType, themeIndex), true);
		Flurry.LogEventWithAParameter("Hoverboard Theme activated", "Id", HoverboardThemes.GetThemeIdentifier(boardType, themeIndex).ToString());
		OnChangedCurrentlyShownHoverboardModel();
		Hoverboard.BoardAbility boardAbility = (allowSignatureAnimation ? HoverboardThemes.hoverboardCustomThemes[boardType][themeIndex].power : Hoverboard.BoardAbility.None);
		HoverboardManager.Instance.ChangeThemeActiveState(_screenContext, boardAbility);
	}

	private void DeactivateTheme(Hoverboards.BoardType boardType, int themeIndex)
	{
		PlayerInfo.Instance.HoverboardCustomizationData.SetThemeActive(boardType, HoverboardThemes.GetThemeIdentifier(boardType, themeIndex), false);
		Flurry.LogEventWithAParameter("Hoverboard Theme deactivated", "Id", HoverboardThemes.GetThemeIdentifier(boardType, themeIndex).ToString());
		OnChangedCurrentlyShownHoverboardModel();
		HoverboardManager.Instance.ChangeThemeActiveState(_screenContext);
	}

	public void PurchaseSuccessful(bool isBoardTheme = false, int themeIndex = 0)
	{
		_purchaseInProgress = false;
		if (isBoardTheme)
		{
			BoardThemePurchaseSuccessful(themeIndex);
		}
		else
		{
			BoardPurchaseSuccessful();
		}
		PlayerInfo.Instance.SaveIfDirty();
	}

	private void BoardPurchaseSuccessful()
	{
		HoverboardManager.Instance.SelectCurrentlyShownBoard(_screenContext);
		Hoverboards.Board value;
		if (Hoverboards.boardData.TryGetValue(PlayerInfo.Instance.currentHoverboard, out value))
		{
			Flurry.LogEventWithAParameter("Hoverboard bought", "Id", char.ToUpper(Strings.Get(value.name)[0]) + Strings.Get(value.name).Substring(1));
			UIScreenController.Instance.ShowUnlockAnimationForHoverboard(PlayerInfo.Instance.currentHoverboard);
		}
		if (_boardUnlockType == Hoverboards.UnlockType.free)
		{
			Flurry.LogEventWithAParameter("Bread crumbs before purchasing the free hoverboard", "Total", PlayerInfo.Instance.numberOfBreadCrumbsShownOnFrontPage + string.Empty);
		}
	}

	private void BoardThemePurchaseSuccessful(int themeIndex)
	{
		HoverboardManager.Instance.SelectCurrentlyShownBoard(_screenContext);
		if (PlayerInfo.Instance.HoverboardCustomizationData.IsThemeOwned(_boardType, HoverboardThemes.GetThemeIdentifier(_boardType, themeIndex)))
		{
			ActivateTheme(_boardType, themeIndex, false);
			UIScreenController.Instance.ShowUnlockAnimationForHoverboard(PlayerInfo.Instance.currentHoverboard, HoverboardThemes.hoverboardCustomThemes[_boardType][themeIndex].power);
			Flurry.LogEventWithAParameter("Hoverboard Theme bought", "Id", HoverboardThemes.GetThemeIdentifier(_boardType, themeIndex).ToString());
		}
	}

	public void PurchaseFailure()
	{
		_purchaseInProgress = false;
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

	[Conditional("ENABLE_BOARD_SELECT_BUTTON_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}
}
