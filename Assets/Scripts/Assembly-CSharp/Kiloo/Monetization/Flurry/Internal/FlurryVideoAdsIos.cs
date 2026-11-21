#define ENABLE_ERROR_LOGS
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Monetization.Flurry.Internal
{
	public class FlurryVideoAdsIos : MonoBehaviour, IFlurryVideoAds
	{
		private const string BRIDGE_DELEGATE_GAMEOBJECT_NAME = "FlurryAdsBridgeReceiver";

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
			Debug.LogError("FlurryVideoAdsIOS: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("FlurryVideoAdsIOS: " + msg, context);
		}

		[Conditional("ENABLE_FLURRYADS_LOGS")]
		private void Log(string msg, Object context = null)
		{
			Debug.Log("FlurryVideoAdsIOS: " + msg, context);
		}

		public static FlurryVideoAdsIos CreatePlugin()
		{
			if (Application.platform != RuntimePlatform.IPhonePlayer || _callbackReceiver != null)
			{
				return null;
			}
			_callbackReceiver = new GameObject("FlurryAdsBridgeReceiver");
			Object.DontDestroyOnLoad(_callbackReceiver);
			return _callbackReceiver.AddComponent<FlurryVideoAdsIos>();
		}

		public void Initialize(string adSpace)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer && !(_callbackReceiver == null))
			{
				fcbridge_initWithSpace(adSpace);
				_initialized = true;
			}
		}

		public void FetchAd()
		{
			if (_initialized && !IsReady())
			{
				fcbridge_fetchAd();
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
				return Ready();
			}
			LogError("ERROR: Calling IsReady before initializing");
			return false;
		}

		public void DisplayAd()
		{
			if (_initialized && IsReady())
			{
				PresentWithViewController();
			}
			else
			{
				LogError("ERROR: Calling DisplayAd before initializing");
			}
		}

		public void EnableDebugLogs()
		{
			fcbridge_enableDebugLogs(true);
		}

		public void EnableTestAds()
		{
			fcbridge_enableTestAds(true);
		}

		private void InvokeHandlerIfNotNull(FlurryAdsCallback handler, string message)
		{
			if (handler != null)
			{
				handler(message);
			}
		}

		private bool Ready()
		{
			return fcbridge_ready();
		}

		private void PresentWithViewController()
		{
			fcbridge_presentWithViewController();
		}

		private void fcbridge_didFetchAd(string adSpace)
		{
			InvokeHandlerIfNotNull(this.onSpaceDidReceiveAdEvent, adSpace);
		}

		private void fcbridge_willPresent(string adSpace)
		{
			InvokeHandlerIfNotNull(this.onAdOpenedEvent, adSpace);
		}

		private void fcbridge_didRender(string adSpace)
		{
		}

		private void fcbridge_willDismiss(string adSpace)
		{
			InvokeHandlerIfNotNull(this.onAdClosedEvent, adSpace);
		}

		private void fcbridge_didDismiss(string adSpace)
		{
		}

		private void fcbridge_didRecieveClick(string adSpace)
		{
		}

		private void fcbridge_willLeaveApplication(string adSpace)
		{
		}

		private void fcbridge_videoDidFinish(string adSpace)
		{
			InvokeHandlerIfNotNull(this.onVideoCompletedEvent, adSpace);
		}

		private void fcbridge_adError(string error)
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

		[DllImport("__Internal")]
		private static extern void fcbridge_initWithSpace(string adSpace);

		[DllImport("__Internal")]
		private static extern void fcbridge_fetchAd();

		[DllImport("__Internal")]
		private static extern bool fcbridge_ready();

		[DllImport("__Internal")]
		private static extern void fcbridge_presentWithViewController();

		[DllImport("__Internal")]
		private static extern void fcbridge_enableTestAds(bool enable);

		[DllImport("__Internal")]
		private static extern void fcbridge_enableDebugLogs(bool enable);
	}
}
