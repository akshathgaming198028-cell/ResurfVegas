using UnityEngine;

public class GuidelineButton : MonoBehaviour
{
	public enum GuidelineButtonType
	{
		_notSet = 0,
		hoverboard = 1,
		facebookOrCollect = 2,
		mission1 = 3,
		itemHunt = 4,
		playerMenuIntroduction = 5,
		shopMenuIntroduction = 6
	}

	private enum AnimationState
	{
		NONE = 0,
		SCALING = 1,
		BLINKING = 2,
		WAITING = 3
	}

	private const float SCALE_MAX_SIZE = 2.5f;

	private const float WAIT_DURATION = 2f;

	private const float BLINK_DURATION = 1f;

	[SerializeField]
	private UISprite infoSprite;

	[SerializeField]
	private UISprite infoSpriteOverlay;

	[SerializeField]
	private GameObject _breadcrumb;

	[SerializeField]
	private GameObject _discount;

	private bool _showIcon;

	private static string HAS_SHOWN_INFO_BUBLE_STRING;

	public bool __forceIcon;

	public GuidelineButtonType guidelineButtonType;

	private bool _hasInited;

	private Vector3 _startScale;

	private Vector3 _maxScale;

	private float SCALE_DURATION;

	private AnimationState animationState;

	private float currentAnimationTime;

	private bool wiggleNow
	{
		get
		{
			return _showIcon;
		}
		set
		{
			_showIcon = value;
			if (guidelineButtonType == GuidelineButtonType.facebookOrCollect)
			{
				if ((double)Random.value > 0.5)
				{
					PlayerInfo.Instance.shouldShowFacebookPopup = true;
					PlayerInfo.Instance.hasShownFacebookPopup = false;
				}
				else
				{
					PlayerInfo.Instance.shouldShowCollectPopup = value;
					PlayerInfo.Instance.hasShownCollectPopup = !value;
				}
			}
			else if (guidelineButtonType == GuidelineButtonType.hoverboard)
			{
				PlayerInfo.Instance.shouldShowHoverboardPopup = value;
				PlayerInfo.Instance.hasShownHoverboardPopup = !value;
			}
			else if (guidelineButtonType == GuidelineButtonType.mission1)
			{
				PlayerInfo.Instance.shouldShowMission1Popup = true;
				PlayerInfo.Instance.hasShownMission1Popup = !value;
				Debug.Log("wiggle mission1 " + value);
			}
			else if (guidelineButtonType != GuidelineButtonType.itemHunt && guidelineButtonType != GuidelineButtonType.playerMenuIntroduction)
			{
			}
			OnEnable();
		}
	}

	private void Awake()
	{
		infoSprite.enabled = false;
		infoSpriteOverlay.enabled = false;
	}

	private void OnEnable()
	{
		HAS_SHOWN_INFO_BUBLE_STRING = Strings.Get(StringID.WEEKLY_HUNT_BUBBLE_KEY).Trim().ToLower();
		if (!_hasInited)
		{
			_startScale = infoSprite.transform.localScale;
			_maxScale = _startScale * 2.5f;
			infoSprite.alpha = 1f;
			infoSpriteOverlay.alpha = 0f;
			SCALE_DURATION = UIScreenController.Instance.guidelineAnimation.keys[UIScreenController.Instance.guidelineAnimation.length - 1].time;
			_hasInited = true;
		}
		infoSprite.enabled = false;
		infoSpriteOverlay.enabled = false;
		bool flag = ShouldInfoIconShow();
		if (flag)
		{
			EnableInfoIcon();
		}
		if (_discount != null && flag)
		{
			_discount.SetActive(false);
		}
		if (_breadcrumb != null)
		{
			_breadcrumb.SetActive(!flag);
		}
	}

	private void OnClick()
	{
		if (_showIcon)
		{
			_showIcon = false;
			infoSprite.enabled = false;
			infoSpriteOverlay.enabled = false;
			infoSpriteOverlay.alpha = 0f;
			animationState = AnimationState.NONE;
			if (guidelineButtonType == GuidelineButtonType.facebookOrCollect && PlayerInfo.Instance.shouldShowCollectPopup)
			{
				PlayerInfo.Instance.dummyFriendShouldShow = true;
				UIScreenController.Instance.PushScreen("FriendsUI");
				UIScreenController.Instance.QueuePopup("TutorialCollectFromFriendsPopup");
				PlayerInfo.Instance.hasShownCollectPopup = true;
				PlayerInfo.Instance.shouldShowCollectPopup = false;
			}
			else if (guidelineButtonType == GuidelineButtonType.facebookOrCollect && PlayerInfo.Instance.shouldShowFacebookPopup)
			{
				UIScreenController.Instance.PushScreen("FriendsUI");
				UIScreenController.Instance.QueuePopup("TutorialFacebookPopup");
				PlayerInfo.Instance.hasShownFacebookPopup = true;
				PlayerInfo.Instance.shouldShowFacebookPopup = false;
			}
			else if (guidelineButtonType == GuidelineButtonType.facebookOrCollect && PlayerInfo.Instance.lastShownTopRunIntroPopupVersionNumber != 2)
			{
				UIScreenController.Instance.PushScreen("TopRunUI");
				UIScreenController.Instance.QueuePopup("TopRunPopup");
				PlayerInfo.Instance.lastShownTopRunIntroPopupVersionNumber = 2;
			}
			else if (guidelineButtonType == GuidelineButtonType.hoverboard)
			{
				UIScreenController.Instance.QueuePopup("TutorialHoverboardsPopup");
				PlayerInfo.Instance.hasShownHoverboardPopup = true;
				PlayerInfo.Instance.shouldShowHoverboardPopup = false;
			}
			else if (guidelineButtonType == GuidelineButtonType.mission1)
			{
				UIScreenController.Instance.QueuePopup("TutorialMissionPopup");
				PlayerInfo.Instance.hasShownMission1Popup = true;
				PlayerInfo.Instance.shouldShowMission1Popup = false;
			}
			else if (guidelineButtonType == GuidelineButtonType.itemHunt)
			{
				PlayerPrefs.SetInt(guidelineButtonType.ToString() + HAS_SHOWN_INFO_BUBLE_STRING, 1);
				UIScreenController.Instance.QueuePopup("ItemHuntPopup");
			}
			else if (guidelineButtonType == GuidelineButtonType._notSet)
			{
				Debug.LogError("A guideline button wiggler is not set properly.", base.gameObject);
			}
		}
		else if (guidelineButtonType == GuidelineButtonType.facebookOrCollect)
		{
			UIScreenController.Instance.PushScreen(UIScreens.friendsMenu_lastScreen);
		}
		else if (guidelineButtonType == GuidelineButtonType.hoverboard)
		{
			UIScreenController.Instance.QueuePopup("HoverboardPopup");
		}
		else if (guidelineButtonType == GuidelineButtonType.itemHunt)
		{
			UIScreenController.Instance.QueuePopup("ItemHuntPopup");
		}
		else if (guidelineButtonType == GuidelineButtonType.mission1)
		{
			UIScreenController.Instance.QueuePopup("Mission_popup");
		}
		else if (guidelineButtonType == GuidelineButtonType.playerMenuIntroduction)
		{
			UIScreenController.Instance.PushScreen(UIScreens.meMenu_lastScreen);
		}
		else if (guidelineButtonType == GuidelineButtonType.shopMenuIntroduction)
		{
			UIScreenController.Instance.PushScreen(UIScreens.shopMenu_lastScreen);
		}
	}

	public bool ShouldInfoIconShow()
	{
		bool result = false;
		if (guidelineButtonType == GuidelineButtonType.hoverboard && PlayerInfo.Instance.shouldShowHoverboardPopup && !PlayerInfo.Instance.hasShownHoverboardPopup)
		{
			result = true;
		}
		else if (guidelineButtonType == GuidelineButtonType.facebookOrCollect && PlayerInfo.Instance.lastShownTopRunIntroPopupVersionNumber != 2 && !PlayerInfo.Instance.isFreshInstall)
		{
			result = true;
		}
		else if (guidelineButtonType == GuidelineButtonType.facebookOrCollect && PlayerInfo.Instance.shouldShowFacebookPopup && !PlayerInfo.Instance.hasShownFacebookPopup)
		{
			if (PlayerInfo.Instance.shouldShowHoverboardPopup)
			{
				Debug.Log("Tried to show facebook guideline icon when hoverboard was active", base.gameObject);
			}
			else
			{
				result = true;
			}
		}
		else if (guidelineButtonType == GuidelineButtonType.facebookOrCollect && PlayerInfo.Instance.shouldShowCollectPopup && !PlayerInfo.Instance.hasShownCollectPopup)
		{
			if (PlayerInfo.Instance.shouldShowHoverboardPopup || PlayerInfo.Instance.shouldShowFacebookPopup)
			{
				Debug.Log("Tried to show collect guideline icon when hoverboard or facebook was active", base.gameObject);
			}
			else
			{
				result = true;
			}
		}
		else if (guidelineButtonType == GuidelineButtonType.mission1 && PlayerInfo.Instance.shouldShowMission1Popup && !PlayerInfo.Instance.hasShownMission1Popup)
		{
			if (PlayerInfo.Instance.shouldShowHoverboardPopup || PlayerInfo.Instance.shouldShowFacebookPopup || PlayerInfo.Instance.shouldShowCollectPopup)
			{
				Debug.Log("Tried to show collect guideline icon when hoverboard, facebook, collect or duel was active", base.gameObject);
			}
			else
			{
				result = true;
			}
		}
		else if (guidelineButtonType == GuidelineButtonType.itemHunt)
		{
			if (!PlayerPrefs.HasKey(guidelineButtonType.ToString() + HAS_SHOWN_INFO_BUBLE_STRING))
			{
				if (PlayerInfo.Instance.highestScore > 0)
				{
					result = true;
				}
				else
				{
					PlayerPrefs.SetInt(guidelineButtonType.ToString() + HAS_SHOWN_INFO_BUBLE_STRING, 1);
				}
			}
		}
		else if (guidelineButtonType == GuidelineButtonType._notSet)
		{
			Debug.LogError("A guideline button wiggler is not set properly.", base.gameObject);
		}
		return result;
	}

	private void EnableInfoIcon()
	{
		_showIcon = true;
		infoSprite.enabled = true;
		infoSpriteOverlay.enabled = true;
		if (animationState == AnimationState.NONE)
		{
			if (!PlayerPrefs.HasKey(guidelineButtonType.ToString() + "hasScaled"))
			{
				animationState = AnimationState.SCALING;
				currentAnimationTime = 0f;
				PlayerPrefs.SetInt(guidelineButtonType.ToString() + "hasScaled", 1);
			}
			else if (PlayerPrefs.GetInt(guidelineButtonType.ToString() + "hasScaled") == 1 && guidelineButtonType == GuidelineButtonType.facebookOrCollect)
			{
				animationState = AnimationState.SCALING;
				currentAnimationTime = 0f;
				PlayerPrefs.SetInt(guidelineButtonType.ToString() + "hasScaled", 2);
			}
			else
			{
				animationState = AnimationState.BLINKING;
				currentAnimationTime = 0f;
			}
		}
	}

	private void Update()
	{
		if (animationState == AnimationState.NONE)
		{
			return;
		}
		if (animationState == AnimationState.SCALING)
		{
			if (currentAnimationTime < SCALE_DURATION)
			{
				float num = UIScreenController.Instance.guidelineAnimation.Evaluate(currentAnimationTime);
				Vector3 localScale = Vector3.Lerp(_startScale, _maxScale, num);
				infoSprite.cachedTransform.localScale = localScale;
				infoSpriteOverlay.cachedTransform.localScale = localScale;
				float alpha = num;
				infoSpriteOverlay.alpha = alpha;
				currentAnimationTime += Time.deltaTime;
			}
			else
			{
				infoSprite.cachedTransform.localScale = _startScale;
				infoSpriteOverlay.cachedTransform.localScale = _startScale;
				infoSpriteOverlay.alpha = 0f;
				animationState = AnimationState.WAITING;
				currentAnimationTime = 0f;
			}
		}
		if (animationState == AnimationState.BLINKING)
		{
			if (currentAnimationTime < 1f)
			{
				float alpha2 = ((!(currentAnimationTime < 0.5f)) ? Mathf.SmoothStep(2f, 0f, currentAnimationTime / 1f) : Mathf.SmoothStep(0f, 2f, currentAnimationTime / 1f));
				infoSpriteOverlay.alpha = alpha2;
				currentAnimationTime += Time.deltaTime;
			}
			else
			{
				infoSpriteOverlay.alpha = 0f;
				animationState = AnimationState.WAITING;
				currentAnimationTime = 0f;
			}
		}
		if (animationState == AnimationState.WAITING)
		{
			if (currentAnimationTime < 2f)
			{
				currentAnimationTime += Time.deltaTime;
				return;
			}
			animationState = AnimationState.BLINKING;
			currentAnimationTime = 0f;
		}
	}
}
