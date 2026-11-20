using System.Diagnostics;
using UnityEngine;

namespace Kiloo.Plugins.MdotM.Internal
{
	public class MdotMAndroid : MonoBehaviour, IMdotMPlatformBridge
	{
		private const string CALLBACK_RECEIVER_NAME = "MdotMBridgeReceiver";

		private AndroidJavaObject _androidBridge;

		public event MdotMLocationCallback didCacheInterstitial;

		public event MdotMLocationCallback didFailToLoadInterstitial;

		public event MdotMLocationCallback didDismissInterstitial;

		public event MdotMLocationCallback didCloseInterstitial;

		public event MdotMLocationCallback didClickInterstitial;

		public event MdotMLocationCallback didShowInterstitial;

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("MdotMAndroid: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("MdotMAndroid: " + msg, context);
		}

		[Conditional("ENABLE_MDOTM_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("MdotMAndroid: " + msg, context);
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

		public void StartSession(string appId)
		{
			if (_androidBridge == null)
			{
				_androidBridge = new AndroidJavaObject("com.kiloo.unityplugin.mdotm.MdotMBridgeAndroid");
			}
			_androidBridge.Set("debugLogsEnabled", false);
			_androidBridge.Call("startSession", appId);
		}

		public void CacheInterstitial()
		{
			_androidBridge.Call("cacheInterstitial");
		}

		public void ShowInterstitial()
		{
			_androidBridge.Call("showInterstitial");
		}

		public bool HasCachedInterstitial()
		{
			return _androidBridge.Call<bool>("hasCachedInterstitial", new object[0]);
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

		private void TriggerEvent(MdotMLocationCallback evt, string location)
		{
			if (evt != null)
			{
				evt(location);
			}
		}

		public static MdotMAndroid CreatePlugin()
		{
			GameObject gameObject = new GameObject("MdotMBridgeReceiver");
			Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<MdotMAndroid>();
		}
	}
}
