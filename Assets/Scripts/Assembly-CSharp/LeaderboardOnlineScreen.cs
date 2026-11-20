using System;
using System.Collections.Generic;
using System.Linq;
using Kiloo.Common.UI;
using UnityEngine;

public class LeaderboardOnlineScreen : UIScreen
{
	public class LeaderboardListAdapter : ListAdapter<LeaderboardCell>
	{
		private const int FRIEND_PREFAB = 0;

		private const int FACEBOOK_PREFAB = 1;

		private const int PLAYER_PREFAB = 2;

		private const int SPACE_PREFAB = 3;

		public LeaderboardOnlineScreen leaderboardScreen;

		public GameObject friendPrefab;

		public GameObject facebookPrefab;

		public GameObject playerPrefab;

		public GameObject spacePrefab;

		public LeaderboardListAdapter(List<LeaderboardCell> contentList, GameObject newFriendPrefab, GameObject newFacebookPrefab, GameObject newPlayerPrefab, GameObject newSpacePrefab, LeaderboardOnlineScreen screenReference)
			: base(contentList)
		{
			friendPrefab = newFriendPrefab;
			facebookPrefab = newFacebookPrefab;
			playerPrefab = newPlayerPrefab;
			spacePrefab = newSpacePrefab;
			leaderboardScreen = screenReference;
		}

		public void setNewDataList(List<LeaderboardCell> leaderboardListT)
		{
			base.dataList = leaderboardListT;
			NotifyDataSetChanged();
		}

		public override int GetModelType(int index)
		{
			switch (GetItem(index).cellType)
			{
			case LeaderboardCellType.FRIEND:
				return 0;
			case LeaderboardCellType.FACEBOOK_BUTTON:
				return 1;
			case LeaderboardCellType.PLAYER:
				return 2;
			case LeaderboardCellType.SPACE:
				return 3;
			case LeaderboardCellType.DUMMY_FRIEND:
				return 0;
			default:
				return 0;
			}
		}

		public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass)
		{
			Container container2 = container;
			LeaderboardCell item = GetItem(dataIndex);
			if (container2 == null)
			{
				LeaderboardCellReferences leaderboardCellReferences = new LeaderboardCellReferences();
				switch (item.cellType)
				{
				case LeaderboardCellType.FRIEND:
				{
					container2 = new Container(friendPrefab);
					FriendHelperHighScore component5 = container2.model.GetComponent<FriendHelperHighScore>();
					leaderboardCellReferences.friendHelper = component5;
					container2.tag = leaderboardCellReferences;
					container2.model.GetComponent<UIDragScrollView>().enabled = !item.lockList;
					break;
				}
				case LeaderboardCellType.FACEBOOK_BUTTON:
				{
					container2 = new Container(facebookPrefab);
					FriendsLoginFBButton component4 = container2.model.GetComponent<FriendsLoginFBButton>();
					leaderboardCellReferences.facebookScript = component4;
					container2.tag = leaderboardCellReferences;
					leaderboardScreen.SetFBReference(container2.model.transform.FindChild("Outline").gameObject);
					break;
				}
				case LeaderboardCellType.PLAYER:
				{
					container2 = new Container(playerPrefab);
					FriendHelperHighScore component3 = container2.model.GetComponent<FriendHelperHighScore>();
					leaderboardCellReferences.friendHelper = component3;
					container2.tag = leaderboardCellReferences;
					container2.model.GetComponent<UIDragScrollView>().enabled = !item.lockList;
					break;
				}
				case LeaderboardCellType.DUMMY_FRIEND:
				{
					container2 = new Container(friendPrefab);
					FriendHelperHighScore component2 = container2.model.GetComponent<FriendHelperHighScore>();
					leaderboardCellReferences.friendHelper = component2;
					container2.tag = leaderboardCellReferences;
					break;
				}
				case LeaderboardCellType.SPACE:
					container2 = new Container(spacePrefab);
					break;
				default:
				{
					Debug.LogError("No cell type match for: " + item.cellType);
					container2 = new Container(friendPrefab);
					FriendHelperHighScore component = container2.model.GetComponent<FriendHelperHighScore>();
					leaderboardCellReferences.friendHelper = component;
					container2.tag = leaderboardCellReferences;
					break;
				}
				}
			}
			LeaderboardCellReferences leaderboardCellReferences2 = (LeaderboardCellReferences)container2.tag;
			switch (item.cellType)
			{
			case LeaderboardCellType.FRIEND:
				leaderboardCellReferences2.friendHelper.InitFriend(item.friend, item.rank, item.useDarkColour);
				item.OnUpdateCell = leaderboardCellReferences2.friendHelper.Refresh;
				break;
			case LeaderboardCellType.FACEBOOK_BUTTON:
				leaderboardCellReferences2.facebookScript.Init(leaderboardScreen.gameObject, "FacebookClicked", true, item.noFriends);
				break;
			case LeaderboardCellType.PLAYER:
				leaderboardCellReferences2.friendHelper.InitLocalUser(item.rank, item.useDarkColour, item.isPlayerRanked);
				item.OnUpdateCell = leaderboardCellReferences2.friendHelper.Refresh;
				break;
			case LeaderboardCellType.DUMMY_FRIEND:
				leaderboardCellReferences2.friendHelper.InitDummy(item.dummyFriend, item.rank, item.useDarkColour);
				break;
			default:
				Debug.LogError("No cell type match for: " + item.cellType);
				break;
			case LeaderboardCellType.SPACE:
				break;
			}
			return container2;
		}
	}

	public GameObject FriendPrefab;

	public GameObject FacebookLoginPrefab;

	public GameObject SpacePrefab;

	[SerializeField]
	public UIWidget back;

	[SerializeField]
	public UIWidget titleAnchor;

	[SerializeField]
	private ScrollView _scrollView;

	[SerializeField]
	private UIPanel _scrollPanel;

	[SerializeField]
	private UIWidget _overlaySizer;

	[SerializeField]
	private GameObject _lockContent;

	[SerializeField]
	private UILabel _noFriendsLabel;

	private int GC_FRIENDS_THRESHOLD = 4;

	public static GameObject fbButtonReference;

	private List<LeaderboardCell> _contentList = new List<LeaderboardCell>();

	private LeaderboardListAdapter _adapter;

	private bool _facebookLoginRunning;

	public override void Init()
	{
		base.Init();
		SetListContent();
		if (_contentList != null)
		{
			_adapter = new LeaderboardListAdapter(_contentList, FriendPrefab, FacebookLoginPrefab, FriendPrefab, SpacePrefab, this);
		}
	}

	public override void Show()
	{
		base.Show();
		SocialManager.instance.AddFriendsChangedHandler(ReloadFriends);
		UIScreenController.OnApplicationResumed += ReloadFriends;
		SocialManager.instance.UpdateFriendScores(delegate
		{
			ReloadFriends();
		});
		ReloadFriends();
	}

	private void ScrollToMyPosition()
	{
		int myIndexInTheList = GetMyIndexInTheList();
		if (myIndexInTheList > 0)
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
				if (_contentList[i].cellType == LeaderboardCellType.PLAYER)
				{
					result = i;
				}
			}
		}
		return result;
	}

	public override void Hide()
	{
		base.Hide();
		SocialManager.instance.RemoveFriendsChangedHandler(ReloadFriends);
		UIScreenController.OnApplicationResumed -= ReloadFriends;
	}

	private void ForceUpdateAnchors()
	{
		titleAnchor.ResetAndUpdateAnchors();
		_scrollPanel.ResetAndUpdateAnchors();
		_scrollPanel.topAnchor.rect.ForceUpdate();
		_scrollPanel.bottomAnchor.rect.ForceUpdate();
		_scrollPanel.leftAnchor.rect.ForceUpdate();
		_scrollPanel.rightAnchor.rect.ForceUpdate();
	}

	private void ReloadFriends()
	{
		SetListContent();
		ForceUpdateAnchors();
		if (_scrollView != null && _adapter != null)
		{
			_scrollView.SetAdapter(_adapter);
		}
		_overlaySizer.ResetAnchors();
		LeaderboardScreenManager.LeaderboardScreenState screenState = LeaderboardScreenManager.Instance.screenState;
		Friend[] source = SocialManager.instance.FriendsSortedByScore();
		bool flag = screenState == LeaderboardScreenManager.LeaderboardScreenState.Facebook || screenState == LeaderboardScreenManager.LeaderboardScreenState.FacebookAndGamecenter;
		if (((screenState == LeaderboardScreenManager.LeaderboardScreenState.GameCenter && source.Count() >= GC_FRIENDS_THRESHOLD) || flag) && screenState != LeaderboardScreenManager.LeaderboardScreenState.Offline)
		{
			ScrollToMyPosition();
		}
	}

	private void ResetFriendList()
	{
		_contentList.Clear();
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

	private void SetOfflineContent()
	{
		int nElements = 0;
		int rankCount = 1;
		InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, false, true);
		AddFacebookButtonToList(ref nElements, true);
		AddDummiesToList(ref rankCount, ref nElements);
		AddOverlayAndLockList(true);
		SetupNoFriends(false);
	}

	private void SetOnlineContent(LeaderboardScreenManager.LeaderboardScreenState screenState)
	{
		Friend[] array = SocialManager.instance.FriendsSortedByScore();
		bool lockList = screenState == LeaderboardScreenManager.LeaderboardScreenState.GameCenter && array.Count() < GC_FRIENDS_THRESHOLD;
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
		bool flag2 = !fbConnected && friends.Count() >= GC_FRIENDS_THRESHOLD;
		bool flag3 = !fbConnected && friends.Count() < GC_FRIENDS_THRESHOLD;
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
				AddDummiesToList(ref rankCount, ref nElements);
				AddOverlayAndLockList(true);
			}
			else
			{
				AddOverlayAndLockList(false);
			}
			AddSpaceToList();
			SetupNoFriends(!friends.Any() && fbConnected);
		}
		else
		{
			InsertPlayerScoreToLeaderboard(ref rankCount, ref nElements, true, true);
			if (!fbConnected)
			{
				InsertFacebookLoginButton(ref nElements);
				AddDummiesToList(ref rankCount, ref nElements);
				AddOverlayAndLockList(true);
				SetupNoFriends(false);
			}
			else
			{
				AddOverlayAndLockList(false);
				SetupNoFriends(true);
			}
		}
	}

	private void InsertPlayerScoreToLeaderboard(ref int rankCount, ref int nElements, bool isRanked, bool listLocked)
	{
		_contentList.Add(new LeaderboardCell
		{
			cellType = LeaderboardCellType.PLAYER,
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
		_contentList.Add(new LeaderboardCell
		{
			cellType = LeaderboardCellType.FRIEND,
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
		_contentList.Add(new LeaderboardCell
		{
			cellType = LeaderboardCellType.SPACE
		});
	}

	private void InsertFacebookLoginButton(ref int nElements)
	{
		_contentList.Add(new LeaderboardCell
		{
			cellType = LeaderboardCellType.FACEBOOK_BUTTON
		});
	}

	private void AddFacebookButtonToList(ref int nElements, bool noFriends = false)
	{
		_contentList.Add(new LeaderboardCell
		{
			cellType = LeaderboardCellType.FACEBOOK_BUTTON,
			noFriends = noFriends
		});
		nElements++;
	}

	private void AddDummiesToList(ref int rankCount, ref int nElements)
	{
		List<LeaderboardDummyData.LeaderboardDummy> dummyData = LeaderboardDummyData.dummyData;
		if (dummyData != null)
		{
			foreach (LeaderboardDummyData.LeaderboardDummy item in dummyData)
			{
				_contentList.Add(new LeaderboardCell
				{
					cellType = LeaderboardCellType.DUMMY_FRIEND,
					dummyFriend = item,
					rank = rankCount,
					useDarkColour = (nElements % 2 == 0)
				});
				rankCount++;
				nElements++;
			}
			return;
		}
		Debug.Log("Dummy datat not initialized. Aborting dummy initialization.");
	}

	private void AddOverlayAndLockList(bool enable)
	{
		_lockContent.SetActive(enable);
	}

	public void SetFBReference(UnityEngine.Object fbRef)
	{
		fbButtonReference = (GameObject)fbRef;
		_overlaySizer.topAnchor.target = fbButtonReference.transform;
	}

	private void SetupNoFriends(bool enable)
	{
		_noFriendsLabel.gameObject.SetActive(enable);
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_noFriendsLabel.text = Strings.Get(StringID.NO_FRIENDS_FB_AND_GC);
		}
		else
		{
			_noFriendsLabel.text = Strings.Get(StringID.NO_FRIENDS_ONLY_FB);
		}
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
			ReloadFriends();
		}
		else if (_facebookLoginRunning)
		{
			OnFacebookLoginResult(false);
		}
		HouseKeeper.SetFacebookPayoutActionToDefault();
		_facebookLoginRunning = false;
	}

	private void OnDisable()
	{
		if (_facebookLoginRunning)
		{
			HouseKeeper.SetFacebookPayoutActionToDefault();
			_facebookLoginRunning = false;
		}
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
