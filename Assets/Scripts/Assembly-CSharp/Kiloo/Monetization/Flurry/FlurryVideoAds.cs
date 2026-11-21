#define ENABLE_ERROR_LOGS
using System.Diagnostics;
using Kiloo.Monetization.Flurry.Internal;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Monetization.Flurry
{
	public static class FlurryVideoAds
	{
		private static IFlurryVideoAds _platformBridge;

		private static bool _initialized;

		public static event FlurryAdsCallback onTakeoverWillDisplay;

		public static event FlurryAdsCallback onTakeoverWillClose;

		public static event FlurryAdsCallback onVideoDidFinish;

		public static event FlurryAdsCallback onVideoDidNotFinish;

		public static event FlurryAdsCallback onVideoAvailable;

		public static event FlurryAdsCallback onVideoUnavailable;

		private static bool IsInitialized()
		{
			if (_platformBridge != null && _initialized)
			{
				return true;
			}
			return false;
		}

		public static bool IsSupportedOnPlatform()
		{
			bool result = false;
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
			case RuntimePlatform.Android:
				result = true;
				break;
			}
			return result;
		}

		public static void Initialize(string adSpace)
		{
			if (_initialized || !IsSupportedOnPlatform())
			{
				return;
			}
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				_platformBridge = FlurryVideoAdsIos.CreatePlugin();
			}
			else
			{
				if (Application.platform != RuntimePlatform.Android)
				{
					LogError("There's no platform bridge for the current platform.");
					return;
				}
				_platformBridge = FlurryVideoAdsAndroid.CreatePlugin();
			}
			if (_platformBridge == null)
			{
				LogError("An error occured when creating Flurry bridge.");
				return;
			}
			_platformBridge.onAdOpenedEvent += FlurryVideoAds.onTakeoverWillDisplay;
			_platformBridge.onAdClosedEvent += FlurryVideoAds.onTakeoverWillClose;
			_platformBridge.onVideoCompletedEvent += FlurryVideoAds.onVideoDidFinish;
			_platformBridge.onRenderFailedEvent += FlurryVideoAds.onVideoDidNotFinish;
			_platformBridge.onSpaceDidReceiveAdEvent += FlurryVideoAds.onVideoAvailable;
			_platformBridge.onSpaceDidFailToReceiveAdEvent += FlurryVideoAds.onVideoUnavailable;
			_platformBridge.Initialize(adSpace);
			if (Globals.DEBUG_USE_TEST_AD_CONSTANTS)
			{
				_platformBridge.EnableTestAds();
			}
			_initialized = true;
		}

		public static void CacheAd()
		{
			if (IsInitialized())
			{
				_platformBridge.FetchAd();
			}
		}

		public static void ShowAd()
		{
			if (IsInitialized())
			{
				_platformBridge.DisplayAd();
			}
		}

		public static bool HasCachedAd()
		{
			if (IsInitialized())
			{
				return _platformBridge.IsReady();
			}
			return false;
		}

		public static void EnableDebugLogs()
		{
			if (IsInitialized())
			{
				_platformBridge.EnableDebugLogs();
			}
		}

		public static void EnableTestAds()
		{
			if (IsInitialized())
			{
				_platformBridge.EnableTestAds();
			}
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("FlurryVideoAds: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("FlurryVideoAds: " + msg, context);
		}

		[Conditional("ENABLE_FLURRYADS_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("FlurryVideoAds: " + msg, context);
		}
	}
}
