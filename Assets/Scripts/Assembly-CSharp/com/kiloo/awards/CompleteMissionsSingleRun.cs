using System;
using System.Globalization;
using System.Text;
using SYBO.Subway;
using UnityEngine;

namespace com.kiloo.awards
{
	public class CompleteMissionsSingleRun : Award
	{
		private int _progress;

		private int _offset;

		public CompleteMissionsSingleRun(AwardStaticData awardData, string savedProgress, Action<Award> handler)
			: base(awardData, savedProgress, handler)
		{
			_progress = 0;
			_offset = 0;
		}

		protected override int GetProgressForGoal(AwardGoal goal)
		{
			return _progress - _offset;
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
			MissionsManager instance = MissionsManager.Instance;
			instance.onMissionComplete = (MissionsManager.MissionCompleteHandler)Delegate.Combine(instance.onMissionComplete, new MissionsManager.MissionCompleteHandler(OnMissionCompleted));
			Game.OnGameStarted = (Action)Delegate.Combine(Game.OnGameStarted, new Action(OnRunStarted));
			Game.OnMenuScreenShown = (Action)Delegate.Combine(Game.OnMenuScreenShown, new Action(OnRunEnded));
			CheckAndUpdateProgress();
		}

		protected override void DeactivateCurrentAwardTier()
		{
			MissionsManager instance = MissionsManager.Instance;
			instance.onMissionComplete = (MissionsManager.MissionCompleteHandler)Delegate.Remove(instance.onMissionComplete, new MissionsManager.MissionCompleteHandler(OnMissionCompleted));
			Game.OnGameStarted = (Action)Delegate.Remove(Game.OnGameStarted, new Action(OnRunStarted));
			Game.OnMenuScreenShown = (Action)Delegate.Remove(Game.OnMenuScreenShown, new Action(OnRunEnded));
		}

		protected override void RecordNewOffsets()
		{
		}

		private void OnRunStarted()
		{
			_offset = 0;
			_progress = 0;
		}

		private void OnRunEnded()
		{
			_offset = _progress;
		}

		private void OnMissionCompleted(string description)
		{
			if (MissionsManager.Instance.inRun)
			{
				_progress++;
				CheckAndUpdateProgress();
			}
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
						Debug.LogError("ScorePointsOneRunNoCoinsAward parse failed: " + ex.ToString());
						ResetAwardToDefaultState();
						return;
					}
				}
				Debug.LogError("ScorePointsOneRunNoCoinsAward parse failed: Split size not right");
				ResetAwardToDefaultState();
			}
			else
			{
				ResetAwardToDefaultState();
			}
		}
	}
}
