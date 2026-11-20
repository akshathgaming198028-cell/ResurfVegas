using System.Collections.Generic;
using UnityEngine;

public class OldTrophies
{
	public enum Trophy
	{
		diamond = 0,
		goldbar = 1,
		goldChainClock = 2,
		goldChainDollar = 3,
		goldSkull = 4,
		headphones = 5,
		lpBlack = 6,
		tapeBlack = 7
	}

	public const int NUMBER_OF_TROPHIES_FOR_FIRST_ACHIEVEMENT = 8;

	public static readonly Dictionary<Trophy, TrophyData> trophyData = new Dictionary<Trophy, TrophyData>
	{
		{
			Trophy.diamond,
			new TrophyData
			{
				name = StringID.TROPHY_GEMSTONE,
				description = StringID.TROPHY_GEMSTONE_DESCRIPTION,
				spriteUnlocked = "trophy_diamond",
				obtainSource = MysteryBox.Type.Super
			}
		},
		{
			Trophy.goldbar,
			new TrophyData
			{
				name = StringID.TROPHY_GOLD_BAR,
				description = StringID.TROPHY_GOLD_BAR_DESCRIPTION,
				spriteUnlocked = "trophy_gold",
				obtainSource = MysteryBox.Type.Super
			}
		},
		{
			Trophy.goldChainClock,
			new TrophyData
			{
				name = StringID.TROPHY_CLASSY_CLOCK,
				description = StringID.TROPHY_CLASSY_CLOCK_DESCRIPTION,
				spriteUnlocked = "trophy_goldChainClock",
				obtainSource = MysteryBox.Type.Normal
			}
		},
		{
			Trophy.goldChainDollar,
			new TrophyData
			{
				name = StringID.TROPHY_DOLLAR_CHAIN,
				description = StringID.TROPHY_DOLLAR_CHAIN_DESCRIPTION,
				spriteUnlocked = "trophy_goldChainDollar",
				obtainSource = MysteryBox.Type.Super
			}
		},
		{
			Trophy.goldSkull,
			new TrophyData
			{
				name = StringID.TROPHY_PAPERWEIGHT,
				description = StringID.TROPHY_PAPERWEIGHT_DESCRIPTION,
				spriteUnlocked = "trophy_goldSkull",
				obtainSource = MysteryBox.Type.Super
			}
		},
		{
			Trophy.headphones,
			new TrophyData
			{
				name = StringID.TROPHY_HEADPHONES,
				description = StringID.TROPHY_HEADPHONES_DESCRIPTION,
				spriteUnlocked = "trophy_headphones",
				obtainSource = MysteryBox.Type.Normal
			}
		},
		{
			Trophy.lpBlack,
			new TrophyData
			{
				name = StringID.TROPHY_PLATINUM_RECORD,
				description = StringID.TROPHY_PLATINUM_RECORD_DESCRIPTION,
				spriteUnlocked = "trophy_lpBlack",
				obtainSource = MysteryBox.Type.Normal
			}
		},
		{
			Trophy.tapeBlack,
			new TrophyData
			{
				name = StringID.TROPHY_CASSETTE,
				description = StringID.TROPHY_CASSETTE_DESCRIPTION,
				spriteUnlocked = "trophy_tapeBlack",
				obtainSource = MysteryBox.Type.Normal
			}
		}
	};
}
