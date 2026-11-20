using System;
using System.Collections.Generic;
using System.Linq;
using Kiloo.Common.UI;
using UnityEngine;

public class FriendsOnlineScreen : UIScreen
{
	public class CollectCoinsListAdapter : ListAdapter<CollectCoinsCell>
	{
		private const int FRIEND_PREFAB = 0;

		private const int FACEBOOK_PREFAB = 1;

		private const int RECOMMEND_PREFAB = 2;

		private const int SPACE_PREFAB = 3;

		public FriendsOnlineScreen friendsScreen;

		public GameObject friendPrefab;

		public GameObject facebookPrefab;

		public GameObject recommendPrefab;

		public GameObject spacePrefab;

		public CollectCoinsListAdapter(List<CollectCoinsCell> contentList, GameObject newFriendPrefab, GameObject newFacebookPrefab, GameObject newRecommendPrefab, GameObject newSpacePrefab, FriendsOnlineScreen screenRef)
			: base(contentList)
		{
			friendPrefab = newFriendPrefab;
			facebookPrefab = newFacebookPrefab;
			recommendPrefab = newRecommendPrefab;
			spacePrefab = newSpacePrefab;
			friendsScreen = screenRef;
		}

		public void setNewDataList(List<CollectCoinsCell> collectCoinsListT)
		{
			base.dataList = collectCoinsListT;
			NotifyDataSetChanged();
		}

		public override int GetModelType(int index)
		{
			switch (GetItem(index).cellType)
			{
			case CollectCoinsCellType.FRIEND:
				return 0;
			case CollectCoinsCellType.DUMMYFRIEND:
				return 0;
			case CollectCoinsCellType.FACEBOOK_BUTTON:
				return 1;
			case CollectCoinsCellType.RECOMMEND_BUTTON:
				return 2;
			case CollectCoinsCellType.SPACE:
				return 3;
			default:
				return 0;
			}
		}

		public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass)
		{
			Container container2 = container;
			CollectCoinsCell item = GetItem(dataIndex);
			if (container2 == null)
			{
				CollectCoinsCellReferences collectCoinsCellReferences = new CollectCoinsCellReferences();
				switch (item.cellType)
				{
				case CollectCoinsCellType.FRIEND:
				{
					container2 = new Container(friendPrefab);
					FriendHelperCrew component4 = container2.model.GetComponent<FriendHelperCrew>();
					collectCoinsCellReferences.friendHelper = component4;
					container2.tag = collectCoinsCellReferences;
					container2.model.GetComponent<UIDragScrollView>().enabled = !item.disableScrolling;
					break;
				}
				case CollectCoinsCellType.DUMMYFRIEND:
				{
					container2 = new Container(friendPrefab);
					FriendHelperCrew component3 = container2.model.GetComponent<FriendHelperCrew>();
					collectCoinsCellReferences.friendHelper = component3;
					container2.tag = collectCoinsCellReferences;
					break;
				}
				case CollectCoinsCellType.FACEBOOK_BUTTON:
				{
					container2 = new Container(facebookPrefab);
					FriendsLoginFBButton component2 = container2.model.GetComponent<FriendsLoginFBButton>();
					collectCoinsCellReferences.facebookScript = component2;
					container2.tag = collectCoinsCellReferences;
					friendsScreen.SetFBReference(container2.model.transform.FindChild("Outline").gameObject);
					break;
				}
				case CollectCoinsCellType.RECOMMEND_BUTTON:
					container2 = new Container(recommendPrefab);
					break;
				case CollectCoinsCellType.SPACE:
					container2 = new Container(spacePrefab);
					break;
				default:
				{
					Debug.LogError("No cell type match for: " + item.cellType);
					container2 = new Container(friendPrefab);
					FriendHelperCrew component = container2.model.GetComponent<FriendHelperCrew>();
					collectCoinsCellReferences.friendHelper = component;
					container2.tag = collectCoinsCellReferences;
					break;
				}
				}
			}
			CollectCoinsCellReferences collectCoinsCellReferences2 = (CollectCoinsCellReferences)container2.tag;
			switch (item.cellType)
			{
			case CollectCoinsCellType.FRIEND:
			{
				collectCoinsCellReferences2.friendHelper.InitFriend(item.friend, item.useDarkColour);
				item.OnUpdateCell = collectCoinsCellReferences2.friendHelper.Refresh;
				UIDragScrollView component5 = container2.model.GetComponent<UIDragScrollView>();
				component5.enabled = !item.disableScrolling;
				break;
			}
			case CollectCoinsCellType.DUMMYFRIEND:
				collectCoinsCellReferences2.friendHelper.InitDummyFriend(item.dummyFriend, item.useDarkColour);
				item.OnUpdateCell = collectCoinsCellReferences2.friendHelper.Refresh;
				break;
			case CollectCoinsCellType.FACEBOOK_BUTTON:
				collectCoinsCellReferences2.facebookScript.Init(friendsScreen.gameObject, "FacebookClicked");
				break;
			default:
				Debug.LogError("No cell type match for: " + item.cellType);
				break;
			case CollectCoinsCellType.RECOMMEND_BUTTON:
			case CollectCoinsCellType.SPACE:
				break;
			}
			return container2;
		}
	}

	[SerializeField]
	private GameObject friendPrefab;

	[SerializeField]
	private GameObject facebookPrefab;

	[SerializeField]
	private GameObject recommendPrefab;

	[SerializeField]
	private GameObject spacePrefab;

	[SerializeField]
	private UILabel CrewHeader;

	[SerializeField]
	private UIWidget titleAnchor;

	[SerializeField]
	private UILabel NoFriends;

	private List<CollectCoinsCell> _contentList = new List<CollectCoinsCell>();

	private CollectCoinsListAdapter _adapter;

	public static GameObject fbButtonReference;

	[SerializeField]
	private ScrollView _collectCoinsListView;

	[SerializeField]
	private UIPanel _scrollPanel;

	[SerializeField]
	private UISprite _scrollBarSprite;

	[SerializeField]
	private UIPanel _scrollBarPanel;

	[SerializeField]
	private UIWidget _overlaySizer;

	[SerializeField]
	private GameObject _lockContent;

	private bool _facebookLoginRunning;

	public override void Init()
	{
		base.Init();
		InitializeCoinbox(null, true, true, true);
		SetListContent();
		if (_contentList != null)
		{
			_adapter = new CollectCoinsListAdapter(_contentList, friendPrefab, facebookPrefab, recommendPrefab, spacePrefab, this);
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

	private void ForceUpdateAnchors()
	{
		titleAnchor.ResetAndUpdateAnchors();
		_scrollPanel.ResetAndUpdateAnchors();
	}

	public override void Hide()
	{
		base.Hide();
		SocialManager.instance.RemoveFriendsChangedHandler(ReloadFriends);
		UIScreenController.OnApplicationResumed -= ReloadFriends;
	}

	private void ReloadFriends()
	{
		SetListContent();
		ForceUpdateAnchors();
		if (_collectCoinsListView != null && _adapter != null)
		{
			_collectCoinsListView.SetAdapter(_adapter);
		}
		_overlaySizer.ResetAnchors();
	}

	private void ResetFriendList()
	{
		_contentList.Clear();
	}

	private void SetListContent()
	{
		ResetFriendList();
		CollectCoinsScreenManager.CollectCoinsScreenState screenState = CollectCoinsScreenManager.Instance.screenState;
		switch (screenState)
		{
		case CollectCoinsScreenManager.CollectCoinsScreenState.Offline:
			SetOfflineContent();
			break;
		case CollectCoinsScreenManager.CollectCoinsScreenState.Facebook:
		case CollectCoinsScreenManager.CollectCoinsScreenState.GameCenter:
		case CollectCoinsScreenManager.CollectCoinsScreenState.FacebookAndGamecenter:
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
		AddFacebookButtonToList(ref nElements);
		AddDummiesToList(ref nElements, false);
		AddOverlayAndLockList(true);
		SetupNoFriends(false);
		CrewHeader.text = "Collect";
	}

	private void SetupNoFriends(bool enable)
	{
		NoFriends.gameObject.SetActive(enable);
		NoFriends.alpha = ((!enable) ? 0f : 1f);
		if (enable)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				NoFriends.text = Strings.Get(StringID.NO_FRIENDS_FB_AND_GC);
			}
			else
			{
				NoFriends.text = Strings.Get(StringID.NO_FRIENDS_ONLY_FB);
			}
		}
	}

	private void SetOnlineContent(CollectCoinsScreenManager.CollectCoinsScreenState screenState)
	{
		Friend[] array = SocialManager.instance.FriendsSortedByCash();
		bool flag = screenState == CollectCoinsScreenManager.CollectCoinsScreenState.Facebook || screenState == CollectCoinsScreenManager.CollectCoinsScreenState.FacebookAndGamecenter;
		bool flag2 = screenState == CollectCoinsScreenManager.CollectCoinsScreenState.GameCenter && array.Count() >= 3;
		bool flag3 = (!flag2 && !flag) || screenState == CollectCoinsScreenManager.CollectCoinsScreenState.Offline;
		int nElements = 0;
		if (flag)
		{
			AddDummiesToList(ref nElements, true);
		}
		if (flag2 && !flag)
		{
			AddFacebookButtonToList(ref nElements);
		}
		AddFriendsToList(array, ref nElements, flag3);
		if (!flag2 && !flag)
		{
			AddFacebookButtonToList(ref nElements);
			AddDummiesToList(ref nElements, false);
		}
		AddOverlayAndLockList(flag3);
		if (flag && SocialManager.facebookDialogsSupported)
		{
			AddRecommendButtonToList(ref nElements);
		}
		AddSpaceToList();
		SetupNoFriends(!array.Any() && nElements == 1 && flag);
		CrewHeader.text = Strings.Get(StringID.FRIENDS_SCREEN_TITLE);
	}

	private void AddDummiesToList(ref int nElements, bool onlyCollectible)
	{
		Dictionary<int, CollectCoinsDummyData.CollectCoinsDummy> dummyData = CollectCoinsDummyData.GetDummyData();
		if (dummyData != null)
		{
			foreach (KeyValuePair<int, CollectCoinsDummyData.CollectCoinsDummy> item in dummyData)
			{
				if ((!onlyCollectible && ((item.Value.hasBeenCollected && item.Value.fakeProgress > 0) || !item.Value.hasBeenCollected)) || (onlyCollectible && !item.Value.hasBeenCollected))
				{
					_contentList.Add(new CollectCoinsCell
					{
						cellType = CollectCoinsCellType.DUMMYFRIEND,
						dummyFriend = item.Value,
						useDarkColour = (nElements % 2 == 0)
					});
					nElements++;
				}
			}
			return;
		}
		Debug.Log("Dummy datat not initialized. Aborting dummy initialization.");
	}

	private void AddOverlayAndLockList(bool enable)
	{
		_lockContent.SetActive(enable);
	}

	private void AddSpaceToList()
	{
		_contentList.Add(new CollectCoinsCell
		{
			cellType = CollectCoinsCellType.SPACE
		});
	}

	public void SetFBReference(UnityEngine.Object fbRef)
	{
		fbButtonReference = (GameObject)fbRef;
		_overlaySizer.topAnchor.target = fbButtonReference.transform;
	}

	private void AddFacebookButtonToList(ref int nElements)
	{
		_contentList.Add(new CollectCoinsCell
		{
			cellType = CollectCoinsCellType.FACEBOOK_BUTTON
		});
		nElements++;
	}

	private void AddFriendsToList(Friend[] friends, ref int nElement, bool lockList)
	{
		for (int i = 0; i < friends.Length; i++)
		{
			_contentList.Add(new CollectCoinsCell
			{
				cellType = CollectCoinsCellType.FRIEND,
				friend = friends[i],
				useDarkColour = (nElement % 2 == 0),
				disableScrolling = lockList
			});
			nElement++;
		}
	}

	private void AddRecommendButtonToList(ref int nElements)
	{
		_contentList.Add(new CollectCoinsCell
		{
			cellType = CollectCoinsCellType.RECOMMEND_BUTTON
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
