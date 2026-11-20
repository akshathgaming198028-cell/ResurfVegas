using System.Collections.Generic;
using TopRun;

public static class TopRunRewardHelper
{
	private static int _amountOfFriendsBeaten;

	public static void PrepareAndCallCelebrationScreen(IManager manager)
	{
		CelebrationReward pendingReward = GetPendingReward();
		RewardManager.AddRewardToUnlock(pendingReward);
		PlayerInfo.Instance.lastTopRunResultAwardedID = PlayerInfo.Instance.pendingTopRunResultID;
		UIScreenController.Instance.QueuePopup("CelebrationPopup");
	}

	private static void GivePendingRewardSilently(PlayerInfo playerInfo)
	{
		CelebrationReward pendingReward = GetPendingReward();
		TrackRewardStats(pendingReward);
		playerInfo.lastTopRunResultAwardedID = playerInfo.pendingTopRunResultID;
		TopRunEventsHandler.instance.TriggerOnTopRunChanged(TopRunEventsHandler.instance, new TournamentChangedEventArgs());
	}

	public static bool ShouldShowResults()
	{
		return !string.IsNullOrEmpty(PlayerInfo.Instance.pendingTopRunResultID) && !string.Equals(PlayerInfo.Instance.pendingTopRunResultID, PlayerInfo.Instance.lastTopRunResultAwardedID);
	}

	private static CelebrationReward GetPendingReward()
	{
		CelebrationReward celebrationReward = new CelebrationReward();
		PlayerInfo instance = PlayerInfo.Instance;
		celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.TopRun;
		celebrationReward.rewardType = CelebrationRewardType.topRun;
		celebrationReward.rank = instance.pendingTopRunResultsRank + 1;
		celebrationReward.score = instance.pendingTopRunResultScore;
		celebrationReward.weekNumber = instance.pendingTopRunResultsWeek;
		return celebrationReward;
	}

	public static void RefreshPendingResults()
	{
		IManager manager = TopRunRoot.manager;
		PlayerInfo instance = PlayerInfo.Instance;
		if (manager == null || manager.tournament == null || instance == null || !manager.tournament.hasResult || manager.score <= 0)
		{
			return;
		}
		if (!string.IsNullOrEmpty(instance.pendingTopRunResultID))
		{
			if (instance.pendingTopRunResultID != manager.tournament.id)
			{
				CalculateBeatenFriendsAward();
				if (instance.lastTopRunResultAwardedID != instance.pendingTopRunResultID)
				{
					GivePendingRewardSilently(instance);
					OverridePendingResults(manager, instance);
				}
				else
				{
					OverridePendingResults(manager, instance);
				}
			}
		}
		else
		{
			OverridePendingResults(manager, instance);
		}
	}

	private static void CalculateBeatenFriendsAward()
	{
		IManager manager = TopRunRoot.manager;
		if (manager == null || manager.tournament == null)
		{
			return;
		}
		List<IPlayer> list = new List<IPlayer>();
		list.AddRange(manager.tournament.GetPlayers());
		List<IPlayer> list2 = new List<IPlayer>();
		foreach (IPlayer item in list)
		{
			if (item.isFriend)
			{
				list2.Add(item);
			}
		}
		foreach (IPlayer item2 in list2)
		{
			if (manager.score > item2.score)
			{
				_amountOfFriendsBeaten++;
			}
		}
	}

	private static void OverridePendingResults(IManager manager, PlayerInfo playerInfo)
	{
		playerInfo.pendingTopRunResultID = manager.tournament.id;
		playerInfo.pendingTopRunResultsWeek = manager.tournament.sequenceId;
		playerInfo.pendingTopRunResultScore = manager.score;
		playerInfo.pendingTopRunResultsRank = manager.tournament.GetRankForScore(manager.score);
		playerInfo.pendingTopRunBeatenFriendsAward = _amountOfFriendsBeaten;
	}

	public static void TrackRewardStats(CelebrationReward reward)
	{
		Stat stat = Stat.NotSet;
		bool flag = false;
		switch (reward.rank)
		{
		case 1:
			flag = true;
			stat = Stat.TopRunRank0;
			break;
		case 2:
			flag = true;
			stat = Stat.TopRunRank1;
			break;
		case 3:
			flag = true;
			stat = Stat.TopRunRank2;
			break;
		case 4:
			flag = true;
			stat = Stat.TopRunRank3;
			break;
		case 5:
			stat = Stat.TopRunRank4;
			break;
		case 6:
			stat = Stat.TopRunRank5;
			break;
		case 7:
			stat = Stat.TopRunRank6;
			break;
		case 8:
			stat = Stat.TopRunRank7;
			break;
		case 9:
			stat = Stat.TopRunRank8;
			break;
		case 10:
			stat = Stat.TopRunRank9;
			break;
		case 11:
			stat = Stat.TopRunRank10;
			break;
		}
		Statistics stats;
		Statistics statistics = (stats = PlayerInfo.Instance.stats);
		Stat stat3;
		Stat stat2 = (stat3 = stat);
		int num = stats[stat3];
		statistics[stat2] = num + 1;
		if (flag)
		{
			Statistics stats2;
			Statistics statistics2 = (stats2 = PlayerInfo.Instance.stats);
			Stat stat4 = (stat3 = Stat.TopRunRank5orBetter);
			num = stats2[stat3];
			statistics2[stat4] = num + 1;
		}
	}
}
