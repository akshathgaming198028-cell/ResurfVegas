using System;
using System.Collections;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Managers;
using SYBO.Core.Utilities;
using SYBO.Subway.Cameras;
using SYBO.Subway.Characters;
using SYBO.Subway.Characters.States;
using SYBO.Subway.Coins;
using SYBO.Subway.Info;
using SYBO.Subway.Managers;
using SYBO.Subway.Pickups;
using SYBO.Subway.Pickups.Log;
using UnityEngine;

namespace SYBO.Subway
{
	public class Game : MonoBehaviour
	{
		public enum GameStateCoroutineType
		{
			None = 0,
			Intro = 1,
			GameIntro = 2,
			TopMenu = 3,
			DieSequence = 4,
			SwitchToDieStateWhenGrounded = 5
		}

		[Serializable]
		public class SwipeInfo
		{
			public float distanceMin = 0.1f;

			public float doubleTapDuration = 0.3f;
		}

		[Serializable]
		public class SpeedInfo
		{
			public float min = 110f;

			public float max = 220f;

			public float rampUpDuration = 200f;
		}

		public delegate void OnSpeedChangedDelegate(float speed);

		public delegate void OnTurboHeadstartInputDelegate(int version);

		public delegate void OnGameOverDelegate(GameStats gameStats);

		public delegate void OnPauseChangeDelegate(bool pause);

		public delegate void OnStageMenuSequenceDelegate();

		public delegate void OnTopMenuDelegate();

		public delegate void OnIntroRunDelegate();

		public const float Z_OFFSET_BACKGROUND_TO_CHARACTER_CAMERA = 900f;

		public const float DISTANCE_PER_METER = 8f;

		private static CharacterController _characterController;

		public static bool HasLoaded;

		public static Action OnGameAwake;

		public static Action OnGameStarted;

		public static Action OnRunStarted;

		public static Action OnRunEnded;

		public static Action OnGameEnded;

		public static Action OnMenuScreenShown;

		public static Action OnGameDestroyed;

		public static Action OnGameEndedLegacy;

		public OnGameOverDelegate OnGameOverLegacy;

		public OnPauseChangeDelegate OnPauseChange;

		public OnStageMenuSequenceDelegate OnStageMenuSequence;

		public OnTopMenuDelegate OnTopMenu;

		public OnIntroRunDelegate OnIntroRun;

		[SerializeField]
		private float highSpeedFOV = 10f;

		[SerializeField]
		private SpeedInfo hotrodSpeedInfo;

		[SerializeField]
		private GameObject characterModelPreviewFactoryPrefab;

		[SerializeField]
		private GameObject hoverboardFactoryPrefab;

		[SerializeField]
		private SpeedInfo highSpeedInfo;

		[SerializeField]
		private float trasitionTime = 1f;

		[SerializeField]
		private float speedFOV = 5f;

		[HideInInspector]
		public bool isDead;

		public bool ingameTouchDetection = true;

		public bool canRevive;

		public float reviveWaitTime = 3f;

		[HideInInspector]
		public float currentSpeed;

		public float currentLevelSpeed = 30f;

		public SwipeInfo swipe;

		public SpeedInfo speed;

		public float backToCheckpointDelayTime = 0.7f;

		public float backToCheckpointZoomTime = 1f;

		private float _elapsedGameTime;

		[OptionalField]
		public AudioStateLoop audioStateLoop;

		public Variable<bool> IsInGame;

		public Variable<bool> IsInTopMenu;

		public AudioClipInfo DieSound;

		public bool awakeDone;

		public bool isReadyForSlideinPowerups;

		public bool wasButtonClicked;

		public DateTime runStartTime;

		[OptionalField]
		public Track track;

		[HideInInspector]
		public string startCharacterName;

		[HideInInspector]
		public int startCharacterTheme;

		public int pogoLevel;

		public int pogoJumpsLeft;

		[OptionalField]
		public Transform introAnimation;

		[OptionalField]
		public Character character;

		private CharacterState _characterState;

		[HideInInspector]
		public CharacterModifierCollection modifiers;

		private RunningState _runningState;

		private JetpackState _jetpack;

		private PogoStickState _pogostick;

		private SpeedInfo _selectedBoardSpeedInfo;

		private float _selectedBoardSpeedFOV;

		private SpeedInfo _currentSpeedInfo = new SpeedInfo();

		private bool goingBackToCheckpoint;

		private SpeedInfo _originalSpeed;

		private IEnumerator _gameStateCoroutine;

		private GameStateCoroutineType _gameStateCoroutineType;

		private Swipe _currentSwipe;

		private float _lastTapTime = float.MinValue;

		private CharacterRendering _characterRendering;

		private CharacterCamera _characterCamera;

		private Transform _characterCameraTransform;

		private FollowingGuard _followingGuard;

		private GameObject[] _menuFactories = new GameObject[2];

		private GameStats _gameStats;

		private float _startTime;

		private float _internalStartTime;

		private bool _paused;

		private float _waitTimeBeforeScreen = 3f;

		private PickupManagerController _pickupManagerController;

		private static Game _instance;

		private IEnumerator _debugRepeatedlySwipeLeftRightCoroutine;

		private bool _isInStumbleDeathSequence;

		public static CharacterController Charactercontroller
		{
			get
			{
				return _characterController ?? (_characterController = UnityEngine.Object.FindObjectOfType(typeof(CharacterController)) as CharacterController);
			}
		}

		public float HighSpeedFOV
		{
			get
			{
				return highSpeedFOV;
			}
		}

		public float SpeedFOV
		{
			get
			{
				return speedFOV;
			}
		}

		public float ElapsedGameTime
		{
			get
			{
				return _elapsedGameTime;
			}
		}

		public Character Character
		{
			get
			{
				return character;
			}
		}

		public CharacterState CharacterState
		{
			get
			{
				return _characterState;
			}
		}

		public CharacterModifierCollection Modifiers
		{
			get
			{
				return modifiers;
			}
		}

		public RunningState RunningState
		{
			get
			{
				if (_runningState == null)
				{
					_runningState = GetComponentInChildren<RunningState>();
				}
				return _runningState;
			}
		}

		public JetpackState Jetpack
		{
			get
			{
				return _jetpack;
			}
		}

		public PogoStickState PogoStick
		{
			get
			{
				return _pogostick;
			}
		}

		public bool IsInJetpackMode
		{
			get
			{
				return _characterState == Jetpack;
			}
		}

		public bool IsInPogostickMode
		{
			get
			{
				return _characterState == PogoStick;
			}
		}

		public bool HasSuperSneakers
		{
			get
			{
				return modifiers.SuperSneakes.isActive;
			}
		}

		public float NormalizedGameSpeed
		{
			get
			{
				return currentSpeed / speed.min;
			}
		}

		public bool IsPaused
		{
			get
			{
				return _paused;
			}
		}

		public static Game Instance
		{
			get
			{
				return _instance ?? (_instance = Utils.FindObject<Game>());
			}
		}

		public bool isInStumbleDeathSequence
		{
			get
			{
				return _isInStumbleDeathSequence;
			}
		}

		public event EventHandler<CharacterStateChangedEventArgs> OnCharacterStateChanged;

		public event OnSpeedChangedDelegate OnSpeedChanged;

		public event OnTurboHeadstartInputDelegate OnTurboHeadstartInput;

		public event Action OnDeath;

		public event Action OnCharacterModelRecreated;

		public Game()
		{
			IsInGame = new Variable<bool>(false);
			IsInTopMenu = new Variable<bool>(false);
		}

		public static float ConvertUnityUnitsToGameMeters(float unityUnits)
		{
			return unityUnits / 8f;
		}

		public static float ConvertGameMetersToUnityUnits(float meters)
		{
			return meters * 8f;
		}

		public float GetDuration()
		{
			return Time.time - _startTime;
		}

		public static bool IsInstantiated()
		{
			return _instance != null;
		}

		protected void Awake()
		{
			SingletonMonobehavior<SpawnPoolManager>.Instantiate();
			SingletonMonobehavior<TimeScaleManager>.Instantiate();
			SingletonMonobehavior<CoinManager2>.Instantiate();
			if (PickupManager2.WillBeInstantiatedByGameClass)
			{
				SingletonMonobehavior<PickupManager2>.Instantiate();
				OnRunStarted = (Action)Delegate.Combine(OnRunStarted, new Action(SingletonMonobehavior<PickupManager2>.Instance.Game_OnRunStarted));
				OnGameStarted = (Action)Delegate.Combine(OnGameStarted, new Action(SingletonMonobehavior<PickupManager2>.Instance.Game_OnGameStarted));
				_pickupManagerController = new PickupManagerController(SingletonMonobehavior<PickupManager2>.Instance);
			}
			if (PickupLogManager.WillBeInstantiated)
			{
				SingletonMonobehavior<PickupLogManager>.Instantiate();
				OnGameAwake = (Action)Delegate.Combine(OnGameAwake, new Action(SingletonMonobehavior<PickupLogManager>.Instance.Game_OnGameAwake));
				OnGameStarted = (Action)Delegate.Combine(OnGameStarted, new Action(SingletonMonobehavior<PickupLogManager>.Instance.Game_OnGameStarted));
				OnRunStarted = (Action)Delegate.Combine(OnRunStarted, new Action(SingletonMonobehavior<PickupLogManager>.Instance.Game_OnRunStarted));
				OnRunEnded = (Action)Delegate.Combine(OnRunEnded, new Action(SingletonMonobehavior<PickupLogManager>.Instance.Game_OnRunEnded));
				OnGameEnded = (Action)Delegate.Combine(OnGameEnded, new Action(SingletonMonobehavior<PickupLogManager>.Instance.Game_OnGameEnded));
				OnMenuScreenShown = (Action)Delegate.Combine(OnMenuScreenShown, new Action(SingletonMonobehavior<PickupLogManager>.Instance.Game_OnMenuScreenShown));
				OnGameDestroyed = (Action)Delegate.Combine(OnGameDestroyed, new Action(SingletonMonobehavior<PickupLogManager>.Instance.Game_OnGameDestroyed));
			}
			_originalSpeed = speed;
			LoadFactories();
			HasLoaded = true;
			character = Character.Instance;
			character.Initialize();
			_characterRendering = CharacterRendering.Instance;
			_characterCamera = CharacterCamera.Instance;
			this.OnCharacterStateChanged = (EventHandler<CharacterStateChangedEventArgs>)Delegate.Combine(this.OnCharacterStateChanged, new EventHandler<CharacterStateChangedEventArgs>(_characterCamera.Game_OnCharacterStateChanged));
			_characterCameraTransform = _characterCamera.transform;
			track = Track.Instance;
			_runningState = RunningState.Instance;
			_jetpack = JetpackState.Instance;
			_pogostick = PogoStickState.Instance;
			_followingGuard = FollowingGuard.Instance;
			_followingGuard.Initialize();
			FollowingGuard followingGuard = _followingGuard;
			followingGuard.OnCatchPlayer = (FollowingGuard.OnCatchPlayerDelegate)Delegate.Combine(followingGuard.OnCatchPlayer, new FollowingGuard.OnCatchPlayerDelegate(OnCatchPlayer));
			modifiers = new CharacterModifierCollection();
			character.OnStumble += OnStumble;
			character.OnCriticalHit += OnCriticalHit;
			currentLevelSpeed = Speed(0f, _currentSpeedInfo);
			_gameStats = GameStats.Instance;
			awakeDone = true;
			if (OnGameAwake != null)
			{
				OnGameAwake();
			}
			Variable<bool> isInTopMenu = IsInTopMenu;
			isInTopMenu.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isInTopMenu.OnChange, (Variable<bool>.OnChangeDelegate)delegate(bool value)
			{
				if (value)
				{
					_characterRendering.CharacterModel.StartEyeAnimations(UIModelController.ModelScreen.Character);
				}
				else
				{
					_characterRendering.CharacterModel.StartEyeAnimations(UIModelController.ModelScreen.Character);
				}
			});
		}

		protected void Start()
		{
			ChangeGameStateCoroutine(GameStateCoroutineType.GameIntro);
			_gameStateCoroutine.MoveNext();
		}

		protected void OnDestroy()
		{
			if (_pickupManagerController != null)
			{
				_pickupManagerController.Destroy();
			}
			if (SingletonMonobehavior<PickupManager2>.IsInstantiated())
			{
				SingletonMonobehavior<PickupManager2>.Destroy();
			}
			if (_characterCamera != null)
			{
				this.OnCharacterStateChanged = (EventHandler<CharacterStateChangedEventArgs>)Delegate.Remove(this.OnCharacterStateChanged, new EventHandler<CharacterStateChangedEventArgs>(_characterCamera.Game_OnCharacterStateChanged));
			}
			if (OnGameDestroyed != null)
			{
				OnGameDestroyed();
			}
		}

		public void RecreateCharacterModel()
		{
			Character.RecreateCharacterModel();
			if (this.OnCharacterModelRecreated != null)
			{
				this.OnCharacterModelRecreated();
			}
		}

		public void ChangeCurrentSpeed(float timeOffset)
		{
			_internalStartTime -= timeOffset;
		}

		public void StartNewRun(bool duel)
		{
			runStartTime = DateTime.UtcNow;
			_isInStumbleDeathSequence = false;
			startCharacterName = SYBO.Subway.Info.Characters.characterData[(SYBO.Subway.Info.Characters.CharacterType)PlayerInfo.Instance.currentCharacter].modelName;
			startCharacterTheme = PlayerInfo.Instance.currentThemeIndex;
			SaveMeManager.IS_PURCHASE_RUNNING_INGAME = false;
			if (Globals.DEBUG_PRINT_SPAWNED_POWER_UPS)
			{
				SpawnedObjects.startPowerUpCount();
			}
			IsInTopMenu.Value = false;
			IsInGame.Value = true;
			Hoverboard.Instance.HardReset();
			ChangeState(null, GameStateCoroutineType.Intro);
			Action onGameStarted = OnGameStarted;
			if (onGameStarted != null)
			{
				onGameStarted();
			}
			Action onRunStarted = OnRunStarted;
			if (onRunStarted != null)
			{
				onRunStarted();
			}
			GameStats.Instance.characterDayEventActivated = false;
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.CharacterDay);
			if (PlayerInfo.Instance.IsTutorialCompleted && activeEventManager != null)
			{
				SYBO.Subway.Info.Characters.CharacterType currentCharacter = (SYBO.Subway.Info.Characters.CharacterType)PlayerInfo.Instance.currentCharacter;
				bool found = false;
				SYBO.Subway.Info.Characters.CharacterType eventValue = activeEventManager.GetEventValue<SYBO.Subway.Info.Characters.CharacterType>(out found);
				if (found && currentCharacter == eventValue)
				{
					StartCoroutine(EnableCharacterDayEventAnimation());
				}
			}
			VideoAdsManager instance = VideoAdsManager.instance;
			if (instance.isInitialized)
			{
				instance.StopProvider();
			}
		}

		public void Update()
		{
			float num = 0f;
			if (IsInGame.Value)
			{
				_elapsedGameTime += Time.deltaTime;
			}
			if (!SaveMeManager.IS_PURCHASE_RUNNING_INGAME && track != null && !track.IsRunningOnTutorialTrack)
			{
				num = Time.time - _internalStartTime;
				currentLevelSpeed = Speed(num, _currentSpeedInfo);
			}
			else
			{
				_startTime += Time.deltaTime;
				_internalStartTime += Time.deltaTime;
			}
			if (_gameStateCoroutine != null)
			{
				_gameStateCoroutine.MoveNext();
			}
			if (_characterState != null)
			{
				modifiers.Update();
			}
			GameStats.Instance.UpdatePowerupTimes(Time.deltaTime);
			if (_pickupManagerController != null)
			{
				_pickupManagerController.DoUpdate();
			}
		}

		public void LayTrackChunks()
		{
			track.LayTrackChunks(character.z);
		}

		public void UpdateMeters()
		{
			_gameStats.MetersInCurrentGame = Mathf.RoundToInt(character.z / 8f);
			if (Globals.DEBUG_PRINT_SPAWNED_POWER_UPS)
			{
				SpawnedObjects.metersCounter = (int)_gameStats.MetersInCurrentGame;
			}
		}

		public void ChangeState(CharacterState newCharacterState)
		{
			CharacterState characterState = _characterState;
			_characterState = newCharacterState;
			if (newCharacterState != null)
			{
				_gameStateCoroutine = newCharacterState.Begin();
			}
			RaiseOnCharacterStateChanged(new CharacterStateChangedEventArgs(characterState, _characterState));
		}

		public void ChangeState(CharacterState newCharacterState, GameStateCoroutineType gameStateCoroutineType)
		{
			CharacterState characterState = _characterState;
			_characterState = newCharacterState;
			ChangeGameStateCoroutine(gameStateCoroutineType);
			RaiseOnCharacterStateChanged(new CharacterStateChangedEventArgs(characterState, _characterState));
		}

		private void ChangeGameStateCoroutine(GameStateCoroutineType gameStateCoroutineType)
		{
			if (_gameStateCoroutineType != gameStateCoroutineType)
			{
				_gameStateCoroutineType = gameStateCoroutineType;
				switch (gameStateCoroutineType)
				{
				case GameStateCoroutineType.Intro:
					_gameStateCoroutine = Intro();
					break;
				case GameStateCoroutineType.GameIntro:
					_gameStateCoroutine = GameIntro();
					break;
				case GameStateCoroutineType.TopMenu:
					_gameStateCoroutine = TopMenu();
					break;
				case GameStateCoroutineType.DieSequence:
					_gameStateCoroutine = DieSequence();
					break;
				case GameStateCoroutineType.SwitchToDieStateWhenGrounded:
					_gameStateCoroutine = SwitchToDieStateWhenGrounded();
					break;
				case GameStateCoroutineType.None:
					_gameStateCoroutine = null;
					break;
				}
			}
		}

		public void ActivateJetpack()
		{
			if (_characterState != Jetpack)
			{
				ChangeState(Jetpack);
			}
		}

		public void ActivateHighSpeed(string type)
		{
			if (type == "highSpeedboard")
			{
				_selectedBoardSpeedInfo = highSpeedInfo;
				_selectedBoardSpeedFOV = highSpeedFOV;
			}
			else if (type == "speedboard")
			{
				_selectedBoardSpeedInfo = hotrodSpeedInfo;
				_selectedBoardSpeedFOV = speedFOV;
			}
			float fov_start = _characterCamera.Camera.fieldOfView;
			float fov_end = (RunningState.cameraFOV + _selectedBoardSpeedFOV) / _characterCamera.Camera.aspect;
			StartCoroutine(pTween.To(trasitionTime, delegate(float t)
			{
				_currentSpeedInfo.min = Mathf.Lerp(speed.min, _selectedBoardSpeedInfo.min, t);
				_currentSpeedInfo.max = Mathf.Lerp(speed.max, _selectedBoardSpeedInfo.max, t);
				if (this.OnSpeedChanged != null)
				{
					this.OnSpeedChanged(Speed(Time.time - _internalStartTime, _currentSpeedInfo));
				}
				_characterCamera.Camera.fieldOfView = Mathf.Lerp(fov_start, fov_end, t);
			}));
		}

		public void DeactivateHighSpeed()
		{
			float fov_start = _characterCamera.Camera.fieldOfView;
			float fov_end = RunningState.cameraFOV / _characterCamera.Camera.aspect;
			StartCoroutine(pTween.To(trasitionTime, delegate(float t)
			{
				if (_characterState == RunningState || _characterState == Jetpack)
				{
					_currentSpeedInfo.min = Mathf.Lerp(_selectedBoardSpeedInfo.min, speed.min, t);
					_currentSpeedInfo.max = Mathf.Lerp(_selectedBoardSpeedInfo.max, speed.max, t);
					if (this.OnSpeedChanged != null)
					{
						this.OnSpeedChanged(Speed(Time.time - _internalStartTime, _currentSpeedInfo));
					}
					_characterCamera.Camera.fieldOfView = Mathf.Lerp(fov_start, fov_end, t);
				}
				else
				{
					_currentSpeedInfo.min = speed.min;
					_currentSpeedInfo.max = speed.max;
					if (this.OnSpeedChanged != null)
					{
						this.OnSpeedChanged(Speed(0f, _currentSpeedInfo));
					}
					_characterCamera.Camera.fieldOfView = RunningState.cameraFOV / _characterCamera.Camera.aspect;
				}
			}));
		}

		public void PickupJetpack()
		{
			Instance.StartJetpack();
			GameStats.Instance.jetpackPickups++;
		}

		public void StartJetpack()
		{
			Jetpack.IsHeadStart = false;
			Jetpack.PowerupType = PowerupType.jetpack;
			ChangeState(Jetpack);
		}

		public void PickupPogostick()
		{
			pogoJumpsLeft = pogoLevel;
			StartPogostick();
			GameStats.Instance.powerJumperPickups++;
		}

		public void StartPogostick()
		{
			PogoStick.powerType = PowerupType.pogostick;
			ChangeState(PogoStick);
		}

		public void StartTopMenu()
		{
			ChangeState(null, GameStateCoroutineType.TopMenu);
		}

		public void Megaheadstart()
		{
			if (_jetpack.CanActivateLongHeadstart && _jetpack.TurboHeadstart < 2)
			{
				Jetpack.ActivateLongHeadstart();
				if (_jetpack.TurboHeadstart == 1)
				{
					Jetpack.PowerupType = PowerupType.turboHeadstart1;
				}
				if (_jetpack.TurboHeadstart == 2)
				{
					Jetpack.PowerupType = PowerupType.turboHeadstart2;
				}
			}
			else if (_jetpack.TurboHeadstart == 0)
			{
				if (isDead)
				{
					return;
				}
				Jetpack.IsHeadStart = true;
				Jetpack.PowerupType = PowerupType.headstart;
				ChangeState(Jetpack);
			}
			if (this.OnTurboHeadstartInput != null)
			{
				this.OnTurboHeadstartInput(_jetpack.TurboHeadstart);
			}
		}

		public void Die()
		{
			if (modifiers.IsActive(modifiers.Hoverboard))
			{
				if (modifiers.Hoverboard.IsTeleporter)
				{
					CharacterRendering.Instance.ResetTeleportFx();
				}
				_followingGuard.Restart(true);
				modifiers.Hoverboard.Stop = CharacterModifier.StopSignal.STOP;
				_followingGuard.MuteProximityLoop();
				_followingGuard.ResetCatchUp();
				if (character.IsStumbling)
				{
					character.StopStumble();
				}
				GameStats.Instance.RemoveHoverBoardPowerup();
				return;
			}
			if (track.IsRunningOnTutorialTrack)
			{
				if (!goingBackToCheckpoint)
				{
					StartCoroutine(BackToCheckPointSequence());
				}
				return;
			}
			canRevive = true;
			GameStats.Instance.PAUSEPOWERUPS = true;
			isDead = true;
			MovingTrain.ActivateAutoPilot();
			MovingCoin.ActivateAutoPilot();
			if (_followingGuard.isShowing)
			{
				if (_characterRendering.CharacterAnimation["death_movingTrain"].enabled)
				{
					_followingGuard.HitByTrainSequence();
				}
				else
				{
					_followingGuard.CatchPlayer(character.x - character.GetTrackX());
				}
			}
			_gameStats.duration = GetDuration();
			_followingGuard.enabled = false;
			if (OnGameOverLegacy != null)
			{
				OnGameOverLegacy(_gameStats);
			}
			Action onGameEndedLegacy = OnGameEndedLegacy;
			if (onGameEndedLegacy != null)
			{
				onGameEndedLegacy();
			}
			StopAllCoroutines();
			ChangeState(null, GameStateCoroutineType.SwitchToDieStateWhenGrounded);
		}

		public void Revive()
		{
			if (OnRunStarted != null)
			{
				OnRunStarted();
			}
			ResetEnemy();
			character.Revive();
			GameStats.Instance.PAUSEPOWERUPS = false;
			IsInGame.Value = true;
			StopAllCoroutines();
			SingletonMonobehavior<TimeScaleManager>.Instance.TimeScale = SingletonMonobehavior<TimeScaleManager>.Instance.TimeScaleInGameDefault;
			ChangeState(RunningState);
			canRevive = false;
			isDead = false;
			_isInStumbleDeathSequence = false;
		}

		public void ResetEnemy()
		{
			_followingGuard.enabled = true;
			_followingGuard.MuteProximityLoop();
			_followingGuard.ResetCatchUp();
			_followingGuard.ResetModelRootPosition();
			_followingGuard.ShowEnemies(false);
		}

		public IEnumerator SkipRevive()
		{
			ingameTouchDetection = false;
			canRevive = false;
			yield return new WaitForSeconds(0.1f);
			ChangeState(null, GameStateCoroutineType.TopMenu);
			StartCoroutine("DelayGameOverScreen");
		}

		private void LoadFactories()
		{
			if (_menuFactories[0] == null)
			{
				_menuFactories[0] = UnityEngine.Object.Instantiate(characterModelPreviewFactoryPrefab);
				_menuFactories[1] = UnityEngine.Object.Instantiate(hoverboardFactoryPrefab);
				HierarchyUtility.ReparentByHierarchyPathType(_menuFactories[0].transform, HierarchyPathType.DynamicManagers);
				HierarchyUtility.ReparentByHierarchyPathType(_menuFactories[1].transform, HierarchyPathType.DynamicManagers);
			}
		}

		public void TriggerPause(bool pauseGame)
		{
			_paused = pauseGame;
			if (pauseGame)
			{
				ingameTouchDetection = false;
				SingletonMonobehavior<TimeScaleManager>.Instance.TimeScale = 0f;
			}
			else
			{
				ingameTouchDetection = true;
				SingletonMonobehavior<TimeScaleManager>.Instance.TimeScale = SingletonMonobehavior<TimeScaleManager>.Instance.TimeScaleInMenuDefault;
			}
			if (OnPauseChange != null)
			{
				OnPauseChange(_paused);
			}
		}

		public void HandleControls()
		{
			if (_paused || Input.touchCount <= 0)
			{
				return;
			}
			Touch touch = Input.touches[0];
			if (touch.phase == TouchPhase.Began)
			{
				_currentSwipe = new Swipe();
				_currentSwipe.start = touch.position;
				_currentSwipe.startTime = Time.time;
			}
			if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && _currentSwipe != null)
			{
				_currentSwipe.endTime = Time.time;
				_currentSwipe.end = touch.position;
				SwipeDir swipeDir = AnalyzeSwipe(_currentSwipe);
				if (swipeDir != SwipeDir.None)
				{
					if (_characterState != null)
					{
						_characterState.HandleSwipe(swipeDir);
					}
					_currentSwipe = null;
				}
			}
			if (touch.phase == TouchPhase.Ended && _currentSwipe != null)
			{
				_currentSwipe.endTime = Time.time;
				_currentSwipe.end = touch.position;
				SwipeDir swipeDir2 = AnalyzeSwipe(_currentSwipe);
				if (swipeDir2 == SwipeDir.None && _characterState != null)
				{
					HandleTap();
				}
			}
		}

		private bool HandleTap()
		{
			bool result = false;
			if (Time.time < _lastTapTime + swipe.doubleTapDuration && _characterState != null && !wasButtonClicked)
			{
				_characterState.HandleDoubleTap();
				result = true;
			}
			wasButtonClicked = false;
			_lastTapTime = Time.time;
			return result;
		}

		private SwipeDir AnalyzeSwipe(Swipe swipe)
		{
			Vector3 b = Camera.main.ScreenToWorldPoint(new Vector3(swipe.start.x, swipe.start.y, 2f));
			Vector3 a = Camera.main.ScreenToWorldPoint(new Vector3(swipe.end.x, swipe.end.y, 2f));
			float num = Vector3.Distance(a, b);
			if (num < this.swipe.distanceMin)
			{
				return SwipeDir.None;
			}
			Vector3 lhs = swipe.end - swipe.start;
			SwipeDir result = SwipeDir.None;
			float num2 = 0f;
			float num3 = Vector3.Dot(lhs, Vector3.up);
			if (num3 > num2)
			{
				num2 = num3;
				result = SwipeDir.Up;
			}
			num3 = Vector3.Dot(lhs, Vector3.down);
			if (num3 > num2)
			{
				num2 = num3;
				result = SwipeDir.Down;
			}
			num3 = Vector3.Dot(lhs, Vector3.left);
			if (num3 > num2)
			{
				num2 = num3;
				result = SwipeDir.Left;
			}
			num3 = Vector3.Dot(lhs, Vector3.right);
			if (num3 > num2)
			{
				num2 = num3;
				result = SwipeDir.Right;
			}
			return result;
		}

		private IEnumerator BackToCheckPointSequence()
		{
			goingBackToCheckpoint = true;
			ChangeState(null);
			yield return new WaitForSeconds(backToCheckpointDelayTime);
			if (!IsInGame.Value)
			{
				goingBackToCheckpoint = false;
				yield break;
			}
			character.SetBackToCheckPoint(backToCheckpointZoomTime);
			yield return new WaitForSeconds(backToCheckpointZoomTime);
			goingBackToCheckpoint = false;
		}

		private IEnumerator EnableCharacterDayEventAnimation()
		{
			yield return new WaitForEndOfFrame();
			GameStats.Instance.characterDayEventActivated = true;
		}

		private void HandleDebugControls()
		{
			if (Input.GetKeyUp(KeyCode.N) && !Jetpack.isActive)
			{
				Megaheadstart();
			}
			if (Input.GetKeyUp(KeyCode.J) && !Jetpack.isActive)
			{
				StartJetpack();
			}
			if (Input.GetKeyDown(KeyCode.S) && !Jetpack.isActive && !PogoStick.isActive)
			{
				GameStats.Instance.superSneakerPickups++;
				modifiers.Add(modifiers.SuperSneakes);
			}
			if (Input.GetKeyDown(KeyCode.M))
			{
				GameStats.Instance.coinMagnetsPickups++;
				modifiers.Add(modifiers.CoinMagnet);
			}
			if (Input.GetKeyDown(KeyCode.X))
			{
				if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
				{
					modifiers.ExpireSoon();
				}
				else
				{
					modifiers.Stop();
				}
			}
			if (Input.GetKeyDown(KeyCode.R))
			{
				CoinLineManager.Instance.DebugReleaseCoins();
				SingletonMonobehavior<CoinSingleManager>.Instance.DebugReleaseCoins();
				SingletonMonobehavior<CoinJumpCurveManager>.Instance.DebugReleaseCoins();
			}
			if (Input.GetKeyDown(KeyCode.O))
			{
				if (_debugRepeatedlySwipeLeftRightCoroutine != null)
				{
					StopCoroutine(_debugRepeatedlySwipeLeftRightCoroutine);
					_debugRepeatedlySwipeLeftRightCoroutine = null;
				}
				else
				{
					_debugRepeatedlySwipeLeftRightCoroutine = DebugRepeatedlySwipeLeftRightCoroutine();
					StartCoroutine(_debugRepeatedlySwipeLeftRightCoroutine);
				}
			}
			if (Input.GetKeyDown(KeyCode.P) && UIScreenController.IsInstanced)
			{
				UIScreenController.Instance.DebugSetApplicationPause(!UIScreenController.Instance.IsApplicationPaused);
			}
			if (Input.GetKeyDown(KeyCode.F) && UIScreenController.IsInstanced)
			{
				UIScreenController.Instance.DebugSetApplicationFocus(!UIScreenController.Instance.IsApplicationFocused);
			}
			if (!(_characterState != null))
			{
				return;
			}
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				if (Input.GetKeyDown(KeyCode.UpArrow) && _characterState == RunningState)
				{
					(_characterState as RunningState).DebugSuperJump();
				}
				return;
			}
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				_characterState.HandleSwipe(SwipeDir.Up);
			}
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				_characterState.HandleSwipe(SwipeDir.Down);
			}
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				_characterState.HandleSwipe(SwipeDir.Left);
			}
			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				_characterState.HandleSwipe(SwipeDir.Right);
			}
		}

		private IEnumerator DebugRepeatedlySwipeLeftRightCoroutine()
		{
			while (true)
			{
				yield return new WaitForSeconds(0.05f);
				_characterState.HandleSwipe(SwipeDir.Left);
				yield return new WaitForSeconds(0.05f);
				_characterState.HandleSwipe(SwipeDir.Right);
			}
		}

		private float Speed(float t, SpeedInfo speedInfo)
		{
			if (t < speed.rampUpDuration)
			{
				return t * (speedInfo.max - speedInfo.min) / speed.rampUpDuration + speedInfo.min;
			}
			return speedInfo.max;
		}

		private IEnumerator StumbleDeathSequence()
		{
			_isInStumbleDeathSequence = true;
			currentSpeed = speed.min;
			yield return new WaitForSeconds(0.2f);
			if (!IsInJetpackMode && IsInGame.Value && !IsInPogostickMode)
			{
				if (this.OnDeath != null)
				{
					this.OnDeath();
				}
				if (_characterState != null)
				{
					_characterState.HandleCriticalHit();
				}
			}
			_isInStumbleDeathSequence = false;
		}

		private void OnCatchPlayer(string currentCharacterCaught, float catchUpTime, float waitTime)
		{
			_waitTimeBeforeScreen = reviveWaitTime;
		}

		private IEnumerator SwitchToDieStateWhenGrounded()
		{
			while (!character.characterController.isGrounded)
			{
				character.MoveWithGravity();
				yield return null;
			}
			ChangeState(null, GameStateCoroutineType.DieSequence);
		}

		private IEnumerator DieSequence()
		{
			SingletonMonobehavior<TimeScaleManager>.Instance.TimeScale = SingletonMonobehavior<TimeScaleManager>.Instance.TimeScaleInMenuDefault;
			if (OnRunEnded != null)
			{
				OnRunEnded();
			}
			float wait = Time.time + _waitTimeBeforeScreen;
			float skipTime = Time.time + 0.5f;
			while (Time.time < skipTime)
			{
				yield return null;
			}
			MissionsManager.Instance.PlayerDidThis(MissionTarget.TimeDeath, Mathf.FloorToInt(GetDuration()));
			if (SaveMeManager.IsSaveMePopupEnabled())
			{
				UIScreenController.Instance.QueuePopup("SaveMePopup");
			}
			while (Time.time < wait && !Input.GetMouseButtonUp(0))
			{
				if (Input.touchCount > 0)
				{
					Touch touch = Input.touches[0];
					if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
					{
						break;
					}
				}
				yield return null;
			}
			ingameTouchDetection = false;
			LoadFactories();
			if (!SaveMeManager.IsSaveMePopupEnabled())
			{
				ChangeState(null, GameStateCoroutineType.TopMenu);
				StartCoroutine("DelayGameOverScreen");
				canRevive = false;
			}
		}

		private IEnumerator DelayGameOverScreen()
		{
			yield return null;
			UIScreenController.Instance.GameOverTriggered();
			if (SwapCharacter.swapCharacterEnabled)
			{
				SwapCharacter.Instance.StopSwapCharacter();
				GameStats.Instance.RemoveSwapCharacter();
			}
			if (OnGameEnded != null)
			{
				OnGameEnded();
			}
		}

		private void StageMenuSequence()
		{
			LoadFactories();
			_followingGuard.enabled = false;
			_followingGuard.ShowEnemies(false);
			_followingGuard.StopAllCoroutines();
			character.StopAllCoroutines();
			character.transform.position = Vector3.zero + new Vector3(0f, 0f, 0f);
			_characterCamera.enabled = false;
			_characterCamera.Camera.fieldOfView = 43f / _characterCamera.Camera.aspect;
			_characterCameraTransform.localPosition = character.transform.position + RunningState.cameraOffset + Vector3.up * 0.8f;
			_characterCameraTransform.localRotation = Quaternion.Euler(21.50143f, 0f, 0f);
			if (OnStageMenuSequence != null)
			{
				OnStageMenuSequence();
			}
			if (character.superSneakers.isActive)
			{
				character.superSneakers.StopHack();
				character.IsJumpingHigher = false;
			}
			Jetpack.ResetTurboHeadstart();
			GameStats.Instance.ClearPowerups();
			GameStats.Instance.PAUSEPOWERUPS = false;
		}

		private IEnumerator GameIntro()
		{
			LoadFactories();
			if (UIScreenController.IsInstanced)
			{
				UIScreenController.Instance.ShowMainMenu();
			}
			ChangeState(null, GameStateCoroutineType.TopMenu);
			yield break;
		}

		private IEnumerator TopMenu()
		{
			LoadFactories();
			IsInGame.Value = false;
			IsInTopMenu.Value = true;
			if (audioStateLoop != null)
			{
				audioStateLoop.ChangeLoop(AudioState.Menu);
			}
			_followingGuard.MuteProximityLoop();
			modifiers.StopWithNoEnding();
			modifiers.Update();
			GameStats.Instance.ClearPowerups();
			_followingGuard.ShowEnemies(false);
			StageMenuSequence();
			_characterCamera.transform.parent.GetComponent<Animation>().CrossFade("menuIdle", 0.1f);
			TrackPrepare();
			Action handler = OnMenuScreenShown;
			if (handler != null)
			{
				handler();
			}
			if (OnTopMenu != null)
			{
				OnTopMenu();
			}
			yield return null;
		}

		private IEnumerator Intro()
		{
			_gameStats.Reset();
			modifiers.Stop();
			modifiers.Update();
			audioStateLoop.ChangeLoop(AudioState.Ingame);
			_followingGuard.MuteProximityLoop();
			isDead = false;
			ingameTouchDetection = true;
			character.CharacterPickupParticleSystem.CoinEFX.transform.localPosition = CharacterPickupParticles.coinEfxOffset;
			StageMenuSequence();
			_followingGuard.ShowEnemies(true);
			_followingGuard.PlayIntro();
			speed.min = _originalSpeed.min;
			speed.max = _originalSpeed.max;
			speed.rampUpDuration = _originalSpeed.rampUpDuration;
			currentLevelSpeed = Speed(0f, _currentSpeedInfo);
			_startTime = Time.time;
			_internalStartTime = Time.time;
			character.Restart();
			track.Activate();
			_characterCamera.transform.parent.GetComponent<Animation>().CrossFade("startPan", 0.2f);
			_characterRendering.CharacterAnimation.CrossFade("introRun", 0.2f);
			if (OnIntroRun != null)
			{
				OnIntroRun();
			}
			IEnumerator cameraMovement = pTween.To(_characterRendering.CharacterAnimation["introRun"].length, delegate
			{
			});
			float time = Time.time;
			float fov_start = _characterCamera.Camera.fieldOfView;
			float fov_end = RunningState.cameraFOV / _characterCamera.Camera.aspect;
			while (cameraMovement.MoveNext())
			{
				_characterCamera.Camera.fieldOfView = Mathf.Lerp(fov_start, fov_end, (Time.time - time) * 0.75f);
				yield return null;
			}
			_characterCamera.Camera.fieldOfView = fov_end;
			_gameStats.Reset();
			_followingGuard.enabled = true;
			if (track.IsRunningOnTutorialTrack)
			{
				_followingGuard.ResetCatchUp();
			}
			if (track.IsRunningOnTutorialTrack)
			{
				isReadyForSlideinPowerups = false;
			}
			else
			{
				isReadyForSlideinPowerups = true;
			}
			SingletonMonobehavior<TimeScaleManager>.Instance.TimeScale = SingletonMonobehavior<TimeScaleManager>.Instance.TimeScaleInGameDefault;
			ChangeState(RunningState);
			yield return null;
		}

		private void TrackPrepare()
		{
			SpawnPointManager.Instance.Restart();
			track.Reset();
			track.LayTrackChunks(0f, false);
		}

		private void RaiseOnCharacterStateChanged(CharacterStateChangedEventArgs characterStateChangedEventArgs)
		{
			if (this.OnCharacterStateChanged != null)
			{
				this.OnCharacterStateChanged(this, characterStateChangedEventArgs);
			}
		}

		public void OnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName)
		{
			if (character.IsStumbling && _characterState != null)
			{
				StartCoroutine(StumbleDeathSequence());
			}
		}

		public void OnCriticalHit(Character.CriticalHitType type)
		{
			if (_characterState != null)
			{
				So.Instance.playSound(DieSound);
				_characterState.HandleCriticalHit();
			}
		}
	}
}
