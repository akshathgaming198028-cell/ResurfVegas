using System.Collections.Generic;
using SYBO.Subway;
using UnityEngine;

public class MysteryPowerup
{
	private const float randomBoardDuration = 15f;

	private const int randomBoardWeight = 22;

	private const string randomBoardIconName = "icon_random_board";

	private static readonly Dictionary<PowerupType, CharacterModifier> mysteryPowerupCharacterModifiers = new Dictionary<PowerupType, CharacterModifier>
	{
		{
			PowerupType.hoverboard,
			Game.Instance.modifiers.Hoverboard
		},
		{
			PowerupType.doubleCoinsMysteryPowerup,
			Game.Instance.modifiers.DoubleCoins
		},
		{
			PowerupType.doubleJump,
			Game.Instance.modifiers.DoubleJump
		},
		{
			PowerupType.plusMultiplier,
			Game.Instance.modifiers.PlusMultiplier
		}
	};

	private static readonly List<Hoverboards.BoardType> excludedBoards = new List<Hoverboards.BoardType>();

	private static CharacterModifier SelectRandomCharacterModifier()
	{
		CharacterModifier result = Game.Instance.modifiers.DoubleCoins;
		List<Upgrade> list = new List<Upgrade>();
		List<PowerupType> list2 = new List<PowerupType>();
		int num = 0;
		foreach (PowerupType key in mysteryPowerupCharacterModifiers.Keys)
		{
			Upgrade item = Upgrades.upgrades[key];
			if (key == PowerupType.hoverboard)
			{
				if (Hoverboard.Instance.isActive || Hoverboard.Instance.IsInCooldown)
				{
					continue;
				}
				num += 22;
			}
			else
			{
				num += item.weight;
			}
			list.Add(item);
			list2.Add(key);
		}
		int num2 = Random.Range(0, num);
		float num3 = 0f;
		for (int i = 0; i < list.Count; i++)
		{
			Upgrade upgrade = list[i];
			num3 = ((list2[i] != PowerupType.hoverboard) ? (num3 + (float)upgrade.weight) : (num3 + 22f));
			if ((float)num2 <= num3)
			{
				result = mysteryPowerupCharacterModifiers[list2[i]];
				break;
			}
		}
		return result;
	}

	public static string GetRandomBoardIconName()
	{
		return "icon_random_board";
	}

	public static CharacterModifier GetRandomModifier()
	{
		CharacterModifier characterModifier = null;
		characterModifier = ((!Globals.DEBUG_IS_HOVERBOARD_ALWAYS_REWARDED_FOR_MYSERY_POWERUP) ? SelectRandomCharacterModifier() : Game.Instance.modifiers.Hoverboard);
		if (characterModifier == Game.Instance.modifiers.Hoverboard)
		{
			Hoverboard.Instance.IsARewardFromMysteryPowerup = true;
		}
		return characterModifier;
	}

	public static List<Hoverboards.BoardType> GetRandomBoardList()
	{
		List<Hoverboards.BoardType> list = new List<Hoverboards.BoardType>();
		foreach (Hoverboards.BoardType item in Hoverboards.boardOrder)
		{
			if ((PlayerInfo.Instance.isHoverboardUnlocked(item) || PlayerInfo.Instance.isBoardActive(item)) && !excludedBoards.Contains(item) && PlayerInfo.Instance.currentHoverboard != item)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static float GetRandomHoverboardDuration()
	{
		return 15f;
	}
}
