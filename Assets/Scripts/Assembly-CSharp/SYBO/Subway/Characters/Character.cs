using System;
using System.Collections;
using System.Collections.Generic;
using SYBO.Core;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Managers;
using SYBO.Subway.Cameras;
using SYBO.Subway.Characters.States;
using SYBO.Subway.Pickups;
using SYBO.Subway.Utilities;
using UnityEngine;

namespace SYBO.Subway.Characters
{
	public class Character : MonoBehaviour
	{
		public enum ObstacleType
		{
			JumpHighBarrier = 0,
			JumpTrain = 1,
			RollBarrier = 2,
			JumpBarrier = 3,
			None = 4
		}

		public enum StumbleHorizontalHit
		{
			Left = 0,
			LeftCorner = 1,
			Center = 2,
			RightCorner = 3,
			Right = 4
		}

		public enum StumbleVerticalHit
		{
			Upper = 0,
			Middle = 1,
			Lower = 2
		}

		public enum StumbleType
		{
			Normal = 0,
			Bush = 1,
			Side = 2
		}

		public enum CriticalHitType
		{
			Train = 0,
			Barrier = 1,
			MovingTrain = 2,
			None = 3
		}

		public enum OnChangeTrackDirection
		{
			Left = 0,
			Right = 1
		}

		private struct SuperSneakersJump
		{
			public float z_start;

			public float z_length;

			public float z_end;

			public float y_start;
		}

		private enum ImpactX
		{
			Left = 0,
			Middle = 1,
			Right = 2
		}

		private enum ImpactY
		{
			Upper = 0,
			Middle = 1,
			Lower = 2
		}

		private enum ImpactZ
		{
			Before = 0,
			Middle = 1,
			After = 2
		}

		public delegate void OnStumbleDelegate(StumbleType stumbleType, StumbleHorizontalHit horizontalHit, StumbleVerticalHit verticalHit, string colliderName);

		public delegate void OnCriticalHitDelegate(CriticalHitType type);

		public delegate void OnReviveDelegate();

		public delegate void OnJumpDelegate();

		public delegate void OnDoubleJumpDelegate();

		public delegate void OnJumpIfHitByTrainDelegate();

		public delegate void OnLandingDelegate(Transform characterTransform);

		public delegate void OnHangtimeDelegate();

		public delegate void OnRollDelegate();

		public delegate void OnRollGuardDelegate();

		public delegate void OnHitByTrainDelegate();

		public delegate void OnChangeTrackDelegate(OnChangeTrackDirection direction);

		public delegate void OnTutorialMoveBackToCheckPointDelegate(float duration);

		public delegate void OnTutorialStartFromCheckPointDelegate();

		public delegate void OnPassedObstacleDelegate(ObstacleType type);

		public delegate void OnJumpOverTrainDelegate();

		public delegate void OnTeleboardStartDelegate(float duration, Vector3 offset, float gameSpeed);

		public delegate void OnGliderEndDelegate();

		public delegate void OnResetTeleportFXDelegate();

		private const float DEBUG_JUMP_HEIGHT = 100f;

		private static Character _instance;

		[SerializeField]
		private float _glideTime = 1f;

		[SerializeField]
		private AnimationCurve _glideVerticalSpeed = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

		[OptionalField]
		public Transform characterRoot;

		[HideInInspector]
		public CapsuleCollider characterTrigger;

		public CollisionNotifier characterTriggerNotifier;

		public CollisionNotifier coinMagnetCollider;

		public CollisionNotifier coinMagnetLongCollider;

		public float characterAngle = 45f;

		public ParticleSystem hoverboardCrashParticleSystem;

		public ParticleSystem mysteryPowerupParticleSystem;

		public ParticleSystem TurboHeadstartParticleSystem;

		public float ColliderTrackWidth = 17f;

		[OptionalField]
		public CharacterPickupParticles CharacterPickupParticleSystem;

		[HideInInspector]
		public CharacterController characterController;

		[HideInInspector]
		public CharacterModel characterModel;

		[HideInInspector]
		public CharacterCamera characterCamera;

		[HideInInspector]
		public Hoverboard hoverboard;

		[HideInInspector]
		public SuperSneakers superSneakers;

		[HideInInspector]
		public RunningState running;

		[HideInInspector]
		public bool immuneToCriticalHit;

		[HideInInspector]
		public float lastGroundedY;

		[HideInInspector]
		public float subwayMaxY;

		[HideInInspector]
		public bool stopColliding;

		[HideInInspector]
		public float jumpHeight;

		[HideInInspector]
		public float x;

		public float z;

		public AnimationCurve superSneakersJumpCurve;

		public float hitPoistionDistance;

		public float superSneakersJumpApexRatio = 0.5f;

		public float gliderGravity = 0.01f;

		public float gravity = 200f;

		public float jumpHeightNormal = 20f;

		public float jumpHeightSuperSneakers = 40f;

		public float verticalFallSpeedLimit = -1f;

		public float stumbleCornerTolerance = 15f;

		public float stumbleDecayTime = 5f;

		public bool inAirJump;

		public Variable<bool> IsGrounded = new Variable<bool>(false);

		public float verticalSpeed;

		public float sameLaneTimeStamp;

		private bool _isStumbling;

		private bool _isFalling;

		private bool _isJumping;

		private bool _isRolling;

		private bool _isInsideSubway;

		[HideInInspector]
		private int _trackIndex;

		private bool _isJumpingHigher;

		private bool _isTeleboard;

		private bool _isDoubleJumping;

		private bool _isGliderboard;

		private bool _isVTol;

		private VariableBool _squeezeCollider = new VariableBool();

		private float _lastZ;

		private int _killSwitchUpdates = 5;

		private Vector3 _characterControllerCenter;

		private float _characterControllerHeight;

		private Vector3 _characterColliderCenter;

		private float _characterColliderHeight;

		private int _initialTrackIndex = 1;

		private int _trackMovement;

		private int _trackMovementNext;

		private float _characterRotation;

		private int _trackIndexTarget;

		private float _trackIndexPosition;

		private bool _willSecondJump;

		private Game _game;

		private Track _track;

		private FollowingGuard _guard;

		private float _originaljumpHeightSuperSneakers;

		private float _glideTimeLeft;

		private bool _vTolEnded;

		private bool _jumpUpperCollision;

		private float _jumpUpperCollisionColliderMinY;

		private bool _jumpUpperCollisionEvaded = true;

		private bool _movedSinceLastFixedUpdate = true;

		private Vector3 _posFixedUpdateLast = Vector3.zero;

		private Vector3 _posFixedUpdatePrevious = Vector3.zero;

		private HashSet<Collider> subwayColliders = new HashSet<Collider>();

		private SuperSneakersJump? _superSneakersJump;

		private string _lastHitTag;

		private bool _startedJumpFromGround;

		private float _trainJumpSampleZ;

		private float _trainJumpSampleLength = 10f;

		private bool _trainJump;

		private float _verticalSpeed_jumpTolerance = -30f;

		private Layers _layers;

		private ObstacleType _lastObstacleTriggerType;

		private int _lastObstacleTriggerTrackIndex;

		private CharacterRendering _characterRendering;

		public static Character Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = UnityEngine.Object.FindObjectOfType(typeof(Character)) as Character;
				}
				return _instance;
			}
		}

		public bool IsStumbling
		{
			get
			{
				return _isStumbling;
			}
			set
			{
				_isStumbling = value;
			}
		}

		public bool IsFalling
		{
			get
			{
				return _isFalling;
			}
			set
			{
				_isFalling = value;
			}
		}

		public bool IsJumping
		{
			get
			{
				return _isJumping;
			}
			set
			{
				_isJumping = value;
			}
		}

		public bool IsRolling
		{
			get
			{
				return _isRolling;
			}
		}

		public bool IsInsideSubway
		{
			get
			{
				return _isInsideSubway;
			}
		}

		public int TrackIndex
		{
			get
			{
				return _trackIndex;
			}
		}

		public bool IsAboveGround
		{
			get
			{
				return base.transform.position.y > 20f;
			}
		}

		public bool IsJumpingHigher
		{
			get
			{
				return _isJumpingHigher || (hoverboard.isActive && Hoverboard.Instance.IsJumper);
			}
			set
			{
				_isJumpingHigher = value;
			}
		}

		public bool IsTeleboard
		{
			get
			{
				return _isTeleboard || (hoverboard.isActive && Hoverboard.Instance.IsTeleporter);
			}
			set
			{
				_isTeleboard = value;
			}
		}

		public bool IsDoubleJumping
		{
			get
			{
				return _isDoubleJumping || DoubleJump.doubleJumpEnabled || hoverboard.IsDoubleJump;
			}
			set
			{
				_isDoubleJumping = value;
			}
		}

		public bool IsGliderboard
		{
			get
			{
				return _isGliderboard || (hoverboard.isActive && Hoverboard.Instance.IsGlider);
			}
			set
			{
				_isGliderboard = value;
			}
		}

		public bool IsVTol
		{
			get
			{
				return _isVTol || (hoverboard.isActive && Hoverboard.Instance.IsVTol);
			}
			set
			{
				_isVTol = value;
			}
		}

		public VariableBool SqueezeCollider
		{
			get
			{
				return _squeezeCollider;
			}
		}

		public CharacterRendering CharacterRendering
		{
			get
			{
				return _characterRendering;
			}
		}

		public event OnStumbleDelegate OnStumble;

		public event OnCriticalHitDelegate OnCriticalHit;

		public event OnReviveDelegate OnRevive;

		public event OnJumpDelegate OnJump;

		public event OnDoubleJumpDelegate OnDoubleJump;

		public event OnJumpIfHitByTrainDelegate OnJumpIfHitByTrain;

		public event OnLandingDelegate OnLanding;

		public event OnHangtimeDelegate OnHangtime;

		public event OnRollDelegate OnRoll;

		public event OnRollGuardDelegate OnRollGuard;

		public event OnHitByTrainDelegate OnHitByTrain;

		public event OnChangeTrackDelegate OnChangeTrack;

		public event OnTutorialMoveBackToCheckPointDelegate OnTutorialMoveBackToCheckPoint;

		public event OnTutorialStartFromCheckPointDelegate OnTutorialStartFromCheckPoint;

		public event OnPassedObstacleDelegate OnPassedObstacle;

		public event OnJumpOverTrainDelegate OnJumpOverTrain;

		public event OnTeleboardStartDelegate OnTeleboardStart;

		public event OnGliderEndDelegate OnVTolEnd;

		public event OnResetTeleportFXDelegate OnResetTeleportFX;

		public void Initialize()
		{
			_instance = this;
			_characterRendering = GetComponent<CharacterRendering>();
			characterController = base.gameObject.GetComponent<CharacterController>();
			_layers = Layers.Instance;
			_game = Game.Instance;
			Variable<bool> isInGame = _game.IsInGame;
			isInGame.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isInGame.OnChange, (Variable<bool>.OnChangeDelegate)delegate(bool flag)
			{
				if (!flag)
				{
					StopAllCoroutines();
					immuneToCriticalHit = false;
					characterController.enabled = true;
					stopColliding = false;
				}
			});
			VariableBool squeezeCollider = _squeezeCollider;
			squeezeCollider.OnChange = (VariableBool.OnChangeDelegate)Delegate.Combine(squeezeCollider.OnChange, (VariableBool.OnChangeDelegate)delegate(bool squeeze)
			{
				if (squeeze)
				{
					characterController.height = 4f;
					characterController.center = new Vector3(0f, 2f, _characterControllerCenter.z);
					characterTrigger.height = 4f;
					characterTrigger.center = new Vector3(0f, 4f, _characterColliderCenter.z);
				}
				else
				{
					characterController.center = _characterControllerCenter;
					characterController.height = _characterControllerHeight;
					characterTrigger.center = _characterColliderCenter;
					characterTrigger.height = _characterColliderHeight;
				}
			});
			_track = Track.Instance;
			hoverboard = Hoverboard.Instance;
			running = RunningState.Instance;
			_characterRendering.Initialize();
			characterModel = GetComponentInChildren<CharacterModel>();
			characterModel.InitializeCharacter();
			characterRoot = characterModel.transform;
			superSneakers = this.FindObject<SuperSneakers>();
			characterCamera = CharacterCamera.Instance;
			_guard = FollowingGuard.Instance;
			CharacterPickupParticleSystem = GetComponentInChildren<CharacterPickupParticles>();
			characterTrigger = (CapsuleCollider)characterTriggerNotifier.Collider;
			characterTriggerNotifier.TriggerEnter += OnCharacterTriggerEnter;
			characterTriggerNotifier.TriggerExit += OnCharacterTriggerExit;
			_characterControllerCenter = characterController.center;
			_characterControllerHeight = characterController.height;
			_characterColliderCenter = characterTrigger.center;
			_characterColliderHeight = characterTrigger.height;
			this.OnLanding = (OnLandingDelegate)Delegate.Combine(this.OnLanding, new OnLandingDelegate(MissionsManager.Instance.OnChangeIsCharacterOnGround));
			_originaljumpHeightSuperSneakers = jumpHeightSuperSneakers;
		}

		public void RecreateCharacterModel()
		{
			UnityEngine.Object.Destroy(characterModel.gameObject);
			characterModel = _characterRendering.RecreateCharacterModel();
			characterRoot = characterModel.transform;
		}

		protected void Awake()
		{
			_instance = this;
		}

		protected void FixedUpdate()
		{
			if (_movedSinceLastFixedUpdate)
			{
				_posFixedUpdatePrevious = _posFixedUpdateLast;
				_posFixedUpdateLast = base.transform.position;
				_movedSinceLastFixedUpdate = false;
			}
		}

		protected void Update()
		{
			Vector3 position = base.transform.position;
			if (position.y < 0f)
			{
				position.y = 1f;
				base.transform.position = position;
			}
			if (_game.IsPaused || SingletonMonobehavior<TimeScaleManager>.Instance.TimeScale == 0f)
			{
				return;
			}
			if (z == _lastZ && _game.IsInGame.Value)
			{
				if (_killSwitchUpdates > 0)
				{
					_killSwitchUpdates--;
				}
				else if (!_game.IsPaused && !_game.isDead && z > 10f)
				{
					KillSwitchOn();
				}
			}
			else if (_killSwitchUpdates != 10)
			{
				_killSwitchUpdates = 10;
			}
			_lastZ = z;
		}

		public void Restart()
		{
			_trackIndex = _initialTrackIndex;
			_trackIndexTarget = _initialTrackIndex;
			x = _track.GetTrackX(_trackIndex);
			_trackIndexPosition = _trackIndex;
			z = 0f;
			_trackMovement = 0;
			_trackMovementNext = 0;
			_squeezeCollider.Clear();
			characterController.transform.position = _track.GetPosition(x, z) + Vector3.up * 5f;
			characterController.Move(-5f * Vector3.up);
			verticalSpeed = 0f;
			_superSneakersJump = null;
			jumpHeight = jumpHeightNormal;
			inAirJump = false;
			_isJumping = false;
			_isRolling = false;
			lastGroundedY = 0f;
			_guard.Restart(true);
			StartStumble();
			_startedJumpFromGround = false;
			sameLaneTimeStamp = Time.time;
			subwayColliders.Clear();
			_isInsideSubway = false;
			_jumpUpperCollision = false;
			_jumpUpperCollisionEvaded = true;
		}

		public void Revive()
		{
			_characterRendering.ResetTeleportFx();
			if (this.OnRevive != null)
			{
				this.OnRevive();
			}
			if (_isStumbling)
			{
				StopStumble();
			}
			_isJumping = true;
			_isFalling = false;
			verticalSpeed = CalculateJumpVerticalSpeed(15f);
			_superSneakersJump = null;
			if (_jumpUpperCollision)
			{
				float num = characterController.bounds.min.y - _jumpUpperCollisionColliderMinY;
				if (num < 0f)
				{
					characterController.Move(num * Vector3.up);
				}
			}
			_jumpUpperCollision = false;
			_jumpUpperCollisionEvaded = true;
		}

		public void ChangeTrack(int movement, float duration)
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.StayInOneLane, (int)(Time.time - sameLaneTimeStamp));
			MissionsManager.Instance.RemoveProgressForThis(MissionTarget.StayInOneLane);
			sameLaneTimeStamp = Time.time;
			if (_trackMovement != movement)
			{
				ForceChangeTrack(movement, duration);
				if (IsTeleboard && !_game.IsInPogostickMode && !_game.IsInJetpackMode)
				{
					switch (movement)
					{
					case 1:
						Teleboard(new Vector3(20f, 0f, 1f));
						break;
					case -1:
						Teleboard(new Vector3(-20f, 0f, 1f));
						break;
					}
				}
			}
			else
			{
				_trackMovementNext = movement;
			}
		}

		public void ForceChangeTrack(int movement, float duration)
		{
			StopAllCoroutines();
			StartCoroutine(ChangeTrackCoroutine(movement, duration));
		}

		public void SetBackToCheckPoint(float zoomTime)
		{
			float lastCheckPoint = _track.GetLastCheckPoint(z);
			_trackIndex = _initialTrackIndex;
			_trackIndexTarget = _initialTrackIndex;
			float trackX = _track.GetTrackX(_trackIndex);
			_trackIndexPosition = _trackIndex;
			_trackMovement = 0;
			_trackMovementNext = 0;
			StartCoroutine(MoveCharacterToPosition(trackX, lastCheckPoint, zoomTime));
		}

		public void ForceLeaveSubway()
		{
			subwayColliders.Clear();
			_isInsideSubway = false;
		}

		public float GetTrackX()
		{
			return _track.GetPosition(_track.GetTrackX(_trackIndex), 0f).x;
		}

		public void DebugSuperJump()
		{
			float num = jumpHeight;
			jumpHeight = 100f;
			Jump();
			jumpHeight = num;
		}

		public void Jump()
		{
			bool flag = !_isJumping && verticalSpeed <= 0f && verticalSpeed > _verticalSpeed_jumpTolerance;
			if (characterController.isGrounded || (flag && !running.transitionFromHeight))
			{
				if (_isJumping && _jumpUpperCollisionEvaded)
				{
					_jumpUpperCollision = false;
				}
				if (_isRolling)
				{
					ExpandColliders();
				}
				_glideTimeLeft = 0f;
				_vTolEnded = false;
				_willSecondJump = true;
				_isJumping = true;
				_isFalling = false;
				IsGrounded.Value = false;
				NotifyOnJump();
				if (IsJumpingHigher)
				{
					Vector3 position = base.transform.position;
					jumpHeightSuperSneakers = _originaljumpHeightSuperSneakers;
					SuperSneakersJump value = default(SuperSneakersJump);
					value.z_start = position.z;
					value.z_length = JumpLength(_game.currentSpeed, jumpHeightSuperSneakers) * superSneakersJumpApexRatio;
					value.z_end = value.z_start + value.z_length;
					value.y_start = position.y;
					_superSneakersJump = value;
					verticalSpeed = 0f;
				}
				else
				{
					verticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
				}
				if (IsRunningOnGround())
				{
					_startedJumpFromGround = true;
					_trainJump = false;
					_trainJumpSampleZ = z + _trainJumpSampleLength;
				}
			}
			else if (AllowDoubleJump())
			{
				if (_isJumping && _jumpUpperCollisionEvaded)
				{
					_jumpUpperCollision = false;
				}
				if (_isRolling)
				{
					ExpandColliders();
				}
				_glideTimeLeft = 0f;
				_vTolEnded = false;
				_willSecondJump = false;
				_isJumping = true;
				_isFalling = false;
				IsGrounded.Value = false;
				NotifyOnDoubleJump();
				if (IsJumpingHigher)
				{
					Vector3 position2 = base.transform.position;
					jumpHeightSuperSneakers = 18f;
					SuperSneakersJump value2 = default(SuperSneakersJump);
					value2.z_start = position2.z;
					value2.z_length = JumpLength(_game.currentSpeed, jumpHeightSuperSneakers) * superSneakersJumpApexRatio;
					value2.z_end = value2.z_start + value2.z_length;
					value2.y_start = position2.y;
					_superSneakersJump = value2;
					verticalSpeed = 0f;
				}
				else
				{
					verticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
				}
				if (IsRunningOnGround())
				{
					_startedJumpFromGround = true;
					_trainJump = false;
					_trainJumpSampleZ = z + _trainJumpSampleLength;
				}
			}
			else if (verticalSpeed < 0f)
			{
				inAirJump = true;
			}
		}

		public void CheckInAirJump()
		{
			if (characterController.isGrounded && inAirJump)
			{
				Jump();
				inAirJump = false;
			}
		}

		private void Teleboard(Vector3 offset)
		{
			float duration = 17f / _game.currentSpeed;
			if (this.OnTeleboardStart != null)
			{
				this.OnTeleboardStart(duration, offset, _game.currentSpeed);
			}
		}

		public void Roll()
		{
			if (!_isRolling)
			{
				_glideTimeLeft = _glideTime;
				SuperSneakersJump? superSneakersJump = _superSneakersJump;
				if (superSneakersJump.HasValue)
				{
					_superSneakersJump = null;
				}
				_squeezeCollider.Add(this);
				if (!running.transitionFromHeight)
				{
					verticalSpeed = 0f - CalculateJumpVerticalSpeed(jumpHeight);
				}
				else
				{
					verticalSpeed = -250f;
				}
				_isRolling = true;
				inAirJump = false;
				NotifyOnRoll();
			}
		}

		public void ApplyGravity()
		{
			if (verticalSpeed < 0f && characterController.isGrounded)
			{
				if (_startedJumpFromGround && _trainJump && IsRunningOnGround())
				{
					NotifyOnJumpOverTrain();
				}
				if (running.currentRunPosition != RunningState.RunPositions.air)
				{
					_startedJumpFromGround = false;
				}
				verticalSpeed = 0f;
				IsGrounded.Value = true;
				if (_isJumping || _isFalling)
				{
					_jumpUpperCollision = false;
					_jumpUpperCollisionEvaded = true;
					_isJumping = false;
					_isFalling = false;
					IsGrounded.Value = true;
					NotifyOnLanding();
				}
			}
			else if (_startedJumpFromGround && _trainJumpSampleZ < z)
			{
				RaycastHit hitInfo;
				if (Physics.Raycast(new Ray(base.transform.position, -Vector3.up), out hitInfo) && (hitInfo.collider.CompareTag("HitMovingTrain") || hitInfo.collider.CompareTag("HitTrain")))
				{
					_trainJump = true;
				}
				_trainJumpSampleZ += _trainJumpSampleLength;
			}
			if (IsFalling && IsGliderboard)
			{
				if (_glideTimeLeft < _glideTime)
				{
					verticalSpeed = gliderGravity * _glideVerticalSpeed.Evaluate(_glideTimeLeft / _glideTime);
					_glideTimeLeft += Time.deltaTime;
				}
				else
				{
					verticalSpeed -= gliderGravity * Time.deltaTime;
				}
			}
			else if (IsFalling && IsVTol)
			{
				if (_glideTimeLeft < _glideTime)
				{
					verticalSpeed = 0f;
					_glideTimeLeft += Time.deltaTime;
				}
				else
				{
					if (!_vTolEnded)
					{
						if (this.OnVTolEnd != null)
						{
							this.OnVTolEnd();
						}
						_vTolEnded = true;
					}
					verticalSpeed -= gravity * Time.deltaTime;
				}
			}
			else
			{
				verticalSpeed -= gravity * Time.deltaTime;
			}
			if (!characterController.isGrounded && !_isFalling && verticalSpeed < verticalFallSpeedLimit && !_isRolling)
			{
				_isFalling = true;
				NotifyOnHangtime();
				IsGrounded.Value = false;
			}
		}

		public void MoveWithGravity()
		{
			if (characterController.enabled)
			{
				verticalSpeed -= gravity * Time.deltaTime;
				if (verticalSpeed > 0f)
				{
					verticalSpeed = 0f;
				}
				Vector3 motion = verticalSpeed * Time.deltaTime * Vector3.up;
				characterController.Move(motion);
			}
		}

		public void MoveForward()
		{
			Vector3 position = base.transform.position;
			float num = z + _game.currentSpeed * Time.deltaTime;
			Vector3 vector = Vector3.zero;
			bool flag = true;
			if (_isJumping && _jumpUpperCollision)
			{
				flag = false;
				if (!_jumpUpperCollisionEvaded && _jumpUpperCollisionColliderMinY < characterTrigger.bounds.max.y)
				{
					if (0f <= verticalSpeed)
					{
						vector = (0f - gravity) * Time.deltaTime * Time.deltaTime * Vector3.up;
					}
				}
				else
				{
					vector = Vector3.zero;
					_jumpUpperCollisionEvaded = true;
				}
			}
			if (verticalSpeed < 0f || flag)
			{
				vector = verticalSpeed * Time.deltaTime * Vector3.up;
			}
			if (_isJumping && _superSneakersJump.HasValue)
			{
				SuperSneakersJump value = _superSneakersJump.Value;
				if (z < value.z_end)
				{
					if (flag)
					{
						float num2 = superSneakersJumpCurve.Evaluate((num - value.z_start) / value.z_length) * jumpHeightSuperSneakers + value.y_start;
						float num3 = num2 - position.y;
						vector = Vector3.up * num3;
					}
					else
					{
						vector = Vector3.zero;
					}
				}
				else
				{
					_superSneakersJump = null;
					verticalSpeed = 0f;
					vector = Vector3.zero;
				}
			}
			Vector3 position2 = _track.GetPosition(x, num);
			Vector3 vector2 = new Vector3(position.x, 0f, position.z);
			Vector3 vector3 = position2 - vector2;
			if (characterController.enabled)
			{
				characterController.Move(vector + vector3);
			}
			else
			{
				characterController.transform.position = characterController.transform.position + vector3;
			}
			z = base.transform.position.z;
			if (characterController.isGrounded)
			{
				lastGroundedY = position.y;
			}
			_movedSinceLastFixedUpdate = true;
		}

		public void EndRoll()
		{
			ExpandColliders();
			_isRolling = false;
		}

		public float CalculateJumpVerticalSpeed(float jumpHeight)
		{
			return Mathf.Sqrt(2f * jumpHeight * gravity);
		}

		public float CalculateJumpVerticalSpeed()
		{
			return CalculateJumpVerticalSpeed(jumpHeight);
		}

		public float JumpLength(float speed, float jumpHeight)
		{
			return speed * 2f * CalculateJumpVerticalSpeed(jumpHeight) / gravity;
		}

		public void StopStumble()
		{
			_guard.ResetCatchUp();
			_isStumbling = false;
		}

		public void TimeOut()
		{
			NotifyCriticalHit();
		}

		private IEnumerator ChangeTrackCoroutine(int move, float duration)
		{
			_trackMovement = move;
			_trackMovementNext = 0;
			int newTrackIndex = _trackIndexTarget + move;
			float trackChangeIndexDistance = Mathf.Abs((float)newTrackIndex - _trackIndexPosition);
			float trackIndexPositionBegin = _trackIndexPosition;
			float startX = x;
			float endX = _track.GetTrackX(newTrackIndex);
			float dir = Mathf.Sign(newTrackIndex - _trackIndexTarget);
			float startRotation = _characterRotation;
			if (newTrackIndex < 0)
			{
				HandleStumble(StumbleType.Side, StumbleHorizontalHit.Left, StumbleVerticalHit.Middle, "side");
				yield break;
			}
			if (newTrackIndex >= _track.numberOfTracks)
			{
				HandleStumble(StumbleType.Side, StumbleHorizontalHit.Right, StumbleVerticalHit.Middle, "side");
				yield break;
			}
			if (this.OnChangeTrack != null)
			{
				this.OnChangeTrack((move >= 0) ? OnChangeTrackDirection.Right : OnChangeTrackDirection.Left);
			}
			_trackIndexTarget = newTrackIndex;
			yield return StartCoroutine(pTween.To(trackChangeIndexDistance * duration, delegate(float t)
			{
				_trackIndexPosition = Mathf.Lerp(trackIndexPositionBegin, newTrackIndex, t);
				x = Mathf.Lerp(startX, endX, t);
				_characterRotation = pMath.Bell(t) * dir * characterAngle + Mathf.Lerp(startRotation, 0f, t);
				characterRoot.localRotation = Quaternion.Euler(0f, _characterRotation, 0f);
			}));
			_trackIndex = newTrackIndex;
			_trackMovement = 0;
			if (_trackMovementNext != 0)
			{
				StartCoroutine(ChangeTrackCoroutine(_trackMovementNext, duration));
			}
		}

		private IEnumerator MoveCharacterToPosition(float newX, float newZ, float time)
		{
			float oldX = x;
			float oldZ = z;
			_game.ChangeState(null);
			immuneToCriticalHit = true;
			stopColliding = true;
			characterController.enabled = false;
			NotifyOnTutorialMoveBackToCheckPoint(time);
			yield return StartCoroutine(pTween.To(time, delegate(float t)
			{
				x = Mathf.SmoothStep(oldX, newX, t);
				z = Mathf.SmoothStep(oldZ, newZ, t);
			}));
			immuneToCriticalHit = false;
			characterController.enabled = true;
			NotifyOnTutorialStartFromCheckPoint();
			stopColliding = false;
			_game.ChangeState(_game.RunningState);
		}

		private ObstacleType ObstacleTagToType(string tag)
		{
			switch (tag)
			{
			case "JumpTrain":
				return ObstacleType.JumpTrain;
			case "RollBarrier":
				return ObstacleType.RollBarrier;
			case "JumpBarrier":
				return ObstacleType.JumpBarrier;
			case "JumpHighBarrier":
				return ObstacleType.JumpHighBarrier;
			default:
				return ObstacleType.None;
			}
		}

		private void OnCharacterTriggerExit(Collider collider)
		{
			if (collider.CompareTag("Subway"))
			{
				if (subwayColliders.Contains(collider))
				{
					subwayColliders.Remove(collider);
					_isInsideSubway = subwayColliders.Count > 0;
				}
				return;
			}
			ObstacleType obstacleType = ObstacleTagToType(collider.tag);
			if (obstacleType == _lastObstacleTriggerType && _lastObstacleTriggerTrackIndex == _trackIndex && this.OnPassedObstacle != null)
			{
				this.OnPassedObstacle(obstacleType);
			}
			if (obstacleType == ObstacleType.RollBarrier && this.OnRollGuard != null)
			{
				this.OnRollGuard();
			}
		}

		private void OnCharacterTriggerEnter(Collider collider)
		{
			hitPoistionDistance = base.transform.position.z;
			if (!_game.IsInGame.Value)
			{
				return;
			}
			if (collider.CompareTag("Subway"))
			{
				subwayColliders.Add(collider);
				_isInsideSubway = subwayColliders.Count > 0;
				subwayMaxY = collider.bounds.max.y - 3f;
			}
			else
			{
				if (stopColliding || collider.gameObject.layer == _layers.KeepOnHoverboard)
				{
					return;
				}
				if (collider.gameObject.layer == Layers.Instance.Pickup)
				{
					Pickup componentInChildren = collider.GetComponentInChildren<Pickup>();
					PickupEvent component = collider.GetComponent<PickupEvent>();
					if (componentInChildren != null)
					{
						float num = 70f;
						if ((base.transform.position.y >= num && componentInChildren.transform.position.y >= num) || (base.transform.position.y < num && componentInChildren.transform.position.y < num))
						{
							NotifyPickup(componentInChildren, null);
							return;
						}
					}
					if (component != null)
					{
						float num2 = 70f;
						if ((base.transform.position.y >= num2 && component.transform.position.y >= num2) || (base.transform.position.y < num2 && component.transform.position.y < num2))
						{
							NotifyPickup(null, component);
							return;
						}
					}
					PickupSpawnPoint2 component2 = collider.GetComponent<PickupSpawnPoint2>();
					if (component2 != null && component2.CanTriggerCollisionWithCharacter)
					{
						component2.TriggerCollisionWithCharacter(CharacterPickupParticleSystem);
					}
				}
				if (collider.gameObject.layer == _layers.Default)
				{
					if (collider.isTrigger && characterController.isGrounded)
					{
						IsGrounded.Value = true;
					}
					if (collider.isTrigger)
					{
						ObstacleType obstacleType = ObstacleTagToType(collider.tag);
						if (obstacleType != ObstacleType.None)
						{
							_lastObstacleTriggerType = obstacleType;
							_lastObstacleTriggerTrackIndex = _trackIndex;
						}
					}
				}
				else if (collider.isTrigger)
				{
					if (collider.gameObject.layer == _layers.Hit)
					{
						if (collider.CompareTag("BushStumble"))
						{
							HandleStumble(StumbleType.Bush, StumbleHorizontalHit.Center, StumbleVerticalHit.Lower, collider.name);
						}
						else
						{
							HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Middle, collider.name);
						}
					}
				}
				else
				{
					if (DebugSettings.IsAllowed && DebugSettings.CharacterInvincible)
					{
						return;
					}
					_lastHitTag = collider.tag;
					ImpactX obstacleImpactX = GetObstacleImpactX(collider);
					ImpactY characterImpactY = GetCharacterImpactY(collider);
					ImpactZ obstacleImpactZ = GetObstacleImpactZ(collider);
					float num3 = (collider.bounds.min.x + collider.bounds.max.x) / 2f;
					float num4 = base.transform.position.x;
					int num5 = ((num4 < num3) ? 1 : ((num4 > num3) ? (-1) : 0));
					bool flag = num5 == 0 || _trackMovement == num5;
					bool flag2 = characterTrigger.bounds.center.z < collider.bounds.min.z;
					bool flag3 = obstacleImpactZ == ImpactZ.Before && !flag2 && flag;
					bool jumpUpperCollision = _jumpUpperCollision;
					if (_isJumping && characterImpactY == ImpactY.Upper && !characterController.isGrounded)
					{
						_jumpUpperCollision = true;
						_jumpUpperCollisionEvaded = false;
						verticalSpeed = 0f;
						if (!jumpUpperCollision)
						{
							_jumpUpperCollisionColliderMinY = collider.bounds.min.y;
						}
					}
					if (obstacleImpactZ == ImpactZ.Middle || flag3)
					{
						if (_trackMovement != 0)
						{
							float duration = 0.5f;
							if (_track.IsRunningOnTutorialTrack)
							{
								duration = 0.2f;
							}
							ChangeTrack(-_trackMovement, duration);
						}
						switch (obstacleImpactX)
						{
						case ImpactX.Left:
							HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Left, StumbleVerticalHit.Middle, collider.name);
							break;
						case ImpactX.Right:
							HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Right, StumbleVerticalHit.Middle, collider.name);
							break;
						}
						return;
					}
					if (obstacleImpactX == ImpactX.Middle || _trackMovement == 0)
					{
						if (obstacleImpactZ != ImpactZ.Before)
						{
							return;
						}
						if (characterImpactY == ImpactY.Lower)
						{
							if (!IsGliderboard)
							{
								verticalSpeed = CalculateJumpVerticalSpeed(8f);
							}
							else
							{
								verticalSpeed = CalculateJumpVerticalSpeed(1.5f);
							}
							if (!IsTeleboard)
							{
								HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Lower, collider.name);
							}
							else
							{
								this.OnResetTeleportFX();
							}
							return;
						}
						if (collider.gameObject.CompareTag("HitMovingTrain"))
						{
							HitByTrainSequence();
							NotifyCriticalHit();
							return;
						}
						if (characterImpactY == ImpactY.Middle)
						{
							if (!IsTeleboard)
							{
								HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Middle, collider.name);
							}
							else
							{
								this.OnResetTeleportFX();
							}
							NotifyCriticalHit();
							return;
						}
						bool flag4 = jumpUpperCollision && _jumpUpperCollisionColliderMinY <= collider.bounds.min.y;
						bool flag5 = EnteredColliderFromBelow(collider, jumpUpperCollision);
						if (_isJumping && ((_jumpUpperCollision && flag5) || flag4))
						{
							Debug.Log(string.Concat("Ignore ceiling penetration ", characterTrigger.bounds.max.y - collider.bounds.min.y, " when jumping charMaxY ", characterTrigger.bounds.max.y, "  x ", obstacleImpactX, " y ", characterImpactY, " z ", obstacleImpactZ));
						}
						else
						{
							if (!IsTeleboard)
							{
								HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Upper, collider.name);
							}
							else
							{
								this.OnResetTeleportFX();
							}
							NotifyCriticalHit();
						}
						return;
					}
					if (obstacleImpactZ == ImpactZ.Before && flag)
					{
						if (collider.gameObject.CompareTag("HitMovingTrain"))
						{
							HitByTrainSequence();
							NotifyCriticalHit();
						}
						else if (collider.gameObject.layer == _layers.HitBounceOnly)
						{
							if (!IsTeleboard)
							{
								HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Lower, collider.name);
							}
						}
						else
						{
							ForceChangeTrack(-_trackMovement, 0.5f);
						}
					}
					else if (collider.gameObject.layer == _layers.HitBounceOnly)
					{
						ForceChangeTrack(-_trackMovement, 0.5f);
					}
					switch (obstacleImpactX)
					{
					case ImpactX.Left:
						if (!IsTeleboard)
						{
							HandleStumble(StumbleType.Normal, StumbleHorizontalHit.LeftCorner, StumbleVerticalHit.Middle, collider.name);
						}
						break;
					case ImpactX.Right:
						if (!IsTeleboard)
						{
							HandleStumble(StumbleType.Normal, StumbleHorizontalHit.RightCorner, StumbleVerticalHit.Middle, collider.name);
						}
						break;
					}
				}
			}
		}

		private bool EnteredColliderFromBelow(Collider collider, bool jumpUpperCollisionRepeated)
		{
			Bounds bounds = characterTrigger.bounds;
			Vector3 vector = bounds.max - 0.5f * new Vector3(bounds.size.x, 0f, bounds.size.z);
			Vector3 normalized = (collider.bounds.min - vector).normalized;
			Vector3 lhs = Vector3.Cross(normalized, Vector3.left);
			Vector3 vector2 = _posFixedUpdateLast - _posFixedUpdatePrevious;
			float num = Vector3.Dot(lhs, -vector2);
			return num < 0f;
		}

		private void HitByTrainSequence()
		{
			if (hoverboard.isActive)
			{
				NotifyOnJumpIfHitByTrain();
			}
			else
			{
				NotifyOnHitByTrain();
			}
		}

		private ImpactX GetObstacleImpactX(Collider collider)
		{
			Bounds bounds = characterTrigger.bounds;
			Bounds bounds2 = collider.bounds;
			float num = Mathf.Max(bounds.min.x, bounds2.min.x);
			float num2 = Mathf.Min(bounds.max.x, bounds2.max.x);
			float num3 = (num + num2) * 0.5f;
			float num4 = num3 - bounds2.min.x;
			if ((double)num4 > (double)bounds2.size.x - (double)ColliderTrackWidth * 0.33)
			{
				return ImpactX.Right;
			}
			if ((double)num4 < (double)ColliderTrackWidth * 0.33)
			{
				return ImpactX.Left;
			}
			return ImpactX.Middle;
		}

		private ImpactZ GetObstacleImpactZ(Collider collider)
		{
			Vector3 position = base.transform.position;
			Bounds bounds = collider.bounds;
			if (position.z > bounds.max.z - ((!(bounds.max.z - bounds.min.z > 30f)) ? ((bounds.max.z - bounds.min.z) * 0.5f) : stumbleCornerTolerance))
			{
				return ImpactZ.After;
			}
			if (position.z < bounds.min.z + stumbleCornerTolerance)
			{
				return ImpactZ.Before;
			}
			return ImpactZ.Middle;
		}

		private ImpactY GetCharacterImpactY(Collider collider)
		{
			Bounds bounds = characterTrigger.bounds;
			Bounds bounds2 = collider.bounds;
			float num = Mathf.Max(bounds.min.y, bounds2.min.y);
			float num2 = Mathf.Min(bounds.max.y, bounds2.max.y);
			float num3 = (num + num2) * 0.5f;
			float num4 = (num3 - bounds.min.y) / bounds.size.y;
			if (num4 < 0.33f)
			{
				return ImpactY.Lower;
			}
			if (num4 < 0.66f)
			{
				return ImpactY.Middle;
			}
			return ImpactY.Upper;
		}

		private void KillSwitchOn()
		{
			_game.Die();
		}

		private bool AllowDoubleJump()
		{
			if (IsDoubleJumping && _willSecondJump)
			{
				return true;
			}
			return false;
		}

		private bool IsRunningFromTrain()
		{
			return running.currentRunPosition == RunningState.RunPositions.train || running.currentRunPosition == RunningState.RunPositions.movingTrain;
		}

		private bool IsRunningOnGround()
		{
			return running.currentRunPosition == RunningState.RunPositions.ground;
		}

		private void ExpandColliders()
		{
			if (characterController.enabled)
			{
				characterController.Move(Vector3.up * 2f);
			}
			_squeezeCollider.Remove(this);
			if (characterController.enabled)
			{
				characterController.Move(Vector3.down * 2f);
			}
		}

		private void StartStumble()
		{
			_isStumbling = true;
			if (!_track.IsRunningOnTutorialTrack)
			{
				_guard.CatchUp();
			}
			_guard.StartCoroutine(StumbleDecay());
		}

		private IEnumerator StumbleDecay()
		{
			yield return new WaitForSeconds(stumbleDecayTime);
			StopStumble();
		}

		private void HandleStumble(StumbleType stumbleType, StumbleHorizontalHit horizontalHit, StumbleVerticalHit verticalHit, string colliderName)
		{
			if (!_game.IsInJetpackMode && !_game.IsInPogostickMode)
			{
				NotifyOnStumble(stumbleType, horizontalHit, verticalHit, colliderName);
				StartStumble();
			}
		}

		private void NotifyCriticalHit()
		{
			if (this.OnCriticalHit != null)
			{
				CriticalHitType type;
				switch (_lastHitTag)
				{
				case "HitTrain":
					type = CriticalHitType.Train;
					break;
				case "HitBarrier":
					type = CriticalHitType.Barrier;
					break;
				case "HitMovingTrain":
					type = CriticalHitType.MovingTrain;
					break;
				default:
					type = CriticalHitType.None;
					break;
				}
				this.OnCriticalHit(type);
			}
		}

		public void NotifyPickup(Pickup pickup, PickupEvent pickupEvent)
		{
			if (pickup != null)
			{
				pickup.NotifyPickup(CharacterPickupParticleSystem);
			}
			if (pickupEvent != null)
			{
				pickupEvent.NotifyPickup(CharacterPickupParticleSystem);
			}
		}

		private void NotifyOnStumble(StumbleType stumbleType, StumbleHorizontalHit horizontalHit, StumbleVerticalHit verticalHit, string colliderName)
		{
			if (this.OnStumble != null)
			{
				this.OnStumble(stumbleType, horizontalHit, verticalHit, colliderName);
			}
		}

		private void NotifyOnCriticalHit(CriticalHitType type)
		{
			if (this.OnCriticalHit != null)
			{
				this.OnCriticalHit(type);
			}
		}

		private void NotifyOnJump()
		{
			if (this.OnJump != null)
			{
				this.OnJump();
			}
		}

		private void NotifyOnDoubleJump()
		{
			if (this.OnDoubleJump != null)
			{
				this.OnDoubleJump();
			}
		}

		private void NotifyOnJumpIfHitByTrain()
		{
			if (this.OnJumpIfHitByTrain != null)
			{
				this.OnJumpIfHitByTrain();
			}
		}

		private void NotifyOnLanding()
		{
			if (this.OnLanding != null)
			{
				this.OnLanding(base.transform);
			}
		}

		private void NotifyOnHangtime()
		{
			if (this.OnHangtime != null)
			{
				this.OnHangtime();
			}
		}

		private void NotifyOnRoll()
		{
			if (this.OnRoll != null)
			{
				this.OnRoll();
			}
		}

		private void NotifyOnHitByTrain()
		{
			if (this.OnHitByTrain != null)
			{
				this.OnHitByTrain();
			}
		}

		private void NotifyOnChangeTrack(OnChangeTrackDirection direction)
		{
			if (this.OnChangeTrack != null)
			{
				this.OnChangeTrack(direction);
			}
		}

		private void NotifyOnTutorialMoveBackToCheckPoint(float duration)
		{
			if (this.OnTutorialMoveBackToCheckPoint != null)
			{
				this.OnTutorialMoveBackToCheckPoint(duration);
			}
		}

		private void NotifyOnTutorialStartFromCheckPoint()
		{
			if (this.OnTutorialStartFromCheckPoint != null)
			{
				this.OnTutorialStartFromCheckPoint();
			}
		}

		private void NotifyOnJumpOverTrain()
		{
			if (this.OnJumpOverTrain != null)
			{
				this.OnJumpOverTrain();
			}
		}
	}
}
