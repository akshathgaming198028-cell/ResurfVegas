using System;
using System.Collections.Generic;
using UnityEngine;

public class CoinScreen : UIScreen
{
	public GameObject coinPrefab;

	public GameObject coinEarnerPrefab;

	public GameObject restorePrefab;

	public GameObject watchVideoPrefab;

	public GameObject facebookGiftPrefab;

	public Font headerFont;

	[SerializeField]
	private UITable _table;

	[SerializeField]
	private UIScrollView _parentDragPanel;

	[SerializeField]
	private UIWidget _outline;

	[SerializeField]
	private GameObject listTitleComponent;

	[SerializeField]
	private bool shouldShowEarners;

	[NonSerialized]
	public int counterForElementsOnDisplay;

	private GameObject go;

	private bool isVideoAddsSupported
	{
		get
		{
			RuntimePlatform platform = Application.platform;
			return platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer;
		}
	}

	public override void Init()
	{
		base.Init();
		FillTable();
		if (UIScreen.IsScreenHeightOutOfProportion())
		{
			Vector3 localPosition = _parentDragPanel.transform.parent.localPosition;
			localPosition.y = 0f;
			_parentDragPanel.transform.parent.localPosition = localPosition;
		}
		InitializeCoinbox(_outline, false, true, true);
	}

	private void OnEnable()
	{
		if (InAppManager.instance != null)
		{
			InAppManager.instance.onProductRequestDone += OnProductRequestDone;
		}
	}

	private void OnDisable()
	{
		if (InAppManager.instance != null)
		{
			InAppManager.instance.onProductRequestDone -= OnProductRequestDone;
		}
	}

	public override void Show()
	{
		base.Show();
		if (_footerHandler != null)
		{
			_footerHandler.HandleStickers();
		}
		RefreshCurrencyEarners();
	}

	public void RefreshCurrencyEarners()
	{
		FillTable();
	}

	private void FillTable()
	{
		counterForElementsOnDisplay = 0;
		foreach (Transform item in _table.transform)
		{
			NGUITools.SetActive(item.gameObject, false);
			UnityEngine.Object.Destroy(item.gameObject);
		}
		int num = 0;
		GameObject gameObject = NGUITools.AddChild(_table.gameObject, listTitleComponent);
		ListTitleComponentHelper component = gameObject.GetComponent<ListTitleComponentHelper>();
		gameObject.name = string.Format("{0:000}", num);
		component.Setup(Strings.Get(StringID.COIN_SCREEN_STORE), new Color(0f, 0.2901961f, 0.5019608f, 1f), -142f);
		gameObject.GetComponent<UIDragScrollView>().scrollView = _parentDragPanel;
		num++;
		Dictionary<string, InAppItemModel> activeInAppOffers = InAppManager.GetActiveInAppOffers();
		foreach (KeyValuePair<string, InAppItemModel> item2 in activeInAppOffers)
		{
			go = NGUITools.AddChild(_table.gameObject, coinPrefab);
			go.name = string.Format("{0:000}", num);
			go.GetComponent<CoinButtonHelper>().Init(item2.Value);
			go.GetComponent<UIDragScrollView>().scrollView = _parentDragPanel;
			NGUITools.AddWidgetCollider(go);
			num++;
		}
		go = NGUITools.AddChild(_table.gameObject, restorePrefab);
		go.name = string.Format("{0:000}", num);
		go.GetComponent<UIDragScrollView>().scrollView = _parentDragPanel;
		go.GetComponent<RestorePurchaseHelper>().setOnClickDelegate(RefreshCurrencyEarners);
		NGUITools.AddWidgetCollider(go);
		num++;
		if (shouldShowEarners)
		{
			GameObject gameObject2 = NGUITools.AddChild(_table.gameObject, listTitleComponent);
			ListTitleComponentHelper component2 = gameObject2.GetComponent<ListTitleComponentHelper>();
			gameObject2.name = string.Format("{0:000}", num);
			component2.Setup(Strings.Get(StringID.COIN_SCREEN_EARN_COINS), new Color(0f, 0.2901961f, 0.5019608f, 1f), -150f);
			gameObject2.GetComponent<UIDragScrollView>().scrollView = _parentDragPanel;
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
			for (int l = 0; l < EarnCoinsData.profiles.Length; l++)
			{
				EarnCoinsData.EarnCurrencyProfile earnCurrencyProfile = EarnCoinsData.profiles[l];
				if (earnCurrencyProfile.id != EarnCoinsData.Id.VideoAd && earnCurrencyProfile.id != EarnCoinsData.Id.FacebookWeeklyGift && earnCurrencyProfile.id != EarnCoinsData.Id.FacebookLogin)
				{
					InsertEarner(l, ref num);
				}
			}
		}
		_table.Reposition();
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
				go = NGUITools.AddChild(_table.gameObject, watchVideoPrefab);
				go.name = string.Format("{0:000}", numberOfObjects);
				string desc = string.Format(Strings.Get(earnCurrencyProfile.desc), earnCurrencyProfile.GetAmountOfCoins());
				go.GetComponent<CoinEarnerVideoAds>().Init(index, Strings.Get(earnCurrencyProfile.title), desc, earnCurrencyProfile.iconName, earnCurrencyProfile.GetAmountOfCoins(), null, _parentDragPanel);
				go.GetComponent<UIDragScrollView>().scrollView = _parentDragPanel;
				NGUITools.AddWidgetCollider(go);
				UIPlaySound component = go.GetComponent<UIPlaySound>();
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
				GameObject gameObject = NGUITools.AddChild(_table.gameObject, facebookGiftPrefab);
				gameObject.name = string.Format("{0:000}", numberOfObjects);
				gameObject.GetComponent<FacebookGiftButton>().Init(index, Strings.Get(earnCurrencyProfile.title), Strings.Get(earnCurrencyProfile.desc), null, _parentDragPanel);
				gameObject.GetComponent<UIDragScrollView>().scrollView = _parentDragPanel;
				NGUITools.AddWidgetCollider(gameObject);
				UIPlaySound component2 = gameObject.GetComponent<UIPlaySound>();
				if (component2 != null)
				{
					component2.enabled = false;
				}
				numberOfObjects++;
			}
		}
		else
		{
			go = NGUITools.AddChild(_table.gameObject, coinEarnerPrefab);
			go.name = string.Format("{0:000}", numberOfObjects);
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				string desc2 = string.Format(Strings.Get(earnCurrencyProfile.iOSDesc), earnCurrencyProfile.GetAmountOfCoins());
				go.GetComponent<CoinEarnerButtonHelper>().Init(index, Strings.Get(earnCurrencyProfile.title), desc2, earnCurrencyProfile.iconName, earnCurrencyProfile.GetAmountOfCoins(), RefreshCurrencyEarners);
			}
			else
			{
				string desc3 = string.Format(Strings.Get(earnCurrencyProfile.desc), earnCurrencyProfile.GetAmountOfCoins());
				go.GetComponent<CoinEarnerButtonHelper>().Init(index, Strings.Get(earnCurrencyProfile.title), desc3, earnCurrencyProfile.iconName, earnCurrencyProfile.GetAmountOfCoins(), RefreshCurrencyEarners);
			}
			go.GetComponent<UIDragScrollView>().scrollView = _parentDragPanel;
			NGUITools.AddWidgetCollider(go);
			numberOfObjects++;
		}
	}

	private void OnProductRequestDone()
	{
		Show();
	}
}
