using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using Kiloo.Common;
using Kiloo.Plugins.DeviceUtility;
using Kiloo.Plugins.Facebook;
using Kiloo.Plugins.GameCenter;
using SYBO.Core.Utilities;
using UnityEngine;

public class SocialManager : MonoBehaviour
{
	public enum SocialPictureSize
	{
		Normal = 0,
		Large = 1
	}

	public enum FacebookState
	{
		None = 0,
		Error = 1,
		LoggingIn = 2,
		LoggedIn = 3
	}

	private enum FacebookPermissionCheckState
	{
		Waiting = 0,
		Ok = 1,
		Missing = 2,
		LoggedOut = 3,
		Error = 4
	}

	private class FacebookPermissionCheck
	{
		public FacebookPermissionCheckState state;
	}

	public enum WWWRequestResult
	{
		Success = 0,
		Error = 1
	}

	public enum PokeType
	{
		Poke = 0,
		Duel = 1
	}

	public delegate void WWWComplete(WWWRequestResult result, string output, object cookie);

	private const byte VERSION = 2;

	private const string UNREGISTER_DEVICE_URL = "/unregisterdevice.php?android";

	private const string REGISTER_DEVICE_URL = "/register2.php?android";

	private const string REPORT_SCORE_URL = "/report2.php?android";

	private const string CONSOLIDATE_FRIENDS_URL = "/friends2.php?android";

	private const string UPDATE_FRIEND_SCORES_URL = "/scores2.php?android";

	private const string POKE_URL = "/poke.php?android";

	private const string BRAG_URL = "/brag.php?android";

	private const string ANNOUNCE_URL = "/announce.php?android";

	private const float FACEBOOK_LOGIN_TIMEOUT = 10f;

	public const string NOTIFICATION_SOUND_NAME = "cpush.wav";

	private const string FACEBOOK_APPID = "254616967963463";

	public const string SECRET = "resxrctrv7tgv7gb8h9h9u0909kllfmolkjnhghgjjkhjghg";

	private const string ONLINESETTINGS_CONSOLIDATE_MIN_SECONDS_KEY = "social_consolidate_min_seconds";

	private const int CONSOLIDATE_MIN_SECONDS_DEFAULT = 3600;

	public const float USER_DATA_REFRESH_ON_AUTHENTICATE_AGE = 600f;

	public const float FRIEND_DATA_REFRESH_ON_RESUME_AGE = 3600f;

	private const string ONLINESETTINGS_REPORT_GAMES_COUNT_KEY = "social_report_games_count";

	private const int REPORT_GAMES_COUNT_DEFAULT = 5;

	private const string ONLINESETTINGS_REPORT_MIN_SECONDS_KEY = "social_report_min_seconds";

	private const int REPOST_MIN_SECONDS_DEFAULT = 1200;

	private const string LAST_REGISTER_TIMESTAMPTICKS_PLAYERPREFSKEY = "socmanlastregtime";

	private const string LAST_REGISTER_DATA_PLAYERPREFSKEY = "socmanlastregdata";

	private const string ONLINESETTINGS_REGISTER_MIN_SECONDS_KEY = "social_register_min_seconds";

	private const int REGISTER_MIN_SECONDS_DEFAULT = 3600;

	private const string ONLINESETTINGS_FACEBOOK_FETCH_MAX_USERS_KEY = "social_facebook_fetch_max_users";

	private const int FACEBOOK_FETCH_MAX_USERS_DEFAULT = 5000;

	private const string ONLINESETTINGS_FACEBOOK_KEEP_MAX_USERS_KEY = "social_facebook_keep_max_users";

	private const int FACEBOOK_KEEP_MAX_USERS_DEFAULT = 200;

	private const string ONLINESETTINGS_FRIENDSCORES_MIN_SECONDS_KEY = "social_friendscores_min_seconds";

	private const int FRIENDSCORES_MIN_SECONDS_DEFAULT = 120;

	public const int FACEBOOK_CONSOLIDATED = 1;

	public const int GAMECENTER_CONSOLIDATED = 2;

	private float _lastConsolidateFriendCompleteTime;

	private List<string> _latestPreConsolidationFacebookIds;

	private List<string> _latestSuccesfullyConsolidatedFacebookIds;

	private string _tempRegisterUserData;

	private bool _appHasFocus = true;

	private static SocialManager _instance;

	private int _userid;

	private Action<FacebookProfile> _facebookPictureDownloadedHandler;

	private bool _registerUserCompleted;

	private Action<bool> _friendsConsolidatedHandler;

	private Action _friendsChangedHandler;

	private FacebookProfile _fbProfile;

	private GameCenterProfile _gcProfile;

	private Action _localUserUpdatedHandler;

	private List<Friend> _friends;

	private IDictionary<string, FacebookProfile> _fbProfiles;

	private bool _fbReady;

	private DateTime _lastFriendScoreUpdateTimestamp = new DateTime(0L);

	private bool _didAddAuthenticationHandlerToGameCenter;

	private Action _onFacebookStateChanged;

	private FacebookState _fbState;

	private bool _didAddChangeHandlerToFacebookSession;

	private readonly string[] requiredPermissions = new string[2] { "public_profile", "user_friends" };

	private int _downloadFacebookPicturesContext;

	private int _consolidationState;

	private bool _consolidatedFriendsCompleted;

	private IDictionary<string, Friend.Status> _friendStatus;

	private bool _dirty;

	private static bool _applicationIsQuiting;

	private IDictionary<string, GameCenter.Achievement> _achievements;

	private int _initAchievementsContext;

	private bool _pausePhotoDownloading;

	public int localUserId
	{
		get
		{
			return _userid;
		}
	}

	public bool isRunningFacebookLogin { get; private set; }

	public bool isRegisterUserCompleted
	{
		get
		{
			return _registerUserCompleted;
		}
	}

	public FacebookProfile facebookProfile
	{
		get
		{
			return _fbProfile;
		}
	}

	public GameCenterProfile gamecenterProfile
	{
		get
		{
			return _gcProfile;
		}
	}

	public Texture2D localUserImage
	{
		get
		{
			if (facebookProfile != null)
			{
				return facebookProfile.image;
			}
			if (gamecenterProfile != null)
			{
				return gamecenterProfile.image;
			}
			return null;
		}
	}

	public string localUserName
	{
		get
		{
			if (facebookProfile != null)
			{
				return facebookProfile.name;
			}
			if (gamecenterProfile != null)
			{
				return gamecenterProfile.displayName;
			}
			return "Me";
		}
	}

	public static SocialManager instance
	{
		get
		{
			Init();
			return _instance;
		}
	}

	public bool hasGameCenterShowLoginView
	{
		get
		{
			return GameCenter.hasLoginView;
		}
	}

	public bool gamecenterIsLoggedIn
	{
		get
		{
			return SocialManagerIOS.instance.gameCenterState != SocialManagerIOS.GCState.Authenticating && _gcProfile != null;
		}
	}

	public bool gamecenterIsLoggedOut
	{
		get
		{
			return SocialManagerIOS.instance.gameCenterState == SocialManagerIOS.GCState.LoggedOut;
		}
	}

	public FacebookState facebookState
	{
		get
		{
			return _fbState;
		}
		private set
		{
			if (_fbState != value)
			{
				_fbState = value;
				Action onFacebookStateChanged = _onFacebookStateChanged;
				if (onFacebookStateChanged != null)
				{
					onFacebookStateChanged();
				}
			}
		}
	}

	public bool facebookIsLoggedIn
	{
		get
		{
			return FacebookSession.HasOpenInstance();
		}
	}

	public static bool facebookDialogsSupported
	{
		get
		{
			return FacebookSession.dialogsSupported;
		}
	}

	public int consolidationState
	{
		get
		{
			return _consolidationState;
		}
	}

	public bool consolidatedFriendsCompleted
	{
		get
		{
			return _consolidatedFriendsCompleted;
		}
	}

	public bool dirty
	{
		get
		{
			return _dirty;
		}
	}

	public bool requestInvolvingDevicetokenInProgress { get; private set; }

	public bool canUnregisterDevice { get; private set; }

	public bool pausePhotoDownloading
	{
		get
		{
			return _pausePhotoDownloading;
		}
		set
		{
			if (SocialManagerIOS.isInstanced)
			{
				SocialManagerIOS.instance.pausePhotoDownloading = value;
			}
			_pausePhotoDownloading = value;
		}
	}

	public event Action<bool> onRegisterUserCompleted;

	public void AddFacebookPictureDownloadedHandler(Action<FacebookProfile> handler)
	{
		_facebookPictureDownloadedHandler = (Action<FacebookProfile>)Delegate.Combine(_facebookPictureDownloadedHandler, handler);
	}

	public void RemoveFacebookPictureDownloadedHandler(Action<FacebookProfile> handler)
	{
		_facebookPictureDownloadedHandler = (Action<FacebookProfile>)Delegate.Remove(_facebookPictureDownloadedHandler, handler);
	}

	[Conditional("UNITY_IPHONE")]
	public void AddGameCenterSmallSizePictureDownloadedHandler(Action<GameCenterProfile> handler)
	{
	}

	[Conditional("UNITY_IPHONE")]
	public void RemoveGameCenterSmallSizePictureDownloadedHandler(Action<GameCenterProfile> handler)
	{
	}

	public void AddFriendsConsolidatedHandler(Action<bool> handler)
	{
		_friendsConsolidatedHandler = (Action<bool>)Delegate.Combine(_friendsConsolidatedHandler, handler);
	}

	public void RemoveFriendsConsolidatedHandler(Action<bool> handler)
	{
		_friendsConsolidatedHandler = (Action<bool>)Delegate.Remove(_friendsConsolidatedHandler, handler);
	}

	private void FriendConsolidationDidFinish(bool success)
	{
		Action<bool> friendsConsolidatedHandler = _friendsConsolidatedHandler;
		if (friendsConsolidatedHandler != null)
		{
			friendsConsolidatedHandler(success);
		}
		FriendsDidChange();
		if (success)
		{
			CachedLeaderboard.Save(FriendsSortedByScore());
		}
	}

	public void AddFriendsChangedHandler(Action handler)
	{
		_friendsChangedHandler = (Action)Delegate.Combine(_friendsChangedHandler, handler);
	}

	public void RemoveFriendsChangedHandler(Action handler)
	{
		_friendsChangedHandler = (Action)Delegate.Remove(_friendsChangedHandler, handler);
	}

	private void FriendsDidChange()
	{
		Action friendsChangedHandler = _friendsChangedHandler;
		if (friendsChangedHandler != null)
		{
			friendsChangedHandler();
		}
	}

	public void AddLocalUserUpdatedHandler(Action handler)
	{
		_localUserUpdatedHandler = (Action)Delegate.Combine(_localUserUpdatedHandler, handler);
	}

	public void RemoveLocalUserUpdatedHandler(Action handler)
	{
		_localUserUpdatedHandler = (Action)Delegate.Remove(_localUserUpdatedHandler, handler);
	}

	public Friend FindFriend(int userid)
	{
		return _friends.Find((Friend f) => userid == f.userid);
	}

	public Friend[] FriendsSortedByScore()
	{
		if (_friends != null)
		{
			Friend[] array = _friends.ToArray();
			Array.Sort(array, (Friend x, Friend y) => y.score - x.score);
			return array;
		}
		return new Friend[0];
	}

	public Friend[] FriendsSortedByCash()
	{
		if (_friends != null)
		{
			Friend[] array = _friends.ToArray();
			Array.Sort(array, (Friend x, Friend y) => y.gamesToCashIn - x.gamesToCashIn);
			return array;
		}
		return new Friend[0];
	}

	public void DownloadLargePictureForUser(Friend friend, Action<Friend, Texture2D> handler)
	{
		if (handler == null)
		{
			return;
		}
		int relation = friend.relation;
		if ((relation & 1) == 1)
		{
			StartCoroutine(DownloadPictureForProfile(friend.fbProfile, SocialPictureSize.Large, delegate(FacebookProfile profile, Texture2D texture)
			{
				handler(friend, texture);
			}));
		}
		else if ((relation & 2) == 2)
		{
			StartCoroutine(SocialManagerIOS.instance.DownloadPictureForProfile(friend.gcProfile, SocialPictureSize.Large, delegate(GameCenterProfile Profiler, Texture2D texture)
			{
				handler(friend, texture);
			}));
		}
	}

	public static void Init()
	{
		if (_instance == null && !_applicationIsQuiting)
		{
			GameObject gameObject = new GameObject("SocialManager");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<SocialManager>();
			HierarchyUtility.ReparentByHierarchyPathType(gameObject.transform, HierarchyPathType.DynamicManagers);
		}
	}

	[Conditional("UNITY_IPHONE")]
	public void ShowGameCenterLoginView()
	{
		GameCenter.ShowLoginView();
	}

	[Conditional("UNITY_IPHONE")]
	private void ClearGameCenter()
	{
		_gcProfile = null;
		if (_friends != null)
		{
			_friends.RemoveAll((Friend item) => item.gcProfile != null && item.fbProfile == null);
			_friends.ForEach(delegate(Friend item)
			{
				item.gcProfile = null;
			});
		}
		SocialManagerIOS.instance.ClearFriends();
		SocialManagerIOS.instance.AbortAsynchronousProcesses();
		_consolidationState &= -3;
	}

	private void OnGameCenterAuthentication(bool hasLoginView, string error)
	{
		GameCenter.LocalPlayer localPlayer = GameCenter.GetLocalPlayer();
		if (localPlayer.authenticated)
		{
			if (_gcProfile != null && !(_gcProfile.id != localPlayer.playerID) && (DateTime.Now - _gcProfile.refreshedTime).TotalSeconds > 600.0)
			{
				_gcProfile = null;
			}
			if (_gcProfile == null)
			{
				_gcProfile = new GameCenterProfile
				{
					alias = localPlayer.alias,
					displayName = localPlayer.displayName,
					id = localPlayer.playerID,
					refreshedTime = DateTime.Now
				};
				GameCenter.LoadPhoto(localPlayer.playerID, GameCenter.PhotoSize.Small, 50, 50, delegate(string playerID, Texture2D photo, string photoError)
				{
					if (_gcProfile != null)
					{
						_gcProfile.image = photo;
					}
				});
				InitAchievements();
			}
			Flurry.LogGameCenterLogin();
		}
		else if (!gamecenterIsLoggedOut)
		{
			SocialManagerIOS.instance.gameCenterState = SocialManagerIOS.GCState.LoggedOut;
			Invalidate();
			FriendsDidChange();
		}
	}

	[Conditional("UNITY_IPHONE")]
	public void InitGameCenter()
	{
		SocialManagerIOS.instance.gameCenterState = SocialManagerIOS.GCState.Authenticating;
		if (!_didAddAuthenticationHandlerToGameCenter)
		{
			GameCenter.authenticationEvent += OnGameCenterAuthentication;
			_didAddAuthenticationHandlerToGameCenter = true;
		}
		if (GameCenter.initialAuthenticationComplete)
		{
			OnGameCenterAuthentication(GameCenter.hasLoginView, null);
		}
		else
		{
			GameCenter.Init();
		}
	}

	private void OnGameCenterPhotoDownloaded(GameCenterProfile gcProfile)
	{
		if (gcProfile == _gcProfile)
		{
			if (_localUserUpdatedHandler != null)
			{
				_localUserUpdatedHandler();
			}
		}
		else
		{
			if (_friends == null)
			{
				return;
			}
			Friend friend = null;
			foreach (Friend friend2 in _friends)
			{
				if (friend2.gcProfile == gcProfile)
				{
					friend = friend2;
				}
			}
			if (friend != null)
			{
				friend.InvokeUpdatedHandler();
			}
		}
	}

	private void OnFacebookPhotoDownloaded(FacebookProfile fbProfile)
	{
		if (fbProfile == _fbProfile)
		{
			if (_localUserUpdatedHandler != null)
			{
				_localUserUpdatedHandler();
			}
		}
		else
		{
			if (_friends == null)
			{
				return;
			}
			Friend friend = null;
			foreach (Friend friend2 in _friends)
			{
				if (friend2.fbProfile == fbProfile)
				{
					friend = friend2;
				}
			}
			if (friend != null)
			{
				friend.InvokeUpdatedHandler();
			}
		}
	}

	public void AddFacebookStateChangedHandler(Action handler)
	{
		_onFacebookStateChanged = (Action)Delegate.Combine(_onFacebookStateChanged, handler);
	}

	public void RemoveFacebookStateChangedHandler(Action handler)
	{
		_onFacebookStateChanged = (Action)Delegate.Remove(_onFacebookStateChanged, handler);
	}

	public bool FacebookLogin(Action<bool> onComplete)
	{
		if (!isRunningFacebookLogin)
		{
			_fbReady = false;
			isRunningFacebookLogin = true;
			StartCoroutine(FacebookLoginCoroutine(onComplete));
			return true;
		}
		LogWarning("SocialManager.FacebookLogin called, but we are already logging in", this);
		return false;
	}

	private void HandleLoggingIntoFacebookComplete(bool success, Action<bool> onComplete)
	{
		isRunningFacebookLogin = false;
		if (onComplete != null)
		{
			onComplete(success);
		}
	}

	public void FacebookLogout()
	{
		FacebookSession facebookSession = FacebookSession.instance;
		if (facebookSession != null)
		{
			facebookSession.Close(true);
		}
		if (_friends != null)
		{
			_friends.RemoveAll((Friend item) => item.gcProfile == null && item.fbProfile != null);
			_friends.ForEach(delegate(Friend item)
			{
				item.fbProfile = null;
			});
		}
		_latestSuccesfullyConsolidatedFacebookIds = null;
		_fbProfiles = null;
		_fbProfile = null;
		FriendsDidChange();
		_consolidationState &= -2;
		CharacterScreenManager.Instance.OnFacebookLogout();
	}

	public static bool IsOAuthException(FacebookError error, object obj)
	{
		if (error != null)
		{
			if (error.category == FacebookError.Category.AUTHENTICATION_REOPEN_SESSION || error.category == FacebookError.Category.PERMISSION)
			{
				return true;
			}
			if (error.errorType == "OAuthException")
			{
				return true;
			}
		}
		return false;
	}

	private void OnFacebookSessionStatusChanged(FacebookSession.State status, FacebookError error)
	{
		if (status == FacebookSession.State.OPENING)
		{
			return;
		}
		if (FacebookSession.IsOpened(status))
		{
			if (facebookState == FacebookState.LoggingIn)
			{
				Flurry.LogFacebookLogin();
			}
			facebookState = FacebookState.LoggedIn;
		}
		else if (status == FacebookSession.State.CLOSED_LOGIN_FAILED)
		{
			if (facebookState == FacebookState.LoggingIn && error != null && error.shouldNotifyUser && !string.IsNullOrEmpty(error.userActionMessage))
			{
				DeviceUtility.ShowNativePopup(Strings.Get(StringID.FACEBOOK), error.userActionMessage, Strings.Get(StringID.OK));
			}
			facebookState = FacebookState.Error;
		}
		else if (error != null)
		{
			facebookState = FacebookState.Error;
		}
		else
		{
			facebookState = FacebookState.None;
		}
	}

	public bool HasFacebookAccessToken()
	{
		FacebookSession facebookSession = FacebookSession.instance;
		if (facebookSession != null)
		{
			FacebookSession.State state = facebookSession.GetState();
			return state == FacebookSession.State.CREATED_TOKEN_LOADED || FacebookSession.IsOpened(state);
		}
		return false;
	}

	private FacebookSession CreateFacebookSessionIfNeeded()
	{
		if (!_didAddChangeHandlerToFacebookSession)
		{
			FacebookSession.statusChangedEvent += OnFacebookSessionStatusChanged;
			_didAddChangeHandlerToFacebookSession = true;
		}
		if (!FacebookSession.HasOpenInstance())
		{
			FacebookSession.appId = "254616967963463";
			if (Application.platform == RuntimePlatform.WP8Player)
			{
				FacebookSession.Create(string.Join(",", requiredPermissions), FacebookDefaultAudience.FRIENDS);
			}
			else
			{
				FacebookSession.Create(null, FacebookDefaultAudience.FRIENDS);
				FacebookSession.State state = FacebookSession.instance.GetState();
				if (state != FacebookSession.State.CREATED_TOKEN_LOADED)
				{
					FacebookSession.instance.Close(false);
					string permissions = string.Join(",", requiredPermissions);
					FacebookSession.Create(permissions, FacebookDefaultAudience.FRIENDS);
				}
			}
		}
		return FacebookSession.instance;
	}

	private Coroutine FacebookCheckPermissionsAsync(FacebookPermissionCheck check)
	{
		return StartCoroutine(FacebookCheckPermissionsCoroutine(check));
	}

	private IEnumerator FacebookCheckPermissionsCoroutine(FacebookPermissionCheck check)
	{
		if (Application.platform == RuntimePlatform.WP8Player)
		{
			check.state = FacebookPermissionCheckState.Ok;
			yield break;
		}
		FacebookSession fbSession = FacebookSession.instance;
		check.state = FacebookPermissionCheckState.Waiting;
		FacebookPermissionCheck check2 = default(FacebookPermissionCheck);
		fbSession.GraphRequest("me/permissions", new Dictionary<string, object>(), HttpMethod.GET, delegate(IDictionary<string, object> result, FacebookError error, bool isFromCache)
		{
			try
			{
				if (check2.state != FacebookPermissionCheckState.Waiting)
				{
					return;
				}
				if (IsOAuthException(error, result))
				{
					FacebookLogout();
					check2.state = FacebookPermissionCheckState.LoggedOut;
				}
				else if (result != null)
				{
					object value;
					result.TryGetValue("data", out value);
					IList<object> list = value as IList<object>;
					if (list != null)
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string>();
						foreach (object item in list)
						{
							IDictionary dictionary2 = item as IDictionary;
							if (dictionary2 != null)
							{
								string text = dictionary2["permission"] as string;
								string text2 = dictionary2["status"] as string;
								if (text != null && text2 != null)
								{
									dictionary[text] = text2;
								}
							}
						}
						bool flag = true;
						string[] array = requiredPermissions;
						foreach (string text3 in array)
						{
							string value2;
							dictionary.TryGetValue(text3, out value2);
							if (value2 != "granted")
							{
								LogError("Required Facebook permission not granted: \"" + text3 + "\"");
								flag = false;
							}
						}
						check2.state = (flag ? FacebookPermissionCheckState.Ok : FacebookPermissionCheckState.Missing);
					}
					else
					{
						LogError("Failed to get list of Facebook permissions");
					}
				}
			}
			catch (Exception)
			{
				LogError("Exception while checking permissions");
			}
			if (check2.state == FacebookPermissionCheckState.Waiting)
			{
				check2.state = FacebookPermissionCheckState.Error;
			}
		});
		float lastRealTime;
		float timeLeftBeforeGivingUp;
		ResetTimeLeft(out lastRealTime, out timeLeftBeforeGivingUp);
		while (check.state == FacebookPermissionCheckState.Waiting)
		{
			UpdateTimeLeft(ref lastRealTime, ref timeLeftBeforeGivingUp);
			if (timeLeftBeforeGivingUp <= 0f)
			{
				LogError("Timeout while checking permissions");
				check.state = FacebookPermissionCheckState.Error;
			}
			else
			{
				yield return null;
			}
		}
	}

	private void ResetTimeLeft(out float lastRealTime, out float timeLeft)
	{
		timeLeft = 10f;
		lastRealTime = Time.realtimeSinceStartup;
	}

	private void UpdateTimeLeft(ref float lastRealTime, ref float timeLeft)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = realtimeSinceStartup - lastRealTime;
		lastRealTime = realtimeSinceStartup;
		if (num < 0.5f && _appHasFocus)
		{
			timeLeft -= num;
		}
	}

	private IEnumerator FacebookLoginCoroutine(Action<bool> onComplete)
	{
		FacebookSession fbSession = CreateFacebookSessionIfNeeded();
		if (fbSession == null)
		{
			HandleLoggingIntoFacebookComplete(false, onComplete);
			yield break;
		}
		bool asynchronousRequestsCompleted = false;
		float lastRealTime;
		float timeLeftBeforeGivingUp;
		if (!FacebookSession.HasOpenInstance())
		{
			facebookState = FacebookState.LoggingIn;
			fbSession.OpenForRead();
			ResetTimeLeft(out lastRealTime, out timeLeftBeforeGivingUp);
			while (facebookState != FacebookState.LoggedIn)
			{
				if (facebookState == FacebookState.Error)
				{
					HandleLoggingIntoFacebookComplete(false, onComplete);
					yield break;
				}
				UpdateTimeLeft(ref lastRealTime, ref timeLeftBeforeGivingUp);
				if (timeLeftBeforeGivingUp <= 0f)
				{
					fbSession.Cancel();
					facebookState = FacebookState.Error;
					HandleLoggingIntoFacebookComplete(false, onComplete);
					LogWarning("SocialManager WARNING: Facebook login timed out", this);
					yield break;
				}
				yield return null;
			}
		}
		FacebookPermissionCheck permissionsCheck = new FacebookPermissionCheck();
		yield return FacebookCheckPermissionsAsync(permissionsCheck);
		if (permissionsCheck.state == FacebookPermissionCheckState.Missing)
		{
			bool statusDidChange = false;
			FacebookSession.StatusChangedHandler statusChangedHandler = null;
			statusChangedHandler = delegate
			{
				FacebookSession.statusChangedEvent -= statusChangedHandler;
				statusDidChange = true;
			};
			FacebookSession.statusChangedEvent += statusChangedHandler;
			fbSession.RequestNewReadPermissions(string.Join(",", requiredPermissions));
			ResetTimeLeft(out lastRealTime, out timeLeftBeforeGivingUp);
			while (!statusDidChange)
			{
				UpdateTimeLeft(ref lastRealTime, ref timeLeftBeforeGivingUp);
				if (timeLeftBeforeGivingUp <= 0f)
				{
					LogError("Timed out waiting for FacebookSession.RequestNewReadPermissions", this);
					facebookState = FacebookState.Error;
					HandleLoggingIntoFacebookComplete(false, onComplete);
					yield break;
				}
				yield return null;
			}
			yield return FacebookCheckPermissionsAsync(permissionsCheck);
		}
		if (permissionsCheck.state != FacebookPermissionCheckState.Ok)
		{
			if (permissionsCheck.state == FacebookPermissionCheckState.Missing)
			{
				LogError("Facebook permissions are NOT ok, logging out");
				FacebookLogout();
			}
			else
			{
				LogError("Error checking Facebook permissions");
			}
			facebookState = FacebookState.Error;
			HandleLoggingIntoFacebookComplete(false, onComplete);
			yield break;
		}
		ResetTimeLeft(out lastRealTime, out timeLeftBeforeGivingUp);
		Action<bool> onComplete2 = default(Action<bool>);
		fbSession.GraphRequest("me", new Dictionary<string, object> { { "fields", "id,name,first_name" } }, HttpMethod.GET, delegate(IDictionary<string, object> result, FacebookError error, bool isFromCache)
		{
			if (!(timeLeftBeforeGivingUp <= 0f))
			{
				bool flag = false;
				if (IsOAuthException(error, result))
				{
					FacebookLogout();
				}
				else if (result != null)
				{
					_fbProfile = new FacebookProfile();
					object value;
					result.TryGetValue("id", out value);
					_fbProfile.id = value as string;
					result.TryGetValue("first_name", out value);
					_fbProfile.name = value as string;
					result.TryGetValue("name", out value);
					_fbProfile.fullName = value as string;
					flag = _fbProfile.id != null;
				}
				if (flag)
				{
					StartCoroutine(DownloadPictureForProfile(_fbProfile, SocialPictureSize.Normal, OnDownloadedFacebookPicture));
					int value2 = OnlineSettings.instance.GetValue("social_facebook_fetch_max_users", 5000);
					fbSession.GraphRequest("me/friends", new Dictionary<string, object>
					{
						{ "fields", "id,name,first_name,installed" },
						{
							"limit",
							value2.ToString(CultureInfo.InvariantCulture)
						}
					}, HttpMethod.GET, delegate(IDictionary<string, object> friendsResult, FacebookError friendsError, bool friendsIsFromCache)
					{
						if (timeLeftBeforeGivingUp <= 0f)
						{
							return;
						}
						asynchronousRequestsCompleted = true;
						try
						{
							bool flag2 = false;
							if (IsOAuthException(friendsError, friendsResult))
							{
								FacebookLogout();
							}
							else
							{
								if (friendsError != null)
								{
								}
								if (friendsResult != null)
								{
									object value3;
									friendsResult.TryGetValue("data", out value3);
									IList<object> list = value3 as IList<object>;
									if (list != null)
									{
										int value4 = OnlineSettings.instance.GetValue("social_facebook_keep_max_users", 200);
										List<FacebookProfile> list2 = new List<FacebookProfile>();
										foreach (object item in list)
										{
											IDictionary<string, object> dictionary = item as IDictionary<string, object>;
											if (dictionary != null && dictionary.ContainsKey("id"))
											{
												dictionary.TryGetValue("id", out value3);
												string text = value3 as string;
												if (text != null && dictionary.ContainsKey("installed"))
												{
													FacebookProfile value5;
													if (_fbProfiles == null || !_fbProfiles.TryGetValue(text, out value5))
													{
														value5 = new FacebookProfile
														{
															id = text
														};
													}
													dictionary.TryGetValue("first_name", out value3);
													value5.name = value3 as string;
													dictionary.TryGetValue("name", out value3);
													value5.fullName = value3 as string;
													list2.Add(value5);
												}
											}
											else
											{
												LogError("FacebookLogin: Unexpected format of FaceBook Friend", this);
											}
										}
										int count = list2.Count;
										if (count > value4)
										{
											list2.Sort((FacebookProfile p1, FacebookProfile p2) => string.Compare(p1.id, p2.id, StringComparison.InvariantCulture));
											list2.RemoveRange(value4, count - value4);
										}
										Dictionary<string, FacebookProfile> dictionary2 = new Dictionary<string, FacebookProfile>();
										foreach (FacebookProfile item2 in list2)
										{
											dictionary2[item2.id] = item2;
										}
										_fbProfiles = dictionary2;
										if (Flurry.ShouldLogDailyEvent("Social Facebook Friends Retrieved"))
										{
											string text2 = Flurry.ConvertFriendCountToBracket(list.Count);
											string text3 = Flurry.ConvertFriendCountToBracket(count);
											Flurry.LogEventWithSeveralParameters("Social Facebook Friends Retrieved", "Total friends;Installed friends", text2 + ";" + text3);
										}
										flag2 = true;
									}
									else
									{
										LogError("FacebookLogin: Error: Could not retreive friends", this);
									}
								}
							}
							if (flag2)
							{
								_fbReady = true;
								Invalidate();
								HandleLoggingIntoFacebookComplete(true, onComplete2);
								StartCoroutine(DownloadFacebookPictures(_fbProfiles));
							}
							else
							{
								_fbProfiles = null;
								_fbReady = true;
								Invalidate();
								HandleLoggingIntoFacebookComplete(false, onComplete2);
							}
						}
						catch (Exception ex)
						{
							LogWarning("Error: " + ex);
							HandleLoggingIntoFacebookComplete(false, onComplete2);
						}
					});
				}
				else
				{
					asynchronousRequestsCompleted = true;
					LogError("FacebookLogin: Error getting user info, error: " + error, this);
					HandleLoggingIntoFacebookComplete(false, onComplete2);
				}
			}
		});
		while (!asynchronousRequestsCompleted && timeLeftBeforeGivingUp > 0f)
		{
			UpdateTimeLeft(ref lastRealTime, ref timeLeftBeforeGivingUp);
			if (timeLeftBeforeGivingUp <= 0f)
			{
				facebookState = FacebookState.Error;
				HandleLoggingIntoFacebookComplete(false, onComplete);
				LogWarning("SocialManager WARNING: Facebook requests to get me and friends info timed out", this);
				break;
			}
			yield return null;
		}
	}

	private bool HasUnconsolidatedFacebookFriends()
	{
		if (_fbProfiles != null)
		{
			if (_latestSuccesfullyConsolidatedFacebookIds == null)
			{
				return true;
			}
			foreach (string key in _fbProfiles.Keys)
			{
				if (!_latestSuccesfullyConsolidatedFacebookIds.Contains(key))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void RecommendAppFacebook()
	{
		if (FacebookSession.HasOpenInstance())
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("link", "http://redirect.kiloo.com/subwayapp.php");
			dictionary.Add("name", "Subway Surfers");
			dictionary.Add("picture", "http://hoodrunner.kiloo.com/fblogo_lasvegas2016.png");
			dictionary.Add("caption", Strings.Get(StringID.FACEBOOK_RECOMMEND_CAPTION));
			Dictionary<string, object> parameters = dictionary;
			FacebookSession.instance.ShowDialog("feed", parameters, delegate(IDictionary<string, object> result, FacebookError error)
			{
				if (result != null && result.ContainsKey("post_id"))
				{
					Flurry.LogEvent("Share link on Facebook");
				}
				else if (error != null)
				{
					DeviceUtility.ShowNativePopup(Strings.Get(StringID.FACEBOOK), Strings.Get(StringID.FACEBOOK_DIALOG_ERROR_MESSAGE), Strings.Get(StringID.OK));
				}
			});
		}
		else
		{
			LogError("Not logged in to facebook");
		}
	}

	public void BragFacebook(List<Friend> friends)
	{
		if (FacebookSession.HasOpenInstance())
		{
			List<Friend> list = null;
			if (friends != null)
			{
				list = new List<Friend>(friends.Count);
				foreach (Friend friend in friends)
				{
					if (friend.fbProfile != null && friend.score < PlayerInfo.Instance.highestScore)
					{
						list.Add(friend);
					}
				}
				list.Sort((Friend x, Friend y) => y.score - x.score);
			}
			string value = ((list == null || list.Count == 0) ? ("I just scored " + PlayerInfo.Instance.highestScore + " points dodging trains in Subway Surfers on my " + GetDeviceTypeString() + ". Check it out!") : ((list.Count == 1) ? ("I just scored " + PlayerInfo.Instance.highestScore + " points in Subway Surfers on my " + GetDeviceTypeString() + " and beat " + list[0].fbProfile.fullName) : ((list.Count == 2) ? ("I just scored " + PlayerInfo.Instance.highestScore + " points in Subway Surfers on my " + GetDeviceTypeString() + " and beat " + list[0].fbProfile.fullName + " and " + list[1].fbProfile.fullName) : ((list.Count != 3) ? ("I just scored " + PlayerInfo.Instance.highestScore + " points in Subway Surfers on my " + GetDeviceTypeString() + " and beat " + list[0].fbProfile.fullName + ", " + list[1].fbProfile.fullName + " and " + (list.Count - 2) + " others") : ("I just scored " + PlayerInfo.Instance.highestScore + " points in Subway Surfers on my " + GetDeviceTypeString() + " and beat " + list[0].fbProfile.fullName + ", " + list[1].fbProfile.fullName + " and " + list[2].fbProfile.fullName)))));
			FacebookSession.instance.ShowDialog("feed", new Dictionary<string, object>
			{
				{ "link", "http://redirect.kiloo.com/subwayapp.php" },
				{ "name", "New Subway Surfers High Score" },
				{ "picture", "http://hoodrunner.kiloo.com/fblogo_lasvegas2016.png" },
				{ "caption", value },
				{ "description", "Download Subway Surfers now" }
			}, delegate(IDictionary<string, object> result, FacebookError error)
			{
				if (result.ContainsKey("post_id"))
				{
					Flurry.LogEvent("Social bragged Facebook");
					if (UIScreenController.IsInstanced)
					{
						UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Brag));
					}
				}
				else if (error != null)
				{
					DeviceUtility.ShowNativePopup(Strings.Get(StringID.FACEBOOK), Strings.Get(StringID.FACEBOOK_DIALOG_ERROR_MESSAGE), Strings.Get(StringID.OK));
				}
			});
		}
		else
		{
			LogError("Not logged in to facebook");
		}
	}

	private IEnumerator DownloadPictureForProfile(FacebookProfile profile, SocialPictureSize pictureSize, Action<FacebookProfile, Texture2D> handler)
	{
		if (profile == null)
		{
			LogError("facebook profile was null in DownloadFacebookPictures!");
			yield break;
		}
		string url = ((pictureSize != SocialPictureSize.Normal) ? ("http://graph.facebook.com/" + profile.id + "/picture?width=300&height=300") : ("http://graph.facebook.com/" + profile.id + "/picture?type=square"));
		WWW www = new WWW(url);
		yield return www;
		if (www.error != null)
		{
		}
		if (handler != null)
		{
			handler(profile, www.texture);
		}
	}

	private void OnDownloadedFacebookPicture(FacebookProfile profile, Texture2D picture)
	{
		if (!(picture == null) && (picture.width != 8 || picture.height != 8))
		{
			profile.image = picture;
			if (_facebookPictureDownloadedHandler != null)
			{
				_facebookPictureDownloadedHandler(profile);
			}
		}
	}

	private IEnumerator DownloadFacebookPictures(IDictionary<string, FacebookProfile> fbProfiles)
	{
		int downloadFacebookPicturesContext = ++_downloadFacebookPicturesContext;
		List<FacebookProfile> profiles = new List<FacebookProfile>(fbProfiles.Count);
		foreach (FacebookProfile profile in fbProfiles.Values)
		{
			if (profile.image == null)
			{
				profiles.Add(profile);
			}
		}
		foreach (FacebookProfile profile2 in profiles)
		{
			if (profile2 != null)
			{
				yield return StartCoroutine(DownloadPictureForProfile(profile2, SocialPictureSize.Normal, OnDownloadedFacebookPicture));
				while (pausePhotoDownloading && downloadFacebookPicturesContext == _downloadFacebookPicturesContext)
				{
					yield return null;
				}
				if (downloadFacebookPicturesContext != _downloadFacebookPicturesContext)
				{
					break;
				}
			}
		}
	}

	public void Invalidate()
	{
		if ((Application.platform == RuntimePlatform.IPhonePlayer && !SocialManagerIOS.instance.IsDoneAuthenticating()) || RemoteNotificationControl.GetDeviceToken() == null || (HasFacebookAccessToken() && !_fbReady))
		{
			return;
		}
		if (ShouldRegisterUser())
		{
			RegisterUser(delegate
			{
			});
		}
		if (ShouldConsolidateFriends())
		{
			ConsolidateFriends(delegate(bool success)
			{
				if (success)
				{
					if (FacebookSession.HasOpenInstance())
					{
						_consolidationState |= 1;
					}
					if (SocialManagerIOS.instance.gameCenterState == SocialManagerIOS.GCState.LoggedIn)
					{
						_consolidationState |= 2;
					}
				}
				_consolidatedFriendsCompleted = true;
				_lastConsolidateFriendCompleteTime = Time.realtimeSinceStartup;
			});
		}
		else
		{
			Action<bool> friendsConsolidatedHandler = _friendsConsolidatedHandler;
			if (friendsConsolidatedHandler != null)
			{
				friendsConsolidatedHandler(true);
			}
		}
	}

	private bool ShouldRegisterUser()
	{
		if (PlayerPrefs.HasKey("socmanlastregtime"))
		{
			string text = PlayerPrefs.GetString("socmanlastregtime");
			long result;
			if (long.TryParse(text, out result))
			{
				DateTime dateTime = new DateTime(result);
				int value = OnlineSettings.instance.GetValue("social_register_min_seconds", 3600);
				if (DateTime.Now >= dateTime + new TimeSpan(0, 0, value))
				{
					return true;
				}
				string fbid;
				string gcid;
				string deviceToken;
				string bundleVersion;
				string highestScore;
				string registerUserData;
				string timezone;
				string locale;
				FetchRegisterUserParams(out fbid, out gcid, out deviceToken, out bundleVersion, out highestScore, out registerUserData, out timezone, out locale);
				if ((!string.IsNullOrEmpty(fbid) || !string.IsNullOrEmpty(gcid)) && _userid == 0)
				{
					return true;
				}
				string text2 = PlayerPrefs.GetString("socmanlastregdata", string.Empty);
				if (text2 != registerUserData)
				{
					return true;
				}
				return false;
			}
			LogError("SocialManager.ShouldRegisterUser() Invalid ticks in playerprefs: " + text);
			return true;
		}
		return true;
	}

	private bool ShouldConsolidateFriends()
	{
		if (!consolidatedFriendsCompleted)
		{
			return true;
		}
		if (HasUnconsolidatedFacebookFriends())
		{
			return true;
		}
		float num = Time.realtimeSinceStartup - _lastConsolidateFriendCompleteTime;
		int value = OnlineSettings.instance.GetValue("social_consolidate_min_seconds", 3600);
		if (num >= (float)value)
		{
			return true;
		}
		return false;
	}

	public void CollectFriendReward(Friend friend)
	{
		friend.status.gamesCashedIn = friend.games;
		_dirty = true;
	}

	public int CashIn(Friend friend, int max)
	{
		int num = friend.games - friend.status.gamesCashedIn;
		if (num > 0)
		{
			friend.status.gamesCashedIn = friend.games;
			_dirty = true;
			return Mathf.Max(num, max);
		}
		return 0;
	}

	public int CashInAll(int maxPerFriend)
	{
		if (_friends == null)
		{
			return 0;
		}
		int num = 0;
		foreach (Friend friend in _friends)
		{
			num += CashIn(friend, maxPerFriend);
		}
		return num;
	}

	public void WriteTo(Stream stream)
	{
		BinaryWriter binaryWriter = new BinaryWriter(stream);
		binaryWriter.Write((byte)2);
		if (_friendStatus != null)
		{
			binaryWriter.Write(_friendStatus.Count);
			{
				foreach (KeyValuePair<string, Friend.Status> item in _friendStatus)
				{
					binaryWriter.Write(item.Key);
					binaryWriter.Write(item.Value.gamesCashedIn);
					binaryWriter.Write(item.Value.lastPokeTime.ToBinary());
					binaryWriter.Write(item.Value.lastDuelPokeTime.ToBinary());
				}
				return;
			}
		}
		binaryWriter.Write(0);
	}

	public void ReadFrom(Stream stream)
	{
		BinaryReader binaryReader = new BinaryReader(stream);
		switch (binaryReader.ReadByte())
		{
		case 2:
		{
			int num2 = binaryReader.ReadInt32();
			_friendStatus = new Dictionary<string, Friend.Status>(num2);
			for (int j = 0; j < num2; j++)
			{
				string text2 = binaryReader.ReadString();
				if (!string.IsNullOrEmpty(text2))
				{
					Friend.Status status2 = new Friend.Status();
					status2.gamesCashedIn = binaryReader.ReadInt32();
					status2.lastPokeTime = DateTime.FromBinary(binaryReader.ReadInt64());
					status2.lastDuelPokeTime = DateTime.FromBinary(binaryReader.ReadInt64());
					_friendStatus[text2] = status2;
				}
			}
			break;
		}
		case 1:
		{
			int num = binaryReader.ReadInt32();
			_friendStatus = new Dictionary<string, Friend.Status>(num);
			for (int i = 0; i < num; i++)
			{
				string text = binaryReader.ReadString();
				if (!string.IsNullOrEmpty(text))
				{
					Friend.Status status = new Friend.Status();
					status.gamesCashedIn = binaryReader.ReadInt32();
					status.lastPokeTime = DateTime.FromBinary(binaryReader.ReadInt64());
					_friendStatus[text] = status;
				}
			}
			break;
		}
		default:
			throw new IOException("Unsupported playerdata file version");
		}
	}

	private static string GetSaveDataPath()
	{
		return DeviceUtility.GetUserDataPath() + "/socialdata";
	}

	public void Load()
	{
		try
		{
			string saveDataPath = GetSaveDataPath();
			byte[] buffer = FileUtil.Load(saveDataPath, "resxrctrv7tgv7gb8h9h9u0909kllfmolkjnhghgjjkhjghg");
			MemoryStream memoryStream = new MemoryStream(buffer);
			ReadFrom(memoryStream);
			memoryStream.Close();
			_dirty = false;
		}
		catch (FileNotFoundException)
		{
		}
		catch (Exception ex2)
		{
			LogError("Could not load data: " + ex2.Message);
		}
	}

	public void SaveIfDirty()
	{
		if (_dirty)
		{
			Save();
		}
	}

	public bool Save()
	{
		try
		{
			MemoryStream memoryStream = new MemoryStream(8192);
			WriteTo(memoryStream);
			byte[] buffer = memoryStream.GetBuffer();
			FileUtil.Save(GetSaveDataPath(), buffer, "resxrctrv7tgv7gb8h9h9u0909kllfmolkjnhghgjjkhjghg", 0, (int)memoryStream.Length);
			memoryStream.Close();
			_dirty = false;
			return true;
		}
		catch (Exception ex)
		{
			LogError("Error saving social data: " + ex.GetType().Name + ": " + ex.Message + "\n" + ex.StackTrace, this);
		}
		return false;
	}

	private void OnRemoteNotificationsEnabled(string deviceToken, string oldDeviceToken)
	{
		if (Application.platform != RuntimePlatform.IPhonePlayer && !string.IsNullOrEmpty(oldDeviceToken))
		{
			Flurry.LogEvent("Old Device Token Unregistered");
			FireAndForgetUnregisterDevice(oldDeviceToken);
		}
		Invalidate();
	}

	[UsedImplicitly]
	private void Awake()
	{
		if (_instance != null)
		{
			LogError("SocialManager ERROR: Instance already set - has the script been added directly to a GameObject?", this);
			UnityEngine.Object.Destroy(this);
			return;
		}
		_instance = this;
		Load();
		CreateFacebookSessionIfNeeded();
		if (PlayerInfo.Instance.remoteNotificationsEnabled)
		{
			RemoteNotificationControl.Enable(OnRemoteNotificationsEnabled, "cpush.wav");
		}
		AddFacebookPictureDownloadedHandler(OnFacebookPhotoDownloaded);
		if (HasFacebookAccessToken())
		{
			FacebookLogin(null);
		}
		Invalidate();
	}

	[UsedImplicitly]
	private void OnApplicationFocus(bool focus)
	{
		_appHasFocus = focus;
	}

	[UsedImplicitly]
	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			SaveIfDirty();
			return;
		}
		if (HasFacebookAccessToken())
		{
			FacebookLogin(null);
		}
		if (GameCenter.GetLocalPlayer().authenticated)
		{
		}
		if (PlayerInfo.Instance.remoteNotificationsEnabled && string.IsNullOrEmpty(RemoteNotificationControl.GetDeviceToken()))
		{
			RemoteNotificationControl.Enable(OnRemoteNotificationsEnabled, "cpush.wav");
		}
	}

	[UsedImplicitly]
	private void OnApplicationQuit()
	{
		_applicationIsQuiting = true;
	}

	private void InitAchievements()
	{
		if (!GameCenter.GetLocalPlayer().authenticated)
		{
			return;
		}
		int context = ++_initAchievementsContext;
		GameCenter.LoadAchievements(delegate(GameCenter.Achievement[] achievements, string error)
		{
			if (context == _initAchievementsContext && achievements != null)
			{
				if (_achievements == null)
				{
					_achievements = new Dictionary<string, GameCenter.Achievement>();
				}
				foreach (GameCenter.Achievement achievement in achievements)
				{
					_achievements[achievement.identifier] = achievement;
				}
			}
		});
	}

	public void ReportAchievement(string achievementId, float percentCompleted)
	{
		if (percentCompleted < 0f || percentCompleted > 100f)
		{
			LogError("SocialManager.ReportAchievement: percentCompleted has invalid value: " + percentCompleted + " (must be >= 0  and <= 100)");
		}
		else
		{
			if (!GameCenter.GetLocalPlayer().authenticated)
			{
				return;
			}
			bool flag = true;
			GameCenter.Achievement value;
			if (_achievements != null && _achievements.TryGetValue(achievementId, out value))
			{
				flag = percentCompleted > value.percentComplete;
			}
			if (!flag)
			{
				return;
			}
			GameCenter.ReportAchievement(achievementId, percentCompleted, false, delegate(string error)
			{
				if (error == null && _achievements != null)
				{
					GameCenter.Achievement value2;
					if (!_achievements.TryGetValue(achievementId, out value2))
					{
						value2 = new GameCenter.Achievement
						{
							identifier = achievementId
						};
						_achievements[achievementId] = value2;
					}
					value2.percentComplete = percentCompleted;
					value2.lastReportedDate = DateTime.Now;
				}
			});
		}
	}

	private static string GetRandomIdentifier()
	{
		return Guid.NewGuid().ToString();
	}

	public static string GetChecksum(string data)
	{
		return StringUtility.ToHexString(Checksum.ComputeSHA1(data + "resxrctrv7tgv7gb8h9h9u0909kllfmolkjnhghgjjkhjghg"), true);
	}

	private static string GetChecksum(params string[] data)
	{
		return GetChecksum(string.Join(null, data));
	}

	public static IEnumerator WWWRequestCoroutine(WWWComplete onWWWComplete, string relativeUrl, object cookie, params string[] postItems)
	{
		string url = "http://hoodrunner.kiloo.com" + relativeUrl;
		string identifier = GetRandomIdentifier();
		StringBuilder checksumSB = new StringBuilder();
		for (int i = 0; i < postItems.Length; i += 2)
		{
			if (postItems[i] == null)
			{
				LogError("WWWRequestCoroutine: Post item key " + i / 2 + " is null, excluding item");
				continue;
			}
			if (postItems[i + 1] == null)
			{
				LogError("WWWRequestCoroutine: Value for Post item " + postItems[i] + " is null, using empty string");
				postItems[i + 1] = string.Empty;
			}
			checksumSB.Append(postItems[i + 1]);
		}
		string checksum = GetChecksum(identifier + checksumSB);
		WWWForm postData = new WWWForm();
		postData.AddField("identifier", identifier);
		postData.AddField("checksum", checksum);
		StringBuilder sb = new StringBuilder();
		sb.Append("WWWRequest(").Append(url).Append(")\n");
		for (int j = 0; j < postItems.Length; j += 2)
		{
			sb.Append("Adding post data: \"").Append(postItems[j]).Append("\" = \"")
				.Append(postItems[j + 1])
				.Append("\"\n");
			if (postItems[j] != null)
			{
				postData.AddField(postItems[j], postItems[j + 1]);
			}
		}
		WWW www = new WWW(url, postData);
		yield return www;
		if (www.error != null)
		{
			sb.Append("Error: \"").Append(www.error).Append("\"\n");
		}
		else if (www.text != null)
		{
			sb.Append("Text: \"").Append(www.text).Append("\"\n");
		}
		if (onWWWComplete == null)
		{
			yield break;
		}
		if (www.error != null)
		{
			onWWWComplete(WWWRequestResult.Error, null, cookie);
			yield break;
		}
		string result = null;
		if (www.text != null)
		{
			int resultStart = www.text.IndexOf("<result>", StringComparison.OrdinalIgnoreCase);
			if (resultStart >= 0)
			{
				resultStart += 8;
				int resultEnd = www.text.IndexOf("</result>", resultStart, StringComparison.OrdinalIgnoreCase);
				if (resultEnd > resultStart)
				{
					result = www.text.Substring(resultStart, resultEnd - resultStart);
				}
				else if (resultEnd == resultStart)
				{
					result = string.Empty;
				}
			}
		}
		onWWWComplete((result == null) ? WWWRequestResult.Error : WWWRequestResult.Success, result, cookie);
	}

	private void FireAndForgetUnregisterDevice(string token)
	{
		if (!string.IsNullOrEmpty(token))
		{
			StartCoroutine(WWWRequestCoroutine(null, "/unregisterdevice.php?android", null, "devicetoken", token));
		}
	}

	private void UnregisterDevice(Action<bool> completionHandler)
	{
		if (canUnregisterDevice)
		{
			if (PlayerInfo.Instance.remoteNotificationsEnabled)
			{
				string deviceToken = RemoteNotificationControl.GetDeviceToken();
				if (!string.IsNullOrEmpty(deviceToken))
				{
					canUnregisterDevice = false;
					requestInvolvingDevicetokenInProgress = true;
					StartCoroutine(WWWRequestCoroutine(WWWUnregisterDeviceCompleted, "/unregisterdevice.php?android", completionHandler, "devicetoken", deviceToken));
					return;
				}
			}
			else
			{
				LogError("SocialManager.UnregisterDevice: Already unregistered");
			}
		}
		else
		{
			LogError("SocialManager.UnregisterDevice: Can't unregister before register.");
		}
		if (completionHandler != null)
		{
			completionHandler(false);
		}
	}

	private void WWWUnregisterDeviceCompleted(WWWRequestResult result, string output, object cookie)
	{
		bool obj = false;
		if (result == WWWRequestResult.Success)
		{
			RemoteNotificationControl.Disable();
			obj = true;
			PlayerPrefs.SetString("socmanlastregdata", string.Empty);
		}
		if (cookie != null)
		{
			((Action<bool>)cookie)(obj);
		}
	}

	private void RegisterUser(Action<bool> registerUserCompleted)
	{
		string fbid;
		string gcid;
		string deviceToken;
		string bundleVersion;
		string highestScore;
		string timezone;
		string locale;
		FetchRegisterUserParams(out fbid, out gcid, out deviceToken, out bundleVersion, out highestScore, out _tempRegisterUserData, out timezone, out locale);
		if (string.IsNullOrEmpty(fbid) && string.IsNullOrEmpty(gcid) && string.IsNullOrEmpty(deviceToken))
		{
			if (registerUserCompleted != null)
			{
				registerUserCompleted(false);
			}
			return;
		}
		PlayerInfo playerInfo = PlayerInfo.Instance;
		StartCoroutine(WWWRequestCoroutine(WWWRegisterUserCompleted, "/register2.php?android", registerUserCompleted, "version", bundleVersion, "gcid", gcid, "fbid", fbid, "devicetoken", deviceToken, "score", highestScore, "meters", "0", "games", "0", "rank", playerInfo.GetCurrentRank().ToString(), "timezone", timezone, "locale", locale));
	}

	private void FetchRegisterUserParams(out string fbid, out string gcid, out string deviceToken, out string bundleVersion, out string highestScore, out string registerUserData, out string timezone, out string locale)
	{
		fbid = ((_fbProfile != null) ? _fbProfile.id : string.Empty);
		bundleVersion = DeviceUtility.GetBundleVersion();
		highestScore = PlayerInfo.Instance.highestScore.ToString();
		timezone = DateTime.Now.GetCurrentTimeZoneUTCOffset().TotalHours.ToString(CultureInfo.InvariantCulture);
		locale = DeviceUtility.GetLocale();
		deviceToken = RemoteNotificationControl.GetDeviceToken();
		gcid = string.Empty;
		registerUserData = fbid + gcid + deviceToken + bundleVersion + highestScore;
	}

	private void WWWRegisterUserCompleted(WWWRequestResult result, string output, object cookie)
	{
		bool flag = false;
		if (result == WWWRequestResult.Success)
		{
			flag = true;
			canUnregisterDevice = true;
			requestInvolvingDevicetokenInProgress = false;
			IDictionary<string, string> dictionary = StringUtility.ParseProperties(output);
			if (dictionary.ContainsKey("userid"))
			{
				string text = dictionary["userid"];
				string text2 = dictionary["score"];
				string text3 = dictionary["meters"];
				string text4 = dictionary["games"];
				string text5 = dictionary["rank"];
				string a = dictionary["checksum"];
				string checksum = GetChecksum(text, text2, text3, text4, text5);
				if (string.Equals(a, checksum, StringComparison.InvariantCultureIgnoreCase))
				{
					try
					{
						int userid = int.Parse(text);
						int score = int.Parse(text2);
						int highestMeters = int.Parse(text3);
						_userid = userid;
						PlayerInfo.Instance.highestMeters = highestMeters;
						PlayerInfo.Instance.ResetHighestScoreTo(score);
					}
					catch (Exception)
					{
						LogError("Error parsing output data from register user");
						flag = false;
					}
				}
				else
				{
					LogError("Output data from register user corrupted or tampered with");
					flag = false;
				}
			}
		}
		if (flag && !string.IsNullOrEmpty(_tempRegisterUserData))
		{
			string value = DateTime.Now.Ticks.ToString();
			PlayerPrefs.SetString("socmanlastregtime", value);
			PlayerPrefs.SetString("socmanlastregdata", _tempRegisterUserData);
			_tempRegisterUserData = null;
		}
		_registerUserCompleted = true;
		if (cookie != null)
		{
			((Action<bool>)cookie)(flag);
		}
		Action<bool> action = this.onRegisterUserCompleted;
		if (action != null)
		{
			action(flag);
		}
	}

	private string GetFBListAsStringAndSaveAsPreConsolidated()
	{
		if (_fbProfiles != null)
		{
			if (_latestPreConsolidationFacebookIds == null)
			{
				_latestPreConsolidationFacebookIds = new List<string>();
			}
			else
			{
				_latestPreConsolidationFacebookIds.Clear();
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string key in _fbProfiles.Keys)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(';');
				}
				stringBuilder.Append(key);
				_latestPreConsolidationFacebookIds.Add(key);
			}
			return stringBuilder.ToString();
		}
		return string.Empty;
	}

	private void ConsolidateFriends(Action<bool> consolidateFriendsCompleted)
	{
		string fBListAsStringAndSaveAsPreConsolidated = GetFBListAsStringAndSaveAsPreConsolidated();
		string empty = string.Empty;
		if (string.IsNullOrEmpty(fBListAsStringAndSaveAsPreConsolidated) && string.IsNullOrEmpty(empty))
		{
			if (consolidateFriendsCompleted != null)
			{
				consolidateFriendsCompleted(true);
			}
			FriendConsolidationDidFinish(true);
		}
		else
		{
			StartCoroutine(WWWRequestCoroutine(WWWConsolidateFriendsCompleted, "/friends2.php?android", consolidateFriendsCompleted, "fblist", fBListAsStringAndSaveAsPreConsolidated, "gclist", empty));
		}
	}

	private void WWWConsolidateFriendsCompleted(WWWRequestResult result, string output, object cookie)
	{
		bool flag = false;
		if (result == WWWRequestResult.Success)
		{
			IDictionary<string, string> dictionary = StringUtility.ParseProperties(output);
			if (dictionary.ContainsKey("friendslist"))
			{
				string text = dictionary["friendslist"];
				string a = dictionary["checksum"];
				string checksum = GetChecksum(text);
				if (string.Equals(a, checksum, StringComparison.InvariantCultureIgnoreCase))
				{
					if (string.IsNullOrEmpty(text))
					{
						_friends = null;
					}
					else
					{
						string[][] array = StringUtility.ParseSets(text);
						_friends = new List<Friend>(array.Length);
						string[][] array2 = array;
						foreach (string[] array3 in array2)
						{
							if (array3.Length >= 6 && (array3[1].Length > 0 || array3[2].Length > 0))
							{
								try
								{
									Friend friend = new Friend();
									friend.userid = int.Parse(array3[0]);
									string text2 = array3[2];
									if (text2.Length > 0)
									{
										FacebookProfile value = null;
										if (_fbProfiles != null)
										{
											_fbProfiles.TryGetValue(text2, out value);
										}
										friend.fbProfile = value;
									}
									friend.score = int.Parse(array3[3]);
									friend.meters = int.Parse(array3[4]);
									friend.games = int.Parse(array3[5]);
									friend.rank = ((array3.Length >= 7) ? int.Parse(array3[6]) : 0);
									Friend.Status status = null;
									if (_friendStatus == null)
									{
										_friendStatus = new Dictionary<string, Friend.Status>();
									}
									if (friend.fbProfile != null && _friendStatus.ContainsKey(friend.fbProfile.id))
									{
										status = _friendStatus[friend.fbProfile.id];
									}
									else if (friend.gcProfile != null && _friendStatus.ContainsKey(friend.gcProfile.id))
									{
										status = _friendStatus[friend.gcProfile.id];
									}
									else
									{
										status = new Friend.Status();
										status.gamesCashedIn = friend.games;
										string key = ((friend.fbProfile == null) ? friend.gcProfile.id : friend.fbProfile.id);
										_friendStatus[key] = status;
										_dirty = true;
									}
									friend.status = status;
									_friends.Add(friend);
								}
								catch (Exception ex)
								{
									LogError("Friend parse error " + ex.ToString());
								}
							}
							else
							{
								LogError("Malformed friend: (" + string.Join(";", array3) + ")");
							}
						}
					}
					if (Flurry.ShouldLogDailyEvent("Social Friends Consolidated"))
					{
						int count = 0;
						int num = 0;
						int num2 = 0;
						if (_friends != null)
						{
							count = _friends.Count;
							foreach (Friend friend2 in _friends)
							{
								if (friend2.fbProfile != null)
								{
									num++;
								}
								if (friend2.gcProfile != null)
								{
									num2++;
								}
							}
						}
						string text3 = Flurry.ConvertFriendCountToBracket(count);
						string text4 = Flurry.ConvertFriendCountToBracket(num);
						string text5 = Flurry.ConvertFriendCountToBracket(num2);
						Flurry.LogEventWithSeveralParameters("Social Friends Consolidated", "Total;Facebook;GameCenter", text3 + ";" + text4 + ";" + text5);
					}
					if (_latestPreConsolidationFacebookIds != null)
					{
						_latestSuccesfullyConsolidatedFacebookIds = new List<string>(_latestPreConsolidationFacebookIds);
					}
					flag = true;
				}
				else
				{
					LogError("SocialManager: Consolidated friend data corrupted");
				}
			}
		}
		if (cookie != null)
		{
			((Action<bool>)cookie)(flag);
		}
		FriendConsolidationDidFinish(flag);
	}

	public void ReportScore(int newScore, bool isNewHighscore, int newMeters)
	{
		if (_userid > 0)
		{
			int num = PlayerInfo.Instance.IncrementUnreportedGames();
			bool flag = false;
			if (isNewHighscore)
			{
				flag = true;
			}
			else if (num >= OnlineSettings.instance.GetValue("social_report_games_count", 5))
			{
				flag = true;
			}
			else if (num > 1 && DateTime.Now > PlayerInfo.Instance.GetFirstUnreportedGameTimestamp() + GetForceReportScoreTimeSpan())
			{
				flag = true;
			}
			if (flag)
			{
				StartCoroutine(WWWRequestCoroutine(WWWReportScoreCompleted, "/report2.php?android", null, "userid", _userid.ToString(), "score", newScore.ToString(), "games", num.ToString()));
			}
		}
	}

	private void WWWReportScoreCompleted(WWWRequestResult result, string output, object cookie)
	{
		if (result == WWWRequestResult.Success)
		{
			PlayerInfo.Instance.ClearUnreportedGames();
		}
	}

	private TimeSpan GetForceReportScoreTimeSpan()
	{
		int num = OnlineSettings.instance.GetValue("social_report_min_seconds", 1200);
		if (num < 0)
		{
			num = 1200;
		}
		return new TimeSpan(0, 0, num);
	}

	public void UpdateFriendScores(Action<bool> updateFriendsScoresCompleted)
	{
		if (_friends == null || _friends.Count == 0)
		{
			return;
		}
		DateTime now = DateTime.Now;
		double totalSeconds = (now - _lastFriendScoreUpdateTimestamp).TotalSeconds;
		if (totalSeconds < (double)OnlineSettings.instance.GetValue("social_friendscores_min_seconds", 120))
		{
			return;
		}
		_lastFriendScoreUpdateTimestamp = now;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (Friend friend in _friends)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(';');
			}
			stringBuilder.Append(friend.userid);
		}
		string text = stringBuilder.ToString();
		StartCoroutine(WWWRequestCoroutine(WWWUpdateFriendScoresCompleted, "/scores2.php?android", updateFriendsScoresCompleted, "idlist", text));
	}

	private void WWWUpdateFriendScoresCompleted(WWWRequestResult result, string output, object cookie)
	{
		bool obj = false;
		if (result == WWWRequestResult.Success)
		{
			IDictionary<string, string> dictionary = StringUtility.ParseProperties(output);
			if (dictionary.ContainsKey("scores"))
			{
				string text = dictionary["scores"];
				string a = dictionary["checksum"];
				string checksum = GetChecksum(text);
				if (string.Equals(a, checksum, StringComparison.InvariantCultureIgnoreCase))
				{
					try
					{
						string[][] array = StringUtility.ParseSets(text);
						string[][] array2 = array;
						foreach (string[] array3 in array2)
						{
							if (array3.Length >= 4)
							{
								int userid = int.Parse(array3[0]);
								Friend friend = _friends.Find((Friend f) => f.userid == userid);
								if (friend != null)
								{
									friend.score = int.Parse(array3[1]);
									friend.meters = int.Parse(array3[2]);
									friend.games = int.Parse(array3[3]);
									friend.rank = ((array3.Length >= 5) ? int.Parse(array3[4]) : 0);
								}
								else
								{
									LogWarning("UpdateFriendScores: Unexpected friend user id");
								}
								continue;
							}
							LogError("UpdateFriendScores: Malformed score (" + string.Join(";", array3) + ")");
							throw new Exception();
						}
						obj = true;
					}
					catch (Exception)
					{
						LogError("UpdateFriendScores: Error parsing output data");
					}
				}
				else
				{
					LogError("UpdateFriendScores: Output data corrupt");
				}
			}
		}
		if (cookie != null)
		{
			((Action<bool>)cookie)(obj);
		}
	}

	public string GetNameForRelation(Friend friend)
	{
		if (friend.fbProfile != null)
		{
			return (facebookProfile == null) ? null : facebookProfile.fullName;
		}
		return (gamecenterProfile == null) ? null : gamecenterProfile.alias;
	}

	public void Poke(Friend friend, PokeType pokeType)
	{
		MissionsManager.Instance.PlayerDidThis(MissionTarget.PokeFriend);
		string nameForRelation = GetNameForRelation(friend);
		if (pokeType == PokeType.Duel)
		{
			friend.status.lastDuelPokeTime = DateTime.UtcNow;
			Flurry.LogEvent("Social duel poked");
		}
		else
		{
			friend.status.lastPokeTime = DateTime.UtcNow;
			Flurry.LogEvent("Social friend poked");
		}
		_dirty = true;
		StartCoroutine(WWWRequestCoroutine(null, "/poke.php?android", null, "friend", friend.userid.ToString(), "name", nameForRelation));
	}

	public void SetPokeFirstTime(Friend friend)
	{
		friend.status.lastPokeTime = DateTime.UtcNow;
		_dirty = true;
	}

	public void BragNotify(int oldScore, List<Friend> friends)
	{
		if (friends == null)
		{
			return;
		}
		int count = friends.Count;
		StringBuilder stringBuilder = new StringBuilder(count * 8);
		StringBuilder stringBuilder2 = new StringBuilder(count * 2);
		foreach (Friend friend in friends)
		{
			int relation = friend.relation;
			int userid = friend.userid;
			if (relation != 0 && userid != 0)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(';');
					stringBuilder2.Append(';');
				}
				stringBuilder.Append(userid);
				stringBuilder2.Append(relation);
			}
		}
		if (stringBuilder.Length > 0)
		{
			string text = ((_fbProfile == null) ? string.Empty : _fbProfile.fullName);
			GameCenter.LocalPlayer localPlayer = GameCenter.GetLocalPlayer();
			string text2 = ((!localPlayer.authenticated) ? string.Empty : localPlayer.alias);
			StartCoroutine(WWWRequestCoroutine(null, "/brag.php?android", null, "oldscore", oldScore.ToString(), "newscore", PlayerInfo.Instance.highestScore.ToString(), "useridlist", stringBuilder.ToString(), "relationlist", stringBuilder2.ToString(), "fbname", text, "gcname", text2));
			Flurry.LogEvent("Social bragged");
		}
	}

	public void AnnounceToFriends(int relationsMask)
	{
		if (_friends == null)
		{
			return;
		}
		int count = _friends.Count;
		StringBuilder stringBuilder = new StringBuilder(count * 8);
		StringBuilder stringBuilder2 = new StringBuilder(count * 2);
		foreach (Friend friend in _friends)
		{
			int num = friend.relation & relationsMask;
			int userid = friend.userid;
			if (num != 0 && userid != 0)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(';');
					stringBuilder2.Append(';');
				}
				stringBuilder.Append(userid);
				stringBuilder2.Append(num);
			}
		}
		if (stringBuilder.Length > 0)
		{
			string text;
			string text2;
			if (_fbProfile != null && (relationsMask & 1) != 0)
			{
				text = _fbProfile.fullName;
				text2 = _fbProfile.id;
			}
			else
			{
				text = string.Empty;
				text2 = string.Empty;
			}
			GameCenter.LocalPlayer localPlayer = GameCenter.GetLocalPlayer();
			string text3;
			string text4;
			if (localPlayer.authenticated && (relationsMask & 2) != 0)
			{
				text3 = localPlayer.alias;
				text4 = localPlayer.playerID;
			}
			else
			{
				text3 = string.Empty;
				text4 = string.Empty;
			}
			StartCoroutine(WWWRequestCoroutine(null, "/announce.php?android", null, "useridlist", stringBuilder.ToString(), "relationlist", stringBuilder2.ToString(), "fbname", text, "gcname", text3, "fbid", text2, "gcid", text4));
		}
	}

	private static string GetDeviceTypeString()
	{
		return "Android Device";
	}

	public void UpdateRemoteNotificationControl(bool newValue)
	{
		if (PlayerInfo.Instance.remoteNotificationsEnabled && !newValue)
		{
			if (canUnregisterDevice)
			{
				UnregisterDevice(delegate(bool success)
				{
					requestInvolvingDevicetokenInProgress = false;
					Debug.Log("SocialManager Unregister device success: " + success);
				});
			}
		}
		else
		{
			requestInvolvingDevicetokenInProgress = true;
			RemoteNotificationControl.Enable(OnRemoteNotificationsEnabled, "cpush.wav");
		}
	}

	public static void LogError(string msg, UnityEngine.Object context = null)
	{
		Debug.LogError(msg, context);
	}

	public static void LogWarning(string msg, UnityEngine.Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("SOCIAL_DEBUG_LOGS")]
	public static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}
}
