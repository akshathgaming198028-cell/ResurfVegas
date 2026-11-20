using System;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace com.kiloo.awards
{
	public class DailyChallengesInRowAward : Award
	{
		private bool isDailyChallengeCompletedToday;

		private int _offset = -1;

		public int offset
		{
			get
			{
				return _offset;
			}
			set
			{
				_offset = value;
			}
		}

		public DailyChallengesInRowAward(AwardStaticData trophyData, string savedProgress, Action<Award> handler)
			: base(trophyData, savedProgress, handler)
		{
			SyncOffset();
		}

		private int GetDailyChallengesInRow()
		{
			return PlayerInfo.Instance.GetDailyWordDaysInARow(out isDailyChallengeCompletedToday);
		}

		protected override int GetProgressForGoal(AwardGoal goal)
		{
			return GetDailyChallengesInRow() - offset;
		}

		public bool IsDailyChallengeCompletedToday()
		{
			return isDailyChallengeCompletedToday;
		}

		public override bool isGoalAccomplished(AwardGoal goal)
		{
			AwardSimpleGoal awardSimpleGoal = goal as AwardSimpleGoal;
			if (awardSimpleGoal != null && GetProgressForGoal(goal) >= awardSimpleGoal.goal)
			{
				return true;
			}
			return false;
		}

		protected override void ActivateCurrentAwardTier()
		{
			PlayerInfo instance = PlayerInfo.Instance;
			instance.OnDailyWordInRowChanged = (Action)Delegate.Combine(instance.OnDailyWordInRowChanged, new Action(OnDailyWordInRowChanged));
			CheckAndUpdateProgress();
		}

		protected override void DeactivateCurrentAwardTier()
		{
			PlayerInfo instance = PlayerInfo.Instance;
			instance.OnDailyWordInRowChanged = (Action)Delegate.Remove(instance.OnDailyWordInRowChanged, new Action(OnDailyWordInRowChanged));
		}

		protected override void RecordNewOffsets()
		{
		}

		private void SyncOffset()
		{
			int dailyChallengesInRow = GetDailyChallengesInRow();
			if (offset == -1 || offset > dailyChallengesInRow)
			{
				offset = dailyChallengesInRow;
			}
			if (offset < 0)
			{
				offset = 0;
			}
		}

		private void OnDailyWordInRowChanged()
		{
			CheckAndUpdateProgress();
		}

		public override string AwardToString()
		{
			if (_awardActiveTier != null && _awardProgressTier != null)
			{
				StringBuilder stringBuilder = new StringBuilder(string.Empty);
				stringBuilder.Append((int)_awardActiveTier.tierType + ",");
				stringBuilder.Append((int)_currentActiveAwardState + ",");
				stringBuilder.Append((int)_awardProgressTier.tierType + ",");
				stringBuilder.Append((int)_currentProgressAwardState + ",");
				stringBuilder.Append(_lastActiveStateChangeDateTime.ToString(CultureInfo.InvariantCulture) + ",");
				stringBuilder.Append(offset);
				return stringBuilder.ToString();
			}
			return null;
		}

		public override void ParseSavedProgressAndCreateAwardState(string s)
		{
			if (s != null && s.Length > 0)
			{
				string[] array = s.Split(',');
				if (array.Length == 6)
				{
					try
					{
						_savedActiveTierType = (AwardTierType)(int)Enum.Parse(typeof(AwardTierType), array[0], true);
						_currentActiveAwardState = (AwardState)(int)Enum.Parse(typeof(AwardState), array[1], true);
						_savedProgressTierType = (AwardTierType)(int)Enum.Parse(typeof(AwardTierType), array[2], true);
						_currentProgressAwardState = (AwardState)(int)Enum.Parse(typeof(AwardState), array[3], true);
						_lastActiveStateChangeDateTime = Utils.StringToDateTime(array[4], DateTime.MinValue);
						offset = int.Parse(array[5]);
						return;
					}
					catch (Exception ex)
					{
						Debug.LogError("DailyChallengeInRowAward parse failed: " + ex.ToString());
						ResetAwardToDefaultState();
						return;
					}
				}
				Debug.LogError("DailyChallengeInRowAward parse failed :Split size not right");
				ResetAwardToDefaultState();
			}
			else
			{
				ResetAwardToDefaultState();
			}
		}
	}
}
