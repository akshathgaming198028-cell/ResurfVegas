using System.Diagnostics;
using Kiloo.Monetization.Inmobi.Internal;
using UnityEngine;

namespace Kiloo.Monetization.Inmobi
{
	public static class Inmobi
	{
		private static IInmobiPlatformBridge _platformBridge;

		public static event InmobiCallback onCacheAd;

		public static event InmobiCallback onFailToLoadAd;

		public static event InmobiCallback onDismissAd;

		public static event InmobiCallback willShowAd;

		public static event InmobiCallback onCompleteIncentive;

		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("[Inmobi] " + msg, context);
		}

		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("[Inmobi] " + msg, context);
		}

		[Conditional("INMOBI_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("[Inmobi] " + msg, context);
		}

		private static void TriggerEvent(InmobiCallback evt, string data)
		{
			if (evt != null)
			{
				evt(data);
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

		public static void Initialize(string appId)
		{
			if (IsInitialized())
			{
				return;
			}
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				_platformBridge = InmobiIOS.CreatePlugin();
				break;
			case RuntimePlatform.Android:
				_platformBridge = InmobiAndroid.CreatePlugin();
				break;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				_platformBridge = InmobiSimulated.CreatePlugin();
				break;
			default:
				LogError("There's no platform bridge for the current platform.");
				return;
			}
			if (_platformBridge != null)
			{
				_platformBridge.didCacheAd += delegate(string obj)
				{
					TriggerEvent(Inmobi.onCacheAd, obj);
				};
				_platformBridge.didDismissAd += delegate(string obj)
				{
					TriggerEvent(Inmobi.onDismissAd, obj);
				};
				_platformBridge.didFailToLoadAd += delegate(string obj)
				{
					TriggerEvent(Inmobi.onFailToLoadAd, obj);
				};
				_platformBridge.willShowAd += delegate(string obj)
				{
					TriggerEvent(Inmobi.willShowAd, obj);
				};
				_platformBridge.didCompleteIncentive += delegate(string obj)
				{
					TriggerEvent(Inmobi.onCompleteIncentive, obj);
				};
				_platformBridge.Init(appId);
			}
		}

		public static void CacheAd(string key, string appId = "")
		{
			if (IsInitialized())
			{
				_platformBridge.CacheAd(key, appId);
			}
		}

		public static void ShowAd(string key, string appId = "")
		{
			if (IsInitialized())
			{
				_platformBridge.ShowAd(key, appId);
			}
		}

		public static bool HasCachedAd(string key, string appId = "")
		{
			if (!IsInitialized())
			{
				return false;
			}
			return _platformBridge.HasCachedAd(key, appId);
		}
	}
}
