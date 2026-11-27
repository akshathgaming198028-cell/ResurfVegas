using System;
using System.Collections;
using System.Collections.Generic;
using Kiloo.Common;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Managers;
using SYBO.Subway.Info;
using UnityEngine;
using com.kiloo.awards;

public class CelebrationPopup : UIScreen
{
	private const float MOVE_IN_NEXT_CELEBRATION_DURATION = 0.35f;

	private const float REMOVE_CELEBRATION_POPUP_AFTER_OPEN = 0.7f;

	private const string POPUP_PATH = "Prefabs/Popups/";

	private const float BOX_IDLE_ANIM_SPEED = 2f;

	private const float BOX_IDLE_ANIM_AMOUNT = 10f;

	private const float BOX_IDLE_ANIM_END_SPEED = 20f;

	private const string STRIPE_COROUTINE_NAME = "AnimateStripes";

	private const float DOUBLE_UP_REWARD_DELAY = 0.5f;

	private const float DOUBLE_UP_SFX_VOLUME = 0.7f;

	public GameObject boxParent;

	public GameObject rewardLabelTemplate;

	private Vector3 _outOfScreenPosition = new Vector3(0f, -500f, 0f);

	private Vector3 _jackpotPosition = new Vector3(0f, -100f, 0f);

	[SerializeField]
	private GameObject prizeButtonNormal;

	[SerializeField]
	private GameObject prizeButtonSuper;

	[SerializeField]
	private GameObject prizeButtonMini;

	private readonly Vector3 _ShowPrizeButtonPosition = new Vector3(-90f, -46f, 0f);

	private readonly Vector3 _HidePrizeButtonPosition = new Vector3(-90f, 46f, 0f);

	[SerializeField]
	private Collider openButtonCollider;

	[SerializeField]
	private Collider skipButtonCollider;

	[SerializeField]
	private Collider continueButtonCollider;

	[SerializeField]
	private GameObject tapToStartLabelPrefab;

	private GameObject coinbox;

	private UILabel coinboxLabel;

	public UILabel newTrophyLabel;

	private static readonly Vector3 FIRST_SLOT_POSITION = new Vector3(0f, 15f, -50f);

	private static readonly Vector3 OTHER_SLOT_OFFSET = new Vector3(0f, 0f, 9370f);

	private static readonly Vector3 SUPERBOX_EFFECT_POSITION = new Vector3(0f, 0f, 180f);

	private GameObject[] slots = new GameObject[0];

	[SerializeField]
	private GameObject achievementBagPrefab;

	[SerializeField]
	private GameObject mysteryEggPrefab;

	[SerializeField]
	private GameObject mysteryBoxPrefab;

	[SerializeField]
	private GameObject superMysteryBoxPrefab;

	[SerializeField]
	private GameObject miniMysteryBoxPrefab;

	[SerializeField]
	private GameObject hiddenPrizePrefab_blue;

	[SerializeField]
	private GameObject hiddenPrizePrefab_green;

	[SerializeField]
	private GameObject hiddenPrizePrefab_red;

	[SerializeField]
	private GameObject superMysteryBoxEffectPrefab;

	[SerializeField]
	private GameObject roundGlowEffectPrefab;

	public GameObject testRewardPrefab;

	public GameObject rewardCoins;

	public GameObject rewardCoinsJackpot;

	[SerializeField]
	private GameObject rewardTokenTricky;

	[SerializeField]
	private GameObject rewardTokenFresh;

	[SerializeField]
	private GameObject rewardTokenSpike;

	[SerializeField]
	private GameObject rewardTokenYutani;

	[SerializeField]
	private GameObject trophyDiamond;

	[SerializeField]
	private GameObject trophyGoldBar;

	[SerializeField]
	private GameObject trophyChainClock;

	[SerializeField]
	private GameObject trophyChainDollar;

	[SerializeField]
	private GameObject trophyGoldSkull;

	[SerializeField]
	private GameObject trophyHeadphones;

	[SerializeField]
	private GameObject trophyLPBlack;

	[SerializeField]
	private GameObject trophyTapeBlack;

	[SerializeField]
	private GameObject rewardPowerupHoverboard;

	[SerializeField]
	private GameObject rewardPowerupHeadstart500;

	[SerializeField]
	private GameObject rewardPowerupHeadstart2000;

	[SerializeField]
	private GameObject rewardPowerupScoreBooster;

	[SerializeField]
	private GameObject rewardSpecialBoard;

	[SerializeField]
	private GameObject rewardKey;

	[SerializeField]
	private GameObject rewardBronzeCan;

	[SerializeField]
	private GameObject rewardSilverCan;

	[SerializeField]
	private GameObject rewardGoldCan;

	[SerializeField]
	private GameObject[] rewardsTopRun;

	[SerializeField]
	private GameObject itemHuntTopTitle;

	[SerializeField]
	private UILabel itemHuntTopTitleLabel;

	[SerializeField]
	private GameObject fleshEffectPrefab;

	[SerializeField]
	private GameObject preOpenLabel;

	private GameObject flashEffectGO;

	private UISprite flashEffectSprite;

	private UILabel tapToStartLabel;

	public AudioStateLoop audioStateLoop;

	private int _currentReward;

	private GameObject[] _celebrationRewardContainer;

	private bool anotherReward = true;

	private Vector3 _boxScale = new Vector3(1150f, 1150f, 1150f);

	private Vector3 _boxRotation = new Vector3(0f, 250f, 20f);

	private Vector3 _boxScaleAward = new Vector3(10f, 10f, 10f);

	private Vector3 _boxRotationAward = new Vector3(20f, 220f, 12f);

	private Vector3 _boxPositionAward = new Vector3(0f, 0f, 0f);

	private Vector3 _boxScaleTopRun = new Vector3(19f, 19f, 19f);

	private Vector3 _miniboxScale = new Vector3(900f, 900f, 900f);

	private Vector3 _boxRotationTopRun = new Vector3(0f, -180f, 0f);

	private Vector3 _boxPositionTopRun = new Vector3(5f, -97f, 0f);

	private Vector3 _rewardStartRotation = new Vector3(15f, -10.5f, 0f);

	private Vector3 _rewardStartScale = new Vector3(4f, 4f, 4f);

	private Vector3 _rewardStartScaleCans = new Vector3(7f, 7f, 7f);

	private Vector3 _rewardStartRotationJackpot = new Vector3(0f, 180f, 0f);

	private Vector3 _rewardStartRotationCans = new Vector3(15f, 180f, 0f);

	private Vector3 _rewardStartRotationTopRunRank = new Vector3(-10f, -180f, 0f);

	private Vector3 _rewardStartRotationTopRunMedal = new Vector3(-10f, -180f, 0f);

	private Vector3 _rewardStartPositionCans = new Vector3(0f, 0f, 0f);

	private Vector3 _rewardStartScaleJackpot = new Vector3(900f, 900f, 900f);

	private Vector3 _rewardEndScale_coin = new Vector3(10f, 10f, 10f);

	private Vector3 _rewardEndScale_coin_jackpot = new Vector3(1300f, 1300f, 1300f);

	private Vector3 _rewardEndScale = new Vector3(20f, 20f, 20f);

	private Vector3 _rewardEndScaleCan = new Vector3(35f, 35f, 35f);

	private Vector3 _rewardEndScaleTopRunRank = new Vector3(14f, 14f, 14f);

	private Vector3 _rewardEndPositionCan = new Vector3(0f, 0f, 0f);

	private Vector3 _rewardEndPositionTopRunRank = new Vector3(0f, -25f, 0f);

	private Vector3 _rewardEndPositionTopRunMedal = new Vector3(0f, 15f, 0f);

	private GameObject _rewardMain;

	private UILabel _labelSingle;

	private UILabel _labelDouble1;

	private Vector3 _labelPosition = new Vector3(0f, -110f, -5f);

	private Vector3 _labelPositionCelebration = new Vector3(0f, -160f, -5f);

	private Vector3 _continueLabelPosition = new Vector3(0f, -220f, -5f);

	private bool _maySetTimeScale;

	public GameObject GlowEffect;

	private Color32 EFFECT_INITIAL_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);

	private bool stopIdleAnim;

	private int _numberOfRewards;

	private CoinBoxSizer coinboxSizer;

	private CelebrationReward[] rewardsToUnlock;

	private bool celebrationHasStarted;

	private bool _isFingerPressed;

	private SuperMysteryBoxEffect _superMysteryBoxEffect;

	private SuperMysteryBoxEffect _roundGlowEffect;

	private float _timeScaleBeforeCelebrationPopup = 1f;

	[SerializeField]
	private Texture2D awardsBackground;

	[SerializeField]
	private Texture2D normalBackground;

	[SerializeField]
	private Texture2D itemHuntBackground;

	[SerializeField]
	private Texture2D itemHuntStripe;

	[SerializeField]
	private GameObject stripePanel;

	[SerializeField]
	private Texture2D characterEventBackgroundTexture;

	[SerializeField]
	private UITexture backgroundTexture;

	[SerializeField]
	private UIPanel _backgroundPanel;

	private UITexture[] _stripeTextures;

	private bool _stripeCoroutineRunning;

	private GameObject tapToStartGo;

	private bool _isRewardingJackpot;

	[SerializeField]
	private BragCelebrationHandler _bragCelebrationHandlerPrefab;

	private BragCelebrationHandler _currentBragCelebrationHandler;

	[SerializeField]
	private MysteryBoxDoubleUpButton _mysteryBoxDoubleUpButton;

	[SerializeField]
	private Vector3 _scaleTargetFactor = new Vector3(1.1f, 1.25f, 1.25f);

	[SerializeField]
	private UITweener.Method _scaleUpMethod = UITweener.Method.EaseOut;

	[SerializeField]
	private UITweener.Method _scaleDownMethod = UITweener.Method.EaseIn;

	[SerializeField]
	private float _scaleUpDuration = 0.0875f;

	[SerializeField]
	private float _scaleDownDurationFactor = 1.5f;

	[SerializeField]
	private AudioClip _doubleRewardClip;

	private Dictionary<CelebrationRewardType, Action> _doubleRewardCallbacks;

	private float _animationLerpFactor;

	private bool isWaitingForInput;

	private bool skipWaitBackButtonPressed;

	private CelebrationReward _rewardToDouble;

	private CelebrationPopupLabelTemplate _rewardTemplate;

	public override void Init()
	{
		base.Init();
		audioStateLoop = this.FindObject<AudioStateLoop>();
		coinbox = InitializeCoinbox(null, false, true, true, UIScreenController.CELEBRATION_POPUP_POSITION.x, UIScreenController.CELEBRATION_POPUP_POSITION.y, UIScreenController.CELEBRATION_POPUP_POSITION.z);
		coinboxSizer = coinbox.GetComponentInChildren<CoinBoxSizer>();
		coinboxLabel = coinboxSizer.coinAmountLabel;
		itemHuntTopTitleLabel.text = Strings.Get(StringID.CELEBRATION_ITEM_HUNT_TOP_TITLE);
		_doubleRewardCallbacks = new Dictionary<CelebrationRewardType, Action>
		{
			{
				CelebrationRewardType.coins,
				DoubleCoins
			},
			{
				CelebrationRewardType.keys,
				DoubleKeys
			},
			{
				CelebrationRewardType.token,
				DoubleTokens
			},
			{
				CelebrationRewardType.powerup,
				DoublePowerups
			}
		};
		tapToStartGo = NGUITools.AddChild(UIScreenController.Instance.CameraOverlay2d.gameObject, tapToStartLabelPrefab);
		tapToStartGo.transform.localPosition = _continueLabelPosition;
		tapToStartLabel = tapToStartGo.GetComponentInChildren<UILabel>();
		tapToStartGo.GetComponent<UIPanel>().depth = _backgroundPanel.depth + 1;
		flashEffectGO = NGUITools.AddChild(UIScreenController.Instance.CameraOverlay2d.gameObject, fleshEffectPrefab);
		flashEffectSprite = flashEffectGO.GetComponentInChildren<UISprite>();
		flashEffectGO.GetComponent<UIPanel>().depth = _backgroundPanel.depth + 1;
	}

	public override void Show()
	{
		base.Show();
		UIScreenController.Instance.OnPopupClosed += OnPopClosed;
		_timeScaleBeforeCelebrationPopup = SingletonMonobehavior<TimeScaleManager>.Instance.TimeScale;
		SingletonMonobehavior<TimeScaleManager>.Instance.TimeScale = SingletonMonobehavior<TimeScaleManager>.Instance.TimeScaleInMenuDefault;
		GlowEffect.GetComponent<MeshRenderer>().enabled = false;
		SetupCelebrationScreen();
	}

	public override void Hide()
	{
		base.Hide();
		UIScreenController.Instance.OnPopupClosed -= OnPopClosed;
		SingletonMonobehavior<TimeScaleManager>.Instance.TimeScale = _timeScaleBeforeCelebrationPopup;
		StopCoroutine("AnimateStripes");
		backgroundTexture.mainTexture = normalBackground;
		NGUITools.SetActive(tapToStartGo, false);
		NGUITools.SetActive(flashEffectGO, false);
	}

	private void Awake()
	{
		if (Application.isEditor && _bragCelebrationHandlerPrefab == null)
		{
			Debug.LogError("Variable _bragCelebrationHandler not set in CelebrationPopup");
		}
	}

	private void SetupCelebrationScreen()
	{
		boxParent.transform.position = UIModelController.Instance.CelebrationPopupAnchor.transform.position;
		rewardsToUnlock = RewardManager.GetRewardsToUnlockForCelebration();
		for (int i = 0; i < rewardsToUnlock.Length; i++)
		{
			if (rewardsToUnlock[i].CelebrationRewardOrigin == CelebrationRewardOrigin.MysteryBox)
			{
				Flurry.LogEvent("Mystery Box opened");
			}
			else if (rewardsToUnlock[i].CelebrationRewardOrigin == CelebrationRewardOrigin.SuperMysteryBox)
			{
				bool flag = false;
				if (PlayerInfo.Instance.willShowSMBAfterDailyChallenge)
				{
					flag = true;
					PlayerInfo.Instance.willShowSMBAfterDailyChallenge = false;
				}
				Flurry.LogEventWithAParameter("Super mystery box opened", "SMB from daily challenge", flag.ToString());
			}
			else if (rewardsToUnlock[i].CelebrationRewardOrigin == CelebrationRewardOrigin.MysteryBoxMini)
			{
				Flurry.LogEvent("Mini Mystery Box opened");
			}
		}
		_numberOfRewards = rewardsToUnlock.Length;
		if (_numberOfRewards <= 0)
		{
			anotherReward = false;
			Debug.LogError("You should not be here when do do not have any celebration items");
			UIScreenController.Instance.CloseActivePopups(screenName);
			openButtonCollider.enabled = false;
			return;
		}
		if (_numberOfRewards == 1)
		{
			anotherReward = false;
		}
		else
		{
			anotherReward = true;
		}
		TogglePrizeButtons(rewardsToUnlock[0].CelebrationRewardOrigin);
		NGUITools.SetActive(tapToStartGo, true);
		tapToStartLabel.alpha = 0f;
		NGUITools.SetActive(flashEffectGO, true);
		flashEffectSprite.alpha = 0f;
		newTrophyLabel.gameObject.SetActive(false);
		GameObject[] array = slots;
		foreach (GameObject obj in array)
		{
			UnityEngine.Object.Destroy(obj);
		}
		slots = new GameObject[_numberOfRewards];
		_celebrationRewardContainer = new GameObject[_numberOfRewards];
		for (int k = 0; k < _numberOfRewards; k++)
		{
			slots[k] = NGUITools.AddChild(boxParent);
			slots[k].transform.localPosition = FIRST_SLOT_POSITION + OTHER_SLOT_OFFSET * ((k > 0) ? 1 : 0);
		}
		FillOutAllTheSlotsWithRewardContainers();
		_currentReward = 0;
		if (superMysteryBoxEffectPrefab != null && roundGlowEffectPrefab != null)
		{
			GameObject gameObject = null;
			gameObject = ((rewardsToUnlock[_currentReward].CelebrationRewardOrigin == CelebrationRewardOrigin.Award || rewardsToUnlock[_currentReward].CelebrationRewardOrigin == CelebrationRewardOrigin.TopRun) ? NGUITools.AddChild(slots[_currentReward], roundGlowEffectPrefab) : NGUITools.AddChild(slots[_currentReward], superMysteryBoxEffectPrefab));
			gameObject.transform.transform.localPosition = SUPERBOX_EFFECT_POSITION;
			_superMysteryBoxEffect = gameObject.GetComponent<SuperMysteryBoxEffect>();
			if (_superMysteryBoxEffect == null)
			{
				Debug.LogError("CelebrationPopup ERROR: unable to find SuperMysteryBoxEffect component on superMysteryBoxEffectPrefab", this);
			}
			else
			{
				ToggleSuperMysteryBoxEffect(rewardsToUnlock[_currentReward].CelebrationRewardOrigin);
			}
		}
		else
		{
			Debug.LogError("CelebrationPopup ERROR: superMysteryBoxEffectPrefab is not assigned", this);
		}
		UpdateGui(rewardsToUnlock[_currentReward]);
		if (IsTwoStepsCelebration(_currentReward))
		{
			StartIdleAnimCoroutine(_celebrationRewardContainer[_currentReward].transform, rewardsToUnlock[_currentReward]);
		}
		else
		{
			StartOneStepUnlock();
		}
	}

	private void FillOutAllTheSlotsWithRewardContainers()
	{
		for (int i = 0; i < _numberOfRewards; i++)
		{
			GameObject gameObject = null;
			CelebrationRewardOrigin celebrationRewardOrigin = rewardsToUnlock[i].CelebrationRewardOrigin;
			bool flag = true;
			switch (celebrationRewardOrigin)
			{
			case CelebrationRewardOrigin.MysteryBox:
			case CelebrationRewardOrigin.WeeklyChallengeMysteryBox:
				gameObject = NGUITools.AddChild(slots[i], mysteryBoxPrefab);
				flag = false;
				break;
			case CelebrationRewardOrigin.SuperMysteryBox:
			case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
			case CelebrationRewardOrigin.CharacterEventSuperMysteryBox:
				gameObject = NGUITools.AddChild(slots[i], superMysteryBoxPrefab);
				flag = false;
				break;
			case CelebrationRewardOrigin.MysteryBoxMini:
				gameObject = NGUITools.AddChild(slots[i], miniMysteryBoxPrefab);
				flag = false;
				break;
			case CelebrationRewardOrigin.WeeklyChallengeHiddenPrize:
				if (WeeklyHuntManager.GetActivePeriode() == WeeklyHuntManager.PrizePeriodId.PeriodOne)
				{
					gameObject = NGUITools.AddChild(slots[i], hiddenPrizePrefab_blue);
				}
				else if (WeeklyHuntManager.GetActivePeriode() == WeeklyHuntManager.PrizePeriodId.PeriodTwo)
				{
					gameObject = NGUITools.AddChild(slots[i], hiddenPrizePrefab_green);
				}
				else if (WeeklyHuntManager.GetActivePeriode() == WeeklyHuntManager.PrizePeriodId.PeriodThree)
				{
					gameObject = NGUITools.AddChild(slots[i], hiddenPrizePrefab_red);
				}
				else
				{
					Debug.LogError("Weekly Hunt Period not available!");
					gameObject = NGUITools.AddChild(slots[i], hiddenPrizePrefab_blue);
				}
				flag = false;
				break;
			case CelebrationRewardOrigin.Award:
				gameObject = NGUITools.AddChild(slots[i], achievementBagPrefab);
				flag = false;
				break;
			case CelebrationRewardOrigin.TopRun:
				gameObject = ShowModel((Characters.CharacterType)PlayerInfo.Instance.currentCharacter, PlayerInfo.Instance.currentThemeIndex, UIModelController.ModelScreen.CelebrationTopRun);
				gameObject.transform.parent = slots[i].transform;
				flag = false;
				break;
			}
			if (!flag)
			{
				switch (celebrationRewardOrigin)
				{
				case CelebrationRewardOrigin.Award:
					gameObject.transform.localScale = _boxScaleAward;
					gameObject.transform.localRotation = Quaternion.Euler(_boxRotationAward);
					gameObject.transform.localPosition = _boxPositionAward;
					break;
				case CelebrationRewardOrigin.TopRun:
					gameObject.transform.localScale = _boxScaleTopRun;
					gameObject.transform.localRotation = Quaternion.Euler(_boxRotationTopRun);
					gameObject.transform.localPosition = _boxPositionTopRun;
					break;
				case CelebrationRewardOrigin.MysteryBoxMini:
					gameObject.transform.localScale = _miniboxScale;
					gameObject.transform.localRotation = Quaternion.Euler(_boxRotation);
					break;
				default:
					gameObject.transform.localScale = _boxScale;
					gameObject.transform.localRotation = Quaternion.Euler(_boxRotation);
					break;
				}
				Utility.SetLayerRecursively(gameObject.transform, boxParent.layer);
				_celebrationRewardContainer[i] = gameObject;
			}
			else
			{
				_celebrationRewardContainer[i] = null;
			}
		}
		GlowEffect.GetComponent<MeshRenderer>().material.SetColor("_MainColor", Color.black);
	}

	private void UpdateGui(CelebrationReward reward)
	{
		newTrophyLabel.gameObject.SetActive(false);
		switch (reward.CelebrationRewardOrigin)
		{
		case CelebrationRewardOrigin.MysteryBox:
		case CelebrationRewardOrigin.MysteryBoxMini:
			coinbox.gameObject.SetActive(true);
			ResetBackgroundToNormal();
			itemHuntTopTitle.SetActive(false);
			openButtonCollider.enabled = true;
			break;
		case CelebrationRewardOrigin.SuperMysteryBox:
			coinbox.gameObject.SetActive(true);
			ResetBackgroundToNormal();
			itemHuntTopTitle.SetActive(false);
			openButtonCollider.enabled = true;
			break;
		case CelebrationRewardOrigin.WeeklyChallengeNormal:
			coinbox.gameObject.SetActive(false);
			StartCoroutine("AnimateStripes");
			itemHuntTopTitle.SetActive(true);
			openButtonCollider.enabled = false;
			break;
		case CelebrationRewardOrigin.WeeklyChallengeMysteryBox:
			coinbox.gameObject.SetActive(false);
			StartCoroutine("AnimateStripes");
			itemHuntTopTitle.SetActive(true);
			openButtonCollider.enabled = true;
			break;
		case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
			coinbox.gameObject.SetActive(false);
			StartCoroutine("AnimateStripes");
			itemHuntTopTitle.SetActive(true);
			openButtonCollider.enabled = true;
			break;
		case CelebrationRewardOrigin.WeeklyChallengeHiddenPrize:
			coinbox.gameObject.SetActive(false);
			StartCoroutine("AnimateStripes");
			itemHuntTopTitle.SetActive(true);
			openButtonCollider.enabled = true;
			break;
		case CelebrationRewardOrigin.CharacterUnlock:
			coinbox.gameObject.SetActive(false);
			ResetBackgroundToNormal();
			itemHuntTopTitle.SetActive(false);
			openButtonCollider.enabled = false;
			break;
		case CelebrationRewardOrigin.BoardUnlock:
			coinbox.gameObject.SetActive(false);
			ResetBackgroundToNormal();
			itemHuntTopTitle.SetActive(false);
			openButtonCollider.enabled = false;
			break;
		case CelebrationRewardOrigin.NewHighScore:
			coinbox.gameObject.SetActive(false);
			ResetBackgroundToNormal();
			itemHuntTopTitle.SetActive(false);
			openButtonCollider.enabled = false;
			_currentBragCelebrationHandler = NGUITools.AddChild(UIScreenController.Instance.CameraOverlay2d.gameObject, _bragCelebrationHandlerPrefab.gameObject).GetComponent<BragCelebrationHandler>();
			Utility.SetLayerRecursively(_currentBragCelebrationHandler.gameObject.transform, UIScreenController.Instance.CameraOverlay2d.gameObject.layer);
			break;
		case CelebrationRewardOrigin.Award:
			coinbox.gameObject.SetActive(true);
			ResetBackgroundToNormal();
			itemHuntTopTitle.SetActive(false);
			openButtonCollider.enabled = true;
			backgroundTexture.mainTexture = awardsBackground;
			break;
		case CelebrationRewardOrigin.TopRun:
			coinbox.gameObject.SetActive(false);
			ResetBackgroundToNormal();
			itemHuntTopTitle.SetActive(false);
			openButtonCollider.enabled = true;
			backgroundTexture.mainTexture = awardsBackground;
			break;
		case CelebrationRewardOrigin.CharacterEventSuperMysteryBox:
			coinbox.gameObject.SetActive(true);
			ResetBackgroundToNormal();
			backgroundTexture.mainTexture = characterEventBackgroundTexture;
			itemHuntTopTitle.SetActive(false);
			openButtonCollider.enabled = true;
			SetPreOpenedCelebrationLabelTemplate(reward);
			break;
		}
	}

	private void SetPreOpenedCelebrationLabelTemplate(CelebrationReward reward)
	{
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterEventSuperMysteryBox)
		{
			preOpenLabel = NGUITools.AddChild(UIScreenController.Instance.CameraOverlay2d.gameObject, rewardLabelTemplate);
			preOpenLabel.transform.localPosition = _labelPosition;
			CelebrationPopupLabelTemplate component = preOpenLabel.GetComponent<CelebrationPopupLabelTemplate>();
			component.SetupCharacterWordEvent(reward.characterEventCompleted, reward.description);
		}
	}

	private void ToggleSuperMysteryBoxEffect(CelebrationRewardOrigin origin)
	{
		if (_superMysteryBoxEffect != null)
		{
			switch (origin)
			{
			case CelebrationRewardOrigin.SuperMysteryBox:
			case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
			case CelebrationRewardOrigin.CharacterEventSuperMysteryBox:
				_superMysteryBoxEffect.FastForwardEffect(3);
				_superMysteryBoxEffect.StartEffect();
				break;
			case CelebrationRewardOrigin.WeeklyChallengeMysteryBox:
				_superMysteryBoxEffect.FastForwardEffect(3);
				_superMysteryBoxEffect.StartEffect();
				break;
			case CelebrationRewardOrigin.WeeklyChallengeHiddenPrize:
				_superMysteryBoxEffect.FastForwardEffect(3);
				_superMysteryBoxEffect.StartEffect();
				break;
			case CelebrationRewardOrigin.Award:
				_superMysteryBoxEffect.FastForwardEffect(3);
				_superMysteryBoxEffect.StartEffect();
				break;
			default:
				_superMysteryBoxEffect.StopEffect();
				break;
			}
		}
	}

	private bool IsTwoStepsCelebration(int rewardIndex)
	{
		if (_celebrationRewardContainer[rewardIndex] == null)
		{
			return false;
		}
		return true;
	}

	private void StartIdleAnimCoroutine(Transform rewardTrans, CelebrationReward reward)
	{
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.TopRun)
		{
			tapToStartLabel.text = Strings.Get(StringID.CELEBRATION_POPUP_CONTINUE);
			tapToStartLabel.alpha = 1f;
			StartCoroutine(CharacterIdleAnimCoroutine(rewardTrans, reward));
		}
		else
		{
			tapToStartLabel.text = Strings.Get(StringID.CELEBRATION_POPUP_OPEN);
			tapToStartLabel.alpha = 1f;
			StartCoroutine(BoxIdleAnimCoroutine(rewardTrans, reward));
		}
	}

	private IEnumerator CharacterIdleAnimCoroutine(Transform rewardTrans, CelebrationReward reward)
	{
		while (!stopIdleAnim)
		{
			yield return null;
		}
	}

	private IEnumerator BoxIdleAnimCoroutine(Transform rewardTrans, CelebrationReward reward)
	{
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox)
		{
			audioStateLoop.PlayUnlockSound();
		}
		Vector3 baseLocalPos = rewardTrans.parent.localPosition;
		stopIdleAnim = false;
		float t = UnityEngine.Random.Range(0f, 6f);
		Vector3 newLocalPos = baseLocalPos;
		while (!stopIdleAnim)
		{
			t += Time.deltaTime;
			newLocalPos.y = baseLocalPos.y + Mathf.Sin(t * 2f) * 10f;
			rewardTrans.parent.localPosition = newLocalPos;
			yield return null;
		}
		bool doneResetting = false;
		while (!doneResetting)
		{
			newLocalPos.y = Mathf.MoveTowards(newLocalPos.y, baseLocalPos.y, Time.deltaTime * 20f);
			if (Mathf.Approximately(newLocalPos.y, baseLocalPos.y))
			{
				doneResetting = true;
			}
			rewardTrans.parent.localPosition = newLocalPos;
			yield return null;
		}
	}

	private IEnumerator StartIdleAnimationFor(Transform rewardTrans, Vector3 baseLocalPos)
	{
		if (rewardTrans != null)
		{
			float t = UnityEngine.Random.Range(0f, 6f);
			Vector3 newLocalPos = baseLocalPos;
			while (rewardTrans != null)
			{
				t += Time.deltaTime;
				newLocalPos.y = baseLocalPos.y + Mathf.Sin(t * 2f) * 10f;
				rewardTrans.localPosition = newLocalPos;
				yield return null;
			}
		}
	}

	private void OnApplicationPause(bool pause)
	{
		CelebrationRewardOrigin celebrationRewardOrigin = rewardsToUnlock[_currentReward].CelebrationRewardOrigin;
		if (!pause && _isFingerPressed && celebrationRewardOrigin != CelebrationRewardOrigin.TopRun)
		{
			FingerReleased();
		}
	}

	private void PlayTopRunAnimation(bool didHighFive)
	{
		if (celebrationHasStarted)
		{
			return;
		}
		celebrationHasStarted = true;
		openButtonCollider.enabled = false;
		if (!IsTwoStepsCelebration(_currentReward))
		{
			return;
		}
		CharacterModel component = _celebrationRewardContainer[_currentReward].GetComponent<CharacterModel>();
		if (!(component != null))
		{
			return;
		}
		TopRunIdleAnimation topRunIdleAnimation = component.TopRunIdleAnimation;
		if (!(topRunIdleAnimation != null))
		{
			return;
		}
		topRunIdleAnimation.AnimationFinished += OnTopRunAnimationFinished;
		if (didHighFive)
		{
			if (audioStateLoop != null)
			{
				audioStateLoop.PlayUnlockSound();
			}
			StartCoroutine(AnimateScreenFlash(flashEffectSprite));
			if (_superMysteryBoxEffect != null)
			{
				_superMysteryBoxEffect.StartEffect();
			}
			topRunIdleAnimation.TR_PlayHit();
		}
		else
		{
			if (audioStateLoop != null)
			{
				audioStateLoop.PlayCelebrationTapSound();
			}
			topRunIdleAnimation.TR_PlayMiss();
		}
	}

	public void OnTopRunAnimationFinished()
	{
		CharacterModel component = _celebrationRewardContainer[_currentReward].GetComponent<CharacterModel>();
		if (component != null)
		{
			TopRunIdleAnimation topRunIdleAnimation = component.TopRunIdleAnimation;
			if (topRunIdleAnimation != null)
			{
				topRunIdleAnimation.AnimationFinished -= OnTopRunAnimationFinished;
			}
		}
		StartCoroutine(_ShowReward(_currentReward));
	}

	private IEnumerator AnimateScreenFlash(UISprite effectSprite)
	{
		effectSprite.color = EFFECT_INITIAL_COLOR;
		yield return StartCoroutine(AnimateEffectColor(effectSprite, effectSprite.color, Color.white, 10f, true));
		StartCoroutine(AnimateEffectColor(effectSprite, effectSprite.color, EFFECT_INITIAL_COLOR, 10f));
	}

	public void FingerPressed()
	{
		if (_isFingerPressed)
		{
			return;
		}
		_isFingerPressed = true;
		stopIdleAnim = true;
		Animation animation = null;
		if (IsTwoStepsCelebration(_currentReward))
		{
			animation = _celebrationRewardContainer[_currentReward].GetComponentInChildren<Animation>();
			AnimationState downState = FindClipState(animation, "down");
			if (downState != null)
			{
				animation.Play(downState.name);
			}
		}
	}

	private void StartOneStepUnlock()
	{
		FingerReleased();
	}

	public void FingerReleased()
	{
		if (preOpenLabel != null)
		{
			UnityEngine.Object.Destroy(preOpenLabel);
		}
		CelebrationRewardOrigin celebrationRewardOrigin = rewardsToUnlock[_currentReward].CelebrationRewardOrigin;
		if (celebrationRewardOrigin != CelebrationRewardOrigin.TopRun)
		{
			if (!celebrationHasStarted)
			{
				openButtonCollider.enabled = false;
				HidePrizeButtons();
				if (_superMysteryBoxEffect != null && celebrationRewardOrigin != CelebrationRewardOrigin.Award)
				{
					_superMysteryBoxEffect.StopEffect();
				}
				StartCoroutine(AnimateAlpha(tapToStartLabel, 0.5f, 0f));
				StartCoroutine(_ShowReward(_currentReward));
				celebrationHasStarted = true;
			}
		}
		else
		{
			StartCoroutine(AnimateAlpha(tapToStartLabel, 0.5f, 0f));
			PlayTopRunAnimation(true);
		}
	}

	private IEnumerator _ShowReward(int currentRewardIndex)
	{
		CelebrationReward reward = rewardsToUnlock[currentRewardIndex];
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.MysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeMysteryBox)
		{
			Statistics stats;
			Statistics statistics = (stats = PlayerInfo.Instance.stats);
			Stat stat = Stat.MysteryBoxesOpened;
			int num = stats[stat];
			statistics[Stat.MysteryBoxesOpened] = num + 1;
		}
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.SuperMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterEventSuperMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox)
		{
			Statistics stats2;
			Statistics statistics2 = (stats2 = PlayerInfo.Instance.stats);
			Stat stat = Stat.SuperMysteryBoxesOpened;
			int num = stats2[stat];
			statistics2[Stat.SuperMysteryBoxesOpened] = num + 1;
		}
		GlowEffect.GetComponent<MeshRenderer>().enabled = true;
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.TopRun)
		{
			if (audioStateLoop != null)
			{
				audioStateLoop.PlayRewardMysterySound();
			}
			StartCoroutine(AnimateColor(GlowEffect.GetComponent<MeshRenderer>().material, 1.5f, Color.white));
			StartCoroutine(RotateGameObject(GlowEffect.transform, 3000f, new Vector3(0f, 0f, -270000f)));
			yield return new WaitForSeconds(0.4f);
			flashEffectSprite.color = EFFECT_INITIAL_COLOR;
			yield return StartCoroutine(AnimateEffectColor(flashEffectSprite, flashEffectSprite.color, Color.white, 4f));
			_celebrationRewardContainer[currentRewardIndex].SetActive(false);
			if (_superMysteryBoxEffect != null)
			{
				_superMysteryBoxEffect.StartEffect();
			}
			StartCoroutine(AnimateEffectColor(flashEffectSprite, flashEffectSprite.color, EFFECT_INITIAL_COLOR, 0.4f));
		}
		GameObject rewardGo = null;
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterUnlock)
		{
			ShowModel(reward.characterType, reward.characterOutfitIndex, UIModelController.ModelScreen.CelebrationCharacterUnlock);
			if (audioStateLoop != null)
			{
				audioStateLoop.PlayUnlockSound();
			}
		}
		else if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.BoardUnlock)
		{
			ShowBackgroundWithStripes(reward.boardType, UIModelController.ModelScreen.CelebrationBoardUnlock);
			if (audioStateLoop != null)
			{
				audioStateLoop.PlayUnlockSound();
			}
		}
		else if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.NewHighScore)
		{
			if (_currentBragCelebrationHandler.state == BragCelebrationHandler.State.BeatenOnlySelf)
			{
				ShowBackgroundWithStripes(PlayerInfo.Instance.currentHoverboard, UIModelController.ModelScreen.CelebrationHighScore);
			}
			else
			{
				UIModelController.Instance.SetDiscoLightBackground();
			}
			if (audioStateLoop != null)
			{
				audioStateLoop.PlayUnlockSound();
			}
		}
		else
		{
			rewardGo = NGUITools.AddChild(prefab: ChooseRewardPrefab(reward), parent: slots[0]);
			if (isJackpot(reward))
			{
				rewardGo.transform.localScale = _rewardStartScaleJackpot;
				rewardGo.transform.localRotation = Quaternion.Euler(_rewardStartRotationJackpot);
			}
			else if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.Award)
			{
				rewardGo.transform.localScale = _rewardStartScaleCans;
				rewardGo.transform.localPosition = _rewardStartPositionCans;
				rewardGo.transform.localRotation = Quaternion.Euler(_rewardStartRotationCans);
			}
			else if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.TopRun)
			{
				rewardGo.transform.localScale = _rewardStartScale;
				if (reward.rank > 4)
				{
					rewardGo.transform.localRotation = Quaternion.Euler(_rewardStartRotationTopRunRank);
				}
				else
				{
					rewardGo.transform.localRotation = Quaternion.Euler(_rewardStartRotationTopRunMedal);
				}
			}
			else
			{
				rewardGo.transform.localScale = _rewardStartScale;
				rewardGo.transform.localRotation = Quaternion.Euler(_rewardStartRotation);
			}
			Utility.SetLayerRecursively(rewardGo.transform, boxParent.layer);
			if (audioStateLoop != null)
			{
				if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeNormal)
				{
					audioStateLoop.PlayUnlockSound();
				}
				else if (reward.CelebrationRewardOrigin != CelebrationRewardOrigin.TopRun)
				{
					audioStateLoop.PlayCelebrationOpenSound();
				}
			}
			Animation animation = null;
			if (IsTwoStepsCelebration(currentRewardIndex))
			{
				animation = _celebrationRewardContainer[currentRewardIndex].GetComponentInChildren<Animation>();
				AnimationState upState = FindClipState(animation, "up");
				if (upState != null)
				{
					animation.Play(upState.name);
					while (upState.normalizedTime < 0.5f)
					{
						yield return null;
					}
				}
			}
			_maySetTimeScale = true;
			if (reward.rewardType == CelebrationRewardType.coins)
			{
				if (isJackpot(reward))
				{
					StartCoroutine(ScaleGameObject(rewardGo.transform, 2f, _rewardEndScale_coin_jackpot));
					Animation animationJackpot = rewardGo.GetComponentInChildren<Animation>();
					if (animationJackpot != null && (bool)animationJackpot["open_superJackpot"])
					{
						animationJackpot.Play("open_superJackpot");
						StartCoroutine(MoveGameObject(rewardGo.transform, 1f, _jackpotPosition));
					}
				}
				else
				{
					StartCoroutine(ScaleGameObject(rewardGo.transform, 2f, _rewardEndScale_coin));
				}
			}
			else if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.Award)
			{
				StartCoroutine(ScaleGameObject(rewardGo.transform, 2f, _rewardEndScaleCan));
				StartCoroutine(MoveGameObject(rewardGo.transform, 2f, _rewardEndPositionCan));
			}
			else if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.TopRun)
			{
				if (reward.rank > 4)
				{
					StartCoroutine(ScaleGameObject(rewardGo.transform, 0.3f, _rewardEndScaleTopRunRank));
					StartCoroutine(MoveGameObject(rewardGo.transform, 0.5f, _rewardEndPositionTopRunRank));
				}
				else
				{
					StartCoroutine(ScaleGameObject(rewardGo.transform, 1.5f, _rewardEndScaleTopRunRank));
					StartCoroutine(MoveGameObject(rewardGo.transform, 1.5f, _rewardEndPositionTopRunMedal));
				}
			}
			else
			{
				StartCoroutine(ScaleGameObject(rewardGo.transform, 2f, _rewardEndScale));
			}
			if (IsTwoStepsCelebration(currentRewardIndex))
			{
				StartCoroutine(MoveGameObject(_celebrationRewardContainer[currentRewardIndex].transform, 0.7f, _outOfScreenPosition));
			}
			if (!isJackpot(reward))
			{
				if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.Award)
				{
					StartCoroutine(RotateGameObject(rewardGo.transform, 2f, new Vector3(0f, 1440f, 0f)));
				}
				else if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.TopRun)
				{
					if (reward.rank < 5)
					{
						StartCoroutine(RotateGameObject(rewardGo.transform, 2f, new Vector3(0f, 1440f, 0f)));
						StartCoroutine(StartIdleAnimationFor(rewardGo.transform, _rewardEndPositionTopRunMedal));
					}
					else
					{
						StartCoroutine(StartIdleAnimationFor(rewardGo.transform, _rewardEndPositionTopRunRank));
					}
				}
				else
				{
					StartCoroutine(RotateGameObject(rewardGo.transform, 4f, new Vector3(0f, 1500f, 0f)));
				}
			}
			yield return new WaitForSeconds(0.5f);
			if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.Award)
			{
				StartCoroutine(AnimateColor(GlowEffect.GetComponent<MeshRenderer>().material, 1.5f, Color.white));
				StartCoroutine(RotateGameObject(GlowEffect.transform, 3000f, new Vector3(0f, 0f, -270000f)));
			}
			else if (reward.CelebrationRewardOrigin != CelebrationRewardOrigin.TopRun)
			{
				StartCoroutine(AnimateColor(GlowEffect.GetComponent<MeshRenderer>().material, 1.5f, Color.white));
				StartCoroutine(RotateGameObject(GlowEffect.transform, 3f, new Vector3(0f, 0f, -270f)));
			}
			yield return new WaitForSeconds(1.3f);
		}
		GameObject labelGo = NGUITools.AddChild(UIScreenController.Instance.CameraOverlay2d.gameObject, rewardLabelTemplate);
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.BoardUnlock || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterUnlock)
		{
			labelGo.transform.localPosition = _labelPositionCelebration;
		}
		else
		{
			labelGo.transform.localPosition = _labelPosition;
		}
		CelebrationPopupLabelTemplate template = labelGo.GetComponent<CelebrationPopupLabelTemplate>();
		template.Init(_backgroundPanel.depth);
		if (_stripeCoroutineRunning)
		{
			template.bigLabel.color = GlobalColors.WEEKLYHUNT_CELEBRATION_TEXT_COLOR_STRIPED_BACKGROUND;
			template.subLabel.color = GlobalColors.WEEKLYHUNT_CELEBRATION_TEXT_COLOR_STRIPED_BACKGROUND;
			template.bigLabel.effectColor = GlobalColors.WEEKLYHUNT_CELEBRATION_TEXT_SHADOW_COLOR_STRIPED_BACKGROUND;
			template.subLabel.effectColor = GlobalColors.WEEKLYHUNT_CELEBRATION_TEXT_SHADOW_COLOR_STRIPED_BACKGROUND;
		}
		else
		{
			template.bigLabel.color = GlobalColors.WEEKLYHUNT_CELEBRATION_TEXT_COLOR;
			template.subLabel.color = GlobalColors.WEEKLYHUNT_CELEBRATION_TEXT_COLOR;
			template.bigLabel.effectColor = GlobalColors.WEEKLYHUNT_CELEBRATION_TEXT_SHADOW_COLOR;
			template.subLabel.effectColor = GlobalColors.WEEKLYHUNT_CELEBRATION_TEXT_SHADOW_COLOR;
		}
		bool showNewTrophyLabel = false;
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.Award)
		{
			template.SetupAward(reward.awardTitle, reward.amount, reward.awardTierType);
		}
		else if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.TopRun)
		{
			template.SetupTopRun(reward.score, reward.rank, reward.weekNumber);
		}
		else if (reward.rewardType == CelebrationRewardType.coins)
		{
			template.SetupCoins();
		}
		else if (reward.rewardType == CelebrationRewardType.powerup)
		{
			template.SetupPowerup(reward.powerupType, reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.token)
		{
			template.SetupToken(tokensLeft: Characters.characterData[reward.characterType].Price - PlayerInfo.Instance.GetCollectedTokens(reward.characterType) - reward.amount, characterType: reward.characterType, amount: reward.amount);
			MissionsManager.Instance.PlayerDidThis(MissionTarget.Tokens, reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.trophy)
		{
			showNewTrophyLabel = true;
			template.SetupTrophy(reward.trophyType);
		}
		else if (reward.rewardType == CelebrationRewardType.medal)
		{
			showNewTrophyLabel = true;
			template.SetupMedal();
		}
		else if (reward.rewardType == CelebrationRewardType.keys)
		{
			template.SetupKeys(reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.character)
		{
			CharacterTheme? modelTheme = CharacterThemes.GetThemeForCharacter(reward.characterType, reward.characterOutfitIndex);
			if (modelTheme.HasValue)
			{
				template.SetupCharacter(Strings.Get(modelTheme.Value.title));
			}
			else
			{
				template.SetupCharacter(Strings.Get(Characters.characterData[reward.characterType].name));
			}
		}
		else if (reward.rewardType == CelebrationRewardType.specialBoard)
		{
			bool eventBoardTypeWasFound = false;
			Hoverboards.BoardType? eventBoardType = null;
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			if (activeEventManager != null)
			{
				SSEventType activeEventType = activeEventManager.GetEventType();
				if (activeEventType == SSEventType.SpecialPowerBoard)
				{
					eventBoardType = activeEventManager.GetEventValue<Hoverboards.BoardType>(out eventBoardTypeWasFound);
				}
			}
			if (eventBoardTypeWasFound && eventBoardType.GetValueOrDefault() == reward.boardType && eventBoardType.HasValue)
			{
				template.SetupEventSpecialBoard(Strings.Get(Hoverboards.boardData[reward.boardType].name));
			}
			else if (reward.boardPower == Hoverboard.BoardAbility.None)
			{
				template.SetupSpecialBoard(Strings.Get(Hoverboards.boardData[reward.boardType].name));
			}
			else
			{
				string powerName = HoverboardThemes.GetBoardPowerName(reward.boardType, reward.boardPower);
				template.SetupSpecialBoard(powerName);
			}
		}
		else if (reward.rewardType == CelebrationRewardType.highscore)
		{
			labelGo.SetActive(false);
		}
		newTrophyLabel.gameObject.SetActive(showNewTrophyLabel);
		if (reward.rewardType != CelebrationRewardType.highscore)
		{
			StartCoroutine(AnimateAlpha(template, 0.2f, 1f));
		}
		yield return new WaitForSeconds(0.5f);
		skipButtonCollider.enabled = false;
		int coinboxCountFromAmount = PlayerInfo.Instance.amountOfCoins;
		PayoutReward(reward);
		if (reward.rewardType == CelebrationRewardType.coins)
		{
			yield return StartCoroutine(CountUpCoins(isMegaJackpot: reward.amount > MysteryBox.superMysteryBoxJackpotAmount, coinboxAmount: coinboxCountFromAmount, toAmount: reward.amount, rewardTemplate: template));
		}
		else if (reward.CelebrationRewardOrigin != CelebrationRewardOrigin.CharacterUnlock && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.BoardUnlock && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.Award && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.TopRun)
		{
			yield return new WaitForSeconds(1f);
		}
		if (_mysteryBoxDoubleUpButton.TryEnable(reward, _doubleRewardCallbacks))
		{
			_rewardToDouble = reward;
			_rewardTemplate = template;
		}
		if (reward.CelebrationRewardOrigin != CelebrationRewardOrigin.Award && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.TopRun)
		{
			StartCoroutine(AnimateColor(GlowEffect.GetComponent<MeshRenderer>().material, 0.5f, Color.black));
		}
		if (reward.CelebrationRewardOrigin != CelebrationRewardOrigin.CharacterUnlock && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.BoardUnlock && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.Award && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.TopRun)
		{
			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.75f));
		}
		tapToStartLabel.text = Strings.Get(StringID.CELEBRATION_POPUP_CONTINUE);
		StartCoroutine(AnimateAlpha(tapToStartLabel, 0.5f, 1f));
		_maySetTimeScale = false;
		SingletonMonobehavior<TimeScaleManager>.Instance.TimeScale = SingletonMonobehavior<TimeScaleManager>.Instance.TimeScaleInMenuDefault;
		isWaitingForInput = true;
		while (ShouldWaitForInput())
		{
			yield return null;
		}
		isWaitingForInput = false;
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterUnlock)
		{
			HideModel();
		}
		else if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.BoardUnlock)
		{
			HideSpecialBoard();
		}
		else if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.NewHighScore)
		{
			HideModel();
		}
		UnityEngine.Object.Destroy(rewardGo);
		UnityEngine.Object.Destroy(labelGo);
		UnityEngine.Object.Destroy(_celebrationRewardContainer[currentRewardIndex]);
		if (_currentBragCelebrationHandler != null)
		{
			UnityEngine.Object.Destroy(_currentBragCelebrationHandler.gameObject);
			_currentBragCelebrationHandler = null;
		}
		_FinishOpening();
	}

	private void PayoutReward(CelebrationReward reward, bool allowRemove = true)
	{
		if (reward.rewardType == CelebrationRewardType.topRun)
		{
			UIScreenController.Instance.justPaidOutTopRun = true;
			UIScreenController.Instance.PayoutCelebrationReward(reward);
		}
		else if (reward.rewardType == CelebrationRewardType.powerup)
		{
			PlayerInfo.Instance.IncreaseUpgradeAmount(reward.powerupType, reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.token)
		{
			PlayerInfo.Instance.CollectToken(reward.characterType, reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.trophy)
		{
			PlayerInfo.Instance.OldTrophySpawned(reward.trophyType);
		}
		else if (reward.rewardType == CelebrationRewardType.keys)
		{
			PlayerInfo.Instance.amountOfKeys += reward.amount;
		}
		else if (reward.rewardType == CelebrationRewardType.coins)
		{
			int amount = reward.amount;
			coinboxSizer.updateAutomatically = false;
			PlayerInfo.Instance.amountOfCoins += amount;
			MissionsManager.Instance.PlayerDidThis(MissionTarget.EarnCoin, amount);
			if (isJackpot(reward) && SocialManager.instance.facebookIsLoggedIn)
			{
				Statistics stats;
				Statistics statistics = (stats = PlayerInfo.Instance.stats);
				Stat stat2;
				Stat stat = (stat2 = Stat.JackpotsWon);
				int num = stats[stat2];
				statistics[stat] = num + 1;
			}
		}
		if (allowRemove)
		{
			RewardManager.RewardPayedOut(reward);
		}
		PlayerInfo.Instance.SaveIfDirty();
		_isRewardingJackpot = false;
	}

	private void _FinishOpening()
	{
		_rewardToDouble = null;
		_rewardTemplate = null;
		celebrationHasStarted = false;
		_isFingerPressed = false;
		if (_superMysteryBoxEffect != null)
		{
			_superMysteryBoxEffect.StopEffect();
		}
		if (anotherReward)
		{
			StartCoroutine(MoveNextBoxToFront());
			return;
		}
		GameObject[] array = slots;
		foreach (GameObject obj in array)
		{
			UnityEngine.Object.Destroy(obj);
		}
		ResetBackgroundToNormal();
		skipButtonCollider.enabled = true;
		openButtonCollider.enabled = false;
		NGUITools.SetActive(tapToStartGo, false);
		NGUITools.SetActive(flashEffectGO, false);
		if (RewardManager.rewardsToUnlockCount > 0 && RewardManager.canShowMultipleQueuedCelebrations)
		{
			SetupCelebrationScreen();
			return;
		}
		if (RewardManager.canShowMultipleQueuedCelebrations)
		{
			RewardManager.canShowMultipleQueuedCelebrations = false;
		}
		UIScreenController.Instance.CloseActivePopups(screenName);
	}

	private IEnumerator MoveNextBoxToFront()
	{
		SingletonMonobehavior<TimeScaleManager>.Instance.TimeScale = SingletonMonobehavior<TimeScaleManager>.Instance.TimeScaleInMenuDefault;
		_currentReward++;
		if (_currentReward >= _numberOfRewards - 1)
		{
			anotherReward = false;
		}
		UpdateGui(rewardsToUnlock[_currentReward]);
		TogglePrizeButtons(rewardsToUnlock[_currentReward].CelebrationRewardOrigin);
		GlowEffect.GetComponent<MeshRenderer>().material.SetColor("_MainColor", Color.black);
		ToggleSuperMysteryBoxEffect(rewardsToUnlock[_currentReward].CelebrationRewardOrigin);
		openButtonCollider.enabled = false;
		if (IsTwoStepsCelebration(_currentReward))
		{
			_celebrationRewardContainer[_currentReward].transform.parent = slots[0].transform;
			if (rewardsToUnlock[_currentReward].CelebrationRewardOrigin != CelebrationRewardOrigin.TopRun)
			{
				StartCoroutine(MoveGameObject(_celebrationRewardContainer[_currentReward].transform, 0.35f, Vector3.zero));
			}
			else
			{
				StartCoroutine(MoveGameObject(_celebrationRewardContainer[_currentReward].transform, 0.35f, _boxPositionTopRun));
			}
			StartIdleAnimCoroutine(_celebrationRewardContainer[_currentReward].transform, rewardsToUnlock[_currentReward]);
			yield return new WaitForSeconds(0.35f);
			openButtonCollider.enabled = true;
		}
		else
		{
			FingerReleased();
		}
		skipButtonCollider.enabled = true;
	}

	private IEnumerator RotateGameObject(Transform trans, float duration, Vector3 angleToRotate)
	{
		if (trans != null)
		{
			Quaternion fromRotation = trans.localRotation;
			float factor = 0f;
			while (factor < 1f && trans != null)
			{
				factor += Time.deltaTime / duration;
				factor = Mathf.Clamp01(factor);
				float angle = 0f;
				angle = Mathf.Lerp(4.712389f, (float)Math.PI * 2f, factor);
				float cosFactor = Mathf.Cos(angle) * 0.5f + 0.5f;
				trans.localRotation = fromRotation;
				trans.Rotate(angleToRotate * cosFactor, Space.World);
				yield return null;
			}
		}
	}

	private IEnumerator CountUpCoins(int coinboxAmount, int toAmount, CelebrationPopupLabelTemplate rewardTemplate, bool isMegaJackpot = false, int fromAmount = 0, float countTimeMultiplier = 1f, bool doubled = false)
	{
		coinboxSizer.updateAutomatically = false;
		float countFactor = 0f;
		float countTime = CalcCoinCountTime(toAmount, fromAmount, countTimeMultiplier);
		int coinboxTo = coinboxAmount + toAmount - fromAmount;
		yield return new WaitForSeconds(0.5f);
		while (countFactor < 1f)
		{
			countFactor += Time.deltaTime / countTime;
			rewardTemplate.UpdateCoins(Mathf.RoundToInt(Mathf.SmoothStep(fromAmount, toAmount, countFactor)), isMegaJackpot, doubled);
			coinboxLabel.text = Mathf.RoundToInt(Mathf.SmoothStep(coinboxAmount, coinboxTo, countFactor)).ToString();
			coinboxSizer.AdjustCoinsSegmentSize();
			yield return null;
		}
		coinboxSizer.updateAutomatically = true;
	}

	private float CalcCoinCountTime(int toAmount, int fromAmount, float countTimeMultiplier)
	{
		return Mathf.Lerp(1.5f, 4f, (float)(toAmount - fromAmount) / 100000f) * countTimeMultiplier;
	}

	private IEnumerator AnimateAlpha(UILabel label, float duration, float toAlpha)
	{
		float fromAlpha = label.alpha;
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			label.alpha = Mathf.Lerp(fromAlpha, toAlpha, factor);
			yield return null;
		}
	}

	private IEnumerator AnimateAlpha(CelebrationPopupLabelTemplate template, float duration, float toAlpha)
	{
		float fromAlpha = template.Alpha;
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			template.Alpha = Mathf.Lerp(fromAlpha, toAlpha, factor);
			yield return null;
		}
	}

	private IEnumerator AnimateColor(UIWidget widget, float duration, Color toColor)
	{
		Color fromColor = widget.color;
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			widget.color = Color.Lerp(fromColor, toColor, factor);
			yield return null;
		}
	}

	private IEnumerator AnimateColor(Material material, float duration, Color toColor)
	{
		Color fromColor = material.GetColor("_MainColor");
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			material.SetColor("_MainColor", Color.Lerp(fromColor, toColor, factor));
			yield return null;
		}
	}

	private IEnumerator AnimateEffectColor(UISprite spriteToAnimate, Color color1, Color color2, float speedFactor = 1f, bool instant = false)
	{
		if (instant)
		{
			spriteToAnimate.color = color2;
			yield break;
		}
		_animationLerpFactor = 0f;
		while (_animationLerpFactor < 1f)
		{
			_animationLerpFactor += Time.deltaTime * speedFactor;
			spriteToAnimate.color = Color.Lerp(color1, color2, _animationLerpFactor);
			yield return null;
		}
	}

	private IEnumerator MoveGameObject(Transform trans, float duration, Vector3 toPos)
	{
		if (trans != null)
		{
			Vector3 fromPos = trans.localPosition;
			float factor = 0f;
			while (factor < 1f && trans != null)
			{
				factor += Time.deltaTime / duration;
				factor = Mathf.Clamp01(factor);
				trans.localPosition = Vector3.Lerp(fromPos, toPos, 0.5f * (Mathf.Sin((factor - 0.5f) * (float)Math.PI) + 1f));
				yield return null;
			}
			if (trans != null)
			{
				trans.localPosition = toPos;
			}
		}
	}

	private IEnumerator ScaleGameObject(Transform trans, float duration, Vector3 toScale)
	{
		if (trans != null)
		{
			float factor = 0f;
			Vector3 fromScale = trans.localScale;
			while (factor < 1f && trans != null)
			{
				factor += Time.deltaTime / duration;
				factor = Mathf.Clamp01(factor);
				float angle = 0f;
				angle = Mathf.Lerp((float)Math.PI, (float)Math.PI * 2f, factor);
				float cosFactor = Mathf.Cos(angle) * 0.5f + 0.5f;
				trans.localScale = Vector3.Lerp(fromScale, toScale, cosFactor);
				yield return null;
			}
		}
	}

	private GameObject ChooseRewardPrefab(CelebrationReward reward)
	{
		GameObject result = rewardCoins;
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.Award)
		{
			switch (reward.awardTierType)
			{
			case AwardTierType.Bronze:
				result = rewardBronzeCan;
				break;
			case AwardTierType.Silver:
				result = rewardSilverCan;
				break;
			case AwardTierType.Gold:
				result = rewardGoldCan;
				break;
			}
			return result;
		}
		if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.TopRun)
		{
			int num = reward.rank - 1;
			if (num <= rewardsTopRun.Length)
			{
				return rewardsTopRun[num];
			}
			result = rewardsTopRun[rewardsTopRun.Length - 1];
		}
		switch (reward.rewardType)
		{
		case CelebrationRewardType.coins:
			result = ((!isJackpot(reward)) ? rewardCoins : rewardCoinsJackpot);
			break;
		case CelebrationRewardType.powerup:
			switch (reward.powerupType)
			{
			case PowerupType.hoverboard:
				result = rewardPowerupHoverboard;
				break;
			case PowerupType.headstart2000:
				result = rewardPowerupHeadstart2000;
				break;
			case PowerupType.scorebooster:
				result = rewardPowerupScoreBooster;
				break;
			}
			break;
		case CelebrationRewardType.token:
			switch (reward.characterType)
			{
			case Characters.CharacterType.fresh:
				result = rewardTokenFresh;
				break;
			case Characters.CharacterType.tricky:
				result = rewardTokenTricky;
				break;
			case Characters.CharacterType.spike:
				result = rewardTokenSpike;
				break;
			case Characters.CharacterType.yutani:
				result = rewardTokenYutani;
				MissionsManager.Instance.PlayerDidThis(MissionTarget.HaveYutani);
				break;
			}
			break;
		case CelebrationRewardType.trophy:
			switch (reward.trophyType)
			{
			case OldTrophies.Trophy.diamond:
				result = trophyDiamond;
				break;
			case OldTrophies.Trophy.goldbar:
				result = trophyGoldBar;
				break;
			case OldTrophies.Trophy.goldChainClock:
				result = trophyChainClock;
				break;
			case OldTrophies.Trophy.goldChainDollar:
				result = trophyChainDollar;
				break;
			case OldTrophies.Trophy.goldSkull:
				result = trophyGoldSkull;
				break;
			case OldTrophies.Trophy.headphones:
				result = trophyHeadphones;
				break;
			case OldTrophies.Trophy.lpBlack:
				result = trophyLPBlack;
				break;
			case OldTrophies.Trophy.tapeBlack:
				result = trophyTapeBlack;
				break;
			default:
				Debug.LogError(string.Concat(reward.trophyType, string.Empty));
				break;
			}
			break;
		case CelebrationRewardType.keys:
			result = rewardKey;
			break;
		case CelebrationRewardType.specialBoard:
			result = rewardSpecialBoard;
			break;
		}
		return result;
	}

	private IEnumerator AnimateStripes()
	{
		_stripeCoroutineRunning = true;
		Color32 colorOfStripe = GlobalColorsGenerated.WEEKLYHUNT_CELEBRATION_STRIPE_COLOR;
		int minSpeed = 600;
		int maxSpeed = 2000;
		float minAlpha = 0.3f;
		float maxAlpha = 0.9f;
		float minScale = 0.5f;
		float maxScale = 1f;
		backgroundTexture.mainTexture = itemHuntBackground;
		int numberOfStripes = 12;
		Vector2 minimumThreshold = new Vector2(-150f, -225f);
		Vector2 maximumThreshold = new Vector2(150f, 700f);
		if (_stripeTextures != null)
		{
			for (int i = 0; i < _stripeTextures.Length; i++)
			{
				_stripeTextures[i].gameObject.SetActive(false);
				UnityEngine.Object.Destroy(_stripeTextures[i].gameObject);
			}
		}
		_stripeTextures = new UITexture[numberOfStripes];
		float[] stripeSpeeds = new float[numberOfStripes];
		for (int j = 0; j < numberOfStripes; j++)
		{
			stripeSpeeds[j] = UnityEngine.Random.Range(minSpeed, maxSpeed);
			_stripeTextures[j] = NGUITools.AddWidget<UITexture>(stripePanel);
			_stripeTextures[j].mainTexture = itemHuntStripe;
			_stripeTextures[j].shader = Shader.Find("Unlit/Transparent Colored");
			_stripeTextures[j].MakePixelPerfect();
			_stripeTextures[j].color = colorOfStripe;
			_stripeTextures[j].alpha = Mathf.Lerp(minAlpha, maxAlpha, stripeSpeeds[j] / (float)maxSpeed);
			Vector3 oldScale = _stripeTextures[j].cachedTransform.localScale;
			_stripeTextures[j].cachedTransform.localScale = Vector3.Lerp(new Vector3(oldScale.x * minScale, oldScale.y * minScale, oldScale.z), new Vector3(oldScale.x * maxScale, oldScale.y * maxScale, oldScale.z), stripeSpeeds[j] / (float)maxSpeed);
			float xPlacement = UnityEngine.Random.Range(minimumThreshold.x, maximumThreshold.x);
			float yPlacement = UnityEngine.Random.Range(minimumThreshold.y, maximumThreshold.y);
			_stripeTextures[j].cachedTransform.localPosition = new Vector3(xPlacement, yPlacement, 0f);
		}
		yield return null;
		while (true)
		{
			for (int k = 0; k < _stripeTextures.Length; k++)
			{
				Vector3 oldPos = _stripeTextures[k].cachedTransform.localPosition;
				float movementUp = stripeSpeeds[k] * Time.deltaTime;
				if (oldPos.y > maximumThreshold.y)
				{
					oldPos.x = UnityEngine.Random.Range(minimumThreshold.x, maximumThreshold.x);
					stripeSpeeds[k] = UnityEngine.Random.Range(minSpeed, maxSpeed);
					movementUp = stripeSpeeds[k] * Time.deltaTime;
					_stripeTextures[k].alpha = Mathf.Lerp(minAlpha, maxAlpha, stripeSpeeds[k] / (float)maxSpeed);
					_stripeTextures[k].MakePixelPerfect();
					Vector3 oldScale2 = _stripeTextures[k].cachedTransform.localScale;
					_stripeTextures[k].cachedTransform.localScale = Vector3.Lerp(new Vector3(oldScale2.x * minScale, oldScale2.y * minScale, oldScale2.z), new Vector3(oldScale2.x * maxScale, oldScale2.y * maxScale, oldScale2.z), stripeSpeeds[k] / (float)maxSpeed);
					oldPos.y = minimumThreshold.y - movementUp;
				}
				_stripeTextures[k].cachedTransform.localPosition = new Vector3(oldPos.x, oldPos.y + movementUp, oldPos.z);
			}
			yield return null;
		}
	}

	private void ResetBackgroundToNormal()
	{
		_stripeCoroutineRunning = false;
		StopCoroutine("AnimateStripes");
		backgroundTexture.mainTexture = normalBackground;
		if (_stripeTextures != null)
		{
			for (int i = 0; i < _stripeTextures.Length; i++)
			{
				_stripeTextures[i].gameObject.SetActive(false);
				UnityEngine.Object.Destroy(_stripeTextures[i].gameObject);
			}
		}
		_stripeTextures = null;
	}

	private void HidePrizeButtons()
	{
		PrizeButtonHide(prizeButtonNormal.transform);
		PrizeButtonHide(prizeButtonSuper.transform);
		PrizeButtonHide(prizeButtonMini.transform);
	}

	private void TogglePrizeButtons(CelebrationRewardOrigin origin)
	{
		switch (origin)
		{
		case CelebrationRewardOrigin.MysteryBox:
			PrizeButtonShow(prizeButtonNormal.transform);
			PrizeButtonHide(prizeButtonSuper.transform);
			PrizeButtonHide(prizeButtonMini.transform);
			break;
		case CelebrationRewardOrigin.SuperMysteryBox:
			PrizeButtonHide(prizeButtonNormal.transform);
			PrizeButtonShow(prizeButtonSuper.transform);
			PrizeButtonHide(prizeButtonMini.transform);
			break;
		case CelebrationRewardOrigin.CharacterEventSuperMysteryBox:
			PrizeButtonHide(prizeButtonNormal.transform);
			PrizeButtonShow(prizeButtonSuper.transform);
			PrizeButtonHide(prizeButtonMini.transform);
			break;
		case CelebrationRewardOrigin.MysteryBoxMini:
			PrizeButtonHide(prizeButtonNormal.transform);
			PrizeButtonHide(prizeButtonSuper.transform);
			PrizeButtonShow(prizeButtonMini.transform);
			break;
		case CelebrationRewardOrigin.WeeklyChallengeHiddenPrize:
			break;
		default:
			PrizeButtonHide(prizeButtonNormal.transform);
			PrizeButtonHide(prizeButtonSuper.transform);
			PrizeButtonHide(prizeButtonMini.transform);
			break;
		}
	}

	private void PrizeButtonShow(Transform toShow)
	{
		toShow.GetComponent<Collider>().enabled = true;
		toShow.localPosition = _ShowPrizeButtonPosition;
	}

	private void PrizeButtonHide(Transform toHide)
	{
		toHide.GetComponent<Collider>().enabled = false;
		toHide.localPosition = _HidePrizeButtonPosition;
	}

	private void ShowPrizeList(MysteryBox.Type type)
	{
		if (preOpenLabel != null)
		{
			preOpenLabel.SetActive(false);
		}
		switch (type)
		{
		case MysteryBox.Type.Normal:
			Flurry.LogEventWithAParameter("Mysterybox view prices", "Screen Name", "ShowMysteryBox");
			UIScreenController.Instance.PushPopup("PrizeMBPopup_Keys");
			break;
		case MysteryBox.Type.Super:
			Flurry.LogEventWithAParameter("Mysterybox view prices", "Screen Name", "ShowSuperMysteryBox");
			Flurry.LogEvent("Super Mystery Box prizes");
			UIScreenController.Instance.PushPopup("PrizeSMBPopup_Keys");
			break;
		case MysteryBox.Type.Mini:
			Flurry.LogEventWithAParameter("Mysterybox view prices", "Screen Name", "ShowMiniMysteryBox");
			Flurry.LogEvent("Mini Mystery Box prizes");
			UIScreenController.Instance.PushPopup("PrizeMMBPopup");
			break;
		default:
			Debug.LogError("Unhandeled mysterybox type " + type, this);
			return;
		}
		for (int i = 0; i < _numberOfRewards; i++)
		{
			if (_celebrationRewardContainer[i] != null)
			{
				NGUITools.SetActive(_celebrationRewardContainer[i], false);
			}
		}
		if (_superMysteryBoxEffect != null)
		{
			_superMysteryBoxEffect.StopEffect();
			_superMysteryBoxEffect.SetVisible(false);
		}
		openButtonCollider.enabled = false;
		skipButtonCollider.enabled = false;
		tapToStartLabel.enabled = false;
	}

	private static AnimationState FindClipState(Animation anim, string clipPrefix)
	{
		if (anim == null)
		{
			return null;
		}
		foreach (AnimationState state in anim)
		{
			if (state.clip != null && state.clip.name.StartsWith(clipPrefix, StringComparison.OrdinalIgnoreCase))
			{
				return state;
			}
		}
		return null;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (openButtonCollider.enabled)
			{
				FingerPressed();
				FingerReleased();
			}
			if (skipButtonCollider.enabled)
			{
				SkipNow();
			}
			if (isWaitingForInput)
			{
				skipWaitBackButtonPressed = true;
			}
		}
	}

	private bool ShouldWaitForInput()
	{
		if (skipWaitBackButtonPressed)
		{
			skipWaitBackButtonPressed = false;
			return false;
		}
		if (isWaitingForInput)
		{
			return true;
		}
		return false;
	}

	public void StopWaitingForInput()
	{
		if (isWaitingForInput)
		{
			isWaitingForInput = false;
			_mysteryBoxDoubleUpButton.DisableButton();
		}
	}

	public void SkipNow()
	{
		if (_maySetTimeScale && !_isRewardingJackpot)
		{
			SingletonMonobehavior<TimeScaleManager>.Instance.TimeScale = 4f;
		}
	}

	private void OnPopClosed(string popupName)
	{
		switch (popupName)
		{
		case "PrizeMBPopup_Keys":
		case "PrizeMMBPopup":
		case "PrizeSMBPopup_Keys":
			ClosePrizeList();
			break;
		}
	}

	public void ClosePrizeList()
	{
		for (int i = 0; i < _numberOfRewards; i++)
		{
			if (_celebrationRewardContainer[i] != null)
			{
				NGUITools.SetActive(_celebrationRewardContainer[i], true);
			}
		}
		if (_superMysteryBoxEffect != null)
		{
			_superMysteryBoxEffect.SetVisible(true);
			ToggleSuperMysteryBoxEffect(rewardsToUnlock[_currentReward].CelebrationRewardOrigin);
		}
		openButtonCollider.enabled = true;
		skipButtonCollider.enabled = true;
		tapToStartLabel.enabled = true;
		if (preOpenLabel != null)
		{
			preOpenLabel.SetActive(true);
		}
	}

	private void PrizeListNormalClicked()
	{
		ShowPrizeList(MysteryBox.Type.Normal);
	}

	private void PrizeListSuperClicked()
	{
		ShowPrizeList(MysteryBox.Type.Super);
	}

	private void PrizeListMiniClicked()
	{
		ShowPrizeList(MysteryBox.Type.Mini);
	}

	private GameObject ShowModel(Characters.CharacterType charType, int themeIndex, UIModelController.ModelScreen modelScreen)
	{
		return UIModelController.Instance.ShowCharacterInCelebration(charType, themeIndex, modelScreen);
	}

	private void HideModel()
	{
		UIModelController.Instance.DeactivateCelebrationPopupModels();
	}

	private void ShowBackgroundWithStripes(Hoverboards.BoardType boardType, UIModelController.ModelScreen modelScreen)
	{
		UIModelController.Instance.ActivateCelebrationBoardWithStripes(boardType, modelScreen);
	}

	private void HideSpecialBoard()
	{
		UIModelController.Instance.DeactivateCelebrationPopupModels();
	}

	private void HideCelebrationTunnel()
	{
		UIModelController.Instance.DeactivateCelebrationTunnel();
	}

	private bool isJackpot(CelebrationReward reward)
	{
		if (reward.rewardType == CelebrationRewardType.coins)
		{
			switch (reward.CelebrationRewardOrigin)
			{
			case CelebrationRewardOrigin.MysteryBox:
			case CelebrationRewardOrigin.WeeklyChallengeMysteryBox:
				if (reward.amount >= MysteryBox.mysteryBoxJackpotAmount)
				{
					_isRewardingJackpot = true;
					return true;
				}
				break;
			case CelebrationRewardOrigin.SuperMysteryBox:
			case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
			case CelebrationRewardOrigin.CharacterEventSuperMysteryBox:
				if (reward.amount >= MysteryBox.superMysteryBoxJackpotAmount)
				{
					_isRewardingJackpot = true;
					return true;
				}
				break;
			}
		}
		return false;
	}

	private void DoubleCoins()
	{
		if (_rewardToDouble != null && _rewardTemplate != null)
		{
			StartCoroutine(DoubleCoins_Routine(_rewardToDouble, _rewardTemplate));
		}
	}

	private IEnumerator Initial_Generic_DoubleUp_Routine(CelebrationReward reward, CelebrationPopupLabelTemplate template)
	{
		flashEffectSprite.color = EFFECT_INITIAL_COLOR;
		yield return StartCoroutine(AnimateEffectColor(flashEffectSprite, flashEffectSprite.color, Color.white, 4f));
		continueButtonCollider.enabled = false;
		tapToStartLabel.alpha = 0f;
		tapToStartLabel.text = Strings.Get(StringID.CELEBRATION_POPUP_CONTINUE);
		template.SetupDoubleUp(reward);
		backgroundTexture.mainTexture = awardsBackground;
		PayoutReward(reward, false);
		StartCoroutine(AnimateColor(GlowEffect.GetComponent<MeshRenderer>().material, 1.5f, Color.white));
		StartCoroutine(RotateGameObject(GlowEffect.transform, 3000f, new Vector3(0f, 0f, -270000f)));
		if (_superMysteryBoxEffect != null)
		{
			_superMysteryBoxEffect.StartEffect();
		}
		StartCoroutine(AnimateEffectColor(flashEffectSprite, flashEffectSprite.color, EFFECT_INITIAL_COLOR, 0.4f));
	}

	private IEnumerator DoubleCoins_Routine(CelebrationReward reward, CelebrationPopupLabelTemplate template)
	{
		yield return StartCoroutine(Initial_Generic_DoubleUp_Routine(reward, template));
		int doubleRewardAmount = reward.amount * 2;
		int from = reward.amount;
		int to = doubleRewardAmount;
		float factor = 0.5f;
		float countTime = CalcCoinCountTime(to, from, factor);
		bool megaJackpot = reward.amount > MysteryBox.superMysteryBoxJackpotAmount;
		int coinboxCountFromAmount = PlayerInfo.Instance.amountOfCoins;
		StartCoroutine(PlaySoundDelayed(_doubleRewardClip, countTime, 0.7f));
		yield return StartCoroutine(CountUpCoins(coinboxCountFromAmount, to, template, megaJackpot, from, factor, true));
		yield return StartCoroutine(TweenScaleUpDown(template.doubleUpItemLabel.gameObject));
		StartCoroutine(AnimateAlpha(tapToStartLabel, 0.5f, 1f));
		continueButtonCollider.enabled = true;
	}

	private void DoubleKeys()
	{
		if (_rewardToDouble != null && _rewardTemplate != null)
		{
			StartCoroutine(DoubleKeys_Routine(_rewardToDouble, _rewardTemplate));
		}
	}

	private IEnumerator DoubleKeys_Routine(CelebrationReward reward, CelebrationPopupLabelTemplate template)
	{
		yield return StartCoroutine(Initial_Generic_DoubleUp_Routine(reward, template));
		NGUITools.PlaySound(_doubleRewardClip, 0.7f);
		yield return StartCoroutine(TweenScaleUpDown(template.doubleUpItemLabel.gameObject));
		StartCoroutine(AnimateAlpha(tapToStartLabel, 0.5f, 1f));
		continueButtonCollider.enabled = true;
	}

	private void DoubleTokens()
	{
		if (_rewardToDouble != null && _rewardTemplate != null)
		{
			StartCoroutine(DoubleTokens_Routine(_rewardToDouble, _rewardTemplate));
		}
	}

	private IEnumerator DoubleTokens_Routine(CelebrationReward reward, CelebrationPopupLabelTemplate template)
	{
		yield return StartCoroutine(Initial_Generic_DoubleUp_Routine(reward, template));
		MissionsManager.Instance.PlayerDidThis(MissionTarget.Tokens, reward.amount);
		yield return StartCoroutine(TweenScaleUpDown(template.doubleUpItemLabel.gameObject));
		NGUITools.PlaySound(_doubleRewardClip, 0.7f);
		StartCoroutine(AnimateAlpha(tapToStartLabel, 0.5f, 1f));
		continueButtonCollider.enabled = true;
	}

	private void DoublePowerups()
	{
		if (_rewardToDouble != null && _rewardTemplate != null)
		{
			StartCoroutine(DoublePowerups_Routine(_rewardToDouble, _rewardTemplate));
		}
	}

	private IEnumerator DoublePowerups_Routine(CelebrationReward reward, CelebrationPopupLabelTemplate template)
	{
		yield return StartCoroutine(Initial_Generic_DoubleUp_Routine(reward, template));
		NGUITools.PlaySound(_doubleRewardClip, 0.7f);
		yield return StartCoroutine(TweenScaleUpDown(template.doubleUpItemLabel.gameObject));
		StartCoroutine(AnimateAlpha(tapToStartLabel, 0.5f, 1f));
		continueButtonCollider.enabled = true;
	}

	private IEnumerator TweenScaleUpDown(GameObject go)
	{
		TweenScale tweenScale = go.GetComponent<TweenScale>();
		if (tweenScale == null)
		{
			tweenScale = go.AddComponent<TweenScale>();
			tweenScale.AddOnFinished(delegate
			{
				tweenScale.duration = _scaleUpDuration * _scaleDownDurationFactor;
				tweenScale.method = _scaleDownMethod;
				tweenScale.PlayReverse();
			});
		}
		tweenScale.from = go.transform.localScale;
		tweenScale.to = Vector3.Scale(base.transform.localScale, _scaleTargetFactor);
		tweenScale.duration = _scaleUpDuration;
		tweenScale.method = _scaleUpMethod;
		tweenScale.PlayForward();
		float totalDuration = _scaleUpDuration + _scaleUpDuration * _scaleDownDurationFactor;
		yield return new WaitForSeconds(totalDuration);
	}

	private IEnumerator PlaySoundDelayed(AudioClip clip, float delay = 0f, float volume = 1f)
	{
		yield return new WaitForSeconds(delay);
		NGUITools.PlaySound(clip, volume);
	}
}
