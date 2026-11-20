using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Kiloo.Monetization.Inmobi.Internal
{
	public class InmobiIOS : MonoBehaviour, IInmobiPlatformBridge
	{
		private const string CALLBACK_RECEIVER_NAME = "InmobiBridgeReceiver";

		public event InmobiCallback didCacheAd;

		public event InmobiCallback didFailToLoadAd;

		public event InmobiCallback didDismissAd;

		public event InmobiCallback willShowAd;

		public event InmobiCallback didCompleteIncentive;

		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("[InmobiIOS] " + msg, context);
		}

		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("[InmobiIOS] " + msg, context);
		}

		[Conditional("INMOBI_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("[InmobiIOS] " + msg, context);
		}

		[DllImport("__Internal")]
		private static extern void _inmobiplugin_init(string appId);

		[DllImport("__Internal")]
		private static extern void _inmobiplugin_cacheAd(string key, string appId);

		[DllImport("__Internal")]
		private static extern void _inmobiplugin_showAd(string key, string appId);

		[DllImport("__Internal")]
		private static extern bool _inmobiplugin_hasCachedAd(string key, string appId);

		[DllImport("__Internal")]
		private static extern void _inmobiplugin_setDebugLogEnabled(bool val);

		public void Init(string appId)
		{
			_inmobiplugin_init(appId);
		}

		public void CacheAd(string key, string appId)
		{
			_inmobiplugin_cacheAd(key, appId);
		}

		public void ShowAd(string key, string appId)
		{
			_inmobiplugin_showAd(key, appId);
		}

		public bool HasCachedAd(string key, string appId)
		{
			return _inmobiplugin_hasCachedAd(key, appId);
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

		public static InmobiIOS CreatePlugin()
		{
			_inmobiplugin_setDebugLogEnabled(false);
			GameObject gameObject = new GameObject("InmobiBridgeReceiver");
			Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<InmobiIOS>();
		}

		private void TriggerEvent(InmobiCallback evt, string data)
		{
			if (evt != null)
			{
				evt(data);
			}
		}
	}
}
