using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace com.kiloo.awards
{
	public class StatAward : Award
	{
		private List<Stat> listActiveStats = new List<Stat>();

		private Dictionary<Stat, int> _statOffset = new Dictionary<Stat, int>();

		private bool _hasMigratedToUseStatOffset;

		private bool _shouldMigrateToUseStatOffset;

		private Statistics stats
		{
			get
			{
				return PlayerInfo.Instance.stats;
			}
		}

		public StatAward(AwardStaticData trophyData, string savedProgress, Action<Award> handler)
			: base(trophyData, savedProgress, handler)
		{
			if (!_hasMigratedToUseStatOffset && trophyData.migrateToUseStatOffset && trophyData.useStatOffset)
			{
				_shouldMigrateToUseStatOffset = true;
			}
		}

		protected override int GetProgressForGoal(AwardGoal goal)
		{
			AwardStatGoal awardStatGoal = goal as AwardStatGoal;
			if (awardStatGoal != null)
			{
				return stats[awardStatGoal.stat] - OffsetForStat(awardStatGoal.stat);
			}
			return 0;
		}

		public override bool isGoalAccomplished(AwardGoal goal)
		{
			AwardStatGoal awardStatGoal = goal as AwardStatGoal;
			if (awardStatGoal != null && stats[awardStatGoal.stat] >= awardStatGoal.goal + OffsetForStat(awardStatGoal.stat))
			{
				return true;
			}
			return false;
		}

		private int OffsetForStat(Stat stat)
		{
			if (_statOffset.ContainsKey(stat) && _awardStaticData.useStatOffset)
			{
				if (_shouldMigrateToUseStatOffset)
				{
					return 0;
				}
				return _statOffset[stat];
			}
			return 0;
		}

		protected override void ActivateCurrentAwardTier()
		{
			AwardGoal[] listGoals = _awardProgressTier.listGoals;
			foreach (AwardGoal awardGoal in listGoals)
			{
				AwardStatGoal awardStatGoal = awardGoal as AwardStatGoal;
				if (awardStatGoal != null)
				{
					Stat stat = awardStatGoal.stat;
					listActiveStats.Add(stat);
					stats.AddStatChangedHandler(stat, OnStatChanged);
				}
			}
			CheckAndUpdateProgress();
		}

		protected override void DeactivateCurrentAwardTier()
		{
			foreach (Stat listActiveStat in listActiveStats)
			{
				stats.RemoveStatChangedHandler(listActiveStat, OnStatChanged);
			}
		}

		protected override void RecordNewOffsets()
		{
			AwardGoal[] listGoals = _awardProgressTier.listGoals;
			foreach (AwardGoal awardGoal in listGoals)
			{
				AwardStatGoal awardStatGoal = awardGoal as AwardStatGoal;
				if (awardStatGoal != null)
				{
					Stat stat = awardStatGoal.stat;
					int value = stats[stat];
					if (!_statOffset.ContainsKey(stat))
					{
						_statOffset.Add(stat, value);
					}
					else
					{
						_statOffset[stat] = value;
					}
				}
			}
			_hasMigratedToUseStatOffset = true;
			_shouldMigrateToUseStatOffset = false;
		}

		public void OnStatChanged(Stat stat, int newValue)
		{
			AwardGoal[] listGoals = _awardProgressTier.listGoals;
			foreach (AwardGoal awardGoal in listGoals)
			{
				AwardStatGoal awardStatGoal = awardGoal as AwardStatGoal;
				if (awardStatGoal != null && stat == awardStatGoal.stat && stats[stat] >= awardGoal.goal + OffsetForStat(stat))
				{
					stats.RemoveStatChangedHandler(stat, OnStatChanged);
				}
			}
			CheckAndUpdateProgress();
		}

		public override string AwardToString()
		{
			if (_awardActiveTier != null && _awardProgressTier != null)
			{
				StringBuilder stringBuilder = new StringBuilder(string.Empty);
				string empty = string.Empty;
				foreach (KeyValuePair<Stat, int> item in _statOffset)
				{
					stringBuilder.Append(string.Format("{0}#{1}%", (int)item.Key, item.Value));
				}
				int num = stringBuilder.ToString().LastIndexOf('%');
				if (num >= 0)
				{
					stringBuilder.Remove(num, 1);
				}
				empty = stringBuilder.ToString();
				stringBuilder = new StringBuilder(string.Empty);
				stringBuilder.Append((int)_awardActiveTier.tierType + ",");
				stringBuilder.Append((int)_currentActiveAwardState + ",");
				stringBuilder.Append((int)_awardProgressTier.tierType + ",");
				stringBuilder.Append((int)_currentProgressAwardState + ",");
				stringBuilder.Append(_lastActiveStateChangeDateTime.ToString(CultureInfo.InvariantCulture) + ",");
				stringBuilder.Append(empty + ",");
				stringBuilder.Append(_hasMigratedToUseStatOffset);
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
				if (array.Length >= 6)
				{
					_savedActiveTierType = (AwardTierType)(int)Enum.Parse(typeof(AwardTierType), array[0], true);
					_currentActiveAwardState = (AwardState)(int)Enum.Parse(typeof(AwardState), array[1], true);
					_savedProgressTierType = (AwardTierType)(int)Enum.Parse(typeof(AwardTierType), array[2], true);
					_currentProgressAwardState = (AwardState)(int)Enum.Parse(typeof(AwardState), array[3], true);
					_lastActiveStateChangeDateTime = Utils.StringToDateTime(array[4], DateTime.MinValue);
					string[] array2 = array[5].Split('%');
					string[] array3 = array2;
					foreach (string text in array3)
					{
						int num = text.IndexOf('#');
						string value = text.Substring(0, num);
						int value2 = int.Parse(text.Substring(num + 1));
						Stat key = (Stat)(int)Enum.Parse(typeof(Stat), value, true);
						if (!_statOffset.ContainsKey(key))
						{
							_statOffset.Add(key, value2);
						}
						else
						{
							_statOffset[key] = value2;
						}
					}
				}
				if (array.Length >= 7)
				{
					_hasMigratedToUseStatOffset = bool.Parse(array[6]);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("StatAward parse failed: " + ex.ToString());
				ResetAwardToDefaultState();
			}
		}
	}
}
