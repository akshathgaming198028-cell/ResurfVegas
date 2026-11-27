using Kiloo.Plugins.DeviceUtility;
using UnityEngine;

public class SettingsPopup : UIScreen
{
	private const int SCALE_INCREASE = 60;

	private const int GRID_CELL_HEIGHT_WITH_GRAPHICS = 52;

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

	private bool _scrollSetup;

	private void Awake()
	{
		titleLabel.text = Strings.Get(StringID.SETTINGS_POPUP_TITLE);
		soundDescription.text = Strings.Get(StringID.SETTINGS_POPUP_SOUND_DESCRIPTION);
		reminderDescription.text = Strings.Get(StringID.SETTINGS_POPUP_REMINDER_DESCRIPTION);
		topRunChallengersDescription.text = Strings.Get(StringID.SETTINGS_POPUP_TOPRUNCHALLENGERS_DESCRIPTION);
		privacyButtonLabel.text = Strings.Get(StringID.SETTINGS_POPUP_PRIVACY);
		privacyDescription.text = Strings.Get(StringID.SETTINGS_POPUP_PRIVACY_DESCRIPTION);
	}

	public override void Init()
	{
		base.Init();
		originalFillScale = fill.height;
		originalOutlineScale = outline.height;
		originalVersionNrHeight = (int)versionNr.localPosition.y;
		originalContactInfoHeight = (int)contactInfo.localPosition.y;
		if (!Globals.DEBUG_POPUP || !Globals.DEBUG)
		{
			Object.Destroy(debugButton);
		}
		AddGraphicsRow();
	}

	// Puts the rows in a clipped, draggable area so the list scrolls when it is taller than the dialog.
	private void SetupScrollableList()
	{
		if (_scrollSetup || grid == null)
		{
			return;
		}
		_scrollSetup = true;
		Transform gridTr = grid.transform;
		Transform parent = gridTr.parent;
		UIPanel parentPanel = parent.GetComponentInParent<UIPanel>();
		GameObject scrollGO = new GameObject("SettingsListScroll");
		scrollGO.layer = gridTr.gameObject.layer;
		Transform scrollTr = scrollGO.transform;
		scrollTr.SetParent(parent, false);
		scrollTr.localPosition = Vector3.zero;
		scrollTr.localRotation = Quaternion.identity;
		scrollTr.localScale = Vector3.one;
		Vector3 gridLocal = gridTr.localPosition;
		gridTr.SetParent(scrollTr, false);
		gridTr.localPosition = gridLocal;
		UIPanel panel = scrollGO.AddComponent<UIPanel>();
		panel.depth = parentPanel != null ? parentPanel.depth : 0;
		panel.clipping = UIDrawCall.Clipping.SoftClip;
		panel.baseClipRegion = new Vector4(0f, -20f, 300f, 400f);
		panel.clipSoftness = new Vector2(4f, 4f);
		UIScrollView scrollView = scrollGO.AddComponent<UIScrollView>();
		scrollView.movement = UIScrollView.Movement.Vertical;
		scrollView.restrictWithinPanel = true;
		scrollView.dragEffect = UIScrollView.DragEffect.MomentumAndSpring;
		UIDragScrollView dragForward = gridTr.gameObject.AddComponent<UIDragScrollView>();
		dragForward.scrollView = scrollView;
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
		UIWidget hit = null;
		foreach (UIWidget widget in clone.GetComponentsInChildren<UIWidget>(true))
		{
			if (widget.name == "Fill")
			{
				hit = widget;
			}
		}
		graphics.SetHitArea(hit);
		// The panel is already full screen height, so the extra row is fitted by tightening the grid spacing
		// instead of growing the panel (growing it pushed the list off screen).
		UIGrid uiGrid = grid.GetComponent<UIGrid>();
		uiGrid.cellHeight = GRID_CELL_HEIGHT_WITH_GRAPHICS;
		SetupScrollableList();
	}

	public override void Show()
	{
		base.Show();
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
		fillParent.localPosition = new Vector3(fillParent.localPosition.x, fillParent.localPosition.y - (float)(numberOfDisabledButtons * 60 / 2), 1f);
		grid.GetComponent<UIGrid>().repositionNow = true;
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
		fillParent.localPosition = new Vector3(fillParent.localPosition.x, fillParent.localPosition.y + (float)(numberOfDisabledButtons * 60 / 2), 1f);
	}

	private bool IsMessagesButtonActive()
	{
		return Application.platform == RuntimePlatform.Android && !DeviceUtility.isKindleDevice && DeviceUtility.GetAndroidApiLevel() < 16;
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
}
