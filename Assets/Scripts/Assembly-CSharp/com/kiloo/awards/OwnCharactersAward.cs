using System;
using System.Globalization;
using System.Text;
using SYBO.Subway.Info;
using UnityEngine;

namespace com.kiloo.awards
{
	public class OwnCharactersAward : Award
	{
		public OwnCharactersAward(AwardStaticData trophyData, string savedProgress, Action<Award> handler)
			: base(trophyData, savedProgress, handler)
		{
		}

		private int GetNumberOfOwnedCharacters()
		{
			int num = 0;
			foreach (Characters.CharacterType item in Characters.characterOrder)
			{
				if (PlayerInfo.Instance.IsCollectionComplete(item))
				{
					num++;
				}
			}
			return num;
		}

		protected override int GetProgressForGoal(AwardGoal goal)
		{
			return GetNumberOfOwnedCharacters();
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
			instance.OnTokenCollected = (Action<Characters.CharacterType>)Delegate.Combine(instance.OnTokenCollected, new Action<Characters.CharacterType>(OnTokenPickUp));
			CheckAndUpdateProgress();
		}

		protected override void DeactivateCurrentAwardTier()
		{
			PlayerInfo instance = PlayerInfo.Instance;
			instance.OnTokenCollected = (Action<Characters.CharacterType>)Delegate.Remove(instance.OnTokenCollected, new Action<Characters.CharacterType>(OnTokenPickUp));
		}

		protected override void RecordNewOffsets()
		{
		}

		public void OnTokenPickUp(Characters.CharacterType type)
		{
			if (Characters.characterData[type].Price <= PlayerInfo.Instance.GetCollectedTokens(type))
			{
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
						Debug.LogError("OwnCharactersAward parse failed: " + ex.ToString());
						ResetAwardToDefaultState();
						return;
					}
				}
				Debug.LogError("OwnCharactersAward parse failed :Split size not right: " + s);
				ResetAwardToDefaultState();
			}
			else
			{
				ResetAwardToDefaultState();
			}
		}
	}
}
