using System.Diagnostics;
using UnityEngine;

namespace Kiloo.Monetization.Inmobi.Internal
{
	public class InmobiAndroid : MonoBehaviour, IInmobiPlatformBridge
	{
		private const string CALLBACK_RECEIVER_NAME = "InmobiBridgeReceiver";

		private AndroidJavaObject _androidBridge;

		public event InmobiCallback didCacheAd;

		public event InmobiCallback didFailToLoadAd;

		public event InmobiCallback didDismissAd;

		public event InmobiCallback willShowAd;

		public event InmobiCallback didCompleteIncentive;

		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("[InmobiAndroid] " + msg, context);
		}

		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("[InmobiAndroid] " + msg, context);
		}

		[Conditional("INMOBI_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("[InmobiAndroid] " + msg, context);
		}

		private void callback_didCacheAd(string msg)
		{
			TriggerEvent(this.didCacheAd, msg);
		}

		private void callback_didFailToLoadAd(string msg)
		{
			TriggerEvent(this.didFailToLoadAd, msg);
		}

		private void callback_didDismissAd(string msg)
		{
			TriggerEvent(this.didDismissAd, msg);
		}

		private void callback_willShowAd(string msg)
		{
			TriggerEvent(this.willShowAd, msg);
		}

		private void callback_didCompleteIncentive(string msg)
		{
			TriggerEvent(this.didCompleteIncentive, msg);
		}

		public void Init(string appId)
		{
			if (_androidBridge == null)
			{
				_androidBridge = new AndroidJavaObject("com.kiloo.inmobi.InmobiBridgeAndroid");
			}
			_androidBridge.Call("setDebugLogsEnabled", false);
			_androidBridge.Call("init", appId);
		}

		public void CacheAd(string key, string appId)
		{
			_androidBridge.Call("cacheAd", key, appId);
		}

		public void ShowAd(string key, string appId)
		{
			_androidBridge.Call("showAd", key, appId);
		}

		public bool HasCachedAd(string key, string appId)
		{
			return _androidBridge.Call<bool>("hasCachedAd", new object[2] { key, appId });
		}

		public static InmobiAndroid CreatePlugin()
		{
			GameObject gameObject = new GameObject("InmobiBridgeReceiver");
			Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<InmobiAndroid>();
		}

		private void TriggerEvent(InmobiCallback evt, string location)
		{
			if (evt != null)
			{
				evt(location);
			}
		}
	}
}
