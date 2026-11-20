using System.Collections.Generic;
using SYBO.Subway.Info;
using UnityEngine;

public class CharacterThemes
{
	public static readonly Dictionary<Characters.CharacterType, List<CharacterTheme>> characterCustomThemes = new Dictionary<Characters.CharacterType, List<CharacterTheme>>
	{
		{
			Characters.CharacterType.tricky,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 25,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_TRICKY_ARMY_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_TRICKY_ARMY_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.TRICKY_ARMY_THEME_UNLOCK_DESCRIPTION,
					title = StringID.TRICKY_ARMY_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 40,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_TRICKY_COOL_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_TRICKY_COOL_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.TRICKY_COOL_THEME_UNLOCK_DESCRIPTION,
					title = StringID.TRICKY_COOL_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.slick,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 0,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_JAKE_DARK_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_JAKE_DARK_SKIN_ICON,
					isNewInThisUpdate = false,
					title = StringID.JAKE_DARK_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 30,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_JAKE_KID_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_JAKE_KID_SKIN_ICON,
					isNewInThisUpdate = false,
					title = StringID.JAKE_KID_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.tag,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_TAGBOT_R2D2_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_TAGBOT_R2D2_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.TAGBOT_THEME_UNLOCK_DESCRIPTION,
					title = StringID.TAGBOT_R2D2_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 30,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_TAGBOT_RED_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_TAGBOT_RED_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.TAGBOT_THEME_UNLOCK_DESCRIPTION,
					title = StringID.TAGBOT_RED_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.spike,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 25,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_SPIKE_DARK_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_SPIKE_DARK_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.SPIKE_THEME_UNLOCK_DESCRIPTION,
					title = StringID.SPIKE_DARK_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 40,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_SPIKE_STAR_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_SPIKE_STAR_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.SPIKE_THEME_UNLOCK_DESCRIPTION,
					title = StringID.SPIKE_STAR_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.fresh,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 40,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_FRESH_BASKET_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_FRESH_BASKET_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.FRESH_THEME_UNLOCK_DESCRIPTION,
					title = StringID.FRESH_BASKET_THEME_TITLE,
					uiPriority = 1
				},
				new CharacterTheme
				{
					price = 25,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_FRESH_SMOOTH_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_FRESH_SMOOTH_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.FRESH_THEME_UNLOCK_DESCRIPTION,
					title = StringID.FRESH_SMOOTH_THEME_TITLE,
					uiPriority = 0
				}
			}
		},
		{
			Characters.CharacterType.lucy,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_LUCY_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_LUCY_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.LUCY_THEME_UNLOCK_DESCRIPTION,
					title = StringID.LUCY_THEME1_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 30,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_LUCY_THEME2_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_LUCY_THEME2_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.LUCY_THEME_UNLOCK_DESCRIPTION,
					title = StringID.LUCY_THEME2_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.king,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_KING_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_KING_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.KING_THEME_UNLOCK_DESCRIPTION,
					title = StringID.KING_THEME1_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 30,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_KING_THEME2_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_KING_THEME2_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.KING_THEME_UNLOCK_DESCRIPTION,
					title = StringID.KING_THEME2_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.frank,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_FRANK_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_FRANK_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.FRANK_THEME_UNLOCK_DESCRIPTION,
					title = StringID.FRANK_THEME1_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 30,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_FRANK_THEME2_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_FRANK_THEME2_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.FRANK_THEME_UNLOCK_DESCRIPTION,
					title = StringID.FRANK_THEME2_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.frizzy,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_FRIZZY_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_FRIZZY_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.FRIZZY_THEME_UNLOCK_DESCRIPTION,
					title = StringID.FRIZZY_THEME1_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 30,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_FRIZZY_THEME2_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_FRIZZY_THEME2_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.FRIZZY_THEME_UNLOCK_DESCRIPTION,
					title = StringID.FRIZZY_THEME2_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.tasha,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_TASHA_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_TASHA_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.TASHA_THEME_UNLOCK_DESCRIPTION,
					title = StringID.TASHA_THEME1_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 30,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_TASHA_THEME2_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_TASHA_THEME2_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.TASHA_THEME_UNLOCK_DESCRIPTION,
					title = StringID.TASHA_THEME2_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.nick,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_NICK_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_NICK_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.NICK_THEME_UNLOCK_DESCRIPTION,
					title = StringID.NICK_THEME1_THEME_TITLE,
					uiPriority = 0
				}
			}
		},
		{
			Characters.CharacterType.seoulCharacter01,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_MINA_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_MINA_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.MINA_THEME_UNLOCK_DESCRIPTION,
					title = StringID.MINA_THEME1_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 30,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_MINA_THEME2_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_MINA_THEME2_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.MINA_THEME_UNLOCK_DESCRIPTION,
					title = StringID.MINA_THEME2_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.brody,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 35,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_BRODY_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_BRODY_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.BRODY_THEME_UNLOCK_DESCRIPTION,
					title = StringID.BRODY_THEME1_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 70,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_BRODY_THEME2_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_BRODY_THEME2_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.BRODY_THEME_UNLOCK_DESCRIPTION,
					title = StringID.BRODY_THEME2_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.tony,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_TONY_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_TONY_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.TONY_THEME_UNLOCK_DESCRIPTION,
					title = StringID.TONY_THEME1_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 30,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_TONY_THEME2_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_TONY_THEME2_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.TONY_THEME_UNLOCK_DESCRIPTION,
					title = StringID.TONY_THEME2_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.mexicoCharacter01,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_ROSA_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_ROSA_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.MEXICO_THEME_UNLOCK_DESCRIPTION,
					title = StringID.MEXICO_THEME1_THEME_TITLE,
					uiPriority = 0
				}
			}
		},
		{
			Characters.CharacterType.roberto,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_ROBERTO_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_ROBERTO_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.ROME_THEME_UNLOCK_DESCRIPTION,
					title = StringID.ROME_THEME1_THEME_TITLE,
					uiPriority = 0
				}
			}
		},
		{
			Characters.CharacterType.zoe,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_ZOE_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_ZOE_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.ZOE_THEME_UNLOCK_DESCRIPTION,
					title = StringID.ZOE_THEME1_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 30,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_ZOE_THEME2_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_ZOE_THEME2_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.ZOE_THEME_UNLOCK_DESCRIPTION,
					title = StringID.ZOE_THEME2_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.vancouverCharacter01,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_VANCOUVER1_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_VANCOUVER1_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.VANCOUVER1_THEME_UNLOCK_DESCRIPTION,
					title = StringID.VANCOUVER1_THEME1_THEME_TITLE,
					uiPriority = 0
				}
			}
		},
		{
			Characters.CharacterType.princek,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 100,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_PRINCEK_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_PRINCEK_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.PRINCEK_THEME_UNLOCK_DESCRIPTION,
					title = StringID.PRINCEK_THEME1_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 280,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_PRINCEK_THEME2_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_PRINCEK_THEME2_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.PRINCEK_THEME_UNLOCK_DESCRIPTION,
					title = StringID.PRINCEK_THEME2_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.harumi,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_HARUMI_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_HARUMI_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HARUMI_THEME_UNLOCK_DESCRIPTION,
					title = StringID.HARUMI_THEME1_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 30,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_HARUMI_THEME2_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_HARUMI_THEME2_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HARUMI_THEME2_UNLOCK_DESCRIPTION,
					title = StringID.HARUMI_THEME2_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.ninja,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_NINJA_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_NINJA_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.NINJA_THEME_UNLOCK_DESCRIPTION,
					title = StringID.NINJA_THEME1_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 30,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_NINJA_THEME2_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_NINJA_THEME2_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.NINJA_THEME_UNLOCK_DESCRIPTION,
					title = StringID.NINJA_THEME2_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.saopauloCharacter01,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_SAOPAULO_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_SAOPAULO_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.SAOPAULO1_THEME_UNLOCK_DESCRIPTION,
					title = StringID.SAOPAULO1_THEME1_THEME_TITLE,
					uiPriority = 0
				}
			}
		},
		{
			Characters.CharacterType.coco,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_COCO_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_COCO_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.COCO_THEME_UNLOCK_DESCRIPTION,
					title = StringID.COCO_THEME1_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 30,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_COCO_THEME2_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_COCO_THEME2_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.COCO_THEME2_UNLOCK_DESCRIPTION,
					title = StringID.COCO_THEME2_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.losangelesCharacter01,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_LOSANGELES_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_LOSANGELES_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.LOSANGELES1_THEME_UNLOCK_DESCRIPTION,
					title = StringID.LOSANGELES_THEME1_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 30,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_LOSANGELES_THEME2_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_LOSANGELES_THEME2_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.LOSANGELES_THEME2_UNLOCK_DESCRIPTION,
					title = StringID.LOSANGELES_THEME2_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.monkeyking,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_SUN_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_SUN_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.SUN_THEME_UNLOCK_DESCRIPTION,
					title = StringID.SUN_THEME1_THEME_TITLE,
					uiPriority = 0
				}
			}
		},
		{
			Characters.CharacterType.cairoCharacter01,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_CAIRO_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_CAIRO_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.CAIRO1_THEME_UNLOCK_DESCRIPTION,
					title = StringID.CAIRO_THEME1_THEME_TITLE,
					uiPriority = 0
				}
			}
		},
		{
			Characters.CharacterType.halloweenCharacter01,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_NEW_ORLEANS_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_NEW_ORLEANS_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.NEW_ORLEANS_THEME_UNLOCK_DESCRIPTION,
					title = StringID.NEW_ORLEANS_THEME1_THEME_TITLE,
					uiPriority = 0
				}
			}
		},
		{
			Characters.CharacterType.bangkokCharacter01,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_BANGKOK_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_BANGKOK_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.BANGKOK_THEME_UNLOCK_DESCRIPTION,
					title = StringID.BANGKOK_THEME1_THEME_TITLE,
					uiPriority = 0
				}
			}
		},
		{
			Characters.CharacterType.londonCharacter02,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_LONDON_CHARACTER_02_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_LONDON_CHARACTER_02_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.LONDON_CHARACTER_02_THEME_UNLOCK_DESCRIPTION,
					title = StringID.LONDON_CHARACTER_02_THEME1_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 30,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_LONDON_CHARACTER_02_THEME2_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_LONDON_CHARACTER_02_THEME2_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.LONDON_CHARACTER_02_THEME2_UNLOCK_DESCRIPTION,
					title = StringID.LONDON_CHARACTER_02_THEME2_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.lasVegasCharacter01,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_LAS_VEGAS_CHARACTER_01_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_LAS_VEGAS_CHARACTER_01_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.LAS_VEGAS_CHARACTER_01_THEME_UNLOCK_DESCRIPTION,
					title = StringID.LAS_VEGAS_CHARACTER_01_THEME1_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 30,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_LAS_VEGAS_CHARACTER_01_THEME2_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_LAS_VEGAS_CHARACTER_01_THEME2_SKIN_ICON,
					isNewInThisUpdate = true,
					unlockDescription = StringID.LAS_VEGAS_CHARACTER_01_THEME_UNLOCK_DESCRIPTION,
					title = StringID.LAS_VEGAS_CHARACTER_01_THEME2_THEME_TITLE,
					uiPriority = 0
				}
			}
		},
		{
			Characters.CharacterType.hawaiiCharacter01,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_HAWAII_CHARACTER_01_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_HAWAII_CHARACTER_01_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HAWAII_CHARACTER_01_THEME_UNLOCK_DESCRIPTION,
					title = StringID.HAWAII_CHARACTER_01_THEME1_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 30,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_HAWAII_CHARACTER_01_THEME2_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_HAWAII_CHARACTER_01_THEME2_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.HAWAII_CHARACTER_01_THEME2_UNLOCK_DESCRIPTION,
					title = StringID.HAWAII_CHARACTER_01_THEME2_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.mumbaiCharacter01,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_MUMBAI_CHARACTER_01_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_MUMBAI_CHARACTER_01_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.MUMBAI_CHARACTER_01_THEME_UNLOCK_DESCRIPTION,
					title = StringID.MUMBAI_CHARACTER_01_THEME1_THEME_TITLE,
					uiPriority = 0
				}
			}
		},
		{
			Characters.CharacterType.arabiaCharacter01,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_ARABIA_CHARACTER_01_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_ARABIA_CHARACTER_01_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.ARABIA_CHARACTER_01_THEME_UNLOCK_DESCRIPTION,
					title = StringID.ARABIA_CHARACTER_01_THEME1_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 30,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_ARABIA_CHARACTER_01_THEME2_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_ARABIA_CHARACTER_01_THEME2_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.ARABIA_CHARACTER_01_THEME_UNLOCK_DESCRIPTION,
					title = StringID.ARABIA_CHARACTER_01_THEME2_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.veniceCharacter01,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_VENICE_CHARACTER_01_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_VENICE_CHARACTER_01_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.VENICE_CHARACTER_01_THEME_UNLOCK_DESCRIPTION,
					title = StringID.VENICE_CHARACTER_01_THEME1_THEME_TITLE,
					uiPriority = 0
				}
			}
		},
		{
			Characters.CharacterType.carmen,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_CARMEN_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_CARMEN_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.CARMEN_THEME1_THEME_UNLOCK_DESCRIPTION,
					title = StringID.CARMEN_THEME1_THEME_TITLE,
					uiPriority = 0
				}
			}
		},
		{
			Characters.CharacterType.kim,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_KIM_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_KIM_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.KIM_THEME1_THEME_UNLOCK_DESCRIPTION,
					title = StringID.KIM_THEME1_THEME_TITLE,
					uiPriority = 0
				},
				new CharacterTheme
				{
					price = 30,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_KIM_THEME2_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_KIM_THEME2_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.KIM_THEME2_THEME_UNLOCK_DESCRIPTION,
					title = StringID.KIM_THEME2_THEME_TITLE,
					uiPriority = 1
				}
			}
		},
		{
			Characters.CharacterType.greeceCharacter01,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_GREECE_CHARACTER_01_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_GREECE_CHARACTER_01_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.GREECE_CHARACTER_01_THEME_UNLOCK_DESCRIPTION,
					title = StringID.GREECE_CHARACTER_01_THEME1_THEME_TITLE,
					uiPriority = 0
				}
			}
		},
		{
			Characters.CharacterType.kenyaCharacter01,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_KENYA_CHARACTER_01_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_KENYA_CHARACTER_01_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.KENYA_CHARACTER_01_THEME_UNLOCK_DESCRIPTION,
					title = StringID.KENYA_CHARACTER_01_THEME1_THEME_TITLE,
					uiPriority = 0
				}
			}
		},
		{
			Characters.CharacterType.transylvaniaCharacter01,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_TRANSYLVANIA_CHARACTER_01_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_TRANSYLVANIA_CHARACTER_01_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.TRANSYLVANIA_CHARACTER_01_THEME_UNLOCK_DESCRIPTION,
					title = StringID.TRANSYLVANIA_CHARACTER_01_THEME1_THEME_TITLE,
					uiPriority = 0
				}
			}
		},
		{
			Characters.CharacterType.sanfranciscoCharacter01,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_SANFRANCISCO_CHARACTER_01_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_SANFRANCISCO_CHARACTER_01_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.SANFRANCISCO_CHARACTER_01_THEME_UNLOCK_DESCRIPTION,
					title = StringID.SANFRANCISCO_CHARACTER_01_THEME1_THEME_TITLE,
					uiPriority = 0
				}
			}
		},
		{
			Characters.CharacterType.pragueCharacter01,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_PRAGUE_CHARACTER_01_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_PRAGUE_CHARACTER_01_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.PRAGUE_CHARACTER_01_THEME_UNLOCK_DESCRIPTION,
					title = StringID.PRAGUE_CHARACTER_01_THEME1_THEME_TITLE,
					uiPriority = 0
				}
			}
		},
		{
			Characters.CharacterType.madagascarCharacter01,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_MADAGASCAR_CHARACTER_01_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_MADAGASCAR_CHARACTER_01_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.MADAGASCAR_CHARACTER_01_THEME_UNLOCK_DESCRIPTION,
					title = StringID.MADAGASCAR_CHARACTER_01_THEME1_THEME_TITLE,
					uiPriority = 0
				}
			}
		},
		{
			Characters.CharacterType.peruCharacter01,
			new List<CharacterTheme>
			{
				new CharacterTheme
				{
					price = 15,
					unlockType = Characters.UnlockType.keys,
					buttonBgColor = GlobalColors.CHARACTERS_CUSTOMIZATION_PERU_CHARACTER_01_THEME1_SKIN_FILL,
					buttonIconColor = GlobalColors.CHARACTERS_CUSTOMIZATION_PERU_CHARACTER_01_THEME1_SKIN_ICON,
					isNewInThisUpdate = false,
					unlockDescription = StringID.PERU_CHARACTER_01_THEME_UNLOCK_DESCRIPTION,
					title = StringID.PERU_CHARACTER_01_THEME1_THEME_TITLE,
					uiPriority = 0
				}
			}
		}
	};

	public static CharacterTheme? GetThemeForCharacter(Characters.CharacterType charType, int index)
	{
		if (index == 0)
		{
			return null;
		}
		List<CharacterTheme> list = TryGetCustomThemesForChar(charType);
		if (list != null && list.Count >= index)
		{
			return list[index - 1];
		}
		return null;
	}

	public static List<CharacterTheme> TryGetCustomThemesForChar(Characters.CharacterType charType)
	{
		List<CharacterTheme> value;
		characterCustomThemes.TryGetValue(charType, out value);
		if (value == null)
		{
		}
		return value;
	}
}
