using System;
using System.Collections;
using Kiloo.Plugins.DeviceUtility;
using UnityEngine;
using com.kiloo.awards;

public class AwardCellHelper : MonoBehaviour
{
	private enum AwardCellState
	{
		NotSet = 0,
		SocialNetworksLocked = 1,
		PreviewInProgress = 2,
		InProgress = 3,
		Collectible = 4,
		PreviewFinished = 5,
		Finished = 6
	}

	private const string AWARD_IN_PROGRESS_DESCRIPTION_HEX_COLOR = "[2486BB]";

	private const string AWARD_IN_PROGRESS_VALUE_HEX_COLOR = "[004A7D]";

	private const string AWARD_COMPLETE_DESCRIPTION_HEX_COLOR = "[549133]";

	private const string AWARD_COMPLETE_VALUE_HEX_COLOR = "[015A01]";

	private const string END_COLOR_SYMBOL = "[-]";

	private const string COMPLETED_ITEM_BACKGROUND_SPRITE_NAME = "icon_award_bg_completed_small";

	private const string COLLECTIBLE_ITEM_BACKGROUND_SPRITE_NAME = "icon_award_collect_bg_small";

	private const string IN_PROGRESS_ITEM_BACKGROUND_SPRITE_NAME = "icon_award_bg_small";

	private const string COMPLETED_GOAL_ICON_BACKGROUND_SPRITE_NAME = "icon_award_bg_completed";

	private const string COLLECTIBLE_GOAL_ICON_BACKGROUND_SPRITE_NAME = "icon_award_collect_bg";

	private const string IN_PROGRESS_GOAL_ICON_BACKGROUND_SPRITE_NAME = "icon_award_bg";

	private static readonly Color32 COMPLETED_TITLE_TEXT_COLOR = new Color32(1, 90, 1, byte.MaxValue);

	private static readonly Color32 ACTIVE_TITLE_TEXT_COLOR = new Color32(0, 74, 125, byte.MaxValue);

	private static readonly Color32 ACTIVE_TITLE_TEXT_SHADOW_COLOR = new Color32(229, 234, 238, byte.MaxValue);

	private static readonly Color32 COMPLETED_TITLE_TEXT_SHADOW_COLOR = new Color32(232, 248, 228, byte.MaxValue);

	private static readonly Color32 COLLECTIBLE_SEGMENT_BACKGROUND_COLOR = new Color32(142, 206, 242, 180);

	private static readonly Color32 PROGRESS_SEGMENT_BACKGROUND_COLOR = new Color32(170, 190, 204, 180);

	private static readonly Color32 COMPLETE_SEGMENT_BACKGROUND_COLOR = new Color32(146, 231, 127, 180);

	private static readonly Color32 REWARD_BIG_BACKGROUND_FINISHED_COLOR = new Color32(90, 124, 90, byte.MaxValue);

	private static readonly Color32 REWARD_BIG_BACKGROUND_IN_PROGRESS_COLOR = new Color32(99, 116, 128, byte.MaxValue);

	private static readonly Color32 REWARD_BIG_BACKGROUND_COLLECTIBLE_COLOR = new Color32(91, 116, 134, byte.MaxValue);

	private static readonly Color32 EFFECT_INITIAL_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);

	[SerializeField]
	private AudioClip _awardTierUnlockSound;

	[SerializeField]
	private AwardCollectibleHelper _mainGoalElementsHelper;

	[SerializeField]
	private GameObject _expandedParent;

	[SerializeField]
	private UILabel _missionTitle;

	[SerializeField]
	private UILabel _missionTextDescription;

	[SerializeField]
	private GameObject _missionIconsDescriptionParent;

	[SerializeField]
	private UISprite _cellBackground;

	[SerializeField]
	private UISprite _contractedIconBG;

	[SerializeField]
	private GameObject _collectButtonParent;

	[SerializeField]
	private UILabel _collectButtonLabel;

	[SerializeField]
	private UISprite _progressIconsBG;

	[SerializeField]
	private GameObject _progressBarParent;

	[SerializeField]
	private UISlider _progressBarSlider;

	[SerializeField]
	private GameObject _progressBarFG;

	[SerializeField]
	private GameObject _tieredParent;

	[SerializeField]
	private AwardTierHelper _notTieredParent;

	[SerializeField]
	private UISprite _rewardBigBG;

	[SerializeField]
	private UILabel _previewCompleteLabel;

	[SerializeField]
	private GameObject _loginFBButton;

	[SerializeField]
	private GameObject _loginButtonContents;

	[SerializeField]
	private GameObject _connectingButtonContents;

	[SerializeField]
	private UISprite _loginFBButtonFill;

	[SerializeField]
	private UILabel _loginFBLabel;

	[SerializeField]
	private UILabel _connectingLabel;

	[SerializeField]
	private AwardTierHelper[] _tierIconHelpers = new AwardTierHelper[3];

	[SerializeField]
	private UISprite _tierEffectSprite;

	[SerializeField]
	private UISprite _noTierEffectSprite;

	[SerializeField]
	private AwardCollectibleHelper[] _goalIcons = new AwardCollectibleHelper[4];

	[SerializeField]
	private UISprite _mysteryBoxIcon;

	[SerializeField]
	private UILabel _inMysteryBoxLabel;

	[SerializeField]
	private UISprite _notTieredIcon;

	[SerializeField]
	private UILabel _progressAmount;

	[SerializeField]
	private GameObject _rewardContentParent;

	[SerializeField]
	private UILabel _rewardAmount;

	[SerializeField]
	private GameObject _stickerParent;

	[SerializeField]
	private UILabel _stickerLabel;

	[SerializeField]
	private UISprite _stickerIconSprite;

	[SerializeField]
	private UISprite _stickerSprite;

	[SerializeField]
	private UISprite _stickerShadowSprite;

	[SerializeField]
	private CenterLabelAndSpriteUnit centerer;

	[SerializeField]
	private Animation wiggleAnimation;

	[SerializeField]
	private BoxCollider _loginCollider;

	[SerializeField]
	private UISprite _socialLock;

	private string[] _mainTierGoalSpriteNames = new string[3] { "icon_bronze_can", "icon_silver_can", "icon_gold_can" };

	private string[] _mysteryBoxIconNames = new string[2] { "icon_small_mysteryBox", "icon_small_superMysteryBox" };

	private float soundVolume = 0.1f;

	private float soundPitch = 1f;

	private AwardCellState _currentCellState;

	private AwardsScreen _awardsScreen;

	private bool _hasManyTiers;

	private Award _currentAward;

	private bool _enablePreview;

	private float _animationLerpFactor;

	private bool _isConnectedToFacebook;

	private void OnDisable()
	{
		if (_currentAward != null)
		{
			_currentAward.isNew = false;
		}
	}

	public void InitCell(Award award, AwardsScreen awardsScreen)
	{
		if (award != null)
		{
			_currentAward = award;
			_enablePreview = award.isInPreview;
			_isConnectedToFacebook = SocialManager.instance.facebookIsLoggedIn;
			_currentCellState = GetCellState();
			_hasManyTiers = award.trophyData.missionTiers.Count > 1;
			_awardsScreen = awardsScreen;
		}
		SetupCellStructure();
		SetupCellContent();
		SetupCellColors();
		if (_enablePreview)
		{
			if (_hasManyTiers)
			{
				if (award.tierAnimationPlayed != (AwardTierType)(GetNbToAnimate() + 1))
				{
					StartCoroutine(AnimateChangingOfTierSprite(_tierEffectSprite, _tierIconHelpers[GetNbToAnimate()]));
					award.tierAnimationPlayed = (AwardTierType)(GetNbToAnimate() + 1);
				}
				else
				{
					_tierEffectSprite.gameObject.SetActive(false);
				}
			}
			else
			{
				StartCoroutine(AnimateChangingOfTierSprite(_noTierEffectSprite, _notTieredParent));
			}
		}
		if (_currentCellState == AwardCellState.Collectible)
		{
			wiggleAnimation.wrapMode = WrapMode.Loop;
			wiggleAnimation.Play();
		}
		else
		{
			wiggleAnimation.Stop();
		}
	}

	private int GetNbToAnimate()
	{
		int result = 0;
		switch (_currentAward.activeTierType)
		{
		case AwardTierType.Silver:
			result = 0;
			break;
		case AwardTierType.Gold:
			result = ((_currentAward.awardState == AwardState.Collectible || _currentAward.awardState == AwardState.InProgress) ? 1 : 2);
			break;
		}
		return result;
	}

	private AwardCellState GetCellState()
	{
		AwardCellState awardCellState = AwardCellState.NotSet;
		bool flag = _currentAward.trophyData.useFacebook && !_isConnectedToFacebook;
		switch (_currentAward.awardState)
		{
		case AwardState.InProgress:
			awardCellState = ((!_enablePreview) ? AwardCellState.InProgress : AwardCellState.PreviewInProgress);
			break;
		case AwardState.Collectible:
			awardCellState = AwardCellState.Collectible;
			break;
		case AwardState.Finished:
			awardCellState = ((!_enablePreview) ? AwardCellState.Finished : AwardCellState.PreviewFinished);
			break;
		default:
			awardCellState = AwardCellState.NotSet;
			break;
		}
		if (flag && (awardCellState == AwardCellState.InProgress || awardCellState == AwardCellState.PreviewInProgress))
		{
			awardCellState = AwardCellState.SocialNetworksLocked;
		}
		return awardCellState;
	}

	private void SetupCellStructure()
	{
		_expandedParent.SetActive(_currentCellState != AwardCellState.Finished);
		_missionTextDescription.gameObject.SetActive(_currentAward.trophyData.hasDescription);
		_missionIconsDescriptionParent.SetActive(!_currentAward.trophyData.hasDescription);
		_socialLock.gameObject.SetActive(_currentCellState == AwardCellState.SocialNetworksLocked);
		if (_expandedParent.activeSelf)
		{
			_tierEffectSprite.gameObject.SetActive(_enablePreview);
			_collectButtonParent.SetActive(_currentCellState == AwardCellState.Collectible);
			_progressBarParent.SetActive(_currentCellState == AwardCellState.InProgress || _currentCellState == AwardCellState.PreviewInProgress);
			_previewCompleteLabel.gameObject.SetActive(_currentCellState == AwardCellState.PreviewFinished);
			_notTieredParent.gameObject.SetActive(!_hasManyTiers);
			_tieredParent.SetActive(_hasManyTiers);
			_stickerParent.SetActive(_currentAward.isNew || _currentCellState == AwardCellState.Collectible);
			_stickerLabel.gameObject.SetActive(_currentAward.isNew && _currentCellState != AwardCellState.Collectible);
			_stickerIconSprite.gameObject.SetActive(_currentCellState == AwardCellState.Collectible);
			_rewardContentParent.SetActive(_currentCellState != AwardCellState.PreviewFinished && _currentCellState != AwardCellState.SocialNetworksLocked);
			_loginFBButton.gameObject.SetActive(_currentCellState == AwardCellState.SocialNetworksLocked);
			if (_currentCellState == AwardCellState.SocialNetworksLocked)
			{
				SetFBLoginButtonToNormalState();
			}
		}
	}

	private void SetupCellContent()
	{
		AwardGoal[] goals = _currentAward.goals;
		int progressAsPercentage = _currentAward.GetProgressAsPercentage();
		int num = (int)(_currentAward.activeTierType - 1);
		_missionTitle.text = Strings.Get(_currentAward.trophyData.title);
		bool flag = _currentCellState == AwardCellState.Finished || _currentCellState == AwardCellState.PreviewFinished;
		bool useGlitter = flag || _currentCellState == AwardCellState.Collectible;
		_mainGoalElementsHelper.SetupAwardCollectible(FindBackgroundToUse(true), _mainTierGoalSpriteNames[num], useGlitter, flag);
		if (_currentCellState == AwardCellState.PreviewFinished)
		{
			_previewCompleteLabel.text = Strings.Get(StringID.AWARD_PREVIEW_COMPLETED_TEXT);
		}
		if (_currentAward.trophyData.hasDescription)
		{
			StringID key = ((!_currentAward.trophyData.hasAnotherDescriptionForOneItemGoal || goals[0].goal != 1) ? _currentAward.trophyData.description : _currentAward.trophyData.descriptionForOneItemGoal);
			if (_currentCellState != AwardCellState.PreviewFinished && _currentCellState != AwardCellState.Finished)
			{
				_missionTextDescription.text = string.Format(Strings.Get(key), "[2486BB]", "[-]", "[004A7D]", goals[0].goal, "[-]", "[2486BB]", "[-]");
			}
			else
			{
				_missionTextDescription.text = string.Format(Strings.Get(key), "[549133]", "[-]", "[015A01]", goals[0].goal, "[-]", "[549133]", "[-]");
			}
		}
		else
		{
			for (int i = 0; i < goals.Length; i++)
			{
				AwardGoal awardGoal = goals[i];
				if (_currentCellState == AwardCellState.Collectible)
				{
					_goalIcons[i].SetupAwardCollectible(FindBackgroundToUse(false), awardGoal.iconName, false, true);
				}
				else
				{
					_goalIcons[i].SetupAwardCollectible(FindBackgroundToUse(false), awardGoal.iconName, false, _currentAward.isGoalAccomplished(awardGoal));
				}
			}
			if (_currentAward.trophyData.id == AwardId.COLLECT_OLD_TROPHY_ITEMS_MB)
			{
				_mysteryBoxIcon.spriteName = _mysteryBoxIconNames[0];
			}
			else if (_currentAward.trophyData.id == AwardId.COLLECT_OLD_TROPHY_ITEMS_SMB)
			{
				_mysteryBoxIcon.spriteName = _mysteryBoxIconNames[1];
			}
			if (_currentCellState != AwardCellState.PreviewFinished && _currentCellState != AwardCellState.Finished)
			{
				_inMysteryBoxLabel.text = string.Format(Strings.Get(StringID.AWARD_IN_MYSTERYBOX_TEXT), "[2486BB]", "[-]");
			}
			else
			{
				_inMysteryBoxLabel.text = string.Format(Strings.Get(StringID.AWARD_IN_MYSTERYBOX_TEXT), "[549133]", "[-]");
			}
		}
		if (!_expandedParent.activeSelf)
		{
			return;
		}
		_rewardAmount.text = _currentAward.reward.ToString();
		if (_progressBarParent.activeSelf)
		{
			float num2 = (float)progressAsPercentage / 100f;
			if (num2 > 0f)
			{
				_progressBarFG.SetActive(true);
				_progressBarSlider.value = num2;
			}
			else
			{
				_progressBarFG.SetActive(false);
			}
			_progressAmount.text = string.Format(Strings.Get(StringID.AWARD_PROGRESS_AMOUNT_FORMAT), progressAsPercentage);
		}
		if (_collectButtonParent.activeSelf)
		{
			_collectButtonLabel.text = Strings.Get(StringID.AWARDS_COLLECT_BUTTON_TEXT);
		}
		if (_loginFBButton.gameObject.activeSelf)
		{
			_loginFBLabel.text = Strings.Get(StringID.AWARD_SOCIAL_NETWORK_LOCKED_TEXT);
			_connectingLabel.text = Strings.Get(StringID.AWARDS_FACEBOOK_LOGIN_BUTTON_CONNECTING);
		}
		if (centerer != null && centerer.gameObject.activeInHierarchy && centerer.gameObject.activeSelf)
		{
			centerer.CenterUnit(0f);
		}
		for (int j = 0; j < _tierIconHelpers.Length; j++)
		{
			_tierIconHelpers[j].TierComplete(j < GetTierNumberToEnable());
		}
	}

	private void SetupCellColors()
	{
		_missionTextDescription.color = Color.white;
		switch (_currentCellState)
		{
		case AwardCellState.PreviewInProgress:
		case AwardCellState.Collectible:
			_cellBackground.color = COLLECTIBLE_SEGMENT_BACKGROUND_COLOR;
			_missionTitle.color = ACTIVE_TITLE_TEXT_COLOR;
			_missionTitle.effectColor = ACTIVE_TITLE_TEXT_SHADOW_COLOR;
			break;
		case AwardCellState.SocialNetworksLocked:
		case AwardCellState.InProgress:
			_cellBackground.color = PROGRESS_SEGMENT_BACKGROUND_COLOR;
			_missionTitle.color = ACTIVE_TITLE_TEXT_COLOR;
			_missionTitle.effectColor = ACTIVE_TITLE_TEXT_SHADOW_COLOR;
			break;
		case AwardCellState.Finished:
			_cellBackground.color = COMPLETE_SEGMENT_BACKGROUND_COLOR;
			_missionTitle.color = COMPLETED_TITLE_TEXT_COLOR;
			_missionTitle.effectColor = COMPLETED_TITLE_TEXT_SHADOW_COLOR;
			break;
		case AwardCellState.PreviewFinished:
			_cellBackground.color = COMPLETE_SEGMENT_BACKGROUND_COLOR;
			_missionTitle.color = COMPLETED_TITLE_TEXT_COLOR;
			_missionTitle.effectColor = COMPLETED_TITLE_TEXT_SHADOW_COLOR;
			break;
		}
		if (_currentCellState == AwardCellState.Collectible)
		{
			_stickerSprite.color = GlobalColors.AWARDS_COLLECTIBLE_STICKER_COLOR;
		}
		else if (_currentAward.isNew)
		{
			_stickerSprite.color = GlobalColors.AWARDS_NEW_STICKER_COLOR;
		}
		if (_currentCellState == AwardCellState.PreviewFinished)
		{
			_rewardBigBG.color = REWARD_BIG_BACKGROUND_FINISHED_COLOR;
			_progressIconsBG.color = REWARD_BIG_BACKGROUND_FINISHED_COLOR;
		}
		else if (_currentCellState == AwardCellState.Collectible)
		{
			_rewardBigBG.color = REWARD_BIG_BACKGROUND_COLLECTIBLE_COLOR;
			_progressIconsBG.color = REWARD_BIG_BACKGROUND_COLLECTIBLE_COLOR;
		}
		else
		{
			_rewardBigBG.color = REWARD_BIG_BACKGROUND_IN_PROGRESS_COLOR;
			_progressIconsBG.color = REWARD_BIG_BACKGROUND_IN_PROGRESS_COLOR;
		}
	}

	private string FindBackgroundToUse(bool mainGoalIcon)
	{
		string empty = string.Empty;
		switch (_currentCellState)
		{
		case AwardCellState.Collectible:
			return (!mainGoalIcon) ? "icon_award_collect_bg_small" : "icon_award_collect_bg";
		case AwardCellState.PreviewFinished:
		case AwardCellState.Finished:
			return (!mainGoalIcon) ? "icon_award_bg_completed_small" : "icon_award_bg_completed";
		default:
			return (!mainGoalIcon) ? "icon_award_bg_small" : "icon_award_bg";
		}
	}

	private int GetTierNumberToEnable()
	{
		int num = -1;
		if (_currentAward.HasCompletedTier(AwardTierType.Gold) && _currentAward.awardState == AwardState.Finished)
		{
			return (int)_currentAward.activeTierType;
		}
		return (int)(_currentAward.activeTierType - 1);
	}

	public void CollectRewardClicked()
	{
		Flurry.LogEventWithSeveralParameters("Award tier collected time", "Award;Tier;Time;Device", string.Concat(_currentAward.trophyData.id.ToString(), ";", (int)_currentAward.activeTierType, ";", DateTime.Now, ";", DeviceUtility.GetDeviceIdentifier()));
		CelebrationReward celebrationReward = new CelebrationReward();
		celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.Award;
		celebrationReward.rewardType = CelebrationRewardType.keys;
		celebrationReward.amount = _currentAward.reward;
		celebrationReward.awardTierType = _currentAward.activeTierType;
		celebrationReward.awardTitle = Strings.Get(_currentAward.trophyData.title);
		_currentAward.isInPreview = true;
		_currentAward.RewardPayedOut();
		RewardManager.AddRewardToUnlock(celebrationReward, false);
		PlayerInfo.Instance.hasCollectedFirstAwardReward = true;
		UIScreenController.Instance.QueuePopup("CelebrationPopup");
	}

	public void FacebookLoginClicked()
	{
		_awardsScreen.FacebookLoginClicked();
		SetFBLoginButtonToConnectingState();
	}

	private IEnumerator AnimateChangingOfTierSprite(UISprite effectSprite, AwardTierHelper tierIconToChange)
	{
		effectSprite.transform.localPosition = tierIconToChange.transform.localPosition;
		effectSprite.color = EFFECT_INITIAL_COLOR;
		NGUITools.PlaySound(_awardTierUnlockSound, soundVolume, soundPitch);
		yield return StartCoroutine(AnimateEffectColor(effectSprite, effectSprite.color, Color.white, 3f));
		tierIconToChange.TierComplete(true);
		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(AnimateEffectColor(effectSprite, effectSprite.color, EFFECT_INITIAL_COLOR, 0.8f));
	}

	private IEnumerator AnimateEffectColor(UISprite spriteToAnimate, Color color1, Color color2, float speedFactor = 1f)
	{
		_animationLerpFactor = 0f;
		while (_animationLerpFactor < 1f)
		{
			_animationLerpFactor += Time.deltaTime * speedFactor;
			spriteToAnimate.color = Color.Lerp(color1, color2, _animationLerpFactor);
			yield return null;
		}
	}

	private void SetFBLoginButtonToNormalState()
	{
		_loginButtonContents.SetActive(true);
		_connectingButtonContents.SetActive(false);
		_loginFBButtonFill.color = GlobalColors.PRIMARY_ACTION_COLOR;
		_loginCollider.enabled = true;
	}

	private void SetFBLoginButtonToConnectingState()
	{
		_loginButtonContents.SetActive(false);
		_connectingButtonContents.SetActive(true);
		_loginFBButtonFill.color = GlobalColors.SOCIAL_GREY;
		_loginCollider.enabled = false;
	}
}
