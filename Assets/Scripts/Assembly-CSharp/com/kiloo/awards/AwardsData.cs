using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.kiloo.awards
{
	public static class AwardsData
	{
		private const string ICON_NAME_DIAMOND = "icon_gemstone";

		private const string ICON_NAME_GOLDBAR = "icon_goldbar";

		private const string ICON_NAME_CLOCK = "icon_classyclock";

		private const string ICON_NAME_DOLLAR = "icon_dollarchain";

		private const string ICON_NAME_SKULL = "icon_paperweight";

		private const string ICON_NAME_HEADPHONES = "icon_headphones";

		private const string ICON_NAME_PLATINUMRECORD = "icon_record";

		private const string ICON_NAME_CASSETTE = "icon_cassettetape";

		private static List<AwardId> trophyMissionsOrder;

		public static AwardStaticData[] _trophyMissionsData { get; private set; }

		static AwardsData()
		{
			trophyMissionsOrder = new List<AwardId>
			{
				AwardId.SCORE_POINTS_SINGLE_LANE,
				AwardId.TOPRUN_RANK_0,
				AwardId.COMPLETE_HUNT_SINGLE_RUN_X_TIMES,
				AwardId.OPEN_X_SUPER_MYSTERY_BOXES,
				AwardId.COMPLETE_X_WORDY_WEEKEND_CHALLENGES_IN_ONE_WEEKEND,
				AwardId.WIN_X_JACKPOTS,
				AwardId.COMPLETE_X_WORDY_WEEKEND_CHALLENGES,
				AwardId.SCORE_POINTS_SINGLE_RUN_NO_JUMP_OR_ROLL,
				AwardId.BEAT_X_TOPRUN_FRIENDS,
				AwardId.OPEN_X_MYSTERY_BOXES,
				AwardId.PICK_X_KEYS_INGAME,
				AwardId.POKE_FRIENDS_X_TIMES,
				AwardId.COLLECT_COINS_SINGLE_RUN,
				AwardId.HAVE_SUPERSNICKERS_ACTIVE_X_MIN_IN_A_ROW,
				AwardId.TOPRUN_RANK_1,
				AwardId.COMPLETE_MISSIONS_SINGLE_RUN,
				AwardId.SCORE_POINTS_SINGLE_RUN_NO_COINS,
				AwardId.OWN_CHARACTER_OUTFITS,
				AwardId.PICKUP_POWERUPS,
				AwardId.TOPRUN_RANK5_OR_BETTER,
				AwardId.OWN_X_BOARDS,
				AwardId.COMPLETE_DAILY_CHALLANGES_INROW,
				AwardId.OWN_X_CHARACTERS,
				AwardId.COMPLETE_X_MISSIONS,
				AwardId.COLLECT_X_COINS_FROM_FRIENDS,
				AwardId.COLLECT_OLD_TROPHY_ITEMS_MB,
				AwardId.COLLECT_OLD_TROPHY_ITEMS_SMB
			};
			_trophyMissionsData = new AwardStaticData[Enum.GetNames(typeof(AwardId)).Length];
			_trophyMissionsData[0] = new AwardStaticData
			{
				id = AwardId.COLLECT_OLD_TROPHY_ITEMS_MB,
				title = StringID.AWARD_1_TITLE,
				hasDescription = false,
				useStatOffset = true,
				awardType = AwardStaticData.AwardType.Stat,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[4]
							{
								new AwardStatGoal
								{
									stat = Stat.GoldChainClock,
									goal = 1,
									hasIcon = true,
									iconName = "icon_classyclock"
								},
								new AwardStatGoal
								{
									stat = Stat.Headphones,
									goal = 1,
									hasIcon = true,
									iconName = "icon_headphones"
								},
								new AwardStatGoal
								{
									stat = Stat.TapeBlack,
									goal = 1,
									hasIcon = true,
									iconName = "icon_cassettetape"
								},
								new AwardStatGoal
								{
									stat = Stat.LpBlack,
									goal = 1,
									hasIcon = true,
									iconName = "icon_record"
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[4]
							{
								new AwardStatGoal
								{
									stat = Stat.GoldChainClock,
									goal = 1,
									hasIcon = true,
									iconName = "icon_classyclock"
								},
								new AwardStatGoal
								{
									stat = Stat.Headphones,
									goal = 1,
									hasIcon = true,
									iconName = "icon_headphones"
								},
								new AwardStatGoal
								{
									stat = Stat.TapeBlack,
									goal = 1,
									hasIcon = true,
									iconName = "icon_cassettetape"
								},
								new AwardStatGoal
								{
									stat = Stat.LpBlack,
									goal = 1,
									hasIcon = true,
									iconName = "icon_record"
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[4]
							{
								new AwardStatGoal
								{
									stat = Stat.GoldChainClock,
									goal = 1,
									hasIcon = true,
									iconName = "icon_classyclock"
								},
								new AwardStatGoal
								{
									stat = Stat.Headphones,
									goal = 1,
									hasIcon = true,
									iconName = "icon_headphones"
								},
								new AwardStatGoal
								{
									stat = Stat.TapeBlack,
									goal = 1,
									hasIcon = true,
									iconName = "icon_cassettetape"
								},
								new AwardStatGoal
								{
									stat = Stat.LpBlack,
									goal = 1,
									hasIcon = true,
									iconName = "icon_record"
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[1] = new AwardStaticData
			{
				id = AwardId.COLLECT_OLD_TROPHY_ITEMS_SMB,
				title = StringID.AWARD_2_TITLE,
				hasDescription = false,
				useStatOffset = true,
				awardType = AwardStaticData.AwardType.Stat,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[4]
							{
								new AwardStatGoal
								{
									stat = Stat.GoldChainDollar,
									goal = 1,
									hasIcon = true,
									iconName = "icon_dollarchain"
								},
								new AwardStatGoal
								{
									stat = Stat.GoldSkull,
									goal = 1,
									hasIcon = true,
									iconName = "icon_paperweight"
								},
								new AwardStatGoal
								{
									stat = Stat.Goldbar,
									goal = 1,
									hasIcon = true,
									iconName = "icon_goldbar"
								},
								new AwardStatGoal
								{
									stat = Stat.Diamond,
									goal = 1,
									hasIcon = true,
									iconName = "icon_gemstone"
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[4]
							{
								new AwardStatGoal
								{
									stat = Stat.GoldChainDollar,
									goal = 1,
									hasIcon = true,
									iconName = "icon_dollarchain"
								},
								new AwardStatGoal
								{
									stat = Stat.GoldSkull,
									goal = 1,
									hasIcon = true,
									iconName = "icon_paperweight"
								},
								new AwardStatGoal
								{
									stat = Stat.Goldbar,
									goal = 1,
									hasIcon = true,
									iconName = "icon_goldbar"
								},
								new AwardStatGoal
								{
									stat = Stat.Diamond,
									goal = 1,
									hasIcon = true,
									iconName = "icon_gemstone"
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[4]
							{
								new AwardStatGoal
								{
									stat = Stat.GoldChainDollar,
									goal = 1,
									hasIcon = true,
									iconName = "icon_dollarchain"
								},
								new AwardStatGoal
								{
									stat = Stat.GoldSkull,
									goal = 1,
									hasIcon = true,
									iconName = "icon_paperweight"
								},
								new AwardStatGoal
								{
									stat = Stat.Goldbar,
									goal = 1,
									hasIcon = true,
									iconName = "icon_goldbar"
								},
								new AwardStatGoal
								{
									stat = Stat.Diamond,
									goal = 1,
									hasIcon = true,
									iconName = "icon_gemstone"
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[2] = new AwardStaticData
			{
				id = AwardId.COLLECT_X_COINS_FROM_FRIENDS,
				title = StringID.AWARD_3_TITLE,
				description = StringID.AWARD_3_DESCRIPTION,
				useFacebook = true,
				useStatOffset = true,
				migrateToUseStatOffset = true,
				awardType = AwardStaticData.AwardType.Stat,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.CoinsFriends,
									goal = 3000
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.CoinsFriends,
									goal = 20000
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.CoinsFriends,
									goal = 100000
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[4] = new AwardStaticData
			{
				id = AwardId.COMPLETE_X_MISSIONS,
				title = StringID.AWARD_4_TITLE,
				description = StringID.AWARD_4_DESCRIPTION,
				awardType = AwardStaticData.AwardType.Stat,
				useStatOffset = true,
				migrateToUseStatOffset = true,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.MissionCompleted,
									goal = 7
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.MissionCompleted,
									goal = 50
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.MissionCompleted,
									goal = 210
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[3] = new AwardStaticData
			{
				id = AwardId.OWN_X_CHARACTERS,
				title = StringID.AWARD_5_TITLE,
				description = StringID.AWARD_5_DESCRIPTION,
				awardType = AwardStaticData.AwardType.OwnCharacters,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 2
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 12
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 30
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[5] = new AwardStaticData
			{
				id = AwardId.COMPLETE_DAILY_CHALLANGES_INROW,
				title = StringID.AWARD_6_TITLE,
				description = StringID.AWARD_6_DESCRIPTION,
				awardType = AwardStaticData.AwardType.DailyChallengeInRow,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 5
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 21
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 42
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[6] = new AwardStaticData
			{
				id = AwardId.OWN_X_BOARDS,
				title = StringID.AWARD_7_TITLE,
				description = StringID.AWARD_7_DESCRIPTION,
				awardType = AwardStaticData.AwardType.OwnBoards,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 5
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 14
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 34
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[7] = new AwardStaticData
			{
				id = AwardId.TOPRUN_RANK5_OR_BETTER,
				title = StringID.AWARD_8_TITLE,
				description = StringID.AWARD_8_DESCRIPTION,
				awardType = AwardStaticData.AwardType.Stat,
				useStatOffset = true,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.TopRunRank5orBetter,
									goal = 3
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.TopRunRank5orBetter,
									goal = 14
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.TopRunRank5orBetter,
									goal = 21
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[8] = new AwardStaticData
			{
				id = AwardId.PICKUP_POWERUPS,
				title = StringID.AWARD_9_TITLE,
				description = StringID.AWARD_9_DESCRIPTION,
				awardType = AwardStaticData.AwardType.Stat,
				useStatOffset = true,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.PickupPowerup,
									goal = 100
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.PickupPowerup,
									goal = 1200
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.PickupPowerup,
									goal = 5000
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[9] = new AwardStaticData
			{
				id = AwardId.OWN_CHARACTER_OUTFITS,
				title = StringID.AWARD_10_TITLE,
				description = StringID.AWARD_10_DESCRIPTION,
				awardType = AwardStaticData.AwardType.OwnCharacterOutfits,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 5
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 16
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 32
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[10] = new AwardStaticData
			{
				id = AwardId.SCORE_POINTS_SINGLE_RUN_NO_COINS,
				title = StringID.AWARD_11_TITLE,
				description = StringID.AWARD_11_DESCRIPTION,
				awardType = AwardStaticData.AwardType.ScorePointsSingleRunNoCoins,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 50000
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 80000
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 120000
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[11] = new AwardStaticData
			{
				id = AwardId.COMPLETE_MISSIONS_SINGLE_RUN,
				title = StringID.AWARD_12_TITLE,
				description = StringID.AWARD_12_DESCRIPTION,
				awardType = AwardStaticData.AwardType.CompleteMissionsSingleRun,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 2
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 3
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 6
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[12] = new AwardStaticData
			{
				id = AwardId.TOPRUN_RANK_1,
				title = StringID.AWARD_13_TITLE,
				description = StringID.AWARD_13_DESCRIPTION,
				awardType = AwardStaticData.AwardType.Stat,
				useStatOffset = true,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.TopRunRank1,
									goal = 3
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.TopRunRank1,
									goal = 7
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.TopRunRank1,
									goal = 15
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[13] = new AwardStaticData
			{
				id = AwardId.HAVE_SUPERSNICKERS_ACTIVE_X_MIN_IN_A_ROW,
				title = StringID.AWARD_14_TITLE,
				description = StringID.AWARD_14_DESCRIPTION,
				hasAnotherDescriptionForOneItemGoal = true,
				descriptionForOneItemGoal = StringID.AWARD_14_DESCRIPTION_ONEITEM_GOAL,
				awardType = AwardStaticData.AwardType.HaveSupersneakersXTime,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 1
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 2
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 3
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[14] = new AwardStaticData
			{
				id = AwardId.COLLECT_COINS_SINGLE_RUN,
				title = StringID.AWARD_15_TITLE,
				description = StringID.AWARD_15_DESCRIPTION,
				hasAnotherDescriptionForOneItemGoal = false,
				awardType = AwardStaticData.AwardType.CollectCoinsSingleRun,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 30
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 120
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 810
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[15] = new AwardStaticData
			{
				id = AwardId.POKE_FRIENDS_X_TIMES,
				title = StringID.AWARD_16_TITLE,
				description = StringID.AWARD_16_DESCRIPTION,
				useFacebook = true,
				useStatOffset = true,
				awardType = AwardStaticData.AwardType.Stat,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.PokeFriend,
									goal = 20
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.PokeFriend,
									goal = 100
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.PokeFriend,
									goal = 300
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[16] = new AwardStaticData
			{
				id = AwardId.PICK_X_KEYS_INGAME,
				title = StringID.AWARD_17_TITLE,
				description = StringID.AWARD_17_DESCRIPTION,
				useFacebook = false,
				useStatOffset = true,
				awardType = AwardStaticData.AwardType.Stat,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.KeysCollected,
									goal = 10
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.KeysCollected,
									goal = 25
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.KeysCollected,
									goal = 50
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[17] = new AwardStaticData
			{
				id = AwardId.OPEN_X_MYSTERY_BOXES,
				title = StringID.AWARD_18_TITLE,
				description = StringID.AWARD_18_DESCRIPTION,
				useFacebook = false,
				useStatOffset = true,
				awardType = AwardStaticData.AwardType.Stat,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.MysteryBoxesOpened,
									goal = 20
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.MysteryBoxesOpened,
									goal = 160
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.MysteryBoxesOpened,
									goal = 320
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[18] = new AwardStaticData
			{
				id = AwardId.BEAT_X_TOPRUN_FRIENDS,
				title = StringID.AWARD_19_TITLE,
				description = StringID.AWARD_19_DESCRIPTION,
				useFacebook = true,
				useStatOffset = true,
				awardType = AwardStaticData.AwardType.Stat,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.TopRunFriendsBeaten,
									goal = 10
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.TopRunFriendsBeaten,
									goal = 50
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.TopRunFriendsBeaten,
									goal = 200
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[19] = new AwardStaticData
			{
				id = AwardId.SCORE_POINTS_SINGLE_RUN_NO_JUMP_OR_ROLL,
				title = StringID.AWARD_20_TITLE,
				description = StringID.AWARD_20_DESCRIPTION,
				useFacebook = false,
				useStatOffset = true,
				awardType = AwardStaticData.AwardType.ScorePointsSingleRunNoJumpOrRoll,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 60000
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 140000
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 300000
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[20] = new AwardStaticData
			{
				id = AwardId.COMPLETE_X_WORDY_WEEKEND_CHALLENGES,
				title = StringID.AWARD_21_TITLE,
				description = StringID.AWARD_21_DESCRIPTION,
				useFacebook = false,
				useStatOffset = true,
				awardType = AwardStaticData.AwardType.Stat,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.WordyWeekendChallengesCompleted,
									goal = 10
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.WordyWeekendChallengesCompleted,
									goal = 70
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.WordyWeekendChallengesCompleted,
									goal = 120
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[21] = new AwardStaticData
			{
				id = AwardId.WIN_X_JACKPOTS,
				title = StringID.AWARD_22_TITLE,
				description = StringID.AWARD_22_DESCRIPTION,
				useFacebook = true,
				useStatOffset = false,
				awardType = AwardStaticData.AwardType.Stat,
				hasAnotherDescriptionForOneItemGoal = true,
				descriptionForOneItemGoal = StringID.AWARD_22_DESCRIPTION_ONEITEM,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.JackpotsWon,
									goal = 1
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.JackpotsWon,
									goal = 2
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.JackpotsWon,
									goal = 3
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[22] = new AwardStaticData
			{
				id = AwardId.COMPLETE_X_WORDY_WEEKEND_CHALLENGES_IN_ONE_WEEKEND,
				title = StringID.AWARD_23_TITLE,
				description = StringID.AWARD_23_DESCRIPTION,
				useFacebook = false,
				useStatOffset = true,
				awardType = AwardStaticData.AwardType.Stat,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.WordyWeekendChallengesCompletedOneWeekend,
									goal = 5
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.WordyWeekendChallengesCompletedOneWeekend,
									goal = 8
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.WordyWeekendChallengesCompletedOneWeekend,
									goal = 12
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[23] = new AwardStaticData
			{
				id = AwardId.OPEN_X_SUPER_MYSTERY_BOXES,
				title = StringID.AWARD_24_TITLE,
				description = StringID.AWARD_24_DESCRIPTION,
				useFacebook = false,
				useStatOffset = true,
				awardType = AwardStaticData.AwardType.Stat,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.SuperMysteryBoxesOpened,
									goal = 10
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.SuperMysteryBoxesOpened,
									goal = 80
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.SuperMysteryBoxesOpened,
									goal = 160
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[24] = new AwardStaticData
			{
				id = AwardId.COMPLETE_HUNT_SINGLE_RUN_X_TIMES,
				title = StringID.AWARD_25_TITLE,
				description = StringID.AWARD_25_DESCRIPTION,
				useFacebook = true,
				useStatOffset = true,
				awardType = AwardStaticData.AwardType.Stat,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.HuntsCompletedInOneRun,
									goal = 2
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.HuntsCompletedInOneRun,
									goal = 6
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.HuntsCompletedInOneRun,
									goal = 12
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[25] = new AwardStaticData
			{
				id = AwardId.TOPRUN_RANK_0,
				title = StringID.AWARD_26_TITLE,
				description = StringID.AWARD_26_DESCRIPTION,
				awardType = AwardStaticData.AwardType.Stat,
				useStatOffset = true,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.TopRunRank0,
									goal = 5
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.TopRunRank0,
									goal = 10
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardStatGoal
								{
									stat = Stat.TopRunRank0,
									goal = 20
								}
							}
						}
					}
				}
			};
			_trophyMissionsData[26] = new AwardStaticData
			{
				id = AwardId.SCORE_POINTS_SINGLE_LANE,
				title = StringID.AWARD_27_TITLE,
				description = StringID.AWARD_27_DESCRIPTION,
				awardType = AwardStaticData.AwardType.ScorePointsSingleLane,
				missionTiers = new Dictionary<AwardTierType, AwardTier>
				{
					{
						AwardTierType.Bronze,
						new AwardTier
						{
							tierType = AwardTierType.Bronze,
							reward = 3,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 45000
								}
							}
						}
					},
					{
						AwardTierType.Silver,
						new AwardTier
						{
							tierType = AwardTierType.Silver,
							reward = 5,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 90000
								}
							}
						}
					},
					{
						AwardTierType.Gold,
						new AwardTier
						{
							tierType = AwardTierType.Gold,
							reward = 8,
							listGoals = new AwardGoal[1]
							{
								new AwardSimpleGoal
								{
									goal = 180000
								}
							}
						}
					}
				}
			};
		}

		public static AwardStaticData[] GetAwardStaticDataList()
		{
			AwardStaticData[] array = new AwardStaticData[trophyMissionsOrder.Count];
			for (int i = 0; i < trophyMissionsOrder.Count; i++)
			{
				array[i] = _trophyMissionsData[(int)trophyMissionsOrder[i]];
			}
			return array;
		}

		public static AwardStaticData GetAwardStaticDataItem(int index)
		{
			return _trophyMissionsData[index];
		}

		public static void ResetWordyWeekendChallengeOneWeekend()
		{
			AwardStaticData awardStaticDataItem = GetAwardStaticDataItem(22);
			Dictionary<AwardTierType, AwardTier> missionTiers = awardStaticDataItem.missionTiers;
			int num = PlayerInfo.Instance.stats[Stat.WordyWeekendChallengesCompletedOneWeekend];
			int goal = missionTiers[AwardTierType.Bronze].listGoals[0].goal;
			int goal2 = missionTiers[AwardTierType.Silver].listGoals[0].goal;
			int goal3 = missionTiers[AwardTierType.Gold].listGoals[0].goal;
			if (num < goal3 + goal2 + goal)
			{
				if (num >= goal2 + goal)
				{
					PlayerInfo.Instance.stats[Stat.WordyWeekendChallengesCompletedOneWeekend] = goal2 + goal;
				}
				else if (num >= goal)
				{
					PlayerInfo.Instance.stats[Stat.WordyWeekendChallengesCompletedOneWeekend] = goal;
				}
				else
				{
					PlayerInfo.Instance.stats[Stat.WordyWeekendChallengesCompletedOneWeekend] = 0;
				}
			}
		}
	}
}
