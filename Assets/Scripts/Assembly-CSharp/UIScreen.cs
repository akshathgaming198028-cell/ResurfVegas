using UnityEngine;

public class UIScreen : MonoBehaviour
{
	[SerializeField]
	protected GameObject[] dynamicallyLoadedElements;

	[OptionalField]
	[SerializeField]
	protected GameObject FooterPrefab;

	[SerializeField]
	protected int selectedFooterButton;

	protected UIFooterHandler _footerHandler;

	private bool _footerInited;

	private bool _isActive;

	public string screenName = string.Empty;

	public bool isActive
	{
		get
		{
			return _isActive;
		}
	}

	public string parentScreen { get; set; }

	public virtual void Init()
	{
		AdjustToResolution();
		for (int i = 0; i < dynamicallyLoadedElements.Length; i++)
		{
			NGUITools.AddChild(base.gameObject, dynamicallyLoadedElements[i]);
		}
	}

	public virtual void Show()
	{
		base.gameObject.SetActive(true);
		_isActive = true;
		string flurryScreenName = UIScreens.GetFlurryScreenName(base.gameObject.name);
		if (flurryScreenName != string.Empty)
		{
			Flurry.LogEvent(flurryScreenName);
		}
		if (!_footerInited)
		{
			InitFooter();
			_footerInited = true;
		}
	}

	public virtual void Hide()
	{
		base.gameObject.SetActive(false);
		_isActive = false;
	}

	public virtual void LooseFocus()
	{
	}

	public virtual void GainFocus()
	{
	}

	public virtual void AdjustToResolution()
	{
	}

	protected virtual void InitFooter()
	{
		if (FooterPrefab != null)
		{
			GameObject gameObject = NGUITools.AddChild(base.gameObject, FooterPrefab);
			_footerHandler = gameObject.GetComponent<UIFooterHandler>();
			switch (selectedFooterButton)
			{
			case 1:
				_footerHandler.button1.GoToSelectedState();
				_footerHandler.button1.LockState(true);
				break;
			case 2:
				_footerHandler.button2.GoToSelectedState();
				_footerHandler.button2.LockState(true);
				break;
			case 3:
				_footerHandler.button3.GoToSelectedState();
				_footerHandler.button3.LockState(true);
				break;
			}
		}
	}

	public static bool IsScreenHeightOutOfProportion()
	{
		return (float)Screen.height * 1f / ((float)Screen.width * 1f) > 1.5f;
	}

	public GameObject InitializeCoinbox(UIWidget widgetAnchor, bool fundsEnabled, bool coinsEnabled, bool keysEnabled, float xOffset = 0f, float yOffset = 0f, float zOffset = 0f)
	{
		GameObject prefab = Resources.Load("Prefabs/DynamicLoad/CoinboxQuick") as GameObject;
		GameObject gameObject = NGUITools.AddChild(base.gameObject, prefab);
		CoinBoxSizer componentInChildren = gameObject.GetComponentInChildren<CoinBoxSizer>();
		componentInChildren.Init(widgetAnchor, fundsEnabled, coinsEnabled, keysEnabled, xOffset, yOffset, zOffset);
		return gameObject;
	}
}
