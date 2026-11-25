using System;
using UnityEngine;

public class CoinEarnerVideoAds : MonoBehaviour
{
	public enum ButtonState
	{
		Loading = 0,
		NoVideo = 1,
		VideoReady = 2
	}

	private const string BG_SPRITENAME_ACTIVE_DEFAULT = "button_fill_shopItem_main";

	private const string BG_SPRITENAME_OVERLAY_DEFAULT = "button_fill_shopItem_highlight";

	private const string BG_SPRITENAME_ACTIVE_SPECIAL = "button_fill_shopItem_main_sale";

	private const string BG_SPRITENAME_OVERLAY_SPECIAL = "button_fill_shopItem_highlight_sale";

	private const string BG_SPRITENAME_INACTIVE = "button_generic_1";

	[SerializeField]
	private GameObject _videoAvailableGameObject;

	[SerializeField]
	private GameObject _loadingGameObject;

	[SerializeField]
	private GameObject _noVideosGameObject;

	[SerializeField]
	private GameObject _contentMinibox;

	[SerializeField]
	private GameObject _contentKeys;

	[SerializeField]
	private UISprite _fillOverlay;

	[SerializeField]
	private UISprite _backgroundFill;

	[SerializeField]
	private UISprite _icon;

	[SerializeField]
	private UILabel _title;

	[SerializeField]
	private UILabel _description;

	[SerializeField]
	private UILabel _priceLabelKeys;

	private int _earnCurrencyProfileIndex;

	private Action _onClickDelegate;

	private ButtonState _currentButtonState;

	private static VideoAdsManager _adsManagerInstance;

	private UIButtonColor _btColor;

	private string _defaultTitle;

	private string _defaultDescription;

	private int _defaultPrice;

	public void Init(int earnCurrencyProfileIndex, string title, string desc, string iconName, int price, Action onClickDelegate, UIScrollView _panel)
	{
		_defaultTitle = title;
		_defaultDescription = desc;
		_defaultPrice = price;
		_earnCurrencyProfileIndex = earnCurrencyProfileIndex;
		_onClickDelegate = onClickDelegate;
		_icon.spriteName = iconName;
		base.gameObject.GetComponent<UIDragScrollView>().scrollView = _panel;
		GetVideoState();
	}

	private void SetButtonText(string title, string desc, int price, bool keys)
	{
		_title.text = title;
		_description.text = desc;
		if (keys && _priceLabelKeys != null)
		{
			_priceLabelKeys.text = price.ToString();
		}
	}

	private void OnClick()
	{
		if (_currentButtonState == ButtonState.VideoReady)
		{
			Flurry.LogEventWithAParameter("Video Ads button clicked", "State", string.Concat(ButtonState.VideoReady, string.Empty));
			EarnCoinsData.Trigger(_earnCurrencyProfileIndex);
			if (_onClickDelegate != null)
			{
				_onClickDelegate();
			}
		}
		else if (_currentButtonState == ButtonState.NoVideo)
		{
			Flurry.LogEventWithAParameter("Video Ads button clicked", "State", string.Concat(ButtonState.NoVideo, string.Empty));
			UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get(StringID.EARN_COINS_NATIVE_POPUP_TITLE));
			_adsManagerInstance.TryCachingVideosAgain();
		}
		else if (_currentButtonState != ButtonState.Loading)
		{
		}
	}

	private void Awake()
	{
		_adsManagerInstance = VideoAdsManager.instance;
	}

	private void SetButtonState(ButtonState state)
	{
		_currentButtonState = state;
		switch (state)
		{
		case ButtonState.VideoReady:
			SetStateGameObjectActive(_videoAvailableGameObject);
			break;
		case ButtonState.Loading:
			SetStateGameObjectActive(_loadingGameObject);
			break;
		case ButtonState.NoVideo:
			SetStateGameObjectActive(_videoAvailableGameObject);
			break;
		default:
			SetStateGameObjectActive(_loadingGameObject);
			break;
		}
	}

	private void SetStateGameObjectActive(GameObject go)
	{
		if (_videoAvailableGameObject != null)
		{
			_videoAvailableGameObject.SetActive(false);
		}
		if (_loadingGameObject != null)
		{
			_loadingGameObject.SetActive(false);
		}
		if (_noVideosGameObject != null)
		{
			_noVideosGameObject.SetActive(false);
		}
		if (go != null)
		{
			go.SetActive(true);
		}
		if (go == _videoAvailableGameObject || go == _noVideosGameObject)
		{
			if (_currentButtonState == ButtonState.VideoReady || _currentButtonState == ButtonState.NoVideo)
			{
				SetButtonText(Strings.Get(StringID.EARN_KEYS_SPONSORED_VIDEO), Strings.Get(StringID.EARN_KEYS_SPONSORED_VIDEO_DESCRIPTION), VideoAdsManager.instance.GetAmountOfKeysRewardedForVideo(), true);
				_contentKeys.SetActive(true);
				_contentMinibox.SetActive(false);
				_backgroundFill.spriteName = "button_fill_shopItem_main_sale";
				_backgroundFill.color = GlobalColors.COLOR_WHITE;
				_fillOverlay.spriteName = "button_fill_shopItem_highlight_sale";
				AddButtonColor(_backgroundFill);
				_fillOverlay.gameObject.SetActive(true);
				_title.color = GlobalColors.WATCH_VIDEO_FOR_COINS_TITLE_KEYS;
				_title.effectColor = GlobalColors.WATCH_VIDEO_FOR_COINS_TITLE_SHADOW_KEYS;
				_description.color = GlobalColors.WATCH_VIDEO_FOR_COINS_DESCRIPTION_KEYS;
				_description.effectColor = GlobalColors.WATCH_VIDEO_FOR_COINS_DESCRIPTION_SHADOW_KEYS;
			}
			else
			{
				SetButtonText(_defaultTitle, _defaultDescription, _defaultPrice, false);
				_contentKeys.SetActive(false);
				_contentMinibox.SetActive(true);
				_backgroundFill.spriteName = "button_fill_shopItem_main";
				_backgroundFill.color = GlobalColors.COLOR_WHITE;
				_fillOverlay.spriteName = "button_fill_shopItem_highlight";
				AddButtonColor(_backgroundFill);
				_fillOverlay.gameObject.SetActive(true);
				_title.color = GlobalColors.WATCH_VIDEO_FOR_COINS_TITLE_COINS;
				_title.effectColor = GlobalColors.WATCH_VIDEO_FOR_COINS_TITLE_SHADOW_COINS;
				_description.color = GlobalColors.WATCH_VIDEO_FOR_COINS_DESCRIPTION_COINS;
				_description.effectColor = GlobalColors.WATCH_VIDEO_FOR_COINS_DESCRIPTION_SHADOW_COINS;
			}
		}
		else
		{
			if (_btColor != null)
			{
				UnityEngine.Object.Destroy(_btColor);
			}
			_fillOverlay.gameObject.SetActive(false);
			_backgroundFill.color = GlobalColors.PRIMARY_ACTION_COLOR_UNAVAILABLE;
			_backgroundFill.spriteName = "button_generic_1";
		}
	}

	private void AddButtonColor(UISprite tweenTarget)
	{
		if (_btColor != null)
		{
			UnityEngine.Object.Destroy(_btColor);
		}
		_btColor = base.gameObject.AddComponent<UIButtonColor>();
		_btColor.tweenTarget = tweenTarget.gameObject;
		_btColor.hover = GlobalColors.COLOR_WHITE;
		_btColor.pressed = GlobalColors.COLOR_PRESSED;
		_btColor.duration = 0.05f;
	}

	private void OnEnable()
	{
		_adsManagerInstance.AddOnVideoStateChangedHandler(GetVideoState);
		_adsManagerInstance.AddOnKeyVideoWatchedChangedHandler(GetVideoState);
		if (!(base.gameObject.GetComponent<UIDragScrollView>().scrollView != null))
		{
		}
	}

	private void OnDisable()
	{
		_adsManagerInstance.RemoveOnVideoStateChangedHandler(GetVideoState);
		_adsManagerInstance.RemoveOnKeyVideoWatchedChangedHandler(GetVideoState);
	}

	private void GetVideoState()
	{
		switch (_adsManagerInstance.GetCurrentVideoState())
		{
		case VideoAdsManager.VideoState.Downloading:
			SetButtonState(ButtonState.Loading);
			break;
		case VideoAdsManager.VideoState.Offline:
		case VideoAdsManager.VideoState.NoVideos:
			SetButtonState(ButtonState.NoVideo);
			break;
		case VideoAdsManager.VideoState.Ready:
			SetButtonState(ButtonState.VideoReady);
			break;
		}
	}
}
