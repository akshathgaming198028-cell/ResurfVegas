using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Plugins.Chartboost.Internal
{
	public class ChartboostIOS : MonoBehaviour, IChartboostPlatformBridge
	{
		private const string CALLBACK_RECEIVER_NAME = "ChartboostBridgeReceiver";

		public event ChartboostLocationCallback didCacheInterstitial;

		public event ChartboostLocationCallback didFailToLoadInterstitial;

		public event ChartboostLocationCallback didDismissInterstitial;

		public event ChartboostLocationCallback didCloseInterstitial;

		public event ChartboostLocationCallback didClickInterstitial;

		public event ChartboostLocationCallback didShowInterstitial;

		public event ChartboostLocationCallback didCacheRewardedVideo;

		public event ChartboostLocationCallback didFailToLoadRewardedVideo;

		public event ChartboostLocationCallback didDismissRewardedVideo;

		public event ChartboostLocationCallback didCloseRewardedVideo;

		public event ChartboostLocationCallback didClickRewardedVideo;

		public event ChartboostLocationCallback didCompleteRewardedVideo;

		public event ChartboostLocationCallback didDisplayRewardedVideo;

		public event ChartboostLocationCallback willDisplayVideo;

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("ChartboostIOS: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("ChartboostIOS: " + msg, context);
		}

		[Conditional("ENABLE_CHARTBOOST_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("ChartboostIOS: " + msg, context);
		}

		[DllImport("__Internal")]
		private static extern void _chartboostplugin_startSession(string appId, string appSignature, string unityVersion);

		[DllImport("__Internal")]
		private static extern void _chartboostplugin_cacheInterstitial(string location);

		[DllImport("__Internal")]
		private static extern void _chartboostplugin_showInterstitial(string location);

		[DllImport("__Internal")]
		private static extern bool _chartboostplugin_hasCachedInterstitial(string location);

		[DllImport("__Internal")]
		private static extern void _chartboostplugin_setShouldRequestInterstitial(bool val);

		[DllImport("__Internal")]
		private static extern void _chartboostplugin_setShouldDisplayInterstitial(bool val);

		[DllImport("__Internal")]
		private static extern void _chartboostplugin_setShouldRequestInterstitialsInFirstSession(bool val);

		[DllImport("__Internal")]
		private static extern void _chartboostplugin_setDebugLogEnabled(bool val);

		[DllImport("__Internal")]
		private static extern void _chartboostplugin_cacheRewardedVideo(string location);

		[DllImport("__Internal")]
		private static extern void _chartboostplugin_showRewardedVideo(string location);

		[DllImport("__Internal")]
		private static extern bool _chartboostplugin_hasCachedRewardedVideo(string location);

		public void StartSession(string appId, string appSignature)
		{
			_chartboostplugin_startSession(appId, appSignature, Application.unityVersion);
		}

		public void CacheInterstitial(string location)
		{
			_chartboostplugin_cacheInterstitial(location);
		}

		public void ShowInterstitial(string location)
		{
			_chartboostplugin_showInterstitial(location);
		}

		public bool HasCachedInterstitial(string location)
		{
			return _chartboostplugin_hasCachedInterstitial(location);
		}

		public void SetShouldRequestInterstitial(bool val)
		{
			_chartboostplugin_setShouldRequestInterstitial(val);
		}

		public void SetShouldDisplayInterstitial(bool val)
		{
			_chartboostplugin_setShouldDisplayInterstitial(val);
		}

		public void SetShouldRequestInterstitialsInFirstSession(bool val)
		{
			_chartboostplugin_setShouldRequestInterstitialsInFirstSession(val);
		}

		public void CacheRewardedVideo(string location)
		{
			_chartboostplugin_cacheRewardedVideo(location);
		}

		public void ShowRewardedVideo(string location)
		{
			_chartboostplugin_showRewardedVideo(location);
		}

		public bool HasCachedRewardedVideo(string location)
		{
			return _chartboostplugin_hasCachedRewardedVideo(location);
		}

		private void callback_didCacheInterstitial(string msg)
		{
			TriggerEvent(this.didCacheInterstitial, msg);
		}

		private void callback_didFailToLoadInterstitial(string msg)
		{
			TriggerEvent(this.didFailToLoadInterstitial, msg);
		}

		private void callback_didDismissInterstitial(string msg)
		{
			TriggerEvent(this.didDismissInterstitial, msg);
		}

		private void callback_didCloseInterstitial(string msg)
		{
			TriggerEvent(this.didCloseInterstitial, msg);
		}

		private void callback_didClickInterstitial(string msg)
		{
			TriggerEvent(this.didClickInterstitial, msg);
		}

		private void callback_didShowInterstitial(string msg)
		{
			TriggerEvent(this.didShowInterstitial, msg);
		}

		private void callback_didDisplayRewardedVideo(string msg)
		{
			TriggerEvent(this.didDisplayRewardedVideo, msg);
		}

		private void callback_didCacheRewardedVideo(string msg)
		{
			TriggerEvent(this.didCacheRewardedVideo, msg);
		}

		private void callback_didFailToLoadRewardedVideo(string msg)
		{
			TriggerEvent(this.didFailToLoadRewardedVideo, msg);
		}

		private void callback_didDismissRewardedVideo(string msg)
		{
			TriggerEvent(this.didDismissRewardedVideo, msg);
		}

		private void callback_didCloseRewardedVideo(string msg)
		{
			TriggerEvent(this.didCloseRewardedVideo, msg);
		}

		private void callback_didClickRewardedVideo(string msg)
		{
			TriggerEvent(this.didClickRewardedVideo, msg);
		}

		private void callback_didCompleteRewardedVideo(string msg)
		{
			TriggerEvent(this.didCompleteRewardedVideo, msg);
		}

		private void callback_willDisplayVideo(string msg)
		{
			TriggerEvent(this.willDisplayVideo, msg);
		}

		public static ChartboostIOS CreatePlugin()
		{
			_chartboostplugin_setDebugLogEnabled(false);
			GameObject gameObject = new GameObject("ChartboostBridgeReceiver");
			Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<ChartboostIOS>();
		}

		private void TriggerEvent(ChartboostLocationCallback evt, string location)
		{
			if (evt != null)
			{
				evt(location);
			}
		}
	}
}
