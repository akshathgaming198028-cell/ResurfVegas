using System.Collections.Generic;
using SYBO.Subway.Info;

public static class CharacterStickerManager
{
	public static bool shouldShowCharacterThemesStickerFLow()
	{
		return PlayerInfo.Instance.ShouldShowStickersForCharacterThemes();
	}

	public static bool isAnyCharacterWithFreeTheme()
	{
		foreach (Characters.CharacterType item in Characters.characterOrder)
		{
			List<CharacterTheme> list = CharacterThemes.TryGetCustomThemesForChar(item);
			if (list == null || list.Count <= 0)
			{
				continue;
			}
			int num = 1;
			foreach (CharacterTheme item2 in list)
			{
				if (item2.price == 0 && !PlayerInfo.Instance.IsThemeUnlockedForCharacter(item, num))
				{
					return true;
				}
				num++;
			}
		}
		return false;
	}

	public static bool hasCharacterFreeTheme(Characters.CharacterType type)
	{
		List<CharacterTheme> list = CharacterThemes.TryGetCustomThemesForChar(type);
		if (list != null && list.Count > 0)
		{
			int num = 1;
			foreach (CharacterTheme item in list)
			{
				if (item.price == 0 && !PlayerInfo.Instance.IsThemeUnlockedForCharacter(type, num))
				{
					return true;
				}
				num++;
			}
		}
		return false;
	}

	public static bool hasCharacterNewTheme(Characters.CharacterType type)
	{
		List<CharacterTheme> list = CharacterThemes.TryGetCustomThemesForChar(type);
		if (list != null && list.Count > 0)
		{
			int num = 1;
			foreach (CharacterTheme item in list)
			{
				if (item.isNewInThisUpdate && !hasThemeBeenSeenForCharacter(type, num))
				{
					return true;
				}
				num++;
			}
		}
		return false;
	}

	public static int getNumberOfNewCharactersOrThemes()
	{
		int num = 0;
		foreach (Characters.CharacterType item in Characters.characterOrder)
		{
			if (Characters.characterData[item].isNewInThisUpdate)
			{
				if (!PlayerInfo.Instance.isCharacterActive(item))
				{
					continue;
				}
				if (!PlayerInfo.Instance.HasCharacterBeenSeen(item))
				{
					num++;
				}
			}
			if (PlayerInfo.Instance.isFreshInstall)
			{
				continue;
			}
			List<CharacterTheme> list = CharacterThemes.TryGetCustomThemesForChar(item);
			if (list == null || list.Count <= 0)
			{
				continue;
			}
			int num2 = 1;
			foreach (CharacterTheme item2 in list)
			{
				if (item2.isNewInThisUpdate && !PlayerInfo.Instance.IsThemeSeenForCharacter(item, num2))
				{
					num++;
				}
				num2++;
			}
		}
		return num;
	}

	public static bool hasThemeBeenSeenForCharacter(Characters.CharacterType type, int index)
	{
		return PlayerInfo.Instance.IsThemeSeenForCharacter(type, index);
	}

	public static bool isThemeUnlockedForCharacter(Characters.CharacterType type, int index)
	{
		return PlayerInfo.Instance.IsThemeUnlockedForCharacter(type, index);
	}

	public static bool hasCharacterBeenSeen(Characters.CharacterType type)
	{
		return PlayerInfo.Instance.HasCharacterBeenSeen(type);
	}

	public static bool isCharacterUnlocked(Characters.CharacterType type)
	{
		return PlayerInfo.Instance.IsCollectionComplete(type);
	}
}
