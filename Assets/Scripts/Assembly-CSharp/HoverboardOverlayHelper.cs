using System.Linq;
using UnityEngine;

public class HoverboardOverlayHelper : MonoBehaviour
{
	private Hoverboards.BoardType _cachedType;

	private Hoverboards.Board _cachedBoard;

	private int _placementIndex;

	private Transform _trackerTransform;

	private Camera _3DClipCam;

	private Camera _2DOverlayCam;

	private bool _hasInited;

	private Transform _cachedTransform;

	[SerializeField]
	private Transform topOverlay;

	[SerializeField]
	private Transform bottomOverlay;

	[SerializeField]
	private UISprite newSprite;

	[SerializeField]
	private UISprite limitedSprite;

	[SerializeField]
	private UISprite ownedSprite;

	[SerializeField]
	private UISprite selectedSprite;

	[SerializeField]
	private UISprite trialSprite;

	[SerializeField]
	private UISprite giftSprite;

	[SerializeField]
	private UISprite eggHuntSprite;

	[SerializeField]
	private UISprite eventSpriteBubble;

	private float _topOverlayMinX = 100f;

	private float _topOverlayMaxX = 110f;

	private float _currentDistFromCenter;

	private bool _dirtyDist = true;

	private HoverboardSelectButton _hoverboardSelectButton;

	private bool _addedButtonListener;

	private bool _hasEventBeenTried;

	private Hoverboards.BoardType? _eventBoardType;

	private bool _eventFinished;

	public float currentDistFromCenter
	{
		set
		{
			if (_currentDistFromCenter != value)
			{
				_currentDistFromCenter = value;
				_dirtyDist = true;
			}
		}
	}

	private void OnEnable()
	{
		SetEventSpriteState();
		if (_hoverboardSelectButton != null && !_addedButtonListener)
		{
			_hoverboardSelectButton.OnClickSelectedButton += SetEventSpriteState;
			_addedButtonListener = true;
		}
		SSEventsManagerController.OnSSEventStartedHandler += SetEventSpriteState;
		SSEventsManagerController.OnSSEventFinishedHandler += DisableAllEventRelatedElements;
		UIScreenController.OnApplicationResumed += SetEventSpriteState;
	}

	private void OnDisable()
	{
		if (_hoverboardSelectButton != null)
		{
			_hoverboardSelectButton.OnClickSelectedButton -= SetEventSpriteState;
			_addedButtonListener = false;
		}
		SSEventsManagerController.OnSSEventStartedHandler -= SetEventSpriteState;
		SSEventsManagerController.OnSSEventFinishedHandler -= DisableAllEventRelatedElements;
		UIScreenController.OnApplicationResumed -= SetEventSpriteState;
	}

	private void SetEventSpriteState()
	{
		_eventBoardType = EventBoard();
		_eventFinished = !_eventBoardType.HasValue;
		_hasEventBeenTried = SSEventsManagerController.instance.HasTheCurrentEventBeenTried();
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		UISprite uISprite = eventSpriteBubble;
		int num;
		if (_hasInited && !_hasEventBeenTried && HoverboardManager.Instance.GetCurrentlyShownHoverboard(HoverboardManager.ScreenContext.BoardScreen) != _cachedType)
		{
			Hoverboards.BoardType? eventBoardType = _eventBoardType;
			if (eventBoardType.GetValueOrDefault() == _cachedType && eventBoardType.HasValue && activeEventManager != null)
			{
				num = ((activeEventManager.GetEventType() == SSEventType.SpecialPowerBoard || activeEventManager.GetEventType() == SSEventType.ExpiredLimitedBoard) ? 1 : 0);
				goto IL_00b4;
			}
		}
		num = 0;
		goto IL_00b4;
		IL_00b4:
		uISprite.enabled = (byte)num != 0;
	}

	private void DisableAllEventRelatedElements()
	{
		_eventFinished = true;
		eventSpriteBubble.enabled = false;
		trialSprite.enabled = false;
	}

	public void Init(int index, Hoverboards.BoardType type, Transform transformToTrack, HoverboardSelectButton button)
	{
		_hoverboardSelectButton = button;
		if (!_addedButtonListener)
		{
			_hoverboardSelectButton.OnClickSelectedButton += SetEventSpriteState;
			_addedButtonListener = true;
		}
		_placementIndex = index;
		_cachedType = type;
		_cachedBoard = Hoverboards.boardData[_cachedType];
		_3DClipCam = UIScreenController.Instance.Camera3DClip;
		_2DOverlayCam = UIScreenController.Instance.CameraOverlay2dClipping;
		_cachedTransform = base.transform;
		_hasInited = true;
		_trackerTransform = transformToTrack;
		RefreshBubbles(true);
		UpdateSelected();
	}

	public void RefreshBubbles(bool initialSetup)
	{
		if (!initialSetup)
		{
			SetEventSpriteState();
		}
		else
		{
			eventSpriteBubble.enabled = false;
		}
		giftSprite.enabled = !eventSpriteBubble.enabled && ((_cachedBoard.unlockType == Hoverboards.UnlockType.free && !HoverboardStickerManager.isHoverboardUnlocked(_cachedType)) || BoardHasFreeThemes(_cachedType));
		newSprite.enabled = !giftSprite.enabled && !eventSpriteBubble.enabled && !_cachedBoard.hasOnlineTimeLimit && !PlayerInfo.Instance.isFreshInstall && (!HoverboardStickerManager.hasHoverboardBeenSeen(_cachedType) || (PlayerInfo.Instance.HoverboardCustomizationData.DoesBoardHaveThemes(_cachedType) && !PlayerInfo.Instance.HoverboardCustomizationData.AreAllNewThemesSeenForBoard(_cachedType))) && !PlayerInfo.Instance.isHoverboardUnlocked(_cachedType);
		if (_cachedType == Hoverboards.BoardType.chick)
		{
			eggHuntSprite.enabled = _cachedType == Hoverboards.BoardType.chick && !HoverboardStickerManager.isHoverboardUnlocked(_cachedType) && !newSprite.enabled;
			limitedSprite.enabled = false;
			return;
		}
		eggHuntSprite.enabled = false;
		UISprite uISprite = limitedSprite;
		int num;
		if (_cachedBoard.hasOnlineTimeLimit && !eventSpriteBubble.enabled && !giftSprite.enabled && !newSprite.enabled)
		{
			Hoverboards.BoardType? eventBoardType = _eventBoardType;
			if (eventBoardType.GetValueOrDefault() != _cachedType || !eventBoardType.HasValue)
			{
				num = ((!HoverboardStickerManager.isHoverboardUnlocked(_cachedType)) ? 1 : 0);
				goto IL_01ee;
			}
		}
		num = 0;
		goto IL_01ee;
		IL_01ee:
		uISprite.enabled = (byte)num != 0;
	}

	private bool BoardHasFreeThemes(Hoverboards.BoardType boardType)
	{
		if (!HoverboardThemes.hoverboardCustomThemes.ContainsKey(_cachedType))
		{
			return false;
		}
		return HoverboardThemes.hoverboardCustomThemes[_cachedType].Any((HoverboardThemes.HoverboardTheme ht) => ht.unlockType == Hoverboards.UnlockType.free && !PlayerInfo.Instance.HoverboardCustomizationData.IsThemeOwned(boardType, ht.identifier));
	}

	private Hoverboards.BoardType? EventBoard()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (activeEventManager != null)
		{
			SSEventType eventType = activeEventManager.GetEventType();
			if (eventType == SSEventType.SpecialPowerBoard || eventType == SSEventType.ExpiredLimitedBoard)
			{
				bool found = false;
				Hoverboards.BoardType eventValue = activeEventManager.GetEventValue<Hoverboards.BoardType>(out found);
				if (found)
				{
					return eventValue;
				}
			}
		}
		return null;
	}

	public int GetIndex()
	{
		return _placementIndex;
	}

	public void SelectedInMenu()
	{
		if (!HoverboardStickerManager.hasHoverboardBeenSeen(_cachedType))
		{
			newSprite.enabled = false;
			if (_cachedType == Hoverboards.BoardType.chick)
			{
				eggHuntSprite.enabled = true;
			}
			PlayerInfo.Instance.MarkHoverboardAsSeen(_cachedType);
		}
		if (limitedSprite.enabled)
		{
			limitedSprite.enabled = false;
		}
		if (giftSprite.enabled)
		{
			giftSprite.enabled = false;
		}
		if (eventSpriteBubble.enabled)
		{
			eventSpriteBubble.enabled = false;
		}
	}

	public Hoverboards.BoardType GetBoardType()
	{
		return _cachedType;
	}

	private void Update()
	{
		UpdateSelected();
		if (limitedSprite.enabled && HoverboardStickerManager.isHoverboardUnlocked(_cachedType))
		{
			limitedSprite.enabled = false;
			eggHuntSprite.enabled = false;
		}
		if (giftSprite.enabled)
		{
			giftSprite.enabled = (_cachedBoard.unlockType == Hoverboards.UnlockType.free && !HoverboardStickerManager.isHoverboardUnlocked(_cachedType)) || BoardHasFreeThemes(_cachedType);
		}
	}

	private void UpdateSelected()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.SpecialPowerBoard);
		UISprite uISprite = trialSprite;
		int num;
		if (_hasEventBeenTried)
		{
			Hoverboards.BoardType? eventBoardType = _eventBoardType;
			if (eventBoardType.GetValueOrDefault() == _cachedType && eventBoardType.HasValue && PlayerInfo.Instance.currentHoverboard == _cachedType && !_eventFinished)
			{
				num = ((activeEventManager != null) ? 1 : 0);
				goto IL_006c;
			}
		}
		num = 0;
		goto IL_006c;
		IL_006c:
		uISprite.enabled = (byte)num != 0;
		selectedSprite.enabled = PlayerInfo.Instance.currentHoverboard == _cachedType && !trialSprite.enabled;
		UISprite uISprite2 = ownedSprite;
		int num2;
		if (!selectedSprite.enabled && !trialSprite.enabled)
		{
			if (!HoverboardStickerManager.isHoverboardUnlocked(_cachedType))
			{
				if (!_eventFinished)
				{
					Hoverboards.BoardType? eventBoardType2 = _eventBoardType;
					if (eventBoardType2.GetValueOrDefault() == _cachedType && eventBoardType2.HasValue && _hasEventBeenTried)
					{
						num2 = ((activeEventManager != null) ? 1 : 0);
						goto IL_0123;
					}
				}
				num2 = 0;
			}
			else
			{
				num2 = 1;
			}
		}
		else
		{
			num2 = 0;
		}
		goto IL_0123;
		IL_0123:
		uISprite2.enabled = (byte)num2 != 0;
	}

	private void LateUpdate()
	{
		if (_hasInited && _dirtyDist)
		{
			_dirtyDist = false;
			Vector3 vector = _cachedTransform.parent.InverseTransformPoint(_2DOverlayCam.ScreenToWorldPoint(_3DClipCam.WorldToScreenPoint(_trackerTransform.position)));
			float y = Mathf.SmoothStep(_topOverlayMaxX, _topOverlayMinX, _currentDistFromCenter);
			topOverlay.localPosition = new Vector3(vector.x, y, _cachedTransform.localPosition.z);
			bottomOverlay.localPosition = new Vector3(vector.x, vector.y, _cachedTransform.localPosition.z);
		}
	}
}
