using System;
using System.Collections.Generic;
using System.Linq;
using Kiloo.Common.UI;
using UnityEngine;

public class GameOverLeaderboardHandler : MonoBehaviour
{
	public class LeaderboardListAdapter : ListAdapter<LeaderboardGameoverCell>
	{
		private const int FRIEND_PREFAB = 0;

		private const int FACEBOOK_PREFAB = 1;

		private const int PLAYER_PREFAB = 2;

		private const int SPACE_PREFAB = 3;

		public GameOverLeaderboardHandler leaderboardScreen;

		public GameObject friendPrefab;

		public GameObject facebookPrefab;

		public GameObject playerPrefab;

		public GameObject spacePrefab;

		public LeaderboardListAdapter(List<LeaderboardGameoverCell> contentList, GameObject newFriendPrefab, GameObject newFacebookPrefab, GameObject newPlayerPrefab, GameObject newSpacePrefab, GameOverLeaderboardHandler screenReference)
			: base(contentList)
		{
			friendPrefab = newFriendPrefab;
			facebookPrefab = newFacebookPrefab;
			playerPrefab = newPlayerPrefab;
			spacePrefab = newSpacePrefab;
			leaderboardScreen = screenReference;
		}

		public void setNewDataList(List<LeaderboardGameoverCell> leaderboardListT)
		{
			base.dataList = leaderboardListT;
			NotifyDataSetChanged();
		}

		public override int GetModelType(int index)
		{
			switch (GetItem(index).cellType)
			{
			case LeaderboardGameoverCellType.FRIEND:
				return 0;
			case LeaderboardGameoverCellType.FACEBOOK_BUTTON:
				return 1;
			case LeaderboardGameoverCellType.PLAYER:
				return 2;
			case LeaderboardGameoverCellType.SPACE:
				return 3;
			default:
				return 0;
			}
		}

		public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass)
		{
			Container container2 = container;
			LeaderboardGameoverCell item = GetItem(dataIndex);
			if (container2 == null)
			{
				LeaderboardGameoverCellReferences leaderboardGameoverCellReferences = new LeaderboardGameoverCellReferences();
				switch (item.cellType)
				{
				case LeaderboardGameoverCellType.FRIEND:
				{
					container2 = new Container(friendPrefab);
					FriendHelperHighScore component4 = container2.model.GetComponent<FriendHelperHighScore>();
					leaderboardGameoverCellReferences.friendHelper = component4;
					container2.tag = leaderboardGameoverCellReferences;
					container2.model.GetComponent<UIDragScrollView>().enabled = !item.lockList;
					break;
				}
				case LeaderboardGameoverCellType.FACEBOOK_BUTTON:
				{
					container2 = new Container(facebookPrefab);
					FriendsLoginFBButton component3 = container2.model.GetComponent<FriendsLoginFBButton>();
					leaderboardGameoverCellReferences.facebookScript = component3;
					container2.tag = leaderboardGameoverCellReferences;
					break;
				}
				case LeaderboardGameoverCellType.PLAYER:
				{
					container2 = new Container(playerPrefab);
					FriendHelperHighScore component2 = container2.model.GetComponent<FriendHelperHighScore>();
					leaderboardGameoverCellReferences.friendHelper = component2;
					container2.tag = leaderboardGameoverCellReferences;
					container2.model.GetComponent<UIDragScrollView>().enabled = !item.lockList;
					break;
				}
				case LeaderboardGameoverCellType.SPACE:
					container2 = new Container(spacePrefab);
					break;
				default:
				{
					Debug.LogError("No cell type match for: " + item.cellType);
					container2 = new Container(friendPrefab);
					FriendHelperHighScore component = container2.model.GetComponent<FriendHelperHighScore>();
					leaderboardGameoverCellReferences.friendHelper = component;
					container2.tag = leaderboardGameoverCellReferences;
					break;
				}
				}
			}
			LeaderboardGameoverCellReferences leaderboardGameoverCellReferences2 = (LeaderboardGameoverCellReferences)container2.tag;
			switch (item.cellType)
			{
			case LeaderboardGameoverCellType.FRIEND:
				leaderboardGameoverCellReferences2.friendHelper.InitFriend(item.friend, item.rank, item.useDarkColour);
				item.OnUpdateCell = leaderboardGameoverCellReferences2.friendHelper.Refresh;
				break;
			case LeaderboardGameoverCellType.FACEBOOK_BUTTON:
				leaderboardGameoverCellReferences2.facebookScript.Init(leaderboardScreen.gameObject, "FacebookClicked", true, item.noFriends);
				break;
			case LeaderboardGameoverCellType.PLAYER:
				leaderboardGameoverCellReferences2.friendHelper.InitLocalUser(item.rank, item.useDarkColour, item.isPlayerRanked);
				item.OnUpdateCell = leaderboardGameoverCellReferences2.friendHelper.Refresh;
				break;
			default:
				Debug.LogError("No cell type match for: " + item.cellType);
				break;
			case LeaderboardGameoverCellType.SPACE:
				break;
			}
			return container2;
		}
	}

	private const int GC_FRIENDS_THRESHOLD = 4;

	private const int OFFSET_FROM_TOP = -39;

	public GameObject FriendPrefab;

	public GameObject FacebookLoginPrefab;

	public GameObject SpacePrefab;

	[SerializeField]
	private ScrollView _scrollView;

	[SerializeField]
	private UIPanel _scrollPanel;

	private List<LeaderboardGameoverCell> _contentList = new List<LeaderboardGameoverCell>();

	private LeaderboardListAdapter _adapter;

	private bool _facebookLoginRunning;

	public static GameObject fbButtonReference;

	public void Init()
	{
		if (_contentList != null)
		{
			_adapter = new LeaderboardListAdapter(_contentList, FriendPrefab, FacebookLoginPrefab, FriendPrefab, SpacePrefab, this);
		}
		SocialManager.instance.AddFriendsChangedHandler(ReloadFriends);
		SocialManager.instance.UpdateFriendScores(delegate
		{
			ReloadFriends();
		});
		ReloadFriends();
	}

	private void OnDisable()
	{
		SocialManager.instance.RemoveFriendsChangedHandler(ReloadFriends);
		if (_facebookLoginRunning)
		{
			HouseKeeper.SetFacebookPayoutActionToDefault();
			_facebookLoginRunning = false;
		}
	}

	private void ReloadFriends()
	{
		SetListContent();
		ForceUpdateAnchors();
		if (_scrollView != null && _adapter != null)
		{
			_scrollView.SetAdapter(_adapter);
		}
		LeaderboardScreenManager.LeaderboardScreenState screenState = LeaderboardScreenManager.Instance.screenState;
		if (screenState == LeaderboardScreenManager.LeaderboardScreenState.Facebook || screenState == LeaderboardScreenManager.LeaderboardScreenState.GameCenter || screenState == LeaderboardScreenManager.LeaderboardScreenState.FacebookAndGamecenter)
		{
			ScrollToMyPosition();
		}
	}

	private void ScrollToMyPosition()
	{
		int myIndexInTheList = GetMyIndexInTheList();
		if (myIndexInTheList >= 0)
		{
			float offset = _scrollView.rootPanel.baseClipRegion.w * 0.5f - _scrollView.GetFirstVisibleContainerOnScreen().height * 0.5f;
			_scrollView.SetSelection(myIndexInTheList, ContainerPivot.Top, offset);
		}
	}

	private int GetMyIndexInTheList()
	{
		int result = -1;
		if (_contentList != null)
		{
			for (int i = 0; i < _contentList.Count; i++)
			{
				if (_contentList[i].cellType == LeaderboardGameoverCellType.PLAYER)
				{
					result = i;
				}
			}
		}
		return result;
	}

	private void ForceUpdateAnchors()
	{
		_scrollPanel.topAnchor.absolute = -39;
		_scrollPanel.topAnchor.relative = 0f;
		_scrollPanel.topAnchor.Set(0f, -39f);
		_scrollPanel.ResetAndUpdateAnchors();
		_scrollPanel.bottomAnchor.rect.ForceUpdate();
		_scrollPanel.leftAnchor.rect.ForceUpdate();
		_scrollPanel.rightAnchor.rect.ForceUpdate();
	}

	private void SetListContent()
	{
		ResetFriendList();
		LeaderboardScreenManager.LeaderboardScreenState screenState = LeaderboardScreenManager.Instance.screenState;
		switch (screenState)
		{
		case LeaderboardScreenManager.LeaderboardScreenState.Offline:
			SetOfflineContent();
			break;
		case LeaderboardScreenManager.LeaderboardScreenState.Facebook:
		case LeaderboardScreenManager.LeaderboardScreenState.GameCenter:
		case LeaderboardScreenManager.LeaderboardScreenState.FacebookAndGamecenter:
			SetOnlineContent(screenState);
			break;
		default:
			SetOfflineContent();
			break;
		}
	}

	private void ResetFriendList()
	{
		_contentList.Clear();
	}

	private void SetOfflineContent()
	{
		int nElements = 0;
		int rankCount = 1;
		InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, false, true);
		AddFacebookButtonToList(ref nElements, true, true);
	}

	private void SetOnlineContent(LeaderboardScreenManager.LeaderboardScreenState screenState)
	{
		Friend[] array = SocialManager.instance.FriendsSortedByScore();
		bool lockList = screenState == LeaderboardScreenManager.LeaderboardScreenState.GameCenter && array.Count() < 4;
		switch (screenState)
		{
		case LeaderboardScreenManager.LeaderboardScreenState.Facebook:
		case LeaderboardScreenManager.LeaderboardScreenState.FacebookAndGamecenter:
			SetOnlineContentListForLeaderboard(true, array, lockList);
			break;
		case LeaderboardScreenManager.LeaderboardScreenState.GameCenter:
			SetOnlineContentListForLeaderboard(false, array, lockList);
			break;
		}
	}

	private void SetOnlineContentListForLeaderboard(bool fbConnected, Friend[] friends, bool lockList)
	{
		bool flag = false;
		int nElements = 0;
		int rankCount = 1;
		int num = friends.Count();
		bool flag2 = !fbConnected && friends.Count() >= 4;
		bool flag3 = !fbConnected && friends.Count() < 4;
		if (num > 0)
		{
			for (int i = 0; i < friends.Length; i++)
			{
				if (!flag && PlayerInfo.Instance.highestScore >= friends[i].score)
				{
					InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, true, lockList);
					flag = true;
					if (flag2)
					{
						InsertFacebookLoginButton(ref nElements);
					}
				}
				InsertFriendScoreToLeaderboard(friends[i], ref rankCount, ref nElements, lockList);
			}
			if (!flag)
			{
				InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, true, lockList);
				if (flag2)
				{
					InsertFacebookLoginButton(ref nElements);
				}
			}
			if (flag3)
			{
				InsertFacebookLoginButton(ref nElements);
			}
			AddSpaceToList();
		}
		else
		{
			InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, true, true);
			if (!fbConnected)
			{
				AddFacebookButtonToList(ref nElements, true, lockList);
			}
		}
	}

	private void InsertPlayerScoreToLeaderboard(ref int rankCount, ref int nElements, bool isRanked, bool listLocked)
	{
		_contentList.Add(new LeaderboardGameoverCell
		{
			cellType = LeaderboardGameoverCellType.PLAYER,
			rank = rankCount,
			useDarkColour = (nElements % 2 == 0),
			isPlayerRanked = isRanked,
			lockList = listLocked
		});
		if (isRanked)
		{
			rankCount++;
		}
		nElements++;
	}

	private void InsertFriendScoreToLeaderboard(Friend newFriend, ref int rankCount, ref int nElements, bool listLocked)
	{
		_contentList.Add(new LeaderboardGameoverCell
		{
			cellType = LeaderboardGameoverCellType.FRIEND,
			friend = newFriend,
			useDarkColour = (nElements % 2 == 0),
			rank = rankCount,
			lockList = listLocked
		});
		rankCount++;
		nElements++;
	}

	private void AddSpaceToList()
	{
		_contentList.Add(new LeaderboardGameoverCell
		{
			cellType = LeaderboardGameoverCellType.SPACE
		});
	}

	private void InsertFacebookLoginButton(ref int nElements)
	{
		_contentList.Add(new LeaderboardGameoverCell
		{
			cellType = LeaderboardGameoverCellType.FACEBOOK_BUTTON
		});
	}

	private void AddFacebookButtonToList(ref int nElements, bool noFriends = false, bool lockList = false)
	{
		_contentList.Add(new LeaderboardGameoverCell
		{
			cellType = LeaderboardGameoverCellType.FACEBOOK_BUTTON,
			noFriends = noFriends,
			lockList = lockList
		});
		nElements++;
	}

	private void FacebookClicked()
	{
		HouseKeeper.SetFacebookPayoutAction(FacebookStatusChanged);
		_facebookLoginRunning = true;
		if (SocialManager.instance.FacebookLogin(OnFacebookLoginResult))
		{
			UIScreenController.Instance.FacebookLogIn(true);
		}
	}

	private void FacebookStatusChanged(bool success)
	{
		if (success)
		{
			if (!FacebookRewardsInfo.HasReceivedAllAvailableRewards())
			{
				UIScreenController.Instance.PushPopup("FacebookRewardsPopup");
			}
			else
			{
				UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Facebook, UISlideInFacebook.FacebookSlideInState.ConnectSucceeded));
			}
		}
		else if (_facebookLoginRunning)
		{
			OnFacebookLoginResult(false);
		}
		HouseKeeper.SetFacebookPayoutActionToDefault();
		_facebookLoginRunning = false;
	}

	public void OnFacebookLoginResult(bool loggedIn)
	{
		if (!loggedIn && _facebookLoginRunning)
		{
			_facebookLoginRunning = false;
			QueueFacebookFailSlideIn(OnFailSlideInFinish);
		}
	}

	private void QueueFacebookFailSlideIn(Action onFinishCallback)
	{
		UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Facebook, UISlideInFacebook.FacebookSlideInState.ConnectFailed, onFinishCallback));
	}

	private void OnFailSlideInFinish()
	{
		FacebookStatusChanged(false);
	}
}
