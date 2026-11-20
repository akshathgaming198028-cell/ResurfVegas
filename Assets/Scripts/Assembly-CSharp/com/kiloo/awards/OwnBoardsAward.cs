using System;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace com.kiloo.awards
{
	public class OwnBoardsAward : Award
	{
		public OwnBoardsAward(AwardStaticData trophyData, string savedProgress, Action<Award> handler)
			: base(trophyData, savedProgress, handler)
		{
		}

		private int GetNumberOfOwnedBoards()
		{
			int num = 0;
			foreach (Hoverboards.BoardType item in Hoverboards.boardOrder)
			{
				if (PlayerInfo.Instance.isHoverboardUnlocked(item))
				{
					num++;
				}
			}
			return num;
		}

		protected override int GetProgressForGoal(AwardGoal goal)
		{
			return GetNumberOfOwnedBoards();
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
			instance.OnBoardUnlocked = (Action<Hoverboards.BoardType>)Delegate.Combine(instance.OnBoardUnlocked, new Action<Hoverboards.BoardType>(OnBoardUnlocked));
			CheckAndUpdateProgress();
		}

		protected override void DeactivateCurrentAwardTier()
		{
			PlayerInfo instance = PlayerInfo.Instance;
			instance.OnBoardUnlocked = (Action<Hoverboards.BoardType>)Delegate.Remove(instance.OnBoardUnlocked, new Action<Hoverboards.BoardType>(OnBoardUnlocked));
		}

		protected override void RecordNewOffsets()
		{
		}

		public void OnBoardUnlocked(Hoverboards.BoardType boardType)
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
				stringBuilder.Append(_lastActiveStateChangeDateTime.ToString(CultureInfo.InvariantCulture));
				return stringBuilder.ToString();
			}
			return null;
		}

		public override void ParseSavedProgressAndCreateAwardState(string s)
		{
			if (s != null && s.Length > 0)
			{
				string[] array = s.Split(',');
				if (array.Length >= 5)
				{
					try
					{
						_savedActiveTierType = (AwardTierType)(int)Enum.Parse(typeof(AwardTierType), array[0], true);
						_currentActiveAwardState = (AwardState)(int)Enum.Parse(typeof(AwardState), array[1], true);
						_savedProgressTierType = (AwardTierType)(int)Enum.Parse(typeof(AwardTierType), array[2], true);
						_currentProgressAwardState = (AwardState)(int)Enum.Parse(typeof(AwardState), array[3], true);
						_lastActiveStateChangeDateTime = Utils.StringToDateTime(array[4], DateTime.MinValue);
						return;
					}
					catch (Exception ex)
					{
						Debug.LogError("OwnBoardsAward parse failed: " + ex.ToString());
						ResetAwardToDefaultState();
						return;
					}
				}
				Debug.LogError("OwnBoardsAward parse failed :Split size not right: " + s);
				ResetAwardToDefaultState();
			}
			else
			{
				ResetAwardToDefaultState();
			}
		}
	}
}
