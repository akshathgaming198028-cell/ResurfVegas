using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendHandlerBrag : MonoBehaviour
{
	private const float THREE_QUARTERS_LOGIN_BAR_HEIGHT = 18f;

	private const float DISTANCE_BETWEEN_GC_FB = 40f;

	[SerializeField]
	private GameObject friendNoBragPrefab;

	[SerializeField]
	private GameObject facebookLoginPrefab;

	[SerializeField]
	private GameObject gameCenterLoginPrefab;

	[SerializeField]
	private GameObject facebookBonus;

	[SerializeField]
	private GameObject gameCenterBonus;

	[SerializeField]
	private GameObject OfflineParent;

	[SerializeField]
	private GameObject OnlineFacebookParent;

	[SerializeField]
	private GameObject OnlineGameCenterParent;

	[SerializeField]
	private GameObject OnlineBothNoBragParent;

	[SerializeField]
	private UIGrid OnlineFacebookGrid;

	[SerializeField]
	private UIGrid OnlineGameCenterGrid;

	[SerializeField]
	private UIGrid OnlineBothNoBragGrid;

	[SerializeField]
	private GameObject FacebookLoginButtonParent;

	[SerializeField]
	private GameObject GameCenterLoginButtonParent;

	[SerializeField]
	private UILabel gettingLabel;

	private ScrollViewHeightResizer heightResizerBothNoBrag;

	private Vector3 initialPositionBothGrid = Vector3.zero;

	private float initialHeightBothGrid;

	private bool initializedPosition;

	private UIGrid _currentGrid;

	private List<Friend> _bragList = new List<Friend>();

	private int _friendIndexOnTopOfPlayer;

	private int _friendsCount;

	[NonSerialized]
	public string preBragPopupString = string.Empty;

	private bool canWeBrag;

	private static FriendHandlerBrag _instance;

	public List<Friend> bragList
	{
		get
		{
			return _bragList;
		}
	}

	public static FriendHandlerBrag instance
	{
		get
		{
			if (_instance == null)
			{
				Debug.LogError("FriendHandlerBrag _instance was null when you tried to access it!");
			}
			return _instance;
		}
	}

	private void ClearAllGrids()
	{
		foreach (Transform item in OnlineBothNoBragGrid.transform)
		{
			NGUITools.SetActive(item.gameObject, false);
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (Transform item2 in OnlineFacebookGrid.transform)
		{
			NGUITools.SetActive(item2.gameObject, false);
			UnityEngine.Object.Destroy(item2.gameObject);
		}
		foreach (Transform item3 in OnlineGameCenterGrid.transform)
		{
			NGUITools.SetActive(item3.gameObject, false);
			UnityEngine.Object.Destroy(item3.gameObject);
		}
	}

	public void ShowGettingReadyLabel()
	{
		ClearEverything();
		NGUITools.SetActive(gettingLabel.gameObject, true);
	}

	public void ClearEverything()
	{
		ClearAllGrids();
		NGUITools.SetActive(OnlineBothNoBragParent, false);
		NGUITools.SetActive(OnlineFacebookParent, false);
		NGUITools.SetActive(OnlineGameCenterParent, false);
		NGUITools.SetActive(OfflineParent, false);
		NGUITools.SetActive(gettingLabel.gameObject, false);
		foreach (Transform item in FacebookLoginButtonParent.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (Transform item2 in GameCenterLoginButtonParent.transform)
		{
			UnityEngine.Object.Destroy(item2.gameObject);
		}
	}

	public void GoOffline(bool enableButtons)
	{
		ClearEverything();
		NGUITools.SetActive(OfflineParent, true);
		if (enableButtons)
		{
			OfflineParent.GetComponent<GameOverOfflineHelper>().EnableButtons();
		}
		else
		{
			OfflineParent.GetComponent<GameOverOfflineHelper>().DisableButtons();
		}
		if (PlayerInfo.Instance.hasPayedOutFacebook)
		{
			NGUITools.SetActive(facebookBonus, false);
		}
		if (PlayerInfo.Instance.hasPayedOutGameCenter)
		{
			NGUITools.SetActive(gameCenterBonus, false);
		}
	}

	private void SetCurrentGridWhenLoginStateBoth(Friend[] friends)
	{
		_currentGrid = OnlineBothNoBragGrid;
		NGUITools.SetActive(OnlineBothNoBragParent, true);
		UIScrollView uIScrollView = NGUITools.FindInParents<UIScrollView>(_currentGrid.gameObject);
		uIScrollView.ResetPosition();
	}

	private void SetCurrentGridWhenLoginStateGameCenter(Friend[] friends)
	{
		_currentGrid = OnlineGameCenterGrid;
		NGUITools.SetActive(OnlineGameCenterParent, true);
		bool flag = !SocialManager.instance.facebookIsLoggedIn || (SocialManager.instance.facebookIsLoggedIn && !AreFacebookFriendsConsolidated());
		if ((bool)facebookLoginPrefab && flag)
		{
			GameObject gameObject = NGUITools.AddChild(FacebookLoginButtonParent, facebookLoginPrefab);
			gameObject.name = "facebookLoginButton";
			FacebookLoginButtonOnline component = gameObject.GetComponent<FacebookLoginButtonOnline>();
			if (component != null)
			{
				component.Init(SocialManager.instance.facebookIsLoggedIn, PlayerInfo.Instance.hasPayedOutFacebook);
			}
			if (GameCenterLoginButtonParent.transform.childCount == 0)
			{
				gameObject.transform.localPosition = new Vector3(0f, -18f, 0f);
			}
			else
			{
				gameObject.transform.localPosition = new Vector3(0f, -58f, 0f);
			}
		}
		UIScrollView uIScrollView = NGUITools.FindInParents<UIScrollView>(_currentGrid.gameObject);
		uIScrollView.ResetPosition();
	}

	private void SetCurrentGridWhenLoginStateFacebook(Friend[] friends)
	{
		_currentGrid = OnlineFacebookGrid;
		NGUITools.SetActive(OnlineFacebookParent, true);
		if (Application.platform != RuntimePlatform.IPhonePlayer)
		{
			return;
		}
		if ((Globals.DEBUG_USE_DEBUG_LOGIN_STATE || !SocialManager.instance.gamecenterIsLoggedIn || (SocialManager.instance.gamecenterIsLoggedIn && !AreGamecenterFriendsConsolidated())) && gameCenterLoginPrefab != null)
		{
			GameObject gameObject = NGUITools.AddChild(GameCenterLoginButtonParent, gameCenterLoginPrefab);
			gameObject.name = "gameCenterLoginButton";
			GameCenterButton component = gameObject.GetComponent<GameCenterButton>();
			if (component != null)
			{
				component.Init(SocialManager.instance.gamecenterIsLoggedIn, PlayerInfo.Instance.hasPayedOutGameCenter);
			}
			if (FacebookLoginButtonParent.transform.childCount == 0)
			{
				gameObject.transform.localPosition = new Vector3(0f, -18f, 0f);
			}
			else
			{
				gameObject.transform.localPosition = new Vector3(0f, -58f, 0f);
			}
		}
		_currentGrid.Reposition();
	}

	private void SetCurrentGridWhenUsingCachedLeaderboard(Friend[] friends)
	{
		SetCurrentGridWhenLoginStateGameCenter(friends);
		SetCurrentGridWhenLoginStateBoth(friends);
	}

	private void AddFriendsIntoGrid(Friend[] friends, Transform cursorTransform)
	{
		bool flag = false;
		GameObject prefab = friendNoBragPrefab;
		int num = 1;
		for (int i = 0; i < friends.Length; i++)
		{
			GameObject gameObject = NGUITools.AddChild(_currentGrid.gameObject, prefab);
			gameObject.name = string.Format("{0:000}", num);
			FriendHelperBrag component = gameObject.GetComponent<FriendHelperBrag>();
			if (!flag && PlayerInfo.Instance.highestScore >= friends[i].score)
			{
				if (i == 0)
				{
					cursorTransform = gameObject.transform;
				}
				component.InitLocalUser(num, num % 2 == 1);
				num++;
				flag = true;
				gameObject = NGUITools.AddChild(_currentGrid.gameObject, prefab);
				gameObject.name = string.Format("{0:000}", num);
				component = gameObject.GetComponent<FriendHelperBrag>();
				_friendIndexOnTopOfPlayer = i;
			}
			if (friends[i].score <= PlayerInfo.Instance.highestScore && friends[i].score > PlayerInfo.Instance.oldHighestScore)
			{
				canWeBrag = true;
			}
			bool flag2 = friends[i].score < PlayerInfo.Instance.highestScore && friends[i].score >= PlayerInfo.Instance.oldHighestScore && (bool)_currentGrid;
			component.InitFriend(friends[i], num, num % 2 == 1);
			if (!flag)
			{
				cursorTransform = gameObject.transform;
			}
			if (flag2 && !_bragList.Contains(friends[i]))
			{
				AddBragFriend(friends[i]);
			}
			num++;
			_friendsCount = friends.Length;
		}
		if (!flag)
		{
			GameObject gameObject2 = NGUITools.AddChild(_currentGrid.gameObject, prefab);
			gameObject2.name = string.Format("{0:000}", num);
			FriendHelperBrag component2 = gameObject2.GetComponent<FriendHelperBrag>();
			cursorTransform = gameObject2.transform;
			component2.InitLocalUser(num, num % 2 == 1);
			num++;
			flag = true;
			_friendIndexOnTopOfPlayer = friends.Length;
		}
	}

	private void SetPropertiesWhenCurrentGridIsOnlineBothNoBragGrid()
	{
		_currentGrid.sorting = UIGrid.Sorting.None;
		_currentGrid.Reposition();
		UIScrollView uIScrollView = NGUITools.FindInParents<UIScrollView>(_currentGrid.gameObject);
		uIScrollView.ResetPosition();
		heightResizerBothNoBrag.Height = initialHeightBothGrid - 40f;
		OnlineBothNoBragParent.transform.localPosition = initialPositionBothGrid + new Vector3(0f, 37f, 0f);
		heightResizerBothNoBrag.RearrangeWidgets();
	}

	private void SetPropertiesWhenCurrentGridIsCachedPlayers(bool hasGameCenter)
	{
		canWeBrag = false;
		if (hasGameCenter)
		{
			heightResizerBothNoBrag.Height = initialHeightBothGrid + 40f;
			OnlineBothNoBragParent.transform.localPosition = initialPositionBothGrid - new Vector3(0f, 46f, 0f);
		}
		else
		{
			heightResizerBothNoBrag.Height = initialHeightBothGrid;
			OnlineBothNoBragParent.transform.localPosition = initialPositionBothGrid - new Vector3(0f, 6f, 0f);
		}
		_currentGrid.Reposition();
		heightResizerBothNoBrag.RearrangeWidgets();
	}

	private void BragNotifyFriends()
	{
		if (PlayerInfo.Instance.highestScore > PlayerInfo.Instance.oldHighestScore && !GameStats.Instance.bragNotifyDone)
		{
			SocialManager.instance.BragNotify(PlayerInfo.Instance.oldHighestScore, bragList);
			GameStats.Instance.bragNotifyDone = true;
		}
	}

	private IEnumerator UpdatePanelAfterFrames(int frames)
	{
		int passedFrames = 0;
		while (passedFrames < frames)
		{
			passedFrames++;
			yield return null;
		}
		UIPanel panel = _currentGrid.transform.parent.GetComponent<UIPanel>();
		panel.Refresh();
		_ScrollGridToUser(panel);
	}

	private bool AreFacebookFriendsConsolidated()
	{
		int consolidationState = SocialManager.instance.consolidationState;
		return (consolidationState & 1) == 1;
	}

	private bool AreGamecenterFriendsConsolidated()
	{
		int consolidationState = SocialManager.instance.consolidationState;
		return (consolidationState & 2) == 2;
	}

	public void ShowFriendList()
	{
		Transform parent = base.transform.parent;
		bool flag = false;
		Friend[] array = SocialManager.instance.FriendsSortedByScore();
		Globals.LogInState loginState = Globals.GetLoginState();
		bool flag2 = SocialManager.instance.facebookState == SocialManager.FacebookState.LoggingIn;
		bool flag3 = Globals.DEBUG_USE_DEBUG_LOGIN_STATE || AreFacebookFriendsConsolidated();
		if ((loginState == Globals.LogInState.Both || loginState == Globals.LogInState.Facebook) && flag3)
		{
			SetCurrentGridWhenLoginStateBoth(array);
		}
		else
		{
			if ((flag2 || !AreFacebookFriendsConsolidated()) && !CachedLeaderboard.IsAvailable())
			{
				SetCurrentGridWhenLoginStateGameCenter(array);
				return;
			}
			if (CachedLeaderboard.IsAvailable())
			{
				array = CachedLeaderboard.Load();
				flag = true;
				SetCurrentGridWhenUsingCachedLeaderboard(array);
			}
		}
		AddFriendsIntoGrid(array, parent);
		if (_currentGrid == OnlineBothNoBragGrid && !flag)
		{
			SetPropertiesWhenCurrentGridIsOnlineBothNoBragGrid();
		}
		else if (flag)
		{
			SetPropertiesWhenCurrentGridIsCachedPlayers(false);
		}
		else
		{
			_currentGrid.sorting = UIGrid.Sorting.None;
			_currentGrid.Reposition();
			UIPanel component = _currentGrid.transform.parent.GetComponent<UIPanel>();
			component.GetComponent<UIScrollView>().RestrictWithinBounds(true);
			_currentGrid.transform.localPosition = initialPositionBothGrid;
			heightResizerBothNoBrag.Height = initialHeightBothGrid;
			heightResizerBothNoBrag.RearrangeWidgets();
		}
		if (canWeBrag)
		{
			BragNotifyFriends();
		}
		StartCoroutine(UpdatePanelAfterFrames(1));
	}

	public void AddBragFriend(Friend friend)
	{
		if (!_bragList.Contains(friend))
		{
			_bragList.Add(friend);
		}
	}

	public void RemoveBragFriend(Friend friend)
	{
		if (_bragList.Contains(friend))
		{
			_bragList.Remove(friend);
		}
	}

	public void CompletedBrag()
	{
		_currentGrid.gameObject.BroadcastMessage("CompletedBragging", SendMessageOptions.DontRequireReceiver);
		_bragList.Clear();
		PlayerInfo.Instance.BragCompleted();
		ClearEverything();
		ShowFriendList();
	}

	private void MovedAwayFromGameOverScreenClicked()
	{
		PlayerInfo.Instance.BragCompleted();
		_bragList.Clear();
	}

	private void OnDisable()
	{
		MovedAwayFromGameOverScreenClicked();
	}

	private void _ScrollGridToUser(UIPanel dragPanel)
	{
		float num = _GetPositionOnGrid(_friendIndexOnTopOfPlayer, dragPanel.baseClipRegion.w);
		Vector3 localPosition = dragPanel.transform.localPosition;
		localPosition.y += num;
		dragPanel.transform.localPosition = localPosition;
		Vector2 clipOffset = dragPanel.clipOffset;
		clipOffset.y -= num;
		dragPanel.clipOffset = clipOffset;
	}

	private float _GetPositionOnGrid(int index, float clipHeight)
	{
		float num = 0f;
		int num2 = 33;
		float num3 = clipHeight / (float)num2;
		int num4 = Mathf.CeilToInt(num3);
		float num5 = (num3 - (float)num4) * (float)num2;
		if (index > 1 && _friendsCount > num4)
		{
			int num6 = _friendsCount + 1 - index;
			if (num6 < num4)
			{
				index -= num4 - num6;
				return (float)(num2 * index) - num5;
			}
			index--;
			return num2 * index;
		}
		return 0f;
	}

	private void Awake()
	{
		_instance = this;
		heightResizerBothNoBrag = OnlineBothNoBragParent.GetComponentInChildren<ScrollViewHeightResizer>();
		if (!initializedPosition)
		{
			initialPositionBothGrid = OnlineBothNoBragParent.transform.localPosition;
			initialHeightBothGrid = heightResizerBothNoBrag.Height;
			initializedPosition = true;
		}
	}
}
