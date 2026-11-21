#define ENABLE_ERROR_LOGS
using System.Collections;
using System.Diagnostics;
using Kiloo.Plugins.Vungle.Internal;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Plugins.Vungle
{
	public class Vungle
	{
		private static IVunglePlatformBridge _platformBridge;

		private static bool _initWasSuccessful;

		private static bool _hasSubscribedToEvents;

		public static event VungleVideoViewFinishedEvent onVideoViewFinished;

		public static event VungleAdStartEvent onAdStart;

		public static event VungleCachedAdAvailableEvent onCachedAdAvailable;

		public static event VungleAdUnavailableEvent onAdUnavailable;

		public static event VungleWillCloseProductSheet onWillCloseProductSheet;

		private static bool IsInitialized()
		{
			if (_platformBridge == null || !_initWasSuccessful)
			{
				LogError("Vungle Android is not initialized.");
				return false;
			}
			return true;
		}

		public static bool IsSupportedOnPlatform()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				return true;
			case RuntimePlatform.Android:
				return true;
			case RuntimePlatform.WindowsEditor:
				return false;
			case RuntimePlatform.OSXEditor:
				return false;
			default:
				return false;
			}
		}

		public static bool InitVungle(string appId)
		{
			if (_platformBridge == null)
			{
				switch (Application.platform)
				{
				case RuntimePlatform.IPhonePlayer:
					_platformBridge = VungleiOS.CreatePlugin();
					break;
				case RuntimePlatform.Android:
					_platformBridge = VungleAndroid.CreatePlugin();
					break;
				default:
					LogError("There's no platform bridge for the current platform: " + Application.platform);
					return false;
				}
			}
			if (_platformBridge != null && !_initWasSuccessful)
			{
				if (!_hasSubscribedToEvents)
				{
					_platformBridge.onVideoViewFinished += Vungle.onVideoViewFinished;
					_platformBridge.onAdStart += Vungle.onAdStart;
					_platformBridge.onCachedAdAvailable += Vungle.onCachedAdAvailable;
					_platformBridge.onAdUnavailable += Vungle.onAdUnavailable;
					_platformBridge.onWillCloseProductSheet += Vungle.onWillCloseProductSheet;
					_hasSubscribedToEvents = true;
				}
				_initWasSuccessful = _platformBridge.InitVungle(appId);
			}
			return _initWasSuccessful;
		}

		public static void PlayAd()
		{
			if (IsInitialized())
			{
				_platformBridge.PlayAd();
			}
		}

		public static void PlayAdWithOptions(IDictionary optionsData)
		{
			if (IsInitialized())
			{
				_platformBridge.PlayAdWithOptions(optionsData);
			}
		}

		public static bool IsCachedAdAvailable()
		{
			if (!IsInitialized())
			{
				return false;
			}
			return _platformBridge.IsCachedAdAvailable();
		}

		public static void OnPause()
		{
			if (IsInitialized())
			{
				_platformBridge.OnPause();
			}
		}

		public static void OnResume()
		{
			if (IsInitialized())
			{
				_platformBridge.OnResume();
			}
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("Vungle: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("Vungle: " + msg, context);
		}

		[Conditional("ENABLE_VUNGLE_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("Vungle: " + msg, context);
		}
	}
}
