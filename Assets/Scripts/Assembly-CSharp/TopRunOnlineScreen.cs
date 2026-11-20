using System;
using System.Collections;
using System.Collections.Generic;
using Kiloo.Common.UI;
using TopRun;
using TopRun.Internal;
using UnityEngine;

public class TopRunOnlineScreen : UIScreen
{
	public class TopRunListAdapter : ListAdapter<TopRunCell>
	{
		private const int PLAYER_PREFAB = 0;

		private const int RANK_PREFAB = 1;

		private const int SPACE_CELL = 2;

		private const int EMPTY_CELL = 3;

		private const int FACEBOOK_BUTTON = 4;

		public GameObject playerPrefab;

		public GameObject rankHeaderPrefab;

		public GameObject spacePrefab;

		public GameObject emptyCell;

		public GameObject facebookButton;

		public TopRunListAdapter(List<TopRunCell> topRunListT, GameObject playerCell, GameObject headerCell, GameObject spaceCell, GameObject emptyCellPrefab, GameObject _facebookButton)
			: base(topRunListT)
		{
			playerPrefab = playerCell;
			rankHeaderPrefab = headerCell;
			spacePrefab = spaceCell;
			emptyCell = emptyCellPrefab;
			facebookButton = _facebookButton;
		}

		public void setNewDataList(List<TopRunCell> topRunListT)
		{
			base.dataList = topRunListT;
			NotifyDataSetChanged();
		}

		public override int GetModelType(int index)
		{
			if (GetItem(index).cellType == TopRunCellType.SPACE_CELL)
			{
				return 2;
			}
			if (GetItem(index).cellType == TopRunCellType.HEADER)
			{
				return 1;
			}
			if (GetItem(index).cellType == TopRunCellType.EMPTY_CELL)
			{
				return 3;
			}
			if (GetItem(index).cellType == TopRunCellType.FACEBOOK_BUTTON)
			{
				return 4;
			}
			return 0;
		}

		public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass)
		{
			Container container2 = container;
			TopRunCell topRunCell = GetItem(dataIndex);
			if (container2 == null)
			{
				TopRunCellReferences topRunCellReferences = new TopRunCellReferences();
				if (topRunCell.cellType == TopRunCellType.HEADER)
				{
					container2 = new Container(rankHeaderPrefab);
					RankHeader component = container2.model.GetComponent<RankHeader>();
					topRunCellReferences.rankHeaderScript = component;
					container2.tag = topRunCellReferences;
				}
				else if (topRunCell.cellType == TopRunCellType.SPACE_CELL)
				{
					container2 = new Container(spacePrefab);
				}
				else if (topRunCell.cellType == TopRunCellType.EMPTY_CELL)
				{
					container2 = new Container(emptyCell);
					TopRunEmptyCell component2 = container2.model.GetComponent<TopRunEmptyCell>();
					topRunCellReferences.topRunEmptyCellScript = component2;
					container2.tag = topRunCellReferences;
				}
				else if (topRunCell.cellType == TopRunCellType.FACEBOOK_BUTTON)
				{
					container2 = new Container(facebookButton);
					FacebookLoginButtonOnline component3 = container2.model.GetComponent<FacebookLoginButtonOnline>();
					ResizeWidgetOnLoginFB_TopRun component4 = container2.model.GetComponent<ResizeWidgetOnLoginFB_TopRun>();
					topRunCellReferences.facebookLoginButtonScript = component3;
					topRunCellReferences.resizeWidgetOnLoginFBScript = component4;
					container2.tag = topRunCellReferences;
				}
				else
				{
					container2 = new Container(playerPrefab);
					TopRunPlayerCell topRunPlayerCellScript = (topRunCellReferences.topRunPlayerCellScript = container2.model.GetComponent<TopRunPlayerCell>());
					topRunCell.topRunPlayerCellScript = topRunPlayerCellScript;
					container2.tag = topRunCellReferences;
					container2.onRecycleContainer = delegate
					{
						if (topRunCell.OnUpdateCell != null)
						{
							topRunCell.OnUpdateCell = null;
						}
					};
				}
			}
			if (topRunCell.cellType == TopRunCellType.HEADER)
			{
				TopRunCellReferences topRunCellReferences2 = (TopRunCellReferences)container2.tag;
				topRunCellReferences2.rankHeaderScript.InitHeader(topRunCell.rank, topRunCell.bestScore);
			}
			else if (topRunCell.cellType != TopRunCellType.SPACE_CELL)
			{
				if (topRunCell.cellType == TopRunCellType.PLAYER)
				{
					TopRunCellReferences topRunCellReferences3 = (TopRunCellReferences)container2.tag;
					topRunCellReferences3.topRunPlayerCellScript.InitCell(topRunCell.player, topRunCell.useDarkColour, topRunCell.rank, topRunCell.isLocalUser);
					if (!isMeasurePass)
					{
						topRunCell.OnUpdateCell = topRunCellReferences3.topRunPlayerCellScript.Refresh;
					}
				}
				else if (topRunCell.cellType == TopRunCellType.FACEBOOK_BUTTON)
				{
					TopRunCellReferences topRunCellReferences4 = (TopRunCellReferences)container2.tag;
					topRunCellReferences4.facebookLoginButtonScript.Init(SocialManager.instance.facebookIsLoggedIn, true, false, SocialManager.instance.gamecenterIsLoggedIn, true);
					if (!topRunCell.noPadding)
					{
						topRunCellReferences4.resizeWidgetOnLoginFBScript.SetEmptyWidgetSize(0, 0);
					}
					else
					{
						topRunCellReferences4.resizeWidgetOnLoginFBScript.SetEmptyWidgetSize(284, 50);
					}
					container2.isMeasured = false;
				}
				else
				{
					TopRunCellReferences topRunCellReferences5 = (TopRunCellReferences)container2.tag;
					topRunCellReferences5.topRunEmptyCellScript.InitCell(topRunCell.rank);
				}
			}
			return container2;
		}
	}

	private const int NORMAL_STATE_NORMAL_OFFSET = -9;

	private const int NORMAL_STATE_RESULTS_OFFSET = -110;

	private const int SCROLL_PANEL_NORMAL_OFFSET = -5;

	private const int SCROLL_PANEL_RESULTS_OFFSET = -15;

	[SerializeField]
	private ScrollView _topRunListView;

	[SerializeField]
	private UILabel _titleLabel;

	[SerializeField]
	private UISprite _flagSrite;

	[SerializeField]
	private UILabel _countryLabel;

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

	[SerializeField]
	private UIPanel _scrollBarPanel;

	private TopRunListAdapter _adapter;

	private TopRunStates _currentState = TopRunStates.OFFLINE;

	private bool _isLoading = true;

	private List<TopRunCell> _contentList = new List<TopRunCell>();

	private int noMedalRankThreshold = 4;

	public override void Init()
	{
		base.Init();
		SetListContent();
		if (_contentList != null)
		{
			_adapter = new TopRunListAdapter(_contentList, _playerPrefab, _rankHeaderPrefab, _spaceCell, _emptyCell, _facebookLogInButton);
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
			SetScreenTop_UI();
			ForceUpdateAnchors();
		}
		ScrollToMyPosition();
		loadingOverlay.SetActive(false);
		if (!UIScreenController.Instance.justPaidOutTopRun)
		{
			StartCoroutine(AnimateMedalAfterSeconds(0.5f));
		}
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
			else
			{
				Debug.LogWarning("No Player Script found on displayed list elements.");
			}
		}
		else
		{
			Debug.Log("Index incorrect");
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

	private void OnEnable()
	{
		TopRunEventsHandler.instance.OnTopRunChangedEvent += TournamentChanged;
		TopRunRoot.manager.playerInfoChanged += OnPlayerChanged;
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
		StartCoroutine(AnimateMedalAfterSeconds(0.5f));
	}

	private void OnDisable()
	{
		TopRunEventsHandler.instance.OnTopRunChangedEvent -= TournamentChanged;
		TopRunRoot.manager.playerInfoChanged -= OnPlayerChanged;
		UIScreenController.OnApplicationResumed -= OnApplicationResumed;
		loadingOverlay.SetActive(false);
		UIScreenController.Instance.justPaidOutTopRun = false;
	}

	public virtual void OnPlayerChanged(object o, PlayerInfoChangedEventArgs eventArg = null)
	{
		if (eventArg == null)
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

	public override void Show()
	{
		base.Show();
		SetScreenTop_UI();
		ForceUpdateAnchors();
		if (_topRunListView != null && _adapter != null)
		{
			_topRunListView.SetAdapter(_adapter);
		}
		ScrollToMyPosition();
	}

	private void ScrollToMyPosition()
	{
		int myIndexInTheList = GetMyIndexInTheList();
		if (myIndexInTheList >= 0)
		{
			float offset = _topRunListView.rootPanel.baseClipRegion.w * 0.5f - _topRunListView.GetFirstVisibleContainerOnScreen().height;
			_topRunListView.SetSelection(myIndexInTheList, ContainerPivot.Top, offset);
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

	private void SetCurrentState()
	{
		IManager manager = TopRunRoot.manager;
		if (manager != null && manager.tournament != null)
		{
			if (TopRunRewardHelper.ShouldShowResults())
			{
				_currentState = TopRunStates.RESULTS;
				_normalState.GetComponent<UIWidget>().topAnchor.absolute = -110;
				_scrollPanel.topAnchor.target = _normalState.transform;
				_scrollPanel.topAnchor.absolute = -15;
				_resultState.SetActive(true);
				_normalState.SetActive(true);
				_offlineState.SetActive(false);
				TopRunNormalStateHelper componentInChildren = _normalState.GetComponentInChildren<TopRunNormalStateHelper>();
				if (componentInChildren != null)
				{
					componentInChildren.InitNormalState();
				}
			}
			else
			{
				_currentState = TopRunStates.NORMAL;
				_normalState.GetComponent<UIWidget>().topAnchor.absolute = -9;
				_scrollPanel.topAnchor.target = _normalState.transform;
				_scrollPanel.topAnchor.absolute = -5;
				_resultState.SetActive(false);
				_normalState.SetActive(true);
				_offlineState.SetActive(false);
				TopRunNormalStateHelper componentInChildren2 = _normalState.GetComponentInChildren<TopRunNormalStateHelper>();
				if (componentInChildren2 != null)
				{
					componentInChildren2.InitNormalState();
				}
			}
		}
		else
		{
			_currentState = TopRunStates.OFFLINE;
			_scrollPanel.topAnchor.target = _offlineState.transform;
			_scrollPanel.topAnchor.absolute = -5;
			_resultState.SetActive(false);
			_normalState.SetActive(false);
			_offlineState.SetActive(true);
			TopRunOfflineStateHelper componentInChildren3 = _offlineState.GetComponentInChildren<TopRunOfflineStateHelper>();
			if (componentInChildren3 != null)
			{
				componentInChildren3.SetState(_isLoading ? TopRunOfflineStateHelper.OfflineState.Loading : TopRunOfflineStateHelper.OfflineState.TryAgain, this);
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

	private void SetScreenTop_UI()
	{
		_titleLabel.text = Strings.Get(StringID.TOPRUN_SCREEN_TITLE);
		_titleLabel.color = GlobalColors.TOPRUN_SCREEN_TITLE_COLOUR;
		_countryLabel.color = GlobalColors.TOPRUN_COUNTRY_NAME_COLOUR;
		_flagSrite.spriteName = CountryManager.instance.GetCurrentTournamentCountry().flag;
		_countryLabel.text = CountryManager.instance.GetCurrentTournamentCountry().name.ToUpper();
	}

	private void SetListContent()
	{
		IManager manager = TopRunRoot.manager;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		Player player = null;
		int num = -1;
		if (_currentState == TopRunStates.NORMAL || _currentState == TopRunStates.RESULTS)
		{
			if (manager.score > 0)
			{
				flag2 = true;
				flag3 = false;
				num = manager.tournament.GetRankForScore(manager.score);
				player = GetMyPlayerProfile();
			}
			else
			{
				flag3 = true;
				player = GetMyPlayerProfile(false);
				num = noMedalRankThreshold;
			}
			_contentList.Clear();
			int num2 = 0;
			for (int i = 0; i < manager.tournament.rankCount; i++)
			{
				List<IPlayer> list = new List<IPlayer>(manager.tournament.GetPlayersForRank(i));
				if (i >= noMedalRankThreshold && list.Count <= 0 && !flag3 && manager.tournament.GetRankForScore(manager.score) < noMedalRankThreshold)
				{
					continue;
				}
				int num3 = 0;
				if (i <= noMedalRankThreshold)
				{
					if (i != 0 || (i == 0 && flag))
					{
						_contentList.Add(new TopRunCell
						{
							cellType = TopRunCellType.SPACE_CELL
						});
					}
					_contentList.Add(new TopRunCell
					{
						cellType = TopRunCellType.HEADER,
						rank = i + 1,
						bestScore = manager.tournament.GetBestScoreForRank(i)
					});
				}
				else
				{
					num3 = num2;
				}
				if (list.Count <= 0)
				{
					if (num == i && flag2)
					{
						_contentList.Add(new TopRunCell
						{
							cellType = TopRunCellType.PLAYER,
							player = player,
							useDarkColour = true,
							rank = i + 1,
							isLocalUser = true
						});
						AddFBButtonAfterMe(_contentList, flag);
						flag2 = false;
					}
					else if (i <= noMedalRankThreshold && (i != noMedalRankThreshold || !flag3))
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
						if (flag2 && num == i && player.score >= list[j].score)
						{
							_contentList.Add(new TopRunCell
							{
								cellType = TopRunCellType.PLAYER,
								player = player,
								useDarkColour = (num3 % 2 == 0),
								rank = i + 1,
								isLocalUser = true
							});
							AddFBButtonAfterMe(_contentList, flag);
							flag2 = false;
							num3++;
						}
						_contentList.Add(new TopRunCell
						{
							cellType = TopRunCellType.PLAYER,
							player = list[j],
							useDarkColour = (num3 % 2 == 0),
							rank = i + 1
						});
						num3++;
					}
				}
				if (i == num && flag2)
				{
					_contentList.Add(new TopRunCell
					{
						cellType = TopRunCellType.PLAYER,
						player = player,
						useDarkColour = (num3 % 2 == 0),
						rank = i + 1,
						isLocalUser = true
					});
					flag2 = false;
					AddFBButtonAfterMe(_contentList, flag);
				}
				num2 = num3;
			}
			if (flag3)
			{
				_contentList.Add(new TopRunCell
				{
					cellType = TopRunCellType.PLAYER,
					player = player,
					useDarkColour = (num2 % 2 == 0),
					rank = num + 1,
					isLocalUser = true
				});
				AddFBButtonAfterMe(_contentList, flag);
			}
			_contentList.Add(new TopRunCell
			{
				cellType = TopRunCellType.SPACE_CELL
			});
			return;
		}
		_contentList.Clear();
		for (int k = 0; k < noMedalRankThreshold; k++)
		{
			if (k != 0)
			{
				_contentList.Add(new TopRunCell
				{
					cellType = TopRunCellType.SPACE_CELL
				});
			}
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

	private void AddFBButtonAfterMe(List<TopRunCell> _contentList, bool isFacebookButtonAdded)
	{
		if (!isFacebookButtonAdded && !SocialManager.instance.facebookIsLoggedIn)
		{
			_contentList.Add(new TopRunCell
			{
				cellType = TopRunCellType.FACEBOOK_BUTTON,
				noPadding = true
			});
			isFacebookButtonAdded = true;
		}
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
}
