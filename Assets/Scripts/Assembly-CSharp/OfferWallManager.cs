using System;
using System.Collections.Generic;
using System.Diagnostics;
using Kiloo.Monetization.Offerwall;
using Kiloo.Monetization.Offerwall.TapjoyOfferwall;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class OfferWallManager
{
	private static OfferWall _offerWall = null;

	private bool _isInitialized;

	private static readonly List<string> ALLOWED_REFRESH_SCREENS = new List<string> { "CoinsUI_shop", "UpgradesUI_shop", "EarnCoinsScreen", "AwardsScreen", "BoardScreen", "CharacterScreen", "FriendsUI", "LeaderboardUI", "FrontUI" };

	private static OfferWallManager _instance = null;

	public static OfferWallManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new OfferWallManager();
			}
			return _instance;
		}
	}

	public event Action<List<IDisplayableOffer>, bool> onOffersReceived;

	public event Action onOffersRequestFailed;

	public event Action<int> onPendingRewardsReceived;

	public event Action onPendingRewardsRequestFailed;

	public event Action<OffersResponseData> notifyOffersReceivedVSOffersRequested;

	private OfferWallManager()
	{
	}

	public bool AreOffersAllowedOnDevice()
	{
		bool result = false;
		if (_isInitialized)
		{
			result = _offerWall.AreOffersAllowedOnDevice();
		}
		return result;
	}

	public void Initialize()
	{
		if (!_isInitialized)
		{
			_offerWall = new OfferWall();
			_offerWall.onOffersReceived += OnOffersReceived;
			_offerWall.onOffersRequestFailed += this.onOffersRequestFailed;
			_offerWall.onPendingRewardsReceived += PayoutRewardToUser;
			_offerWall.onPendingRewardsRequestFailed += this.onPendingRewardsRequestFailed;
			UIScreenController.OnNavigateToScreen += RefreshPendingRewardsOnScreenShow;
			OfferWallDataManager dataManager = new OfferWallDataManager();
			_offerWall.Initialize(dataManager, new List<IOfferWallProvider>
			{
				new TapJoyOfferWallProvider()
			});
			_isInitialized = true;
		}
	}

	private void OnOffersReceived(List<IDisplayableOffer> offers, bool cached)
	{
		if (this.onOffersReceived != null)
		{
			this.onOffersReceived(offers, cached);
		}
	}

	public void RefreshOffers(bool bypassTimeLimitsEnforcedByOutside = false)
	{
		if (_isInitialized)
		{
			_offerWall.RefreshOffers(bypassTimeLimitsEnforcedByOutside);
		}
	}

	public void GetPendingRewards(bool bypassTimeLimitsEnforcedByOutside = false)
	{
		if (_isInitialized)
		{
			_offerWall.GetPendingRewards(bypassTimeLimitsEnforcedByOutside);
		}
	}

	private void PayoutRewardToUser(int rewardAmount)
	{
		if (rewardAmount > 0)
		{
			PlayerInfo.Instance.amountOfCoins += rewardAmount;
			Flurry.LogEventWithAParameter("Offerwall Offer Redeemed", "Daily report of coins", GetRewardAmountCategoryForValue(rewardAmount.ToString()));
			if (UIScreenController.IsInstanced)
			{
				UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.OfferWall, Mathf.CeilToInt(rewardAmount).ToString()));
			}
			if (this.onPendingRewardsReceived != null)
			{
				this.onPendingRewardsReceived(rewardAmount);
			}
		}
	}

	public void OfferClicked(IDisplayableOffer offer)
	{
		Flurry.LogEventWithAParameter("Offerwall Offer Clicked", "Daily report of coins", GetRewardAmountCategoryForValue(offer.payoutAmount));
	}

	private string GetRewardAmountCategoryForValue(string offerAmount)
	{
		int result = 0;
		string result2 = string.Empty;
		if (int.TryParse(offerAmount, out result))
		{
			result2 = result / 100 * 100 + " - " + (result / 100 * 100 + 99);
		}
		return result2;
	}

	private void RefreshPendingRewardsOnScreenShow(string screenName)
	{
		if (ALLOWED_REFRESH_SCREENS.Contains(screenName))
		{
			GetPendingRewards();
		}
	}

	private static void LogError(string msg, UnityEngine.Object context = null)
	{
		Debug.LogError("OfferWallManager: " + msg, context);
	}

	private static void LogWarning(string msg, UnityEngine.Object context = null)
	{
		Debug.LogWarning("OfferWallManager: " + msg, context);
	}

	[Conditional("OFFERWALL_DEBUG_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log("OfferWallManager: " + msg, context);
	}
}
