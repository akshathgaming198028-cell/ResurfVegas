using System;
using System.Collections;
using System.Diagnostics;
using Kiloo.Common;
using Kiloo.Plugins.AdColony;
using UnityEngine;

public class AdColonyProvider : VideoAdProvider
{
	private const string PROVIDER_ADCOLONY_PRIORITY = "adcolony_video_priority";

	private static readonly int ADCOLONY_DEFAULT_PRIORITY = 1;

	private static readonly int ADCOLONY_DEFAULT_WEIGHT = 10;

	private IEnumerator _checkVideoCoroutine;

	private bool _isCheckAvailabilityCoroutineRunning;

	private static AdColonyProvider _instance;

	private bool _isInitialized;

	private float _cachingStartTime = float.MinValue;

	private float _appMinimizedTime;

	private float _appResumedTime;

	private float _amountOfTimePaused;

	private string _id;

	private int _priority;

	private int _weight;

	public static AdColonyProvider instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new AdColonyProvider();
			}
			return _instance;
		}
	}

	public static bool isVideoProviderSupported
	{
		get
		{
			bool flag = AdColony.IsSupportedOnPlatform();
			bool flag2 = !string.IsNullOrEmpty(AdConstants.instance.AdColonyAppId) && !string.IsNullOrEmpty(AdConstants.instance.AdColonyZoneId);
			bool flag3 = true;
			return flag && flag2 && flag3;
		}
	}

	public override bool isInitialized
	{
		get
		{
			return _isInitialized;
		}
	}

	public override bool IsStopped
	{
		get
		{
			return false;
		}
	}

	public override string id
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
		}
	}

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

	private AdColonyProvider()
	{
		VideoAdProvider.SetVideoProviderData("adcolony_video_priority", ADCOLONY_DEFAULT_PRIORITY, ADCOLONY_DEFAULT_WEIGHT, ref _priority, ref _weight);
	}

	public override void Init()
	{
		if (!_isInitialized)
		{
			AdColony.onAdAttemptFinished += OnVideoFinished;
			AdColony.onAdStarted += OnVideoStarted;
			AdColony.onV4VCReward += OnV4VCWatched;
			AdColony.Configure(AdConstants.instance.AdColonyAppId, new string[1] { AdConstants.instance.AdColonyZoneId });
			_isInitialized = true;
		}
	}

	public override void StartCachingVideo()
	{
		if (!_isCheckAvailabilityCoroutineRunning)
		{
			_checkVideoCoroutine = CheckVideoAvailability();
			CoroutineUtil.StartCoroutine(_checkVideoCoroutine);
		}
	}

	private IEnumerator CheckVideoAvailability()
	{
		InitiateCachingTimeLogging();
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

	public override bool PlayVideoIfAvailable()
	{
		if (AdColony.IsVirtualCurrencyAwardAvailable(AdConstants.instance.AdColonyZoneId))
		{
			AdColony.PlayVideoAdForZoneWithPopups(AdConstants.instance.AdColonyZoneId);
			return true;
		}
		return false;
	}

	public override void StopProvider()
	{
	}

	public override void ResumeProvider()
	{
	}

	public override void OnPause()
	{
		_appMinimizedTime = Time.realtimeSinceStartup;
	}

	public override void OnResume()
	{
		_appResumedTime = Time.realtimeSinceStartup;
		_amountOfTimePaused += _appResumedTime - _appMinimizedTime;
	}

	private bool ProviderHasAdAvailable()
	{
		return AdColony.IsVirtualCurrencyAwardAvailable(AdConstants.instance.AdColonyZoneId);
	}

	public void OnVideoCachingSucceded()
	{
		_isCheckAvailabilityCoroutineRunning = false;
		CalculateAndReportCachingTime("Success");
		InvokeHandlerIfNotNull(_onVideoAvailable, this);
	}

	public void OnVideoCachingFailed()
	{
		_isCheckAvailabilityCoroutineRunning = false;
		CalculateAndReportCachingTime("Failed");
		InvokeHandlerIfNotNull(_onVideoUnavailable, this, string.Empty);
	}

	private void OnVideoCachingTimedOut()
	{
		ResetCachingTimeLogging();
		_isCheckAvailabilityCoroutineRunning = false;
		InvokeHandlerIfNotNull(_onCachingTimedOut, this);
	}

	private void OnVideoStarted()
	{
		InvokeHandlerIfNotNull(_onTakeoverBegan, this);
	}

	private void OnVideoFinished()
	{
		InvokeHandlerIfNotNull(_onTakeoverEnded, this);
	}

	private void OnV4VCWatched(bool success)
	{
		if (success)
		{
			InvokeHandlerIfNotNull(_onVideoWatched, this);
		}
	}

	public void CalculateAndReportCachingTime(string argkeyState)
	{
		if (_cachingStartTime > float.MinValue)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			double num = Math.Round((realtimeSinceStartup - (_cachingStartTime + _amountOfTimePaused)) * 2f) / 2.0;
			Flurry.LogEventWithAParameter("VideoAds Caching Time(2)", instance.id + argkeyState + " Time", num.ToString());
		}
		ResetCachingTimeLogging();
	}

	private void InitiateCachingTimeLogging()
	{
		_cachingStartTime = Time.realtimeSinceStartup;
		_amountOfTimePaused = 0f;
	}

	private void ResetCachingTimeLogging()
	{
		_cachingStartTime = float.MinValue;
		_amountOfTimePaused = 0f;
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, UnityEngine.Object context = null)
	{
		Debug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, UnityEngine.Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("ENABLE_ADCOLONY_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}
}
