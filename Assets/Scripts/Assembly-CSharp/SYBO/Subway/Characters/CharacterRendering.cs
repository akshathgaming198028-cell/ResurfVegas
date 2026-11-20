using System;
using System.Collections;
using System.Collections.Generic;
using SYBO.Core.Utilities;
using SYBO.Subway.Cameras;
using SYBO.Subway.Characters.States;
using UnityEngine;

namespace SYBO.Subway.Characters
{
	public class CharacterRendering : MonoBehaviour
	{
		public class Animations
		{
			public string[] TOP_MENU = new string[1] { "topMenu" };

			public string[] INTRO_RUN = new string[1] { "introRun" };

			public string[] RUN = new string[1] { "run" };

			public string[] LAND = new string[1] { "land" };

			public string[] JUMP = new string[1] { "jump" };

			public string[] HANGTIME = new string[1] { "hangtime" };

			public string[] DOUBLEJUMP = new string[1] { "jump" };

			public string[] DOUBLEHANGTIME = new string[1] { "hangtime" };

			public string[] ROLL = new string[1] { "roll" };

			public string[] DODGE_LEFT = new string[1] { "dodgeLeft" };

			public string[] DODGE_RIGHT = new string[1] { "dodgeRight" };

			public string[] TURBO_HEADSTART1 = new string[1] { "dodgeRight" };

			public string[] TURBO_HEADSTART2 = new string[1] { "dodgeRight" };

			public string[] GRIND = new string[1] { "grind" };

			public string[] GET_ON_BOARD = new string[1] { "getOnBoard" };

			public string[] HIT_MID = new string[1] { "hitMid" };

			public string[] HIT_UPPER = new string[1] { "hitUpper" };

			public string[] HIT_LOWER = new string[1] { "hitLower" };

			public string[] HIT_MOVING = new string[1] { "hitMoving" };

			public string[] STUMBLE = new string[1] { "stumble_low" };

			public string[] STUMBLEDEATH = new string[1] { "stumbleFall" };

			public string[] STUMBLE_MIX = new string[1] { "stumble" };

			public string[] STUMBLE_LEFT_SIDE = new string[1] { "stumbleLeftSide" };

			public string[] STUMBLE_RIGHT_SIDE = new string[1] { "stumbleRightSide" };

			public string[] STUMBLE_LEFT_CORNER = new string[1] { "stumbleLeftCorner" };

			public string[] STUMBLE_RIGHT_CORNER = new string[1] { "stumbleRightCorner" };

			public string[] HOLDMAGNET = new string[1] { "hold_magnet" };

			public string DEFAULT_HOVERBOARD_ANIMATION;

			public string TopMenu
			{
				get
				{
					return GetRandomAnimationName(TOP_MENU);
				}
			}

			public string IntroRun
			{
				get
				{
					return GetRandomAnimationName(INTRO_RUN);
				}
			}

			public string Run
			{
				get
				{
					return GetRandomAnimationName(RUN);
				}
			}

			public string Land
			{
				get
				{
					return GetRandomAnimationName(LAND);
				}
			}

			public string Jump
			{
				get
				{
					return GetRandomAnimationName(JUMP);
				}
			}

			public string Hangtime
			{
				get
				{
					return GetRandomAnimationName(HANGTIME);
				}
			}

			public string DoubleJump
			{
				get
				{
					return GetRandomAnimationName(DOUBLEJUMP);
				}
			}

			public string DoubleHangtime
			{
				get
				{
					return GetRandomAnimationName(DOUBLEHANGTIME);
				}
			}

			public string TurboHeadstart1
			{
				get
				{
					return GetRandomAnimationName(TURBO_HEADSTART1);
				}
			}

			public string TurboHeadstart2
			{
				get
				{
					return GetRandomAnimationName(TURBO_HEADSTART2);
				}
			}

			public string[] HoverboardJump
			{
				get
				{
					return GetRandomHoverJumps(JUMP, HANGTIME);
				}
			}

			public string Roll
			{
				get
				{
					return GetRandomAnimationName(ROLL);
				}
			}

			public string DodgeLeft
			{
				get
				{
					return GetRandomAnimationName(DODGE_LEFT);
				}
			}

			public string DodgeRight
			{
				get
				{
					return GetRandomAnimationName(DODGE_RIGHT);
				}
			}

			public string Grind
			{
				get
				{
					return GetRandomAnimationName(GRIND);
				}
			}

			public string GetOnBoard
			{
				get
				{
					return GetRandomAnimationName(GET_ON_BOARD);
				}
			}

			public string HitMid
			{
				get
				{
					return GetRandomAnimationName(HIT_MID);
				}
			}

			public string HitUpper
			{
				get
				{
					return GetRandomAnimationName(HIT_UPPER);
				}
			}

			public string HitLower
			{
				get
				{
					return GetRandomAnimationName(HIT_LOWER);
				}
			}

			public string HitMoving
			{
				get
				{
					return GetRandomAnimationName(HIT_MOVING);
				}
			}

			public string Stumble
			{
				get
				{
					return GetRandomAnimationName(STUMBLE);
				}
			}

			public string StumbleDeath
			{
				get
				{
					return GetRandomAnimationName(STUMBLEDEATH);
				}
			}

			public string StumbleMix
			{
				get
				{
					return GetRandomAnimationName(STUMBLE_MIX);
				}
			}

			public string StumbleLeftSide
			{
				get
				{
					return GetRandomAnimationName(STUMBLE_LEFT_SIDE);
				}
			}

			public string StumbleRightSide
			{
				get
				{
					return GetRandomAnimationName(STUMBLE_RIGHT_SIDE);
				}
			}

			public string StumbleLeftCorner
			{
				get
				{
					return GetRandomAnimationName(STUMBLE_LEFT_CORNER);
				}
			}

			public string StumbleRightCorner
			{
				get
				{
					return GetRandomAnimationName(STUMBLE_RIGHT_CORNER);
				}
			}

			private string GetRandomAnimationName(string[] animationsNames)
			{
				int num = UnityEngine.Random.Range(0, animationsNames.Length);
				return animationsNames[num];
			}

			private string[] GetRandomHoverJumps(string[] hoverboardJump, string[] hoverboardHangtime)
			{
				if (hoverboardJump.Length != hoverboardHangtime.Length)
				{
					Debug.LogWarning("hoverboardJump.Length (" + hoverboardJump.Length + ") != (" + hoverboardHangtime.Length + ") hoverboardHangtime.Length");
				}
				int num = UnityEngine.Random.Range(0, Mathf.Min(hoverboardJump.Length, hoverboardHangtime.Length));
				return new string[2]
				{
					hoverboardJump[num],
					hoverboardHangtime[num]
				};
			}
		}

		[Serializable]
		public class AnimationClipLists
		{
			public AnimationClip[] topMenu;

			public AnimationClip[] introRun;

			public AnimationClip[] run;

			public AnimationClip[] jump;

			public AnimationClip[] hangtime;

			public AnimationClip[] doubleJump;

			public AnimationClip[] doubleHangtime;

			public AnimationClip[] landing;

			public AnimationClip[] dodgeLeft;

			public AnimationClip[] dodgeRight;

			public AnimationClip[] roll;

			public AnimationClip[] hitMid;

			public AnimationClip[] hitUpper;

			public AnimationClip[] hitLower;

			public AnimationClip[] hitMoving;

			public AnimationClip[] stumble;

			public AnimationClip[] stumbleMix;

			public AnimationClip[] stumbleDeath;

			public AnimationClip[] stumbleLeftSide;

			public AnimationClip[] stumbleRightSide;

			public AnimationClip[] stumbleLeftCorner;

			public AnimationClip[] stumbleRightCorner;

			public AnimationClip[] holdMagnet;
		}

		[Serializable]
		public class JetpackClips
		{
			public AnimationClip[] run;

			public AnimationClip[] dodgeLeft;

			public AnimationClip[] dodgeRight;

			public AnimationClip[] turboHeadstart1;

			public AnimationClip[] turboHeadstart2;
		}

		[Serializable]
		public class PogostickClips
		{
			public AnimationClip[] run;

			public AnimationClip[] hangtime;

			public AnimationClip[] dodgeLeft;

			public AnimationClip[] dodgeRight;
		}

		[Serializable]
		public class SuperSneaksClips
		{
			public AnimationClip[] run;
		}

		[SerializeField]
		private AnimationClipLists defaultAnimations;

		[SerializeField]
		private JetpackClips jetpackAnimations;

		[SerializeField]
		private SuperSneaksClips SuperSneaksAnimations;

		[SerializeField]
		private PogostickClips pogostickClips;

		[SerializeField]
		private GameObject teleboardWarpPrefab;

		[SerializeField]
		private Material teleboardFXMaterial;

		[OptionalField]
		public Animation characterAnimation;

		[SerializeField]
		private AnimationCurve jetpackParticleOffsetCurve;

		[SerializeField]
		private GameObject characterModelPrefab;

		[SerializeField]
		private GameObject characterRenderingEffectsPrefab;

		public Animations animations;

		private CharacterModel _characterModel;

		private static CharacterRendering _instance;

		private Game _game;

		private Character _character;

		private Hoverboard _hoverboard;

		private SuperSneakers _superSneakers;

		private JetpackState _jetpackState;

		private PogoStickState _pogoStickState;

		private FollowingGuard _followingGuard;

		private CharacterController _characterController;

		private MeshRenderer _shadowMeshRenderer;

		private CharacterRenderingEffects _characterRenderingEffects;

		private Revive _revive;

		private bool _initialized;

		private HoverboardRendering _defaultHoverboardRendering;

		private HoverboardRendering.AbilityAnimationList _defaultHoverboardAnimationList;

		private GameObject _teleboardWarpGameObject;

		private Renderer _telewarpRenderer;

		private AnimationState _teleboardAnimation;

		private Vector3 _teleboardWarpPosition;

		private CharacterCamera _characterCamera;

		private Animation _hoverboardAnimation;

		private HoverboardRendering _hoverboardRendering;

		private List<AnimationClip> _addedAnimClipsNames = new List<AnimationClip>();

		private GameObject _currentHoverboardGameObject;

		private Hoverboard.BoardAbility[] _currentBoardAbilities;

		private Vector3 _initRot = Vector3.zero;

		private Vector3 _initScale = Vector3.one;

		private ParticleSystem[] _killParticleSystem;

		private AnimationState _caughtAnimationState;

		private Material _currentModelMaterial;

		private Material[] _currentHeadMaterials;

		private Material _currentCustomizationMaterial;

		private Material _currentHoverboardMaterial;

		private Material _currentSupersneakersMaterial;

		private Material _currentDoubleJumperMaterial;

		private Material[] _currentCustomHoverboardModelMaterials;

		private bool _isTeleportFxActive;

		private bool _isCurrentCustomizationState = true;

		private bool _isDoublejump;

		private bool _isRolling;

		private bool _isDeath;

		private static float _DEFAULT_SPEED_RUN_ANIMATION = 1f;

		private string jumpAnimation;

		private string hangtimeAnimation;

		public CharacterModel CharacterModel
		{
			get
			{
				return _characterModel;
			}
		}

		public Animation CharacterAnimation
		{
			get
			{
				return characterAnimation;
			}
		}

		public static CharacterRendering Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Utils.FindObject<CharacterRendering>();
				}
				return _instance;
			}
		}

		public void Initialize()
		{
			_instance = this;
			_game = Game.Instance;
			_character = Character.Instance;
			_characterController = Game.Charactercontroller;
			_hoverboard = Hoverboard.Instance;
			_superSneakers = this.FindObject<SuperSneakers>();
			_jetpackState = JetpackState.Instance;
			_pogoStickState = PogoStickState.Instance;
			_followingGuard = FollowingGuard.Instance;
			_revive = Revive.Instance;
			_characterCamera = CharacterCamera.Instance;
			InitializeCharacterModel();
			InitializeAnimations();
			InitializeCharacterRenderingEffects();
			Variable<bool> isInGame = _game.IsInGame;
			isInGame.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isInGame.OnChange, new Variable<bool>.OnChangeDelegate(IsInGame_OnChange));
			_character.OnChangeTrack += OnChangeTrack;
			_character.OnStumble += OnStumble;
			_character.OnRevive += OnRevive;
			_character.OnResetTeleportFX += ResetCharacterTeleportFX;
			_character.OnTutorialMoveBackToCheckPoint += OnTutorialMoveBackToCheckPoint;
			_character.OnTutorialStartFromCheckPoint += OnTutorialStartFromCheckPoint;
			_character.OnHitByTrain += OnHitByTrain;
			_character.OnJump += OnJump;
			_character.OnDoubleJump += OnDoubleJump;
			_character.OnJumpIfHitByTrain += OnJump;
			_character.OnRoll += OnRoll;
			_character.OnLanding += OnLanding;
			_character.OnHangtime += OnHangtime;
			Variable<bool> isGrounded = _character.IsGrounded;
			isGrounded.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isGrounded.OnChange, new Variable<bool>.OnChangeDelegate(OnChangeIsGrounded));
			_character.OnTeleboardStart += HandleOnTeleboardStart;
			Game game = _game;
			game.OnStageMenuSequence = (Game.OnStageMenuSequenceDelegate)Delegate.Combine(game.OnStageMenuSequence, new Game.OnStageMenuSequenceDelegate(OnStageMenuSequence));
			Game game2 = _game;
			game2.OnIntroRun = (Game.OnIntroRunDelegate)Delegate.Combine(game2.OnIntroRun, new Game.OnIntroRunDelegate(OnIntroRun));
			_game.OnTurboHeadstartInput += HandleOnTurboHeadstart;
			_game.OnDeath += HandleOnDeath;
			_hoverboard.OnSwitchToHoverboard += OnSwitchToHoverboard;
			_hoverboard.OnEndHoverboard += OnSwitchToRunning;
			_hoverboard.OnJump += OnJump;
			_hoverboard.OnRun += OnRun;
			JetpackState jetpackState = _jetpackState;
			jetpackState.OnStart = (JetpackState.OnStartDelegate)Delegate.Combine(jetpackState.OnStart, new JetpackState.OnStartDelegate(OnSwitchToJetpack));
			JetpackState jetpackState2 = _jetpackState;
			jetpackState2.OnStop = (JetpackState.OnStopDelegate)Delegate.Combine(jetpackState2.OnStop, new JetpackState.OnStopDelegate(JetpackOnStop));
			JetpackState jetpackState3 = _jetpackState;
			jetpackState3.OnFlyAheadStart = (JetpackState.OnFlyAheadStartDelegate)Delegate.Combine(jetpackState3.OnFlyAheadStart, new JetpackState.OnFlyAheadStartDelegate(JetpackOnFlyAheadStart));
			JetpackState jetpackState4 = _jetpackState;
			jetpackState4.OnFlyAheadUpdate = (JetpackState.OnFlyAheadUpdateDelegate)Delegate.Combine(jetpackState4.OnFlyAheadUpdate, new JetpackState.OnFlyAheadUpdateDelegate(JetpackOnFlyAheadUpdate));
			SlideinPowerupHelper.OnScoreBoostActivated += HandleOnScorebooster;
			_superSneakers.OnSwitchToSuperSneakers += OnSwitchToSuperSneakers;
			_superSneakers.SuperSneakerOnStop += SuperSneakersOnStop;
			PogoStickState pogoStickState = _pogoStickState;
			pogoStickState.OnStart = (PogoStickState.OnStartDelegate)Delegate.Combine(pogoStickState.OnStart, new PogoStickState.OnStartDelegate(PogostickOnStart));
			PogoStickState pogoStickState2 = _pogoStickState;
			pogoStickState2.OnHangtime = (PogoStickState.OnStartDelegate)Delegate.Combine(pogoStickState2.OnHangtime, new PogoStickState.OnStartDelegate(PogostickOnHangtime));
			PogoStickState pogoStickState3 = _pogoStickState;
			pogoStickState3.OnStop = (PogoStickState.OnStopDelegate)Delegate.Combine(pogoStickState3.OnStop, new PogoStickState.OnStopDelegate(PogostickOnStop));
			FollowingGuard followingGuard = _followingGuard;
			followingGuard.OnCatchPlayer = (FollowingGuard.OnCatchPlayerDelegate)Delegate.Combine(followingGuard.OnCatchPlayer, new FollowingGuard.OnCatchPlayerDelegate(OnCatchPlayer));
			_revive.OnRevive += OnRevive;
			_revive.OnSwitchToRunning += OnSwitchToRunning;
			_defaultHoverboardRendering = Hoverboard.Instance.hoverboardSelector[0].hoverboardPrefab.GetComponent<HoverboardRendering>();
			_defaultHoverboardAnimationList = new HoverboardRendering.AbilityAnimationList();
			_defaultHoverboardAnimationList.defaultHoverboardAnimation = _defaultHoverboardRendering.defaultHoverboardAnimation;
			_defaultHoverboardAnimationList.runAnimations = _defaultHoverboardRendering.runAnimations;
			_defaultHoverboardAnimationList.landAnimations = _defaultHoverboardRendering.landAnimations;
			_defaultHoverboardAnimationList.jumpAnimations = _defaultHoverboardRendering.jumpAnimations;
			_defaultHoverboardAnimationList.hangtimeAnimations = _defaultHoverboardRendering.hangtimeAnimations;
			_defaultHoverboardAnimationList.rollAnimations = _defaultHoverboardRendering.rollAnimations;
			_defaultHoverboardAnimationList.dodgeLeftAnimations = _defaultHoverboardRendering.dodgeLeftAnimations;
			_defaultHoverboardAnimationList.dodgeRightAnimations = _defaultHoverboardRendering.dodgeRightAnimations;
			_defaultHoverboardAnimationList.grindAnimations = _defaultHoverboardRendering.grindAnimations;
			_defaultHoverboardAnimationList.grindLandAnimations = _defaultHoverboardRendering.grindLandAnimations;
			_defaultHoverboardAnimationList.getOnBoardAnimations = _defaultHoverboardRendering.getOnBoardAnimations;
		}

		protected void Awake()
		{
			_instance = this;
		}

		public CharacterModel RecreateCharacterModel()
		{
			InitializeCharacterModel();
			InitializeAnimations();
			_characterModel.RecreateCharacter();
			_characterRenderingEffects.RecreateModel(_characterModel);
			OnStageMenuSequence();
			return _characterModel;
		}

		private void InitializeCharacterModel()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(characterModelPrefab);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			_killParticleSystem = gameObject.GetComponentsInChildren<ParticleSystem>();
			_characterModel = gameObject.GetComponent<CharacterModel>();
			_shadowMeshRenderer = _characterModel.MeshBlobShadow;
			characterAnimation = _characterModel.CharacterAnimation;
		}

		private void InitializeCharacterRenderingEffects()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(characterRenderingEffectsPrefab);
			HierarchyUtility.ReparentByHierarchyPathType(gameObject.transform, HierarchyPathType.Dynamic);
			_characterRenderingEffects = gameObject.GetComponent<CharacterRenderingEffects>();
			_characterRenderingEffects.Initialize(_characterModel);
			_teleboardWarpGameObject = UnityEngine.Object.Instantiate(teleboardWarpPrefab);
			_teleboardWarpGameObject.transform.parent = _character.transform;
			_teleboardAnimation = _teleboardWarpGameObject.GetComponent<Animation>()["teleboardWarp"];
			_teleboardAnimation.normalizedTime = 0f;
			_teleboardAnimation.weight = 1f;
			_teleboardAnimation.enabled = true;
			SetSpeed(_teleboardAnimation, 0f);
			_teleboardWarpGameObject.SetActive(false);
		}

		private void InitializeAnimations()
		{
			animations = new Animations();
			_addedAnimClipsNames.Clear();
			animations.RUN = InitializeClips(SuperSneaksAnimations.run);
			animations.RUN = InitializeClips(defaultAnimations.run);
			animations.INTRO_RUN = InitializeClips(defaultAnimations.introRun);
			animations.LAND = InitializeClips(defaultAnimations.landing);
			animations.JUMP = InitializeClips(defaultAnimations.jump);
			animations.HANGTIME = InitializeClips(defaultAnimations.hangtime);
			animations.DOUBLEJUMP = InitializeClips(defaultAnimations.doubleJump);
			animations.DOUBLEHANGTIME = InitializeClips(defaultAnimations.doubleHangtime);
			animations.ROLL = InitializeClips(defaultAnimations.roll);
			animations.DODGE_LEFT = InitializeClips(defaultAnimations.dodgeLeft);
			animations.DODGE_RIGHT = InitializeClips(defaultAnimations.dodgeRight);
			animations.TOP_MENU = InitializeClips(defaultAnimations.topMenu);
			animations.HIT_MID = InitializeClips(defaultAnimations.hitMid);
			animations.HIT_UPPER = InitializeClips(defaultAnimations.hitUpper);
			animations.HIT_LOWER = InitializeClips(defaultAnimations.hitLower);
			animations.HIT_MOVING = InitializeClips(defaultAnimations.hitMoving);
			animations.STUMBLE = InitializeClips(defaultAnimations.stumble);
			animations.STUMBLEDEATH = InitializeClips(defaultAnimations.stumbleDeath);
			animations.STUMBLE_MIX = InitializeClips(defaultAnimations.stumbleMix);
			animations.STUMBLE_LEFT_SIDE = InitializeClips(defaultAnimations.stumbleLeftSide);
			animations.STUMBLE_RIGHT_SIDE = InitializeClips(defaultAnimations.stumbleRightSide);
			animations.STUMBLE_LEFT_CORNER = InitializeClips(defaultAnimations.stumbleLeftCorner);
			animations.STUMBLE_RIGHT_CORNER = InitializeClips(defaultAnimations.stumbleRightCorner);
			animations.HOLDMAGNET = InitializeClips(defaultAnimations.holdMagnet);
			characterAnimation["stumble"].AddMixingTransform(_characterModel.SpineTransform);
			characterAnimation["stumble"].layer = 2;
			characterAnimation["stumble"].weight = 1f;
			characterAnimation["stumbleCornerLeft"].AddMixingTransform(_characterModel.SpineTransform);
			characterAnimation["stumbleCornerLeft"].layer = 2;
			characterAnimation["stumbleCornerLeft"].weight = 1f;
			characterAnimation["stumbleCornerRight"].AddMixingTransform(_characterModel.SpineTransform);
			characterAnimation["stumbleCornerRight"].layer = 2;
			characterAnimation["stumbleCornerRight"].weight = 1f;
		}

		private string[] InitializeClips(AnimationClip[] clips)
		{
			string[] array = new string[clips.Length];
			for (int i = 0; i < clips.Length; i++)
			{
				AnimationClip animationClip = clips[i];
				if (!_addedAnimClipsNames.Contains(animationClip))
				{
					_addedAnimClipsNames.Add(animationClip);
					characterAnimation.AddClip(animationClip, animationClip.name);
				}
				array[i] = animationClip.name;
			}
			return array;
		}

		private string GetHoverboardAnimationName(string animationName)
		{
			AnimationClip clip = _hoverboardAnimation.GetClip(animationName);
			if (clip != null)
			{
				return animationName;
			}
			return animations.DEFAULT_HOVERBOARD_ANIMATION;
		}

		private void SetSpeed(AnimationState animationState, float speed)
		{
			animationState.speed = speed;
		}

		private void OnIntroRun()
		{
			ParticleSystem[] killParticleSystem = _killParticleSystem;
			foreach (ParticleSystem particleSystem in killParticleSystem)
			{
				particleSystem.Stop();
			}
			OnSwitchToRunning();
			string run = animations.Run;
			characterAnimation.CrossFadeQueued(run, 0.2f);
			_characterModel.SprayCanModel.SetActive(false);
			_characterModel.StopEyeAnimations();
			_isDeath = false;
		}

		private void OnRun()
		{
			if (_character.IsFalling || _character.IsJumping || _character.IsRolling || _isDeath)
			{
				return;
			}
			string run = animations.Run;
			SetSpeed(characterAnimation[run], _game.NormalizedGameSpeed);
			if (_characterController.isGrounded)
			{
				if (_superSneakers.isActive)
				{
					SetSpeed(characterAnimation[run], _DEFAULT_SPEED_RUN_ANIMATION);
				}
				characterAnimation.CrossFade(run);
				if (_hoverboardAnimation != null)
				{
					_hoverboardAnimation.CrossFade(GetHoverboardAnimationName(run));
				}
			}
		}

		private void OnChangeTrack(Character.OnChangeTrackDirection direction)
		{
			if (_characterController.isGrounded && !PogoStickState.Instance.isActive && !_isDeath)
			{
				string text = ((direction != Character.OnChangeTrackDirection.Left) ? animations.DodgeRight : animations.DodgeLeft);
				if (!_hoverboard.isActive && !_jetpackState.isActive)
				{
					SetSpeed(characterAnimation[text], _game.NormalizedGameSpeed);
				}
				else
				{
					SetSpeed(characterAnimation[text], _DEFAULT_SPEED_RUN_ANIMATION);
				}
				characterAnimation.CrossFade(text, 0.2f);
				if (_hoverboard.isActive && _hoverboardAnimation != null)
				{
					_hoverboardAnimation.CrossFade(GetHoverboardAnimationName(text), 0.2f);
				}
			}
			if (!_character.IsJumping)
			{
				string run = animations.Run;
				if (!_hoverboard.isActive && !_jetpackState.isActive)
				{
					SetSpeed(characterAnimation[run], _game.NormalizedGameSpeed);
				}
				else
				{
					SetSpeed(characterAnimation[run], _DEFAULT_SPEED_RUN_ANIMATION);
				}
				characterAnimation.CrossFadeQueued(run, (!_game.Modifiers.IsActive(_game.Modifiers.Hoverboard)) ? 0.02f : 0.4f);
				if (_hoverboard.isActive && _hoverboardAnimation != null)
				{
					_hoverboardAnimation.CrossFadeQueued(GetHoverboardAnimationName(run), 0.02f);
				}
			}
		}

		private void OnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName)
		{
			switch (stumbleType)
			{
			case Character.StumbleType.Bush:
				characterAnimation.CrossFade(animations.StumbleMix, 0.05f);
				characterAnimation.CrossFadeQueued(animations.Run, 0.5f);
				return;
			case Character.StumbleType.Side:
				if (!_game.Modifiers.IsActive(_game.Modifiers.Hoverboard) && !_game.IsInJetpackMode && !_game.IsInPogostickMode)
				{
					if (horizontalHit == Character.StumbleHorizontalHit.LeftCorner || horizontalHit == Character.StumbleHorizontalHit.Left)
					{
						characterAnimation.CrossFade(animations.StumbleLeftSide, 0.2f);
					}
					if (horizontalHit == Character.StumbleHorizontalHit.RightCorner || horizontalHit == Character.StumbleHorizontalHit.Right)
					{
						characterAnimation.CrossFade(animations.StumbleRightSide, 0.2f);
					}
				}
				if (!_character.IsJumping)
				{
					characterAnimation.CrossFadeQueued(animations.Run, (!_game.Modifiers.IsActive(_game.Modifiers.Hoverboard)) ? 0.02f : 0.4f);
				}
				return;
			}
			switch (horizontalHit)
			{
			case Character.StumbleHorizontalHit.Center:
				switch (verticalHit)
				{
				case Character.StumbleVerticalHit.Lower:
					if (_game.Modifiers.IsActive(_game.Modifiers.Hoverboard))
					{
						characterAnimation.CrossFade(animations.StumbleMix, 0.05f);
					}
					else
					{
						characterAnimation.CrossFade(animations.Stumble, 0.05f);
					}
					characterAnimation.CrossFadeQueued(animations.Run, 0.5f);
					break;
				case Character.StumbleVerticalHit.Middle:
					characterAnimation.CrossFade(animations.HitMid, 0.07f);
					break;
				case Character.StumbleVerticalHit.Upper:
					characterAnimation.CrossFade(animations.HitUpper, 0.07f);
					break;
				}
				return;
			case Character.StumbleHorizontalHit.Left:
				characterAnimation.Play((!_game.Modifiers.IsActive(_game.Modifiers.Hoverboard)) ? animations.StumbleLeftSide : animations.StumbleLeftCorner);
				break;
			case Character.StumbleHorizontalHit.LeftCorner:
				characterAnimation.Play(animations.StumbleLeftCorner);
				break;
			case Character.StumbleHorizontalHit.Right:
				characterAnimation.Play((!_game.Modifiers.IsActive(_game.Modifiers.Hoverboard)) ? animations.StumbleRightSide : animations.StumbleRightCorner);
				break;
			case Character.StumbleHorizontalHit.RightCorner:
				characterAnimation.Play(animations.StumbleRightCorner);
				break;
			}
			characterAnimation.PlayQueued(animations.Run);
		}

		private void HandleOnDeath()
		{
			_isDeath = true;
			string stumbleDeath = animations.StumbleDeath;
			characterAnimation.CrossFade(stumbleDeath, 0.2f);
		}

		private void OnChangeIsGrounded(bool isGrounded)
		{
			if (_shadowMeshRenderer != null)
			{
				_shadowMeshRenderer.enabled = isGrounded;
			}
		}

		private void OnRoll()
		{
			StartCoroutine(OnRollPlayAnimation());
		}

		private IEnumerator OnRollPlayAnimation()
		{
			_isRolling = true;
			string rollAnimation = animations.Roll;
			string runAnimation = animations.Run;
			if (_superSneakers.isActive)
			{
				SetSpeed(characterAnimation[runAnimation], _DEFAULT_SPEED_RUN_ANIMATION);
			}
			else
			{
				SetSpeed(characterAnimation[runAnimation], _game.NormalizedGameSpeed);
			}
			characterAnimation.CrossFade(rollAnimation, 0.1f);
			if (_hoverboard.isActive)
			{
				characterAnimation.CrossFadeQueued(runAnimation, 0.2f);
				if (_hoverboardAnimation != null)
				{
					_hoverboardAnimation.CrossFade(GetHoverboardAnimationName(rollAnimation), 0.1f);
					_hoverboardAnimation.CrossFadeQueued(GetHoverboardAnimationName(runAnimation), (!_hoverboard.isActive) ? 0f : 0.2f);
				}
			}
			else
			{
				characterAnimation.CrossFadeQueued(runAnimation, 0.1f);
			}
			float endTime = Time.time + characterAnimation[rollAnimation].length;
			while (Time.time < endTime && characterAnimation[rollAnimation].enabled && _isRolling)
			{
				yield return null;
			}
			EndRollAnimation();
		}

		private void EndRollAnimation()
		{
			_isRolling = false;
			_character.EndRoll();
		}

		private void OverrideRollTransition()
		{
			string run = animations.Run;
			if (_hoverboard.isActive)
			{
				characterAnimation.CrossFadeQueued(run, 0.2f);
				if (_hoverboardAnimation != null)
				{
					_hoverboardAnimation.CrossFadeQueued(GetHoverboardAnimationName(run), (!_hoverboard.isActive) ? 0f : 0.2f);
				}
			}
			else
			{
				characterAnimation.CrossFadeQueued(run, 0.1f);
			}
		}

		private void OnHitByTrain()
		{
			characterAnimation.Play(animations.HitMoving);
			Vector3 currentPos = _character.transform.position;
			Vector3 camPos = _character.characterCamera.transform.position;
			StartCoroutine(pTween.To(0.5f, delegate(float t)
			{
				_character.transform.position = Vector3.Lerp(currentPos, new Vector3(camPos.x, camPos.y - 33f, currentPos.z), t);
			}));
		}

		private void OnJump()
		{
			if (_hoverboard.isActive)
			{
				string[] hoverboardJump = animations.HoverboardJump;
				jumpAnimation = hoverboardJump[0];
				hangtimeAnimation = hoverboardJump[1];
			}
			else
			{
				jumpAnimation = animations.Jump;
			}
			characterAnimation.CrossFade(jumpAnimation, 0.15f);
			if (_hoverboard.isActive && _hoverboardAnimation != null)
			{
				_hoverboardAnimation.CrossFade(GetHoverboardAnimationName(jumpAnimation), 0.15f);
			}
		}

		private void OnDoubleJump()
		{
			_isDoublejump = true;
			_characterRenderingEffects.PlayDoubleJumpThruster();
			if (_hoverboard.isActive)
			{
				string[] hoverboardJump = animations.HoverboardJump;
				jumpAnimation = hoverboardJump[0];
				hangtimeAnimation = hoverboardJump[1];
			}
			else
			{
				jumpAnimation = animations.DoubleJump;
			}
			characterAnimation.Play(jumpAnimation);
			if (_hoverboard.isActive && _hoverboardAnimation != null)
			{
				_hoverboardAnimation.CrossFade(GetHoverboardAnimationName(jumpAnimation), 0.15f);
			}
		}

		private void OnRevive()
		{
			StopAllCoroutines();
			_isDeath = false;
			jumpAnimation = animations.Jump;
			characterAnimation.Stop();
			characterAnimation.Play(jumpAnimation);
			if (CoinMagnet.Instance.IsActive)
			{
				characterAnimation["hold_magnet"].enabled = true;
				characterAnimation.Play("hold_magnet");
			}
		}

		private void OnHangtime()
		{
			if (_character.IsRolling)
			{
				return;
			}
			if (_isDoublejump)
			{
				OnDoubleHangtime();
				return;
			}
			if (!_hoverboard.isActive || hangtimeAnimation == null)
			{
				hangtimeAnimation = animations.Hangtime;
			}
			characterAnimation.CrossFade(hangtimeAnimation, 0.3f);
			if (_hoverboardAnimation != null)
			{
				_hoverboardAnimation.CrossFade(GetHoverboardAnimationName(hangtimeAnimation), 0.3f);
			}
		}

		private void OnDoubleHangtime()
		{
			_isDoublejump = false;
			if (!_hoverboard.isActive || hangtimeAnimation == null)
			{
				hangtimeAnimation = animations.DoubleHangtime;
			}
			characterAnimation.CrossFade(hangtimeAnimation, 0.3f);
			if (_hoverboardAnimation != null)
			{
				_hoverboardAnimation.CrossFade(GetHoverboardAnimationName(hangtimeAnimation), 0.3f);
			}
		}

		private void OnLanding(Transform characterTransform)
		{
			string text = animations.Run;
			if (_superSneakers.isActive)
			{
				SetSpeed(characterAnimation[text], _DEFAULT_SPEED_RUN_ANIMATION);
			}
			else
			{
				SetSpeed(characterAnimation[text], _game.NormalizedGameSpeed);
			}
			if (_character.IsRolling)
			{
				return;
			}
			if (_hoverboard.isActive)
			{
				string text2;
				if (_character.IsAboveGround)
				{
					text = animations.Grind;
					text2 = text + "_land";
				}
				else
				{
					text2 = animations.Land;
				}
				characterAnimation.CrossFade(text2, 0.1f);
				characterAnimation.CrossFadeQueued(text, 0.2f);
				if (_hoverboardAnimation != null)
				{
					_hoverboardAnimation.CrossFade(GetHoverboardAnimationName(text2), 0.1f);
					_hoverboardAnimation.CrossFadeQueued(GetHoverboardAnimationName(text), 0.2f);
				}
			}
			else
			{
				string land = animations.Land;
				characterAnimation.CrossFade(land, 0.05f);
				characterAnimation.CrossFadeQueued(text, 0.1f);
			}
		}

		private void OnTutorialMoveBackToCheckPoint(float duration)
		{
			characterAnimation.CrossFade(animations.Run, duration);
		}

		private void OnTutorialStartFromCheckPoint()
		{
			characterAnimation.Play(animations.Run);
		}

		private void OnCatchPlayer(string currentCharacterCaught, float catchUpTime, float waitTimeBeforeScreen)
		{
			_caughtAnimationState = characterAnimation[currentCharacterCaught];
			StartCoroutine(CatchPlayerAnimStarter(_caughtAnimationState, catchUpTime));
		}

		private IEnumerator CatchPlayerAnimStarter(AnimationState caught, float delay)
		{
			caught.weight = 0f;
			caught.normalizedTime = 0f;
			caught.enabled = true;
			yield return new WaitForSeconds(delay);
			AnimationState caught2 = default(AnimationState);
			StartCoroutine(pTween.To(0.2f, delegate(float t)
			{
				caught2.weight = Mathf.Lerp(0f, 1f, t);
			}));
		}

		private void OnStageMenuSequence()
		{
			StopAllCoroutines();
			_isCurrentCustomizationState = _characterModel.OutfitModel.enabled;
			if (characterAnimation != null)
			{
				if (_caughtAnimationState != null)
				{
					_caughtAnimationState.enabled = false;
				}
				characterAnimation.transform.rotation = Quaternion.identity;
				characterAnimation.Play(animations.TopMenu);
			}
			_characterModel.SprayCanModel.SetActive(true);
			_characterModel.StartEyeAnimations(UIModelController.ModelScreen.Character);
			ResetJetpackTrusters();
		}

		public void OnSwitchToRunning()
		{
			if (_character.superSneakers.isActive || _game.HasSuperSneakers)
			{
				animations.RUN = InitializeClips(SuperSneaksAnimations.run);
			}
			else
			{
				animations.RUN = InitializeClips(defaultAnimations.run);
			}
			animations.LAND = InitializeClips(defaultAnimations.landing);
			animations.JUMP = InitializeClips(defaultAnimations.jump);
			animations.HANGTIME = InitializeClips(defaultAnimations.hangtime);
			animations.DOUBLEJUMP = InitializeClips(defaultAnimations.doubleJump);
			animations.DOUBLEHANGTIME = InitializeClips(defaultAnimations.doubleHangtime);
			animations.ROLL = InitializeClips(defaultAnimations.roll);
			animations.DODGE_LEFT = InitializeClips(defaultAnimations.dodgeLeft);
			animations.DODGE_RIGHT = InitializeClips(defaultAnimations.dodgeRight);
			if (_currentHoverboardGameObject != null)
			{
				ToggleCustomHoverboard(null, null);
			}
		}

		private void OnSwitchToHoverboard(GameObject hoverBoard, Hoverboard.BoardAbility[] abilities)
		{
			ToggleCustomHoverboard(hoverBoard, abilities);
			string getOnBoard = animations.GetOnBoard;
			hangtimeAnimation = animations.Hangtime;
			characterAnimation.CrossFade(getOnBoard, 0.1f);
			if (_hoverboardAnimation != null)
			{
				_hoverboardAnimation.CrossFade(GetHoverboardAnimationName(getOnBoard), 0.1f);
			}
			if (!_character.IsFalling && !_character.IsJumping)
			{
				string run = animations.Run;
				characterAnimation.CrossFadeQueued(run, 0.2f);
				if (_hoverboardAnimation != null)
				{
					_hoverboardAnimation.CrossFadeQueued(GetHoverboardAnimationName(run), 0.2f);
				}
			}
			else
			{
				characterAnimation.CrossFade(hangtimeAnimation, 0.2f);
				if (_hoverboardAnimation != null)
				{
					_hoverboardAnimation.CrossFadeQueued(GetHoverboardAnimationName(hangtimeAnimation), 0.2f);
				}
			}
		}

		private void ToggleCustomHoverboard(GameObject newHoverboard, Hoverboard.BoardAbility[] abilities)
		{
			if (_currentHoverboardGameObject != null && _currentHoverboardGameObject != newHoverboard)
			{
				UnityEngine.Object.Destroy(_currentHoverboardGameObject);
			}
			_currentHoverboardGameObject = newHoverboard;
			_currentBoardAbilities = abilities;
			_currentCustomHoverboardModelMaterials = new Material[_characterModel.CurrentCustomBoardModels.Count];
			if (newHoverboard != null)
			{
				_characterModel.CurrentHoverboard.SetActive(true);
				_teleboardWarpGameObject.SetActive(true);
				if (_telewarpRenderer == null)
				{
					_telewarpRenderer = _teleboardWarpGameObject.GetComponentInChildren<Renderer>();
				}
				_telewarpRenderer.enabled = false;
				_teleboardAnimation.normalizedTime = 0f;
				_teleboardAnimation.weight = 0f;
				_teleboardAnimation.enabled = false;
				_hoverboardAnimation = newHoverboard.GetComponent<Animation>();
				_hoverboardRendering = newHoverboard.GetComponent<HoverboardRendering>();
				if (_hoverboardRendering != null)
				{
					_hoverboardRendering.Initialize(characterAnimation, _hoverboardAnimation, _defaultHoverboardAnimationList, _addedAnimClipsNames, abilities);
					return;
				}
				_hoverboardRendering = _defaultHoverboardRendering;
				_hoverboardRendering.Initialize(characterAnimation, _hoverboardAnimation, _defaultHoverboardAnimationList, _addedAnimClipsNames, abilities);
			}
			else
			{
				_hoverboardAnimation = null;
				_hoverboardRendering = null;
				_teleboardWarpGameObject.SetActive(false);
			}
		}

		private void OnSwitchToJetpack(bool isHeadStart)
		{
			animations.RUN = InitializeClips(jetpackAnimations.run);
			animations.DODGE_LEFT = InitializeClips(jetpackAnimations.dodgeLeft);
			animations.DODGE_RIGHT = InitializeClips(jetpackAnimations.dodgeRight);
			animations.TURBO_HEADSTART1 = InitializeClips(jetpackAnimations.turboHeadstart1);
			animations.TURBO_HEADSTART2 = InitializeClips(jetpackAnimations.turboHeadstart2);
			_characterModel.MeshJetpack.enabled = true;
			_characterRenderingEffects.SetRightAndLeftParticlesActive(true);
			string run = animations.Run;
			characterAnimation.CrossFade(run);
		}

		private void JetpackOnFlyAheadStart()
		{
			_initRot = _characterRenderingEffects.JetpackParticles.transform.rotation.eulerAngles;
			_initScale = _characterRenderingEffects.JetpackParticles.transform.localScale;
		}

		private void JetpackOnFlyAheadUpdate(float ratio)
		{
			float num = Mathf.Lerp(0f, 1f, jetpackParticleOffsetCurve.Evaluate(ratio));
			_characterRenderingEffects.JetpackParticles.transform.rotation = Quaternion.Euler(_initRot - new Vector3(num, 0f, 0f));
			_characterRenderingEffects.JetpackParticles.transform.localScale = _initScale + new Vector3(0f, 0f, num * 2f);
		}

		private void JetpackOnStop()
		{
			_characterRenderingEffects.SetRightAndLeftParticlesActive(false);
			_characterRenderingEffects.JetpackParticles.transform.localScale = _initScale;
			_characterModel.MeshJetpack.enabled = false;
			if (_hoverboard.isActive)
			{
				OnSwitchToHoverboard(_currentHoverboardGameObject, _currentBoardAbilities);
			}
			else if (_superSneakers.isActive)
			{
				OnSwitchToSuperSneakers();
				OnHangtime();
			}
			else
			{
				OnSwitchToRunning();
				OnHangtime();
			}
		}

		private void HandleOnTurboHeadstart(int version)
		{
			if (_isRolling)
			{
				EndRollAnimation();
			}
			string text = string.Empty;
			switch (version)
			{
			case 1:
				text = animations.TurboHeadstart1;
				characterAnimation.CrossFade(text, 0.1f);
				characterAnimation.CrossFadeQueued(animations.Run, 0.2f);
				break;
			case 2:
				_character.TurboHeadstartParticleSystem.Play();
				text = animations.TurboHeadstart2;
				characterAnimation.CrossFade(text, 0.1f);
				characterAnimation.CrossFadeQueued(animations.Run, 0.2f);
				break;
			}
			CharacterCamera.Instance.FOVGip(0.8f, 1.2f);
			if (!(text != string.Empty))
			{
				return;
			}
			Transform[] jetpackThrusters = _characterModel.JetpackThrusters;
			foreach (Transform transform in jetpackThrusters)
			{
				transform.GetComponent<Renderer>().enabled = true;
			}
			StopAllCoroutines();
			StartCoroutine(pTween.To(characterAnimation[text].length, delegate(float t)
			{
				JetpackOnFlyAheadUpdate(t);
				_characterCamera.Shake(20f);
				float num = 0f;
				num = ((t < 0.2f) ? (t * 5f) : ((!(t > 0.8f)) ? 1f : ((1f - t) * 5f)));
				num += Mathf.Sin(t * 2f) * 0.2f;
				if (num < 0f)
				{
					num *= -1f;
				}
				Transform[] jetpackThrusters2 = _characterModel.JetpackThrusters;
				foreach (Transform transform2 in jetpackThrusters2)
				{
					if (_jetpackState.TurboHeadstart < 2)
					{
						transform2.localScale = Vector3.one * num;
					}
					else
					{
						transform2.localScale = new Vector3(num * 2.5f, num, num);
					}
				}
				if (t == 1f)
				{
					Transform[] jetpackThrusters3 = _characterModel.JetpackThrusters;
					foreach (Transform transform3 in jetpackThrusters3)
					{
						transform3.GetComponent<Renderer>().enabled = false;
					}
				}
			}));
		}

		private void ResetJetpackTrusters()
		{
			if (_characterModel.JetpackThrusters != null)
			{
				Transform[] jetpackThrusters = _characterModel.JetpackThrusters;
				foreach (Transform transform in jetpackThrusters)
				{
					transform.GetComponent<Renderer>().enabled = false;
				}
			}
		}

		private void HandleOnScorebooster(int tier)
		{
			_characterRenderingEffects.PlayScoreboostFX(tier);
		}

		private void PogostickOnStart()
		{
			animations.RUN = InitializeClips(pogostickClips.run);
			animations.DODGE_LEFT = InitializeClips(pogostickClips.dodgeLeft);
			animations.DODGE_RIGHT = InitializeClips(pogostickClips.dodgeRight);
			animations.HANGTIME = InitializeClips(pogostickClips.hangtime);
			_characterModel.MeshPogostick.enabled = true;
			_characterModel.MeshBlobShadow.enabled = false;
			_characterRenderingEffects.SetPogostickParticlesActive(true);
			ResetTeleportFx();
			string run = animations.Run;
			characterAnimation.CrossFade(run);
		}

		private void PogostickOnHangtime()
		{
			string hangtime = animations.Hangtime;
			characterAnimation.CrossFade(hangtime, 0.2f);
			_characterRenderingEffects.SetPogostickParticlesActive(false);
		}

		private void PogostickOnStop()
		{
			_characterModel.MeshPogostick.enabled = false;
			_characterRenderingEffects.SetPogostickParticlesActive(false);
			if (_hoverboard.isActive)
			{
				OnSwitchToHoverboard(_currentHoverboardGameObject, _currentBoardAbilities);
			}
			else if (_superSneakers.isActive)
			{
				OnSwitchToSuperSneakers();
				OnHangtime();
			}
			else
			{
				OnSwitchToRunning();
				OnHangtime();
			}
		}

		private void IsInGame_OnChange(bool isInGame)
		{
			if (!isInGame)
			{
				JetpackOnStop();
				PogostickOnStop();
			}
		}

		private void OnSwitchToSuperSneakers()
		{
			if (_hoverboardRendering == null)
			{
				OnSwitchToRunning();
				if (!_isRolling)
				{
					OnRun();
				}
				else
				{
					OverrideRollTransition();
				}
			}
		}

		private void SuperSneakersOnStop()
		{
			if (_hoverboardRendering == null)
			{
				OnSwitchToRunning();
				OnRun();
			}
		}

		private void HandleOnTeleboardStart(float duration, Vector3 offset, float gameSpeed)
		{
			if (_isTeleportFxActive)
			{
				StopAllCoroutines();
				ResetCharacterTeleportFX();
			}
			_telewarpRenderer.enabled = true;
			_teleboardAnimation.normalizedTime = 0f;
			_teleboardAnimation.weight = 0f;
			_teleboardAnimation.enabled = false;
			_isTeleportFxActive = true;
			Material sharedMaterial = _characterModel.Model.GetComponent<Renderer>().sharedMaterial;
			if (sharedMaterial != teleboardFXMaterial)
			{
				_currentModelMaterial = sharedMaterial;
			}
			_characterModel.Model.GetComponent<Renderer>().material = teleboardFXMaterial;
			Material[] sharedMaterials = _characterModel.HeadModel.sharedMaterials;
			_currentHeadMaterials = new Material[sharedMaterials.Length];
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				_currentHeadMaterials[i] = sharedMaterials[i];
				sharedMaterials[i] = teleboardFXMaterial;
			}
			_characterModel.HeadModel.materials = sharedMaterials;
			Material sharedMaterial2 = _characterModel.OutfitModel.GetComponent<Renderer>().sharedMaterial;
			if (sharedMaterial2 != teleboardFXMaterial)
			{
				_currentCustomizationMaterial = sharedMaterial2;
			}
			_characterModel.OutfitModel.GetComponent<Renderer>().material = teleboardFXMaterial;
			Material sharedMaterial3 = _characterModel.CurrentHoverboard.GetComponent<Renderer>().sharedMaterial;
			if (sharedMaterial3 != teleboardFXMaterial)
			{
				_currentHoverboardMaterial = sharedMaterial3;
			}
			_characterModel.CurrentHoverboard.GetComponent<Renderer>().material = teleboardFXMaterial;
			Material sharedMaterial4 = _characterModel.MeshSuperSneaker.GetComponent<Renderer>().sharedMaterial;
			if (sharedMaterial4 != teleboardFXMaterial)
			{
				_currentSupersneakersMaterial = sharedMaterial4;
			}
			_characterModel.MeshSuperSneaker.GetComponent<Renderer>().material = teleboardFXMaterial;
			Material sharedMaterial5 = _characterModel.DoubleJumpers.GetComponent<Renderer>().sharedMaterial;
			if (sharedMaterial5 != teleboardFXMaterial)
			{
				_currentDoubleJumperMaterial = sharedMaterial5;
			}
			_characterModel.DoubleJumpers.GetComponent<Renderer>().material = teleboardFXMaterial;
			if (_characterModel.CurrentCustomBoardModels.Count > 0)
			{
				for (int j = 0; j < _characterModel.CurrentCustomBoardModels.Count; j++)
				{
					if (_characterModel.CurrentCustomBoardModels[j] != null)
					{
						Material sharedMaterial6 = _characterModel.CurrentCustomBoardModels[j].GetComponentInChildren<Renderer>().sharedMaterial;
						if (sharedMaterial6 != teleboardFXMaterial && sharedMaterial6 != null)
						{
							_currentCustomHoverboardModelMaterials[j] = sharedMaterial6;
						}
					}
				}
				for (int k = 0; k < _characterModel.CurrentCustomBoardModels.Count; k++)
				{
					if (_characterModel.CurrentCustomBoardModels[k] != null)
					{
						_characterModel.CurrentCustomBoardModels[k].GetComponentInChildren<Renderer>().material = teleboardFXMaterial;
					}
				}
			}
			_isCurrentCustomizationState = _characterModel.OutfitModel.enabled;
			StartCoroutine(TeleboardStartScale(duration, gameSpeed));
		}

		private IEnumerator TeleboardStartScale(float duration, float gameSpeed)
		{
			float time = 0f;
			float t = 0f;
			float z = 0f;
			Vector3 model1position = _character.transform.position;
			HierarchyUtility.ReparentByHierarchyPathType(_characterModel.transform, HierarchyPathType.Null);
			characterAnimation.Stop();
			while (time < duration)
			{
				time += Time.deltaTime;
				t = Mathf.Clamp01(time / duration);
				if (!_isTeleportFxActive)
				{
					ResetTeleportFx();
					break;
				}
				z = _character.transform.position.z;
				model1position.z = z;
				_characterModel.transform.position = model1position;
				_characterModel.transform.localScale = new Vector3(1f - 0.5f * t, 1f + 0.5f * t, 1f - t);
				teleboardFXMaterial.SetFloat("Offset", t * 0.2f);
				if (t == 1f)
				{
					_characterModel.transform.parent = base.transform;
					_characterModel.transform.localPosition = Vector3.zero;
					_characterModel.transform.localScale = Vector3.zero;
					if (_character.IsGrounded.Value)
					{
						OnRun();
					}
					HandleOnTeleboardEnd(duration, gameSpeed);
				}
				yield return null;
			}
		}

		private void HandleOnTeleboardEnd(float duration, float gameSpeed)
		{
			if (_character.IsGrounded.Value)
			{
				OnRun();
			}
			else
			{
				OnHangtime();
			}
			_teleboardWarpGameObject.transform.parent = _character.transform;
			_teleboardWarpGameObject.transform.localPosition = Vector3.zero;
			_teleboardAnimation.normalizedTime = 1f;
			_teleboardAnimation.weight = 1f;
			SetSpeed(_teleboardAnimation, 0f - 0.01f * gameSpeed);
			_teleboardAnimation.enabled = true;
			StartCoroutine(TeleboardEndScale(duration));
		}

		private IEnumerator TeleboardEndScale(float duration)
		{
			float time = 0f;
			float t = 0f;
			while (time < duration)
			{
				time += Time.deltaTime;
				t = Mathf.Clamp01(time / duration);
				teleboardFXMaterial.SetFloat("Offset", t * 0.2f);
				if (t == 1f)
				{
					ResetCharacterTeleportFX();
				}
				yield return null;
			}
		}

		private void ResetCharacterTeleportFX()
		{
			_characterModel.transform.parent = base.transform;
			_characterModel.transform.localPosition = Vector3.zero;
			_characterModel.transform.localScale = Vector3.one;
			_characterModel.Model.enabled = true;
			_characterModel.HeadModel.enabled = true;
			_characterModel.OutfitModel.enabled = _isCurrentCustomizationState;
			if (Hoverboard.Instance.isActive)
			{
				if (_characterModel.CurrentHoverboard != null && !PogoStickState.Instance.isActive)
				{
					_characterModel.CurrentHoverboard.SetActive(true);
				}
				if (_characterModel.CurrentHoverboard != null)
				{
					_characterModel.CurrentHoverboard.GetComponent<Renderer>().material = _currentHoverboardMaterial;
				}
				if (_currentCustomHoverboardModelMaterials != null && _currentCustomHoverboardModelMaterials.Length > 0)
				{
					for (int i = 0; i < _characterModel.CurrentCustomBoardModels.Count; i++)
					{
						if (_characterModel.CurrentCustomBoardModels[i] != null)
						{
							Renderer componentInChildren = _characterModel.CurrentCustomBoardModels[i].GetComponentInChildren<Renderer>();
							if (componentInChildren != null)
							{
								componentInChildren.material = _currentCustomHoverboardModelMaterials[i];
							}
						}
					}
				}
			}
			if (_currentModelMaterial != null)
			{
				_characterModel.Model.GetComponent<Renderer>().material = _currentModelMaterial;
				_currentModelMaterial = null;
			}
			if (_currentHeadMaterials != null)
			{
				_characterModel.HeadModel.materials = _currentHeadMaterials;
				_currentHeadMaterials = null;
			}
			if (_currentCustomizationMaterial != null)
			{
				_characterModel.OutfitModel.GetComponent<Renderer>().material = _currentCustomizationMaterial;
				_currentCustomizationMaterial = null;
			}
			if (_currentSupersneakersMaterial != null)
			{
				_characterModel.MeshSuperSneaker.GetComponent<Renderer>().material = _currentSupersneakersMaterial;
				_currentSupersneakersMaterial = null;
			}
			if (_currentDoubleJumperMaterial != null)
			{
				_characterModel.DoubleJumpers.GetComponent<Renderer>().material = _currentDoubleJumperMaterial;
				_currentDoubleJumperMaterial = null;
			}
			characterAnimation.Play();
			if (_character.IsGrounded.Value)
			{
				OnRun();
			}
			_isTeleportFxActive = false;
		}

		private void OnApplicationPause(bool isApplicationPaused)
		{
			if (isApplicationPaused)
			{
				ResetTeleportFx();
			}
		}

		public void ResetTeleportFx()
		{
			if (_isTeleportFxActive)
			{
				StopAllCoroutines();
				ResetCharacterTeleportFX();
			}
		}
	}
}
