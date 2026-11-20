#define ENABLE_ERROR_LOGS
using System.Diagnostics;
using Kiloo.Plugins.Chartboost.Internal;
using UnityEngine;

namespace Kiloo.Plugins.Chartboost
{
	public static class Chartboost
	{
		private static IChartboostPlatformBridge _platformBridge;

		public static event ChartboostLocationCallback onCacheInterstitial;

		public static event ChartboostLocationCallback onFailToLoadInterstitial;

		public static event ChartboostLocationCallback onDismissInterstitial;

		public static event ChartboostLocationCallback onCloseInterstitial;

		public static event ChartboostLocationCallback onClickInterstitial;

		public static event ChartboostLocationCallback onShowInterstitial;

		public static event ChartboostLocationCallback onCacheRewardedVideo;

		public static event ChartboostLocationCallback onFailToLoadRewardedVideo;

		public static event ChartboostLocationCallback onDismissRewardedVideo;

		public static event ChartboostLocationCallback onCloseRewardedVideo;

		public static event ChartboostLocationCallback onClickRewardedVideo;

		public static event ChartboostLocationCallback onCompleteRewardedVideo;

		public static event ChartboostLocationCallback onDisplayRewardedVideo;

		public static event ChartboostLocationCallback onWillDisplayVideo;

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("Chartboost: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("Chartboost: " + msg, context);
		}

		[Conditional("ENABLE_CHARTBOOST_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("Chartboost: " + msg, context);
		}

		private static void TriggerEvent(ChartboostLocationCallback evt, string location)
		{
			if (evt != null)
			{
				evt(location);
			}
		}

		private static bool IsInitialized()
		{
			if (_platformBridge == null)
			{
				return false;
			}
			return true;
		}

		public static bool IsSupportedOnPlatform()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.IPhonePlayer:
			case RuntimePlatform.Android:
				return true;
			default:
				return false;
			}
		}

		public static void StartSession(string appId, string appSignature)
		{
			if (IsInitialized())
			{
				return;
			}
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				_platformBridge = ChartboostIOS.CreatePlugin();
				break;
			case RuntimePlatform.Android:
				_platformBridge = ChartboostAndroid.CreatePlugin();
				break;
			default:
				LogError("There's no platform bridge for the current platform.");
				return;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				break;
			}
			if (_platformBridge != null)
			{
				_platformBridge.didCacheInterstitial += delegate(string obj)
				{
					TriggerEvent(Chartboost.onCacheInterstitial, obj);
				};
				_platformBridge.didClickInterstitial += delegate(string obj)
				{
					TriggerEvent(Chartboost.onClickInterstitial, obj);
				};
				_platformBridge.didCloseInterstitial += delegate(string obj)
				{
					TriggerEvent(Chartboost.onCloseInterstitial, obj);
				};
				_platformBridge.didDismissInterstitial += delegate(string obj)
				{
					TriggerEvent(Chartboost.onDismissInterstitial, obj);
				};
				_platformBridge.didFailToLoadInterstitial += delegate(string obj)
				{
					TriggerEvent(Chartboost.onFailToLoadInterstitial, obj);
				};
				_platformBridge.didShowInterstitial += delegate(string obj)
				{
					TriggerEvent(Chartboost.onShowInterstitial, obj);
				};
				_platformBridge.didCacheRewardedVideo += delegate(string obj)
				{
					TriggerEvent(Chartboost.onCacheRewardedVideo, obj);
				};
				_platformBridge.didFailToLoadRewardedVideo += delegate(string obj)
				{
					TriggerEvent(Chartboost.onFailToLoadRewardedVideo, obj);
				};
				_platformBridge.didDismissRewardedVideo += delegate(string obj)
				{
					TriggerEvent(Chartboost.onDismissRewardedVideo, obj);
				};
				_platformBridge.didCloseRewardedVideo += delegate(string obj)
				{
					TriggerEvent(Chartboost.onCloseRewardedVideo, obj);
				};
				_platformBridge.didClickRewardedVideo += delegate(string obj)
				{
					TriggerEvent(Chartboost.onClickRewardedVideo, obj);
				};
				_platformBridge.didCompleteRewardedVideo += delegate(string obj)
				{
					TriggerEvent(Chartboost.onCompleteRewardedVideo, obj);
				};
				_platformBridge.didDisplayRewardedVideo += delegate(string obj)
				{
					TriggerEvent(Chartboost.onDisplayRewardedVideo, obj);
				};
				_platformBridge.willDisplayVideo += delegate(string obj)
				{
					TriggerEvent(Chartboost.onWillDisplayVideo, obj);
				};
				_platformBridge.StartSession(appId, appSignature);
			}
		}

		public static void CacheInterstitial(string location)
		{
			if (IsInitialized())
			{
				_platformBridge.CacheInterstitial(location);
			}
		}

		public static void ShowInterstitial(string location)
		{
			if (IsInitialized())
			{
				_platformBridge.ShowInterstitial(location);
			}
		}

		public static bool HasCachedInterstitial(string location)
		{
			if (!IsInitialized())
			{
				return false;
			}
			return _platformBridge.HasCachedInterstitial(location);
		}

		public static void SetShouldRequestInterstitial(bool val)
		{
			if (IsInitialized())
			{
				_platformBridge.SetShouldRequestInterstitial(val);
			}
		}

		public static void SetShouldDisplayInterstitial(bool val)
		{
			if (IsInitialized())
			{
				_platformBridge.SetShouldDisplayInterstitial(val);
			}
		}

		public static void SetShouldRequestInterstitialsInFirstSession(bool val)
		{
			if (IsInitialized())
			{
				_platformBridge.SetShouldRequestInterstitialsInFirstSession(val);
			}
		}

		public static void CacheRewardedVideo(string location)
		{
			if (IsInitialized())
			{
				_platformBridge.CacheRewardedVideo(location);
			}
		}

		public static void ShowRewardedVideo(string location)
		{
			if (IsInitialized())
			{
				_platformBridge.ShowRewardedVideo(location);
			}
		}

		public static bool HasCachedRewardedVideo(string location)
		{
			if (!IsInitialized())
			{
				return false;
			}
			return _platformBridge.HasCachedRewardedVideo(location);
		}
	}
}
