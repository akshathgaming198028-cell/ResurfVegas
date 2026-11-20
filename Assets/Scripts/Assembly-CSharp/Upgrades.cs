using System.Collections.Generic;
using UnityEngine;

public class Upgrades
{
	public const int SPAWNRATE_FOR_CHARACTER_LETTERS = 25;

	public const float UPGRADE_SPAWN_SPACING_METERS = 300f;

	private const int _SPAWNRATE_TO_FORCE_A_SPAWNING = 1500000000;

	public const int SPAWNRATE_FOR_LETTERS = 1500000000;

	public const int SPAWNRATE_FOR_MYSTERYPOWERUP = 23;

	public const float UPGRADE_FIRST_SPAWN_METERS = 250f;

	public const int SCORE_BOOSTER_MULTIPLIER_TIER1 = 5;

	public const int SCORE_BOOSTER_MULTIPLIER_TIER2 = 6;

	public const int SCORE_BOOSTER_MULTIPLIER_TIER3 = 7;

	public const int CHARACTER_DAY_MULTIPLIER = 5;

	public const int PLUS_MULTIPLIER_MYSTERY_POWERUP = 10;

	public static readonly Dictionary<PowerupType, Upgrade> upgrades = new Dictionary<PowerupType, Upgrade>
	{
		{
			PowerupType.hoverboard,
			new Upgrade
			{
				name = StringID.UPGRADES_HOVERBOARD,
				description = StringID.UPGRADES_HOVERBOARD_DESCRIPTION,
				mysteryBoxDescription = StringID.UPGRADES_HOVERBOARD_DESCRIPTION_MYSTERY,
				durations = new float[1] { 30f },
				pricesRaw = new int[1] { 300 },
				iconName = "icon_upgrades_hoverboard"
			}
		},
		{
			PowerupType.headstart500,
			new Upgrade
			{
				name = StringID.UPGRADES_HEADSTART,
				durations = new float[1] { 250f },
				description = StringID.UPGRADES_HEADSTART_DESCRIPTION,
				mysteryBoxDescription = StringID.UPGRADES_HEADSTART_DESCRIPTION_MYSTERY,
				pricesRaw = new int[1] { 400 },
				iconName = "icon_upgrades_headstart500"
			}
		},
		{
			PowerupType.scorebooster,
			new Upgrade
			{
				name = StringID.UPGRADES_SCOREBOOSTER,
				description = StringID.UPGRADES_SCOREBOOSTER_DESCRIPTION,
				mysteryBoxDescription = StringID.UPGRADES_SCOREBOOSTER_DESCRIPTION_MYSTERY,
				pricesRaw = new int[1] { 3000 },
				iconName = "icon_upgrades_scorebooster"
			}
		},
		{
			PowerupType.headstart2000,
			new Upgrade
			{
				name = StringID.UPGRADES_MEGAHEADSTART,
				nameTwoLines = StringID.UPGRADES_MEGAHEADSTART_TWO_LINES,
				mysteryBoxDescription = StringID.UPGRADES_MEGAHEADSTART_DESCRIPTION_MYSTERY,
				durations = new float[1] { 1000f },
				description = StringID.UPGRADES_MEGAHEADSTART_DESCRIPTION,
				pricesRaw = new int[1] { 2000 },
				iconName = "icon_upgrades_headstart2000"
			}
		},
		{
			PowerupType.mysterybox,
			new Upgrade
			{
				name = StringID.UPGRADES_MYSTERY_BOX,
				description = StringID.NULL,
				spawnProbability = 18,
				minimumMeters = 8000,
				pricesRaw = new int[1] { 500 },
				iconName = "icon_upgrades_mysteryBox"
			}
		},
		{
			PowerupType.jetpack,
			new Upgrade
			{
				name = StringID.UPGRADES_JETPACK,
				description = StringID.UPGRADES_JETPACK_DESCRIPTION,
				numberOfTiers = 7,
				durations = new float[7] { 8f, 9f, 10.5f, 12.5f, 15f, 19f, 25f },
				spawnProbability = 14,
				minimumMeters = 1000,
				pricesRaw = new int[7] { 0, 500, 1500, 3000, 10000, 30000, 60000 },
				iconName = "icon_upgrades_jetpack"
			}
		},
		{
			PowerupType.supersneakers,
			new Upgrade
			{
				name = StringID.UPGRADES_SUPERSNEAKERS,
				description = StringID.UPGRADES_SUPERSNEAKERS_DESCRIPTION,
				numberOfTiers = 7,
				spawnProbability = 26,
				durations = new float[7] { 10f, 11.5f, 13.4f, 15.8f, 19f, 24f, 30f },
				pricesRaw = new int[7] { 0, 500, 1500, 3000, 10000, 30000, 60000 },
				iconName = "icon_upgrades_superSneakers"
			}
		},
		{
			PowerupType.coinmagnet,
			new Upgrade
			{
				name = StringID.UPGRADES_COIN_MAGNET,
				description = StringID.UPGRADES_COIN_MAGNET_DESCRIPTION,
				numberOfTiers = 7,
				durations = new float[7] { 10f, 11.5f, 13.4f, 15.8f, 19f, 24f, 30f },
				spawnProbability = 29,
				coinmagnetRange = 2,
				pricesRaw = new int[7] { 0, 500, 1500, 3000, 10000, 30000, 60000 },
				iconName = "icon_upgrades_coinMagnet"
			}
		},
		{
			PowerupType.doubleMultiplier,
			new Upgrade
			{
				name = StringID.UPGRADES_DOUBLE_MULTIPLIER,
				description = StringID.UPGRADES_DOUBLE_MULTIPLIER_DESCRIPTION,
				numberOfTiers = 7,
				durations = new float[7] { 10f, 11.5f, 13.4f, 15.8f, 19f, 24f, 30f },
				spawnProbability = 29,
				pricesRaw = new int[7] { 0, 500, 1500, 3000, 10000, 30000, 60000 },
				iconName = "icon_upgrades_doubleScore"
			}
		},
		{
			PowerupType.saveMeToken,
			new Upgrade
			{
				name = StringID.UPGRADES_KEY,
				description = StringID.NULL,
				spawnProbability = 2,
				minimumMeters = 1000,
				pricesRaw = new int[1],
				iconName = "icon_key"
			}
		},
		{
			PowerupType.skipmission1,
			new Upgrade
			{
				name = StringID.UPGRADES_SKIP_MISSION_1,
				description = StringID.UPGRADES_SKIP_MISSION_1_DESCRIPTION,
				pricesRaw = new int[1] { 1500 },
				iconName = "icon_upgrades_skipMission1",
				levelPriceMultiplyer = 100
			}
		},
		{
			PowerupType.skipmission2,
			new Upgrade
			{
				name = StringID.UPGRADES_SKIP_MISSION_2,
				description = StringID.UPGRADES_SKIP_MISSION_2_DESCRIPTION,
				pricesRaw = new int[1] { 1500 },
				iconName = "icon_upgrades_skipMission2",
				levelPriceMultiplyer = 100
			}
		},
		{
			PowerupType.skipmission3,
			new Upgrade
			{
				name = StringID.UPGRADES_SKIP_MISSION_3,
				description = StringID.UPGRADES_SKIP_MISSION_3_DESCRIPTION,
				pricesRaw = new int[1] { 1500 },
				iconName = "icon_upgrades_skipMission3",
				levelPriceMultiplyer = 100
			}
		},
		{
			PowerupType.letters,
			new Upgrade
			{
				spawnProbability = 1500000000
			}
		},
		{
			PowerupType.characterLetter,
			new Upgrade
			{
				spawnProbability = 25
			}
		},
		{
			PowerupType.headstartShort,
			new Upgrade
			{
				name = StringID.UPGRADES_MEGAHEADSTART,
				nameTwoLines = StringID.UPGRADES_MEGAHEADSTART_TWO_LINES,
				mysteryBoxDescription = StringID.UPGRADES_MEGAHEADSTART_DESCRIPTION_MYSTERY,
				durations = new float[1] { 1000f },
				speed = 1000f,
				landSpeed = 140f,
				description = StringID.UPGRADES_MEGAHEADSTART_DESCRIPTION,
				pricesRaw = new int[1] { 2000 },
				iconName = "icon_upgrades_headstart2000"
			}
		},
		{
			PowerupType.headstartLong,
			new Upgrade
			{
				name = StringID.UPGRADES_MEGAHEADSTART,
				nameTwoLines = StringID.UPGRADES_MEGAHEADSTART_TWO_LINES,
				mysteryBoxDescription = StringID.UPGRADES_MEGAHEADSTART_DESCRIPTION_MYSTERY,
				durations = new float[1] { 1000f },
				speed = 1000f,
				landSpeed = 200f,
				description = StringID.UPGRADES_MEGAHEADSTART_DESCRIPTION,
				pricesRaw = new int[1] { 2000 },
				iconName = "icon_upgrades_headstart2000"
			}
		},
		{
			PowerupType.headstart,
			new Upgrade
			{
				name = StringID.UPGRADES_MEGAHEADSTART,
				nameTwoLines = StringID.UPGRADES_MEGAHEADSTART_TWO_LINES,
				mysteryBoxDescription = StringID.UPGRADES_MEGAHEADSTART_DESCRIPTION_MYSTERY,
				durations = new float[1] { 6f },
				speed = 400f,
				landSpeed = 0.01f,
				description = StringID.UPGRADES_MEGAHEADSTART_DESCRIPTION,
				pricesRaw = new int[1] { 2000 },
				iconName = "icon_upgrades_headstart2000"
			}
		},
		{
			PowerupType.turboHeadstart1,
			new Upgrade
			{
				name = StringID.UPGRADES_MEGAHEADSTART,
				nameTwoLines = StringID.UPGRADES_MEGAHEADSTART_TWO_LINES,
				mysteryBoxDescription = StringID.UPGRADES_MEGAHEADSTART_DESCRIPTION_MYSTERY,
				durations = new float[1] { 8f },
				speed = 550f,
				landSpeed = 0.5f,
				description = StringID.UPGRADES_MEGAHEADSTART_DESCRIPTION,
				pricesRaw = new int[1] { 2000 },
				iconName = "icon_upgrades_headstart2000"
			}
		},
		{
			PowerupType.turboHeadstart2,
			new Upgrade
			{
				name = StringID.UPGRADES_MEGAHEADSTART,
				nameTwoLines = StringID.UPGRADES_MEGAHEADSTART_TWO_LINES,
				mysteryBoxDescription = StringID.UPGRADES_MEGAHEADSTART_DESCRIPTION_MYSTERY,
				durations = new float[1] { 10f },
				speed = 700f,
				landSpeed = 1f,
				description = StringID.UPGRADES_MEGAHEADSTART_DESCRIPTION,
				pricesRaw = new int[1] { 2000 },
				iconName = "icon_upgrades_headstart2000"
			}
		},
		{
			PowerupType.mysteryPowerup,
			new Upgrade
			{
				name = StringID.UPGRADES_MYSTERY_POWERUP,
				numberOfTiers = 7,
				spawnProbability = 23,
				durations = new float[7] { 10f, 11.5f, 13.4f, 15.8f, 19f, 24f, 30f },
				pricesRaw = new int[7] { 0, 500, 1500, 3000, 10000, 30000, 60000 },
				iconName = "icon_upgrades_mysteryBox"
			}
		},
		{
			PowerupType.swapCharacter,
			new Upgrade
			{
				name = StringID.MYESTERY_POWERUP_SWAP_CHARACTER,
				description = StringID.MYESTERY_POWERUP_SWAP_CHARACTER_DESCRIPTION,
				weight = 10,
				durations = new float[1] { 15f },
				pricesRaw = new int[1],
				iconName = "icon_random_character"
			}
		},
		{
			PowerupType.doubleCoinsMysteryPowerup,
			new Upgrade
			{
				name = StringID.MYESTERY_POWERUP_DOUBLE_COINS,
				description = StringID.MYESTERY_POWERUP_DOUBLE_COINS_DESCRIPTION,
				weight = 16,
				durations = new float[1] { 15f },
				pricesRaw = new int[1],
				iconName = "icon_doubleCoins"
			}
		},
		{
			PowerupType.doubleJump,
			new Upgrade
			{
				name = StringID.MYESTERY_POWERUP_DOUBLE_JUMP,
				description = StringID.MYESTERY_POWERUP_DOUBLE_JUMP_DESCRIPTION,
				weight = 31,
				durations = new float[1] { 15f },
				pricesRaw = new int[1],
				iconName = "icon_double_jump"
			}
		},
		{
			PowerupType.plusMultiplier,
			new Upgrade
			{
				name = StringID.MYESTERY_POWERUP_PLUS_MULTIPLIER,
				description = StringID.MYESTERY_POWERUP_PLUS_MULTIPLIER_DESCRIPTION,
				weight = 31,
				durations = new float[1] { 15f },
				pricesRaw = new int[1],
				iconName = "icon_plus_10"
			}
		}
	};
}
