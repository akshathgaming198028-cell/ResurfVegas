using UnityEngine;

public class BreadCrumbHelper : MonoBehaviour
{
	private enum BreadCrumbStates
	{
		inactive = 0,
		giftCharacter = 1,
		giftBoard = 2,
		boardEventActive = 3,
		expiredLimitedCharacter = 4,
		biggerJackpot = 5,
		earnKeys = 6,
		collectAward = 7,
		expiredLimitedBoard = 8,
		topRun = 9,
		characterDay = 10
	}

	private enum BreadCrumbLocations
	{
		meButton = 0,
		shopButton = 1,
		hoverboardsButton = 2,
		charactersButton = 3,
		upgradesFooter = 4,
		upgradesScroll = 5,
		earnCoinsFooterButton = 6,
		earnCoinsVideoButton = 7,
		mysteryBoxPrizeList = 8,
		awardsButton = 9,
		friendsButton = 10,
		topRunButton = 11,
		topRunTab = 12
	}

	[SerializeField]
	private UISprite sticker;

	[SerializeField]
	private UISprite stickerShadow;

	[SerializeField]
	private UILabel topLabel;

	[SerializeField]
	private UILabel bottomLabel;

	[SerializeField]
	private UISprite eventIcon;

	[SerializeField]
	private UISprite awardIcon;

	[SerializeField]
	private UISprite topRunIcon;

	[SerializeField]
	private GameObject giftLabels;

	[SerializeField]
	private GameObject notification;

	[SerializeField]
	private GameObject discount;

	private bool _hideSaleSticker;

	private BreadCrumbStates _currentState;

	[SerializeField]
	private BreadCrumbLocations _currentLocation;

	private void OnEnable()
	{
		SSEventsManagerController.OnSSEventStartedHandler += SetupBreadCrumbState;
		SSEventsManagerController.OnSSEventFinishedHandler += setLabelsDisabled;
		UIScreenController.OnApplicationResumed += DisableEventBreadcrumb;
		SetupBreadCrumbState();
	}

	private void OnDisable()
	{
		SSEventsManagerController.OnSSEventStartedHandler -= SetupBreadCrumbState;
		SSEventsManagerController.OnSSEventFinishedHandler -= setLabelsDisabled;
		UIScreenController.OnApplicationResumed -= DisableEventBreadcrumb;
	}

	public void SetupBreadCrumbState()
	{
		_hideSaleSticker = false;
		_currentState = BreadCrumbStates.inactive;
		string topScreenName = UIScreenController.Instance.GetTopScreenName();
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (_currentLocation == BreadCrumbLocations.meButton)
		{
			if (AwardsManager.Instance.GetAwardsForState(AwardState.Collectible).Count > 0)
			{
				_currentState = BreadCrumbStates.collectAward;
			}
			else if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.SpecialPowerBoard)
			{
				_currentState = BreadCrumbStates.boardEventActive;
			}
			else if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.ExpiredLimitedCharacter)
			{
				_currentState = BreadCrumbStates.expiredLimitedCharacter;
			}
			else if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.CharacterDay)
			{
				_currentState = BreadCrumbStates.characterDay;
			}
			else if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.ExpiredLimitedBoard)
			{
				_currentState = BreadCrumbStates.expiredLimitedBoard;
			}
			else if (ShouldShowBoardsGiftSticker())
			{
				_currentState = BreadCrumbStates.giftBoard;
			}
			else if (ShouldShowCharactersGiftSticker())
			{
				_currentState = BreadCrumbStates.giftCharacter;
			}
		}
		else if (_currentLocation == BreadCrumbLocations.hoverboardsButton && !topScreenName.Equals("BoardScreen") && !SSEventsManagerController.instance.HasTheCurrentEventBeenTried())
		{
			if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.SpecialPowerBoard)
			{
				_currentState = BreadCrumbStates.boardEventActive;
			}
			else if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.ExpiredLimitedBoard)
			{
				_currentState = BreadCrumbStates.expiredLimitedBoard;
			}
			else if (ShouldShowBoardsGiftSticker())
			{
				_currentState = BreadCrumbStates.giftBoard;
			}
		}
		else if (_currentLocation == BreadCrumbLocations.charactersButton && !topScreenName.Equals("CharacterScreen"))
		{
			if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.ExpiredLimitedCharacter)
			{
				_currentState = BreadCrumbStates.expiredLimitedCharacter;
			}
			else if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.CharacterDay)
			{
				_currentState = BreadCrumbStates.characterDay;
			}
			else if (ShouldShowCharactersGiftSticker())
			{
				_currentState = BreadCrumbStates.giftCharacter;
			}
		}
		else if (_currentLocation == BreadCrumbLocations.awardsButton && !topScreenName.Equals("AwardsScreen"))
		{
			if (AwardsManager.Instance.GetAwardsForState(AwardState.Collectible).Count > 0)
			{
				_currentState = BreadCrumbStates.collectAward;
			}
		}
		else if (((_currentLocation == BreadCrumbLocations.shopButton || _currentLocation == BreadCrumbLocations.upgradesFooter) && !topScreenName.Equals("UpgradesUI_shop")) || _currentLocation == BreadCrumbLocations.upgradesScroll || _currentLocation == BreadCrumbLocations.mysteryBoxPrizeList)
		{
			if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.BiggerJackpot)
			{
				_currentState = BreadCrumbStates.biggerJackpot;
				_hideSaleSticker = true;
			}
		}
		else if ((_currentLocation == BreadCrumbLocations.earnCoinsVideoButton || (_currentLocation == BreadCrumbLocations.earnCoinsFooterButton && !topScreenName.Equals("EarnCoinsScreen"))) && ShouldShowEarnKeysSticker())
		{
			_currentState = BreadCrumbStates.earnKeys;
		}
		else if ((_currentLocation == BreadCrumbLocations.friendsButton || _currentLocation == BreadCrumbLocations.topRunButton || _currentLocation == BreadCrumbLocations.topRunTab) && TopRunRewardHelper.ShouldShowResults())
		{
			bool flag = false;
			if (topScreenName.Equals("GameoverUI"))
			{
				GameOverScreen gameOverScreen = UIScreenController.Instance.GetScreenFromCache(topScreenName) as GameOverScreen;
				if (gameOverScreen != null && gameOverScreen.GetActiveTabState(SetupBreadCrumbState) == GameOverScreen.ActiveTabState.leaderBoard)
				{
					flag = true;
				}
			}
			else if (!topScreenName.Equals("TopRunUI"))
			{
				flag = true;
			}
			if (flag)
			{
				_currentState = BreadCrumbStates.topRun;
			}
		}
		SetupBreadCrumb();
	}

	private bool ShouldShowEarnKeysSticker()
	{
		if (VideoAdsManager.instance.IsVideoForKeysAvailable())
		{
			return true;
		}
		if (_currentLocation != BreadCrumbLocations.earnCoinsVideoButton)
		{
			return false;
		}
		VideoAdsManager.VideoState videoState = VideoAdsManager.instance.GetCurrentVideoState();
		return videoState == VideoAdsManager.VideoState.NoVideos || videoState == VideoAdsManager.VideoState.Offline;
	}

	private bool ShouldShowBoardsGiftSticker()
	{
		return HoverboardStickerManager.shouldShowHoverboardStickerFLow() && HoverboardStickerManager.IsAnyGiftBoard();
	}

	private bool ShouldShowCharactersGiftSticker()
	{
		return CharacterStickerManager.shouldShowCharacterThemesStickerFLow() && CharacterStickerManager.isAnyCharacterWithFreeTheme();
	}

	private void SetupBreadCrumb()
	{
		if (_currentState == BreadCrumbStates.inactive)
		{
			setLabelsDisabled();
		}
		else
		{
			setLabelsEnabled();
		}
	}

	private void setLabelsEnabled()
	{
		sticker.enabled = true;
		if (stickerShadow != null)
		{
			stickerShadow.enabled = true;
		}
		sticker.color = GlobalColors.GIFT_COLOR;
		if (notification != null)
		{
			notification.SetActive(false);
		}
		if (awardIcon != null)
		{
			awardIcon.enabled = _currentState == BreadCrumbStates.collectAward;
		}
		if (topRunIcon != null)
		{
			topRunIcon.enabled = _currentState == BreadCrumbStates.topRun;
		}
		eventIcon.enabled = _currentState == BreadCrumbStates.boardEventActive || _currentState == BreadCrumbStates.expiredLimitedCharacter || _currentState == BreadCrumbStates.characterDay || _currentState == BreadCrumbStates.biggerJackpot || _currentState == BreadCrumbStates.expiredLimitedBoard;
		bool flag = _currentState == BreadCrumbStates.giftBoard || _currentState == BreadCrumbStates.giftCharacter || _currentState == BreadCrumbStates.earnKeys;
		topLabel.enabled = flag;
		bottomLabel.enabled = flag;
		if (flag)
		{
			if (_currentState == BreadCrumbStates.earnKeys)
			{
				sticker.color = GlobalColorsGenerated.DISCOUNT_STICKER_COLOR;
				topLabel.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
				bottomLabel.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
				topLabel.text = Strings.Get(StringID.BREADCRUMB_EARN_KEYS_LINE1);
				bottomLabel.text = Strings.Get(StringID.BREADCRUMB_EARN_KEYS_LINE2);
			}
			else
			{
				topLabel.color = GlobalColors.GIFT_LABEL_COLOR;
				bottomLabel.color = GlobalColors.GIFT_LABEL_COLOR;
				topLabel.text = Strings.Get(StringID.BREADCRUMB_FREE_GIFT_LINE1);
				bottomLabel.text = Strings.Get(StringID.BREADCRUMB_FREE_GIFT_LINE2);
				sticker.color = GlobalColors.GIFT_COLOR;
			}
		}
		else if (eventIcon.enabled)
		{
			sticker.color = GlobalColors.GIFT_COLOR;
		}
		else if (awardIcon != null && awardIcon.enabled)
		{
			sticker.color = GlobalColors.AWARDS_COLLECTIBLE_STICKER_COLOR;
		}
		else if (topRunIcon != null && topRunIcon.enabled)
		{
			sticker.color = GlobalColors.TOP_RUN_STICKER_COLOR;
		}
		if (_currentLocation == BreadCrumbLocations.meButton)
		{
			PlayerInfo.Instance.BreadCrumbShownOnFrontPage();
		}
	}

	public void setLabelsDisabled()
	{
		if (sticker != null && topLabel != null && bottomLabel != null && eventIcon != null)
		{
			sticker.enabled = false;
			topLabel.enabled = false;
			bottomLabel.enabled = false;
			eventIcon.enabled = false;
		}
		if (stickerShadow != null)
		{
			stickerShadow.enabled = false;
		}
		if (awardIcon != null)
		{
			awardIcon.enabled = false;
		}
		if (topRunIcon != null)
		{
			topRunIcon.enabled = false;
		}
		if (notification != null)
		{
			notification.SetActive(true);
		}
		if (discount != null && _hideSaleSticker)
		{
			discount.SetActive(false);
		}
	}

	private void DisableEventBreadcrumb()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		bool flag = activeEventManager != null;
		if (base.gameObject.activeInHierarchy && !flag && (_currentState == BreadCrumbStates.boardEventActive || _currentState == BreadCrumbStates.characterDay || _currentState == BreadCrumbStates.expiredLimitedCharacter || _currentState == BreadCrumbStates.biggerJackpot))
		{
			setLabelsDisabled();
		}
	}

	private void Update()
	{
		if (_currentState == BreadCrumbStates.collectAward && stickerShadow != null && sticker != null)
		{
			float zAngle = Time.deltaTime * 60f;
			sticker.transform.Rotate(0f, 0f, zAngle);
			stickerShadow.transform.Rotate(0f, 0f, zAngle);
		}
	}
}
