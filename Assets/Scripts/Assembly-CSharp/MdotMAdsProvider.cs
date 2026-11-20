using System.Diagnostics;
using Kiloo.Plugins.MdotM;
using UnityEngine;

public class MdotMAdsProvider : ScreenAdProvider
{
	private ScreenAdProvider _mdotmProvider;

	private static MdotMAdsProvider _instance;

	private bool _isCaching;

	private bool _isPluginInitialized;

	public override bool isInitialized
	{
		get
		{
			return _isPluginInitialized;
		}
	}

	public static MdotMAdsProvider instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new MdotMAdsProvider();
			}
			return _instance;
		}
	}

	public override bool isScreenAdProviderSupported
	{
		get
		{
			return MdotM.IsSupportedOnPlatform() && !string.IsNullOrEmpty(AdConstants.instance.MdotmAppId);
		}
	}

	public override bool HasCachedInterstials
	{
		get
		{
			return MdotM.HasCachedInterstitial();
		}
	}

	public override string id
	{
		get
		{
			return "mdotm";
		}
	}

	private MdotMAdsProvider()
	{
	}

	public override void Init()
	{
		if (!_isPluginInitialized && isScreenAdProviderSupported)
		{
			_isPluginInitialized = true;
			MdotM.StartSession(AdConstants.instance.MdotmAppId);
			MdotM.onClickInterstitial += OnDidClickInterstitial;
			MdotM.onFailToLoadInterstitial += OnDidFailToLoadInterstitial;
			MdotM.onCacheInterstitial += OnDidCacheInterstitial;
			MdotM.onDismissInterstitial += OnDidDismissInterstitial;
			MdotM.onShowInterstitial += OnShowInterstitial;
		}
	}

	public override void ShowInterstitial()
	{
		MdotM.ShowInterstitial();
	}

	public override void CacheNowInterstitials()
	{
		if (!_isCaching)
		{
			MdotM.CacheInterstitial();
		}
	}

	private void OnDidClickInterstitial(string location)
	{
		_isCaching = false;
		OnDidClickInterstitial(this);
	}

	private void OnDidCacheInterstitial(string location)
	{
		_isCaching = false;
		OnDidCacheInterstitial(this);
	}

	private void OnDidFailToLoadInterstitial(string location)
	{
		_isCaching = false;
		OnDidFailToLoadInterstitial(this);
	}

	private void OnDidDismissInterstitial(string location)
	{
		_isCaching = false;
		OnDidDismissInterstitial(this);
	}

	private void OnShowInterstitial(string location)
	{
		OnDidShowInterstitial(this);
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

	[Conditional("ENABLE_MDOTM_LOGS")]
	private static void Log(string msg, Object context = null)
	{
		Debug.Log(msg, context);
	}
}
