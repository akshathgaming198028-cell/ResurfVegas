using System;
using System.Collections.Generic;
using Kiloo.Common.UI;
using UnityEngine;
using com.kiloo.awards;

public class AwardsScreen : UIScreen
{
	public class AwardListAdapter : ListAdapter<AwardCell>
	{
		private const int AWARD_TITLE_PREFAB = 0;

		private const int AWARD_CELL_PREFAB = 1;

		private const int AWARD_FINISHED_CELL_PREFAB = 2;

		public GameObject finishedAwardPrefab;

		public GameObject awardPrefab;

		public GameObject titlePrefab;

		public AwardsScreen screenReference;

		public AwardListAdapter(List<AwardCell> awardList, GameObject awardCell, GameObject finishedAwardCell, GameObject titleCell, AwardsScreen screenReference)
			: base(awardList)
		{
			finishedAwardPrefab = finishedAwardCell;
			awardPrefab = awardCell;
			titlePrefab = titleCell;
			this.screenReference = screenReference;
		}

		public override int GetModelType(int index)
		{
			switch (GetItem(index).cellType)
			{
			case AwardCellType.AWARD:
				return 1;
			case AwardCellType.FINISHED_AWARD:
				return 2;
			case AwardCellType.TITLE:
				return 0;
			default:
				Debug.Log(string.Format("Award cell type '{0}' does not exist. Cell type 'AWARD_CELL_PREFAB' returned", GetItem(index).cellType));
				return 1;
			}
		}

		public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass)
		{
			Container container2 = container;
			AwardCell item = GetItem(dataIndex);
			if (container2 == null)
			{
				AwardCellReferences awardCellReferences = new AwardCellReferences();
				switch (item.cellType)
				{
				case AwardCellType.AWARD:
				{
					container2 = new Container(awardPrefab);
					AwardCellHelper component3 = container2.model.GetComponent<AwardCellHelper>();
					awardCellReferences.awardCellHelper = component3;
					container2.tag = awardCellReferences;
					break;
				}
				case AwardCellType.FINISHED_AWARD:
				{
					container2 = new Container(finishedAwardPrefab);
					AwardFinishedCellHelper component2 = container2.model.GetComponent<AwardFinishedCellHelper>();
					awardCellReferences.awardFinishedCellHelper = component2;
					container2.tag = awardCellReferences;
					break;
				}
				case AwardCellType.TITLE:
				{
					container2 = new Container(titlePrefab);
					ListTitleComponentHelper component = container2.model.GetComponent<ListTitleComponentHelper>();
					awardCellReferences.awarsTitleHelper = component;
					container2.tag = awardCellReferences;
					break;
				}
				}
			}
			if (!isMeasurePass)
			{
				AwardCellReferences awardCellReferences2 = (AwardCellReferences)container2.tag;
				switch (item.cellType)
				{
				case AwardCellType.AWARD:
					awardCellReferences2.awardCellHelper.InitCell(item.award, screenReference);
					break;
				case AwardCellType.FINISHED_AWARD:
					awardCellReferences2.awardFinishedCellHelper.InitCell(item.award);
					break;
				case AwardCellType.TITLE:
					awardCellReferences2.awarsTitleHelper.Setup(Strings.Get(StringID.AWARDS_LIST_HEADER), new Color(0f, 0.2901961f, 0.5019608f, 1f), -150f);
					break;
				}
			}
			return container2;
		}
	}

	[SerializeField]
	private GameObject _awardPrefab;

	[SerializeField]
	private GameObject _finishedAwardPrefab;

	[SerializeField]
	private GameObject _titlePrefab;

	[SerializeField]
	private ScrollView _awardListView;

	[SerializeField]
	private UIPanel _scrollPanel;

	[SerializeField]
	private UIPanel _scrollBarPanel;

	private int _indexOnHide;

	private AwardListAdapter _adapter;

	private List<AwardCell> _contentList = new List<AwardCell>();

	private bool _isSubscribedToChangedScreen;

	private bool _isReturningFromCelebration;

	private bool _containsCollectableAward;

	private bool _facebookLoginRunning;

	public override void Init()
	{
		base.Init();
		UIScreenController.OnNavigateToScreen += OnScreenChanged;
		OnScreenChanged("AwardsScreen");
		if (_contentList != null)
		{
			_adapter = new AwardListAdapter(_contentList, _awardPrefab, _finishedAwardPrefab, _titlePrefab, this);
		}
	}

	private void RefreshAwards()
	{
		_containsCollectableAward = SetListContent();
		if (_awardListView != null && _adapter != null)
		{
			_awardListView.SetAdapter(_adapter);
			if (!_containsCollectableAward)
			{
				_awardListView.SetSelection(_indexOnHide, ContainerPivot.Top, 0f);
			}
		}
	}

	public override void Show()
	{
		base.Show();
		PlayerInfo.Instance.hasShownAwardsIntroductionPopup = true;
		ForceUpdateAnchors();
		RefreshAwards();
	}

	public override void Hide()
	{
		base.Hide();
		_indexOnHide = _awardListView.GetFirstVisibleContainerOnScreen().dataIndex;
		if (UIScreenController.Instance.isShowingPopup)
		{
			return;
		}
		bool containsCollectibleAwards = false;
		foreach (Award sortedListAward in AwardsManager.Instance.GetSortedListAwards(out containsCollectibleAwards))
		{
			if (sortedListAward.isInPreview)
			{
				sortedListAward.isInPreview = false;
			}
		}
	}

	private void ForceUpdateAnchors()
	{
		_scrollPanel.ResetAnchors();
		_scrollPanel.topAnchor.rect.ForceUpdate();
		_scrollPanel.bottomAnchor.rect.ForceUpdate();
		_scrollPanel.leftAnchor.rect.ForceUpdate();
		_scrollPanel.rightAnchor.rect.ForceUpdate();
		_scrollPanel.UpdateAnchors();
	}

	public void OnScreenChanged(string screenName)
	{
		if (screenName != "AwardsScreen")
		{
			if (_isSubscribedToChangedScreen)
			{
				UIScreenController.Instance.OnPopupClosed -= PopupClosed;
				_isReturningFromCelebration = false;
				_isSubscribedToChangedScreen = false;
			}
		}
		else if (!_isSubscribedToChangedScreen)
		{
			UIScreenController.Instance.OnPopupClosed += PopupClosed;
			_isSubscribedToChangedScreen = true;
		}
	}

	private void PopupClosed(string popupName)
	{
		if (popupName == "CelebrationPopup")
		{
			_isReturningFromCelebration = true;
		}
	}

	public void FacebookLoginClicked()
	{
		HouseKeeper.SetFacebookPayoutAction(FacebookStatusChanged);
		_facebookLoginRunning = true;
		if (SocialManager.instance.FacebookLogin(OnFacebookLoginResult))
		{
			UIScreenController.Instance.FacebookLogIn(true);
		}
		_indexOnHide = _awardListView.GetFirstVisibleContainerOnScreen().dataIndex;
	}

	public void OnFacebookLoginResult(bool loggedIn)
	{
		if (!loggedIn && _facebookLoginRunning)
		{
			_facebookLoginRunning = false;
			QueueFacebookFailSlideIn(OnFailSlideInFinish);
		}
	}

	private void OnFailSlideInFinish()
	{
		FacebookStatusChanged(false);
	}

	private void QueueFacebookFailSlideIn(Action onFinishCallback)
	{
		UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Facebook, UISlideInFacebook.FacebookSlideInState.ConnectFailed, onFinishCallback));
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
		RefreshAwards();
	}

	private bool SetListContent()
	{
		_contentList.Clear();
		bool containsCollectibleAwards;
		List<Award> sortedListAwards = AwardsManager.Instance.GetSortedListAwards(out containsCollectibleAwards);
		_contentList.Add(new AwardCell
		{
			cellType = AwardCellType.TITLE
		});
		for (int i = 0; i < sortedListAwards.Count; i++)
		{
			_contentList.Add(new AwardCell
			{
				cellType = ((sortedListAwards[i].awardState != AwardState.Finished || sortedListAwards[i].isInPreview) ? AwardCellType.AWARD : AwardCellType.FINISHED_AWARD),
				award = sortedListAwards[i]
			});
		}
		return containsCollectibleAwards;
	}
}
