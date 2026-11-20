using System;
using System.Collections.Generic;
using UnityEngine;
using com.kiloo.awards;

public class AwardsManager
{
	private static AwardsManager _instance;

	private List<Award> _listAwards;

	private static bool _hasInited;

	public static AwardsManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new AwardsManager();
			}
			return _instance;
		}
	}

	public static bool isInitialized
	{
		get
		{
			return _instance != null;
		}
	}

	private AwardsManager()
	{
	}

	private void UpdateTheNewStatusForTheAwardsOnCleanInstall()
	{
		if (PlayerPrefs.HasKey("awardsFirstLoaded"))
		{
			return;
		}
		PlayerPrefs.SetInt("awardsFirstLoaded", 1);
		bool flag = true;
		foreach (Award listAward in _listAwards)
		{
			if (!listAward.isNew)
			{
				flag = false;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		foreach (Award listAward2 in _listAwards)
		{
			listAward2.isNew = false;
		}
	}

	public void InitTrophies()
	{
		if (_hasInited)
		{
			return;
		}
		_hasInited = true;
		Dictionary<AwardId, string> dictionary = Parse(PlayerInfo.Instance.trophyMissionData);
		if (_listAwards == null)
		{
			_listAwards = new List<Award>();
		}
		AwardStaticData[] awardStaticDataList = AwardsData.GetAwardStaticDataList();
		AwardStaticData[] array = awardStaticDataList;
		foreach (AwardStaticData awardStaticData in array)
		{
			string savedProgress = null;
			if (dictionary.ContainsKey(awardStaticData.id))
			{
				savedProgress = dictionary[awardStaticData.id];
			}
			if (awardStaticData.awardType == AwardStaticData.AwardType.Stat)
			{
				Award item = new StatAward(awardStaticData, savedProgress, OnMissionStateChanged);
				_listAwards.Add(item);
			}
			else if (awardStaticData.awardType == AwardStaticData.AwardType.DailyChallengeInRow)
			{
				Award item2 = new DailyChallengesInRowAward(awardStaticData, savedProgress, OnMissionStateChanged);
				_listAwards.Add(item2);
			}
			else if (awardStaticData.awardType == AwardStaticData.AwardType.OwnCharacters)
			{
				Award item3 = new OwnCharactersAward(awardStaticData, savedProgress, OnMissionStateChanged);
				_listAwards.Add(item3);
			}
			else if (awardStaticData.awardType == AwardStaticData.AwardType.OwnBoards)
			{
				Award item4 = new OwnBoardsAward(awardStaticData, savedProgress, OnMissionStateChanged);
				_listAwards.Add(item4);
			}
			else if (awardStaticData.awardType == AwardStaticData.AwardType.OwnCharacterOutfits)
			{
				Award item5 = new OwnCharactersOutfitsAward(awardStaticData, savedProgress, OnMissionStateChanged);
				_listAwards.Add(item5);
			}
			else if (awardStaticData.awardType == AwardStaticData.AwardType.ScorePointsSingleRunNoCoins)
			{
				Award item6 = new ScorePointsSingleRunNoCoinsAward(awardStaticData, savedProgress, OnMissionStateChanged);
				_listAwards.Add(item6);
			}
			else if (awardStaticData.awardType == AwardStaticData.AwardType.CompleteMissionsSingleRun)
			{
				Award item7 = new CompleteMissionsSingleRun(awardStaticData, savedProgress, OnMissionStateChanged);
				_listAwards.Add(item7);
			}
			else if (awardStaticData.awardType == AwardStaticData.AwardType.HaveSupersneakersXTime)
			{
				Award item8 = new SupersneakersTimeInRowAward(awardStaticData, savedProgress, OnMissionStateChanged);
				_listAwards.Add(item8);
			}
			else if (awardStaticData.awardType == AwardStaticData.AwardType.CollectCoinsSingleRun)
			{
				Award item9 = new CollectCoinsSingleRun(awardStaticData, savedProgress, OnMissionStateChanged);
				_listAwards.Add(item9);
			}
			else if (awardStaticData.awardType == AwardStaticData.AwardType.ScorePointsSingleRunNoJumpOrRoll)
			{
				Award item10 = new ScorePointsSingleRunNoJumpOrRollAward(awardStaticData, savedProgress, OnMissionStateChanged);
				_listAwards.Add(item10);
			}
			else if (awardStaticData.awardType == AwardStaticData.AwardType.ScorePointsSingleLane)
			{
				Award item11 = new ScorePointsSingleLane(awardStaticData, savedProgress, OnMissionStateChanged);
				_listAwards.Add(item11);
			}
		}
		foreach (Award listAward in _listAwards)
		{
			listAward.Activate();
		}
		UpdateTheNewStatusForTheAwardsOnCleanInstall();
		SaveAwards();
	}

	private void InitTrophiesIfNeeded()
	{
		if (!_hasInited)
		{
			InitTrophies();
		}
	}

	public void StartGame()
	{
		InitTrophiesIfNeeded();
		foreach (Award listAward in _listAwards)
		{
			if (listAward != null)
			{
				listAward.OnGameStarted();
			}
		}
	}

	public void EndGame()
	{
		InitTrophiesIfNeeded();
		foreach (Award listAward in _listAwards)
		{
			if (listAward != null)
			{
				listAward.OnGameEnded();
			}
		}
	}

	public Dictionary<AwardId, string> Parse(string s)
	{
		Dictionary<AwardId, string> dictionary = new Dictionary<AwardId, string>();
		if (!string.IsNullOrEmpty(s))
		{
			string[] array = s.Split(';');
			string[] array2 = array;
			foreach (string text in array2)
			{
				try
				{
					int num = text.IndexOf('=');
					string value = text.Substring(0, num);
					string value2 = text.Substring(num + 1);
					dictionary.Add((AwardId)(int)Enum.Parse(typeof(AwardId), value, true), value2);
				}
				catch (Exception ex)
				{
					Debug.LogError("AwardsManaged parse failed for item ERROR = " + ex);
				}
			}
		}
		return dictionary;
	}

	public override string ToString()
	{
		InitTrophiesIfNeeded();
		string[] array = new string[_listAwards.Count];
		int num = 0;
		foreach (Award listAward in _listAwards)
		{
			string text = listAward.AwardToString();
			if (text != null)
			{
				array[num] = string.Format("{0}={1}", listAward.trophyData.id.ToString(), text);
				num++;
			}
		}
		return string.Join(";", array);
	}

	private void OnMissionStateChanged(Award award)
	{
		SaveAwards();
	}

	private void SaveAwards()
	{
		PlayerInfo.Instance.trophyMissionData = ToString();
	}

	public List<Award> GetAwardsForState(AwardState state)
	{
		InitTrophiesIfNeeded();
		List<Award> list = new List<Award>();
		foreach (Award listAward in _listAwards)
		{
			if (listAward.BelongsToState(state) && ShouldInsertAwardToList(listAward))
			{
				list.Add(listAward);
			}
		}
		if (state == AwardState.Collectible || state == AwardState.Finished)
		{
			list.Sort(SortAwardByDate);
		}
		return list;
	}

	public List<Award> GetSortedListAwards(out bool containsCollectibleAwards)
	{
		List<Award> list = new List<Award>();
		containsCollectibleAwards = false;
		foreach (Award item in GetAwardsForState(AwardState.Collectible))
		{
			containsCollectibleAwards = true;
			list.Add(item);
		}
		foreach (Award item2 in GetAwardsForState(AwardState.InProgress))
		{
			list.Add(item2);
		}
		foreach (Award item3 in GetAwardsForState(AwardState.Finished))
		{
			list.Add(item3);
		}
		return list;
	}

	public int GetNumberOfNewVisibleAwards()
	{
		int num = 0;
		foreach (Award listAward in _listAwards)
		{
			if (listAward.isNew && ShouldInsertAwardToList(listAward))
			{
				num++;
			}
		}
		return num;
	}

	public int GetNumberOfFinishedAwards(AwardTierType awardTierType)
	{
		InitTrophiesIfNeeded();
		int num = 0;
		foreach (Award listAward in _listAwards)
		{
			if (listAward != null && listAward.HasCompletedTier(awardTierType))
			{
				num++;
			}
		}
		return num;
	}

	public bool AllowedToInsertFacebookElement()
	{
		Globals.LogInState loginState = Globals.GetLoginState();
		bool flag = loginState == Globals.LogInState.Facebook || loginState == Globals.LogInState.Both;
		int consolidationState = SocialManager.instance.consolidationState;
		bool flag2 = (consolidationState & 1) == 1;
		return !flag || (flag && !flag2);
	}

	public bool AllowedToInsertGameCenterElement()
	{
		Globals.LogInState loginState = Globals.GetLoginState();
		bool flag = (Application.platform == RuntimePlatform.IPhonePlayer || Application.isEditor) && (loginState == Globals.LogInState.Both || loginState == Globals.LogInState.GameCenter);
		int consolidationState = SocialManager.instance.consolidationState;
		bool flag2 = (consolidationState & 2) == 2;
		return !flag || (flag && !flag2);
	}

	private bool ShouldInsertAwardToList(Award award)
	{
		return true;
	}

	private int SortAwardByDate(Award a, Award b)
	{
		return DateTime.Compare(b.lastStateChangeDateTime, a.lastStateChangeDateTime);
	}

	public Award GetAwardForId(AwardId id)
	{
		InitTrophiesIfNeeded();
		foreach (Award listAward in _listAwards)
		{
			if (listAward.trophyData.id == id)
			{
				return listAward;
			}
		}
		return null;
	}

	public bool ShouldSpawnOldTrophyForStat(Stat stat)
	{
		InitTrophiesIfNeeded();
		foreach (Award listAward in _listAwards)
		{
			if (listAward.trophyData.awardType != AwardStaticData.AwardType.Stat)
			{
				continue;
			}
			AwardGoal[] goals = listAward.goals;
			AwardGoal[] array = goals;
			foreach (AwardGoal awardGoal in array)
			{
				AwardStatGoal awardStatGoal = awardGoal as AwardStatGoal;
				if (awardStatGoal != null && stat == awardStatGoal.stat)
				{
					if (listAward.awardState != AwardState.InProgress)
					{
						return false;
					}
					return !listAward.isGoalAccomplished(awardStatGoal);
				}
			}
		}
		return false;
	}
}
