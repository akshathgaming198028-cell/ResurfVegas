using UnityEngine;

public class FriendsLoginFBButton : MonoBehaviour
{
	[SerializeField]
	private GameObject _connectingContent;

	[SerializeField]
	private GameObject _normalContentFriends;

	[SerializeField]
	private GameObject _normalContentLeaderboard;

	[SerializeField]
	private GameObject _overlay;

	[SerializeField]
	private UILabel _leaderboardLabel1;

	[SerializeField]
	private UILabel _leaderboardLabel2;

	[SerializeField]
	private Collider _leaderboardCollider;

	[SerializeField]
	private Collider _friendsCollider;

	[SerializeField]
	private UISprite _connectingFill;

	private GameObject _FBClickedReceiver;

	private string _eventMethodName;

	public void Init(GameObject receiver, string methodName, bool leaderboardScreen = false, bool noFriends = false)
	{
		_FBClickedReceiver = receiver;
		_eventMethodName = methodName;
		_connectingFill.color = GlobalColors.SOCIAL_GREY;
		SetToLogInState(leaderboardScreen, noFriends);
	}

	private void OnClick()
	{
		_FBClickedReceiver.SendMessage(_eventMethodName);
		SetToConnectingState();
	}

	private void SetToLogInState(bool leaderboardScreen, bool noFriends)
	{
		_overlay.SetActive(true);
		_connectingContent.SetActive(false);
		_normalContentFriends.SetActive(!leaderboardScreen);
		_normalContentLeaderboard.SetActive(leaderboardScreen);
		UIAnchor component = _leaderboardLabel1.GetComponent<UIAnchor>();
		if (noFriends)
		{
			_leaderboardLabel2.text = Strings.Get(StringID.FACEBOOK_BUTTON_LEADERBOARD_TEXT_NO_FRIENDS);
			component.pixelOffset = new Vector2(30f, 0f);
		}
		else
		{
			_leaderboardLabel2.text = Strings.Get(StringID.FACEBOOK_BUTTON_LEADERBOARD_TEXT);
			component.pixelOffset = new Vector2(12f, 0f);
		}
		_leaderboardCollider.enabled = true;
		_friendsCollider.enabled = true;
	}

	private void SetToConnectingState()
	{
		_overlay.SetActive(false);
		_normalContentFriends.SetActive(false);
		_normalContentLeaderboard.SetActive(false);
		_connectingContent.SetActive(true);
		_leaderboardCollider.enabled = false;
		_friendsCollider.enabled = false;
	}
}
