using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RewardManager
{
	public static bool canShowMultipleQueuedCelebrations;

	public static int rewardsToUnlockCount
	{
		get
		{
			return PlayerInfo.Instance.pendingRewards.Count;
		}
	}

	public static void AddRewardToUnlock(CelebrationRewardOrigin origin, bool shouldSaveToDisk = true)
	{
		CelebrationReward celebrationReward = null;
		switch (origin)
		{
		case CelebrationRewardOrigin.MysteryBox:
		case CelebrationRewardOrigin.WeeklyChallengeMysteryBox:
			celebrationReward = MysteryBox.Roll(MysteryBox.Type.Normal);
			break;
		case CelebrationRewardOrigin.SuperMysteryBox:
		case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
			celebrationReward = MysteryBox.Roll(MysteryBox.Type.Super);
			break;
		case CelebrationRewardOrigin.MysteryBoxMini:
			celebrationReward = MysteryBox.Roll(MysteryBox.Type.Mini);
			break;
		default:
			Debug.LogWarning("RewardManger Use this method for unknown prize only: MB, SMB,WCMB,WCSMB and MMB.");
			break;
		}
		if (celebrationReward != null)
		{
			celebrationReward.CelebrationRewardOrigin = origin;
			AddMysteryBoxToUnlock(celebrationReward, shouldSaveToDisk);
		}
	}

	public static void AddRewardToUnlock(CelebrationReward reward, bool shouldSaveToDisk = true)
	{
		if (reward.CelebrationRewardOrigin != CelebrationRewardOrigin.Notset && reward.rewardType != CelebrationRewardType._notset)
		{
			AddMysteryBoxToUnlock(reward, shouldSaveToDisk);
		}
		else
		{
			Debug.LogWarning("RewardManager You can't add a not set reward to unlock");
		}
	}

	private static void AddMysteryBoxToUnlock(CelebrationReward reward, bool shouldSaveToDisk = true)
	{
		PlayerInfo instance = PlayerInfo.Instance;
		bool flag = instance.pendingRewards.Exists((CelebrationReward mb) => mb.Uid == reward.Uid);
		if (reward.Uid <= 0 || !flag)
		{
			reward.Uid = DateTime.Now.Ticks + UnityEngine.Random.Range(0, int.MaxValue);
		}
		if (!flag)
		{
			instance.AddPendingReward(reward);
			if (shouldSaveToDisk)
			{
				instance.SaveIfDirty();
			}
		}
	}

	public static CelebrationReward[] GetRewardsToUnlockForCelebration()
	{
		if (PlayerInfo.Instance.lastAddedReward != null && !canShowMultipleQueuedCelebrations)
		{
			return new CelebrationReward[1] { PlayerInfo.Instance.lastAddedReward };
		}
		return PlayerInfo.Instance.pendingRewards.ToArray();
	}

	public static void RewardPayedOut(CelebrationReward celebrationReward)
	{
		PlayerInfo.Instance.RemovePendingReward(celebrationReward);
	}

	public static List<CelebrationReward> GetWeeklyHuntRewardsToUnlock()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		List<CelebrationReward> list = new List<CelebrationReward>();
		for (int i = 0; i < instance.pendingRewards.Count; i++)
		{
			CelebrationReward celebrationReward = instance.pendingRewards[i];
			if (celebrationReward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeNormal)
			{
				list.Add(celebrationReward);
			}
		}
		return list;
	}

	public static void PayoutNonMysteryBoxRewards()
	{
		List<CelebrationReward> source = new List<CelebrationReward>(PlayerInfo.Instance.pendingRewards);
		source = source.Where((CelebrationReward celebrationReward) => !IsRewardMysteryBox(celebrationReward)).ToList();
		foreach (CelebrationReward item in source)
		{
			UIScreenController.Instance.PayoutCelebrationReward(item);
		}
	}

	private static bool IsRewardMysteryBox(CelebrationReward reward)
	{
		return reward.CelebrationRewardOrigin == CelebrationRewardOrigin.MysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.SuperMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterEventSuperMysteryBox;
	}
}
