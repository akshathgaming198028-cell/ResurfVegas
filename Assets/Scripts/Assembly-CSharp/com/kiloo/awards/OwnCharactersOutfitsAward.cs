using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using SYBO.Subway.Info;
using UnityEngine;

namespace com.kiloo.awards
{
	public class OwnCharactersOutfitsAward : Award
	{
		public OwnCharactersOutfitsAward(AwardStaticData trophyData, string savedProgress, Action<Award> handler)
			: base(trophyData, savedProgress, handler)
		{
		}

		private int GetNumberOfOwnedCharacterOutfits()
		{
			int num = 0;
			foreach (Characters.CharacterType item in Characters.characterOrder)
			{
				List<CharacterTheme> list = CharacterThemes.TryGetCustomThemesForChar(item);
				if (list == null || list.Count <= 0)
				{
					continue;
				}
				for (int i = 1; i <= list.Count; i++)
				{
					if (PlayerInfo.Instance.IsThemeUnlockedForCharacter(item, i))
					{
						num++;
					}
				}
			}
			return num;
		}

		protected override int GetProgressForGoal(AwardGoal goal)
		{
			return GetNumberOfOwnedCharacterOutfits();
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
			instance.OnCharacterOutfitUnlocked = (Action<Characters.CharacterType>)Delegate.Combine(instance.OnCharacterOutfitUnlocked, new Action<Characters.CharacterType>(OnOutfitUnlocked));
			CheckAndUpdateProgress();
		}

		protected override void DeactivateCurrentAwardTier()
		{
			PlayerInfo instance = PlayerInfo.Instance;
			instance.OnCharacterOutfitUnlocked = (Action<Characters.CharacterType>)Delegate.Remove(instance.OnCharacterOutfitUnlocked, new Action<Characters.CharacterType>(OnOutfitUnlocked));
		}

		private void OnOutfitUnlocked(Characters.CharacterType type)
		{
			CheckAndUpdateProgress();
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
						Debug.LogError("OwnCharacterOutfitAward parse failed: " + ex.ToString());
						ResetAwardToDefaultState();
						return;
					}
				}
				Debug.LogError("OwnCharacterOutfitAward parse failed :Split size not right: " + s);
				ResetAwardToDefaultState();
			}
			else
			{
				ResetAwardToDefaultState();
			}
		}
	}
}
