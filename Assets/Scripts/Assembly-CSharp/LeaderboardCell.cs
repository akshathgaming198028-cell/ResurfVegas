using System;

public class LeaderboardCell
{
	public LeaderboardCellType cellType;

	public Friend friend;

	public LeaderboardDummyData.LeaderboardDummy dummyFriend;

	public int rank;

	public bool isPlayerRanked;

	public bool useDarkColour;

	public bool lockList;

	public bool noFriends;

	public Action OnUpdateCell;

	public void UpdateCell()
	{
		if (OnUpdateCell != null)
		{
			OnUpdateCell();
		}
	}
}
