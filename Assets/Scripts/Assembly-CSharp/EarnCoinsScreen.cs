using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Kiloo.Monetization.Offerwall;
using UnityEngine;

public class EarnCoinsScreen : UIScreen
{
	private const int TITLE_FONT_SIZE = 30;

	private const float TABLE_CELL_SPACING = 2f;

	private const int CELL_HEIGHT = 64;

	public GameObject WatchVideoPrefab;

	public GameObject coinEarnerPrefab;

	public GameObject facebookGiftPrefab;

	public Font headerFont;

	[SerializeField]
	private UITable _table;

	[SerializeField]
	private UIScrollView _dragPanel;

	[SerializeField]
	private GameObject _loadingOffersPrefab;

	[SerializeField]
	private GameObject listTitleComponent;

	[SerializeField]
	private GameObject _scrollBar;

	private CoinEarnerLoadingOffer _coinEarnerLoadingOffer;

	private GameObject _tableItemParent;

	private UIPanel _panel;

	private static bool _facebookLoginRunning;

	private bool _forceRefreshOfferWall;

	private static bool _shouldRefreshCurrencyEarners;

	private int _onDisplayRowCounter;

	private bool _shouldWaitForUserFeedback;

	private List<IDisplayableOffer> _offers = new List<IDisplayableOffer>();

	private bool isVideoAddsSupported
	{
		get
		{
			RuntimePlatform platform = Application.platform;
			return platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer;
		}
	}

	[UsedImplicitly]
	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			return;
		}
		if (OfferWallManager.Instance.AreOffersAllowedOnDevice())
		{
			OfferWallManager.Instance.RefreshOffers(_forceRefreshOfferWall);
			if (_forceRefreshOfferWall)
			{
				_forceRefreshOfferWall = false;
			}
			else
			{
				RefreshCurrencyEarners();
			}
		}
		else
		{
			RefreshCurrencyEarners();
		}
	}

	public override void Init()
	{
		base.Init();
		InitializeCoinbox(null, true, true, true);
		_panel = _table.transform.parent.GetComponent<UIPanel>();
	}

	public override void Show()
	{
		base.Show();
		if (_footerHandler != null)
		{
			_footerHandler.HandleStickers();
		}
		_forceRefreshOfferWall = false;
		OfferWallManager.Instance.onOffersReceived += OffersDownloaded;
		OfferWallManager.Instance.onOffersRequestFailed += RequestForOffersFailed;
		if (OfferWallManager.Instance.AreOffersAllowedOnDevice())
		{
			OfferWallManager.Instance.RefreshOffers();
			OfferWallManager.Instance.GetPendingRewards();
		}
		RefreshCurrencyEarners();
		VideoAdsManager instance = VideoAdsManager.instance;
		VideoAdsManager.VideoState currentVideoState = instance.GetCurrentVideoState();
		Flurry.LogEventWithAParameter("Earn coins screen opened", "State", string.Concat(currentVideoState, string.Empty));
		if (instance.isInitialized)
		{
			instance.ResumeProvider();
			if (currentVideoState == VideoAdsManager.VideoState.NoVideos || currentVideoState == VideoAdsManager.VideoState.Offline)
			{
				instance.TryCachingVideosAgain();
			}
		}
	}

	public override void Hide()
	{
		base.Hide();
		OfferWallManager.Instance.onOffersReceived -= OffersDownloaded;
		OfferWallManager.Instance.onOffersRequestFailed -= RequestForOffersFailed;
		if (_facebookLoginRunning)
		{
			HouseKeeper.SetFacebookPayoutActionToDefault();
			_facebookLoginRunning = false;
		}
	}

	public void RefreshCurrencyEarners()
	{
		FillTable();
	}

	private void Update()
	{
		if (_shouldRefreshCurrencyEarners)
		{
			RefreshCurrencyEarners();
		}
	}

	private void FillTable()
	{
		foreach (Transform item in _table.transform)
		{
			NGUITools.SetActive(item.gameObject, false);
			UnityEngine.Object.Destroy(item.gameObject);
		}
		_onDisplayRowCounter = 0;
		int num = 0;
		_tableItemParent = NGUITools.AddChild(_table.gameObject);
		_tableItemParent.name = string.Format("{0:000}", num);
		GameObject gameObject = NGUITools.AddChild(_table.gameObject, listTitleComponent);
		ListTitleComponentHelper component = gameObject.GetComponent<ListTitleComponentHelper>();
		gameObject.transform.parent = _tableItemParent.transform;
		gameObject.name = string.Format("{0:000}", num);
		gameObject.transform.localPosition = new Vector3(0f, -67 * _onDisplayRowCounter + 15, 0f);
		component.Setup(Strings.Get(StringID.EARN_COINS_TITLE), new Color(0f, 0.2901961f, 0.5019608f, 1f), -142.5f);
		num++;
		for (int i = 0; i < EarnCoinsData.profiles.Length; i++)
		{
			if (EarnCoinsData.profiles[i].id == EarnCoinsData.Id.VideoAd)
			{
				InsertEarner(i, ref num);
				break;
			}
		}
		for (int j = 0; j < EarnCoinsData.profiles.Length; j++)
		{
			if (EarnCoinsData.profiles[j].id == EarnCoinsData.Id.FacebookWeeklyGift)
			{
				InsertEarner(j, ref num);
				break;
			}
		}
		for (int k = 0; k < EarnCoinsData.profiles.Length; k++)
		{
			if (EarnCoinsData.profiles[k].id == EarnCoinsData.Id.FacebookLogin)
			{
				if (!SocialManager.instance.facebookIsLoggedIn && !PlayerInfo.Instance.hasPayedOutFacebook)
				{
					InsertEarner(k, ref num);
				}
				break;
			}
		}
		if (OfferWallManager.Instance.AreOffersAllowedOnDevice())
		{
			if (_offers != null && _offers.Count > 0 && !_shouldWaitForUserFeedback)
			{
				InsertOffer(ref num, CoinEarnerLoadingOffer.OffersState.Offer, _offers[0]);
			}
			else
			{
				InsertOffer(ref num, CoinEarnerLoadingOffer.OffersState.Loading, null);
			}
		}
		for (int l = 0; l < EarnCoinsData.profiles.Length; l++)
		{
			EarnCoinsData.EarnCurrencyProfile earnCurrencyProfile = EarnCoinsData.profiles[l];
			if (earnCurrencyProfile.id != EarnCoinsData.Id.VideoAd && earnCurrencyProfile.id != EarnCoinsData.Id.FacebookWeeklyGift && earnCurrencyProfile.id != EarnCoinsData.Id.FacebookLogin)
			{
				InsertEarner(l, ref num);
			}
		}
		if (num == 1)
		{
			_table.gameObject.transform.localPosition = new Vector3(-85f, 452f, 0f);
		}
		else
		{
			_table.gameObject.transform.localPosition = new Vector3(-145f, 452f, 0f);
		}
		if (_offers != null && _offers.Count > 1 && !_shouldWaitForUserFeedback)
		{
			for (int m = 1; m < _offers.Count; m++)
			{
				InsertOffer(ref num, CoinEarnerLoadingOffer.OffersState.Offer, _offers[m]);
			}
		}
		if (Application.platform == RuntimePlatform.WP8Player && num == 1)
		{
			gameObject.SetActive(false);
		}
		if (_panel != null)
		{
			if (Math.Abs(Math.Abs(_panel.baseClipRegion.w)) < 1f)
			{
				StartCoroutine(WAIT(num));
			}
			else
			{
				SetScrollViewCenterOffset(num);
			}
		}
		_shouldRefreshCurrencyEarners = false;
	}

	private void InsertEarner(int index, ref int numberOfObjects)
	{
		if (EarnCoinsData.profiles.Length <= index || !EarnCoinsData.ShouldShowInGUI(index))
		{
			return;
		}
		EarnCoinsData.EarnCurrencyProfile earnCurrencyProfile = EarnCoinsData.profiles[index];
		if (earnCurrencyProfile.id == EarnCoinsData.Id.VideoAd)
		{
			if (isVideoAddsSupported)
			{
				_onDisplayRowCounter++;
				GameObject gameObject = NGUITools.AddChild(_table.gameObject, WatchVideoPrefab);
				gameObject.name = string.Format("{0:000}", numberOfObjects);
				gameObject.transform.parent = _tableItemParent.transform;
				gameObject.transform.localPosition = new Vector3(0f, -67 * _onDisplayRowCounter + 15, 0f);
				string desc = string.Format(Strings.Get(earnCurrencyProfile.desc), earnCurrencyProfile.GetAmountOfCoins());
				gameObject.GetComponent<CoinEarnerVideoAds>().Init(index, Strings.Get(earnCurrencyProfile.title), desc, earnCurrencyProfile.iconName, earnCurrencyProfile.GetAmountOfCoins(), null, _dragPanel);
				gameObject.GetComponent<UIDragScrollView>().scrollView = _dragPanel;
				NGUITools.AddWidgetCollider(gameObject);
				UIPlaySound component = gameObject.GetComponent<UIPlaySound>();
				if (component != null)
				{
					component.enabled = false;
				}
				numberOfObjects++;
			}
		}
		else if (earnCurrencyProfile.id == EarnCoinsData.Id.FacebookWeeklyGift)
		{
			if (FacebookWeeklyGiftManager.Instance.IsGiftAvailable())
			{
				_onDisplayRowCounter++;
				GameObject gameObject2 = NGUITools.AddChild(_table.gameObject, facebookGiftPrefab);
				gameObject2.name = string.Format("{0:000}", numberOfObjects);
				gameObject2.transform.parent = _tableItemParent.transform;
				gameObject2.transform.localPosition = new Vector3(0f, -67 * _onDisplayRowCounter + 15, 0f);
				gameObject2.GetComponent<FacebookGiftButton>().Init(index, Strings.Get(earnCurrencyProfile.title), Strings.Get(earnCurrencyProfile.desc), null, _dragPanel);
				gameObject2.GetComponent<UIDragScrollView>().scrollView = _dragPanel;
				NGUITools.AddWidgetCollider(gameObject2);
				UIPlaySound component2 = gameObject2.GetComponent<UIPlaySound>();
				if (component2 != null)
				{
					component2.enabled = false;
				}
				numberOfObjects++;
			}
		}
		else
		{
			_onDisplayRowCounter++;
			GameObject gameObject3 = NGUITools.AddChild(_table.gameObject, coinEarnerPrefab);
			gameObject3.transform.parent = _tableItemParent.transform;
			gameObject3.transform.localPosition = new Vector3(0f, -67 * _onDisplayRowCounter + 15, 0f);
			gameObject3.name = string.Format("{0:000}", numberOfObjects);
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				string desc2 = string.Format(Strings.Get(earnCurrencyProfile.iOSDesc), earnCurrencyProfile.GetAmountOfCoins());
				gameObject3.GetComponent<CoinEarnerButtonHelper>().Init(index, Strings.Get(earnCurrencyProfile.title), desc2, earnCurrencyProfile.iconName, earnCurrencyProfile.GetAmountOfCoins(), null);
			}
			else
			{
				string desc3 = string.Format(Strings.Get(earnCurrencyProfile.desc), earnCurrencyProfile.GetAmountOfCoins());
				gameObject3.GetComponent<CoinEarnerButtonHelper>().Init(index, Strings.Get(earnCurrencyProfile.title), desc3, earnCurrencyProfile.iconName, earnCurrencyProfile.GetAmountOfCoins(), null);
			}
			gameObject3.GetComponent<UIDragScrollView>().scrollView = _dragPanel;
			NGUITools.AddWidgetCollider(gameObject3);
			numberOfObjects++;
		}
	}

	private void InsertOffer(ref int numberOfObjects, CoinEarnerLoadingOffer.OffersState offerState, IDisplayableOffer offer)
	{
		_onDisplayRowCounter++;
		GameObject gameObject = NGUITools.AddChild(_table.gameObject, _loadingOffersPrefab);
		gameObject.transform.parent = _tableItemParent.transform;
		gameObject.transform.localPosition = new Vector3(0f, -67 * _onDisplayRowCounter + 15, 0f);
		gameObject.name = string.Format("{0:000}", numberOfObjects);
		gameObject.GetComponent<UIDragScrollView>().scrollView = _dragPanel;
		_coinEarnerLoadingOffer = gameObject.GetComponent<CoinEarnerLoadingOffer>();
		NGUITools.AddWidgetCollider(gameObject);
		numberOfObjects++;
		if (_coinEarnerLoadingOffer != null)
		{
			if (offer != null)
			{
				_coinEarnerLoadingOffer.Init(offer, EnableRfreshOfferwall);
			}
			_coinEarnerLoadingOffer.SetOffersState(offerState);
		}
	}

	private void EnableRfreshOfferwall()
	{
		_forceRefreshOfferWall = true;
	}

	private void OffersDownloaded(List<IDisplayableOffer> offers, bool cachedList)
	{
		if (offers == null || offers.Count == 0)
		{
			if (_coinEarnerLoadingOffer != null)
			{
				_coinEarnerLoadingOffer.SetOffersState(CoinEarnerLoadingOffer.OffersState.NoOffersAvailable);
			}
			_shouldWaitForUserFeedback = false;
			RefreshCurrencyEarners();
			return;
		}
		_offers = offers;
		_shouldWaitForUserFeedback = !cachedList;
		if (!cachedList)
		{
			if (_coinEarnerLoadingOffer != null && _coinEarnerLoadingOffer.getOffersState() != CoinEarnerLoadingOffer.OffersState.Loading)
			{
				RefreshCurrencyEarners();
			}
			StartCoroutine(DelayingRefreshForUserFeedback());
		}
	}

	private IEnumerator DelayingRefreshForUserFeedback()
	{
		yield return new WaitForSeconds(1f);
		_shouldWaitForUserFeedback = false;
		RefreshCurrencyEarners();
	}

	private void RequestForOffersFailed()
	{
		_shouldWaitForUserFeedback = false;
		RefreshCurrencyEarners();
	}

	public static void FacebookLoginClicked()
	{
		HouseKeeper.SetFacebookPayoutAction(FacebookStatusChanged);
		_facebookLoginRunning = true;
		if (SocialManager.instance.FacebookLogin(OnFacebookLoginResult))
		{
			UIScreenController.Instance.FacebookLogIn(true);
		}
	}

	public static void OnFacebookLoginResult(bool loggedIn)
	{
		if (!loggedIn && _facebookLoginRunning)
		{
			_facebookLoginRunning = false;
			QueueFacebookFailSlideIn(OnFailSlideInFinish);
		}
	}

	private static void QueueFacebookFailSlideIn(Action onFinishCallback)
	{
		UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Facebook, UISlideInFacebook.FacebookSlideInState.ConnectFailed, onFinishCallback));
	}

	private static void OnFailSlideInFinish()
	{
		FacebookStatusChanged(false);
	}

	private static void FacebookStatusChanged(bool success)
	{
		if (success)
		{
			if (!FacebookRewardsInfo.HasReceivedAllAvailableRewards())
			{
				UIScreenController.Instance.PushPopup("FacebookRewardsPopup");
			}
			else
			{
				UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Facebook, UISlideInFacebook.FacebookSlideInState.ConnectSucceeded));
			}
		}
		else if (_facebookLoginRunning)
		{
			OnFacebookLoginResult(false);
		}
		HouseKeeper.SetFacebookPayoutActionToDefault();
		_shouldRefreshCurrencyEarners = true;
		_facebookLoginRunning = false;
	}

	private void OnDisable()
	{
		if (_facebookLoginRunning)
		{
			HouseKeeper.SetFacebookPayoutActionToDefault();
			_facebookLoginRunning = false;
		}
	}

	private IEnumerator WAIT(int numberOfObjects)
	{
		yield return new WaitForEndOfFrame();
		SetScrollViewCenterOffset(numberOfObjects);
	}

	private void SetScrollViewCenterOffset(int numberOfObjects)
	{
		if (IsListBiggerThanClip(numberOfObjects))
		{
			_dragPanel.enabled = true;
			_scrollBar.SetActive(true);
		}
		else
		{
			UIWidget uIWidget = NGUITools.AddWidget<UIWidget>(_table.gameObject);
			uIWidget.height = 1000;
			_dragPanel.ResetPosition();
			_dragPanel.enabled = false;
			_scrollBar.SetActive(false);
		}
		_table.Reposition();
	}

	private bool IsListBiggerThanClip(int nbOfElementsInList)
	{
		bool result = false;
		int num = (nbOfElementsInList - 1) * 64 + 30;
		float num2 = (float)nbOfElementsInList * 2f;
		if (_panel != null && (float)num + num2 > _panel.baseClipRegion.w)
		{
			result = true;
		}
		return result;
	}
}
