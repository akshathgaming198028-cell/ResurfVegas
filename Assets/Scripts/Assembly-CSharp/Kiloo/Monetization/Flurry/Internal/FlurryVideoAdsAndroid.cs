#define ENABLE_ERROR_LOGS
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Monetization.Flurry.Internal
{
	public class FlurryVideoAdsAndroid : MonoBehaviour, IFlurryVideoAds
	{
		private const string CALLBACK_RECEIVER_NAME = "FlurryAdsBridgeReceiver";

		private const string NATIVE_JAVA_CLASS = "com.kiloo.flurryads.FlurryAdsBridge";

		private const string NATIVE_INITIALIZE = "initialize";

		private const string NATIVE_FETCH = "fetchAd";

		private const string NATIVE_READY = "isReady";

		private const string NATIVE_DISPLAY = "displayAd";

		private const string NATIVE_ENABLETESTADS = "setEnableTestAds";

		private const string NATIVE_ENABLEDEBUG = "setDebugLogsEnabled";

		private static AndroidJavaObject _plugin;

		private static bool _initialized;

		private static GameObject _callbackReceiver;

		public event FlurryAdsCallback onAdClosedEvent;

		public event FlurryAdsCallback onApplicationExitEvent;

		public event FlurryAdsCallback onRenderFailedEvent;

		public event FlurryAdsCallback onSpaceDidFailToReceiveAdEvent;

		public event FlurryAdsCallback onSpaceDidReceiveAdEvent;

		public event FlurryAdsCallback onAdClickedEvent;

		public event FlurryAdsCallback onAdOpenedEvent;

		public event FlurryAdsCallback onVideoCompletedEvent;

		[Conditional("ENABLE_ERROR_LOGS")]
		private void LogError(string msg, Object context = null)
		{
			Debug.LogError("FlurryVideoAdsAndroid: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("FlurryVideoAdsAndroid: " + msg, context);
		}

		[Conditional("ENABLE_FLURRYADS_LOGS")]
		private void Log(string msg, Object context = null)
		{
			Debug.Log("FlurryVideoAdsAndroid: " + msg, context);
		}

		public static FlurryVideoAdsAndroid CreatePlugin()
		{
			if (Application.platform != RuntimePlatform.Android || _callbackReceiver != null)
			{
				return null;
			}
			if (_plugin == null)
			{
				_plugin = new AndroidJavaObject("com.kiloo.flurryads.FlurryAdsBridge");
			}
			_callbackReceiver = new GameObject("FlurryAdsBridgeReceiver");
			Object.DontDestroyOnLoad(_callbackReceiver);
			return _callbackReceiver.AddComponent<FlurryVideoAdsAndroid>();
		}

		public void Initialize(string adSpace)
		{
			if (Application.platform == RuntimePlatform.Android && !(_callbackReceiver == null))
			{
				native_Initialize(adSpace);
				_initialized = true;
			}
		}

		public void FetchAd()
		{
			if (_initialized && !IsReady())
			{
				native_FetchAd();
			}
			else
			{
				LogError("ERROR: Calling FetchAd before initializing");
			}
		}

		public bool IsReady()
		{
			if (_initialized)
			{
				return native_Ready();
			}
			LogError("ERROR: Calling IsReady before initializing");
			return false;
		}

		public void DisplayAd()
		{
			if (_initialized && IsReady())
			{
				native_DisplayAd();
			}
			else
			{
				LogError("ERROR: Calling DisplayAd before initializing");
			}
		}

		public void EnableTestAds()
		{
			native_EnableTestAds(true);
		}

		public void EnableDebugLogs()
		{
			native_enableDebugLogs(true);
		}

		private void InvokeHandlerIfNotNull(FlurryAdsCallback handler, string message)
		{
			if (handler != null)
			{
				handler(message);
			}
		}

		public void onAdClicked(string adSpace)
		{
			InvokeHandlerIfNotNull(this.onAdClickedEvent, adSpace);
		}

		public void onClose(string adSpace)
		{
			InvokeHandlerIfNotNull(this.onAdClosedEvent, adSpace);
		}

		public void onDisplay(string adSpace)
		{
			InvokeHandlerIfNotNull(this.onAdOpenedEvent, adSpace);
		}

		public void onAppExit(string adSpace)
		{
			InvokeHandlerIfNotNull(this.onApplicationExitEvent, adSpace);
		}

		public void onVideoCompleted(string adSpace)
		{
			InvokeHandlerIfNotNull(this.onVideoCompletedEvent, adSpace);
		}

		public void onFetched(string adSpace)
		{
			InvokeHandlerIfNotNull(this.onSpaceDidReceiveAdEvent, adSpace);
		}

		public void onError(string error)
		{
			error = error.ToLower();
			if (error.Contains("render"))
			{
				InvokeHandlerIfNotNull(this.onRenderFailedEvent, error);
			}
			else if (error.Contains("fetch"))
			{
				InvokeHandlerIfNotNull(this.onSpaceDidFailToReceiveAdEvent, error);
			}
		}

		private void native_Initialize(string adSpace)
		{
			_plugin.Call("initialize", adSpace);
		}

		private void native_FetchAd()
		{
			_plugin.Call("fetchAd");
		}

		private bool native_Ready()
		{
			return _plugin.Call<bool>("isReady", new object[0]);
		}

		private void native_DisplayAd()
		{
			_plugin.Call("displayAd");
		}

		private void native_EnableTestAds(bool enable)
		{
			_plugin.Call("setEnableTestAds", enable);
		}

		private void native_enableDebugLogs(bool enable)
		{
			_plugin.Call("setDebugLogsEnabled", enable);
		}
	}
}
