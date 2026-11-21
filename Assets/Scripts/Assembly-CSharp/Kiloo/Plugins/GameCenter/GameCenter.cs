using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Plugins.GameCenter
{
	public class GameCenter : MonoBehaviour
	{
		public struct Player
		{
			public string playerID;

			public string alias;

			public string displayName;

			public bool isFriend;
		}

		public struct LocalPlayer
		{
			public string playerID;

			public string alias;

			public string displayName;

			public bool underage;

			public bool authenticated;
		}

		public struct Score
		{
			public string playerID;

			public int rank;

			public long value;

			public string formattedValue;

			public DateTime date;

			public ulong context;
		}

		public class Leaderboard
		{
			public string category;

			public int rangeBegin;

			public int rangeCount;

			public LeaderboardPlayerScope playerScope;

			public LeaderboardTimeScope timeScope;

			public Score localPlayerScore;

			public Score[] scores;

			public string groupIdentifier;

			public uint maxRange;

			public string title;
		}

		public class Achievement
		{
			public string identifier;

			public float percentComplete;

			public DateTime lastReportedDate;

			public bool complete
			{
				get
				{
					return percentComplete >= 100f;
				}
			}
		}

		public class AchievementDescription
		{
			public string identifier;

			public string title;

			public string unachievedDescription;

			public string achievedDescription;

			public int maximumPoints;

			public bool replayable;

			public bool hidden;

			public string groupIdentifier;
		}

		public enum PhotoSize
		{
			Small = 0,
			Normal = 1
		}

		public enum ShowViewState
		{
			Default = -1,
			LeaderBoards = 0,
			Achievements = 1,
			Challenges = 2
		}

		public enum LeaderboardTimeScope
		{
			Today = 0,
			Week = 1,
			AllTime = 2
		}

		public enum LeaderboardPlayerScope
		{
			Global = 0,
			FriendsOnly = 1
		}

		public delegate void AuthenticateHandler(bool hasLoginView, string error);

		public delegate void LoadFriendsCompletionHandler(string[] friends, string error);

		public delegate void LoadPlayersCompletionHandler(Player[] players, string error);

		public delegate void LoadPhotoCompletionHandler(string playerID, Texture2D photo, string error);

		private delegate void InternalLoadPhotoCompletionHandler(int imgWidth, int imgHeight, string error);

		public delegate void ViewDismissedHandler(ShowViewState viewState, string leaderboardCategory, LeaderboardTimeScope leaderboardTimeScope);

		public delegate void RequestCompleteHandler(string error);

		public delegate void LoadLeaderboardCompletionHandler(Leaderboard leaderboard, string error);

		public delegate void LoadAchievementsCompletionHandler(Achievement[] achievements, string error);

		public delegate void LoadAchievementDescriptionsCompletionHandler(AchievementDescription[] achievements, string error);

		private const char SEPARATION_CHAR = '|';

		private const string ESCAPED_SEPARATION_CHAR = "&#124;";

		private bool _hasLoginView;

		private bool _initialAuthenticationComplete;

		private ViewDismissedHandler _viewDismissedHandler;

		private static GameCenter _instance;

		public static bool hasLoginView
		{
			get
			{
				return _instance != null && _instance._hasLoginView;
			}
		}

		public static bool initialAuthenticationComplete
		{
			get
			{
				return _instance != null && _instance._initialAuthenticationComplete;
			}
		}

		public static event AuthenticateHandler authenticationEvent;

		[Conditional("UNITY_IPHONE")]
		private static void _GameCenterInit(string gameObject) { }

		[Conditional("UNITY_IPHONE")]
		private static void _GameCenterShowLoginView() { }

		[Conditional("UNITY_IPHONE")]
		private static void _GameCenterGetLocalPlayer(ref LocalPlayer localPlayer) { }

		[Conditional("UNITY_IPHONE")]
		private static void _GameCenterLoadFriends(int handlerContext) { }

		[Conditional("UNITY_IPHONE")]
		private static void _GameCenterLoadPlayers(string playerIDs, int handlerContext) { }

		[Conditional("UNITY_IPHONE")]
		private static void _GameCenterLoadPhoto(string playerID, int size, int handlerContext) { }

		private static bool _GameCenterArePhotosSupported()
		{
			return false;
		}

		private static bool _GameCenterGetPhotoPixels(int context, int width, int height, IntPtr pixels)
		{
			return false;
		}

		[Conditional("UNITY_IPHONE")]
		private static void _GameCenterShowView(int viewState, string leaderboardCategory, int leaderboardTimeScope) { }

		[Conditional("UNITY_IPHONE")]
		private static void _GameCenterReportScore(string category, long scoreValue, ulong scoreContext, int handlerContext) { }

		[Conditional("UNITY_IPHONE")]
		private static void _GameCenterLoadLeaderboard(string category, int playerScope, int timeScope, int rangeBegin, int rangeCount, int handlerContext) { }

		[Conditional("UNITY_IPHONE")]
		private static void _GameCenterLoadLeaderboardForIDs(string category, string playerIDs, int timeScope, int handlerContext) { }

		[Conditional("UNITY_IPHONE")]
		private static void _GameCenterReportAchievement(string achievementID, float percentComplete, int showsCompletionBanner, int handlerContext) { }

		[Conditional("UNITY_IPHONE")]
		private static void _GameCenterReportAchievements(string interleavedAchievements, int showsCompletionBanner, int handlerContext) { }

		[Conditional("UNITY_IPHONE")]
		private static void _GameCenterLoadAchievements(int handlerContext) { }

		[Conditional("UNITY_IPHONE")]
		private static void _GameCenterLoadAchievementDescriptions(int handlerContext) { }

		[Conditional("UNITY_IPHONE")]
		private static void _GameCenterResetAchievements(int handlerContext) { }

		public static bool IsSupported()
		{
			return Application.platform == RuntimePlatform.IPhonePlayer;
		}

		public static void Init()
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance == null)
				{
					_instance = PluginUtil.CreateBridgeObject<GameCenter>();
				}
			}
			else
			{
				LogWarning("Could not authenticate local player (not on iOS)");
			}
		}

		public static void ShowLoginView()
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					if (_instance._hasLoginView)
					{
						_instance._hasLoginView = false;
					}
				}
				else
				{
					LogError("GameCenter.ShowLoginView was called before Init");
				}
			}
			else
			{
				LogError("Could not show login view (not on iOS)");
			}
		}

		public static LocalPlayer GetLocalPlayer()
		{
			LocalPlayer result = default(LocalPlayer);
			if (Application.platform == RuntimePlatform.IPhonePlayer && result.displayName == null)
			{
				result.displayName = result.alias;
			}
			return result;
		}

		public static void GetLocalPlayer(ref LocalPlayer player)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (player.displayName == null)
				{
					player.displayName = player.alias;
				}
			}
			else
			{
				player = default(LocalPlayer);
			}
		}

		public static void LoadFriends(LoadFriendsCompletionHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = PluginUtil.CheckInHandler(completionHandler, _instance);
				}
				else
				{
					LogError("GameCenter.LoadFriends was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler(null, "Could not load friends (not on iOS)");
			}
		}

		public static void LoadPlayers(string[] playerIDs, LoadPlayersCompletionHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = PluginUtil.CheckInHandler(completionHandler, _instance);
				}
				else
				{
					LogError("GameCenter.LoadPlayers was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler(null, "Could not load players (not on iOS)");
			}
		}

		public static bool ArePhotosSupported()
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					return _GameCenterArePhotosSupported();
				}
				LogError("GameCenter.ArePhotosSupported was called before Init");
			}
			return false;
		}

		public static void LoadPhoto(string playerID, PhotoSize size, int textureWidth, int textureHeight, LoadPhotoCompletionHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int context = 0;
					context = PluginUtil.CheckInHandler((InternalLoadPhotoCompletionHandler)delegate(int imgWidth, int imgHeight, string error)
					{
						Texture2D texture2D = null;
						if (imgWidth != 0 && imgHeight != 0)
						{
							Color32[] array = new Color32[textureWidth * textureHeight];
							GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
							bool flag = _GameCenterGetPhotoPixels(context, textureWidth, textureHeight, gCHandle.AddrOfPinnedObject());
							gCHandle.Free();
							if (flag)
							{
								texture2D = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
								texture2D.wrapMode = TextureWrapMode.Clamp;
								texture2D.SetPixels32(array);
								texture2D.Apply();
							}
						}
						if (completionHandler != null)
						{
							completionHandler(playerID, texture2D, error);
						}
					}, _instance);
				}
				else
				{
					LogError("GameCenter.LoadPhoto was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler(playerID, null, "Could not load photo (not on iOS)");
			}
		}

		public static void ShowView(ShowViewState viewState, string leaderboardCategory = null, LeaderboardTimeScope leaderboardTimeScope = LeaderboardTimeScope.AllTime, ViewDismissedHandler viewDismissedHandler = null)
		{
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				return;
			}
			if (_instance != null)
			{
				if (_instance._viewDismissedHandler != null)
				{
					LogWarning("GameCenter.ShowView called while a view is already shown. Only one dismissed handler can be set at a time.");
				}
				_instance._viewDismissedHandler = viewDismissedHandler;
			}
			else
			{
				LogError("GameCenter.ShowView was called before Init");
			}
		}

		public static void ReportScore(string category, long scoreValue, ulong scoreContext, RequestCompleteHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = PluginUtil.CheckInHandler(completionHandler, _instance);
				}
				else
				{
					LogError("GameCenter.ReportScore was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler("Could not report score (not on iOS)");
			}
		}

		public static void LoadLeaderboard(string category, LeaderboardPlayerScope playerScope, LeaderboardTimeScope timeScope, int rangeBegin, int rangeCount, LoadLeaderboardCompletionHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = PluginUtil.CheckInHandler((LoadLeaderboardCompletionHandler)delegate(Leaderboard leaderboard, string error)
					{
						if (completionHandler != null)
						{
							if (leaderboard != null)
							{
								leaderboard.category = category;
								leaderboard.playerScope = playerScope;
								leaderboard.timeScope = timeScope;
								leaderboard.rangeBegin = rangeBegin;
								leaderboard.rangeCount = rangeCount;
							}
							completionHandler(leaderboard, error);
						}
					}, _instance);
				}
				else
				{
					LogError("GameCenter.LoadLeaderboard was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler(null, "Could not load leaderboard (not on iOS)");
			}
		}

		public static void LoadLeaderboardForIDs(string category, string[] playerIDs, LeaderboardTimeScope timeScope, LoadLeaderboardCompletionHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = PluginUtil.CheckInHandler((LoadLeaderboardCompletionHandler)delegate(Leaderboard leaderboard, string error)
					{
						if (completionHandler != null)
						{
							if (leaderboard != null)
							{
								leaderboard.category = category;
								leaderboard.timeScope = timeScope;
							}
							completionHandler(leaderboard, error);
						}
					}, _instance);
				}
				else
				{
					LogError("GameCenter.LoadLeaderboardForIDs was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler(null, "Could not load leaderboard for ids (not on iOS)");
			}
		}

		public static void ReportAchievement(string identifier, float percentComplete, bool showsCompletionBanner, RequestCompleteHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = PluginUtil.CheckInHandler(completionHandler, _instance);
				}
				else
				{
					LogError("GameCenter.ReportAchievement was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler("Could not report achievement (not on iOS)");
			}
		}

		public static void ReportAchievement(Achievement achievement, bool showsCompletionBanner, RequestCompleteHandler completionHandler)
		{
			ReportAchievement(achievement.identifier, achievement.percentComplete, showsCompletionBanner, completionHandler);
		}

		public static void ReportAchievements(Achievement[] achievements, bool showsCompletionBanner, RequestCompleteHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = PluginUtil.CheckInHandler(completionHandler, _instance);
					StringBuilder stringBuilder = new StringBuilder();
					bool flag = false;
					foreach (Achievement achievement in achievements)
					{
						if (flag)
						{
							stringBuilder.Append('|');
						}
						else
						{
							flag = true;
						}
						stringBuilder.Append(achievement.identifier).Append('|').Append(achievement.percentComplete);
					}
				}
				else
				{
					LogError("GameCenter.ReportAchievements was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler("Could not report achievements (not on iOS)");
			}
		}

		public static void LoadAchievements(LoadAchievementsCompletionHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = PluginUtil.CheckInHandler(completionHandler, _instance);
				}
				else
				{
					LogError("GameCenter.LoadAchievements was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler(null, "Could not load achievements (not on iOS)");
			}
		}

		public static void LoadAchievementDescriptions(LoadAchievementDescriptionsCompletionHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = PluginUtil.CheckInHandler(completionHandler, _instance);
				}
				else
				{
					LogError("GameCenter.LoadAchievementDescriptions was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler(null, "Could not load achievement descriptions (not on iOS)");
			}
		}

		public static void ResetAchievements(RequestCompleteHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = PluginUtil.CheckInHandler(completionHandler, _instance);
				}
				else
				{
					LogError("GameCenter.ResetAchievements was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler("Could not reset achievements (not on iOS)");
			}
		}

		[UsedImplicitly]
		private void OnGameCenterAuthenticationEvent(string param)
		{
			_initialAuthenticationComplete = true;
			AuthenticateHandler authenticateHandler = GameCenter.authenticationEvent;
			if (authenticateHandler != null)
			{
				string[] array = param.Split('|');
				if (array.Length != 2)
				{
					LogError("GameCenter.OnGameCenterAuthenticationEvent: Unexpected number of parameters :" + array.Length);
				}
				_hasLoginView = array[0] == "true";
				string error = ((array[1].Length <= 0) ? null : UnescapeResultString(array[1]));
				authenticateHandler(_hasLoginView, error);
			}
		}

		[UsedImplicitly]
		private void OnGameCenterLoadFriendsComplete(string param)
		{
			string[] array = param.Split('|');
			int num = array.Length;
			if (num < 2)
			{
				LogError("GameCenter.OnGameCenterLoadFriendsComplete: Unexpected number of parameters :" + array.Length);
				return;
			}
			int ticket = int.Parse(array[0]);
			LoadFriendsCompletionHandler loadFriendsCompletionHandler = PluginUtil.CheckOutHandler(ticket) as LoadFriendsCompletionHandler;
			if (loadFriendsCompletionHandler != null)
			{
				string error = ((array[1].Length <= 0) ? null : UnescapeResultString(array[1]));
				string[] array2 = null;
				if (num > 2)
				{
					array2 = new string[num - 2];
					int num2 = 2;
					int num3 = 0;
					while (num2 < num)
					{
						array2[num3] = array[num2];
						num2++;
						num3++;
					}
				}
				loadFriendsCompletionHandler(array2, error);
			}
			else
			{
				LogError("GameCenter.OnGameCenterLoadFriendsComplete: Handler not registered");
			}
		}

		[UsedImplicitly]
		private void OnGameCenterLoadPlayersComplete(string param)
		{
			string[] array = param.Split('|');
			int num = array.Length;
			if (num < 2 || (num - 2) % 4 != 0)
			{
				LogError("GameCenter.OnGameCenterLoadPlayersComplete: Unexpected number of parameters :" + array.Length);
				return;
			}
			int ticket = int.Parse(array[0]);
			LoadPlayersCompletionHandler loadPlayersCompletionHandler = PluginUtil.CheckOutHandler(ticket) as LoadPlayersCompletionHandler;
			if (loadPlayersCompletionHandler != null)
			{
				string error = ((array[1].Length <= 0) ? null : UnescapeResultString(array[1]));
				Player[] array2 = null;
				if (num > 2)
				{
					int num2 = (num - 2) / 4;
					array2 = new Player[num2];
					int num3 = 2;
					for (int i = 0; i < num2; i++)
					{
						array2[i].playerID = array[num3];
						array2[i].alias = UnescapeResultString(array[num3 + 1]);
						array2[i].displayName = UnescapeResultString(array[num3 + 2]);
						array2[i].isFriend = array[num3 + 3] == "true";
						if (array2[i].displayName.Length == 0)
						{
							array2[i].displayName = array2[i].alias;
						}
						num3 += 4;
					}
				}
				loadPlayersCompletionHandler(array2, error);
			}
			else
			{
				LogError("GameCenter.OnGameCenterLoadPlayersComplete: Handler not registered");
			}
		}

		[UsedImplicitly]
		private void OnGameCenterLoadPhotoComplete(string param)
		{
			string[] array = param.Split('|');
			int num = array.Length;
			if (num != 4)
			{
				LogError("GameCenter.OnGameCenterLoadPhotoComplete: Unexpected number of parameters :" + array.Length);
				return;
			}
			int num2 = int.Parse(array[0]);
			InternalLoadPhotoCompletionHandler internalLoadPhotoCompletionHandler = PluginUtil.CheckOutHandler(num2) as InternalLoadPhotoCompletionHandler;
			if (internalLoadPhotoCompletionHandler != null)
			{
				string error = ((array[1].Length <= 0) ? null : UnescapeResultString(array[1]));
				int imgWidth = int.Parse(array[2]);
				int imgHeight = int.Parse(array[3]);
				internalLoadPhotoCompletionHandler(imgWidth, imgHeight, error);
			}
			else
			{
				LogError("GameCenter.OnGameCenterLoadPhotoComplete: Handler not registered: " + num2);
			}
		}

		[UsedImplicitly]
		private void OnGameCenterRequestComplete(string param)
		{
			string[] array = param.Split('|');
			int num = array.Length;
			if (num != 2)
			{
				LogError("GameCenter.OnGameCenterRequestComplete: Unexpected number of parameters :" + array.Length);
				return;
			}
			int num2 = int.Parse(array[0]);
			RequestCompleteHandler requestCompleteHandler = PluginUtil.CheckOutHandler(num2) as RequestCompleteHandler;
			if (requestCompleteHandler != null)
			{
				string error = ((array[1].Length <= 0) ? null : UnescapeResultString(array[1]));
				requestCompleteHandler(error);
			}
			else
			{
				LogError("GameCenter.OnGameCenterRequestComplete: Handler not registered: " + num2);
			}
		}

		[UsedImplicitly]
		private void OnGameCenterLoadLeaderboardComplete(string param)
		{
			string[] array = param.Split('|');
			int num = array.Length;
			if (num != 2 && (num < 11 || (num - 5) % 6 != 0))
			{
				LogError("GameCenter.OnGameCenterLoadLeaderboardComplete: Unexpected number of parameters :" + array.Length);
				return;
			}
			int ticket = int.Parse(array[0]);
			LoadLeaderboardCompletionHandler loadLeaderboardCompletionHandler = PluginUtil.CheckOutHandler(ticket) as LoadLeaderboardCompletionHandler;
			if (loadLeaderboardCompletionHandler != null)
			{
				string error = ((array[1].Length <= 0) ? null : UnescapeResultString(array[1]));
				Leaderboard leaderboard = null;
				if (num > 2)
				{
					Leaderboard leaderboard2 = new Leaderboard();
					leaderboard2.title = array[2];
					leaderboard2.groupIdentifier = ((array[3].Length != 0) ? array[3] : null);
					leaderboard2.maxRange = uint.Parse(array[4]);
					leaderboard = leaderboard2;
					int num2 = (num - 5) / 6;
					int num3 = 5;
					leaderboard.localPlayerScore.playerID = array[num3];
					leaderboard.localPlayerScore.rank = int.Parse(array[num3 + 1]);
					leaderboard.localPlayerScore.value = long.Parse(array[num3 + 2]);
					leaderboard.localPlayerScore.formattedValue = UnescapeResultString(array[num3 + 3]);
					leaderboard.localPlayerScore.date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(int.Parse(array[num3 + 4]));
					leaderboard.localPlayerScore.context = ulong.Parse(array[num3 + 5]);
					num3 += 6;
					num2--;
					Score[] array2 = new Score[num2];
					for (int i = 0; i < num2; i++)
					{
						array2[i].playerID = array[num3];
						array2[i].rank = int.Parse(array[num3 + 1]);
						array2[i].value = long.Parse(array[num3 + 2]);
						array2[i].formattedValue = UnescapeResultString(array[num3 + 3]);
						array2[i].date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(int.Parse(array[num3 + 4]));
						array2[i].context = ulong.Parse(array[num3 + 5]);
						num3 += 6;
					}
					leaderboard.scores = array2;
				}
				loadLeaderboardCompletionHandler(leaderboard, error);
			}
			else
			{
				LogError("GameCenter.OnGameCenterLoadLeaderboardComplete: Handler not registered");
			}
		}

		[UsedImplicitly]
		private void OnGameCenterLoadAchievementsComplete(string param)
		{
			string[] array = param.Split('|');
			int num = array.Length;
			if (num < 2 || (num - 2) % 3 != 0)
			{
				LogError("GameCenter.OnGameCenterLoadAchievementsComplete: Unexpected number of parameters :" + array.Length);
				return;
			}
			int ticket = int.Parse(array[0]);
			LoadAchievementsCompletionHandler loadAchievementsCompletionHandler = PluginUtil.CheckOutHandler(ticket) as LoadAchievementsCompletionHandler;
			if (loadAchievementsCompletionHandler != null)
			{
				string error = ((array[1].Length <= 0) ? null : UnescapeResultString(array[1]));
				Achievement[] array2 = null;
				if (num > 2)
				{
					int num2 = (num - 2) / 3;
					array2 = new Achievement[num2];
					int num3 = 2;
					for (int i = 0; i < num2; i++)
					{
						array2[i] = new Achievement
						{
							identifier = array[num3],
							percentComplete = float.Parse(array[num3 + 1]),
							lastReportedDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(int.Parse(array[num3 + 2]))
						};
						num3 += 3;
					}
				}
				loadAchievementsCompletionHandler(array2, error);
			}
			else
			{
				LogError("GameCenter.OnGameCenterLoadAchievementsComplete: Handler not registered");
			}
		}

		[UsedImplicitly]
		private void OnGameCenterLoadAchievementDescriptionsComplete(string param)
		{
			string[] array = param.Split('|');
			int num = array.Length;
			if (num < 2 || (num - 2) % 8 != 0)
			{
				LogError("GameCenter.OnGameCenterLoadAchievementDescriptionsComplete: Unexpected number of parameters :" + array.Length);
				return;
			}
			int ticket = int.Parse(array[0]);
			LoadAchievementDescriptionsCompletionHandler loadAchievementDescriptionsCompletionHandler = PluginUtil.CheckOutHandler(ticket) as LoadAchievementDescriptionsCompletionHandler;
			if (loadAchievementDescriptionsCompletionHandler != null)
			{
				string error = ((array[1].Length <= 0) ? null : UnescapeResultString(array[1]));
				AchievementDescription[] array2 = null;
				if (num > 2)
				{
					int num2 = (num - 2) / 8;
					array2 = new AchievementDescription[num2];
					int num3 = 2;
					for (int i = 0; i < num2; i++)
					{
						array2[i] = new AchievementDescription
						{
							identifier = array[num3],
							title = UnescapeResultString(array[num3 + 1]),
							unachievedDescription = UnescapeResultString(array[num3 + 2]),
							achievedDescription = UnescapeResultString(array[num3 + 3]),
							maximumPoints = int.Parse(array[num3 + 4]),
							replayable = (array[num3 + 5] == "true"),
							hidden = (array[num3 + 6] == "true"),
							groupIdentifier = ((array[num3 + 7].Length != 0) ? array[num3 + 7] : null)
						};
						num3 += 8;
					}
				}
				loadAchievementDescriptionsCompletionHandler(array2, error);
			}
			else
			{
				LogError("GameCenter.OnGameCenterLoadAchievementDescriptionsComplete: Handler not registered");
			}
		}

		[UsedImplicitly]
		private void OnGameCenterViewDismissed(string param)
		{
			string[] array = param.Split('|');
			int num = array.Length;
			if (num != 3)
			{
				LogError("GameCenter.OnGameCenterViewDismissed: Unexpected number of parameters :" + array.Length);
				return;
			}
			if (_viewDismissedHandler != null)
			{
				ShowViewState viewState = (ShowViewState)int.Parse(array[0]);
				string leaderboardCategory = ((array[1].Length <= 0) ? null : array[1]);
				LeaderboardTimeScope leaderboardTimeScope = (LeaderboardTimeScope)int.Parse(array[2]);
				_viewDismissedHandler(viewState, leaderboardCategory, leaderboardTimeScope);
			}
			_viewDismissedHandler = null;
		}

		private static string UnescapeResultString(string s)
		{
			if (s != null && s.Contains("&#124;"))
			{
				return s.Replace("&#124;", '|'.ToString(CultureInfo.InvariantCulture));
			}
			return s;
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[GameCenter] " + msg);
		}

		private static void LogWarning(string msg)
		{
			Debug.LogWarning("[GameCenter] " + msg);
		}

		[Conditional("GAMECENTER_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[GameCenter] " + msg);
		}
	}
}
