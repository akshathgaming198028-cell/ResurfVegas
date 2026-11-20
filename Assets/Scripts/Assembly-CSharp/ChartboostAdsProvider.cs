using System.Diagnostics;
using Kiloo.Plugins.Chartboost;
using UnityEngine;

public class ChartboostAdsProvider : ScreenAdProvider
{
	private const string LOCATION_INTERSTITIAL = "Main Menu";

	private ScreenAdProvider _chartboostProvider;

	private static ChartboostAdsProvider _instance;

	private bool _isCaching;

	private bool _isPluginInitialized;

	public override bool isInitialized
	{
		get
		{
			return _isPluginInitialized;
		}
	}

	public static ChartboostAdsProvider instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ChartboostAdsProvider();
			}
			return _instance;
		}
	}

	public override bool isScreenAdProviderSupported
	{
		get
		{
			return Chartboost.IsSupportedOnPlatform() && !string.IsNullOrEmpty(AdConstants.instance.ChartboostAppId) && !string.IsNullOrEmpty(AdConstants.instance.ChartboostAppSignature);
		}
	}

	public override bool HasCachedInterstials
	{
		get
		{
			return Chartboost.HasCachedInterstitial("Main Menu");
		}
	}

	public override string id
	{
		get
		{
			return "chartboost";
		}
	}

	private ChartboostAdsProvider()
	{
	}

	public override void Init()
	{
		if (!_isPluginInitialized && isScreenAdProviderSupported)
		{
			_isPluginInitialized = true;
			Chartboost.StartSession(AdConstants.instance.ChartboostAppId, AdConstants.instance.ChartboostAppSignature);
			Chartboost.onClickInterstitial += OnDidClickInterstitial;
			Chartboost.onFailToLoadInterstitial += OnDidFailToLoadInterstitial;
			Chartboost.onCacheInterstitial += OnDidCacheInterstitial;
			Chartboost.onDismissInterstitial += OnDidDismissInterstitial;
			Chartboost.onShowInterstitial += OnShowInterstitial;
		}
	}

	public override void ShowInterstitial()
	{
		Chartboost.ShowInterstitial("Main Menu");
	}

	public override void CacheNowInterstitials()
	{
		if (!_isCaching)
		{
			Chartboost.CacheInterstitial("Main Menu");
		}
	}

	private void OnDidClickInterstitial(string location)
	{
		if (location.Equals("Main Menu"))
		{
			_isCaching = false;
			OnDidClickInterstitial(this);
		}
	}

	private void OnDidCacheInterstitial(string location)
	{
		if (location.Equals("Main Menu"))
		{
			_isCaching = false;
			OnDidCacheInterstitial(this);
		}
	}

	private void OnDidFailToLoadInterstitial(string location)
	{
		if (location.Equals("Main Menu"))
		{
			_isCaching = false;
			OnDidFailToLoadInterstitial(this);
		}
	}

	private void OnDidDismissInterstitial(string location)
	{
		if (location.Equals("Main Menu"))
		{
			_isCaching = false;
			OnDidDismissInterstitial(this);
		}
	}

	private void OnShowInterstitial(string location)
	{
		if (location.Equals("Main Menu"))
		{
			OnDidShowInterstitial(this);
		}
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, Object context = null)
	{
		Debug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("ENABLE_CHARTBOOST_LOGS")]
	private static void Log(string msg, Object context = null)
	{
		Debug.Log(msg, context);
	}
}
