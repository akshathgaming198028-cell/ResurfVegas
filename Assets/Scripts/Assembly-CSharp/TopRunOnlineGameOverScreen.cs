using System;
using System.Collections;
using System.Collections.Generic;
using Kiloo.Common.UI;
using TopRun;
using TopRun.Internal;
using UnityEngine;

public class TopRunOnlineGameOverScreen : MonoBehaviour
{
	public class TopRunListAdapter : ListAdapter<TopRunCell>
	{
		private const int PLAYER_PREFAB_ID = 0;

		private const int RANK_PREFAB_ID = 1;

		private const int SPACE_CELL = 2;

		private const int EMPTY_CELL_ID = 3;

		private const int NORMAL_STATE_ID = 4;

		private const int RESULT_STATE_ID = 5;

		private const int OFFLINE_STATE_ID = 6;

		private const int FACEBOOK_BUTTON = 7;

		private TopRunOnlineGameOverScreen _topRunScreen;

		public GameObject playerPrefab;

		public GameObject rankHeaderPrefab;

		public GameObject spacePrefab;

		public GameObject emptyCellPrefab;

		public GameObject facebookButton;

		public GameObject normalStatePrefab;

		public GameObject resultStatePrefab;

		public GameObject offlineStatePrefab;

		public TopRunListAdapter(List<TopRunCell> topRunListT, TopRunOnlineGameOverScreen topRunScreen, GameObject playerCell, GameObject headerCell, GameObject spaceCell, GameObject emptyCell, GameObject normalState, GameObject resultState, GameObject offlineState, GameObject _facebookButton)
			: base(topRunListT)
		{
			_topRunScreen = topRunScreen;
			playerPrefab = playerCell;
			rankHeaderPrefab = headerCell;
			spacePrefab = spaceCell;
			normalStatePrefab = normalState;
			resultStatePrefab = resultState;
			offlineStatePrefab = offlineState;
			emptyCellPrefab = emptyCell;
			facebookButton = _facebookButton;
		}

		public override int GetModelType(int index)
		{
			if (GetItem(index).cellType == TopRunCellType.HEADER)
			{
				return 1;
			}
			if (GetItem(index).cellType == TopRunCellType.PLAYER)
			{
				return 0;
			}
			if (GetItem(index).cellType == TopRunCellType.SPACE_CELL)
			{
				return 2;
			}
			if (GetItem(index).cellType == TopRunCellType.STATE_NORMAL)
			{
				return 4;
			}
			if (GetItem(index).cellType == TopRunCellType.STATE_RESULTS)
			{
				return 5;
			}
			if (GetItem(index).cellType == TopRunCellType.STATE_OFFLINE)
			{
				return 6;
			}
			if (GetItem(index).cellType == TopRunCellType.FACEBOOK_BUTTON)
			{
				return 7;
			}
			return 3;
		}

		public void setNewDataList(List<TopRunCell> topRunListT)
		{
			base.dataList = topRunListT;
			NotifyDataSetChanged();
		}

		public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass)
		{
			Container container2 = container;
			TopRunCell item = GetItem(dataIndex);
			if (container2 == null)
			{
				TopRunCellReferences topRunCellReferences = new TopRunCellReferences();
				if (item.cellType == TopRunCellType.HEADER)
				{
					container2 = new Container(rankHeaderPrefab);
					RankHeader component = container2.model.GetComponent<RankHeader>();
					topRunCellReferences.rankHeaderScript = component;
					container2.tag = topRunCellReferences;
				}
				else if (item.cellType == TopRunCellType.PLAYER)
				{
					container2 = new Container(playerPrefab);
					item.topRunPlayerCellScript = (topRunCellReferences.topRunPlayerCellScript = container2.model.GetComponent<TopRunPlayerCell>());
					container2.tag = topRunCellReferences;
				}
				else if (item.cellType == TopRunCellType.STATE_NORMAL)
				{
					container2 = new Container(normalStatePrefab);
					TopRunNormalStateHelper componentInChildren = container2.model.GetComponentInChildren<TopRunNormalStateHelper>();
					topRunCellReferences.topRunNormalStateScript = componentInChildren;
					container2.tag = topRunCellReferences;
				}
				else if (item.cellType == TopRunCellType.STATE_RESULTS)
				{
					container2 = new Container(resultStatePrefab);
				}
				else if (item.cellType == TopRunCellType.SPACE_CELL)
				{
					container2 = new Container(spacePrefab);
				}
				else if (item.cellType == TopRunCellType.STATE_OFFLINE)
				{
					container2 = new Container(offlineStatePrefab);
					TopRunOfflineStateHelper componentInChildren2 = container2.model.GetComponentInChildren<TopRunOfflineStateHelper>();
					topRunCellReferences.topRunOfflineStateScript = componentInChildren2;
					container2.tag = topRunCellReferences;
				}
				else if (item.cellType == TopRunCellType.FACEBOOK_BUTTON)
				{
					container2 = new Container(facebookButton);
					FacebookLoginButtonOnline component2 = container2.model.GetComponent<FacebookLoginButtonOnline>();
					ResizeWidgetOnLoginFB_TopRun component3 = container2.model.GetComponent<ResizeWidgetOnLoginFB_TopRun>();
					topRunCellReferences.facebookLoginButtonScript = component2;
					topRunCellReferences.resizeWidgetOnLoginFBScript = component3;
					container2.tag = topRunCellReferences;
				}
				else
				{
					container2 = new Container(emptyCellPrefab);
					TopRunEmptyCell component4 = container2.model.GetComponent<TopRunEmptyCell>();
					topRunCellReferences.topRunEmptyCellScript = component4;
					container2.tag = topRunCellReferences;
				}
			}
			if (item.cellType == TopRunCellType.HEADER)
			{
				TopRunCellReferences topRunCellReferences2 = (TopRunCellReferences)container2.tag;
				topRunCellReferences2.rankHeaderScript.InitHeader(item.rank, item.bestScore);
			}
			else if (item.cellType == TopRunCellType.PLAYER)
			{
				TopRunCellReferences topRunCellReferences3 = (TopRunCellReferences)container2.tag;
				topRunCellReferences3.topRunPlayerCellScript.InitCell(item.player, item.useDarkColour, item.rank, item.isLocalUser);
				item.OnUpdateCell = topRunCellReferences3.topRunPlayerCellScript.Refresh;
			}
			else if (item.cellType == TopRunCellType.STATE_NORMAL)
			{
				TopRunCellReferences topRunCellReferences4 = (TopRunCellReferences)container2.tag;
				topRunCellReferences4.topRunNormalStateScript.InitNormalState();
			}
			else if (item.cellType != TopRunCellType.STATE_RESULTS && item.cellType != TopRunCellType.SPACE_CELL)
			{
				if (item.cellType == TopRunCellType.STATE_OFFLINE)
				{
					TopRunCellReferences topRunCellReferences5 = (TopRunCellReferences)container2.tag;
					topRunCellReferences5.topRunOfflineStateScript.SetState(_topRunScreen._isLoading ? TopRunOfflineStateHelper.OfflineState.Loading : TopRunOfflineStateHelper.OfflineState.TryAgain, null, _topRunScreen);
				}
				else if (item.cellType == TopRunCellType.EMPTY_CELL)
				{
					TopRunCellReferences topRunCellReferences6 = (TopRunCellReferences)container2.tag;
					topRunCellReferences6.topRunEmptyCellScript.InitCell(item.rank);
				}
				else if (item.cellType == TopRunCellType.FACEBOOK_BUTTON)
				{
					TopRunCellReferences topRunCellReferences7 = (TopRunCellReferences)container2.tag;
					topRunCellReferences7.facebookLoginButtonScript.Init(SocialManager.instance.facebookIsLoggedIn, true, false, SocialManager.instance.gamecenterIsLoggedIn, true, false, true);
					if (!item.noPadding)
					{
						topRunCellReferences7.resizeWidgetOnLoginFBScript.SetEmptyWidgetSize(0, 0);
					}
					else
					{
						topRunCellReferences7.resizeWidgetOnLoginFBScript.SetEmptyWidgetSize(284, 50);
					}
					container2.isMeasured = false;
				}
			}
			return container2;
		}
	}

	private const int _offsetFromTop = -39;

	[SerializeField]
	private ScrollView _topRunListView;

	[SerializeField]
	private GameObject _playerPrefab;

	[SerializeField]
	private GameObject _rankHeaderPrefab;

	[SerializeField]
	private GameObject _spaceCell;

	[SerializeField]
	private GameObject _emptyCell;

	[SerializeField]
	private GameObject _facebookLogInButton;

	[SerializeField]
	private GameObject _normalState;

	[SerializeField]
	private GameObject _resultState;

	[SerializeField]
	private GameObject _offlineState;

	[SerializeField]
	public GameObject loadingOverlay;

	[SerializeField]
	private UIPanel _scrollPanel;

	private TopRunListAdapter _adapter;

	private TopRunStates _currentState = TopRunStates.OFFLINE;

	private bool _hasBeenInitialized;

	private bool _isLoading = true;

	private List<TopRunCell> _contentList = new List<TopRunCell>();

	private void Init()
	{
		SetListContent();
		if (_contentList != null)
		{
			_adapter = new TopRunListAdapter(_contentList, this, _playerPrefab, _rankHeaderPrefab, _spaceCell, _emptyCell, _normalState, _resultState, _offlineState, _facebookLogInButton);
		}
		SetScrollPanelTopAnchor();
		SetAdapter();
		_hasBeenInitialized = true;
	}

	private void OnEnable()
	{
		TopRunRoot.manager.playerInfoChanged += OnPlayerChanged;
		TopRunEventsHandler.instance.OnTopRunChangedEvent += TournamentChanged;
		UIScreenController.OnApplicationResumed += OnApplicationResumed;
		_isLoading = true;
		if (!UIScreenController.Instance.justPaidOutTopRun)
		{
			loadingOverlay.SetActive(true);
		}
		SetCurrentState();
		if (!UIScreenController.Instance.justPaidOutTopRun)
		{
			RefreshOrFetchNewTournament();
		}
		if (!_hasBeenInitialized)
		{
			Init();
		}
		else
		{
			SetAdapter();
		}
		StartCoroutine(AnimateMedalAfterSeconds(0.5f));
	}

	private void OnDisable()
	{
		TopRunRoot.manager.playerInfoChanged -= OnPlayerChanged;
		TopRunEventsHandler.instance.OnTopRunChangedEvent -= TournamentChanged;
		UIScreenController.OnApplicationResumed -= OnApplicationResumed;
		loadingOverlay.SetActive(false);
		UIScreenController.Instance.justPaidOutTopRun = false;
	}

	private void OnApplicationResumed()
	{
		_isLoading = true;
		if (!UIScreenController.Instance.justPaidOutTopRun)
		{
			loadingOverlay.SetActive(true);
		}
		SetCurrentState();
		if (!UIScreenController.Instance.justPaidOutTopRun)
		{
			RefreshOrFetchNewTournament();
		}
		if (!_hasBeenInitialized)
		{
			Init();
		}
		else
		{
			SetAdapter();
		}
	}

	public virtual void OnPlayerChanged(object o, PlayerInfoChangedEventArgs eventArg = null)
	{
		if (eventArg != null)
		{
			return;
		}
		IPlayer player = eventArg.player;
		foreach (TopRunCell content in _contentList)
		{
			if (content.player != null && content.player.publicKey == player.publicKey)
			{
				content.player = player;
				content.UpdateCell();
				break;
			}
		}
	}

	private void SetAdapter()
	{
		if (_topRunListView != null && _adapter != null)
		{
			_topRunListView.SetAdapter(_adapter);
		}
		ScrollToRightPosition();
	}

	private void ScrollToRightPosition()
	{
		int num = GetMyIndexInTheList();
		if (_currentState == TopRunStates.RESULTS)
		{
			num = 0;
		}
		if (num >= 0)
		{
			float offset = _topRunListView.rootPanel.baseClipRegion.w * 0.5f - _topRunListView.GetFirstVisibleContainerOnScreen().height;
			_topRunListView.SetSelection(num, ContainerPivot.Top, offset);
		}
	}

	private void TournamentChanged(object sender, EventArgs eventArg)
	{
		if (base.gameObject.activeSelf && base.gameObject.activeInHierarchy && !TopRunRoot.manager.isRefreshing)
		{
			_isLoading = false;
			TopRunRewardHelper.RefreshPendingResults();
			SetCurrentState();
			RefreshAdapterIfDataChanged();
		}
	}

	private void RefreshAdapterIfDataChanged()
	{
		if (_adapter != null)
		{
			SetListContent();
			if (_contentList != null)
			{
				_adapter.setNewDataList(_contentList);
			}
		}
		ScrollToRightPosition();
		loadingOverlay.SetActive(false);
		if (!UIScreenController.Instance.justPaidOutTopRun)
		{
			StartCoroutine(AnimateMedalAfterSeconds(0.5f));
		}
	}

	public void AnimateMedalIfNeeded()
	{
		StartCoroutine(AnimateMedalAfterSeconds(0.5f));
	}

	private IEnumerator AnimateMedalAfterSeconds(float seconds)
	{
		UIScreenController.Instance.justPaidOutTopRun = false;
		yield return new WaitForSeconds(seconds);
		int indexToScroll = GetMyIndexInTheList();
		if (indexToScroll >= 0)
		{
			TopRunPlayerCell playerCellScript = GetPlayerScriptFromList();
			if (playerCellScript != null)
			{
				playerCellScript.AnimateMedalIfNeeded();
			}
		}
	}

	private TopRunPlayerCell GetPlayerScriptFromList()
	{
		foreach (TopRunCell data in _adapter.dataList)
		{
			if (data.topRunPlayerCellScript != null && data.topRunPlayerCellScript.IsLocalUser())
			{
				return data.topRunPlayerCellScript;
			}
		}
		return null;
	}

	private void RefreshOrFetchNewTournament()
	{
		IManager manager = TopRunRoot.manager;
		bool flag = true;
		TopRunRewardHelper.RefreshPendingResults();
		if (manager != null)
		{
			if (manager.tournament != null && manager.nextTournamentFetchTime != DateTime.MinValue && manager.nextTournamentFetchTime < DateTime.UtcNow && manager.tournament.hasResult)
			{
				manager.FetchNextTournament();
				flag = false;
			}
			if (flag)
			{
				manager.RefreshTournament();
			}
		}
	}

	private void SetScrollPanelTopAnchor()
	{
		_scrollPanel.topAnchor.absolute = -39;
		_scrollPanel.topAnchor.relative = 0f;
		_scrollPanel.topAnchor.Set(0f, -39f);
		_scrollPanel.ResetAnchors();
		_scrollPanel.UpdateAnchors();
	}

	private void SetCurrentState()
	{
		IManager manager = TopRunRoot.manager;
		if (manager != null && manager.tournament != null)
		{
			if (TopRunRewardHelper.ShouldShowResults())
			{
				_currentState = TopRunStates.RESULTS;
			}
			else
			{
				_currentState = TopRunStates.NORMAL;
			}
		}
		else
		{
			_currentState = TopRunStates.OFFLINE;
		}
	}

	private int GetMyIndexInTheList()
	{
		int result = -1;
		if (_contentList != null)
		{
			for (int i = 0; i < _contentList.Count; i++)
			{
				if (_contentList[i].cellType == TopRunCellType.PLAYER && _contentList[i].isLocalUser)
				{
					result = i;
				}
			}
		}
		return result;
	}

	private Player GetMyPlayerProfile(bool hasScore = true)
	{
		Player player = new Player();
		player.score = (hasScore ? TopRunRoot.manager.score : 0);
		player.isFriend = true;
		if (SocialManager.instance.facebookState == SocialManager.FacebookState.LoggedIn && SocialManager.instance.facebookProfile != null)
		{
			player.facebookProfile = SocialManager.instance.facebookProfile;
		}
		return player;
	}

	private void SetListContent()
	{
		IManager manager = TopRunRoot.manager;
		Player player = null;
		bool flag = false;
		bool flag2 = false;
		bool isFacebookButtonAdded = false;
		int num = 4;
		int num2 = -1;
		if (_currentState == TopRunStates.NORMAL || _currentState == TopRunStates.RESULTS)
		{
			_contentList.Clear();
			if (_currentState == TopRunStates.RESULTS)
			{
				_contentList.Add(new TopRunCell
				{
					cellType = TopRunCellType.STATE_RESULTS
				});
				_contentList.Add(new TopRunCell
				{
					cellType = TopRunCellType.SPACE_CELL
				});
			}
			_contentList.Add(new TopRunCell
			{
				cellType = TopRunCellType.STATE_NORMAL
			});
			if (manager.score > 0)
			{
				flag = true;
				flag2 = false;
				num2 = manager.tournament.GetRankForScore(manager.score);
				player = GetMyPlayerProfile();
			}
			else
			{
				flag2 = true;
				player = GetMyPlayerProfile(false);
				num2 = 4;
			}
			int num3 = 0;
			for (int i = 0; i < manager.tournament.rankCount; i++)
			{
				List<IPlayer> list = new List<IPlayer>(manager.tournament.GetPlayersForRank(i));
				if (i >= num && list.Count <= 0 && !flag2 && manager.tournament.GetRankForScore(manager.score) < num)
				{
					continue;
				}
				int num4 = 0;
				if (i <= num)
				{
					_contentList.Add(new TopRunCell
					{
						cellType = TopRunCellType.SPACE_CELL
					});
					_contentList.Add(new TopRunCell
					{
						cellType = TopRunCellType.HEADER,
						rank = i + 1,
						bestScore = manager.tournament.GetBestScoreForRank(i)
					});
				}
				else
				{
					num4 = num3;
				}
				if (list.Count <= 0)
				{
					if (num2 == i && flag)
					{
						_contentList.Add(new TopRunCell
						{
							cellType = TopRunCellType.PLAYER,
							player = player,
							useDarkColour = true,
							rank = i + 1,
							isLocalUser = true
						});
						flag = false;
						AddFBButtonAfterMe(_contentList, isFacebookButtonAdded);
					}
					else if (i <= num && (i != num || !flag2))
					{
						_contentList.Add(new TopRunCell
						{
							cellType = TopRunCellType.EMPTY_CELL,
							rank = i + 1
						});
					}
				}
				else
				{
					for (int j = 0; j < list.Count; j++)
					{
						if (flag && num2 == i && player.score >= list[j].score)
						{
							_contentList.Add(new TopRunCell
							{
								cellType = TopRunCellType.PLAYER,
								player = player,
								useDarkColour = (num4 % 2 == 0),
								rank = i + 1,
								isLocalUser = true
							});
							AddFBButtonAfterMe(_contentList, isFacebookButtonAdded);
							flag = false;
							num4++;
						}
						_contentList.Add(new TopRunCell
						{
							cellType = TopRunCellType.PLAYER,
							player = list[j],
							useDarkColour = (num4 % 2 == 0),
							rank = i + 1
						});
						num4++;
					}
				}
				if (i == num2 && flag)
				{
					_contentList.Add(new TopRunCell
					{
						cellType = TopRunCellType.PLAYER,
						player = player,
						useDarkColour = (num4 % 2 == 0),
						rank = i + 1,
						isLocalUser = true
					});
					AddFBButtonAfterMe(_contentList, isFacebookButtonAdded);
					flag = false;
				}
				num3 = num4;
			}
			if (flag2)
			{
				_contentList.Add(new TopRunCell
				{
					cellType = TopRunCellType.PLAYER,
					player = player,
					useDarkColour = (num3 % 2 == 0),
					rank = num2 + 1,
					isLocalUser = true
				});
				AddFBButtonAfterMe(_contentList, isFacebookButtonAdded);
			}
			_contentList.Add(new TopRunCell
			{
				cellType = TopRunCellType.SPACE_CELL
			});
		}
		else
		{
			_contentList.Clear();
			_contentList.Add(new TopRunCell
			{
				cellType = TopRunCellType.STATE_OFFLINE
			});
			for (int k = 0; k < num; k++)
			{
				_contentList.Add(new TopRunCell
				{
					cellType = TopRunCellType.SPACE_CELL
				});
				_contentList.Add(new TopRunCell
				{
					cellType = TopRunCellType.HEADER,
					rank = k + 1,
					bestScore = -1
				});
				_contentList.Add(new TopRunCell
				{
					cellType = TopRunCellType.EMPTY_CELL,
					rank = k + 1
				});
			}
			_contentList.Add(new TopRunCell
			{
				cellType = TopRunCellType.SPACE_CELL
			});
		}
	}

	private void AddFBButtonAfterMe(List<TopRunCell> _topRunContentList, bool isFacebookButtonAdded)
	{
		if (!isFacebookButtonAdded && !SocialManager.instance.facebookIsLoggedIn)
		{
			_topRunContentList.Add(new TopRunCell
			{
				cellType = TopRunCellType.FACEBOOK_BUTTON,
				noPadding = true
			});
			isFacebookButtonAdded = true;
		}
	}
}
