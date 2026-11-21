#define ENABLE_ERROR_LOGS
using System.Collections.Generic;
using System.Diagnostics;
using SYBO.Subway.Characters;
using SYBO.Subway.Info;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MissionsManager
{
	public delegate void MissionSetCompleteHandler();

	public delegate void MissionCompleteHandler(string msg);

	private int _currentMissionTemplateSetLoaded = -1;

	private MissionTemplate[] _templates;

	private int[] _currentRunProgress;

	private int _currentMissionSetLoaded = -1;

	private Mission[] _combinedArray;

	public MissionSetCompleteHandler onMissionSetComplete;

	public MissionCompleteHandler onMissionComplete;

	private Mission[][] _missions;

	private static MissionsManager _instance;

	private PlayerInfo playerinfo;

	private MissionTemplate[] templates
	{
		get
		{
			if (_currentMissionTemplateSetLoaded != playerinfo.currentMissionSet || _templates == null)
			{
				int num = 3;
				Dictionary<MissionType, MissionTemplate> missionTemplates = MissionsData.missionTemplates;
				if (_templates == null)
				{
					_templates = new MissionTemplate[44];
				}
				for (int i = 0; i < num; i++)
				{
					_templates[i] = missionTemplates[missions[playerinfo.currentMissionSet][i].type];
				}
				if (_currentMissionTemplateSetLoaded == -1)
				{
					for (int j = 0; j < 41; j++)
					{
						_templates[j + num] = missionTemplates[Achievements.Instance.achievementArray[j].type];
					}
				}
				_currentMissionTemplateSetLoaded = playerinfo.currentMissionSet;
			}
			return _templates;
		}
	}

	public bool inRun
	{
		get
		{
			return _currentRunProgress != null;
		}
		set
		{
			if (value)
			{
				_currentRunProgress = new int[combinedArray.Length];
			}
			else
			{
				if (_currentRunProgress == null)
				{
					return;
				}
				if (UIScreenController.Instance.GetTopScreenName() != "PauseUI")
				{
					for (int i = 0; i < combinedArray.Length; i++)
					{
						if (templates[i].singleRun && templates[i].completeIfLess && _currentRunProgress[i] < combinedArray[i].goal)
						{
							playerinfo.SetCurrentMissionProgress(i, combinedArray[i].goal);
							Complete(i);
						}
					}
				}
				PlayerDidThis(MissionTarget.StayInOneLane, (int)(Time.time - Character.Instance.sameLaneTimeStamp));
				RemoveProgressForThis(MissionTarget.StayInOneLane);
				CheckAllCompleteAndIncrement();
				SyncAchievements();
				if (GameStats.Instance.coins == 50)
				{
					SocialManager.instance.ReportAchievement("CollectExactly50Coins", 100f);
				}
				_currentRunProgress = null;
			}
		}
	}

	private Mission[] combinedArray
	{
		get
		{
			if (_currentMissionSetLoaded != playerinfo.currentMissionSet || _combinedArray == null)
			{
				if (_combinedArray == null)
				{
					_combinedArray = new Mission[44];
				}
				for (int i = 0; i < 3; i++)
				{
					_combinedArray[i] = missions[playerinfo.currentMissionSet][i];
				}
				if (_currentMissionSetLoaded == -1)
				{
					for (int j = 0; j < 41; j++)
					{
						_combinedArray[j + 3] = Achievements.Instance.achievementArray[j];
					}
				}
			}
			_currentMissionSetLoaded = currentMissionSet;
			return _combinedArray;
		}
	}

	public int currentMissionSet
	{
		get
		{
			return playerinfo.currentMissionSet;
		}
		set
		{
			if (value != playerinfo.currentMissionSet)
			{
				if (value >= missionSetCount)
				{
					int num = playerinfo.currentMissionSet - MissionsData.repeatableMissions.Length + 1;
					playerinfo.InitCurrentMissionSet(num, missions[num].Length, true);
				}
				else
				{
					playerinfo.InitCurrentMissionSet(value, missions[value].Length, true);
				}
			}
		}
	}

	private Mission[][] missions
	{
		get
		{
			if (_missions == null)
			{
				Mission[][] repeatableMissions = MissionsData.repeatableMissions;
				Mission[][] storylineMissions = MissionsData.storylineMissions;
				_missions = new Mission[GetNumberOfBasicMission() + repeatableMissions.Length][];
				for (int i = 0; i < GetNumberOfBasicMission(); i++)
				{
					_missions[i] = storylineMissions[i];
				}
				for (int j = 0; j < repeatableMissions.Length; j++)
				{
					_missions[GetNumberOfBasicMission() + j] = repeatableMissions[j];
				}
			}
			return _missions;
		}
	}

	public int missionSetCount
	{
		get
		{
			return missions.Length;
		}
	}

	public int missionSetStoryCount
	{
		get
		{
			return missions.Length - MissionsData.repeatableMissions.Length;
		}
	}

	public static MissionsManager Instance
	{
		get
		{
			return _instance ?? (_instance = new MissionsManager());
		}
	}

	private MissionsManager()
	{
		playerinfo = PlayerInfo.Instance;
		if (playerinfo.currentMissionSet == -1)
		{
			playerinfo.InitCurrentMissionSet(0, missions[0].Length, true);
		}
		ApplyChangesToProgressIfNecessary();
	}

	public void SkipMission(int missionNumber)
	{
		PlayerDidThis(MissionTarget.SkipMission);
		playerinfo.SetCurrentMissionProgress(missionNumber, missions[playerinfo.currentMissionSet][missionNumber].goal);
		Complete(missionNumber);
	}

	private void Complete(int mission, float completedFactor = 1f, bool sendEvenIfInRun = false)
	{
		int num = 3;
		if (mission < num)
		{
			if (completedFactor != 1f)
			{
				return;
			}
			MissionCompleteHandler missionCompleteHandler = onMissionComplete;
			if (missionCompleteHandler != null)
			{
				string format = ((combinedArray[mission].goal != 1) ? Strings.Get(templates[mission].ultraShortDescription) : Strings.Get(templates[mission].ultraShortDescriptionSingle));
				missionCompleteHandler(string.Format(format, combinedArray[mission].goal));
			}
			Statistics stats;
			Statistics statistics = (stats = PlayerInfo.Instance.stats);
			Stat stat2;
			Stat stat = (stat2 = Stat.MissionCompleted);
			int num2 = stats[stat2];
			statistics[stat] = num2 + 1;
			bool flag = true;
			int num3 = 0;
			for (int i = 0; i < 3; i++)
			{
				int currentMissionProgress = playerinfo.GetCurrentMissionProgress(i);
				if (currentMissionProgress >= combinedArray[i].goal)
				{
					num3++;
				}
				if (currentMissionProgress < combinedArray[i].goal)
				{
					flag = false;
					break;
				}
			}
			if (num3 >= 2 && inRun)
			{
				SocialManager.instance.ReportAchievement(Achievements.Instance.achievementIds[36], Mathf.Clamp01(1f) * 100f);
			}
			if (!flag)
			{
				return;
			}
			Flurry.LogEventWithAParameter("Mission Set completed", "Mission Set", (PlayerInfo.Instance.missionCompletedSum + 1).ToString());
			PlayerDidThis(MissionTarget.MissionSet);
			if (_currentRunProgress != null)
			{
				for (int j = 0; j < num; j++)
				{
					if (_currentRunProgress[j] != 0)
					{
						_currentRunProgress[j] = 0;
					}
				}
			}
			if (currentMissionSet + 1 == missionSetStoryCount)
			{
				playerinfo.shouldShowMission2Popup = true;
			}
			if (currentMissionSet + 1 > missionSetStoryCount)
			{
				RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.SuperMysteryBox);
				if (!inRun)
				{
					UIScreenController.Instance.QueueMysteryBox();
				}
			}
			int num4 = ((currentMissionSet < missionSetCount - 1) ? (playerinfo.currentMissionSet + 1) : (playerinfo.currentMissionSet - MissionsData.repeatableMissions.Length + 1));
			playerinfo.missionCompletedSum++;
			int missionCount = 0;
			if (num4 < missionSetCount)
			{
				missionCount = missions[num4].Length;
			}
			playerinfo.InitCurrentMissionSet(num4, missionCount, true);
			MissionSetCompleteHandler missionSetCompleteHandler = onMissionSetComplete;
			if (missionSetCompleteHandler != null)
			{
				missionSetCompleteHandler();
			}
			PlayerDidThis(MissionTarget.ReachMissionSet);
		}
		else if (!inRun || sendEvenIfInRun)
		{
			int num5 = mission - num;
			if (num5 < 0 || num5 >= 41)
			{
				LogError("Missions.Complete: achievement index is out of bounds");
			}
			SocialManager.instance.ReportAchievement(Achievements.Instance.achievementIds[num5], Mathf.Clamp01(completedFactor) * 100f);
		}
	}

	public void RemoveProgressForThis(MissionTarget myTask)
	{
		if (templates == null)
		{
			LogError("currentTemplates == null");
		}
		for (int i = 0; i < combinedArray.Length; i++)
		{
			if (templates[i].missionTarget == myTask)
			{
				ResetProgressForMissionIndex(i);
			}
		}
	}

	private void CheckAllCompleteAndIncrement()
	{
		for (int i = 3; i < combinedArray.Length; i++)
		{
			if (templates[i].singleRun && templates[i].completeIfLess)
			{
				if (_currentRunProgress[i] < combinedArray[i].goal)
				{
					playerinfo.SetCurrentMissionProgress(i, combinedArray[i].goal);
					Complete(i, 1f, true);
				}
			}
			else if (_currentRunProgress != null)
			{
				if ((float)_currentRunProgress[i] / (float)combinedArray[i].goal > (float)playerinfo.GetCurrentMissionProgress(i) / (float)combinedArray[i].goal)
				{
					Complete(i, (float)_currentRunProgress[i] / (float)combinedArray[i].goal, true);
				}
				else
				{
					Complete(i, (float)playerinfo.GetCurrentMissionProgress(i) / (float)combinedArray[i].goal, true);
				}
			}
			else
			{
				Complete(i, (float)playerinfo.GetCurrentMissionProgress(i) / (float)combinedArray[i].goal, true);
			}
		}
	}

	private void OverWriteAchievementsProgress(MissionTarget myTask, int overWrite)
	{
		for (int i = 3; i < combinedArray.Length; i++)
		{
			if (templates[i].missionTarget == myTask)
			{
				playerinfo.SetCurrentMissionProgress(i, overWrite);
			}
		}
	}

	private void ResetProgressForMissionIndex(int questIndex)
	{
		if (!GetMissionInfo(questIndex).complete)
		{
			playerinfo.SetCurrentMissionProgress(questIndex, 0);
			if (_currentRunProgress != null)
			{
				_currentRunProgress[questIndex] = 0;
			}
		}
	}

	private void ApplyChangesForVersion(int versionNumber)
	{
		int numberOfBasicMission = GetNumberOfBasicMission();
		if (currentMissionSet < numberOfBasicMission)
		{
			List<MissionReplacement> missionsModificationsForStoryline = MissionsData.missionsModificationsForStoryline;
			for (int i = 0; i < missionsModificationsForStoryline.Count; i++)
			{
				MissionReplacement missionReplacement = missionsModificationsForStoryline[i];
				if (missionReplacement.versionNo == versionNumber && currentMissionSet == missionReplacement.missionSet - 1)
				{
					int missionNumberInSet = missionReplacement.missionNumberInSet;
					playerinfo.SetCurrentMissionProgress(missionNumberInSet, 0);
					if (_currentRunProgress != null)
					{
						_currentRunProgress[missionNumberInSet] = 0;
					}
				}
			}
			return;
		}
		int[] array = MissionsData.missionsInsertedForRepeatable[versionNumber];
		int num = currentMissionSet - numberOfBasicMission;
		int num2 = num;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j] - 1 <= num2)
			{
				num2++;
			}
		}
		if (Application.isEditor)
		{
			for (int k = 0; k < array.Length - 1; k++)
			{
				if (array[k] >= array[k + 1])
				{
					LogError("The missionsInsertedForRepeatable (in MissionsData) dictionary contains unsorted arrays as values ");
				}
			}
		}
		int num3 = num2 - num;
		int num4 = Mathf.Clamp(currentMissionSet + num3, 0, missionSetCount);
		if (missions.Length > num4)
		{
			playerinfo.InitCurrentMissionSet(num4, missions[num4].Length, false);
		}
	}

	private void ApplyChangesToProgressIfNecessary()
	{
		int missionsVersionLastUpdate = playerinfo.missionsVersionLastUpdate;
		int num = 1;
		if (missionsVersionLastUpdate < num)
		{
			for (int i = missionsVersionLastUpdate + 1; i <= num; i++)
			{
				ApplyChangesForVersion(i);
			}
		}
		playerinfo.missionsVersionLastUpdate = 1;
	}

	public void OnChangeIsCharacterOnGround(Transform characterTransform)
	{
		if (characterTransform.localPosition.y < 1f)
		{
			RemoveProgressForThis(MissionTarget.EarnCoinWithoutTouchingGround);
		}
	}

	private void SyncAchievements()
	{
		OverWriteAchievementsProgress(MissionTarget.ReachMissionSet, playerinfo.currentMissionSet);
		OverWriteAchievementsProgress(MissionTarget.HaveUpgrades, playerinfo.GetUpgradeTierSum());
		OverWriteAchievementsProgress(MissionTarget.HaveTrophiesFirstAchievements, playerinfo.NumberOfUnlockedTrophyForFirstAchievement());
		OverWriteAchievementsProgress(MissionTarget.FacebookLoggedIn, SocialManager.instance.facebookIsLoggedIn.CompareTo(false));
		int achievementIndexInArray = Achievements.Instance.getAchievementIndexInArray(MissionType.HaveHeadStartLarge);
		if (achievementIndexInArray > -1 && playerinfo.GetUpgradeAmount(PowerupType.headstart2000) >= Achievements.Instance.achievementArray[achievementIndexInArray].goal)
		{
			SocialManager.instance.ReportAchievement(Achievements.Instance.achievementIds[achievementIndexInArray], 100f);
		}
		foreach (KeyValuePair<Characters.CharacterType, Characters.Model> characterDatum in Characters.characterData)
		{
			if (characterDatum.Value.missionTargetKey != MissionTarget.none)
			{
				OverWriteAchievementsProgress(characterDatum.Value.missionTargetKey, playerinfo.GetCollectedTokens(characterDatum.Key));
			}
		}
	}

	public bool IsMissionTargetActive(MissionTarget target)
	{
		for (int i = 0; i < combinedArray.Length; i++)
		{
			if (templates[i].missionTarget == target)
			{
				int currentMissionProgress = playerinfo.GetCurrentMissionProgress(i);
				return currentMissionProgress < combinedArray[i].goal;
			}
		}
		return false;
	}

	public void PlayerDidThisCoinMission(MissionTarget missionTarget)
	{
		if (PlayerInfo.Instance.hasDoubleCoins)
		{
			PlayerDidThis(missionTarget, (!DoubleCoins.mysteryDoubleCoinsEnabled) ? 2 : 4);
		}
		else
		{
			PlayerDidThis(missionTarget, (!DoubleCoins.mysteryDoubleCoinsEnabled) ? 1 : 2);
		}
	}

	public void PlayerDidThis(MissionTarget myTask, int magnitude = 1, int missionToIgnore = -1)
	{
		if (templates == null)
		{
			LogError("currentTemplates == null");
		}
		for (int i = 0; i < combinedArray.Length; i++)
		{
			if (i == missionToIgnore || (templates[i].singleRun && !inRun) || templates[i].missionTarget != myTask)
			{
				continue;
			}
			int num = playerinfo.GetCurrentMissionProgress(i);
			if (templates[i].singleRun && inRun && num < combinedArray[i].goal && _currentRunProgress != null)
			{
				num = _currentRunProgress[i];
			}
			int num2 = num + magnitude;
			if (templates[i].completeIfLess)
			{
				if (num2 > combinedArray[i].goal)
				{
					if (templates[i].singleRun && inRun)
					{
						if (_currentRunProgress != null)
						{
							_currentRunProgress[i] = num2;
						}
						else
						{
							LogError("_currentRunProgress is null - PlayerDidThis called outside a run for a singleRun mission with MissionTarget: " + myTask);
						}
					}
					else
					{
						playerinfo.SetCurrentMissionProgress(i, num2);
						Complete(i, (float)num2 / (float)combinedArray[i].goal);
					}
					continue;
				}
				if (templates[i].singleRun)
				{
					if (_currentRunProgress != null)
					{
						_currentRunProgress[i] = num2;
					}
					else
					{
						LogError("_currentRunProgress is null - PlayerDidThis called outside a run for a singleRun mission with MissionTarget: " + myTask);
					}
				}
				playerinfo.SetCurrentMissionProgress(i, combinedArray[i].goal);
				Complete(i);
			}
			else if (templates[i].completeIfEqual)
			{
				if (myTask == MissionTarget.GetExactlyAmountOfCoins)
				{
					num2 = magnitude;
				}
				if (num2 == combinedArray[i].goal)
				{
					playerinfo.SetCurrentMissionProgress(i, missions[playerinfo.currentMissionSet][i].goal);
					Complete(i);
				}
			}
			else if (num2 < combinedArray[i].goal)
			{
				if (templates[i].singleRun)
				{
					if (_currentRunProgress != null)
					{
						_currentRunProgress[i] = num2;
					}
					else
					{
						LogError("_currentRunProgress is null - PlayerDidThis called outside a run for a singleRun mission with MissionTarget: " + myTask);
					}
					continue;
				}
				if (_currentRunProgress != null)
				{
					_currentRunProgress[i] = num2;
				}
				playerinfo.SetCurrentMissionProgress(i, num2);
				Complete(i, (float)num2 / (float)combinedArray[i].goal);
			}
			else
			{
				playerinfo.SetCurrentMissionProgress(i, combinedArray[i].goal);
				if (num < combinedArray[i].goal)
				{
					Complete(i);
				}
			}
		}
	}

	public MissionInfo[] GetMissionInfo()
	{
		MissionInfo[] array = new MissionInfo[combinedArray.Length];
		for (int i = 0; i < combinedArray.Length; i++)
		{
			GetMissionInfo(i, ref array[i]);
		}
		return array;
	}

	public MissionInfo GetMissionInfo(int missionNumber)
	{
		MissionInfo info = default(MissionInfo);
		GetMissionInfo(missionNumber, ref info);
		return info;
	}

	public void GetMissionInfo(int missionNumber, ref MissionInfo info)
	{
		info.mission = combinedArray[missionNumber];
		info.template = templates[missionNumber];
		info.progress = playerinfo.GetCurrentMissionProgress(missionNumber);
		info.complete = info.progress >= info.mission.goal;
		if (!info.complete && info.template.singleRun && inRun)
		{
			info.progress = _currentRunProgress[missionNumber];
		}
	}

	[Conditional("ENABLE_MISSIONMANAGER_DEBUG")]
	private void TestGoToMissionSum(int mission)
	{
		int num = ((mission <= MissionsData.storylineMissions.Length) ? mission : ((mission - MissionsData.storylineMissions.Length) % MissionsData.repeatableMissions.Length + MissionsData.storylineMissions.Length));
		int missionCount = 0;
		if (num < missionSetCount)
		{
			missionCount = missions[num].Length;
		}
		playerinfo.InitCurrentMissionSet(num - 1, missionCount, true);
	}

	public int GetCurrentRank()
	{
		return playerinfo.GetCurrentRank();
	}

	public int GetNumberOfBasicMission()
	{
		return MissionsData.storylineMissions.Length;
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, Object context = null)
	{
		Debug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("ENABLE_MISSIONMANAGER_LOGS")]
	private static void Log(string msg, Object context = null)
	{
		Debug.Log(msg, context);
	}
}
