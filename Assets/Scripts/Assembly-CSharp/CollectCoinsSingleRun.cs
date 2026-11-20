using System;
using System.Globalization;
using System.Text;
using SYBO.Subway;
using UnityEngine;
using com.kiloo.awards;

public class CollectCoinsSingleRun : Award
{
	private int _progress;

	private int _offset;

	public CollectCoinsSingleRun(AwardStaticData awardData, string savedProgress, Action<Award> handler)
		: base(awardData, savedProgress, handler)
	{
		ResetProgress();
	}

	protected override int GetProgressForGoal(AwardGoal goal)
	{
		return _progress - _offset;
	}

	public override bool isGoalAccomplished(AwardGoal goal)
	{
		AwardSimpleGoal awardSimpleGoal = goal as AwardSimpleGoal;
		if (awardSimpleGoal != null && GetProgressForGoal(goal) == awardSimpleGoal.goal)
		{
			return true;
		}
		return false;
	}

	protected override void ActivateCurrentAwardTier()
	{
		GameStats instance = GameStats.Instance;
		instance.OnCoinsIncreased = (Action)Delegate.Combine(instance.OnCoinsIncreased, new Action(OnCoinsIncreased));
		Game.OnGameStarted = (Action)Delegate.Combine(Game.OnGameStarted, new Action(OnRunStarted));
		GameOverScreen.OnGameOverScreenShown = (Action)Delegate.Combine(GameOverScreen.OnGameOverScreenShown, new Action(OnRunEnded));
		UIScreenController.OnNavigateToScreen += OnFrontScreen;
		CheckAndUpdateProgress();
	}

	protected override void DeactivateCurrentAwardTier()
	{
		GameStats instance = GameStats.Instance;
		instance.OnCoinsIncreased = (Action)Delegate.Remove(instance.OnCoinsIncreased, new Action(OnCoinsIncreased));
		Game.OnGameStarted = (Action)Delegate.Remove(Game.OnGameStarted, new Action(OnRunStarted));
		GameOverScreen.OnGameOverScreenShown = (Action)Delegate.Remove(GameOverScreen.OnGameOverScreenShown, new Action(OnRunEnded));
		UIScreenController.OnNavigateToScreen -= OnFrontScreen;
	}

	private void OnFrontScreen(string screenName)
	{
		if (screenName == "FrontUI")
		{
			ResetProgress();
		}
	}

	protected override void RecordNewOffsets()
	{
	}

	private void OnRunStarted()
	{
		ResetProgress();
	}

	private void OnRunEnded()
	{
		CheckAndUpdateProgress();
	}

	private void OnCoinsIncreased()
	{
		if (PlayerInfo.Instance.hasDoubleCoins)
		{
			if (DoubleCoins.mysteryDoubleCoinsEnabled)
			{
				_progress += 4;
			}
			else
			{
				_progress += 2;
			}
		}
		else if (DoubleCoins.mysteryDoubleCoinsEnabled)
		{
			_progress += 2;
		}
		else
		{
			_progress++;
		}
	}

	private void ResetProgress()
	{
		_offset = 0;
		_progress = 0;
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
