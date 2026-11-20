using System;
using System.Text;
using SYBO.Subway.Info;
using UnityEngine;
using com.kiloo.awards;

public class CelebrationReward
{
	public CelebrationRewardOrigin CelebrationRewardOrigin;

	public CelebrationRewardType rewardType;

	public int amount;

	public Characters.CharacterType characterType;

	public int characterOutfitIndex;

	public Hoverboards.BoardType boardType;

	public OldTrophies.Trophy trophyType;

	public PowerupType powerupType;

	public long Uid;

	public AwardTierType awardTierType;

	public string awardTitle;

	public int rank = 1;

	public int score;

	public int weekNumber;

	public Hoverboard.BoardAbility boardPower;

	public int characterEventCompleted;

	public string description = string.Empty;

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(CelebrationRewardOrigin).Append(",");
		stringBuilder.Append(rewardType).Append(",");
		stringBuilder.Append(amount).Append(",");
		stringBuilder.Append(characterType).Append(",");
		stringBuilder.Append(characterOutfitIndex).Append(",");
		stringBuilder.Append(boardType).Append(",");
		stringBuilder.Append(trophyType).Append(",");
		stringBuilder.Append(powerupType).Append(",");
		stringBuilder.Append(Uid).Append(",");
		stringBuilder.Append(awardTierType).Append(",");
		stringBuilder.Append(awardTitle).Append(",");
		stringBuilder.Append(rank).Append(",");
		stringBuilder.Append(score).Append(",");
		stringBuilder.Append(weekNumber).Append(",");
		stringBuilder.Append(boardPower).Append(",");
		stringBuilder.Append(characterEventCompleted).Append(",");
		stringBuilder.Append(description);
		return stringBuilder.ToString();
	}

	public void PopulateFromString(string rewardAsString)
	{
		ResetToDefaultValues();
		string[] array = rewardAsString.Split(',');
		try
		{
			if (array.Length >= 9)
			{
				CelebrationRewardOrigin = (CelebrationRewardOrigin)(int)Enum.Parse(typeof(CelebrationRewardOrigin), array[0]);
				rewardType = (CelebrationRewardType)(int)Enum.Parse(typeof(CelebrationRewardType), array[1]);
				amount = int.Parse(array[2]);
				characterType = (Characters.CharacterType)(int)Enum.Parse(typeof(Characters.CharacterType), array[3]);
				characterOutfitIndex = int.Parse(array[4]);
				boardType = (Hoverboards.BoardType)(int)Enum.Parse(typeof(Hoverboards.BoardType), array[5]);
				trophyType = (OldTrophies.Trophy)(int)Enum.Parse(typeof(OldTrophies.Trophy), array[6]);
				powerupType = (PowerupType)(int)Enum.Parse(typeof(PowerupType), array[7]);
				Uid = long.Parse(array[8]);
			}
			if (array.Length >= 11)
			{
				awardTierType = (AwardTierType)(int)Enum.Parse(typeof(AwardTierType), array[9]);
				awardTitle = array[10];
			}
			if (array.Length >= 14)
			{
				rank = int.Parse(array[11]);
				score = int.Parse(array[12]);
				weekNumber = int.Parse(array[13]);
			}
			if (array.Length >= 15)
			{
				boardPower = (Hoverboard.BoardAbility)(int)Enum.Parse(typeof(Hoverboard.BoardAbility), array[14]);
			}
			if (array.Length >= 17)
			{
				characterEventCompleted = int.Parse(array[15]);
				description = array[16];
			}
		}
		catch (Exception ex)
		{
			ResetToDefaultValues();
			Debug.LogError("The string could not be converted to a reward!" + ex.Message);
		}
	}

	private void ResetToDefaultValues()
	{
		CelebrationRewardOrigin = CelebrationRewardOrigin.Notset;
		rewardType = CelebrationRewardType._notset;
		amount = 0;
		characterType = Characters.CharacterType.slick;
		characterOutfitIndex = 0;
		boardType = Hoverboards.BoardType.normal;
		trophyType = OldTrophies.Trophy.diamond;
		powerupType = PowerupType._notset;
		Uid = 0L;
		awardTierType = AwardTierType.None;
		awardTitle = string.Empty;
		rank = 1;
		score = 0;
		weekNumber = 1;
		boardPower = Hoverboard.BoardAbility.None;
	}
}
