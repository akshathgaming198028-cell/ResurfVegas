#define ENABLE_ERROR_LOGS
using System.Diagnostics;
using Kiloo.Plugins.MdotM.Internal;
using UnityEngine;

namespace Kiloo.Plugins.MdotM
{
	public static class MdotM
	{
		private static IMdotMPlatformBridge _platformBridge;

		public static event MdotMLocationCallback onCacheInterstitial;

		public static event MdotMLocationCallback onFailToLoadInterstitial;

		public static event MdotMLocationCallback onDismissInterstitial;

		public static event MdotMLocationCallback onCloseInterstitial;

		public static event MdotMLocationCallback onClickInterstitial;

		public static event MdotMLocationCallback onShowInterstitial;

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("MdotM: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("MdotM: " + msg, context);
		}

		[Conditional("ENABLE_MDOTM_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("MdotM: " + msg, context);
		}

		private static void TriggerEvent(MdotMLocationCallback evt, string location)
		{
			if (evt != null)
			{
				evt(location);
			}
		}

		private static bool CheckSessionStarted()
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
			case RuntimePlatform.IPhonePlayer:
				return false;
			case RuntimePlatform.Android:
				return true;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				return false;
			default:
				return false;
			}
		}

		public static void StartSession(string appId)
		{
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
				_platformBridge = MdotMAndroid.CreatePlugin();
				break;
			default:
				LogError("There's no platform bridge for the current platform.");
				return;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.IPhonePlayer:
				break;
			}
			if (_platformBridge != null)
			{
				_platformBridge.didCacheInterstitial += delegate(string obj)
				{
					TriggerEvent(MdotM.onCacheInterstitial, obj);
				};
				_platformBridge.didClickInterstitial += delegate(string obj)
				{
					TriggerEvent(MdotM.onClickInterstitial, obj);
				};
				_platformBridge.didCloseInterstitial += delegate(string obj)
				{
					TriggerEvent(MdotM.onCloseInterstitial, obj);
				};
				_platformBridge.didDismissInterstitial += delegate(string obj)
				{
					TriggerEvent(MdotM.onDismissInterstitial, obj);
				};
				_platformBridge.didFailToLoadInterstitial += delegate(string obj)
				{
					TriggerEvent(MdotM.onFailToLoadInterstitial, obj);
				};
				_platformBridge.didShowInterstitial += delegate(string obj)
				{
					TriggerEvent(MdotM.onShowInterstitial, obj);
				};
				_platformBridge.StartSession(appId);
			}
		}

		public static void CacheInterstitial()
		{
			if (CheckSessionStarted())
			{
				_platformBridge.CacheInterstitial();
			}
		}

		public static void ShowInterstitial()
		{
			if (CheckSessionStarted())
			{
				_platformBridge.ShowInterstitial();
			}
		}

		public static bool HasCachedInterstitial()
		{
			if (!CheckSessionStarted())
			{
				return false;
			}
			return _platformBridge.HasCachedInterstitial();
		}

		public static void SetShouldRequestInterstitial(bool val)
		{
			if (CheckSessionStarted())
			{
				_platformBridge.SetShouldRequestInterstitial(val);
			}
		}

		public static void SetShouldDisplayInterstitial(bool val)
		{
			if (CheckSessionStarted())
			{
				_platformBridge.SetShouldDisplayInterstitial(val);
			}
		}

		public static void SetShouldRequestInterstitialsInFirstSession(bool val)
		{
			if (CheckSessionStarted())
			{
				_platformBridge.SetShouldRequestInterstitialsInFirstSession(val);
			}
		}
	}
}
