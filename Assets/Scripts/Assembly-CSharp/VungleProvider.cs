using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Kiloo.Common;
using Kiloo.Plugins.Vungle;
using Kiloo.Plugins.Vungle.Internal;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class VungleProvider : VideoAdProvider
{
	private const bool _isStopped = false;

	private const string PROVIDER_VUNGLE_PRIORITY = "vungle_video_priority";

	private const int VUNGLE_DEFAULT_PRIORITY = 5;

	private const int VUNGLE_DEFAULT_WEIGHT = 10;

	private bool _initialized;

	private bool _hasSubscribedToEvents;

	private IEnumerator _checkVideoCoroutine;

	private bool _isCheckAvailabilityCoroutineRunning;

	private static VungleProvider _instance;

	private int _priority;

	private int _weight;

	public static VungleProvider instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new VungleProvider();
			}
			return _instance;
		}
	}

	public static bool isVideoProviderSupported
	{
		get
		{
			return Vungle.IsSupportedOnPlatform() && !string.IsNullOrEmpty(AdConstants.instance.VungleAppId);
		}
	}

	public override bool isInitialized
	{
		get
		{
			return _initialized;
		}
	}

	public override bool IsStopped
	{
		get
		{
			return false;
		}
	}

	public override string id { get; set; }

	public override int priority
	{
		get
		{
			return _priority;
		}
		set
		{
			_priority = value;
		}
	}

	public override int weight
	{
		get
		{
			return _weight;
		}
		set
		{
			_weight = value;
		}
	}

	private VungleProvider()
	{
		VideoAdProvider.SetVideoProviderData("vungle_video_priority", 5, 10, ref _priority, ref _weight);
	}

	public override void Init()
	{
		if (!_hasSubscribedToEvents)
		{
			Vungle.onVideoViewFinished += OnVideoViewFinished;
			Vungle.onAdStart += OnAdStart;
			Vungle.onAdUnavailable += OnAdUnavailable;
			Vungle.onCachedAdAvailable += OnCachedAdAvailable;
			_hasSubscribedToEvents = true;
		}
		if (!_initialized && isVideoProviderSupported)
		{
			_initialized = Vungle.InitVungle(AdConstants.instance.VungleAppId);
		}
	}

	public override void StartCachingVideo()
	{
		if (!_initialized)
		{
			OnVideoCachingFailed();
		}
		else if (!_isCheckAvailabilityCoroutineRunning)
		{
			_checkVideoCoroutine = CheckVideoAvailability();
			CoroutineUtil.StartCoroutine(_checkVideoCoroutine);
		}
	}

	private IEnumerator CheckVideoAvailability()
	{
		_isCheckAvailabilityCoroutineRunning = true;
		if (ProviderHasAdAvailable())
		{
			OnVideoCachingSucceded();
			yield break;
		}
		for (int i = 0; i < 10; i++)
		{
			yield return CoroutineUtil.StartCoroutine(CoroutineUtil.WaitForRealSeconds(1f));
			if (ProviderHasAdAvailable())
			{
				OnVideoCachingSucceded();
				yield break;
			}
		}
		OnVideoCachingTimedOut();
	}

	public void OnVideoCachingSucceded()
	{
		StopCheckVideoAvailabilityCouroutine();
		InvokeHandlerIfNotNull(_onVideoAvailable, this);
	}

	public void OnVideoCachingFailed()
	{
		StopCheckVideoAvailabilityCouroutine();
		InvokeHandlerIfNotNull(_onVideoUnavailable, this, string.Empty);
	}

	private void OnVideoCachingTimedOut()
	{
		StopCheckVideoAvailabilityCouroutine();
		InvokeHandlerIfNotNull(_onCachingTimedOut, this);
	}

	private void StopCheckVideoAvailabilityCouroutine()
	{
		if (_isCheckAvailabilityCoroutineRunning)
		{
			CoroutineUtil.StopCoroutine(_checkVideoCoroutine);
			_isCheckAvailabilityCoroutineRunning = false;
		}
	}

	private bool ProviderHasAdAvailable()
	{
		return Vungle.IsCachedAdAvailable();
	}

	public override bool PlayVideoIfAvailable()
	{
		if (isInitialized && ProviderHasAdAvailable())
		{
			Dictionary<string, string> videoAdOptions = GetVideoAdOptions();
			Vungle.PlayAdWithOptions(videoAdOptions);
			return true;
		}
		return false;
	}

	private Dictionary<string, string> GetVideoAdOptions()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if (Application.platform == RuntimePlatform.Android)
		{
			dictionary.Add("setOrientation", VungleAndroid.AndroidOrientationOptions.matchVideo.ToString());
			dictionary.Add("setSoundEnabled", "true");
			dictionary.Add("setIncentivized", "true");
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			dictionary.Add("showClose", "NO");
			dictionary.Add("incentivized", "YES");
			dictionary.Add("orientations", string.Empty + 4);
		}
		return dictionary;
	}

	public override void StopProvider()
	{
	}

	public override void ResumeProvider()
	{
	}

	public override void OnPause()
	{
		Vungle.OnPause();
	}

	public override void OnResume()
	{
		Vungle.OnResume();
	}

	private void OnVideoViewFinished(string isCompletedView)
	{
		bool result = false;
		InvokeHandlerIfNotNull(_onTakeoverEnded, this);
		bool.TryParse(isCompletedView, out result);
		if (result)
		{
			InvokeHandlerIfNotNull(_onVideoWatched, this);
		}
	}

	private void OnAdStart()
	{
		InvokeHandlerIfNotNull(_onTakeoverBegan, this);
	}

	private void OnCachedAdAvailable()
	{
		OnVideoCachingSucceded();
		InvokeHandlerIfNotNull(_onVideoAvailable, this);
	}

	private void OnAdUnavailable(string result)
	{
		OnVideoCachingFailed();
		InvokeHandlerIfNotNull(_onVideoUnavailable, this, string.Empty);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, Object context = null)
	{
		Debug.LogError("VungleProvider: " + msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, Object context = null)
	{
		Debug.LogWarning("VungleProvider: " + msg, context);
	}

	[Conditional("ENABLE_VUNGLE_LOGS")]
	private static void Log(string msg, Object context = null)
	{
		Debug.Log("VungleProvider: " + msg, context);
	}
}
