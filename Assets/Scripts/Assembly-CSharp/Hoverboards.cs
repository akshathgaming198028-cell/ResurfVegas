using System;
using System.Collections.Generic;
using UnityEngine;

public class Hoverboards
{
	public enum BoardType
	{
		normal = 0,
		bouncer = 1,
		lowrider = 2,
		snowboard = 3,
		surfboard = 4,
		theoriginal = 5,
		starboard = 6,
		miami = 7,
		monster = 8,
		skullfire = 9,
		liberty = 10,
		speedboard = 11,
		toucan = 12,
		treehugger = 13,
		chick = 14,
		rome = 15,
		soccer = 16,
		theOutback = 17,
		greatWhiteWakeboard = 18,
		cherry = 19,
		kitty = 20,
		hotrod = 21,
		flamingo = 22,
		rose = 23,
		panda = 24,
		dragon = 25,
		moscowBoard01 = 26,
		halloweenBoard01 = 27,
		londonBoard01 = 28,
		londonBoard02 = 29,
		mumbaiBoard01 = 30,
		teleboard = 31,
		gliderboard = 32,
		seoulBoard01 = 33,
		mexicoBoard01 = 34,
		vancouverBoard01 = 35,
		saopauloBoard01 = 36,
		losangelesBoard01 = 37,
		cairoBoard01 = 38,
		bangkokBoard01 = 39,
		londonBoard03 = 40,
		lasVegasBoard01 = 41,
		hawaiiBoard01 = 42,
		arabiaBoard01 = 43,
		veniceBoard01 = 44,
		rioBoard01 = 45,
		sydneyBoard01 = 46,
		greeceBoard01 = 47,
		kenyaBoard01 = 48,
		transylvaniaBoard01 = 49,
		northpoleBoard01 = 50,
		sanfranciscoBoard01 = 51,
		pragueBoard01 = 52,
		madagascarBoard01 = 53,
		sydneyBoard02 = 54,
		peruBoard01 = 55
	}

	public enum UnlockType
	{
		alwaysUnlocked = 0,
		free = 1,
		coins = 2,
		eggHunt = 3,
		itemHunt = 4,
		hiddenUntillUnlocked = 5,
		keys = 6
	}

	public struct Board
	{
		public StringID name;

		public string boardModelName;

		public int price;

		public UnlockType unlockType;

		public string tokenSprite2dName;

		public StringID description;

		public StringID limitedDescription;

		public bool isNewInThisUpdate;

		public bool hasOnlineTimeLimit;

		public DateTime defaultExpirationTime;

		public DateTime defaultStartDate;

		public bool overrideDynamicIapIdAndroid;

		public bool overrideDynamicIapIdiOS;

		public bool overrideDynamicIapIdKindle;

		public bool overrideDynamicIapIdWP8;

		public string dynamicIapId;
	}

	public const string ONLINE_SETTINGS_BOARD_PREFIX_EXPIRE = "hoverboard_expiretime_";

	public const string ONLINE_SETTINGS_BOARD_PREFIX_START = "hoverboard_starttime_";

	public static readonly Dictionary<BoardType, Board> boardData = new Dictionary<BoardType, Board>
	{
		{
			BoardType.normal,
			new Board
			{
				name = StringID.HOVERBOARDS_HOVERBOARD,
				boardModelName = "Hoverboard",
				price = 0,
				unlockType = UnlockType.alwaysUnlocked,
				description = StringID.NULL,
				limitedDescription = StringID.NULL,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.bouncer,
			new Board
			{
				name = StringID.HOVERBOARDS_BOUNCER,
				boardModelName = "Jumpboard",
				price = 280000,
				unlockType = UnlockType.coins,
				description = StringID.HOVERBOARDS_BOUNCER_DESCRIPTION,
				limitedDescription = StringID.NULL,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.lowrider,
			new Board
			{
				name = StringID.HOVERBOARDS_LOWRIDER,
				boardModelName = "Lowrider",
				price = 320000,
				unlockType = UnlockType.coins,
				description = StringID.HOVERBOARDS_LOWRIDER_DESCRIPTION,
				limitedDescription = StringID.NULL,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.snowboard,
			new Board
			{
				name = StringID.HOVERBOARDS_FREESTYLER,
				boardModelName = "Snowboard",
				price = 45000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.NULL,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.surfboard,
			new Board
			{
				name = StringID.HOVERBOARDS_BIG_KAHUNA,
				boardModelName = "Surfboard",
				price = 65000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.NULL,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.theoriginal,
			new Board
			{
				name = StringID.HOVERBOARDS_SUPERHERO,
				boardModelName = "Bamboard",
				price = 8000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.NULL,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.starboard,
			new Board
			{
				name = StringID.HOVERBOARDS_STARBOARD,
				boardModelName = "Starboard",
				price = 0,
				unlockType = UnlockType.free,
				description = StringID.NULL,
				limitedDescription = StringID.NULL,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.miami,
			new Board
			{
				name = StringID.HOVERBOARDS_MIAMI,
				boardModelName = "Miami",
				price = 12000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.NULL,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.monster,
			new Board
			{
				name = StringID.HOVERBOARDS_MONSTER,
				boardModelName = "Monster",
				price = 30000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.NULL,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.skullfire,
			new Board
			{
				name = StringID.HOVERBOARDS_SKULL_FIRE,
				boardModelName = "SkullFire",
				price = 75000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.NULL,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.liberty,
			new Board
			{
				name = StringID.HOVERBOARDS_LIBERTY,
				boardModelName = "Liberty",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_LIBERTY_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2015, 9, 24),
				defaultStartDate = new DateTime(2015, 8, 18)
			}
		},
		{
			BoardType.speedboard,
			new Board
			{
				name = StringID.HOVERBOARDS_DAREDEVIL,
				boardModelName = "Daredevil",
				price = 85000,
				unlockType = UnlockType.coins,
				description = StringID.HOVERBOARDS_DAREDEVIL_DESCRIPTION,
				limitedDescription = StringID.NULL,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.toucan,
			new Board
			{
				name = StringID.HOVERBOARDS_TOUCAN,
				boardModelName = "RioBoard",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_TOUCAN_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2013, 2, 28),
				defaultStartDate = new DateTime(2013, 1, 28)
			}
		},
		{
			BoardType.treehugger,
			new Board
			{
				name = StringID.HOVERBOARDS_LUMBERJACK,
				boardModelName = "Treehugger",
				price = 4000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.NULL,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.chick,
			new Board
			{
				name = StringID.HOVERBOARDS_CHICKY,
				boardModelName = "Chick",
				tokenSprite2dName = "icon_egg",
				price = 100,
				unlockType = UnlockType.eggHunt,
				description = StringID.NULL,
				limitedDescription = StringID.NULL,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultStartDate = new DateTime(2016, 1, 22),
				defaultExpirationTime = new DateTime(2016, 3, 31)
			}
		},
		{
			BoardType.rome,
			new Board
			{
				name = StringID.HOVERBOARDS_SCOOT,
				boardModelName = "Rome",
				price = 35000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.NULL,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.soccer,
			new Board
			{
				name = StringID.HOVERBOARDS_KICK_OFF,
				boardModelName = "Soccer",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_KICK_OFF_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2014, 5, 8),
				defaultStartDate = new DateTime(2014, 3, 20)
			}
		},
		{
			BoardType.theOutback,
			new Board
			{
				name = StringID.HOVERBOARDS_OUTBACK,
				boardModelName = "TheOutback",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_OUTBACK_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2013, 5, 2),
				defaultStartDate = new DateTime(2013, 3, 20)
			}
		},
		{
			BoardType.greatWhiteWakeboard,
			new Board
			{
				name = StringID.HOVERBOARDS_GREAT_WHITE,
				boardModelName = "GreatWhiteWakeboard",
				price = 20000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.NULL,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.cherry,
			new Board
			{
				name = StringID.HOVERBOARDS_CHERRY,
				boardModelName = "Cherry",
				tokenSprite2dName = "icon_doll_medium",
				price = 60,
				unlockType = UnlockType.itemHunt,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_CHERRY_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2013, 5, 30),
				defaultStartDate = new DateTime(2013, 5, 23)
			}
		},
		{
			BoardType.kitty,
			new Board
			{
				name = StringID.HOVERBOARDS_KITTY,
				boardModelName = "Fortune",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_KITTY_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2015, 12, 3),
				defaultStartDate = new DateTime(2015, 10, 13)
			}
		},
		{
			BoardType.hotrod,
			new Board
			{
				name = StringID.HOVERBOARDS_HOTROD,
				boardModelName = "HotRod",
				price = 280000,
				unlockType = UnlockType.coins,
				description = StringID.HOVERBOARDS_HOTROD_DESCRIPTION,
				limitedDescription = StringID.NULL,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.flamingo,
			new Board
			{
				name = StringID.HOVERBOARDS_FLAMINGO,
				boardModelName = "Flamingo",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_FLAMINGO_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2014, 2, 13),
				defaultStartDate = new DateTime(2014, 1, 7)
			}
		},
		{
			BoardType.rose,
			new Board
			{
				name = StringID.HOVERBOARDS_ROSE,
				boardModelName = "Rose",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_ROSE_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2015, 4, 16),
				defaultStartDate = new DateTime(2015, 2, 25)
			}
		},
		{
			BoardType.panda,
			new Board
			{
				name = StringID.HOVERBOARDS_PANDA,
				boardModelName = "Panda",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_PANDA_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2014, 9, 18),
				defaultStartDate = new DateTime(2014, 7, 8)
			}
		},
		{
			BoardType.dragon,
			new Board
			{
				name = StringID.HOVERBOARDS_DRAGON,
				boardModelName = "Dragon",
				tokenSprite2dName = "icon_hunt_item_big",
				price = 60,
				unlockType = UnlockType.itemHunt,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_DRAGON_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2013, 8, 1),
				defaultStartDate = new DateTime(2013, 6, 2)
			}
		},
		{
			BoardType.moscowBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_MOSCOWBOARD01,
				boardModelName = "Teddy",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_MOSCOWBOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2013, 10, 10),
				defaultStartDate = new DateTime(2013, 9, 4)
			}
		},
		{
			BoardType.halloweenBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_HALLOWEENBOARD01,
				boardModelName = "Pumpkin",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_HALLOWEENBOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2014, 11, 6),
				defaultStartDate = new DateTime(2014, 9, 15),
				overrideDynamicIapIdiOS = true,
				dynamicIapId = "com.kiloo.subwayboard.halloweenBoard01v2"
			}
		},
		{
			BoardType.londonBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_LONDONBOARD01,
				boardModelName = "londonBoard01",
				tokenSprite2dName = "icon_doll_medium",
				price = 60,
				unlockType = UnlockType.hiddenUntillUnlocked,
				description = StringID.NULL,
				limitedDescription = StringID.NULL,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.londonBoard02,
			new Board
			{
				name = StringID.HOVERBOARDS_LONDONBOARD02,
				boardModelName = "londonBoard02",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_LONDONBOARD02_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2015, 1, 2),
				defaultStartDate = new DateTime(2014, 10, 21)
			}
		},
		{
			BoardType.mumbaiBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_MUMBAIBOARD01,
				boardModelName = "mumbaiBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_MUMBAIBOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2015, 3, 26),
				defaultStartDate = new DateTime(2015, 2, 4)
			}
		},
		{
			BoardType.teleboard,
			new Board
			{
				name = StringID.HOVERBOARDS_TELEBOARD,
				boardModelName = "teleboard",
				price = 220000,
				unlockType = UnlockType.coins,
				description = StringID.HOVERBOARDS_TELEBOARD_DESCRIPTION,
				limitedDescription = StringID.NULL,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.seoulBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_SEOULBOARD01,
				boardModelName = "seoulBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_SEOULBOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2015, 2, 12),
				defaultStartDate = new DateTime(2014, 12, 16)
			}
		},
		{
			BoardType.mexicoBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_MEXICOBOARD01,
				boardModelName = "mexicoBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_MEXICOBOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2014, 4, 16),
				defaultStartDate = new DateTime(2014, 3, 20)
			}
		},
		{
			BoardType.vancouverBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_VANCOUVERBOARD01,
				boardModelName = "vancouverBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_VANCOUVERBOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2014, 5, 28),
				defaultStartDate = new DateTime(2014, 4, 1)
			}
		},
		{
			BoardType.gliderboard,
			new Board
			{
				name = StringID.HOVERBOARDS_GLIDER,
				boardModelName = "gliderboard",
				price = 360000,
				unlockType = UnlockType.coins,
				description = StringID.HOVERBOARDS_GLIDER_DESCRIPTION,
				limitedDescription = StringID.NULL,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.saopauloBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_SAOPAULOBOARD01,
				boardModelName = "saopauloBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_SAOPAULOBOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2014, 7, 10),
				defaultStartDate = new DateTime(2014, 5, 2)
			}
		},
		{
			BoardType.losangelesBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_LOSANGELESBOARD01,
				boardModelName = "losangelesBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_LOSANGELESBOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2015, 6, 4),
				defaultStartDate = new DateTime(2015, 4, 21)
			}
		},
		{
			BoardType.cairoBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_CAIROBOARD01,
				boardModelName = "cairoBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_CAIROBOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2014, 10, 9),
				defaultStartDate = new DateTime(2014, 8, 7)
			}
		},
		{
			BoardType.bangkokBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_BANGKOKBOARD01,
				boardModelName = "bangkokBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_BANGKOKBOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2014, 12, 4),
				defaultStartDate = new DateTime(2014, 9, 25)
			}
		},
		{
			BoardType.londonBoard03,
			new Board
			{
				name = StringID.HOVERBOARDS_LONDONBOARD03,
				boardModelName = "londonBoard03",
				price = 1,
				unlockType = UnlockType.hiddenUntillUnlocked,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_LONDONBOARD03_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.lasVegasBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_LAS_VEGAS_BOARD01,
				boardModelName = "lasVegasBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_LAS_VEGAS_BOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = true,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2016, 6, 23),
				defaultStartDate = new DateTime(2016, 6, 2)
			}
		},
		{
			BoardType.hawaiiBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_HAWAII_BOARD01,
				boardModelName = "hawaiiBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_HAWAII_BOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2016, 1, 21),
				defaultStartDate = new DateTime(2015, 11, 26)
			}
		},
		{
			BoardType.arabiaBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_ARABIA_BOARD01,
				boardModelName = "arabiaBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_ARABIA_BOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2016, 3, 10),
				defaultStartDate = new DateTime(2015, 3, 16)
			}
		},
		{
			BoardType.veniceBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_VENICE_BOARD01,
				boardModelName = "veniceBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_VENICE_BOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2015, 7, 2),
				defaultStartDate = new DateTime(2015, 5, 7)
			}
		},
		{
			BoardType.rioBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_RIO_BOARD01,
				boardModelName = "rioBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_RIO_BOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultStartDate = new DateTime(2015, 6, 3),
				defaultExpirationTime = new DateTime(2015, 7, 23)
			}
		},
		{
			BoardType.sydneyBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_SYDNEY_BOARD01,
				boardModelName = "sydneyBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_SYDNEY_BOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultStartDate = new DateTime(2015, 6, 24),
				defaultExpirationTime = new DateTime(2015, 8, 13)
			}
		},
		{
			BoardType.greeceBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_GREECE_BOARD01,
				boardModelName = "greeceBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_GREECE_BOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultStartDate = new DateTime(2015, 7, 15),
				defaultExpirationTime = new DateTime(2015, 9, 3)
			}
		},
		{
			BoardType.kenyaBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_KENYA_BOARD01,
				boardModelName = "kenyaBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_KENYA_BOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultStartDate = new DateTime(2015, 9, 3),
				defaultExpirationTime = new DateTime(2015, 10, 15)
			}
		},
		{
			BoardType.transylvaniaBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_TRANSYLVANIA_BOARD01,
				boardModelName = "transylvaniaBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_TRANSYLVANIA_BOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultStartDate = new DateTime(2015, 9, 23),
				defaultExpirationTime = new DateTime(2015, 11, 5)
			}
		},
		{
			BoardType.northpoleBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_NORTHPOLE_BOARD01,
				boardModelName = "northpoleBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_NORTHPOLE_BOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultStartDate = new DateTime(2015, 11, 4),
				defaultExpirationTime = new DateTime(2015, 12, 31)
			}
		},
		{
			BoardType.sanfranciscoBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_SANFRANCISCO_BOARD01,
				boardModelName = "sanfranciscoBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_SANFRANCISCO_BOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultStartDate = new DateTime(2015, 12, 11),
				defaultExpirationTime = new DateTime(2016, 2, 11)
			}
		},
		{
			BoardType.pragueBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_PRAGUE_BOARD01,
				boardModelName = "pragueBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_PRAGUE_BOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultStartDate = new DateTime(2016, 1, 22),
				defaultExpirationTime = new DateTime(2016, 3, 31)
			}
		},
		{
			BoardType.sydneyBoard02,
			new Board
			{
				name = StringID.HOVERBOARDS_SYDNEY_BOARD02,
				boardModelName = "sydneyBoard02",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_SYDNEY_BOARD02_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultStartDate = new DateTime(2016, 3, 30),
				defaultExpirationTime = new DateTime(2016, 5, 12)
			}
		},
		{
			BoardType.madagascarBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_MADAGASCAR_BOARD01,
				boardModelName = "madagascarBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_MADAGASCAR_BOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultStartDate = new DateTime(2016, 1, 22),
				defaultExpirationTime = new DateTime(2016, 4, 21)
			}
		},
		{
			BoardType.peruBoard01,
			new Board
			{
				name = StringID.HOVERBOARDS_PERU_BOARD01,
				boardModelName = "peruBoard01",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.NULL,
				limitedDescription = StringID.HOVERBOARDS_PERU_BOARD01_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultStartDate = new DateTime(2016, 1, 22),
				defaultExpirationTime = new DateTime(2016, 6, 2)
			}
		}
	};

	public static List<BoardType> boardOrder = new List<BoardType>
	{
		BoardType.snowboard,
		BoardType.bouncer,
		BoardType.chick,
		BoardType.hotrod,
		BoardType.teleboard,
		BoardType.skullfire,
		BoardType.normal,
		BoardType.lasVegasBoard01,
		BoardType.peruBoard01,
		BoardType.sydneyBoard02,
		BoardType.madagascarBoard01,
		BoardType.pragueBoard01,
		BoardType.arabiaBoard01,
		BoardType.sanfranciscoBoard01,
		BoardType.hawaiiBoard01,
		BoardType.northpoleBoard01,
		BoardType.londonBoard01,
		BoardType.kitty,
		BoardType.transylvaniaBoard01,
		BoardType.kenyaBoard01,
		BoardType.liberty,
		BoardType.greeceBoard01,
		BoardType.sydneyBoard01,
		BoardType.rioBoard01,
		BoardType.veniceBoard01,
		BoardType.losangelesBoard01,
		BoardType.londonBoard03,
		BoardType.bangkokBoard01,
		BoardType.cairoBoard01,
		BoardType.saopauloBoard01,
		BoardType.vancouverBoard01,
		BoardType.mexicoBoard01,
		BoardType.seoulBoard01,
		BoardType.mumbaiBoard01,
		BoardType.londonBoard02,
		BoardType.halloweenBoard01,
		BoardType.moscowBoard01,
		BoardType.panda,
		BoardType.dragon,
		BoardType.rose,
		BoardType.flamingo,
		BoardType.toucan,
		BoardType.soccer,
		BoardType.cherry,
		BoardType.theOutback,
		BoardType.lowrider,
		BoardType.starboard,
		BoardType.speedboard,
		BoardType.greatWhiteWakeboard,
		BoardType.gliderboard,
		BoardType.rome,
		BoardType.treehugger,
		BoardType.theoriginal,
		BoardType.surfboard,
		BoardType.miami,
		BoardType.monster
	};
}
