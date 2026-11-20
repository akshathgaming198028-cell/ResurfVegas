using System;
using System.Collections.Generic;
using Kiloo.Common.UI;
using UnityEngine;

public class ScrollListSelectPopup : UIScreen
{
	public class ScrollListSelectAdapter : ListAdapter<ScrollListPopupCell>
	{
		private const int DEBUGSETTINGS_TOGGLE_PREFAB = 0;

		private const int DEBUGSETTINGS_BUTTON_PREFAB = 1;

		private const int DEBUGSETTINGS_SPACE_PREFAB = 2;

		public ScrollListSelectPopup scrollListSettingsPopup;

		public GameObject _scrollListTogglePrefab;

		public GameObject _scrollListSpacePrefab;

		public ScrollListSelectAdapter(List<ScrollListPopupCell> contentList, GameObject scrollListTogglePrefab, GameObject scrollListSpacePrefab, ScrollListSelectPopup screenReference)
			: base(contentList)
		{
			_scrollListTogglePrefab = scrollListTogglePrefab;
			_scrollListSpacePrefab = scrollListSpacePrefab;
			scrollListSettingsPopup = screenReference;
		}

		public override int GetModelType(int index)
		{
			switch (GetItem(index).cellType)
			{
			case ScrollListPopupCellType.Toggle:
				return 0;
			case ScrollListPopupCellType.Space:
				return 2;
			default:
				return 0;
			}
		}

		public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass)
		{
			Container container2 = container;
			ScrollListPopupCell item = GetItem(dataIndex);
			if (container2 == null)
			{
				ScrollListPopupCellReferences scrollListPopupCellReferences = new ScrollListPopupCellReferences();
				switch (item.cellType)
				{
				case ScrollListPopupCellType.Toggle:
				{
					container2 = new Container(_scrollListTogglePrefab);
					ScrollListPopupCellHelper component = container2.model.GetComponent<ScrollListPopupCellHelper>();
					scrollListPopupCellReferences.scrollListPopopCellHelper = component;
					scrollListPopupCellReferences.uiEventListener = UIEventListener.Get(container2.model);
					scrollListPopupCellReferences.uiEventListener.parameter = container2;
					UIEventListener uiEventListener = scrollListPopupCellReferences.uiEventListener;
					uiEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uiEventListener.onClick, new UIEventListener.VoidDelegate(scrollListSettingsPopup.OnClickContainer));
					container2.tag = scrollListPopupCellReferences;
					break;
				}
				case ScrollListPopupCellType.Space:
					container2 = new Container(_scrollListSpacePrefab);
					container2.tag = scrollListPopupCellReferences;
					break;
				}
			}
			if (!isMeasurePass)
			{
				ScrollListPopupCellReferences scrollListPopupCellReferences2 = (ScrollListPopupCellReferences)container2.tag;
				switch (item.cellType)
				{
				case ScrollListPopupCellType.Toggle:
					scrollListPopupCellReferences2.scrollListPopopCellHelper.UpdateTitle(item.cellTitle);
					scrollListPopupCellReferences2.scrollListPopopCellHelper.cellID = item.cellID;
					scrollListPopupCellReferences2.scrollListPopopCellHelper.SetToggleIcon(scrollListSettingsPopup.indicators[item.cellID]);
					break;
				default:
					Debug.LogError("No cell type match for: " + item.cellType);
					break;
				case ScrollListPopupCellType.Space:
					break;
				}
			}
			return container2;
		}
	}

	private const int POPUP_MINIMUM_HEIGHT = 58;

	private const int POPUP_MAXIMUM_HEIGHT = 400;

	private const int CELL_HEIGHT_WITH_SPACE = 34;

	private const int FRAME_OFFSET = 13;

	[SerializeField]
	private GameObject ScrollListTogglePrefab;

	[SerializeField]
	private GameObject ScrollListSpacePrefab;

	[SerializeField]
	private UISprite _popupBackground;

	[SerializeField]
	private UISprite _popupFrame;

	private List<ScrollListPopupCell> _contentList = new List<ScrollListPopupCell>();

	private ScrollListSelectAdapter _adapter;

	[SerializeField]
	private ScrollView _scrollView;

	private Dictionary<string, string> items;

	private Dictionary<string, bool> indicators;

	private bool multiSelect;

	private Action<Dictionary<string, string>> callBack;

	private static ScrollListSelectPopup _instance;

	public static ScrollListSelectPopup instance
	{
		get
		{
			return _instance;
		}
	}

	public override void Init()
	{
		base.Init();
		_instance = this;
		if (_contentList != null)
		{
			_adapter = new ScrollListSelectAdapter(_contentList, ScrollListTogglePrefab, ScrollListSpacePrefab, this);
		}
		SetContentList(null);
	}

	public List<string> getSelected()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, bool> indicator in indicators)
		{
			if (indicator.Value)
			{
				list.Add(indicator.Key);
			}
		}
		return list;
	}

	public override void Show()
	{
		base.Show();
		RefreshScrollList();
	}

	public void Initialize(Dictionary<string, string> items, Action<Dictionary<string, string>> callback, List<string> selectedItems = null, bool multiSelect = false)
	{
		this.items = items;
		callBack = callback;
		this.multiSelect = multiSelect;
		indicators = new Dictionary<string, bool>();
		foreach (KeyValuePair<string, string> item in items)
		{
			if (selectedItems != null && selectedItems.Contains(item.Key))
			{
				indicators.Add(item.Key, true);
			}
			else
			{
				indicators.Add(item.Key, false);
			}
		}
		RefreshScrollList();
	}

	public override void Hide()
	{
		base.Hide();
		SendCallBack();
		ResetList();
	}

	private void SendCallBack()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (KeyValuePair<string, bool> indicator in indicators)
		{
			if (indicator.Value)
			{
				dictionary.Add(indicator.Key, items[indicator.Key]);
			}
		}
		callBack(dictionary);
	}

	private void SetContentList(Dictionary<string, string> items)
	{
		ResetList();
		if (items != null)
		{
			foreach (KeyValuePair<string, string> item in items)
			{
				InsertToggle(item.Value, item.Key);
				InsertSpace();
			}
		}
		if (_scrollView != null && _adapter != null)
		{
			_scrollView.SetAdapter(_adapter);
		}
	}

	private void InsertToggle(string title, string id)
	{
		_contentList.Add(new ScrollListPopupCell
		{
			cellType = ScrollListPopupCellType.Toggle,
			cellTitle = title,
			cellID = id
		});
	}

	private void InsertSpace()
	{
		_contentList.Add(new ScrollListPopupCell
		{
			cellType = ScrollListPopupCellType.Space
		});
	}

	private void ResetList()
	{
		_contentList.Clear();
	}

	public void OnClickContainer(GameObject go)
	{
		Container container = (Container)go.GetComponent<UIEventListener>().parameter;
		ScrollListPopupCellHelper scrollListPopopCellHelper = ((ScrollListPopupCellReferences)container.tag).scrollListPopopCellHelper;
		SelectItem(scrollListPopopCellHelper.cellID);
		RefreshScrollList();
	}

	private void SelectItem(string key)
	{
		if (!multiSelect)
		{
			foreach (string key2 in items.Keys)
			{
				indicators[key2] = false;
			}
		}
		indicators[key] = !indicators[key];
	}

	private void AdjustPopupSize()
	{
		int num = 58 + 34 * items.Count;
		int height = 58 + 34 * items.Count + 13;
		if (num < 400)
		{
			_popupBackground.height = num;
			_popupFrame.height = height;
		}
		else
		{
			_popupBackground.height = 400;
			_popupFrame.height = 413;
		}
	}

	private void RefreshScrollList()
	{
		if (items != null)
		{
			int position = 0;
			Container firstVisibleContainerOnScreen = _scrollView.GetFirstVisibleContainerOnScreen();
			if (firstVisibleContainerOnScreen != null)
			{
				position = firstVisibleContainerOnScreen.dataIndex;
			}
			SetContentList(items);
			_scrollView.SetSelection(position, ContainerPivot.Top, 0f);
			AdjustPopupSize();
		}
	}
}
