#define ENABLE_ERROR_LOGS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Kiloo.Plugins.GameCenter;
using SYBO.Core.Utilities;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SocialManagerIOS : MonoBehaviour
{
	public enum GCState
	{
		LoggedOut = 0,
		Authenticating = 1,
		LoadingFriends = 2,
		LoggedIn = 3
	}

	private const string ONLINESETTINGS_FILTER_GC_FRIENDS_KEY = "social_filter_gc_friends";

	private const bool FILTER_GC_FRIENDS_USING_LEADERBOARD_DEFAULT = true;

	private const string GC_LEADERBOARD_ID = "com.kiloo.subwaysurfers.ScoreLeaderboard";

	private static SocialManagerIOS _instance;

	private GCState _gcState;

	private Dictionary<string, GameCenterProfile> _gcFriends;

	private Action<GameCenterProfile> _playerPhotoDownloadedHandler;

	private bool _pausePhotoDownloading;

	private int _photoDownloadContext;

	private List<string> _latestPreConsolidationGCIds;

	private List<string> _latestSuccesfullyConsolidationGCIds;

	private Action _gameCenterStateChanged;

	private static bool _applicationIsQuiting;

	private int _loadFriendsContext;

	private GameCenter.ShowViewState _lastViewState;

	private string _lastLeaderboardCategory = "com.kiloo.subwaysurfers.ScoreLeaderboard";

	private GameCenter.LeaderboardTimeScope _lastTimeScope;

	private Action _invalidate;

	public GCState gameCenterState
	{
		get
		{
			return _gcState;
		}
		set
		{
			if (_gcState != value)
			{
				_gcState = value;
				Action gameCenterStateChanged = _gameCenterStateChanged;
				if (gameCenterStateChanged != null)
				{
					gameCenterStateChanged();
				}
			}
		}
	}

	public bool pausePhotoDownloading
	{
		get
		{
			return _pausePhotoDownloading;
		}
		set
		{
			_pausePhotoDownloading = value;
		}
	}

	public static SocialManagerIOS instance
	{
		get
		{
			Init();
			return _instance;
		}
	}

	public static bool isInstanced
	{
		get
		{
			return _instance != null;
		}
	}

	public void AddGameCenterStateChangedHandler(Action handler)
	{
		_gameCenterStateChanged = (Action)Delegate.Combine(_gameCenterStateChanged, handler);
	}

	public void RemoveGameCenterStateChangedHandler(Action handler)
	{
		_gameCenterStateChanged = (Action)Delegate.Remove(_gameCenterStateChanged, handler);
	}

	private void OnApplicationQuit()
	{
		_applicationIsQuiting = true;
	}

	private static void Init()
	{
		if (_instance == null && !_applicationIsQuiting)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "SocialManagerIOS";
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<SocialManagerIOS>();
			HierarchyUtility.ReparentByHierarchyPathType(gameObject.transform, HierarchyPathType.DynamicManagers);
		}
	}

	private void Awake()
	{
		_instance = this;
	}

	[Conditional("UNITY_IPHONE")]
	public void LoadFriends()
	{
		if (GameCenter.GetLocalPlayer().authenticated)
		{
			int context = ++_loadFriendsContext;
			gameCenterState = GCState.LoadingFriends;
			GameCenter.LoadLeaderboard("com.kiloo.subwaysurfers.ScoreLeaderboard", GameCenter.LeaderboardPlayerScope.FriendsOnly, GameCenter.LeaderboardTimeScope.AllTime, 1, 100, delegate(GameCenter.Leaderboard leaderboard, string leaderboardError)
			{
				if (context == _loadFriendsContext)
				{
					if (leaderboard != null)
					{
						Dictionary<string, GameCenterProfile> dictionary = new Dictionary<string, GameCenterProfile>(leaderboard.scores.Length);
						GameCenter.Score localPlayerScore = leaderboard.localPlayerScore;
						long value = localPlayerScore.value;
						if (value < PlayerInfo.Instance.highestScore)
						{
						}
						int num = 0;
						GameCenter.Score[] scores = leaderboard.scores;
						for (int i = 0; i < scores.Length; i++)
						{
							GameCenter.Score score = scores[i];
							if (score.playerID != localPlayerScore.playerID)
							{
								GameCenterProfile value2;
								if (_gcFriends == null || !_gcFriends.TryGetValue(score.playerID, out value2))
								{
									value2 = new GameCenterProfile
									{
										id = score.playerID
									};
								}
								dictionary[score.playerID] = value2;
								num++;
							}
						}
						_gcFriends = dictionary;
						Dictionary<string, GameCenterProfile>.KeyCollection keys = _gcFriends.Keys;
						string[] array = new string[keys.Count];
						int num2 = 0;
						foreach (string item in keys)
						{
							array[num2++] = item;
						}
						GameCenter.LoadPlayers(array, delegate(GameCenter.Player[] players, string playersError)
						{
							if (context == _loadFriendsContext)
							{
								if (players != null)
								{
									for (int j = 0; j < players.Length; j++)
									{
										GameCenter.Player player = players[j];
										GameCenterProfile value3;
										if (_gcFriends != null && _gcFriends.TryGetValue(player.playerID, out value3))
										{
											value3.alias = player.alias;
											value3.displayName = player.displayName;
										}
									}
								}
								StartCoroutine(RefreshFriendImages(_gcFriends));
							}
							else
							{
								LogWarning("SocialManagerIOS.LoadFriends: Loading players complete but was started in another context.");
							}
						});
					}
					else
					{
						_gcFriends = null;
					}
				}
				else
				{
					LogWarning("SocialManagerIOS.LoadFriends: Loading leaderboard complete but was started in another context.");
				}
			});
		}
		else
		{
			LogError("SocialManagerIOS.LoadFriends: Not authenticated");
		}
	}

	private IEnumerator RefreshFriendImages(Dictionary<string, GameCenterProfile> gcProfiles)
	{
		int photoDownloadContext = ++_photoDownloadContext;
		List<GameCenterProfile> profiles = new List<GameCenterProfile>(gcProfiles.Count);
		foreach (GameCenterProfile profile in gcProfiles.Values)
		{
			if (profile.image == null)
			{
				profiles.Add(profile);
			}
		}
		foreach (GameCenterProfile profile2 in profiles)
		{
			yield return StartCoroutine(DownloadPictureForProfile(profile2, SocialManager.SocialPictureSize.Normal, delegate(GameCenterProfile prof, Texture2D picture)
			{
				profile2.image = picture;
			}));
			while (pausePhotoDownloading && photoDownloadContext == _photoDownloadContext)
			{
				yield return null;
			}
			if (photoDownloadContext != _photoDownloadContext)
			{
				break;
			}
		}
	}

	public IEnumerator DownloadPictureForProfile(GameCenterProfile profile, SocialManager.SocialPictureSize pictureSize, Action<GameCenterProfile, Texture2D> handler)
	{
		DateTime lastRefresh = profile.refreshedTime;
		if (!(profile.image == null) && !((DateTime.Now - lastRefresh).TotalSeconds > 3600.0))
		{
			yield break;
		}
		int textureDimension = ((pictureSize != SocialManager.SocialPictureSize.Large) ? 50 : 250);
		GameCenterProfile profile2 = default(GameCenterProfile);
		Action<GameCenterProfile, Texture2D> handler2 = default(Action<GameCenterProfile, Texture2D>);
		GameCenter.LoadPhoto(profile.id, GameCenter.PhotoSize.Small, textureDimension, textureDimension, delegate(string playerID, Texture2D photo, string error)
		{
			profile2.refreshedTime = DateTime.Now;
			if (error == null && handler2 != null)
			{
				handler2(profile2, photo);
			}
		});
		while (profile.refreshedTime == lastRefresh)
		{
			yield return null;
		}
	}

	public void AbortAsynchronousProcesses()
	{
		_photoDownloadContext++;
		_loadFriendsContext++;
	}

	[Conditional("UNITY_IPHONE")]
	private void FlurryLogGCFriendsRetrieved(int total, int withGameInstalled)
	{
		if (Flurry.ShouldLogDailyEvent("Social GameCenter Friends Retrieved"))
		{
			string text = Flurry.ConvertFriendCountToBracket(total);
			string text2 = Flurry.ConvertFriendCountToBracket(withGameInstalled);
			Flurry.LogEventWithSeveralParameters("Social GameCenter Friends Retrieved", "Total friends;Installed friends", text + ";" + text2);
		}
	}

	[Conditional("UNITY_IPHONE")]
	private void PerformLoadFriendsDoneActions()
	{
		gameCenterState = GCState.LoggedIn;
		Action invalidate = _invalidate;
		if (invalidate != null)
		{
			invalidate();
		}
	}

	public bool IsDoneAuthenticating()
	{
		return gameCenterState == GCState.LoggedIn || gameCenterState == GCState.LoggedOut;
	}

	[Conditional("UNITY_IPHONE")]
	public void ReportScore(int newScore)
	{
		if (!GameCenter.GetLocalPlayer().authenticated)
		{
			return;
		}
		GameCenter.ReportScore("com.kiloo.subwaysurfers.ScoreLeaderboard", newScore, 0uL, delegate(string error)
		{
			if (error != null)
			{
				LogError("SocialManagerIOS.ReportToGameCenter: Error reporting score to GameCenter");
			}
		});
	}

	[Conditional("UNITY_IPHONE")]
	public void ShowLeaderBoard()
	{
		GameCenter.ShowView(_lastViewState, _lastLeaderboardCategory, _lastTimeScope, delegate(GameCenter.ShowViewState viewState, string leaderboardCategory, GameCenter.LeaderboardTimeScope leaderboardTimescope)
		{
			_lastViewState = viewState;
			_lastLeaderboardCategory = leaderboardCategory;
			_lastTimeScope = leaderboardTimescope;
		});
	}

	public string GetGCListAsStringAndSaveAsPreConsolidated()
	{
		if (_latestPreConsolidationGCIds == null)
		{
			_latestPreConsolidationGCIds = new List<string>();
		}
		else
		{
			_latestPreConsolidationGCIds.Clear();
		}
		string result;
		if (_gcFriends != null)
		{
			Dictionary<string, GameCenterProfile>.KeyCollection keys = _gcFriends.Keys;
			string[] array = new string[keys.Count];
			int num = 0;
			foreach (string item in keys)
			{
				array[num++] = item;
			}
			result = string.Join(";", array);
			_latestPreConsolidationGCIds.AddRange(array);
		}
		else
		{
			result = string.Empty;
		}
		return result;
	}

	public void HandleConsolidatedFriendsSuccess()
	{
		_latestSuccesfullyConsolidationGCIds = _latestPreConsolidationGCIds;
		_latestPreConsolidationGCIds = null;
	}

	public bool HasUnconsolidatedGCFriends()
	{
		if (_gcFriends != null)
		{
			if (_latestSuccesfullyConsolidationGCIds == null)
			{
				return true;
			}
			foreach (string key in _gcFriends.Keys)
			{
				if (!_latestSuccesfullyConsolidationGCIds.Contains(key))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void ClearFriends()
	{
		_latestSuccesfullyConsolidationGCIds = null;
		_latestPreConsolidationGCIds = null;
		_gcFriends = null;
	}

	public bool GCFriendsContainKey(string key)
	{
		return _gcFriends != null && _gcFriends.ContainsKey(key);
	}

	public GameCenterProfile TryGetUserProfileFromKey(string key)
	{
		GameCenterProfile value;
		if (_gcFriends != null && _gcFriends.TryGetValue(key, out value))
		{
			return value;
		}
		return null;
	}

	public void AddInvalidateHandler(Action handler)
	{
		_invalidate = (Action)Delegate.Combine(_invalidate, handler);
	}

	[Conditional("UNITY_IPHONE")]
	public void AddPlayerPhotoDownloadedHandler(Action<GameCenterProfile> handler)
	{
		_playerPhotoDownloadedHandler = (Action<GameCenterProfile>)Delegate.Combine(_playerPhotoDownloadedHandler, handler);
	}

	[Conditional("UNITY_IPHONE")]
	public void RemovePlayerPhotoDownloadedHandler(Action<GameCenterProfile> handler)
	{
		_playerPhotoDownloadedHandler = (Action<GameCenterProfile>)Delegate.Remove(_playerPhotoDownloadedHandler, handler);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogError(string msg, UnityEngine.Object context = null)
	{
		Debug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
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
