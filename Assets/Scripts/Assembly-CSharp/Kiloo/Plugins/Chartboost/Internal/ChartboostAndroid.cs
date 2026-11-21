using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Plugins.Chartboost.Internal
{
	public class ChartboostAndroid : MonoBehaviour, IChartboostPlatformBridge
	{
		private const string CALLBACK_RECEIVER_NAME = "ChartboostBridgeReceiver";

		private AndroidJavaObject _androidBridge;

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
			Debug.LogError("ChartboostAndroid: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("ChartboostAndroid: " + msg, context);
		}

		[Conditional("ENABLE_CHARTBOOST_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("ChartboostAndroid: " + msg, context);
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

		public void StartSession(string appId, string appSignature)
		{
			if (_androidBridge == null)
			{
				_androidBridge = new AndroidJavaObject("com.kiloo.unityplugin.chartboost.ChartboostBridgeAndroid");
			}
			_androidBridge.Set("debugLogsEnabled", false);
			_androidBridge.Call("startSession", appId, appSignature, Application.unityVersion);
		}

		public void CacheInterstitial(string location)
		{
			_androidBridge.Call("cacheInterstitial", location);
		}

		public void ShowInterstitial(string location)
		{
			_androidBridge.Call("showInterstitial", location);
		}

		public bool HasCachedInterstitial(string location)
		{
			return _androidBridge.Call<bool>("hasCachedInterstitial", new object[1] { location });
		}

		public void SetShouldRequestInterstitial(bool val)
		{
			_androidBridge.Set("delegate_shouldRequestInterstitial", val);
		}

		public void SetShouldDisplayInterstitial(bool val)
		{
			_androidBridge.Set("delegate_shouldDisplayInterstitial", val);
		}

		public void SetShouldRequestInterstitialsInFirstSession(bool val)
		{
			_androidBridge.Set("delegate_shouldRequestInterstitialsInFirstSession", val);
		}

		public void CacheRewardedVideo(string location)
		{
			_androidBridge.Call("cacheRewardedVideo", location);
		}

		public void ShowRewardedVideo(string location)
		{
			_androidBridge.Call("showRewardedVideo", location);
		}

		public bool HasCachedRewardedVideo(string location)
		{
			return _androidBridge.Call<bool>("hasCachedRewardedVideo", new object[1] { location });
		}

		private void TriggerEvent(ChartboostLocationCallback evt, string location)
		{
			if (evt != null)
			{
				evt(location);
			}
		}

		public static ChartboostAndroid CreatePlugin()
		{
			GameObject gameObject = new GameObject("ChartboostBridgeReceiver");
			Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<ChartboostAndroid>();
		}
	}
}
