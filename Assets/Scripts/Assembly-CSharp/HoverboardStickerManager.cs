using System.Collections.Generic;

public static class HoverboardStickerManager
{
	public static bool shouldShowHoverboardStickerFLow()
	{
		return PlayerInfo.Instance.ShouldShowStickersForHoverboars();
	}

	public static bool IsAnyGiftBoard()
	{
		bool flag = false;
		List<Hoverboards.BoardType> list = new List<Hoverboards.BoardType>();
		foreach (Hoverboards.BoardType item in Hoverboards.boardOrder)
		{
			if (Hoverboards.boardData[item].unlockType == Hoverboards.UnlockType.free)
			{
				list.Add(item);
			}
		}
		Hoverboards.BoardType[] array = list.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			if (!PlayerInfo.Instance.isHoverboardUnlocked(array[i]))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
		}
		return flag;
	}

	public static int getNumberOfNewHoverboards()
	{
		int num = 0;
		foreach (Hoverboards.BoardType item in Hoverboards.boardOrder)
		{
			if (Hoverboards.boardData[item].isNewInThisUpdate && PlayerInfo.Instance.isBoardActive(item) && !PlayerInfo.Instance.HasHoverboardBeenSeen(item))
			{
				num++;
			}
		}
		return num;
	}

	public static bool hasHoverboardBeenSeen(Hoverboards.BoardType type)
	{
		return PlayerInfo.Instance.HasHoverboardBeenSeen(type);
	}

	public static bool isHoverboardUnlocked(Hoverboards.BoardType type)
	{
		return PlayerInfo.Instance.isHoverboardUnlocked(type);
	}
}
