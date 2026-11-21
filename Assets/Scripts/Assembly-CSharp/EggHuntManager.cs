using System;

public static class EggHuntManager
{
	private static readonly Hoverboards.BoardType HOVERBOARDTYPE = Hoverboards.BoardType.chick;

	public static event Action EggCountUpdated;

	public static bool ShouldSpawnEggs()
	{
		return true;
	}

	public static void EggPickedUp()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.amountOfSeasonalEggs++;
		if (CanUnlockEgg())
		{
			Hoverboards.Board board = Hoverboards.boardData[HOVERBOARDTYPE];
			instance.UnlockHoverboard(HOVERBOARDTYPE);
			Flurry.LogEventWithAParameter("Hoverboard bought", "Id", char.ToUpper(Strings.Get(board.name)[0]) + Strings.Get(board.name).Substring(1));
		}
		if (EggHuntManager.EggCountUpdated != null)
		{
			EggHuntManager.EggCountUpdated();
		}
	}

	public static bool TryToUnlockEggBoard()
	{
		if (CanUnlockEgg())
		{
			PlayerInfo.Instance.UnlockHoverboard(HOVERBOARDTYPE);
			return true;
		}
		return false;
	}

	private static bool CanUnlockEgg()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		return instance.amountOfSeasonalEggs >= GetMaximumNumberOfEggs() && !instance.isHoverboardUnlocked(HOVERBOARDTYPE);
	}

	public static bool HasEggHuntExpired()
	{
		return false;
	}

	public static int GetMaximumNumberOfEggs()
	{
		return Hoverboards.boardData[HOVERBOARDTYPE].price;
	}

	public static int GetCurrentNumberOfEggs()
	{
		return PlayerInfo.Instance.amountOfSeasonalEggs;
	}
}
