using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HoverboardThemes
{
	public struct HoverboardTheme
	{
		public int price;

		public Hoverboards.UnlockType unlockType;

		public Color32 buttonBgColor;

		public Color32 buttonIconColor;

		public bool isNewInThisUpdate;

		public StringID themeDescription;

		public StringID unlockDescription;

		public StringID title;

		public UIOrder uiOrder;

		public Hoverboard.BoardAbility power;

		public ThemeIdentifier identifier;

		public string iconName;
	}

	public enum UIOrder
	{
		First = 0,
		Second = 1
	}

	public enum ThemeIdentifier
	{
		none = 0,
		heroboardTheme01 = 1,
		heroboardTheme02 = 2,
		starboardTheme01 = 3,
		starboardTheme02 = 4,
		greatWhiteTheme01 = 5,
		greatWhiteTheme02 = 6,
		surfboardTheme01 = 7,
		surfboardTheme02 = 8,
		monsterTheme1 = 9,
		monsterTheme2 = 10,
		snowboardTheme1 = 11,
		snowboardTheme2 = 12,
		miamiTheme1 = 13,
		miamiTheme2 = 14,
		skullfireTheme1 = 15,
		romeTheme01 = 16,
		skullfireTheme02 = 17,
		lumberjackTheme01 = 18,
		lumberjackTheme02 = 19,
		romeTheme02 = 20,
		londonBoard01Theme01 = 21,
		londonBoard01Theme02 = 22,
		chicky01Theme01 = 23,
		chicky01Theme02 = 24
	}

	public static readonly Dictionary<Hoverboards.BoardType, List<HoverboardTheme>> hoverboardCustomThemes = new Dictionary<Hoverboards.BoardType, List<HoverboardTheme>>
	{
		{
			Hoverboards.BoardType.theoriginal,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 80,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_HERO_TELEPORT_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_HERO_TELEPORT_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_HERO_TELEPORT_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_HERO_TELEPORT_TITLE,
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.heroboardTheme01,
					power = Hoverboard.BoardAbility.Teleport,
					iconName = "icon_upgrade_zap_sideways"
				},
				new HoverboardTheme
				{
					price = 110,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_HERO_SUPER_JUMP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_HERO_SUPER_JUMP_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_HERO_SUPER_JUMP_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_HERO_SUPER_JUMP_TITLE,
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.heroboardTheme02,
					power = Hoverboard.BoardAbility.Jumper,
					iconName = "icon_upgrade_super_jump"
				}
			}
		},
		{
			Hoverboards.BoardType.starboard,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 0,
					unlockType = Hoverboards.UnlockType.free,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_STARBOARD_TRAIL_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_STARBOARD_TRAIL_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_STARBOARD_TRAIL_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_STARBOARD_TRAIL_TITLE,
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.starboardTheme01,
					power = Hoverboard.BoardAbility.Vanity,
					iconName = "icon_upgrade_star_trail"
				},
				new HoverboardTheme
				{
					price = 90,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_STARBOARD_DOUBLE_JUMP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_STARBOARD_DOUBLE_JUMP_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_STARBOARD_DOUBLE_JUMP_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_STARBOARD_DOUBLE_JUMP_TITLE,
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.heroboardTheme02,
					power = Hoverboard.BoardAbility.DoubleJump,
					iconName = "icon_upgrade_double_jump"
				}
			}
		},
		{
			Hoverboards.BoardType.greatWhiteWakeboard,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 100,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_GREAT_WHITE_SPEEDUP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_GREAT_WHITE_SPEEDUP_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_GREAT_WHITE_SPEEDUP_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_GREAT_WHITE_SPEEDUP_TITLE,
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.greatWhiteTheme01,
					power = Hoverboard.BoardAbility.HighSpeed,
					iconName = "icon_upgrade_speed_up"
				},
				new HoverboardTheme
				{
					price = 150,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_GREAT_WHITE_GLIDER_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_GREAT_WHITE_GLIDER_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_GREAT_WHITE_GLIDER_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_GREAT_WHITE_GLIDER_TITLE,
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.greatWhiteTheme02,
					power = Hoverboard.BoardAbility.Glider,
					iconName = "icon_upgrade_smooth_drift"
				}
			}
		},
		{
			Hoverboards.BoardType.surfboard,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 130,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_SURF_BOARD_LOWRIDER_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_SURF_BOARD_LOWRIDER_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_SURF_BOARD_LOWRIDER_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_SURF_BOARD_LOWRIDER_TITLE,
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.surfboardTheme01,
					power = Hoverboard.BoardAbility.Lowrider,
					iconName = "icon_upgrade_stay_low"
				},
				new HoverboardTheme
				{
					price = 150,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_SURF_BOARD_GLIDER_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_SURF_BOARD_GLIDER_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_SURF_BOARD_GLIDER_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_SURF_BOARD_GLIDER_TITLE,
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.surfboardTheme02,
					power = Hoverboard.BoardAbility.Glider,
					iconName = "icon_upgrade_smooth_drift"
				}
			}
		},
		{
			Hoverboards.BoardType.monster,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 100,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_MONSTER_SPEEDUP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_MONSTER_SPEEDUP_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_MONSTER_SPEEDUP_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_MONSTER_SPEEDUP_TITLE,
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.monsterTheme1,
					power = Hoverboard.BoardAbility.HighSpeed,
					iconName = "icon_upgrade_speed_up"
				},
				new HoverboardTheme
				{
					price = 110,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_MONSTER_JUMPER_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_MONSTER_JUMPER_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_MONSTER_JUMPER_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_MONSTER_JUMPER_TITLE,
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.monsterTheme2,
					power = Hoverboard.BoardAbility.Jumper,
					iconName = "icon_upgrade_super_jump"
				}
			}
		},
		{
			Hoverboards.BoardType.snowboard,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 90,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_FREESTYLER_DOUBLE_JUMP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_FREESTYLER_DOUBLE_JUMP_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_FREESTYLER_DOUBLE_JUMP_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_FREESTYLER_DOUBLE_JUMP_TITLE,
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.snowboardTheme1,
					power = Hoverboard.BoardAbility.DoubleJump,
					iconName = "icon_upgrade_double_jump"
				},
				new HoverboardTheme
				{
					price = 110,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_FREESTYLER_SUPER_JUMP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_FREESTYLER_SUPER_JUMP_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_FREESTYLER_SUPER_JUMP_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_FREESTYLER_SUPER_JUMP_TITLE,
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.snowboardTheme2,
					power = Hoverboard.BoardAbility.Jumper,
					iconName = "icon_upgrade_super_jump"
				}
			}
		},
		{
			Hoverboards.BoardType.miami,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 60,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_MIAMI_PINK_TRAIL_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_MIAMI_PINK_TRAIL_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_MIAMI_PINK_TRAIL_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_MIAMI_PINK_TRAIL_TITLE,
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.miamiTheme1,
					power = Hoverboard.BoardAbility.Vanity,
					iconName = "icon_upgrade_pink_trail"
				},
				new HoverboardTheme
				{
					price = 150,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_MIAMI_SMOOTHDRIFT_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_MIAMI_SMOOTHDRIFT_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_MIAMI_SMOOTHDRIFT_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_MIAMI_SMOOTHDRIFT_TITLE,
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.miamiTheme2,
					power = Hoverboard.BoardAbility.Glider,
					iconName = "icon_upgrade_smooth_drift"
				}
			}
		},
		{
			Hoverboards.BoardType.skullfire,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 90,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_SKULLFIRE_DOUBLE_JUMP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_SKULLFIRE_DOUBLE_JUMP_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_SKULLFIRE_DOUBLE_JUMP_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_SKULLFIRE_DOUBLE_JUMP_TITLE,
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.skullfireTheme1,
					power = Hoverboard.BoardAbility.DoubleJump,
					iconName = "icon_upgrade_double_jump"
				},
				new HoverboardTheme
				{
					price = 100,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_SKULLFIRE_SPEEDUP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_SKULLFIRE_SPEEDUP_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_SKULLFIRE_SPEEDUP_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_SKULLFIRE_SPEEDUP_TITLE,
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.skullfireTheme02,
					power = Hoverboard.BoardAbility.HighSpeed,
					iconName = "icon_upgrade_speed_up"
				}
			}
		},
		{
			Hoverboards.BoardType.rome,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 80,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_ROME_ZAPSIDEWAYS_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_ROME_ZAPSIDEWAYS_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_ROME_ZAPSIDEWAYS_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_ROME_ZAPSIDEWAYS_TITLE,
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.romeTheme01,
					power = Hoverboard.BoardAbility.Teleport,
					iconName = "icon_upgrade_zap_sideways"
				},
				new HoverboardTheme
				{
					price = 100,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_ROME_SPEEDUP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_ROME_SPEEDUP_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_ROME_SPEEDUP_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_ROME_SPEEDUP_TITLE,
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.romeTheme02,
					power = Hoverboard.BoardAbility.HighSpeed,
					iconName = "icon_upgrade_speed_up"
				}
			}
		},
		{
			Hoverboards.BoardType.treehugger,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 90,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_LUMBERJACK_DOUBLE_JUMP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_LUMBERJACK_DOUBLE_JUMP_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_LUMBERJACK_DOUBLE_JUMP_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_LUMBERJACK_DOUBLE_JUMP_TITLE,
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.lumberjackTheme01,
					power = Hoverboard.BoardAbility.DoubleJump,
					iconName = "icon_upgrade_double_jump"
				},
				new HoverboardTheme
				{
					price = 130,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_LUMBERJACK_LOWRIDER_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_LUMBERJACK_LOWRIDER_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_LUMBERJACK_LOWRIDER_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_LUMBERJACK_LOWRIDER_TITLE,
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.lumberjackTheme02,
					power = Hoverboard.BoardAbility.Lowrider,
					iconName = "icon_upgrade_stay_low"
				}
			}
		},
		{
			Hoverboards.BoardType.londonBoard01,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 0,
					unlockType = Hoverboards.UnlockType.free,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_LONDONBOARD01_TRAIL_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_LONDONBOARD01_TRAIL_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_LONDONBOARD01_TRAIL_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_LONDONBOARD01_TRAIL_TITLE,
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.londonBoard01Theme01,
					power = Hoverboard.BoardAbility.Vanity,
					iconName = "icon_upgrade_snow_trail"
				},
				new HoverboardTheme
				{
					price = 110,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_LONDONBOARD01_SUPER_JUMP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_LONDONBOARD01_SUPER_JUMP_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_LONDONBOARD01_SUPER_JUMP_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_LONDONBOARD01_SUPER_JUMP_TITLE,
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.londonBoard01Theme02,
					power = Hoverboard.BoardAbility.Jumper,
					iconName = "icon_upgrade_super_jump"
				}
			}
		},
		{
			Hoverboards.BoardType.chick,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 0,
					unlockType = Hoverboards.UnlockType.free,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_CHICK_TRAIL_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_CHICK_TRAIL_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_CHICKY_TRAIL_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_CHICKY_TRAIL_TITLE,
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.chicky01Theme01,
					power = Hoverboard.BoardAbility.Vanity,
					iconName = "icon_feather"
				},
				new HoverboardTheme
				{
					price = 110,
					unlockType = Hoverboards.UnlockType.keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_CHICK_SPEEDUP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_CHICK_SPEEDUP_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HOVERBOARD_THEME_CHICKY_SPEED_DESCRIPTION,
					title = StringID.HOVERBOARD_THEME_CHICKY_SPEED_TITLE,
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.chicky01Theme02,
					power = Hoverboard.BoardAbility.HighSpeed,
					iconName = "icon_upgrade_speed_up"
				}
			}
		}
	};

	public static string GetBoardPowerName(Hoverboards.BoardType board, Hoverboard.BoardAbility power)
	{
		if (hoverboardCustomThemes.ContainsKey(board))
		{
			foreach (HoverboardTheme item in hoverboardCustomThemes[board])
			{
				if (item.power == power)
				{
					return Strings.Get(item.title);
				}
			}
			Debug.LogError("No board theme of type " + board.ToString() + " not found. Returning empty string.");
			return string.Empty;
		}
		Debug.LogError("No board themes exist for board type: " + board.ToString() + ". Returning empty string.");
		return string.Empty;
	}

	public static ThemeIdentifier GetThemeIdentifier(Hoverboards.BoardType bType, int tIndex)
	{
		if (tIndex < 0)
		{
			Debug.LogWarning("Theme index is smaller than 0");
			return ThemeIdentifier.none;
		}
		if (hoverboardCustomThemes.ContainsKey(bType))
		{
			foreach (HoverboardTheme item in hoverboardCustomThemes[bType])
			{
				if (tIndex == (int)item.uiOrder)
				{
					return item.identifier;
				}
			}
			return ThemeIdentifier.none;
		}
		return ThemeIdentifier.none;
	}

	public static List<HoverboardTheme> TryGetCustomThemesForBoard(Hoverboards.BoardType boardType)
	{
		List<HoverboardTheme> value;
		hoverboardCustomThemes.TryGetValue(boardType, out value);
		return value;
	}

	public static void VerifyData()
	{
		foreach (KeyValuePair<Hoverboards.BoardType, List<HoverboardTheme>> hoverboardCustomTheme in hoverboardCustomThemes)
		{
			ValidateUIOrder(hoverboardCustomTheme);
		}
	}

	private static void ValidateUIOrder(KeyValuePair<Hoverboards.BoardType, List<HoverboardTheme>> themeSet)
	{
		List<UIOrder> list = Enum.GetValues(typeof(UIOrder)).Cast<UIOrder>().ToList();
		foreach (HoverboardTheme item in themeSet.Value)
		{
			if (list.Contains(item.uiOrder))
			{
				list.Remove(item.uiOrder);
			}
			else
			{
				Debug.LogError(string.Concat("UIOrder is repeated in themes for board: ", themeSet.Key, ". Please make sure no UIOrder enum member is repeated within the same theme set for a specific board."));
			}
		}
	}
}
