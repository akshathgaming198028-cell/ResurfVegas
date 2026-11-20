#define ENABLE_ERROR_LOGS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Kiloo.Common;
using Kiloo.Plugins.DeviceUtility;
using UnityEngine;

public class VideoAdsManager
{
	public enum VideoState
	{
		Offline = 0,
		Downloading = 1,
		Ready = 2,
		NoVideos = 3
	}

	public enum State
	{
		Uninitialized = 0,
		Initialized = 1,
		TakeoverActive = 2
	}

	public delegate void OnVideoCacherDelegate();

	private const int MAX_PROVIDERS_TO_INIT_PER_CALL = 1;

	private const int NUMBER_OF_PROVIDERS_ALLOWERD_ON_LOW_END_DEVICES = 1;

	private const int DEFAULT_NUMBER_OF_KEYS_TO_REWARD_FOR_VIDEO_AD = 2;

	private const string ONLINESETTINGS_PROVIDERLIST_KEY = "videoads_providerlist";

	private const string ONLINE_SETTING_KEYS_REWARDED_FOR_VIDEO = "keys_rewarded_for_video_ad";

	private const string PROVIDER_ADCOLONY = "adcolony";

	private const string PROVIDER_FLURRYVIDEOADS = "flurryvideoads";

	private const string PROVIDER_VUNGLECLIPS = "vungleclips";

	private const string PROVIDER_TAPJOYVIDEOADS = "tapjoyvideoads";

	private const string PROVIDER_INMOBIVIDEOADS = "inmobivideoads";

	private const string PROVIDER_CHARTBOOSTVIDEOADS = "chartboostvideoads";

	private int _numberOfKeysToRewardForVideo = 2;

	private static readonly string[] DEFAULT_PROVIDER_LIST = new string[1] { "vungleclips" };

	private VideoAdProvider currentProvider;

	private VideoAdProvider lastShownProvider;

	private List<VideoAdProvider> currentListOfProviders;

	private int currentProviderIndex = -1;

	private VideoAdProvider _adColonyProvider;

	private VideoAdProvider _flurryVideoAdsProvider;

	private VideoAdProvider _vungleVideoAdsProvider;

	private VideoAdProvider _tapjoyVideoAdProvider;

	private VideoAdProvider _inmobiVideoAdsProvider;

	private VideoAdProvider _chartboostVideoAdProvider;

	private bool _isVideoPlaying;

	private Action _onVideoStateChangedHandler;

	private Action _onKeyVideoWatchedChangedHandler;

	private Action _onVideoProviderChangedHandler;

	private static VideoAdsManager _instance;

	private Action _rewardOverride;

	private bool _gameSoundOnBeforeMute;

	public VideoState currentVideoState { get; private set; }

	public State state { get; private set; }

	public bool isVideoPlaying
	{
		get
		{
			return _isVideoPlaying;
		}
	}

	public static VideoAdsManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new VideoAdsManager();
			}
			return _instance;
		}
	}

	public bool isInitialized
	{
		get
		{
			return state != State.Uninitialized;
		}
	}

	public bool IsStopped
	{
		get
		{
			return false;
		}
	}

	public event OnVideoCacherDelegate OnVideoCached;

	public event Action<VideoAdProvider> OnTakeoverBegan;

	public event Action<VideoAdProvider> OnTakeoverEnded;

	private VideoAdsManager()
	{
		currentVideoState = VideoState.Offline;
		state = State.Uninitialized;
	}

	public VideoAdProvider GetCurrentProvider()
	{
		return currentProvider;
	}

	public VideoAdProvider GetLastShownProvider()
	{
		return lastShownProvider;
	}

	public void AddOnVideoStateChangedHandler(Action handler)
	{
		_onVideoStateChangedHandler = (Action)Delegate.Combine(_onVideoStateChangedHandler, handler);
	}

	public void RemoveOnVideoStateChangedHandler(Action handler)
	{
		_onVideoStateChangedHandler = (Action)Delegate.Remove(_onVideoStateChangedHandler, handler);
	}

	public void AddOnKeyVideoWatchedChangedHandler(Action handler)
	{
		_onKeyVideoWatchedChangedHandler = (Action)Delegate.Combine(_onKeyVideoWatchedChangedHandler, handler);
	}

	public void RemoveOnKeyVideoWatchedChangedHandler(Action handler)
	{
		_onKeyVideoWatchedChangedHandler = (Action)Delegate.Remove(_onKeyVideoWatchedChangedHandler, handler);
	}

	public void AddOnVideoProviderChangedHandler(Action handler)
	{
		_onVideoProviderChangedHandler = (Action)Delegate.Combine(_onVideoProviderChangedHandler, handler);
	}

	public void RemoveOnVideoProviderChangedHandler(Action handler)
	{
		_onVideoProviderChangedHandler = (Action)Delegate.Remove(_onVideoProviderChangedHandler, handler);
	}

	public void Init()
	{
		if (state == State.Uninitialized)
		{
			RefreshListProviders();
			state = State.Initialized;
		}
		else
		{
			LogWarning("VideoAdsManager: Already initialized");
		}
	}

	public void StartCachingVideo()
	{
		if (state != State.Uninitialized)
		{
			ResumeProvider();
			if (currentProvider != null && currentVideoState != VideoState.Downloading)
			{
				currentVideoState = VideoState.Downloading;
				if (_onVideoStateChangedHandler != null)
				{
					_onVideoStateChangedHandler();
				}
				currentProvider.StartCachingVideo();
			}
		}
		else
		{
			LogWarning("VideoAdsManager: Init first!");
		}
	}

	private void OnVideoCachingSucceded(VideoAdProvider provider)
	{
		Flurry.LogEventWithAParameter("VideoAds Cache Request Ended", provider.id + " Result", "Success");
		string contryCodeFromLocale = Utils.GetContryCodeFromLocale(DeviceUtility.GetLocale());
		Flurry.LogEventWithAParameter("VideoAds Cache Request Ended (Country Info)", provider.id + "Success Country", contryCodeFromLocale);
		if (currentProvider != null && provider.id == currentProvider.id)
		{
			currentVideoState = VideoState.Ready;
			if (_onVideoStateChangedHandler != null)
			{
				_onVideoStateChangedHandler();
			}
			if (this.OnVideoCached != null)
			{
				this.OnVideoCached();
			}
		}
	}

	private void OnVideoCachingFailed(VideoAdProvider provider, string extraInfo)
	{
		if (extraInfo == string.Empty)
		{
			Flurry.LogEventWithAParameter("VideoAds Cache Request Ended", provider.id + " Result", "Failed");
		}
		else
		{
			Flurry.LogEventWithSeveralParameters("VideoAds Cache Request Ended", provider.id + " Result;Error code", "Failed;" + extraInfo);
		}
		string contryCodeFromLocale = Utils.GetContryCodeFromLocale(DeviceUtility.GetLocale());
		Flurry.LogEventWithAParameter("VideoAds Cache Request Ended (Country Info)", provider.id + "Failed Country", contryCodeFromLocale);
		if (currentProvider != null && provider.id == currentProvider.id)
		{
			GoToNextProvider();
		}
	}

	private void OnVideoCachingTimedOut(VideoAdProvider provider)
	{
		Flurry.LogEventWithAParameter("VideoAds Cache Request Ended", provider.id + " Result", "Timedout");
		string contryCodeFromLocale = Utils.GetContryCodeFromLocale(DeviceUtility.GetLocale());
		Flurry.LogEventWithAParameter("VideoAds Cache Request Ended (Country Info)", provider.id + "Timedout Country", contryCodeFromLocale);
		if (currentProvider != null && provider.id == currentProvider.id)
		{
			GoToNextProvider();
		}
	}

	private void GoToNextProvider()
	{
		if (currentProvider != null)
		{
			currentProvider.StopProvider();
		}
		currentProvider = GetNextProvider();
		if (_onVideoProviderChangedHandler != null)
		{
			_onVideoProviderChangedHandler();
		}
		if (currentProvider != null)
		{
			if (!currentProvider.isInitialized)
			{
				currentProvider.Init();
			}
			if (currentProvider.IsStopped)
			{
				currentProvider.ResumeProvider();
			}
			currentProvider.StartCachingVideo();
		}
		else
		{
			currentVideoState = VideoState.NoVideos;
			if (_onVideoStateChangedHandler != null)
			{
				_onVideoStateChangedHandler();
			}
		}
	}

	private void RefreshListProviders()
	{
		currentProviderIndex = -1;
		currentListOfProviders = GetSortedListOfProviders(GetProviderList());
		if (Application.platform == RuntimePlatform.IPhonePlayer && DeviceInfo.Instance.performanceLevel == DeviceInfo.PerformanceLevel.Low && currentListOfProviders.Count > 1)
		{
			currentListOfProviders.RemoveRange(1, currentListOfProviders.Count - 1);
		}
		currentProvider = GetNextProvider();
		if (_onVideoProviderChangedHandler != null)
		{
			_onVideoProviderChangedHandler();
		}
		if (currentProvider != null && !currentProvider.isInitialized)
		{
			currentProvider.Init();
		}
	}

	public void TryCachingVideosAgain()
	{
		ResetCurrentVideoState();
		RefreshListProviders();
		StartCachingVideo();
	}

	public bool PlayVideoIfAvailable(Action rewardOverride = null)
	{
		_rewardOverride = rewardOverride;
		if (Globals.DEBUG_SKIP_VIDEO_ADS)
		{
			CoroutineUtil.StartCoroutine(FakeOnProviderVideoWatched());
			return true;
		}
		if (state == State.Initialized && currentVideoState == VideoState.Ready)
		{
			if (currentProvider.isInitialized)
			{
				if (currentProvider.PlayVideoIfAvailable())
				{
					lastShownProvider = currentProvider;
					_isVideoPlaying = true;
					Flurry.LogEventWithAParameter("VideoAds attempt to play", "Result", currentProvider.id);
					Flurry.LogEventWithAParameter(string.Format("VideoAds {0} request", currentProvider.id), "Result", "Ok");
					return true;
				}
				Flurry.LogEventWithAParameter(string.Format("VideoAds {0} request", currentProvider.id), "Result", "No video");
				currentVideoState = VideoState.NoVideos;
				if (_onVideoStateChangedHandler != null)
				{
					_onVideoStateChangedHandler();
				}
			}
		}
		else
		{
			LogWarning(string.Concat("VideoAdsManager: Not ready to play (state is ", state, ") and CurrentVideoState is ", currentVideoState));
			if (state == State.TakeoverActive)
			{
				state = State.Initialized;
			}
		}
		Flurry.LogEventWithAParameter("VideoAds attempt to play", "Result", "No video");
		return false;
	}

	private IEnumerator FakeOnProviderVideoWatched()
	{
		yield return null;
		OnProviderVideoWatched(null);
	}

	public void StopProvider()
	{
		if (currentProvider != null && currentProvider.isInitialized)
		{
			currentProvider.StopProvider();
		}
	}

	public void ResumeProvider()
	{
		if (currentProvider != null && currentProvider.isInitialized && currentProvider.IsStopped)
		{
			currentProvider.ResumeProvider();
		}
	}

	private VideoAdProvider GetNextProvider()
	{
		if (currentListOfProviders != null)
		{
			if (currentListOfProviders.Count > currentProviderIndex + 1)
			{
				currentProviderIndex++;
				return currentListOfProviders[currentProviderIndex];
			}
			Flurry.LogEventWithAParameter("VideoAds end of waterfall", "Country", Utils.GetContryCodeFromLocale(DeviceUtility.GetLocale()));
			currentProviderIndex = -1;
			return null;
		}
		return null;
	}

	private List<VideoAdProvider> GetSortedListOfProviders(string[] providerIds)
	{
		List<VideoAdProvider> list = new List<VideoAdProvider>();
		List<VideoAdProvider> list2 = new List<VideoAdProvider>();
		for (int i = 0; i < providerIds.Length; i++)
		{
			VideoAdProvider provider = GetProvider(providerIds[i]);
			if (provider != null)
			{
				list.Add(provider);
			}
		}
		return SortListUsingTheWeightSystem(list);
	}

	private List<VideoAdProvider> SortListUsingTheWeightSystem(List<VideoAdProvider> sourceList)
	{
		List<VideoAdProvider> list = new List<VideoAdProvider>();
		List<List<VideoAdProvider>> list2 = new List<List<VideoAdProvider>>();
		if (sourceList != null && sourceList.Count != 0)
		{
			for (int i = 0; i < sourceList.Count; i++)
			{
				VideoAdProvider videoAdProvider = sourceList[i];
				for (int j = 0; j < videoAdProvider.priority; j++)
				{
					if (j > list2.Count - 1)
					{
						list2.Add(new List<VideoAdProvider>());
					}
					if (j == videoAdProvider.priority - 1)
					{
						list2[j].Add(videoAdProvider);
					}
				}
			}
			for (int k = 0; k < list2.Count; k++)
			{
				List<VideoAdProvider> list3 = OrderListOfSamePriorityByWeight(list2[k]);
				for (int l = 0; l < list3.Count; l++)
				{
					list.Add(list3[l]);
				}
			}
		}
		return list;
	}

	private List<VideoAdProvider> OrderListOfSamePriorityByWeight(List<VideoAdProvider> sourceList)
	{
		List<VideoAdProvider> list = new List<VideoAdProvider>();
		while (sourceList.Count >= 2)
		{
			int num = 0;
			foreach (VideoAdProvider source in sourceList)
			{
				num += source.weight;
			}
			int num2 = UnityEngine.Random.Range(0, num) + 1;
			int num3 = 0;
			VideoAdProvider item = sourceList[0];
			for (int i = 0; i < sourceList.Count; i++)
			{
				num3 += sourceList[i].weight;
				if (num2 <= num3)
				{
					item = sourceList[i];
					break;
				}
			}
			list.Add(item);
			sourceList.Remove(item);
		}
		foreach (VideoAdProvider source2 in sourceList)
		{
			list.Add(source2);
		}
		return list;
	}

	private string[] GetProviderList()
	{
		string[] array = DEFAULT_PROVIDER_LIST;
		string valueString;
		if (Globals.DEBUG_VIDEOADS_PROVIDERS)
		{
			List<string> list = new List<string>();
			if (Globals.DEBUG_VIDEOADS_PROVIDERS_ADCOLONY)
			{
				list.Add("adcolony");
			}
			if (Globals.DEBUG_VIDEOADS_PROVIDERS_FLURRY)
			{
				list.Add("flurryvideoads");
			}
			if (Globals.DEBUG_VIDEOADS_PROVIDERS_INMOBI)
			{
				list.Add("inmobivideoads");
			}
			if (Globals.DEBUG_VIDEOADS_PROVIDERS_TABJOY)
			{
				list.Add("tapjoyvideoads");
			}
			if (Globals.DEBUG_VIDEOADS_PROVIDERS_VUNGLE)
			{
				list.Add("vungleclips");
			}
			if (Globals.DEBUG_VIDEOADS_PROVIDERS_CHARTBOOST)
			{
				list.Add("chartboostvideoads");
			}
			array = list.ToArray();
		}
		else if (OnlineSettings.instance.TryGetValue("videoads_providerlist", out valueString))
		{
			if (string.IsNullOrEmpty(valueString))
			{
				array = new string[0];
			}
			else
			{
				array = valueString.Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = array[i].Trim();
				}
			}
		}
		return array;
	}

	private VideoAdProvider GetProvider(string providerId)
	{
		switch (providerId)
		{
		case "adcolony":
			if (_adColonyProvider == null && AdColonyProvider.isVideoProviderSupported)
			{
				_adColonyProvider = AdColonyProvider.instance;
				_adColonyProvider.id = "adcolony";
				_adColonyProvider.AddTakeoverBeganHandler(OnProviderTakeoverBegan);
				_adColonyProvider.AddTakeoverEndedHandler(OnProviderTakeoverEnded);
				_adColonyProvider.AddVideoWatchedHandler(OnProviderVideoWatched);
				_adColonyProvider.AddVideoUnavailableHandler(OnVideoCachingFailed);
				_adColonyProvider.AddVideoAvailableHandler(OnVideoCachingSucceded);
				_adColonyProvider.AddCachingTimedOutHandler(OnVideoCachingTimedOut);
			}
			return _adColonyProvider;
		case "flurryvideoads":
			if (_flurryVideoAdsProvider == null && FlurryVideoAdsProvider.isVideoProviderSupported)
			{
				_flurryVideoAdsProvider = FlurryVideoAdsProvider.instance;
				_flurryVideoAdsProvider.id = "flurryvideoads";
				_flurryVideoAdsProvider.AddTakeoverBeganHandler(OnProviderTakeoverBegan);
				_flurryVideoAdsProvider.AddTakeoverEndedHandler(OnProviderTakeoverEnded);
				_flurryVideoAdsProvider.AddVideoWatchedHandler(OnProviderVideoWatched);
				_flurryVideoAdsProvider.AddVideoUnavailableHandler(OnVideoCachingFailed);
				_flurryVideoAdsProvider.AddVideoAvailableHandler(OnVideoCachingSucceded);
				_flurryVideoAdsProvider.AddCachingTimedOutHandler(OnVideoCachingTimedOut);
			}
			return _flurryVideoAdsProvider;
		case "vungleclips":
			if (_vungleVideoAdsProvider == null && VungleProvider.isVideoProviderSupported)
			{
				_vungleVideoAdsProvider = VungleProvider.instance;
				_vungleVideoAdsProvider.id = "vungleclips";
				_vungleVideoAdsProvider.AddTakeoverBeganHandler(OnProviderTakeoverBegan);
				_vungleVideoAdsProvider.AddTakeoverEndedHandler(OnProviderTakeoverEnded);
				_vungleVideoAdsProvider.AddVideoWatchedHandler(OnProviderVideoWatched);
				_vungleVideoAdsProvider.AddVideoUnavailableHandler(OnVideoCachingFailed);
				_vungleVideoAdsProvider.AddVideoAvailableHandler(OnVideoCachingSucceded);
				_vungleVideoAdsProvider.AddCachingTimedOutHandler(OnVideoCachingTimedOut);
			}
			return _vungleVideoAdsProvider;
		case "tapjoyvideoads":
			if (_tapjoyVideoAdProvider == null && TapJoyVideoProvider.isVideoProviderSupported)
			{
				_tapjoyVideoAdProvider = TapJoyVideoProvider.instance;
				_tapjoyVideoAdProvider.id = "tapjoyvideoads";
				_tapjoyVideoAdProvider.AddTakeoverBeganHandler(OnProviderTakeoverBegan);
				_tapjoyVideoAdProvider.AddTakeoverEndedHandler(OnProviderTakeoverEnded);
				_tapjoyVideoAdProvider.AddVideoWatchedHandler(OnProviderVideoWatched);
				_tapjoyVideoAdProvider.AddVideoUnavailableHandler(OnVideoCachingFailed);
				_tapjoyVideoAdProvider.AddVideoAvailableHandler(OnVideoCachingSucceded);
				_tapjoyVideoAdProvider.AddCachingTimedOutHandler(OnVideoCachingTimedOut);
			}
			return _tapjoyVideoAdProvider;
		case "inmobivideoads":
			if (_inmobiVideoAdsProvider == null && InmobiVideoAdsProvider.isVideoProviderSupported)
			{
				_inmobiVideoAdsProvider = InmobiVideoAdsProvider.instance;
				_inmobiVideoAdsProvider.id = "inmobivideoads";
				_inmobiVideoAdsProvider.AddTakeoverBeganHandler(OnProviderTakeoverBegan);
				_inmobiVideoAdsProvider.AddTakeoverEndedHandler(OnProviderTakeoverEnded);
				_inmobiVideoAdsProvider.AddVideoWatchedHandler(OnProviderVideoWatched);
				_inmobiVideoAdsProvider.AddVideoUnavailableHandler(OnVideoCachingFailed);
				_inmobiVideoAdsProvider.AddVideoAvailableHandler(OnVideoCachingSucceded);
				_inmobiVideoAdsProvider.AddCachingTimedOutHandler(OnVideoCachingTimedOut);
			}
			return _inmobiVideoAdsProvider;
		case "chartboostvideoads":
			if (_chartboostVideoAdProvider == null && ChartboostVideoAdsProvider.isVideoProviderSupported)
			{
				_chartboostVideoAdProvider = ChartboostVideoAdsProvider.instance;
				_chartboostVideoAdProvider.id = "chartboostvideoads";
				_chartboostVideoAdProvider.AddTakeoverBeganHandler(OnProviderTakeoverBegan);
				_chartboostVideoAdProvider.AddTakeoverEndedHandler(OnProviderTakeoverEnded);
				_chartboostVideoAdProvider.AddVideoWatchedHandler(OnProviderVideoWatched);
				_chartboostVideoAdProvider.AddVideoUnavailableHandler(OnVideoCachingFailed);
				_chartboostVideoAdProvider.AddVideoAvailableHandler(OnVideoCachingSucceded);
				_chartboostVideoAdProvider.AddCachingTimedOutHandler(OnVideoCachingTimedOut);
			}
			return _chartboostVideoAdProvider;
		default:
			LogWarning("VideoAdsManager WARNING: Unknown provider id: " + providerId);
			return null;
		}
	}

	private string GetProviderId(VideoAdProvider provider)
	{
		if (provider != null)
		{
			if (provider == _adColonyProvider)
			{
				return "adcolony";
			}
			if (provider == _flurryVideoAdsProvider)
			{
				return "flurryvideoads";
			}
			if (provider == _vungleVideoAdsProvider)
			{
				return "vungleclips";
			}
			if (provider == _tapjoyVideoAdProvider)
			{
				return "tapjoyvideoads";
			}
			if (provider == _inmobiVideoAdsProvider)
			{
				return "inmobivideoads";
			}
			if (provider == _chartboostVideoAdProvider)
			{
				return "chartboostvideoads";
			}
		}
		LogError("VideoAdsManager GetProviderId() ERROR: Unknown provider");
		return null;
	}

	public VideoState GetCurrentVideoState()
	{
		return currentVideoState;
	}

	private void ResetCurrentVideoState()
	{
		currentVideoState = VideoState.NoVideos;
	}

	private void OnProviderTakeoverBegan(VideoAdProvider provider)
	{
		if (state != State.Initialized)
		{
			LogWarning(string.Concat("VideoAdsManager: Unexpected state in OnProviderTakeoverBegan (", state, ")"));
		}
		state = State.TakeoverActive;
		MuteSound();
	}

	private void OnProviderTakeoverEnded(VideoAdProvider provider)
	{
		if (state != State.TakeoverActive)
		{
			LogWarning(string.Concat("VideoAdsManager: Unexpected state in OnProviderTakeoverEnded (", state, ")"));
		}
		state = State.Initialized;
		if (UIScreenController.Instance.IsVideoAdsOverlayVisible())
		{
			UIScreenController.Instance.HideVideoAdsOverlay();
		}
		RestoreSound();
		_isVideoPlaying = false;
		StartCachingVideo();
	}

	private void OnProviderVideoWatched(VideoAdProvider provider)
	{
		state = State.Initialized;
		VideoWatched();
	}

	private void MuteSound()
	{
		_gameSoundOnBeforeMute = Settings.optionSound;
		Settings.optionSound = false;
		HouseKeeper.SaveSoundOptionBeforeVideoAds(_gameSoundOnBeforeMute);
	}

	private void RestoreSound()
	{
		Settings.optionSound = _gameSoundOnBeforeMute;
		HouseKeeper.DeleteSoundOptionBeforeVideoAds();
	}

	private void VideoWatched()
	{
		PlayerInfo playerInfo = PlayerInfo.Instance;
		_isVideoPlaying = false;
		if (_rewardOverride == null)
		{
			if (AbleToWatchAVideoForKeysToday() && _numberOfKeysToRewardForVideo > 0)
			{
				playerInfo.amountOfKeys += _numberOfKeysToRewardForVideo;
				playerInfo.lastTimeAVideoForKeysWasSeen = playerInfo.lastKnownServerTime;
				if (_onKeyVideoWatchedChangedHandler != null)
				{
					_onKeyVideoWatchedChangedHandler();
				}
				UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.VideoForKeys));
			}
			else
			{
				RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.MysteryBoxMini);
				UIScreenController.Instance.QueueMysteryBox();
			}
		}
		else
		{
			Action rewardOverride = _rewardOverride;
			_rewardOverride = null;
			rewardOverride();
		}
		if (UIScreenController.Instance.IsVideoAdsOverlayVisible())
		{
			UIScreenController.Instance.HideVideoAdsOverlay();
		}
	}

	public bool IsAnyVideoAvailable()
	{
		if (Globals.DEBUG_SKIP_VIDEO_ADS)
		{
			return true;
		}
		return currentVideoState == VideoState.Ready;
	}

	public bool IsVideoForKeysAvailable()
	{
		GetAmountOfKeysRewardedForVideo();
		return AbleToWatchAVideoForKeysToday() && currentVideoState == VideoState.Ready && _numberOfKeysToRewardForVideo > 0;
	}

	private bool AbleToWatchAVideoForKeysToday()
	{
		return DateTime.Compare(PlayerInfo.Instance.lastKnownServerTime.Date, PlayerInfo.Instance.lastTimeAVideoForKeysWasSeen.Date) != 0;
	}

	public DateTime GetLastTimeAVideoForKeysWasSeen()
	{
		return PlayerInfo.Instance.lastTimeAVideoForKeysWasSeen;
	}

	public int GetAmountOfKeysRewardedForVideo()
	{
		_numberOfKeysToRewardForVideo = OnlineSettings.instance.GetValue("keys_rewarded_for_video_ad", 2);
		return _numberOfKeysToRewardForVideo;
	}

	public void OnApplicationPause(bool pause)
	{
		if (currentProvider != null)
		{
			if (pause)
			{
				currentProvider.OnPause();
			}
			else
			{
				currentProvider.OnResume();
			}
		}
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

	[Conditional("ENABLE_VIDEOADSMANAGER_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}
}
