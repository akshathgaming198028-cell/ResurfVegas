using System;
using System.Globalization;
using System.Text;
using SYBO.Subway;
using SYBO.Subway.Characters;
using UnityEngine;

namespace com.kiloo.awards
{
	public class ScorePointsSingleLane : Award
	{
		private Character character;

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

		public ScorePointsSingleLane(AwardStaticData awardData, string savedProgress, Action<Award> handler)
			: base(awardData, savedProgress, handler)
		{
			offset = 0;
		}

		protected override int GetProgressForGoal(AwardGoal goal)
		{
			return GameStats.Instance.score - offset;
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
			GameStats instance = GameStats.Instance;
			instance.OnScoreChanged = (Action)Delegate.Combine(instance.OnScoreChanged, new Action(OnScoreChanged));
			Game.OnGameStarted = (Action)Delegate.Combine(Game.OnGameStarted, new Action(OnRunStarted));
			Game.OnMenuScreenShown = (Action)Delegate.Combine(Game.OnMenuScreenShown, new Action(OnRunEnded));
			CheckAndUpdateProgress();
		}

		protected override void DeactivateCurrentAwardTier()
		{
			GameStats instance = GameStats.Instance;
			instance.OnScoreChanged = (Action)Delegate.Remove(instance.OnScoreChanged, new Action(OnScoreChanged));
			Game.OnGameStarted = (Action)Delegate.Remove(Game.OnGameStarted, new Action(OnRunStarted));
			Game.OnMenuScreenShown = (Action)Delegate.Remove(Game.OnMenuScreenShown, new Action(OnRunEnded));
		}

		protected override void RecordNewOffsets()
		{
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
			if (!string.IsNullOrEmpty(s))
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
						Debug.LogError("ScorePointsOneRunNoJumpOrRollAward parse failed: " + ex.ToString());
						ResetAwardToDefaultState();
						return;
					}
				}
				Debug.LogError("ScorePointsOneRunNoJumpOrRollAward parse failed: Split size not right");
				ResetAwardToDefaultState();
			}
			else
			{
				ResetAwardToDefaultState();
			}
		}

		private void OnRunStarted()
		{
			character = Character.Instance;
			if (character != null)
			{
				character.OnChangeTrack += OnLaneChanged;
			}
			offset = 0;
		}

		private void OnRunEnded()
		{
			if (character != null)
			{
				character.OnChangeTrack -= OnLaneChanged;
			}
			offset = GameStats.Instance.score;
		}

		private void OnScoreChanged()
		{
			CheckAndUpdateProgress();
		}

		private void OnLaneChanged(Character.OnChangeTrackDirection direction)
		{
			offset = GameStats.Instance.score;
		}
	}
}
