using System.Diagnostics;
using Kiloo.Monetization.Inmobi;
using UnityEngine;

public class InmobiAdsProvider : ScreenAdProvider
{
	private const string KEY_INTERSTITIAL = "interstitial";

	private ScreenAdProvider _inmobiProvider;

	private static InmobiAdsProvider _instance;

	private bool _isCaching;

	private bool _isPluginInitialized;

	public static InmobiAdsProvider instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new InmobiAdsProvider();
			}
			return _instance;
		}
	}

	public override bool isInitialized
	{
		get
		{
			return _isPluginInitialized;
		}
	}

	public override bool isScreenAdProviderSupported
	{
		get
		{
			return Inmobi.IsSupportedOnPlatform() && !string.IsNullOrEmpty(AdConstants.instance.InmobiAppIdInterstitial);
		}
	}

	public override bool HasCachedInterstials
	{
		get
		{
			return Inmobi.HasCachedAd("interstitial", AdConstants.instance.InmobiAppIdInterstitial);
		}
	}

	public override string id
	{
		get
		{
			return "inmobi";
		}
	}

	private InmobiAdsProvider()
	{
	}

	public override void Init()
	{
		if (!_isPluginInitialized && isScreenAdProviderSupported)
		{
			_isPluginInitialized = true;
			Inmobi.Initialize(AdConstants.instance.InmobiAppIdInterstitial);
			Inmobi.onCacheAd += OnDidCacheInterstitial;
			Inmobi.onFailToLoadAd += OnDidFailToLoadInterstitial;
			Inmobi.onDismissAd += OnDidDismissScreen;
			Inmobi.willShowAd += OnShowInterstitial;
		}
	}

	public override void CacheNowInterstitials()
	{
		if (!_isCaching)
		{
			Inmobi.CacheAd("interstitial", AdConstants.instance.InmobiAppIdInterstitial);
		}
	}

	public override void ShowInterstitial()
	{
		Inmobi.ShowAd("interstitial", AdConstants.instance.InmobiAppIdInterstitial);
	}

	private void OnDidCacheInterstitial(string key)
	{
		if (key.Equals("interstitial"))
		{
			_isCaching = false;
			OnDidCacheInterstitial(this);
		}
	}

	private void OnDidFailToLoadInterstitial(string key)
	{
		if (key.Equals("interstitial"))
		{
			_isCaching = false;
			OnDidFailToLoadInterstitial(this);
		}
	}

	private void OnDidDismissScreen(string key)
	{
		if (key.Equals("interstitial"))
		{
			_isCaching = false;
			OnDidDismissInterstitial(this);
		}
	}

	private void OnDidClickInterstitial(string key)
	{
		if (key.Equals("interstitial"))
		{
			_isCaching = false;
			OnDidClickInterstitial(this);
		}
	}

	private void OnShowInterstitial(string key)
	{
		if (key.Equals("interstitial"))
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

	[Conditional("ENABLE_INMOBI_LOGS")]
	private static void Log(string msg, Object context = null)
	{
		Debug.Log(msg, context);
	}
}
