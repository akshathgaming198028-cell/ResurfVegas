using System;
using System.Collections.Generic;
using System.Text;
using SYBO.Subway;
using SYBO.Subway.Info;
using UnityEngine;

public static class WeeklyHuntManager
{
	public enum WeeklyHuntPrize
	{
		NotAvailable = 0,
		Key = 1,
		SuperMysteryBox = 2,
		Hoverboard = 3,
		ScoreBooster = 4,
		Headstart = 5,
		ChristmasBoard = 6
	}

	public enum PrizePeriodId
	{
		None = 0,
		PeriodOne = 1,
		PeriodTwo = 2,
		PeriodThree = 3,
		PeriodFour = 4
	}

	public enum PeriodStatus
	{
		Unstarted = 0,
		Expired = 1,
		Current = 2
	}

	public struct WeeklyHuntPeriodData
	{
		public int itemsToColect;

		public int itemsCollected;

		public bool hiddenUntilUnlocked;

		public StringID periodTitle;

		public DateTime periodStartDate;

		public DateTime periodEndDate;

		public PrizeData mainPrize;

		public PrizeData alternatePrize;

		public bool hasAlternatePrize;

		public Func<bool> alternatePrizeCondition;
	}

	public struct PrizeData
	{
		public int amount;

		public CelebrationRewardOrigin CelebrationRewardOrigin;

		public CelebrationRewardType rewardType;

		public PowerupType powerupType;

		public Characters.CharacterType characterType;

		public Hoverboards.BoardType boardType;

		public OldTrophies.Trophy trophyType;
	}

	private static DateTime START_CHALLANGE_DATE_TIME = DateTime.MinValue;

	private static bool _hasInited = false;

	public static bool HIDE_LAST_PERIOD = true;

	public static Vector3 HUNT_ITEM_POSITION_OFFSET = new Vector3(7.38f, 0.76f, 0f);

	public static Vector3 HUNT_ITEM_ROTATION = new Vector3(1f, 145f, -4f);

	public static Vector3 HUNT_ITEM_SCALE = new Vector3(14f, 14f, 14f);

	public static bool _canCollectAllTokensInOneRun = false;

	private static Dictionary<PrizePeriodId, WeeklyHuntPeriodData> huntPeriodData = new Dictionary<PrizePeriodId, WeeklyHuntPeriodData>
	{
		{
			PrizePeriodId.PeriodOne,
			new WeeklyHuntPeriodData
			{
				itemsToColect = 40,
				itemsCollected = 0,
				hiddenUntilUnlocked = false,
				periodTitle = StringID.WEEKLY_HUNT_PERIOD_ONE_PERIOD_TITLE,
				periodStartDate = new DateTime(2016, 5, 12),
				periodEndDate = new DateTime(2016, 6, 9),
				mainPrize = new PrizeData
				{
					amount = 25,
					CelebrationRewardOrigin = CelebrationRewardOrigin.WeeklyChallengeNormal,
					rewardType = CelebrationRewardType.powerup,
					powerupType = PowerupType.hoverboard
				},
				hasAlternatePrize = false
			}
		},
		{
			PrizePeriodId.PeriodTwo,
			new WeeklyHuntPeriodData
			{
				itemsToColect = 10,
				itemsCollected = 0,
				hiddenUntilUnlocked = false,
				periodTitle = StringID.WEEKLY_HUNT_PERIOD_TWO_PERIOD_TITLE,
				periodStartDate = new DateTime(2016, 6, 9),
				periodEndDate = new DateTime(2016, 6, 16),
				mainPrize = new PrizeData
				{
					amount = 3,
					CelebrationRewardOrigin = CelebrationRewardOrigin.WeeklyChallengeNormal,
					rewardType = CelebrationRewardType.keys
				},
				hasAlternatePrize = false
			}
		},
		{
			PrizePeriodId.PeriodThree,
			new WeeklyHuntPeriodData
			{
				itemsToColect = 20,
				itemsCollected = 0,
				hiddenUntilUnlocked = false,
				periodTitle = StringID.WEEKLY_HUNT_PERIOD_THREE_PERIOD_TITLE,
				periodStartDate = new DateTime(2016, 6, 16),
				periodEndDate = new DateTime(2016, 6, 23),
				mainPrize = new PrizeData
				{
					amount = 40,
					CelebrationRewardOrigin = CelebrationRewardOrigin.WeeklyChallengeNormal,
					rewardType = CelebrationRewardType.powerup,
					powerupType = PowerupType.hoverboard
				},
				hasAlternatePrize = false
			}
		},
		{
			PrizePeriodId.PeriodFour,
			new WeeklyHuntPeriodData
			{
				itemsToColect = 10,
				itemsCollected = 0,
				hiddenUntilUnlocked = false,
				periodTitle = StringID.WEEKLY_HUNT_PERIOD_FOUR_PERIOD_TITLE,
				periodStartDate = new DateTime(2016, 3, 3),
				periodEndDate = new DateTime(2016, 3, 10),
				mainPrize = new PrizeData
				{
					amount = 2,
					CelebrationRewardOrigin = CelebrationRewardOrigin.WeeklyChallengeNormal,
					rewardType = CelebrationRewardType.powerup,
					powerupType = PowerupType.scorebooster
				},
				hasAlternatePrize = false
			}
		}
	};

	public static event Action ItemCountUpdated;

	public static bool IsAnyPeriodActive()
	{
		InitIfNeeded();
		PrizePeriodId activePeriode = GetActivePeriode();
		if (activePeriode != PrizePeriodId.None)
		{
			WeeklyHuntPeriodData weeklyHuntPeriodData = huntPeriodData[activePeriode];
			if (weeklyHuntPeriodData.itemsCollected < weeklyHuntPeriodData.itemsToColect)
			{
				if (weeklyHuntPeriodData.itemsCollected == 0)
				{
					_canCollectAllTokensInOneRun = true;
				}
				return true;
			}
			return false;
		}
		return false;
	}

	public static bool IsHuntStillActive()
	{
		InitIfNeeded();
		WeeklyHuntPeriodData weeklyHuntPeriodData = huntPeriodData[PrizePeriodId.PeriodOne];
		if (DateTime.Compare(((!HIDE_LAST_PERIOD) ? huntPeriodData[PrizePeriodId.PeriodFour] : huntPeriodData[PrizePeriodId.PeriodThree]).periodEndDate, DateTime.Now) >= 0 && DateTime.Compare(weeklyHuntPeriodData.periodStartDate, DateTime.Now) <= 0)
		{
			return true;
		}
		return false;
	}

	public static PrizePeriodId GetActivePeriode()
	{
		InitIfNeeded();
		foreach (KeyValuePair<PrizePeriodId, WeeklyHuntPeriodData> huntPeriodDatum in huntPeriodData)
		{
			PrizePeriodId key = huntPeriodDatum.Key;
			WeeklyHuntPeriodData value = huntPeriodDatum.Value;
			DateTime periodEndDate = value.periodEndDate;
			DateTime periodStartDate = value.periodStartDate;
			if (DateTime.Compare(periodEndDate, DateTime.Now) >= 0 && DateTime.Compare(periodStartDate, DateTime.Now) <= 0 && (huntPeriodDatum.Key != PrizePeriodId.PeriodFour || !HIDE_LAST_PERIOD))
			{
				return key;
			}
		}
		return PrizePeriodId.None;
	}

	public static PeriodStatus GetPeriodStatus(PrizePeriodId period)
	{
		InitIfNeeded();
		WeeklyHuntPeriodData weeklyHuntPeriodData = huntPeriodData[period];
		DateTime periodEndDate = weeklyHuntPeriodData.periodEndDate;
		DateTime periodStartDate = weeklyHuntPeriodData.periodStartDate;
		if (DateTime.Compare(periodEndDate, DateTime.Now) >= 0 && DateTime.Compare(periodStartDate, DateTime.Now) <= 0)
		{
			return PeriodStatus.Current;
		}
		if (DateTime.Compare(periodEndDate, DateTime.Now) < 0)
		{
			return PeriodStatus.Expired;
		}
		return PeriodStatus.Unstarted;
	}

	public static WeeklyHuntPeriodData GetPrizeDataForPeriod(PrizePeriodId period)
	{
		InitIfNeeded();
		if (period == PrizePeriodId.None)
		{
			if (HIDE_LAST_PERIOD)
			{
				return huntPeriodData[PrizePeriodId.PeriodThree];
			}
			return huntPeriodData[PrizePeriodId.PeriodFour];
		}
		return huntPeriodData[period];
	}

	public static bool CanSpawnToken()
	{
		bool result = false;
		if (IsAnyPeriodActive())
		{
			WeeklyHuntPeriodData prizeDataForPeriod = GetPrizeDataForPeriod(GetActivePeriode());
			result = prizeDataForPeriod.itemsCollected < prizeDataForPeriod.itemsToColect;
		}
		return result;
	}

	public static bool GetHiddenUntilUnlockedForPeriod(PrizePeriodId period)
	{
		InitIfNeeded();
		if (period == PrizePeriodId.None)
		{
			return false;
		}
		return huntPeriodData[period].hiddenUntilUnlocked;
	}

	private static CelebrationReward GetRewardForPeriod(PrizePeriodId period)
	{
		CelebrationReward celebrationReward = null;
		if (huntPeriodData.ContainsKey(period))
		{
			PrizeData prizeForPeriod = GetPrizeForPeriod(period);
			if (prizeForPeriod.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeNormal)
			{
				celebrationReward = new CelebrationReward();
				celebrationReward.CelebrationRewardOrigin = prizeForPeriod.CelebrationRewardOrigin;
				celebrationReward.rewardType = prizeForPeriod.rewardType;
				celebrationReward.amount = prizeForPeriod.amount;
				if (prizeForPeriod.rewardType == CelebrationRewardType.powerup)
				{
					celebrationReward.powerupType = prizeForPeriod.powerupType;
				}
				else if (prizeForPeriod.rewardType == CelebrationRewardType.character)
				{
					celebrationReward.characterType = prizeForPeriod.characterType;
				}
				else if (prizeForPeriod.rewardType == CelebrationRewardType.specialBoard)
				{
					celebrationReward.boardType = prizeForPeriod.boardType;
					PlayerInfo.Instance.UnlockHoverboard(celebrationReward.boardType);
				}
				else if (prizeForPeriod.rewardType == CelebrationRewardType.token)
				{
					celebrationReward.characterType = prizeForPeriod.characterType;
				}
			}
			else if (prizeForPeriod.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox || prizeForPeriod.CelebrationRewardOrigin == CelebrationRewardOrigin.SuperMysteryBox)
			{
				celebrationReward = MysteryBox.Roll(MysteryBox.Type.Super);
				celebrationReward.CelebrationRewardOrigin = prizeForPeriod.CelebrationRewardOrigin;
			}
			else if (prizeForPeriod.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeMysteryBox || prizeForPeriod.CelebrationRewardOrigin == CelebrationRewardOrigin.MysteryBox)
			{
				celebrationReward = MysteryBox.Roll(MysteryBox.Type.Normal);
				celebrationReward.CelebrationRewardOrigin = prizeForPeriod.CelebrationRewardOrigin;
			}
			else if (prizeForPeriod.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeHiddenPrize)
			{
				celebrationReward = new CelebrationReward();
				celebrationReward.CelebrationRewardOrigin = prizeForPeriod.CelebrationRewardOrigin;
				celebrationReward.rewardType = prizeForPeriod.rewardType;
				celebrationReward.amount = prizeForPeriod.amount;
				if (prizeForPeriod.rewardType == CelebrationRewardType.powerup)
				{
					celebrationReward.powerupType = prizeForPeriod.powerupType;
				}
				else if (prizeForPeriod.rewardType == CelebrationRewardType.character)
				{
					celebrationReward.characterType = prizeForPeriod.characterType;
				}
				else if (prizeForPeriod.rewardType == CelebrationRewardType.specialBoard)
				{
					celebrationReward.boardType = prizeForPeriod.boardType;
					PlayerInfo.Instance.UnlockHoverboard(celebrationReward.boardType);
					PlayerInfo.Instance.currentHoverboard = celebrationReward.boardType;
					UIModelController.Instance.SelectCurrentlyShownBoard(celebrationReward.boardType);
				}
				else if (prizeForPeriod.rewardType == CelebrationRewardType.token)
				{
					celebrationReward.characterType = prizeForPeriod.characterType;
				}
			}
			else
			{
				celebrationReward = MysteryBox.Roll(MysteryBox.Type.Normal);
				celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.WeeklyChallengeNormal;
			}
		}
		return celebrationReward;
	}

	public static void ItemPickedUp()
	{
		InitIfNeeded();
		PrizePeriodId activePeriode = GetActivePeriode();
		WeeklyHuntPeriodData value = huntPeriodData[activePeriode];
		if (value.itemsCollected < value.itemsToColect)
		{
			value.itemsCollected++;
			huntPeriodData[activePeriode] = value;
			SaveNewProgress(false);
			if (value.itemsCollected == value.itemsToColect)
			{
				string text = START_CHALLANGE_DATE_TIME.Day + "-" + START_CHALLANGE_DATE_TIME.Month + "-" + START_CHALLANGE_DATE_TIME.Year;
				UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.ItemHunt));
				CelebrationReward rewardForPeriod = GetRewardForPeriod(activePeriode);
				RewardManager.AddRewardToUnlock(rewardForPeriod);
				text += string.Format(":{0}", (int)activePeriode);
				if (rewardForPeriod.rewardType == CelebrationRewardType.specialBoard)
				{
					PlayerInfo.Instance.UnlockHoverboard(rewardForPeriod.boardType);
				}
				Flurry.LogEventWithAParameter("Weekly Challenge Completed", "Id", text);
				OnWeekCompleted();
			}
		}
		else
		{
			Debug.LogWarning("WeeklyHuntManager ItemPickedUp but collected items has been completed already");
		}
		if (WeeklyHuntManager.ItemCountUpdated != null)
		{
			WeeklyHuntManager.ItemCountUpdated();
		}
	}

	private static void InitIfNeeded()
	{
		if (!_hasInited)
		{
			_hasInited = true;
			START_CHALLANGE_DATE_TIME = huntPeriodData[PrizePeriodId.PeriodOne].periodStartDate;
			LoadProgressFromResources();
			Game.OnGameStarted = (Action)Delegate.Combine(Game.OnGameStarted, new Action(OnRunStarted));
			Game.OnMenuScreenShown = (Action)Delegate.Combine(Game.OnMenuScreenShown, new Action(OnRunEnded));
		}
	}

	private static void OnRunStarted()
	{
		PrizePeriodId activePeriode = GetActivePeriode();
		if (activePeriode != PrizePeriodId.None && huntPeriodData[activePeriode].itemsCollected == 0)
		{
			_canCollectAllTokensInOneRun = true;
		}
	}

	private static void OnRunEnded()
	{
		_canCollectAllTokensInOneRun = false;
	}

	private static void OnWeekCompleted()
	{
		if (_canCollectAllTokensInOneRun)
		{
			Statistics stats;
			Statistics statistics = (stats = PlayerInfo.Instance.stats);
			Stat stat2;
			Stat stat = (stat2 = Stat.HuntsCompletedInOneRun);
			int num = stats[stat2];
			statistics[stat] = num + 1;
		}
	}

	private static void LoadProgressFromResources()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		string weeklyChallangeData = instance.weeklyChallangeData;
		string[] array = weeklyChallangeData.Split(',');
		string storedStringDate = array[0];
		if (!ShouldResetChallangeData(storedStringDate))
		{
			for (int i = 1; i < array.Length; i++)
			{
				int result = 0;
				int.TryParse(array[i], out result);
				WeeklyHuntPeriodData value = huntPeriodData[(PrizePeriodId)i];
				value.itemsCollected = result;
				huntPeriodData[(PrizePeriodId)i] = value;
			}
		}
		else
		{
			SaveNewProgress(true);
		}
	}

	private static bool ShouldResetChallangeData(string storedStringDate)
	{
		DateTime result = DateTime.MinValue;
		if (DateTime.TryParse(storedStringDate, out result))
		{
			if (DateTime.Compare(result, START_CHALLANGE_DATE_TIME) != 0)
			{
				return true;
			}
			return false;
		}
		return true;
	}

	public static void SaveNewProgress(bool shouldStartNewProgress)
	{
		PlayerInfo instance = PlayerInfo.Instance;
		StringBuilder stringBuilder = new StringBuilder(string.Empty);
		stringBuilder.Append(START_CHALLANGE_DATE_TIME.ToString() + ",");
		foreach (KeyValuePair<PrizePeriodId, WeeklyHuntPeriodData> huntPeriodDatum in huntPeriodData)
		{
			if (shouldStartNewProgress)
			{
				WeeklyHuntPeriodData value = huntPeriodDatum.Value;
				value.itemsCollected = 0;
				stringBuilder.Append("0,");
			}
			else
			{
				stringBuilder.Append(huntPeriodDatum.Value.itemsCollected + ",");
			}
		}
		int num = stringBuilder.ToString().LastIndexOf(',');
		if (num >= 0)
		{
			stringBuilder.Remove(num, 1);
		}
		instance.weeklyChallangeData = stringBuilder.ToString();
	}

	public static int GetPrizeDataCount()
	{
		return huntPeriodData.Count;
	}

	public static bool IsBoardAwardedInCurrentPeriod(Hoverboards.BoardType boardType)
	{
		PrizePeriodId activePeriode = GetActivePeriode();
		if (activePeriode != PrizePeriodId.None)
		{
			PrizeData prizeForPeriod = GetPrizeForPeriod(activePeriode);
			return prizeForPeriod.rewardType == CelebrationRewardType.specialBoard && prizeForPeriod.boardType == boardType;
		}
		return false;
	}

	public static int ItemsCollectedForBoardType(Hoverboards.BoardType boardType)
	{
		foreach (PrizePeriodId key in huntPeriodData.Keys)
		{
			PrizeData prizeForPeriod = GetPrizeForPeriod(key);
			if (prizeForPeriod.rewardType == CelebrationRewardType.specialBoard && prizeForPeriod.boardType == boardType)
			{
				return huntPeriodData[key].itemsCollected;
			}
		}
		return 0;
	}

	public static int ItemsToCollectForBoardType(Hoverboards.BoardType boardType)
	{
		foreach (PrizePeriodId key in huntPeriodData.Keys)
		{
			PrizeData prizeForPeriod = GetPrizeForPeriod(key);
			if (prizeForPeriod.rewardType == CelebrationRewardType.specialBoard && prizeForPeriod.boardType == boardType)
			{
				return huntPeriodData[key].itemsToColect;
			}
		}
		return 0;
	}

	public static PrizeData GetPrizeForPeriod(PrizePeriodId periodId)
	{
		return (!IsPeriodShowingAlternatePrize(periodId)) ? huntPeriodData[periodId].mainPrize : huntPeriodData[periodId].alternatePrize;
	}

	public static bool IsPeriodShowingAlternatePrize(PrizePeriodId periodId)
	{
		WeeklyHuntPeriodData weeklyHuntPeriodData = huntPeriodData[periodId];
		return weeklyHuntPeriodData.hasAlternatePrize && weeklyHuntPeriodData.alternatePrizeCondition();
	}

	public static WeeklyHuntPrize DeterminePrize(PrizeData prizeData)
	{
		switch (prizeData.CelebrationRewardOrigin)
		{
		case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
			return WeeklyHuntPrize.SuperMysteryBox;
		default:
			switch (prizeData.rewardType)
			{
			case CelebrationRewardType.keys:
				return WeeklyHuntPrize.Key;
			case CelebrationRewardType.specialBoard:
			{
				Hoverboards.BoardType boardType = prizeData.boardType;
				if (boardType == Hoverboards.BoardType.londonBoard01)
				{
					return WeeklyHuntPrize.ChristmasBoard;
				}
				Debug.LogError("Unknown BoardType");
				break;
			}
			}
			switch (prizeData.powerupType)
			{
			case PowerupType.hoverboard:
				return WeeklyHuntPrize.Hoverboard;
			case PowerupType.headstart2000:
				return WeeklyHuntPrize.Headstart;
			case PowerupType.scorebooster:
				return WeeklyHuntPrize.ScoreBooster;
			default:
				return WeeklyHuntPrize.NotAvailable;
			}
		}
	}

	public static string GetPrizeTitle(PrizeData prizeData)
	{
		switch (DeterminePrize(prizeData))
		{
		case WeeklyHuntPrize.Key:
			return Strings.Get(StringID.WEEKLYHUNT_PRIZE_KEYS_TITLE);
		case WeeklyHuntPrize.SuperMysteryBox:
			return Strings.Get(StringID.WEEKLYHUNT_PRIZE_SUPERMYSTERYBOX_TITLE);
		case WeeklyHuntPrize.Hoverboard:
			return Strings.Get(StringID.WEEKLYHUNT_PRIZE_HOVERBOARD_TITLE);
		case WeeklyHuntPrize.ScoreBooster:
			return Strings.Get(StringID.WEEKLYHUNT_PRIZE_SCOREBOOSTER_TITLE);
		case WeeklyHuntPrize.Headstart:
			return Strings.Get(StringID.WEEKLYHUNT_PRIZE_HEADSTART_TITLE);
		case WeeklyHuntPrize.ChristmasBoard:
			return Strings.Get(StringID.HOVERBOARDS_LONDONBOARD01);
		default:
			return string.Empty;
		}
	}

	public static string GetPrizeTitleShort(PrizeData prizeData)
	{
		switch (DeterminePrize(prizeData))
		{
		case WeeklyHuntPrize.Key:
			return Strings.Get(StringID.WEEKLYHUNT_PRIZE_KEYS_TITLE_SHORT);
		case WeeklyHuntPrize.SuperMysteryBox:
			return Strings.Get(StringID.WEEKLYHUNT_PRIZE_SUPERMYSTERYBOX_TITLE_SHORT);
		case WeeklyHuntPrize.Hoverboard:
			return Strings.Get(StringID.WEEKLYHUNT_PRIZE_HOVERBOARD_TITLE_SHORT);
		case WeeklyHuntPrize.ScoreBooster:
			return Strings.Get(StringID.WEEKLYHUNT_PRIZE_SCOREBOOSTER_TITLE_SHORT);
		case WeeklyHuntPrize.Headstart:
			return Strings.Get(StringID.WEEKLYHUNT_PRIZE_HEADSTART_TITLE_SHORT);
		case WeeklyHuntPrize.ChristmasBoard:
			return Strings.Get(StringID.HOVERBOARDS_LONDONBOARD01);
		default:
			return string.Empty;
		}
	}
}
