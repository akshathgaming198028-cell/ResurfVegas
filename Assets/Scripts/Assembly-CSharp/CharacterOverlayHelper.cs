using Kiloo.Common;
using SYBO.Subway.Characters;
using SYBO.Subway.Info;
using UnityEngine;

public class CharacterOverlayHelper : MonoBehaviour
{
	private Characters.CharacterType _cachedType;

	private int _cachedThemeIndex;

	private int _placementIndex;

	private Camera _3DClipCam;

	private Camera _2DOverlayCam;

	[SerializeField]
	private Transform headTrack;

	[SerializeField]
	private Transform feetTrack;

	private bool _hasInited;

	private Transform _cachedTransform;

	private Transform _headTransform;

	private Transform _chestTransform;

	[SerializeField]
	private float yOffset = 1f;

	[SerializeField]
	private float xOffset = 1f;

	[SerializeField]
	private UISprite newSprite;

	[SerializeField]
	private UISprite limitedSprite;

	[SerializeField]
	private UISprite ownedSprite;

	[SerializeField]
	private UISprite selectedSprite;

	[SerializeField]
	private UISprite giftSprite;

	[SerializeField]
	private UISprite eventSpriteBubble;

	private Characters.CharacterType? _eventCharType;

	public void Init(int index, Characters.CharacterType type, Transform transformToTrack, bool showSprite, int themeVersion = 0)
	{
		_placementIndex = index;
		_cachedType = type;
		_cachedThemeIndex = themeVersion;
		_3DClipCam = UIScreenController.Instance.Camera3DClip;
		_2DOverlayCam = UIScreenController.Instance.CameraOverlay2dClipping;
		_cachedTransform = base.transform;
		_hasInited = true;
		_headTransform = TransformUtility.FindChild(transformToTrack, Character.Instance.characterModel.HeadJoint.name);
		_chestTransform = TransformUtility.FindChild(transformToTrack, Character.Instance.characterModel.Hip.name);
		enableOverlay(showSprite);
		UpdateSelected();
	}

	private void OnEnable()
	{
		_eventCharType = EventCharacter();
		SSEventsManagerController.OnSSEventStartedHandler += EnableEventRelatedElements;
		SSEventsManagerController.OnSSEventFinishedHandler += EnableEventRelatedElements;
	}

	private void OnDisable()
	{
		SSEventsManagerController.OnSSEventStartedHandler -= EnableEventRelatedElements;
		SSEventsManagerController.OnSSEventFinishedHandler -= EnableEventRelatedElements;
	}

	private void EnableEventRelatedElements()
	{
		_eventCharType = EventCharacter();
		RefreshOverlay();
	}

	public int GetIndex()
	{
		return _placementIndex;
	}

	public Characters.CharacterType getCharacterType()
	{
		return _cachedType;
	}

	public void RefreshOverlay()
	{
		enableOverlay(true);
	}

	public void SelectedInMenu()
	{
		if (!CharacterStickerManager.hasCharacterBeenSeen(_cachedType))
		{
			PlayerInfo.Instance.MarkCharacterAsSeen(_cachedType);
		}
		if (limitedSprite.enabled)
		{
			limitedSprite.enabled = false;
		}
		if (giftSprite.enabled)
		{
			giftSprite.enabled = false;
		}
		if (newSprite.enabled)
		{
			newSprite.enabled = false;
		}
		if (eventSpriteBubble.enabled)
		{
			eventSpriteBubble.enabled = false;
		}
	}

	public void enableOverlay(bool showSprite)
	{
		if (!(eventSpriteBubble != null) || !(giftSprite != null) || !(newSprite != null) || !(limitedSprite != null))
		{
			return;
		}
		UISprite uISprite = eventSpriteBubble;
		int num;
		if (showSprite)
		{
			Characters.CharacterType? eventCharType = _eventCharType;
			if (eventCharType.HasValue)
			{
				Characters.CharacterType? eventCharType2 = _eventCharType;
				num = ((eventCharType2.GetValueOrDefault() == _cachedType && eventCharType2.HasValue) ? 1 : 0);
				goto IL_0086;
			}
		}
		num = 0;
		goto IL_0086;
		IL_0086:
		uISprite.enabled = (byte)num != 0;
		giftSprite.enabled = showSprite && !eventSpriteBubble.enabled && CharacterStickerManager.hasCharacterFreeTheme(_cachedType);
		newSprite.enabled = showSprite && !giftSprite.enabled && !eventSpriteBubble.enabled && !PlayerInfo.Instance.isFreshInstall && ((!CharacterStickerManager.hasCharacterBeenSeen(_cachedType) && Characters.characterData[_cachedType].isNewInThisUpdate) || CharacterStickerManager.hasCharacterNewTheme(_cachedType));
		limitedSprite.enabled = showSprite && !eventSpriteBubble.enabled && !giftSprite.enabled && !newSprite.enabled && Characters.characterData[_cachedType].hasOnlineSettings && !CharacterStickerManager.isCharacterUnlocked(_cachedType);
	}

	public Characters.CharacterType GetCharacterType()
	{
		return _cachedType;
	}

	public int GetCharacterThemeIndex()
	{
		return _cachedThemeIndex;
	}

	private Characters.CharacterType? EventCharacter()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (activeEventManager != null)
		{
			SSEventType eventType = activeEventManager.GetEventType();
			if (eventType == SSEventType.ExpiredLimitedCharacter || eventType == SSEventType.CharacterDay)
			{
				bool found = false;
				Characters.CharacterType eventValue = activeEventManager.GetEventValue<Characters.CharacterType>(out found);
				if (found)
				{
					return eventValue;
				}
			}
		}
		return null;
	}

	private void Update()
	{
		UpdateSelected();
		if (limitedSprite.enabled && CharacterStickerManager.isCharacterUnlocked(_cachedType))
		{
			limitedSprite.enabled = false;
		}
	}

	private void UpdateSelected()
	{
		selectedSprite.enabled = PlayerInfo.Instance.currentCharacter == (int)_cachedType;
		ownedSprite.enabled = !selectedSprite.enabled && CharacterStickerManager.isCharacterUnlocked(_cachedType);
	}

	private void LateUpdate()
	{
		if (_hasInited)
		{
			Vector3 position = new Vector3(_chestTransform.position.x, _headTransform.position.y * yOffset, _chestTransform.position.z);
			Vector3 position2 = _3DClipCam.WorldToScreenPoint(position);
			Vector3 position3 = _2DOverlayCam.ScreenToWorldPoint(position2);
			position3 = _cachedTransform.parent.InverseTransformPoint(position3);
			headTrack.localPosition = new Vector3(position3.x, position3.y, _cachedTransform.localPosition.z);
			Vector3 position4 = new Vector3(_chestTransform.position.x * xOffset, _chestTransform.position.y, _chestTransform.position.z);
			Vector3 position5 = _3DClipCam.WorldToScreenPoint(position4);
			Vector3 position6 = _2DOverlayCam.ScreenToWorldPoint(position5);
			position6 = _cachedTransform.parent.InverseTransformPoint(position6);
			position6 = new Vector3(position6.x + 20f, _cachedTransform.localPosition.y + 100f, _cachedTransform.localPosition.z);
			feetTrack.localPosition = position6;
		}
	}
}
