using System.Collections.Generic;
using UnityEngine;

namespace com.kiloo.awards
{
	public class AwardStaticData
	{
		public enum AwardType
		{
			NotSet = 0,
			Stat = 1,
			DailyChallengeInRow = 2,
			OwnCharacters = 3,
			OwnBoards = 4,
			OwnCharacterOutfits = 5,
			ScorePointsSingleRunNoCoins = 6,
			CompleteMissionsSingleRun = 7,
			HaveSupersneakersXTime = 8,
			CollectCoinsSingleRun = 9,
			ScorePointsSingleRunNoJumpOrRoll = 10,
			ScorePointsSingleLane = 11
		}

		public AwardId id { get; set; }

		public bool isHidden { get; set; }

		public StringID title { get; set; }

		public StringID description { get; set; }

		public bool hasDescription { get; set; }

		public Dictionary<AwardTierType, AwardTier> missionTiers { get; set; }

		public bool useFacebook { get; set; }

		public bool useStatOffset { get; set; }

		public bool migrateToUseStatOffset { get; set; }

		public AwardType awardType { get; set; }

		public bool hasAnotherDescriptionForOneItemGoal { get; set; }

		public StringID descriptionForOneItemGoal { get; set; }

		public AwardStaticData()
		{
			isHidden = false;
			hasDescription = true;
			useFacebook = false;
			useStatOffset = true;
			migrateToUseStatOffset = false;
			hasAnotherDescriptionForOneItemGoal = false;
		}
	}
}
