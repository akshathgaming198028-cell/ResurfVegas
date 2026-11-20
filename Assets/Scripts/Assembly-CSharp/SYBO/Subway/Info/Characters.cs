using System;
using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Subway.Info
{
	public class Characters
	{
		public enum UnlockType
		{
			free = 0,
			tokens = 1,
			coins = 2,
			keys = 3,
			facebook = 4
		}

		public enum CharacterType
		{
			slick = 0,
			tricky = 1,
			fresh = 2,
			spike = 3,
			yutani = 4,
			frank = 5,
			frizzy = 6,
			king = 7,
			lucy = 8,
			ninja = 9,
			tag = 10,
			tasha = 11,
			zoe = 12,
			brody = 13,
			princek = 14,
			zombiejake = 15,
			elftricky = 16,
			tony = 17,
			carmen = 18,
			roberto = 19,
			kim = 20,
			harumi = 21,
			nick = 22,
			coco = 23,
			monkeyking = 24,
			moscowCharacter01 = 25,
			halloweenCharacter01 = 26,
			londonCharacter01 = 27,
			mumbaiCharacter01 = 28,
			seoulCharacter01 = 29,
			mexicoCharacter01 = 30,
			vancouverCharacter01 = 31,
			saopauloCharacter01 = 32,
			losangelesCharacter01 = 33,
			cairoCharacter01 = 34,
			bangkokCharacter01 = 35,
			facebookRewardCharacter01 = 36,
			londonCharacter02 = 37,
			lasVegasCharacter01 = 38,
			hawaiiCharacter01 = 39,
			arabiaCharacter01 = 40,
			veniceCharacter01 = 41,
			greeceCharacter01 = 42,
			kenyaCharacter01 = 43,
			transylvaniaCharacter01 = 44,
			sanfranciscoCharacter01 = 45,
			pragueCharacter01 = 46,
			madagascarCharacter01 = 47,
			peruCharacter01 = 48
		}

		public struct Model
		{
			public StringID name;

			public string modelName;

			public int Price;

			public UnlockType unlockType;

			public string tokenName;

			public string tokenSprite2dName;

			public MissionTarget missionTargetKey;

			public StringID characterLimitedDescription;

			public bool isNewInThisUpdate;

			public bool hasOnlineSettings;

			public DateTime defaultExpirationDate;

			public DateTime defaultStartDate;

			public bool overrideDynamicIapIdAndroid;

			public bool overrideDynamicIapIdiOS;

			public bool overrideDynamicIapIdKindle;

			public bool overrideDynamicIapIdWP8;

			public string dynamicIapId;
		}

		public const string ONLINE_SETTINGS_CHARACTER_PREFIX_EXPIRE = "character_expiretime_";

		public const string ONLINE_SETTINGS_CHARACTER_PREFIX_START = "character_starttime_";

		public static readonly Dictionary<CharacterType, Model> characterData = new Dictionary<CharacterType, Model>
		{
			{
				CharacterType.slick,
				new Model
				{
					name = StringID.CHARACTERS_JAKE,
					modelName = "slick",
					unlockType = UnlockType.free,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterType.tricky,
				new Model
				{
					name = StringID.CHARACTERS_TRICKY,
					modelName = "tricky",
					unlockType = UnlockType.tokens,
					tokenName = "Tricky's Hat",
					tokenSprite2dName = "trophy_tricky_token",
					Price = 3,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterType.fresh,
				new Model
				{
					name = StringID.CHARACTERS_FRESH,
					modelName = "fresh",
					tokenName = "Fresh's Stereo",
					tokenSprite2dName = "trophy_fresh_token",
					unlockType = UnlockType.tokens,
					Price = 50,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterType.spike,
				new Model
				{
					name = StringID.CHARACTERS_SPIKE,
					modelName = "spike",
					unlockType = UnlockType.tokens,
					tokenName = "Spike's Guitar",
					tokenSprite2dName = "trophy_spike_token",
					Price = 200,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterType.yutani,
				new Model
				{
					name = StringID.CHARACTERS_YUTANI,
					modelName = "yutani",
					unlockType = UnlockType.tokens,
					tokenName = "Yutani's UFO",
					tokenSprite2dName = "trophy_yutani_token",
					Price = 500,
					missionTargetKey = MissionTarget.HaveYutani,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterType.lucy,
				new Model
				{
					name = StringID.CHARACTERS_LUCY,
					modelName = "lucy",
					unlockType = UnlockType.coins,
					Price = 7000,
					missionTargetKey = MissionTarget.HaveLucy,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterType.ninja,
				new Model
				{
					name = StringID.CHARACTERS_NINJA,
					modelName = "ninja",
					unlockType = UnlockType.coins,
					Price = 20000,
					missionTargetKey = MissionTarget.HaveNinja,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterType.frank,
				new Model
				{
					name = StringID.CHARACTERS_FRANK,
					modelName = "frank",
					unlockType = UnlockType.coins,
					Price = 40000,
					missionTargetKey = MissionTarget.HaveFrank,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterType.king,
				new Model
				{
					name = StringID.CHARACTERS_KING,
					modelName = "king",
					unlockType = UnlockType.coins,
					Price = 80000,
					missionTargetKey = MissionTarget.HaveKing,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterType.frizzy,
				new Model
				{
					name = StringID.CHARACTERS_FRIZZY,
					modelName = "frizzy",
					unlockType = UnlockType.coins,
					Price = 150000,
					missionTargetKey = MissionTarget.HaveFrizzy,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterType.tag,
				new Model
				{
					name = StringID.CHARACTERS_TAGBOT,
					modelName = "tag",
					unlockType = UnlockType.coins,
					Price = 12000,
					missionTargetKey = MissionTarget.HaveTag,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterType.tasha,
				new Model
				{
					name = StringID.CHARACTERS_TASHA,
					modelName = "tasha",
					unlockType = UnlockType.coins,
					Price = 30000,
					missionTargetKey = MissionTarget.HaveTasha,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterType.zoe,
				new Model
				{
					name = StringID.CHARACTERS_ZOE,
					modelName = "zoe",
					unlockType = UnlockType.coins,
					Price = 120000,
					missionTargetKey = MissionTarget.HaveZoe,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterType.brody,
				new Model
				{
					name = StringID.CHARACTERS_BRODY,
					modelName = "brody",
					unlockType = UnlockType.coins,
					Price = 350000,
					missionTargetKey = MissionTarget.HaveBrody,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterType.princek,
				new Model
				{
					name = StringID.CHARACTERS_PRINCE_K,
					modelName = "princek",
					unlockType = UnlockType.coins,
					Price = 980000,
					missionTargetKey = MissionTarget.HavePrinceK,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterType.zombiejake,
				new Model
				{
					name = StringID.CHARACTERS_ZOMBIE_JAKE,
					modelName = "zombiejake",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2012, 10, 10),
					defaultStartDate = new DateTime(2012, 9, 18),
					characterLimitedDescription = StringID.CHARACTERS_ZOMBIEJAKE_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.elftricky,
				new Model
				{
					name = StringID.CHARACTERS_ELF_TRICHY,
					modelName = "elftricky",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2013, 1, 1),
					defaultStartDate = new DateTime(2012, 11, 19),
					characterLimitedDescription = StringID.CHARACTERS_ELFTRICKY_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.tony,
				new Model
				{
					name = StringID.CHARACTERS_TONY,
					modelName = "tony",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2015, 9, 24),
					defaultStartDate = new DateTime(2015, 8, 14),
					characterLimitedDescription = StringID.CHARACTERS_TONY_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.carmen,
				new Model
				{
					name = StringID.CHARACTERS_CARMEN,
					modelName = "carmen",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2015, 7, 23),
					defaultStartDate = new DateTime(2015, 6, 3),
					characterLimitedDescription = StringID.CHARACTERS_CARMEN_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.roberto,
				new Model
				{
					name = StringID.CHARACTERS_ROBERTO,
					modelName = "roberto",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2014, 5, 8),
					defaultStartDate = new DateTime(2014, 3, 20),
					characterLimitedDescription = StringID.CHARACTERS_ROBERTO_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.kim,
				new Model
				{
					name = StringID.CHARACTERS_KIM,
					modelName = "kim",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2016, 5, 12),
					defaultStartDate = new DateTime(2016, 3, 30),
					characterLimitedDescription = StringID.CHARACTERS_KIM_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.harumi,
				new Model
				{
					name = StringID.CHARACTERS_HARUMI,
					modelName = "harumi",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2015, 12, 3),
					defaultStartDate = new DateTime(2015, 10, 13),
					characterLimitedDescription = StringID.CHARACTERS_HARUMI_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.nick,
				new Model
				{
					name = StringID.CHARACTERS_NICK,
					modelName = "nick",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2014, 2, 13),
					defaultStartDate = new DateTime(2014, 1, 7),
					characterLimitedDescription = StringID.CHARACTERS_NICK_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.coco,
				new Model
				{
					name = StringID.CHARACTERS_COCO,
					modelName = "coco",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2015, 4, 16),
					defaultStartDate = new DateTime(2015, 2, 25),
					characterLimitedDescription = StringID.CHARACTERS_COCO_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.monkeyking,
				new Model
				{
					name = StringID.CHARACTERS_MONKEYKING,
					modelName = "monkeyking",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2014, 9, 18),
					defaultStartDate = new DateTime(2014, 7, 8),
					characterLimitedDescription = StringID.CHARACTERS_MONKEYKING_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.moscowCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_MOSCOWCHARACTER01,
					modelName = "moscowCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2013, 10, 10),
					defaultStartDate = new DateTime(2013, 9, 4),
					characterLimitedDescription = StringID.CHARACTERS_MOSCOWCHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.halloweenCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_HALLOWEENCHARACTER01,
					modelName = "halloweenCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2014, 11, 6),
					defaultStartDate = new DateTime(2014, 9, 15),
					characterLimitedDescription = StringID.CHARACTERS_HALLOWEENCHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.londonCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_LONDON_CHARACTER01,
					modelName = "londonCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2014, 1, 2),
					defaultStartDate = new DateTime(2013, 11, 5),
					characterLimitedDescription = StringID.CHARACTERS_LONDON_CHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.mumbaiCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_MUMBAI_CHARACTER01,
					modelName = "mumbaiCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2015, 3, 26),
					defaultStartDate = new DateTime(2015, 2, 4),
					characterLimitedDescription = StringID.CHARACTERS_MUMBAI_CHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.seoulCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_SEOUL_CHARACTER01,
					modelName = "seoulCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2015, 2, 12),
					defaultStartDate = new DateTime(2014, 12, 16),
					characterLimitedDescription = StringID.CHARACTERS_SEOUL_CHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.mexicoCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_MEXICO_CHARACTER01,
					modelName = "mexicoCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2014, 4, 16),
					defaultStartDate = new DateTime(2014, 3, 19),
					characterLimitedDescription = StringID.CHARACTERS_MEXICO_CHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.vancouverCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_VANCOUVER_CHARACTER01,
					modelName = "vancouverCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2014, 5, 28),
					defaultStartDate = new DateTime(2014, 4, 1),
					characterLimitedDescription = StringID.CHARACTERS_VANCOUVER_CHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.saopauloCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_SAOPAULO_CHARACTER01,
					modelName = "saopauloCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2014, 7, 10),
					defaultStartDate = new DateTime(2014, 5, 2),
					characterLimitedDescription = StringID.CHARACTERS_SAOPAULO_CHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.losangelesCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_LOSANGELES_CHARACTER01,
					modelName = "losangelesCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2015, 6, 4),
					defaultStartDate = new DateTime(2015, 4, 21),
					characterLimitedDescription = StringID.CHARACTERS_LOSANGELES_CHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.cairoCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_CAIRO_CHARACTER01,
					modelName = "cairoCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2014, 10, 9),
					defaultStartDate = new DateTime(2014, 8, 7),
					characterLimitedDescription = StringID.CHARACTERS_CAIRO_CHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.bangkokCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_BANGKOK_CHARACTER01,
					modelName = "bangkokCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2014, 12, 4),
					defaultStartDate = new DateTime(2014, 9, 25),
					characterLimitedDescription = StringID.CHARACTERS_BANGKOK_CHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.facebookRewardCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_FACEBOOK_REWARD_CHARACTER01,
					modelName = "facebookRewardCharacter01",
					unlockType = UnlockType.facebook,
					Price = 1,
					isNewInThisUpdate = false,
					hasOnlineSettings = false,
					characterLimitedDescription = StringID.CHARACTERS_FACEBOOK_REWARD_CHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.londonCharacter02,
				new Model
				{
					name = StringID.CHARACTERS_LONDON_CHARACTER02,
					modelName = "londonCharacter02",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2015, 12, 31),
					defaultStartDate = new DateTime(2015, 11, 4),
					characterLimitedDescription = StringID.CHARACTERS_LONDON_CHARACTER02_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.lasVegasCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_LAS_VEGAS_CHARACTER01,
					modelName = "lasVegasCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = true,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2016, 6, 23),
					defaultStartDate = new DateTime(2016, 4, 5),
					characterLimitedDescription = StringID.CHARACTERS_LAS_VEGAS_CHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.hawaiiCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_HAWAII_CHARACTER01,
					modelName = "hawaiiCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2016, 1, 21),
					defaultStartDate = new DateTime(2015, 11, 26),
					characterLimitedDescription = StringID.CHARACTERS_HAWAII_CHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.arabiaCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_ARABIA_CHARACTER01,
					modelName = "arabiaCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2015, 3, 16),
					defaultExpirationDate = new DateTime(2016, 3, 10),
					characterLimitedDescription = StringID.CHARACTERS_ARABIA_CHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.veniceCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_VENICE_CHARACTER01,
					modelName = "veniceCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2015, 7, 2),
					defaultStartDate = new DateTime(2015, 5, 7),
					characterLimitedDescription = StringID.CHARACTERS_VENICE_CHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.greeceCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_GREECE_CHARACTER01,
					modelName = "greeceCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2015, 9, 3),
					defaultStartDate = new DateTime(2015, 7, 15),
					characterLimitedDescription = StringID.CHARACTERS_GREECE_CHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.kenyaCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_KENYA_CHARACTER01,
					modelName = "kenyaCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2015, 10, 15),
					defaultStartDate = new DateTime(2015, 9, 3),
					characterLimitedDescription = StringID.CHARACTERS_KENYA_CHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.transylvaniaCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_TRANSYLVANIA_CHARACTER01,
					modelName = "transylvaniaCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2015, 11, 5),
					defaultStartDate = new DateTime(2015, 9, 23),
					characterLimitedDescription = StringID.CHARACTERS_TRANSYLVANIA_CHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.sanfranciscoCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_SANFRANCISCO_CHARACTER01,
					modelName = "sanfranciscoCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2016, 2, 11),
					defaultStartDate = new DateTime(2015, 12, 11),
					characterLimitedDescription = StringID.CHARACTERS_SANFRANCISCO_CHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.pragueCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_PRAGUE_CHARACTER01,
					modelName = "pragueCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2016, 3, 31),
					defaultStartDate = new DateTime(2016, 1, 22),
					characterLimitedDescription = StringID.CHARACTERS_PRAGUE_CHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.madagascarCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_MADAGASCAR_CHARACTER01,
					modelName = "madagascarCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2016, 4, 21),
					defaultStartDate = new DateTime(2016, 2, 17),
					characterLimitedDescription = StringID.CHARACTERS_MADAGASCAR_CHARACTER01_LIMITED_DESCRIPTION
				}
			},
			{
				CharacterType.peruCharacter01,
				new Model
				{
					name = StringID.CHARACTERS_PERU_CHARACTER01,
					modelName = "peruCharacter01",
					unlockType = UnlockType.coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultExpirationDate = new DateTime(2016, 6, 2),
					defaultStartDate = new DateTime(2016, 2, 17),
					characterLimitedDescription = StringID.CHARACTERS_PERU_CHARACTER01_LIMITED_DESCRIPTION
				}
			}
		};

		public static List<CharacterType> characterOrder = new List<CharacterType>
		{
			CharacterType.yutani,
			CharacterType.spike,
			CharacterType.fresh,
			CharacterType.tricky,
			CharacterType.slick,
			CharacterType.facebookRewardCharacter01,
			CharacterType.lasVegasCharacter01,
			CharacterType.peruCharacter01,
			CharacterType.kim,
			CharacterType.madagascarCharacter01,
			CharacterType.pragueCharacter01,
			CharacterType.arabiaCharacter01,
			CharacterType.sanfranciscoCharacter01,
			CharacterType.hawaiiCharacter01,
			CharacterType.londonCharacter02,
			CharacterType.harumi,
			CharacterType.transylvaniaCharacter01,
			CharacterType.kenyaCharacter01,
			CharacterType.tony,
			CharacterType.greeceCharacter01,
			CharacterType.carmen,
			CharacterType.veniceCharacter01,
			CharacterType.losangelesCharacter01,
			CharacterType.bangkokCharacter01,
			CharacterType.cairoCharacter01,
			CharacterType.saopauloCharacter01,
			CharacterType.vancouverCharacter01,
			CharacterType.mexicoCharacter01,
			CharacterType.seoulCharacter01,
			CharacterType.mumbaiCharacter01,
			CharacterType.londonCharacter01,
			CharacterType.halloweenCharacter01,
			CharacterType.moscowCharacter01,
			CharacterType.monkeyking,
			CharacterType.coco,
			CharacterType.nick,
			CharacterType.roberto,
			CharacterType.zombiejake,
			CharacterType.elftricky,
			CharacterType.frank,
			CharacterType.frizzy,
			CharacterType.king,
			CharacterType.lucy,
			CharacterType.ninja,
			CharacterType.tag,
			CharacterType.tasha,
			CharacterType.zoe,
			CharacterType.brody,
			CharacterType.princek
		};
	}
}
