using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using Kiloo.Common;
using Kiloo.HTTP;
using Kiloo.Plugins.DeviceUtility;
using MiniJSON;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TopRun.Internal
{
	internal class Manager : IManager, ITournamentPlayerManager
	{
		private class Rank : IRank
		{
			public int rank { get; internal set; }

			public string country { get; internal set; }
		}

		private enum FacebookLoginResult
		{
			Success = 0,
			Failed = 1,
			NotFound = 2
		}

		private const string KEY_COUNTRY = "country";

		private const string KEY_NEXT_TOURNAMENT_TIME = "nextTournamentTime";

		private const string KEY_LOCAL_PLAYER_FACEBOOK_ID = "localPlayerFacebookId";

		private const string KEY_LOCAL_PLAYER_PRIVATE_KEY = "localPlayerPrivateKey";

		private const string KEY_LOCAL_PLAYER_PUBLIC_KEY = "localPlayerPublicKey";

		private const string KEY_LAST_SUBMITTED_SCORE_START_TIME = "lastSubmittedScoreStartTime";

		private const string KEY_LAST_SUBMITTED_SCORE_END_TIME = "lastSubmittedScoreEndTime";

		private const string KEY_LAST_SUBMITTED_SCORE_COUNTRY = "lastSubmittedScoreCountry";

		private const string KEY_UNSUBMITTED_SCORE_NO_TOURNAMENT = "unsubmittedScoreNoTournament";

		private const string KEY_UNSUBMITTED_SCORE_NO_TOURNAMENT_TIME = "unsubmittedScoreNoTournamentTime";

		private const string KEY_CURRENT_TOURNAMENT = "currentTournament";

		private const string KEY_ID = "id";

		private const string KEY_SEQUENCE_ID = "sequenceId";

		private const string KEY_SUBMITTED_SCORE = "submittedScore";

		private const string KEY_UNSUBMITTED_SCORE = "unsubmittedScore";

		private const string KEY_START_TIME = "startTime";

		private const string KEY_END_TIME = "endTime";

		private const string KEY_NEXT_START_TIME = "nextStartTime";

		private const string KEY_CLOSED = "closed";

		private const string KEY_PARTICIPANTS = "participants";

		private const string KEY_PARTICIPANTS_TIME_STAMP = "participantsTimeStamp";

		private const string KEY_SCORE_MATCHED_PARTICIPANTS = "scoreMatchedParticipants";

		private const string KEY_SCORE_MATCHED_PARTICIPANTS_TIME_STAMP = "scoreMatchedParticipantsTimeStamp";

		private const string KEY_PREV_TOURNAMENT = "previousTournament";

		private const string KEY_RANKS = "ranks";

		private const string KEY_TOURNAMENT_ID = "tournamentId";

		private const string KEY_RANK = "rank";

		private const string KEY_PLAYER_ID_PUBLIC_KEY = "publicKey";

		private const string KEY_PLAYER_ID_NAME = "name";

		private readonly IDictionary _managerData;

		private bool _managerDataIsDirty;

		private readonly IDictionary _playerIdData;

		private bool _playerIdDataIsDirty;

		private readonly IDictionary _rankData;

		private Dictionary<string, Player> _currentTournamentPlayers;

		private Dictionary<string, Rank> _ranks;

		private Rank[] _currentTournamentRanks = new Rank[0];

		private bool _fetchingNewTournament;

		private bool _isRefreshingTournament;

		private bool _isRefreshPending;

		private long _lastFriendsUpdate;

		private long _lastTournamentUpdate;

		private IList<string> _lastTournamentUpdateParticipantsPublicKeys;

		private bool _pauseRefreshing;

		private static readonly string[] TOURNAMENT_PARTICIPANT_LIST_KEYS = new string[2] { "participants", "scoreMatchedParticipants" };

		public string country
		{
			get
			{
				return _managerData["country"] as string;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException();
				}
				string text = _managerData["country"] as string;
				if (!(text == value))
				{
					_managerData["country"] = value;
					_managerData.Remove("nextTournamentTime");
					_managerDataIsDirty = true;
				}
			}
		}

		public int multiplier { get; set; }

		public int highScore { get; set; }

		public bool canSubmitScore
		{
			get
			{
				IDictionary dictionary = _managerData["currentTournament"] as IDictionary;
				if (dictionary == null)
				{
					return false;
				}
				if (JsonObject.Get(dictionary, "closed", false))
				{
					return false;
				}
				long num = JsonObject.Get(dictionary, "endTime", 0L);
				long num2 = DateTime.UtcNow.ToUnixTime();
				if (num2 >= num)
				{
					return false;
				}
				string text = JsonObject.Get<string>(_managerData, "lastSubmittedScoreCountry", null, null);
				if (text != null && text != GetDataSubObjectValue(_managerData, "currentTournament", "country") as string)
				{
					long num3 = JsonObject.Get(_managerData, "lastSubmittedScoreEndTime", 0L);
					if (num3 > num2)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool isRefreshing
		{
			get
			{
				return _isRefreshingTournament || _isRefreshPending;
			}
		}

		public bool pauseRefreshing
		{
			get
			{
				return _pauseRefreshing;
			}
			set
			{
				_pauseRefreshing = value;
			}
		}

		public bool hasFetchedTournament
		{
			get
			{
				return !string.IsNullOrEmpty(GetDataSubObjectValue(_managerData, "currentTournament", "id") as string);
			}
		}

		public int score
		{
			get
			{
				object jsonObject = JsonObject.Get<object>(_managerData, "currentTournament");
				int result;
				if (JsonObject.TryGet<int>(jsonObject, "unsubmittedScore", out result))
				{
					return result;
				}
				return JsonObject.Get(jsonObject, "submittedScore", 0);
			}
		}

		public IRank rank { get; private set; }

		public DateTime nextTournamentFetchTime
		{
			get
			{
				long num = JsonObject.Get(_managerData, "nextTournamentTime", 0L);
				if (num != 0L)
				{
					return TimeUtility.FromUnixTime(num);
				}
				IDictionary dictionary = _managerData["currentTournament"] as IDictionary;
				if (dictionary != null)
				{
					long num2 = JsonObject.Get(dictionary, "nextStartTime", 0L);
					if (num2 > 0)
					{
						return TimeUtility.FromUnixTime(num2);
					}
					return DateTime.MaxValue;
				}
				return DateTime.MinValue;
			}
		}

		public ITournament tournament { get; private set; }

		private float requestTimeout
		{
			get
			{
				return (float)OnlineSettings.instance.GetValue("toprun_request_timeout", 10.0);
			}
		}

		private float requestMinBackOffTime
		{
			get
			{
				return (float)OnlineSettings.instance.GetValue("toprun_request_min_back_off_time", 1.0);
			}
		}

		private float requestMaxBackOffTime
		{
			get
			{
				return (float)OnlineSettings.instance.GetValue("toprun_request_max_back_off_time", 120.0);
			}
		}

		public event EventHandler<TournamentChangedEventArgs> tournamentChanged;

		public event EventHandler<PlayerInfoChangedEventArgs> playerInfoChanged;

		public Manager(string defaultCountry, int multiplier, int highScore)
		{
			if (string.IsNullOrEmpty(defaultCountry))
			{
				throw new ArgumentException();
			}
			_managerData = (LoadJsonFile("toprun_manager") as IDictionary) ?? new Dictionary<string, object>();
			_managerDataIsDirty = false;
			_playerIdData = (LoadJsonFile("toprun_playerids") as IDictionary) ?? new Dictionary<string, object>();
			_playerIdDataIsDirty = false;
			_rankData = (LoadJsonFile("toprun_ranks") as IDictionary) ?? new Dictionary<string, object>();
			InitializeRanksFromSaveData();
			InitializeCurrentTournamentPlayers();
			if (!(_managerData["country"] is string))
			{
				country = defaultCountry;
			}
			this.multiplier = multiplier;
			this.highScore = highScore;
			ApplicationState.paused += OnPaused;
			ApplicationState.quit += OnQuit;
		}

		private void OnQuit()
		{
			Save();
		}

		private void OnPaused()
		{
			Save();
		}

		private void InitializeCurrentTournamentPlayers()
		{
			_currentTournamentPlayers = new Dictionary<string, Player>();
			AddFacebookFriendsWithIdsToCurrentTournamentPlayers();
			AddCurrentTournamentParticipantsToCurrentTournamentPlayers();
		}

		private void AddCurrentTournamentParticipantsToCurrentTournamentPlayers()
		{
			IDictionary dictionary = _managerData["currentTournament"] as IDictionary;
			if (dictionary == null)
			{
				return;
			}
			string[] tOURNAMENT_PARTICIPANT_LIST_KEYS = TOURNAMENT_PARTICIPANT_LIST_KEYS;
			foreach (string key in tOURNAMENT_PARTICIPANT_LIST_KEYS)
			{
				IList list = dictionary[key] as IList;
				if (list == null)
				{
					continue;
				}
				for (int num = list.Count - 1; num >= 0; num--)
				{
					Player player = null;
					string text = null;
					string text2 = list[num] as string;
					if (text2 != null)
					{
						text = FindFacebookIdFromPublicKey(text2);
						if (!string.IsNullOrEmpty(text))
						{
							string text3 = GetDataSubObjectValue(_playerIdData, text, "name") as string;
							if (!string.IsNullOrEmpty(text3))
							{
								FacebookProfile facebookProfile = new FacebookProfile();
								facebookProfile.id = text;
								facebookProfile.name = text3;
								facebookProfile.fullName = text3;
								FacebookProfile facebookProfile2 = facebookProfile;
								Player player2 = new Player();
								player2.publicKey = text2;
								player2.isFriend = false;
								player2.facebookProfile = facebookProfile2;
								player = player2;
							}
						}
					}
					if (player != null)
					{
						_currentTournamentPlayers[text2] = player;
					}
					else
					{
						if (!string.IsNullOrEmpty(text))
						{
							_playerIdData.Remove(text);
							_playerIdDataIsDirty = true;
						}
						list.RemoveAt(num);
						dictionary.Remove("participantsTimeStamp");
						dictionary.Remove("scoreMatchedParticipantsTimeStamp");
						_managerDataIsDirty = true;
					}
				}
			}
		}

		private void AddFacebookFriendsWithIdsToCurrentTournamentPlayers()
		{
			IList<Player> facebookFriendsWithPublicKeysAsPlayers = GetFacebookFriendsWithPublicKeysAsPlayers();
			foreach (Player item in facebookFriendsWithPublicKeysAsPlayers)
			{
				_currentTournamentPlayers[item.publicKey] = item;
			}
		}

		public IRank GetPlayerRank(string facebookId)
		{
			Rank value = null;
			string text = GetDataSubObjectValue(_playerIdData, facebookId, "publicKey") as string;
			if (text != null)
			{
				ITournament tournament = this.tournament;
				Player value2;
				if (tournament != null && tournament.hasResult && _currentTournamentPlayers.TryGetValue(text, out value2))
				{
					int rankForScore = tournament.GetRankForScore(value2.score);
					if (rankForScore >= _currentTournamentRanks.Length || tournament.country != _currentTournamentRanks[0].country)
					{
						int rankCount = tournament.rankCount;
						_currentTournamentRanks = new Rank[rankCount];
						for (int i = 0; i < rankCount; i++)
						{
							_currentTournamentRanks[i] = new Rank
							{
								country = tournament.country,
								rank = i
							};
						}
					}
					if (rankForScore < _currentTournamentRanks.Length)
					{
						value = _currentTournamentRanks[rankForScore];
					}
				}
				if (value == null)
				{
					_ranks.TryGetValue(text, out value);
				}
			}
			return value;
		}

		public void FetchNextTournament()
		{
			if (_isRefreshingTournament)
			{
				LogWarning("FetchNextTournament: Cannot fetch next tournament while refreshing, ignoring");
				return;
			}
			_isRefreshingTournament = true;
			InProcessFetchNextTournament();
		}

		public void RefreshTournament()
		{
			CoroutineUtil.StartCoroutine(RefreshTournamentCoroutine());
		}

		public void SubmitScore(int multiplier, int scoreToSubmit, DateTime gameStartedTime)
		{
			CoroutineUtil.StartCoroutine(RequestSubmitScoreCoroutine(multiplier, scoreToSubmit, false, false));
		}

		public IPlayer GetPlayerByPublicKey(string publicKey)
		{
			Player value;
			if (_currentTournamentPlayers != null && _currentTournamentPlayers.TryGetValue(publicKey, out value))
			{
				return value;
			}
			return null;
		}

		internal static void ClearSavedData()
		{
			SaveJsonFile("toprun_manager", null);
			SaveJsonFile("toprun_playerids", null);
			SaveJsonFile("toprun_ranks", null);
		}

		private void FirePlayerInfoChangedEvent(IPlayer player)
		{
			EventHandler<PlayerInfoChangedEventArgs> eventHandler = this.playerInfoChanged;
			if (eventHandler != null)
			{
				eventHandler(this, new PlayerInfoChangedEventArgs
				{
					player = player
				});
			}
		}

		private void FireTournamentChangedEvent(bool wasUpdated, bool didChange)
		{
			EventHandler<TournamentChangedEventArgs> eventHandler = this.tournamentChanged;
			if (eventHandler != null)
			{
				eventHandler(this, new TournamentChangedEventArgs
				{
					tournamentWasUpdated = wasUpdated,
					tournamentDidChange = didChange
				});
			}
		}

		private string MakeApiURL(string apiResource)
		{
			return OnlineSettings.instance.GetValue("toprun_base_url", "https://subwaysurfers.kiloo-games.com/api/v3") + apiResource;
		}

		private string FindFacebookIdFromPublicKey(string publicKey)
		{
			foreach (DictionaryEntry playerIdDatum in _playerIdData)
			{
				IDictionary dictionary = playerIdDatum.Value as IDictionary;
				if (dictionary != null && publicKey == dictionary["publicKey"] as string)
				{
					return (string)playerIdDatum.Key;
				}
			}
			return null;
		}

		[Pure]
		private static object GetDataSubObjectValue(IDictionary data, string subObjectKey, string subObjectValueKey)
		{
			IDictionary dictionary = data[subObjectKey] as IDictionary;
			if (dictionary != null)
			{
				return dictionary[subObjectValueKey];
			}
			return null;
		}

		private static void SetDataSubObjectValue(IDictionary data, string subObjectKey, string subObjectValueKey, object value)
		{
			IDictionary result;
			if (!JsonObject.TryGet<IDictionary>(data, subObjectKey, out result))
			{
				result = (IDictionary)(data[subObjectKey] = new Dictionary<string, object>());
			}
			result[subObjectValueKey] = value;
		}

		private static TimeSpan GetClientTimeOffset(IRequest request)
		{
			IResponse response = request.response;
			if (response != null && response.headers.Contains("Date"))
			{
				DateTime dateTime = response.headers.Get("Date", DateTime.MinValue);
				if (dateTime != DateTime.MinValue)
				{
					return DateTime.UtcNow - dateTime;
				}
			}
			return TimeSpan.Zero;
		}

		private void CompleteRefreshProcess(bool isNewTournament)
		{
			_isRefreshingTournament = false;
			FireTournamentChangedEvent(true, isNewTournament);
		}

		private IEnumerator RefreshTournamentCoroutine()
		{
			if (_isRefreshPending)
			{
				yield break;
			}
			if (_isRefreshingTournament)
			{
				_isRefreshPending = true;
				while (_isRefreshingTournament)
				{
					yield return null;
				}
				_isRefreshPending = false;
			}
			_isRefreshingTournament = true;
			yield return CoroutineUtil.StartCoroutine(InProcessRefreshTournamentCoroutine());
		}

		private IEnumerator InProcessRefreshTournamentCoroutine()
		{
			string currentTournamentId = GetDataSubObjectValue(_managerData, "currentTournament", "id") as string;
			if (string.IsNullOrEmpty(currentTournamentId))
			{
				while (pauseRefreshing)
				{
					yield return null;
				}
				InProcessFetchNextTournament();
				yield break;
			}
			SocialManager socialManager = SocialManager.instance;
			if (socialManager != null && !socialManager.consolidatedFriendsCompleted)
			{
				do
				{
					yield return null;
				}
				while (!socialManager.consolidatedFriendsCompleted);
			}
			bool isNewTournament = _fetchingNewTournament;
			_fetchingNewTournament = false;
			while (pauseRefreshing)
			{
				yield return null;
			}
			yield return CoroutineUtil.WaitForAll(CoroutineUtil.StartCoroutine(RequestPlayersByFacebookIds()), CoroutineUtil.StartCoroutine(RequestFindMatchedPlayers()));
			while (pauseRefreshing)
			{
				yield return null;
			}
			yield return CoroutineUtil.StartCoroutine(RequestFindMatchedPlayersByCurrentScore());
			while (pauseRefreshing)
			{
				yield return null;
			}
			PurgeSelfFromParticipants();
			PurgeUnneededIds();
			bool mustFetchNewTournament = false;
			yield return CoroutineUtil.StartCoroutine(RequestTournamentData(delegate(bool isTournamentNotFound)
			{
				if (isTournamentNotFound)
				{
					mustFetchNewTournament = true;
				}
			}));
			while (pauseRefreshing)
			{
				yield return null;
			}
			if (mustFetchNewTournament)
			{
				_isRefreshPending = false;
				InProcessFetchNextTournament();
				yield break;
			}
			GetMissingFacebookInfo();
			int unsubmittedScore = PropagateUnsubmittedScore();
			if (unsubmittedScore > 0)
			{
				CoroutineUtil.StartCoroutine(RequestSubmitScoreCoroutine(multiplier, unsubmittedScore, false, true));
			}
			yield return CoroutineUtil.WaitForAll(CoroutineUtil.StartCoroutine(RequestRanksForPlayers()), CoroutineUtil.StartCoroutine(RequestNextTournamentFetchTime()));
			Save();
			CompleteRefreshProcess(isNewTournament);
		}

		private int PropagateUnsubmittedScore()
		{
			int num = 0;
			IDictionary dictionary = _managerData["currentTournament"] as IDictionary;
			if (dictionary != null)
			{
				num = JsonObject.Get(dictionary, "unsubmittedScore", 0);
				int num2 = JsonObject.Get(_managerData, "unsubmittedScoreNoTournament", 0);
				if (num2 > 0)
				{
					long num3 = JsonObject.Get(_managerData, "unsubmittedScoreNoTournamentTime", 0L);
					long num4 = JsonObject.Get(dictionary, "startTime", 0L);
					long num5 = JsonObject.Get(dictionary, "endTime", 0L);
					if (num4 != 0L && num5 != 0L && num3 < num5)
					{
						_managerData.Remove("unsubmittedScoreNoTournament");
						_managerData.Remove("unsubmittedScoreNoTournamentTime");
						if (num3 >= num4 && num2 > num)
						{
							dictionary["unsubmittedScore"] = (num = num2);
						}
						_managerDataIsDirty = true;
						Save();
					}
				}
			}
			return num;
		}

		private void InProcessFetchNextTournament()
		{
			if (string.IsNullOrEmpty(country))
			{
				LogError("FetchNextTournament: Country must be set first");
				return;
			}
			string newTournamentCountry = country;
			KilooWebClient.CreateGet(new Uri(string.Format("{0}?Country={1}", MakeApiURL("/tournaments/getlatesttournamentidbycountry"), newTournamentCountry))).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
				.WithMaxBackOffTime(requestMaxBackOffTime)
				.ThenMakeTheRequestAsync(delegate(IRequest request, bool isFromCache)
				{
					string text = null;
					IResponse response = request.response;
					string json = ((response != null) ? response.ToString() : null);
					if (request.isSuccessful && response != null && response.IsJSON())
					{
						IDictionary dictionary;
						try
						{
							dictionary = Json.Deserialize(json) as IDictionary;
						}
						catch (Exception)
						{
							dictionary = null;
						}
						if (dictionary != null)
						{
							string text2 = dictionary["TournamentId"] as string;
							if (text2 != null)
							{
								text = text2;
							}
						}
					}
					if (text == null)
					{
						LogError("FetchNextTournament: Request failed or bad response, status: " + request.statusMessage);
						CompleteRefreshProcess(false);
					}
					else
					{
						string value = GetDataSubObjectValue(_managerData, "currentTournament", "id") as string;
						if (!text.Equals(value))
						{
							_managerData.Remove("nextTournamentTime");
							IDictionary dictionary2 = _managerData["currentTournament"] as IDictionary;
							if (dictionary2 != null && (dictionary2.Contains("submittedScore") || dictionary2.Contains("unsubmittedScore")))
							{
								string[] tOURNAMENT_PARTICIPANT_LIST_KEYS = TOURNAMENT_PARTICIPANT_LIST_KEYS;
								foreach (string key in tOURNAMENT_PARTICIPANT_LIST_KEYS)
								{
									dictionary2.Remove(key);
								}
								_managerData["previousTournament"] = dictionary2;
							}
							_managerData["currentTournament"] = new Dictionary<string, object>
							{
								{ "id", text },
								{ "country", newTournamentCountry }
							};
							_currentTournamentPlayers = new Dictionary<string, Player>();
							_managerDataIsDirty = true;
							_fetchingNewTournament = true;
						}
						CoroutineUtil.StartCoroutine(InProcessRefreshTournamentCoroutine());
					}
				});
		}

		private IList<FacebookProfile> GetFacebookFriendsWithNoPublicKeys()
		{
			List<FacebookProfile> list = new List<FacebookProfile>();
			SocialManager instance = SocialManager.instance;
			if (instance.facebookIsLoggedIn && (instance.consolidationState & 1) != 0)
			{
				Friend[] array = instance.FriendsSortedByScore();
				Friend[] array2 = array;
				foreach (Friend friend in array2)
				{
					if (friend.fbProfile != null && !_playerIdData.Contains(friend.fbProfile.id))
					{
						list.Add(friend.fbProfile);
					}
				}
			}
			return list;
		}

		private IList<Player> GetFacebookFriendsWithPublicKeysAsPlayers()
		{
			List<Player> list = new List<Player>();
			if (_playerIdData != null)
			{
				SocialManager instance = SocialManager.instance;
				if (instance.facebookIsLoggedIn && (instance.consolidationState & 1) != 0)
				{
					Friend[] array = instance.FriendsSortedByScore();
					Friend[] array2 = array;
					foreach (Friend friend in array2)
					{
						FacebookProfile fbProfile = friend.fbProfile;
						if (fbProfile != null)
						{
							string text = GetDataSubObjectValue(_playerIdData, fbProfile.id, "publicKey") as string;
							if (text != null)
							{
								list.Add(new Player
								{
									publicKey = text,
									facebookProfile = fbProfile,
									isFriend = true
								});
							}
						}
					}
				}
			}
			return list;
		}

		private IEnumerator RequestPlayersByFacebookIds()
		{
			IList<FacebookProfile> friendsWithNoPublicKeys = null;
			if (_lastFriendsUpdate + OnlineSettings.instance.GetValue("toprun_friends_refresh_interval", 1800) < DateTime.UtcNow.ToUnixTime())
			{
				friendsWithNoPublicKeys = GetFacebookFriendsWithNoPublicKeys();
			}
			if (friendsWithNoPublicKeys == null || friendsWithNoPublicKeys.Count <= 0)
			{
				yield break;
			}
			List<object> facebookIds = new List<object>(friendsWithNoPublicKeys.Count);
			foreach (FacebookProfile friend in friendsWithNoPublicKeys)
			{
				ulong fbid;
				if (ulong.TryParse(friend.id, NumberStyles.None, CultureInfo.InvariantCulture, out fbid))
				{
					facebookIds.Add(fbid);
				}
			}
			IRequest request = RequestExtensions.WithJsonContent(jsonData: new Dictionary<string, object> { { "FacebookIds", facebookIds } }, request: KilooWebClient.CreatePost(new Uri(MakeApiURL("/facebook/getplayersbyfacebookids")))).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
				.WithMaxBackOffTime(requestMaxBackOffTime);
			yield return request.SendAsync();
			bool badResponse = true;
			IResponse response = request.response;
			string responseText = ((response != null) ? response.ToString() : null);
			if (request.isSuccessful && request.response.IsJSON())
			{
				IDictionary data;
				try
				{
					data = Json.Deserialize(responseText) as IDictionary;
				}
				catch (Exception)
				{
					data = null;
				}
				if (data != null)
				{
					IList players = data["Players"] as IList;
					if (players != null)
					{
						badResponse = false;
						_lastFriendsUpdate = DateTime.UtcNow.ToUnixTime();
						foreach (object obj in players)
						{
							IDictionary player = obj as IDictionary;
							if (player != null)
							{
								string facebookId = JsonObject.Get<string>(player, "FacebookId", null, null);
								string publicKey = JsonObject.Get<string>(player, "PublicKey", null, null);
								if (facebookId != null && publicKey != null)
								{
									_playerIdData[facebookId] = new Dictionary<string, object> { { "publicKey", publicKey } };
									_playerIdDataIsDirty = true;
								}
							}
						}
					}
				}
			}
			if (badResponse)
			{
				LogError("LookupFriendPublicKeys: Request failed or bad response, status: " + request.statusMessage);
			}
			SocialManager social = SocialManager.instance;
			if (!social.facebookIsLoggedIn || (social.consolidationState & 1) == 0)
			{
				yield break;
			}
			Friend[] friends = social.FriendsSortedByScore();
			Friend[] array = friends;
			foreach (Friend friend2 in array)
			{
				if (friend2.fbProfile == null)
				{
					continue;
				}
				string publicKey2 = GetDataSubObjectValue(_playerIdData, friend2.fbProfile.id, "publicKey") as string;
				if (publicKey2 != null)
				{
					Player player2;
					_currentTournamentPlayers.TryGetValue(publicKey2, out player2);
					if (player2 == null)
					{
						player2 = new Player
						{
							publicKey = publicKey2
						};
						_currentTournamentPlayers[publicKey2] = player2;
					}
					player2.isFriend = true;
					player2.facebookProfile = friend2.fbProfile;
				}
			}
		}

		private bool HandleFindMatchedPlayersResponse(IRequest request, string managerDataTournamentParticipantListKey, string managerDataTournamentParticipantListTimeStampKey)
		{
			IResponse response = request.response;
			string json = ((response != null) ? response.ToString() : null);
			if (request.isSuccessful && response != null && response.IsJSON())
			{
				try
				{
					IDictionary dictionary = Json.Deserialize(json) as IDictionary;
					if (dictionary != null)
					{
						IList list = dictionary["Players"] as IList;
						if (list != null)
						{
							IList list2 = GetDataSubObjectValue(_managerData, "currentTournament", managerDataTournamentParticipantListKey) as IList;
							if (list2 == null)
							{
								list2 = new List<object>();
								SetDataSubObjectValue(_managerData, "currentTournament", managerDataTournamentParticipantListKey, list2);
							}
							foreach (object item in list)
							{
								string text = JsonObject.Get<string>(item, "FacebookId", null, null);
								string text2 = JsonObject.Get<string>(item, "PublicKey", null, null);
								string text3 = JsonObject.Get<string>(item, "PlayerName", null, null);
								if (text != null && text2 != null && text3 != null)
								{
									if (!list2.Contains(text2))
									{
										list2.Add(text2);
										_managerDataIsDirty = true;
									}
									_playerIdData[text] = new Dictionary<string, object>
									{
										{ "publicKey", text2 },
										{ "name", text3 }
									};
									_playerIdDataIsDirty = true;
									if (!_currentTournamentPlayers.ContainsKey(text2))
									{
										FacebookProfile facebookProfile = new FacebookProfile();
										facebookProfile.id = text;
										facebookProfile.name = text3;
										facebookProfile.fullName = text3;
										FacebookProfile facebookProfile2 = facebookProfile;
										_currentTournamentPlayers[text2] = new Player
										{
											publicKey = text2,
											isFriend = false,
											facebookProfile = facebookProfile2
										};
									}
								}
								else
								{
									LogWarning("HandleFindMatchedPlayersResponse: Invalid Player Data: FacebookId=" + QuoteString(text) + ", PublicKey=" + QuoteString(text2) + ", PlayerName=" + QuoteString(text3));
								}
							}
							SetDataSubObjectValue(_managerData, "currentTournament", managerDataTournamentParticipantListTimeStampKey, DateTime.UtcNow.ToUnixTime());
							return true;
						}
					}
				}
				catch (Exception)
				{
				}
			}
			return false;
		}

		private IEnumerator RequestFindMatchedPlayers()
		{
			IDictionary tour = JsonObject.Get<IDictionary>(_managerData, "currentTournament", null, null);
			long timeStamp = JsonObject.Get(tour, "participantsTimeStamp", 0L);
			long now = DateTime.UtcNow.ToUnixTime();
			if (timeStamp > now)
			{
				tour.Remove("participantsTimeStamp");
				timeStamp = 0L;
			}
			int needStrangers;
			if (timeStamp + OnlineSettings.instance.GetValue("toprun_stranger_amount_retry_interval", 3600) < now)
			{
				needStrangers = OnlineSettings.instance.GetValue("toprun_stranger_amount", 10);
				IList participants = GetDataSubObjectValue(_managerData, "currentTournament", "participants") as IList;
				if (participants != null)
				{
					needStrangers -= participants.Count;
				}
			}
			else
			{
				needStrangers = 0;
			}
			string tournamentId = GetDataSubObjectValue(_managerData, "currentTournament", "id") as string;
			if (needStrangers > 0 && !string.IsNullOrEmpty(tournamentId))
			{
				Uri uri = new Uri(string.Format("{0}?TournamentId={1}&Score={2}&Multiplier={3}&Amount={4}", MakeApiURL("/tournaments/findmatchedplayers"), tournamentId, highScore, multiplier, needStrangers));
				IRequest request = KilooWebClient.CreateGet(uri).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
					.WithMaxBackOffTime(requestMaxBackOffTime);
				yield return request.SendAsync();
				if (!HandleFindMatchedPlayersResponse(request, "participants", "participantsTimeStamp"))
				{
					LogError("RequestFindMatchedPlayers: Request failed or bad response, status: " + request.statusMessage);
				}
			}
		}

		private int GetAmountOfChallengersToRequest()
		{
			if (tournament == null || tournament.isFinished)
			{
				return 0;
			}
			int value = OnlineSettings.instance.GetValue("toprun_challengers_amount", 0);
			if (value <= 0)
			{
				return 0;
			}
			IList list = GetDataSubObjectValue(_managerData, "currentTournament", "scoreMatchedParticipants") as IList;
			if (list != null)
			{
				return 0;
			}
			long num = tournament.startTimeUTC.ToUnixTime();
			long num2 = tournament.endTimeUTC.ToUnixTime();
			long num3 = num2 - num;
			double num4 = (double)(DateTime.UtcNow.ToUnixTime() - num) / (double)num3;
			double value2 = OnlineSettings.instance.GetValue("toprun_challengers_period_normalized_start", 0.5);
			if (num4 < value2)
			{
				return 0;
			}
			double value3 = OnlineSettings.instance.GetValue("toprun_challengers_period_normalized_end", 0.7142857142857143);
			if (num4 >= value3)
			{
				return 0;
			}
			int value4 = OnlineSettings.instance.GetValue("toprun_challengers_placement_requirement", 4);
			int num5 = 1;
			foreach (IPlayer player in tournament.GetPlayers())
			{
				if (score > player.score)
				{
					break;
				}
				num5++;
			}
			if (num5 > value4)
			{
				return 0;
			}
			return value;
		}

		private IEnumerator RequestFindMatchedPlayersByCurrentScore()
		{
			IDictionary tour = JsonObject.Get<IDictionary>(_managerData, "currentTournament", null, null);
			int needStrangers = GetAmountOfChallengersToRequest();
			if (needStrangers > 0)
			{
				IList participants = JsonObject.Get<IList>(tour, "scoreMatchedParticipants", null, null);
				if (participants != null)
				{
					needStrangers -= participants.Count;
				}
			}
			if (needStrangers > 0)
			{
				string tournamentId = JsonObject.Get(tour, "id", string.Empty);
				Uri uri = new Uri(string.Format("{0}?TournamentId={1}&Score={2}&Multiplier={3}&Amount={4}", MakeApiURL("/tournaments/findmatchedplayersbycurrentscore"), tournamentId, score, multiplier, needStrangers));
				IRequest request = KilooWebClient.CreateGet(uri).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
					.WithMaxBackOffTime(requestMaxBackOffTime);
				yield return request.SendAsync();
				if (!HandleFindMatchedPlayersResponse(request, "scoreMatchedParticipants", "scoreMatchedParticipantsTimeStamp"))
				{
					LogError("RequestFindMatchedPlayersByCurrentScore: Request failed or bad response, status: " + request.statusMessage);
				}
			}
		}

		private void GetMissingFacebookInfo()
		{
			CoroutineUtil.StartCoroutine(GetMissingFacebookInfoCoroutine());
		}

		private IEnumerator GetMissingFacebookInfoCoroutine()
		{
			List<Player> players = GetTournamentPlayersWithMissingFacebookInfo();
			foreach (Player p in players)
			{
				Player player = p;
				FacebookProfile profile = player.facebookProfile;
				if (profile == null || !(profile.image == null) || IsFacebookIDInvalid(profile.id))
				{
					continue;
				}
				while (pauseRefreshing)
				{
					yield return null;
				}
				yield return DownloadFacebookImage(profile.id, delegate(Texture2D image)
				{
					if (image != null && image.width == 8 && image.height == 8)
					{
						image = null;
					}
					if (image != null)
					{
						profile.image = image;
						FirePlayerInfoChangedEvent(player);
					}
				});
			}
		}

		private bool IsFacebookIDInvalid(string facebookId)
		{
			int result;
			return int.TryParse(facebookId, out result) && result == 0;
		}

		private Coroutine DownloadFacebookImage(string facebookId, Action<Texture2D> onComplete)
		{
			Uri uri = new Uri(string.Format("http://graph.facebook.com/{0}/picture?type=square", facebookId));
			return WebClient.CreateGet(uri).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
				.WithMaxBackOffTime(requestMaxBackOffTime)
				.WithIgnoreCertificateErrors(true)
				.ThenMakeTheRequestAsync(delegate(IRequest request, bool cache)
				{
					Texture2D texture2D = null;
					bool flag = true;
					if (request.isSuccessful && request.response.IsTexture())
					{
						texture2D = request.response.AsTexture();
						if (texture2D != null)
						{
							flag = false;
						}
					}
					if (flag)
					{
						LogError("RequestUserInfoFromFacebook: Request failed or bad response, status: " + request.statusMessage);
					}
					onComplete(texture2D);
				});
		}

		private List<Player> GetTournamentPlayersWithMissingFacebookInfo()
		{
			List<Player> list = new List<Player>();
			foreach (KeyValuePair<string, Player> currentTournamentPlayer in _currentTournamentPlayers)
			{
				Player value = currentTournamentPlayer.Value;
				if (!value.isFriend && (value.facebookProfile == null || value.facebookProfile.image == null))
				{
					list.Add(value);
				}
			}
			return list;
		}

		private void PurgeSelfFromParticipants()
		{
			string text = JsonObject.Get<string>(_managerData, "localPlayerPublicKey", null, null);
			if (text == null)
			{
				SocialManager instance = SocialManager.instance;
				if (instance.facebookIsLoggedIn && (instance.consolidationState & 1) != 0)
				{
					FacebookProfile facebookProfile = instance.facebookProfile;
					if (facebookProfile != null)
					{
						text = GetDataSubObjectValue(_playerIdData, facebookProfile.id, "publicKey") as string;
					}
				}
			}
			if (text == null || !_currentTournamentPlayers.Remove(text))
			{
				return;
			}
			string[] tOURNAMENT_PARTICIPANT_LIST_KEYS = TOURNAMENT_PARTICIPANT_LIST_KEYS;
			foreach (string subObjectValueKey in tOURNAMENT_PARTICIPANT_LIST_KEYS)
			{
				IList list = GetDataSubObjectValue(_managerData, "currentTournament", subObjectValueKey) as IList;
				if (list != null)
				{
					list.Remove(text);
				}
			}
		}

		private void PurgeUnneededIds()
		{
			SocialManager instance = SocialManager.instance;
			if (!instance.facebookIsLoggedIn || (instance.consolidationState & 1) == 0)
			{
				return;
			}
			Friend[] array = instance.FriendsSortedByScore();
			List<string> list = new List<string>();
			foreach (DictionaryEntry playerIdDatum in _playerIdData)
			{
				string text = (string)playerIdDatum.Key;
				IDictionary dictionary = playerIdDatum.Value as IDictionary;
				string text2 = ((dictionary == null) ? null : (dictionary["publicKey"] as string));
				if (text2 == null)
				{
					list.Add(text);
				}
				else
				{
					if (_currentTournamentPlayers.ContainsKey(text2))
					{
						continue;
					}
					bool flag = false;
					Friend[] array2 = array;
					foreach (Friend friend in array2)
					{
						if (friend.fbProfile != null && friend.fbProfile.id == text)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						list.Add(text);
					}
				}
			}
			if (list.Count <= 0)
			{
				return;
			}
			foreach (string item in list)
			{
				_playerIdData.Remove(item);
				_playerIdDataIsDirty = true;
			}
		}

		private IEnumerator RequestTournamentData(Action<bool> onComplete)
		{
			string tournamentId = GetDataSubObjectValue(_managerData, "currentTournament", "id") as string;
			bool shouldUpdate = true;
			long now = DateTime.UtcNow.ToUnixTime();
			if (this.tournament != null && tournamentId == this.tournament.id)
			{
				int interval = ((!this.tournament.isFinished || this.tournament.hasResult) ? OnlineSettings.instance.GetValue("toprun_refresh_interval", 300) : OnlineSettings.instance.GetValue("toprun_refresh_interval_when_result_is_due", 30));
				if (_lastTournamentUpdate > now)
				{
					_lastTournamentUpdate = 0L;
				}
				shouldUpdate = now - _lastTournamentUpdate > interval;
			}
			List<string> participantsPublicKeys = new List<string>(_currentTournamentPlayers.Keys);
			if (!shouldUpdate && _lastTournamentUpdateParticipantsPublicKeys != null)
			{
				foreach (string publicKey in participantsPublicKeys)
				{
					if (!_lastTournamentUpdateParticipantsPublicKeys.Contains(publicKey))
					{
						shouldUpdate = true;
						break;
					}
				}
			}
			if (shouldUpdate)
			{
				_lastTournamentUpdate = now;
				_lastTournamentUpdateParticipantsPublicKeys = participantsPublicKeys;
				IRequest request = RequestExtensions.WithJsonContent(jsonData: new Dictionary<string, object>
				{
					{ "TournamentId", tournamentId },
					{ "ParticipantsPublicKeys", participantsPublicKeys }
				}, request: KilooWebClient.CreatePost(new Uri(MakeApiURL("/tournaments/gettournamentbyid")))).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
					.WithMaxBackOffTime(requestMaxBackOffTime);
				yield return request.SendAsync();
				bool isTournamentNotFound = false;
				bool badResponse = true;
				IResponse response = request.response;
				string responseText = ((response != null) ? response.ToString() : null);
				if (request.isSuccessful && response != null && response.IsJSON())
				{
					try
					{
						TimeSpan clientTimeOffset = GetClientTimeOffset(request);
						object json;
						try
						{
							json = Json.Deserialize(responseText);
						}
						catch (Exception)
						{
							json = null;
						}
						Tournament tour = (Tournament)(this.tournament = new Tournament(new JsonObject(json), clientTimeOffset, GetDataSubObjectValue(_managerData, "currentTournament", "country") as string, this));
						SetDataSubObjectValue(_managerData, "currentTournament", "startTime", tour.startTimeUTC.ToUnixTime());
						SetDataSubObjectValue(_managerData, "currentTournament", "endTime", tour.endTimeUTC.ToUnixTime());
						SetDataSubObjectValue(_managerData, "currentTournament", "nextStartTime", tour.nextTournamentTimeUTC.ToUnixTime());
						SetDataSubObjectValue(_managerData, "currentTournament", "sequenceId", tour.sequenceId);
						if (tour.hasResult)
						{
							SetDataSubObjectValue(_managerData, "currentTournament", "closed", true);
							int s = score;
							if (s > 0)
							{
								SetDataSubObjectValue(_managerData, "currentTournament", "rank", tour.GetRankForScore(s));
							}
						}
						Rank newRank = null;
						IDictionary previous = _managerData["previousTournament"] as IDictionary;
						if (previous != null)
						{
							int num;
							int previousRank = (num = JsonObject.Get(previous, "rank", -1));
							if (num >= 0)
							{
								string text;
								string previousRankCountry = (text = JsonObject.Get<string>(previous, "country", null, null));
								if (text != null)
								{
									int previousSequenceId = JsonObject.Get(previous, "sequenceId", -1);
									if (previousSequenceId + 1 == tour.sequenceId)
									{
										newRank = new Rank
										{
											country = previousRankCountry,
											rank = previousRank
										};
									}
								}
							}
						}
						rank = newRank;
						_managerDataIsDirty = true;
						foreach (KeyValuePair<string, Player> participant in _currentTournamentPlayers)
						{
							int highscore = tour.FindParticipantScoreByPublicKey(participant.Key);
							participant.Value.score = highscore;
						}
						badResponse = false;
					}
					catch (Exception ex2)
					{
						LogError("GetTournamentData: Error creating tournament object from received data: " + ex2);
					}
				}
				else if (response != null && response.status == 404)
				{
					isTournamentNotFound = true;
				}
				if (badResponse)
				{
					LogError("GetTournamentData: Request failed or bad response, status: " + request.statusMessage);
				}
				onComplete(isTournamentNotFound);
			}
			else
			{
				onComplete(false);
			}
		}

		private IEnumerator RequestNextTournamentFetchTime()
		{
			if (_managerData.Contains("nextTournamentTime"))
			{
				yield break;
			}
			IDictionary tour = _managerData["currentTournament"] as IDictionary;
			if (tour == null)
			{
				yield break;
			}
			string tourCountry = tour["country"] as string;
			if (country != tourCountry)
			{
				int submittedScore = JsonObject.Get(tour, "submittedScore", 0);
				if (submittedScore > 0)
				{
					string privateKey = JsonObject.Get<string>(_managerData, "localPlayerPrivateKey", null, null);
					if (privateKey == null)
					{
						yield break;
					}
					Uri uri = new Uri(string.Format("{0}?PrivateKey={1}&ToCountry={2}", MakeApiURL("/players/getcountrychangedate"), Uri.EscapeDataString(privateKey), country));
					IRequest request = KilooWebClient.CreateGet(uri).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
						.WithMaxBackOffTime(requestMaxBackOffTime);
					yield return request.SendAsync();
					bool badResponse = true;
					IResponse response = request.response;
					string responseText = ((response != null) ? response.ToString() : null);
					if (request.isSuccessful && response != null && response.IsJSON())
					{
						TimeSpan clientTimeOffset = GetClientTimeOffset(request);
						IDictionary data;
						try
						{
							data = Json.Deserialize(responseText) as IDictionary;
						}
						catch (Exception)
						{
							data = null;
						}
						DateTime changeDate;
						if (data != null && JsonObject.TryGet<DateTime>(data, "ChangeDateUtc", out changeDate))
						{
							changeDate += clientTimeOffset;
							_managerData["nextTournamentTime"] = changeDate.ToUnixTime();
							_managerDataIsDirty = true;
							badResponse = false;
						}
					}
					if (badResponse)
					{
						LogError("GetNextTournamentFetchTime: Request failed or bad response, status: " + request.statusMessage);
					}
				}
				else
				{
					_managerData["nextTournamentTime"] = DateTime.UtcNow.ToUnixTime();
					_managerDataIsDirty = true;
				}
			}
			else
			{
				_managerData.Remove("nextTournamentTime");
				_managerDataIsDirty = true;
			}
		}

		private IList<string> GetPublicKeysForPlayersWeHaveNotRequestedRanksFor()
		{
			IDictionary dictionary = _rankData["ranks"] as IDictionary;
			List<string> list = new List<string>(_playerIdData.Count);
			foreach (DictionaryEntry playerIdDatum in _playerIdData)
			{
				IDictionary dictionary2 = playerIdDatum.Value as IDictionary;
				string text = ((dictionary2 != null) ? (dictionary2["publicKey"] as string) : null);
				if (text != null && (dictionary == null || !dictionary.Contains(text)))
				{
					list.Add(text);
				}
			}
			return list;
		}

		private IEnumerator RequestRanksForPlayers()
		{
			string currentTournamentId = null;
			IDictionary tour = _managerData["currentTournament"] as IDictionary;
			if (tour != null)
			{
				currentTournamentId = tour["id"] as string;
			}
			string ranksUpdatedTournamentId = _rankData["tournamentId"] as string;
			if (ranksUpdatedTournamentId == null || ranksUpdatedTournamentId != currentTournamentId)
			{
				_rankData.Clear();
				_rankData["ranks"] = new Dictionary<string, object>(_playerIdData.Count + 1);
			}
			IList<string> keysToGetRanksFor = GetPublicKeysForPlayersWeHaveNotRequestedRanksFor();
			if (keysToGetRanksFor.Count <= 0)
			{
				yield break;
			}
			yield return RequestExtensions.WithJsonContent(jsonData: new Dictionary<string, object> { { "PublicKeys", keysToGetRanksFor } }, request: KilooWebClient.CreatePost(new Uri(MakeApiURL("/tournaments/getprevioustournamentresultsforplayers")))).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
				.WithMaxBackOffTime(requestMaxBackOffTime)
				.ThenMakeTheRequestAsync(delegate(IRequest request, bool isFromCache)
				{
					bool flag = true;
					IResponse response = request.response;
					string json = ((response != null) ? response.ToString() : null);
					if (request.isSuccessful && response != null && response.IsJSON())
					{
						IDictionary dictionary;
						try
						{
							dictionary = Json.Deserialize(json) as IDictionary;
						}
						catch (Exception)
						{
							dictionary = null;
						}
						if (dictionary != null)
						{
							IList list = dictionary["PreviousTournamentResults"] as IList;
							if (list != null)
							{
								_rankData["tournamentId"] = currentTournamentId;
								foreach (string item in keysToGetRanksFor)
								{
									IDictionary value = null;
									foreach (object item2 in list)
									{
										IDictionary dictionary2 = item2 as IDictionary;
										if (dictionary2 != null)
										{
											object obj = dictionary2["PublicKey"];
											if (obj != null)
											{
												if (item.Equals(obj))
												{
													string value2 = dictionary2["Country"] as string;
													int num = JsonObject.Get(dictionary2, "DivisionIndex", -1);
													if (num >= 0 && !string.IsNullOrEmpty(value2))
													{
														value = new Dictionary<string, object>
														{
															{ "country", value2 },
															{ "rank", num }
														};
													}
													else
													{
														LogError("GetRanksForPlayers: Invalid rank data received");
													}
													break;
												}
											}
											else
											{
												LogError("GetRanksForPlayers: Invalid rank data received");
											}
										}
										else
										{
											LogError("GetRanksForPlayers: Invalid rank data received");
										}
									}
									SetDataSubObjectValue(_rankData, "ranks", item, value);
								}
								SaveJsonFile("toprun_ranks", _rankData);
								InitializeRanksFromSaveData();
								flag = false;
							}
						}
					}
					if (flag)
					{
						LogError("GetRanksForPlayers: Request failed or bad response, status: " + request.statusMessage);
					}
				});
		}

		private IEnumerator RequestSubmitScoreCoroutine(int multiplier, int scoreToSubmit, bool isRetry, bool previouslySavedScore)
		{
			this.multiplier = multiplier;
			if (scoreToSubmit > highScore)
			{
				highScore = scoreToSubmit;
			}
			if (!canSubmitScore)
			{
				if (!previouslySavedScore)
				{
					int unsubmittedScoreNoTournament = JsonObject.Get(_managerData, "unsubmittedScoreNoTournament", 0);
					if (scoreToSubmit > unsubmittedScoreNoTournament)
					{
						_managerData["unsubmittedScoreNoTournament"] = scoreToSubmit;
						_managerData["unsubmittedScoreNoTournamentTime"] = DateTime.UtcNow.ToUnixTime();
						_managerDataIsDirty = true;
						Save();
					}
				}
				yield break;
			}
			IDictionary tour;
			JsonObject.TryGet<IDictionary>(_managerData, "currentTournament", out tour);
			int submittedScore = JsonObject.Get(tour, "submittedScore", 0);
			if (scoreToSubmit <= submittedScore)
			{
				yield break;
			}
			bool scoreWasUpdated = false;
			int unsubmittedScore = JsonObject.Get(tour, "unsubmittedScore", 0);
			if (scoreToSubmit > unsubmittedScore)
			{
				IDictionary dictionary = tour;
				int num;
				unsubmittedScore = (num = scoreToSubmit);
				dictionary["unsubmittedScore"] = num;
				_managerDataIsDirty = true;
				Save();
				scoreWasUpdated = true;
			}
			if (_isRefreshingTournament)
			{
				do
				{
					yield return null;
				}
				while (_isRefreshingTournament);
			}
			_isRefreshingTournament = true;
			yield return CoroutineUtil.StartCoroutine(RefreshKeysIfNecessary());
			Save();
			string privateKey = _managerData["localPlayerPrivateKey"] as string;
			string tournamentId = tour["id"] as string;
			string tournamentCountry = tour["country"] as string;
			long tournamentStart = JsonObject.Get(tour, "startTime", 0L);
			long tournamentEnd = JsonObject.Get(tour, "endTime", 0L);
			if (privateKey != null && tournamentId != null && tournamentCountry != null && tournamentStart != 0L && tournamentEnd != 0L)
			{
				IRequest request = RequestExtensions.WithJsonContent(jsonData: new Dictionary<string, object>
				{
					{ "Score", unsubmittedScore },
					{ "Multiplier", multiplier },
					{ "PrivateKey", privateKey },
					{ "TournamentId", tournamentId },
					{
						"PlayerName",
						GetNameFromSocialManager()
					}
				}, request: KilooWebClient.CreatePost(new Uri(MakeApiURL("/scores/submitscore"))));
				yield return request.SendAsync();
				JsonObject.TryGet<IDictionary>(_managerData, "currentTournament", out tour);
				if (tour != null && tournamentId == tour["id"] as string)
				{
					bool badResponse = true;
					IResponse response = request.response;
					string responseText = ((response != null) ? response.ToString() : null);
					if (request.isSuccessful && response != null && response.IsJSON())
					{
						IDictionary data;
						try
						{
							data = Json.Deserialize(responseText) as IDictionary;
						}
						catch (Exception)
						{
							data = null;
						}
						if (data != null)
						{
							if (JsonObject.Get(data, "Success", false))
							{
								tour.Remove("unsubmittedScore");
								tour["submittedScore"] = unsubmittedScore;
								_managerData["lastSubmittedScoreCountry"] = tournamentCountry;
								_managerData["lastSubmittedScoreStartTime"] = tournamentStart;
								_managerData["lastSubmittedScoreEndTime"] = tournamentEnd;
							}
							else
							{
								tour["closed"] = true;
							}
							_managerDataIsDirty = true;
							badResponse = false;
						}
					}
					else if (response != null)
					{
						if (response.status == 400)
						{
							if (responseText.Contains("Invalid PrivateKey"))
							{
								LogError("SubmitScore: Invalid User Info, discarding and retrying (once)");
								_managerData.Remove("localPlayerFacebookId");
								_managerData.Remove("localPlayerPrivateKey");
								_managerData.Remove("localPlayerPublicKey");
								_managerDataIsDirty = true;
								if (!isRetry)
								{
									_isRefreshingTournament = false;
									CoroutineUtil.StartCoroutine(RequestSubmitScoreCoroutine(multiplier, scoreToSubmit, true, true));
									yield break;
								}
							}
						}
						else if (response.status == 404)
						{
							tour["closed"] = true;
							_managerDataIsDirty = true;
						}
					}
					if (badResponse)
					{
						LogError("SubmitScore: Request failed or bad response, status: " + request.statusMessage);
					}
				}
				else
				{
					scoreWasUpdated = false;
				}
			}
			_isRefreshingTournament = false;
			if (scoreWasUpdated)
			{
				FireTournamentChangedEvent(true, false);
			}
		}

		private string GetFacebookIdFromSocialManager()
		{
			SocialManager instance = SocialManager.instance;
			if (instance.facebookIsLoggedIn)
			{
				FacebookProfile facebookProfile = instance.facebookProfile;
				if (facebookProfile != null)
				{
					return facebookProfile.id;
				}
			}
			return null;
		}

		private string GetNameFromSocialManager()
		{
			SocialManager instance = SocialManager.instance;
			if (instance.facebookIsLoggedIn)
			{
				FacebookProfile facebookProfile = instance.facebookProfile;
				if (facebookProfile != null)
				{
					return facebookProfile.name;
				}
			}
			return null;
		}

		private IEnumerator RefreshKeysIfNecessary()
		{
			string privateKey = _managerData["localPlayerPrivateKey"] as string;
			if (privateKey != null)
			{
				string facebookId = GetFacebookIdFromSocialManager();
				if (facebookId == null)
				{
					yield break;
				}
				string privateKeyFacebookId = _managerData["localPlayerFacebookId"] as string;
				if (privateKeyFacebookId != null)
				{
					if (facebookId != privateKeyFacebookId)
					{
						ClearKeys();
						yield return CoroutineUtil.StartCoroutine(LoginOrCreatePlayer());
					}
				}
				else
				{
					yield return CoroutineUtil.StartCoroutine(LoginOrRegisterFacebook());
				}
			}
			else
			{
				yield return CoroutineUtil.StartCoroutine(LoginOrCreatePlayer());
			}
		}

		private IEnumerator LoginOrRegisterFacebook()
		{
			FacebookLoginResult facebookLoginResult = FacebookLoginResult.Failed;
			yield return CoroutineUtil.StartCoroutine(RequestFacebookLogin(delegate(FacebookLoginResult result)
			{
				facebookLoginResult = result;
			}));
			if (facebookLoginResult == FacebookLoginResult.NotFound)
			{
				yield return CoroutineUtil.StartCoroutine(RequestFacebookRegister());
			}
		}

		private IEnumerator LoginOrCreatePlayer()
		{
			string facebookId = GetFacebookIdFromSocialManager();
			if (facebookId != null)
			{
				FacebookLoginResult facebookLoginResult = FacebookLoginResult.Failed;
				yield return CoroutineUtil.StartCoroutine(RequestFacebookLogin(delegate(FacebookLoginResult result)
				{
					facebookLoginResult = result;
				}));
				if (facebookLoginResult != FacebookLoginResult.NotFound)
				{
					yield break;
				}
			}
			yield return CoroutineUtil.StartCoroutine(RequestCreatePlayer());
		}

		private IEnumerator RequestFacebookLogin(Action<FacebookLoginResult> onComplete)
		{
			string facebookId = GetFacebookIdFromSocialManager();
			ulong longFbId;
			if (facebookId != null && ulong.TryParse(facebookId, NumberStyles.None, CultureInfo.InvariantCulture, out longFbId))
			{
				Action<FacebookLoginResult> onComplete2 = default(Action<FacebookLoginResult>);
				yield return RequestExtensions.WithJsonContent(jsonData: new Dictionary<string, object> { { "FacebookId", longFbId } }, request: KilooWebClient.CreatePost(new Uri(MakeApiURL("/facebook/login")))).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
					.WithMaxBackOffTime(requestMaxBackOffTime)
					.ThenMakeTheRequestAsync(delegate(IRequest request, bool isFromCache)
					{
						bool flag = true;
						FacebookLoginResult obj = FacebookLoginResult.Failed;
						IResponse response = request.response;
						string json = ((response != null) ? response.ToString() : null);
						if (request.isSuccessful && response != null && response.IsJSON())
						{
							IDictionary dictionary;
							try
							{
								dictionary = Json.Deserialize(json) as IDictionary;
							}
							catch (Exception)
							{
								dictionary = null;
							}
							if (dictionary != null)
							{
								string text = dictionary["PrivateKey"] as string;
								string text2 = dictionary["PublicKey"] as string;
								if (text != null && text2 != null)
								{
									_managerData["localPlayerFacebookId"] = facebookId;
									_managerData["localPlayerPrivateKey"] = text;
									_managerData["localPlayerPublicKey"] = text2;
									_managerDataIsDirty = true;
									obj = FacebookLoginResult.Success;
									flag = false;
								}
							}
						}
						else if (request.response != null && request.response.status == 404)
						{
							obj = FacebookLoginResult.NotFound;
							flag = false;
						}
						if (flag)
						{
							LogError("FacebookLogin: Request failed or bad response, status: " + request.statusMessage);
						}
						onComplete2(obj);
					});
			}
			else
			{
				onComplete(FacebookLoginResult.Failed);
			}
		}

		private IEnumerator RequestCreatePlayer()
		{
			string facebookId = GetFacebookIdFromSocialManager();
			ulong longFbId = 0uL;
			if (facebookId != null)
			{
				ulong.TryParse(facebookId, NumberStyles.None, CultureInfo.InvariantCulture, out longFbId);
				if (longFbId == 0L)
				{
					facebookId = null;
				}
			}
			yield return RequestExtensions.WithJsonContent(jsonData: new Dictionary<string, object> { { "FacebookId", longFbId } }, request: KilooWebClient.CreatePost(new Uri(MakeApiURL("/players/createplayer")))).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
				.WithMaxBackOffTime(requestMaxBackOffTime)
				.ThenMakeTheRequestAsync(delegate(IRequest request, bool isFromCache)
				{
					bool flag = true;
					IResponse response = request.response;
					string json = ((response != null) ? response.ToString() : null);
					if (request.isSuccessful && response != null && response.IsJSON())
					{
						IDictionary dictionary;
						try
						{
							dictionary = Json.Deserialize(json) as IDictionary;
						}
						catch (Exception)
						{
							dictionary = null;
						}
						if (dictionary != null)
						{
							string text = dictionary["PrivateKey"] as string;
							string text2 = dictionary["PublicKey"] as string;
							if (text != null && text2 != null)
							{
								_managerData["localPlayerFacebookId"] = facebookId;
								_managerData["localPlayerPrivateKey"] = text;
								_managerData["localPlayerPublicKey"] = text2;
								_managerDataIsDirty = true;
								flag = false;
							}
						}
					}
					if (flag)
					{
						LogError("CreatePlayer: Request failed or bad response, status: " + request.statusMessage);
					}
				});
		}

		private IEnumerator RequestFacebookRegister()
		{
			string privateKey = _managerData["localPlayerPrivateKey"] as string;
			string facebookId = GetFacebookIdFromSocialManager();
			ulong longFbId;
			if (privateKey != null && facebookId != null && ulong.TryParse(facebookId, NumberStyles.None, CultureInfo.InvariantCulture, out longFbId))
			{
				yield return RequestExtensions.WithJsonContent(jsonData: new Dictionary<string, object>
				{
					{ "PrivateKey", privateKey },
					{ "FacebookId", longFbId }
				}, request: KilooWebClient.CreatePost(new Uri(MakeApiURL("/facebook/register")))).WithTimeout(requestTimeout).WithMinBackOffTime(requestMinBackOffTime)
					.WithMaxBackOffTime(requestMaxBackOffTime)
					.ThenMakeTheRequestAsync(delegate(IRequest request, bool isFromCache)
					{
						bool flag = true;
						IResponse response = request.response;
						string json = ((response != null) ? response.ToString() : null);
						if (request.isSuccessful && response != null && response.IsJSON())
						{
							IDictionary dictionary;
							try
							{
								dictionary = Json.Deserialize(json) as IDictionary;
							}
							catch (Exception)
							{
								dictionary = null;
							}
							if (dictionary != null && JsonObject.Get(dictionary, "Success", false))
							{
								_managerData["localPlayerFacebookId"] = facebookId;
								_managerDataIsDirty = true;
								flag = false;
							}
						}
						if (flag)
						{
							LogError("FacebookRegister: Request failed or bad response, status: " + request.statusMessage);
						}
					});
			}
			else
			{
				LogError("FacebookRegister: Cannot register now, missing private key and/or facebook id.");
			}
		}

		private void InitializeRanksFromSaveData()
		{
			if (_rankData != null)
			{
				IDictionary dictionary = _rankData["ranks"] as IDictionary;
				if (dictionary != null)
				{
					_ranks = new Dictionary<string, Rank>(dictionary.Count);
					{
						foreach (DictionaryEntry item in dictionary)
						{
							IDictionary dictionary2 = item.Value as IDictionary;
							string text = item.Key as string;
							if (dictionary2 != null && text != null)
							{
								string value = dictionary2["country"] as string;
								int num = JsonObject.Get(dictionary2, "rank", -1);
								if (!string.IsNullOrEmpty(value) && num >= 0)
								{
									_ranks[text] = new Rank
									{
										country = value,
										rank = num
									};
								}
							}
						}
						return;
					}
				}
			}
			_ranks = new Dictionary<string, Rank>();
		}

		private void ClearKeys()
		{
			_managerData.Remove("localPlayerFacebookId");
			_managerData.Remove("localPlayerPrivateKey");
			_managerData.Remove("localPlayerPublicKey");
			_managerDataIsDirty = true;
		}

		private static object LoadJsonFile(string file)
		{
			string text = null;
			try
			{
				text = Path.Combine(DeviceUtility.GetUserDataPath(), file);
				byte[] array = FileUtil.Load(text, "GhgtrRFRfPLJhFDsWe#dEdrt5rfg56");
				string json = Encoding.UTF8.GetString(array, 0, array.Length);
				return Json.Deserialize(json);
			}
			catch (FileNotFoundException)
			{
			}
			catch (Exception ex2)
			{
				LogError("LoadJsonFile failed: " + ex2);
			}
			return null;
		}

		private static void SaveJsonFile(string file, object json)
		{
			try
			{
				string path = Path.Combine(DeviceUtility.GetUserDataPath(), file);
				if (json != null)
				{
					string s = Json.Serialize(json);
					byte[] bytes = Encoding.UTF8.GetBytes(s);
					FileUtil.Save(path, bytes, "GhgtrRFRfPLJhFDsWe#dEdrt5rfg56");
				}
				else
				{
					File.Delete(path);
				}
			}
			catch (Exception ex)
			{
				LogError("SaveJsonFile failed: " + ex);
			}
		}

		private void Save()
		{
			if (_managerDataIsDirty)
			{
				SaveJsonFile("toprun_manager", _managerData);
				_managerDataIsDirty = false;
			}
			if (_playerIdDataIsDirty)
			{
				SaveJsonFile("toprun_playerids", _playerIdData);
				_playerIdDataIsDirty = false;
			}
		}

		public static string QuoteString(string s)
		{
			if (s != null)
			{
				return '"' + s + '"';
			}
			return "null";
		}

		public static void LogError(object msg)
		{
			Debug.LogError("[TopRun.Manager] " + msg);
		}

		public static void LogWarning(object msg)
		{
			Debug.LogWarning("[TopRun.Manager] " + msg);
		}

		[Conditional("TOPRUN_DEBUG_LOGS")]
		public static void Log(object msg)
		{
			Debug.Log("[TopRun.Manager] " + msg);
		}
	}
}
