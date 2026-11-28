using System.Collections.Generic;
using Kiloo.Plugins.DeviceUtility;
using UnityEngine;

public class SettingsPopup : UIScreen
{
	private const int SCALE_INCREASE = 60;

	[SerializeField]
	private Transform fillParent;

	[SerializeField]
	private UISprite fill;

	[SerializeField]
	private UISprite outline;

	[SerializeField]
	private Transform grid;

	[SerializeField]
	private Transform versionNr;

	[SerializeField]
	private Transform contactInfo;

	[SerializeField]
	private GameObject topRunChallengersButton;

	[SerializeField]
	private GameObject logOutFBButton;

	[SerializeField]
	private GameObject logInFBButton;

	[SerializeField]
	private GameObject messagesButton;

	[SerializeField]
	private GameObject reminderButton;

	[SerializeField]
	private GameObject debugButton;

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel soundDescription;

	[SerializeField]
	private UILabel reminderDescription;

	[SerializeField]
	private UILabel topRunChallengersDescription;

	[SerializeField]
	private UILabel privacyButtonLabel;

	[SerializeField]
	private UILabel privacyDescription;

	[SerializeField]
	private UILabel messagesButtonLabel;

	[SerializeField]
	private UILabel messagesDescription;

	private int originalFillScale;

	private int originalOutlineScale;

	private int originalVersionNrHeight;

	private int originalContactInfoHeight;

	private int numberOfDisabledButtons;

	private bool _graphicsRowAdded;

	private UIPanel _listPanel;

	private UIScrollView _listScroll;

	private Transform _scrollBar;

	private float _fillParentBaseY;

	private int _openCount;

	private Vector3 _listStartLocal;

	private string _scrollReadout = "";

	private void Awake()
	{
		titleLabel.text = Strings.Get(StringID.SETTINGS_POPUP_TITLE);
		soundDescription.text = Strings.Get(StringID.SETTINGS_POPUP_SOUND_DESCRIPTION);
		reminderDescription.text = Strings.Get(StringID.SETTINGS_POPUP_REMINDER_DESCRIPTION);
		topRunChallengersDescription.text = Strings.Get(StringID.SETTINGS_POPUP_TOPRUNCHALLENGERS_DESCRIPTION);
		privacyButtonLabel.text = Strings.Get(StringID.SETTINGS_POPUP_PRIVACY);
		privacyDescription.text = Strings.Get(StringID.SETTINGS_POPUP_PRIVACY_DESCRIPTION);
	}

	// Measures the real rows and footer on screen, then sets the scroll window and how far the rows may scroll.
	private void UpdateScrollWindow()
	{
		if (_listPanel == null || _listScroll == null || grid == null)
		{
			return;
		}
		// Dragging moves the scroll view's own transform. Put it back to where it was authored, every open.
		_listPanel.transform.localPosition = _listStartLocal;
		_listPanel.clipOffset = Vector2.zero;
		UIGrid uiGrid = grid.GetComponent<UIGrid>();
		if (uiGrid != null)
		{
			uiGrid.Reposition();
		}
		Transform st = _listPanel.transform;
		Transform first = null;
		foreach (Transform child in grid.transform)
		{
			if (child.gameObject.activeInHierarchy)
			{
				if (first == null)
				{
					first = child;
				}
			}
		}
		if (first == null)
		{
			return;
		}
		float rowTop = RowEdge(st, first, true);
		float footerTop = st.InverseTransformPoint(versionNr.position).y + 6f;
		float windowTop = rowTop + 10f;
		// End above the version and architecture line, with a gap.
		float windowBottom = footerTop + 6f;
		// Never let the window leave the dialog frame: rows must not draw past the border.
		Vector3[] frame = fill.worldCorners;
		float frameBottom = st.InverseTransformPoint(frame[0]).y;
		float frameTop = st.InverseTransformPoint(frame[1]).y;
		float frameRight = st.InverseTransformPoint(frame[2]).x;
		windowTop = Mathf.Min(windowTop, frameTop - 6f);
		windowBottom = Mathf.Max(windowBottom, frameBottom + 6f);
		float windowHeight = windowTop - windowBottom;
		float centerY = (windowTop + windowBottom) * 0.5f;
		_listPanel.baseClipRegion = new Vector4(0f, centerY, 280f, windowHeight);
		// Re-collect the rows into the clipped panel and recompute how far the list can move,
		// so rows never escape the window and the list stays draggable after every re-layout.
		_listPanel.Refresh();
		_listScroll.InvalidateBounds();
		_listScroll.ResetPosition();
		_scrollReadout = $"open#{_openCount} frame T{frameTop:0} B{frameBottom:0} R{frameRight:0} | window T{windowTop:0} B{windowBottom:0} centerY {centerY:0} | footerTop {footerTop:0} | fillParentY {fillParent.localPosition.y:0}" + (_scrollBar != null ? $" | barLocalY {_scrollBar.localPosition.y:0} barX {_scrollBar.localPosition.x:0}" : " | no bar");
		if (_scrollBar != null)
		{
			// Just inside the dialog's right border, like the Awards and Shop bars.
			_scrollBar.position = st.TransformPoint(new Vector3(frameRight - 8f, centerY, 0f));
			UISprite bg = _scrollBar.Find("Background").GetComponent<UISprite>();
			UISprite fg = _scrollBar.Find("Foreground").GetComponent<UISprite>();
			bg.height = Mathf.RoundToInt(windowHeight - 8f);
			fg.height = bg.height;
		}
	}

	// Top or bottom edge of a row's background, in the scroll panel's local space.
	private float RowEdge(Transform panelSpace, Transform row, bool top)
	{
		float edge = top ? float.MinValue : float.MaxValue;
		bool found = false;
		foreach (UIWidget w in row.GetComponentsInChildren<UIWidget>(true))
		{
			if (w.name != "Outline")
			{
				continue;
			}
			found = true;
			foreach (Vector3 corner in w.worldCorners)
			{
				float y = panelSpace.InverseTransformPoint(corner).y;
				edge = top ? Mathf.Max(edge, y) : Mathf.Min(edge, y);
			}
		}
		return found ? edge : panelSpace.InverseTransformPoint(row.position).y + (top ? 28f : -28f);
	}

	public override void Init()
	{
		base.Init();
		originalFillScale = fill.height;
		originalOutlineScale = outline.height;
		originalVersionNrHeight = (int)versionNr.localPosition.y;
		originalContactInfoHeight = (int)contactInfo.localPosition.y;
		_fillParentBaseY = fillParent.localPosition.y;
		if (!Globals.DEBUG_POPUP || !Globals.DEBUG)
		{
			Object.Destroy(debugButton);
		}
		AddGraphicsRow();
	}

	// Clones the sound row (same look and layout) into a Graphics Auto/High/Low row under it.
	private void AddGraphicsRow()
	{
		if (_graphicsRowAdded || grid == null)
		{
			return;
		}
		SoundButton sound = grid.GetComponentInChildren<SoundButton>(true);
		if (sound == null)
		{
			return;
		}
		_graphicsRowAdded = true;
		GameObject row = sound.gameObject;
		GameObject clone = NGUITools.AddChild(grid.gameObject, row);
		clone.name = "1b Graphics";
		clone.transform.SetSiblingIndex(row.transform.GetSiblingIndex() + 1);
		Object.Destroy(clone.GetComponent<SoundButton>());
		foreach (UIButtonMessage message in clone.GetComponentsInChildren<UIButtonMessage>(true))
		{
			Object.Destroy(message);
		}
		GraphicsButton graphics = clone.AddComponent<GraphicsButton>();
		UILabel title = null;
		UILabel desc = null;
		foreach (UILabel label in clone.GetComponentsInChildren<UILabel>(true))
		{
			if (label.name == "Title")
			{
				title = label;
			}
			if (label.name == "Desc")
			{
				desc = label;
			}
		}
		foreach (Transform child in clone.GetComponentsInChildren<Transform>(true))
		{
			if (child.name == "IconOff")
			{
				child.gameObject.SetActive(false);
			}
		}
		graphics.Setup(title, desc);
		foreach (UIButton button in clone.GetComponentsInChildren<UIButton>(true))
		{
			button.onClick.Clear();
		}
		// Taps are detected in GraphicsButton.Update from the screen position, not through NGUI's event chain.
		// Tap areas: the row's background, its title and its description, so tapping the text works too.
		List<UIWidget> hits = new List<UIWidget>();
		foreach (UIWidget widget in clone.GetComponentsInChildren<UIWidget>(true))
		{
			if (widget.name == "Fill" || widget.name == "Title" || widget.name == "Desc")
			{
				hits.Add(widget);
			}
		}
		graphics.SetHitAreas(hits.ToArray());
		// Scroll the rows inside the clipped panel when they do not all fit.
		UIPanel listPanel = grid.transform.parent.GetComponent<UIPanel>();
		if (listPanel != null)
		{
			_listPanel = listPanel;
			_listStartLocal = listPanel.transform.localPosition;
			_listScroll = listPanel.GetComponent<UIScrollView>();
			_scrollBar = listPanel.transform.parent.Find("SettingsScrollBar");
			// Drags that start on a row scroll the list, the same way the Awards and Shop lists do.
			foreach (Collider col in grid.GetComponentsInChildren<Collider>(true))
			{
				UIDragScrollView drag = col.GetComponent<UIDragScrollView>();
				if (drag == null)
				{
					drag = col.gameObject.AddComponent<UIDragScrollView>();
				}
				drag.scrollView = _listScroll;
			}
		}
	}

	public override void Show()
	{
		base.Show();
		_openCount++;
		numberOfDisabledButtons = 0;
		float num = originalFillScale;
		float num2 = originalOutlineScale;
		float num3 = originalVersionNrHeight;
		float num4 = originalContactInfoHeight;
		if (Application.platform == RuntimePlatform.WP8Player || !PlayerInfo.Instance.IsTutorialCompleted)
		{
			if (reminderButton != null)
			{
				num -= 60f;
				num2 -= 60f;
				num3 += 60f;
				num4 += 60f;
				reminderButton.SetActive(false);
				numberOfDisabledButtons++;
			}
		}
		else if (!reminderButton.activeSelf)
		{
			reminderButton.SetActive(true);
		}
		if (!PlayerInfo.Instance.IsTutorialCompleted)
		{
			if (topRunChallengersButton != null)
			{
				num -= 60f;
				num2 -= 60f;
				num3 += 60f;
				num4 += 60f;
				topRunChallengersButton.SetActive(false);
				numberOfDisabledButtons++;
			}
		}
		else if (!topRunChallengersButton.activeSelf)
		{
			topRunChallengersButton.SetActive(true);
		}
		if (!PlayerInfo.Instance.IsTutorialCompleted)
		{
			if (logInFBButton != null && logOutFBButton != null)
			{
				num -= 60f;
				num2 -= 60f;
				num3 += 60f;
				num4 += 60f;
				logInFBButton.SetActive(false);
				logOutFBButton.SetActive(false);
				numberOfDisabledButtons++;
			}
		}
		else
		{
			Globals.LogInState loginState = Globals.GetLoginState();
			if (loginState != Globals.LogInState.Facebook && loginState != Globals.LogInState.Both)
			{
				if (logOutFBButton != null)
				{
					logOutFBButton.SetActive(false);
				}
				if (logInFBButton != null)
				{
					logInFBButton.SetActive(true);
				}
			}
			else
			{
				if (logOutFBButton != null)
				{
					logOutFBButton.SetActive(true);
				}
				if (logInFBButton != null)
				{
					logInFBButton.SetActive(false);
				}
			}
		}
		if (!IsMessagesButtonActive() || !PlayerInfo.Instance.IsTutorialCompleted)
		{
			if (messagesButton != null)
			{
				num -= 60f;
				num2 -= 60f;
				num3 += 60f;
				num4 += 60f;
				messagesButton.SetActive(false);
				numberOfDisabledButtons++;
			}
		}
		else if (!messagesButton.activeSelf)
		{
			messagesButton.SetActive(true);
		}
		fill.height = Mathf.RoundToInt(num);
		outline.height = Mathf.RoundToInt(num2);
		versionNr.localPosition = new Vector3(versionNr.localPosition.x, num3, -1f);
		contactInfo.localPosition = new Vector3(contactInfo.localPosition.x, num4, -1f);
		// Always set from the authored position, so repeated opens cannot stack the offset.
		fillParent.localPosition = new Vector3(fillParent.localPosition.x, _fillParentBaseY - (float)(numberOfDisabledButtons * 60 / 2), 1f);
		grid.GetComponent<UIGrid>().repositionNow = true;
		UpdateScrollWindow();
	}

	private bool AreFacebookFriendsConsolidated()
	{
		int consolidationState = SocialManager.instance.consolidationState;
		bool flag = (consolidationState & 1) == 1;
		return false;
	}

	public override void Hide()
	{
		base.Hide();
		fillParent.localPosition = new Vector3(fillParent.localPosition.x, _fillParentBaseY, 1f);
	}

	private bool IsMessagesButtonActive()
	{
		// GetAndroidApiLevel returns -1 when the device bridge is missing; that must not count as "old Android".
		int apiLevel = DeviceUtility.GetAndroidApiLevel();
		return Application.platform == RuntimePlatform.Android && !DeviceUtility.isKindleDevice && apiLevel > 0 && apiLevel < 16;
	}

	public void LoggedOutOfFacebook()
	{
		if (logOutFBButton != null)
		{
			logOutFBButton.SetActive(false);
		}
		if (logInFBButton != null)
		{
			logInFBButton.SetActive(true);
		}
		grid.GetComponent<UIGrid>().repositionNow = true;
	}

	public void ConnectedToFacebook()
	{
		if (logOutFBButton != null)
		{
			logOutFBButton.SetActive(true);
		}
		if (logInFBButton != null)
		{
			logInFBButton.SetActive(false);
		}
		grid.GetComponent<UIGrid>().repositionNow = true;
	}

	private void OnGUI()
	{
		if (!Globals.DEBUG_SCROLL_READOUT || string.IsNullOrEmpty(_scrollReadout))
		{
			return;
		}
		GUI.Label(new Rect(10f, 40f, 1200f, 300f), _scrollReadout);
	}
}
