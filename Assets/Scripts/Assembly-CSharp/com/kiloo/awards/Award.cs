using System;
using Kiloo.Plugins.DeviceUtility;
using UnityEngine;

namespace com.kiloo.awards
{
	public abstract class Award
	{
		private const string AWARD_ISNEW_KEY_PREFIX = "isAwardNew";

		private const string AWARD_HASPAYED_OUT_KEY_PREFIX = "hasPayed";

		protected AwardStaticData _awardStaticData;

		protected AwardState _currentActiveAwardState;

		protected AwardTier _awardActiveTier;

		protected AwardTierType _savedActiveTierType;

		protected DateTime _lastActiveStateChangeDateTime = DateTime.MinValue;

		protected DateTime _previewDateTime = DateTime.MinValue;

		protected AwardState _currentProgressAwardState;

		protected AwardTier _awardProgressTier;

		protected AwardTierType _savedProgressTierType;

		public AwardTierType tierAnimationPlayed;

		public Action<Award> onStateChangedHandler;

		public AwardStaticData trophyData
		{
			get
			{
				return _awardStaticData;
			}
		}

		public AwardGoal[] goals
		{
			get
			{
				if (_awardActiveTier != null)
				{
					return _awardActiveTier.listGoals;
				}
				return null;
			}
		}

		public DateTime lastStateChangeDateTime
		{
			get
			{
				if (isInPreview)
				{
					return _previewDateTime;
				}
				return _lastActiveStateChangeDateTime;
			}
		}

		public AwardState awardState
		{
			get
			{
				return _currentActiveAwardState;
			}
			set
			{
				_currentActiveAwardState = value;
			}
		}

		public bool isNew
		{
			get
			{
				return (PlayerPrefs.GetInt("isAwardNew" + _awardStaticData.id, 1) != 0) ? true : false;
			}
			set
			{
				PlayerPrefs.SetInt("isAwardNew" + _awardStaticData.id, value ? 1 : 0);
			}
		}

		public AwardTierType activeTierType
		{
			get
			{
				return _awardActiveTier.tierType;
			}
		}

		public bool isInPreview { get; set; }

		public int reward
		{
			get
			{
				if (_awardActiveTier != null)
				{
					return _awardActiveTier.reward;
				}
				return 0;
			}
		}

		public bool complete
		{
			get
			{
				return _currentActiveAwardState == AwardState.Finished;
			}
		}

		public Award(AwardStaticData trophyData, string savedProgress, Action<Award> handler)
		{
			onStateChangedHandler = (Action<Award>)Delegate.Combine(onStateChangedHandler, handler);
			isInPreview = false;
			_awardStaticData = trophyData;
			ParseSavedProgressAndCreateAwardState(savedProgress);
		}

		public void Activate()
		{
			if (!complete)
			{
				if (_savedProgressTierType != AwardTierType.None && trophyData.missionTiers.ContainsKey(_savedProgressTierType))
				{
					_awardProgressTier = _awardStaticData.missionTiers[_savedProgressTierType];
					if (_savedActiveTierType != AwardTierType.None && trophyData.missionTiers.ContainsKey(_savedActiveTierType))
					{
						_awardActiveTier = _awardStaticData.missionTiers[_savedActiveTierType];
					}
					CheckTierValues();
				}
				else
				{
					for (int i = 0; i < Enum.GetNames(typeof(AwardTierType)).Length; i++)
					{
						if (trophyData.missionTiers.ContainsKey((AwardTierType)i))
						{
							_awardProgressTier = _awardStaticData.missionTiers[(AwardTierType)i];
							_awardActiveTier = _awardStaticData.missionTiers[(AwardTierType)i];
							_currentActiveAwardState = AwardState.InProgress;
							_currentProgressAwardState = AwardState.InProgress;
							RecordNewOffsets();
							break;
						}
					}
				}
				if (_awardProgressTier != null && _currentProgressAwardState == AwardState.InProgress)
				{
					ActivateCurrentAwardTier();
				}
				return;
			}
			for (int num = Enum.GetNames(typeof(AwardTierType)).Length - 1; num > 0; num--)
			{
				if (trophyData.missionTiers.ContainsKey((AwardTierType)num))
				{
					_awardActiveTier = _awardStaticData.missionTiers[(AwardTierType)num];
					_awardProgressTier = _awardStaticData.missionTiers[(AwardTierType)num];
					break;
				}
			}
		}

		private void CheckTierValues()
		{
			if (_awardActiveTier.tierType > _awardProgressTier.tierType)
			{
				Debug.Log("Out of sync. Syncing...");
				_awardActiveTier = _awardStaticData.missionTiers[_awardProgressTier.tierType];
				_currentActiveAwardState = _currentProgressAwardState;
			}
		}

		public bool BelongsToState(AwardState state)
		{
			if (isInPreview)
			{
				if (state == AwardState.Collectible)
				{
					return true;
				}
			}
			else if (awardState == state)
			{
				return true;
			}
			return false;
		}

		public bool hasFinishedAnyTier()
		{
			if (_currentActiveAwardState == AwardState.Collectible || _currentActiveAwardState == AwardState.Finished)
			{
				return true;
			}
			if (_currentActiveAwardState == AwardState.InProgress && PlayerPrefs.HasKey("hasPayed" + _awardStaticData.id))
			{
				return true;
			}
			return false;
		}

		public int GetProgressAsPercentage()
		{
			if (_currentActiveAwardState == AwardState.Collectible || _currentActiveAwardState == AwardState.Finished)
			{
				return 100;
			}
			float num = 0f;
			int num2 = 0;
			int num3 = 0;
			AwardGoal[] listGoals = _awardActiveTier.listGoals;
			foreach (AwardGoal awardGoal in listGoals)
			{
				num2 += GetProgressForGoal(awardGoal);
				num3 += awardGoal.goal;
			}
			num = (float)num2 * 100f / (float)num3;
			if (num < 0f)
			{
				num = 0f;
			}
			else if (num > 100f)
			{
				num = 100f;
			}
			return (int)num;
		}

		protected void CheckAndUpdateProgress()
		{
			if (_currentProgressAwardState == AwardState.Finished)
			{
				return;
			}
			bool flag = true;
			AwardGoal[] listGoals = _awardProgressTier.listGoals;
			foreach (AwardGoal goal in listGoals)
			{
				if (!isGoalAccomplished(goal))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				GoToState(AwardState.Collectible);
			}
		}

		protected void GoToState(AwardState state)
		{
			_currentProgressAwardState = state;
			Action<Award> action = onStateChangedHandler;
			if (action != null)
			{
				action(this);
			}
			if (_currentProgressAwardState == AwardState.InProgress)
			{
				ActivateCurrentAwardTier();
			}
			else if (_currentProgressAwardState == AwardState.Collectible)
			{
				if (UIScreenController.IsInstanced)
				{
					UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Awards, Strings.Get(trophyData.title), _awardProgressTier.tierType));
				}
				Flurry.LogEventWithSeveralParameters("Award tier completed time", "Award;Tier;Time;Device", string.Concat(trophyData.id.ToString(), ";", (int)activeTierType, ";", DateTime.Now, ";", DeviceUtility.GetDeviceIdentifier()));
				Flurry.LogEventWithSeveralParameters("Award tier completed", "Award;Tier", trophyData.id.ToString() + ";" + (int)activeTierType);
				if (_awardActiveTier.tierType == _awardProgressTier.tierType && _currentActiveAwardState == AwardState.InProgress)
				{
					_currentActiveAwardState = AwardState.Collectible;
					_previewDateTime = DateTime.UtcNow;
					_lastActiveStateChangeDateTime = DateTime.UtcNow;
				}
				DeactivateCurrentAwardTier();
				MoveToNextTierOrFinishIfLast();
			}
			else if (_currentProgressAwardState == AwardState.Finished)
			{
				Flurry.LogEventWithAParameter("Award completed", "Award", trophyData.id.ToString());
				if (_currentActiveAwardState == AwardState.InProgress)
				{
					_currentActiveAwardState = AwardState.Collectible;
					_lastActiveStateChangeDateTime = DateTime.UtcNow;
				}
			}
		}

		public void RewardPayedOut()
		{
			if (_currentActiveAwardState == AwardState.Collectible)
			{
				if (!PlayerPrefs.HasKey("hasPayed" + _awardStaticData.id))
				{
					PlayerPrefs.SetInt("hasPayed" + _awardStaticData.id, 1);
				}
				CalculateNextActiveTier();
			}
		}

		protected void CalculateNextActiveTier()
		{
			if (_awardActiveTier.tierType < _awardProgressTier.tierType)
			{
				int key = (int)(_awardActiveTier.tierType + 1);
				if (trophyData.missionTiers.ContainsKey((AwardTierType)key))
				{
					_awardActiveTier = _awardStaticData.missionTiers[(AwardTierType)key];
					if (_awardActiveTier.tierType == _awardProgressTier.tierType)
					{
						if (_currentProgressAwardState == AwardState.Collectible || _currentProgressAwardState == AwardState.Finished)
						{
							if (_currentActiveAwardState != AwardState.Collectible)
							{
								_previewDateTime = DateTime.UtcNow;
							}
							_currentActiveAwardState = AwardState.Collectible;
						}
						else
						{
							_currentActiveAwardState = AwardState.InProgress;
						}
					}
				}
			}
			else if (_currentProgressAwardState == AwardState.Finished)
			{
				_lastActiveStateChangeDateTime = DateTime.UtcNow;
				_currentActiveAwardState = AwardState.Finished;
			}
			Action<Award> action = onStateChangedHandler;
			if (action != null)
			{
				action(this);
			}
		}

		protected void MoveToNextTierOrFinishIfLast()
		{
			bool flag = false;
			for (int i = (int)(_awardProgressTier.tierType + 1); i < Enum.GetNames(typeof(AwardTierType)).Length; i++)
			{
				if (trophyData.missionTiers.ContainsKey((AwardTierType)i))
				{
					_awardProgressTier = _awardStaticData.missionTiers[(AwardTierType)i];
					flag = true;
					break;
				}
			}
			if (flag)
			{
				RecordNewOffsets();
				GoToState(AwardState.InProgress);
			}
			else
			{
				GoToState(AwardState.Finished);
			}
		}

		public void CompleteCurrentTierIfDebug()
		{
			if (Globals.DEBUG_POPUP)
			{
				MoveToNextTierOrFinishIfLast();
			}
		}

		protected void ResetAwardToDefaultState()
		{
			_savedProgressTierType = AwardTierType.None;
			_savedProgressTierType = AwardTierType.None;
			_currentActiveAwardState = AwardState.NotSet;
			_currentProgressAwardState = AwardState.NotSet;
			_lastActiveStateChangeDateTime = DateTime.MinValue;
		}

		public void Complete()
		{
			DeactivateCurrentAwardTier();
		}

		public bool HasCompletedTier(AwardTierType progressTierTypeToCheck)
		{
			if (_awardProgressTier.tierType == progressTierTypeToCheck)
			{
				if (_currentProgressAwardState == AwardState.Collectible || _currentProgressAwardState == AwardState.Finished)
				{
					return true;
				}
			}
			else if (_awardProgressTier.tierType == progressTierTypeToCheck + 1 && _currentProgressAwardState == AwardState.InProgress)
			{
				return true;
			}
			return false;
		}

		public abstract bool isGoalAccomplished(AwardGoal goal);

		protected abstract int GetProgressForGoal(AwardGoal goal);

		protected abstract void ActivateCurrentAwardTier();

		protected abstract void DeactivateCurrentAwardTier();

		protected abstract void RecordNewOffsets();

		public abstract string AwardToString();

		public abstract void ParseSavedProgressAndCreateAwardState(string s);

		public void OnGameStarted()
		{
		}

		public void OnGameEnded()
		{
		}
	}
}
