using System;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace com.kiloo.awards
{
	public class SupersneakersTimeInRowAward : Award
	{
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

		public SupersneakersTimeInRowAward(AwardStaticData awardData, string savedProgress, Action<Award> handler)
			: base(awardData, savedProgress, handler)
		{
			offset = 0;
		}

		protected override int GetProgressForGoal(AwardGoal goal)
		{
			SuperSneakers instance = SuperSneakers.Instance;
			if (instance != null)
			{
				float timeActiveInARow = instance.timeActiveInARow;
				return (int)timeActiveInARow / 60;
			}
			return 0;
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
			SuperSneakers.OnSupersneakersTimeActiveChange = (Action)Delegate.Combine(SuperSneakers.OnSupersneakersTimeActiveChange, new Action(OnTimeChanged));
			CheckAndUpdateProgress();
		}

		protected override void DeactivateCurrentAwardTier()
		{
			SuperSneakers.OnSupersneakersTimeActiveChange = (Action)Delegate.Remove(SuperSneakers.OnSupersneakersTimeActiveChange, new Action(OnTimeChanged));
		}

		protected override void RecordNewOffsets()
		{
		}

		private void OnTimeChanged()
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
			ResetAwardToDefaultState();
			if (s == null || s.Length <= 0)
			{
				return;
			}
			try
			{
				string[] array = s.Split(',');
				if (array.Length >= 5)
				{
					_savedActiveTierType = (AwardTierType)(int)Enum.Parse(typeof(AwardTierType), array[0], true);
					_currentActiveAwardState = (AwardState)(int)Enum.Parse(typeof(AwardState), array[1], true);
					_savedProgressTierType = (AwardTierType)(int)Enum.Parse(typeof(AwardTierType), array[2], true);
					_currentProgressAwardState = (AwardState)(int)Enum.Parse(typeof(AwardState), array[3], true);
					_lastActiveStateChangeDateTime = Utils.StringToDateTime(array[4], DateTime.MinValue);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("SupersneakersTimeInRowAward parse failed: " + ex.ToString());
				ResetAwardToDefaultState();
			}
		}
	}
}
