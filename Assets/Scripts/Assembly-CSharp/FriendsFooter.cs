using UnityEngine;

public class FriendsFooter : MonoBehaviour
{
	[SerializeField]
	private UILabel menuLabel;

	[SerializeField]
	private UILabel friendsLabel;

	[SerializeField]
	private UILabel leaderboardLabel;

	[SerializeField]
	private UILabel toprunLabel;

	private void Awake()
	{
		menuLabel.text = Strings.Get(StringID.FRIENDS_FOOTER_MENU);
		friendsLabel.text = Strings.Get(StringID.FRIENDS_FOOTER_COLLECT);
		leaderboardLabel.text = Strings.Get(StringID.FRIENDS_FOOTER_LEADERBOARD);
		toprunLabel.text = Strings.Get(StringID.FRIENDS_FOOTER_TOPRUN);
	}
}
