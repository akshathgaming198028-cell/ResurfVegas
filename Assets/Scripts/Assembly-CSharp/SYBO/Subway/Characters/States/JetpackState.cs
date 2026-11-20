using System;
using System.Collections;
using System.Collections.Generic;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities;
using SYBO.Subway.Cameras;
using SYBO.Subway.Coins;
using SYBO.Subway.Pickups;
using SYBO.Subway.Pickups.Migration;
using UnityEngine;

namespace SYBO.Subway.Characters.States
{
	public class JetpackState : CharacterState
	{
		[Serializable]
		public class FlyAheadInfo
		{
			public AnimationCurve cameraMovement;
		}

		public delegate void OnStartDelegate(bool isHeadStart);

		public delegate void OnStopDelegate();

		public delegate void OnFlyAheadStartDelegate();

		public delegate void OnFlyAheadUpdateDelegate(float ratio);

		public delegate void OnActivateTurboHeadstartDelegate();

		public delegate void OnDeactivateTurboHeadstartDelegate();

		public delegate void OnHidTurboHeadstartButtonsDelegate();

		public const int TURBO_HEADSTART_MAX_GEARS = 3;

		private const float _coinStayInTrackDistance = 60f;

		private const float _coinSpacingInZ = 30f;

		[SerializeField]
		private float megaHeadstartFOV = 10f;

		[SerializeField]
		private float trasitionTime = 1f;

		[SerializeField]
		private GameObject _endOfFlightPickupPrefab;

		public OnStartDelegate OnStart;

		public OnStopDelegate OnStop;

		public OnFlyAheadStartDelegate OnFlyAheadStart;

		public OnFlyAheadUpdateDelegate OnFlyAheadUpdate;

		public AnimationCurve fisso;

		public bool isActive;

		public AudioClipInfo powerDownSound;

		public Vector3 cameraOffset = new Vector3(0f, 33f, -33f);

		public float cameraOffsetSmoothDuration = 1f;

		public float smoothCameraXDuration = 0.05f;

		public float cameraAimOffset = 20f;

		public float cameraFOV = 60f;

		public float ySmoothDuration = 0.5f;

		public float speedup = 2f;

		public float flyHeight = 95f;

		public float coinOffset = 200f;

		public float flyAheadDuration = 1.5f;

		public float stopBeforeLandingChunkDistance = 50f;

		public float stopBeforeLandingChunkDistance1 = 50f;

		public float stopBeforeLandingChunkDistance2 = 50f;

		public float characterAngle = 45f;

		public float characterChangeTrackLength = 60f;

		private bool _isHeadStart;

		public FlyAheadInfo flyAhead;

		private float landingZ;

		private float landingTime;

		[SerializeField]
		private PowerupType _powerupType;

		private float _powerupTimeLeftMax;

		private int _dynamicCoinsSpawned;

		[SerializeField]
		private ActivePowerup Powerup;

		private float _startTime;

		private bool _canActivateLongHeadstart;

		private float _flyingDuration;

		private float _flyAheadDistance;

		private float _jetpackDurationExtended;

		private float _jetpackSpeed;

		private int _turboHeadstart;

		private float _currentHeadstartTime;

		private float _headStartDistance;

		private float _headStartSpeed;

		private float _headStartTopSpeed;

		private float _newDuration;

		private float _newSpeed;

		private bool _activateTurboHeadstart;

		private bool _deactivateTurboHeadstart;

		private float _newflyHeight;

		private float _turboHeadstart1FlyHeight = 105f;

		private float _turboHeadstart2FlyHeight = 120f;

		private ParticleSystem _ceilingBrickExpolsion;

		private GameObject[] _endOfFlightPickups;

		private GameObject[] _pickupSpawnPointPrefabInstances;

		private float headstartLongStartPosition;

		private float changeHeightSpeed = 50f;

		private float startPosition;

		private List<PickupDefault[]> _pickupLists = new List<PickupDefault[]>();

		private Game _game;

		private Track _track;

		private Character _character;

		private CharacterController _characterController;

		private Transform _characterTransform;

		private CharacterCamera _characterCamera;

		private Transform _characterCameraTransform;

		private Animation _characterAnimation;

		private InAirCoinsManager _inAirCoinsManager;

		private Vector3 _jetpackParticlesInitialRotation;

		private Vector3 _jetpackParticlesInitialScale;

		private static JetpackState instance;

		public bool CanActivateLongHeadstart
		{
			get
			{
				return _canActivateLongHeadstart;
			}
		}

		public bool IsHeadStart
		{
			get
			{
				return _isHeadStart;
			}
			set
			{
				_isHeadStart = value;
			}
		}

		public int TurboHeadstart
		{
			get
			{
				return _turboHeadstart;
			}
		}

		public float LandingZ
		{
			get
			{
				return landingZ;
			}
		}

		public float LandingTime
		{
			get
			{
				return landingTime;
			}
		}

		public override bool PauseActiveModifiers
		{
			get
			{
				return true;
			}
		}

		public PowerupType PowerupType
		{
			get
			{
				return _powerupType;
			}
			set
			{
				_powerupType = value;
			}
		}

		public float PowerupTimeLeftMax
		{
			get
			{
				return _powerupTimeLeftMax;
			}
		}

		public InAirCoinsManager InAirCoinsManager
		{
			get
			{
				return _inAirCoinsManager;
			}
		}

		public static JetpackState Instance
		{
			get
			{
				return instance ?? (instance = UnityEngine.Object.FindObjectOfType(typeof(JetpackState)) as JetpackState);
			}
		}

		public event OnActivateTurboHeadstartDelegate OnActivateTurboHeadstart;

		public event OnDeactivateTurboHeadstartDelegate OnDeactivateTurboHeadstart;

		public event OnHidTurboHeadstartButtonsDelegate OnHidTurboHeadstartButtons;

		protected void Awake()
		{
			_game = Game.Instance;
			Game game = _game;
			game.OnStageMenuSequence = (Game.OnStageMenuSequenceDelegate)Delegate.Combine(game.OnStageMenuSequence, new Game.OnStageMenuSequenceDelegate(HandleOnStageMenuSequence));
			_track = Track.Instance;
			_character = Character.Instance;
			_ceilingBrickExpolsion = _character.transform.FindChild("turboHeadstartPuff").GetComponent<ParticleSystem>();
			_characterController = _character.characterController;
			_characterTransform = _characterController.transform;
			_characterCamera = CharacterCamera.Instance;
			_characterCameraTransform = _characterCamera.transform;
			_inAirCoinsManager = this.FindObject<InAirCoinsManager>();
			_pickupSpawnPointPrefabInstances = new GameObject[3];
			_endOfFlightPickups = new GameObject[3];
			for (int i = 0; i < 3; i++)
			{
				_endOfFlightPickups[i] = UnityEngine.Object.Instantiate(_endOfFlightPickupPrefab);
				HierarchyUtility.ReparentByHierarchyPathType(_endOfFlightPickups[i].transform, HierarchyPathType.Dynamic);
				_pickupLists.Add(_endOfFlightPickups[i].GetComponentsInChildren<PickupDefault>());
				SetActiveEndOfFlightPickups(i, false);
				if (PickupsMigration.Migration_IsActivePickupSystemVersion2015)
				{
					_pickupSpawnPointPrefabInstances[i] = SingletonMonobehavior<PickupManager2>.Instance.SpawnPickupSpawnPoint();
					HierarchyUtility.ReparentByHierarchyPathType(_pickupSpawnPointPrefabInstances[i].transform, HierarchyPathType.Dynamic, new Vector3(0f, 0f, 0f));
					SetActivePickupSpawnPointPrefabInstances(i, false);
				}
			}
		}

		public void ExtraHeadstart()
		{
			if (_turboHeadstart == 1)
			{
				UpdateHeadstart(PowerupType.turboHeadstart1, _turboHeadstart1FlyHeight, _turboHeadstart);
			}
			else if (_turboHeadstart == 2)
			{
				UpdateHeadstart(PowerupType.turboHeadstart2, _turboHeadstart2FlyHeight, _turboHeadstart);
			}
		}

		public override void HandleSwipe(SwipeDir swipeDir)
		{
			switch (swipeDir)
			{
			case SwipeDir.None:
				break;
			case SwipeDir.Left:
				_character.ChangeTrack(-1, characterChangeTrackLength / _game.currentSpeed);
				break;
			case SwipeDir.Right:
				_character.ChangeTrack(1, characterChangeTrackLength / _game.currentSpeed);
				break;
			}
		}

		public void ActivateLongHeadstart()
		{
			if (!_deactivateTurboHeadstart)
			{
				_activateTurboHeadstart = true;
				_turboHeadstart++;
			}
		}

		public void ResetTurboHeadstart()
		{
			_canActivateLongHeadstart = false;
			_deactivateTurboHeadstart = false;
			_activateTurboHeadstart = false;
			_turboHeadstart = 0;
		}

		public override IEnumerator Begin()
		{
			isActive = true;
			CharacterRendering.Instance.ResetTeleportFx();
			_character.IsGrounded.Value = false;
			if (_powerupType != PowerupType.headstart && _powerupType != PowerupType.turboHeadstart1 && _powerupType != PowerupType.turboHeadstart2)
			{
				GameStats.Instance.pickedUpPowerups++;
			}
			Powerup = GameStats.Instance.TriggerPowerup(_powerupType);
			_game.Modifiers.PauseInJetpackMode();
			if (_character.IsStumbling)
			{
				_character.StopStumble();
			}
			if (PogoStickState.Instance.isActive)
			{
				PogoStickState.Instance.StopHack();
			}
			NotifyOnStart(_isHeadStart);
			CoinLineManager.Instance.ClearTopLevelLines();
			Vector3 startCameraOffset = _characterCameraTransform.position - _characterTransform.position;
			float startCameraAimOffset = _game.RunningState.cameraAimOffset;
			float startY = _characterTransform.position.y;
			_characterController.detectCollisions = false;
			_character.characterTrigger.enabled = false;
			SmoothDampFloat smoothCameraX = new SmoothDampFloat(_characterCameraTransform.position.x, smoothCameraXDuration);
			SmoothDampFloat y = new SmoothDampFloat(_characterTransform.position.y, ySmoothDuration)
			{
				Target = flyHeight
			};
			_newflyHeight = _character.transform.position.y;
			float flyingDuration;
			if (_isHeadStart)
			{
				float duration = PlayerInfo.Instance.GetPowerupDuration(_powerupType) - flyAheadDuration;
				if (this.OnActivateTurboHeadstart != null)
				{
					this.OnActivateTurboHeadstart();
				}
				_headStartSpeed = PlayerInfo.Instance.GetPowerupSpeed(_powerupType);
				_headStartDistance = duration * _headStartSpeed;
				flyingDuration = _headStartDistance / _headStartSpeed;
				SetFOV();
			}
			else
			{
				flyingDuration = Powerup.timeLeft;
			}
			_jetpackSpeed = ((!_isHeadStart) ? (_game.currentLevelSpeed * speedup) : _headStartSpeed);
			float flyDistance = _jetpackSpeed * flyingDuration;
			_flyAheadDistance = _jetpackSpeed * flyAheadDuration;
			float _jetpackDistanceOriginal = _flyAheadDistance + flyDistance;
			float _jetpackDistanceCorrected = _track.LayJetpackChunks(_character.z, _jetpackDistanceOriginal) - stopBeforeLandingChunkDistance * Game.Instance.NormalizedGameSpeed;
			float _jetpackDurationCorrected = _jetpackDistanceCorrected / _jetpackSpeed;
			_jetpackDurationExtended = _jetpackDurationCorrected - flyAheadDuration;
			flyDistance = _jetpackDurationExtended * _jetpackSpeed;
			if (!_isHeadStart)
			{
				float coinsDistance = flyDistance - coinOffset;
				float coinsStartZ = _character.z + _flyAheadDistance + coinOffset;
				if (SingletonMonobehavior<CoinManager2>.Instance.QA_SpawnOldDynamicCoins)
				{
					_inAirCoinsManager.Spawn(coinsStartZ, coinsDistance, flyHeight);
				}
				if (SingletonMonobehavior<CoinManager2>.Instance.QA_SpawnNewDynamicCoins)
				{
					SpawnCoins(coinsStartZ, coinsDistance, flyHeight);
				}
			}
			landingTime = Time.time + _jetpackDurationCorrected;
			landingZ = _character.z + _jetpackDistanceCorrected;
			float cameraZ = _character.z;
			_startTime = Time.time;
			float percentageThroughMovement = GetRatioRemainingForFlyAhead();
			_game.currentSpeed = _jetpackSpeed;
			if (!_isHeadStart)
			{
				UpdatePowerupTimeLeft(_jetpackDurationCorrected);
			}
			Vector3 cameraPositionStart = _characterCamera.Position;
			Vector3 cameraTargetStart = _characterCamera.Target;
			if (OnFlyAheadStart != null)
			{
				OnFlyAheadStart();
			}
			bool hasExploded = false;
			while (percentageThroughMovement < 1f)
			{
				if (!_canActivateLongHeadstart)
				{
					_canActivateLongHeadstart = true;
				}
				_game.HandleControls();
				Vector3 currentCameraOffset = Vector3.Lerp(startCameraOffset, cameraOffset, Mathf.SmoothStep(0f, 1f, percentageThroughMovement));
				float characterAimOffset = Mathf.Lerp(startCameraAimOffset, cameraAimOffset, Mathf.SmoothStep(0f, 1f, percentageThroughMovement));
				_character.z += _jetpackSpeed * Time.deltaTime;
				Vector3 pivot = _track.GetPosition(_character.x, _character.z) + Vector3.up * (startY + (flyHeight - startY) * Mathf.SmoothStep(0f, 1f, percentageThroughMovement));
				_characterTransform.position = pivot;
				smoothCameraX.Update();
				smoothCameraX.Target = _character.transform.position.x * 0.9f;
				cameraZ += ((!_isHeadStart) ? _game.currentLevelSpeed : _jetpackSpeed) * Time.deltaTime;
				Vector3 cameraPositionEnd = new Vector3(smoothCameraX.Value, pivot.y, _character.z) + currentCameraOffset;
				Vector3 cameraTargetEnd = new Vector3(smoothCameraX.Value, pivot.y, pivot.z) + Vector3.up * characterAimOffset;
				float warpedRatio = flyAhead.cameraMovement.Evaluate(percentageThroughMovement);
				Vector3 cameraPositionNew = Vector3.Lerp(cameraPositionStart, cameraPositionEnd, warpedRatio);
				Vector3 cameraTargetNew = Vector3.Lerp(cameraTargetStart, cameraTargetEnd, warpedRatio);
				cameraPositionNew.x = smoothCameraX.Value;
				cameraTargetNew.x = smoothCameraX.Value;
				_characterCamera.Position = cameraPositionNew;
				_characterCamera.Target = cameraTargetNew;
				if (OnFlyAheadUpdate != null)
				{
					OnFlyAheadUpdate(percentageThroughMovement);
				}
				if (!hasExploded && pivot.y > _character.subwayMaxY && _character.IsInsideSubway)
				{
					_ceilingBrickExpolsion.gameObject.SetActive(true);
					_ceilingBrickExpolsion.Play();
					hasExploded = true;
					_character.ForceLeaveSubway();
				}
				_game.UpdateMeters();
				_game.LayTrackChunks();
				yield return null;
				percentageThroughMovement = GetRatioRemainingForFlyAhead();
			}
			_character.characterTrigger.enabled = true;
			_startTime = Time.time;
			for (percentageThroughMovement = GetRatioRemainingForFlying(); percentageThroughMovement < 1f; percentageThroughMovement = GetRatioRemainingForFlying())
			{
				if (percentageThroughMovement > 0.8f && !_deactivateTurboHeadstart && this.OnHidTurboHeadstartButtons != null)
				{
					this.OnHidTurboHeadstartButtons();
				}
				if (percentageThroughMovement > 0.9f && !_deactivateTurboHeadstart)
				{
					_deactivateTurboHeadstart = true;
					if (this.OnDeactivateTurboHeadstart != null)
					{
						this.OnDeactivateTurboHeadstart();
					}
				}
				if (_isHeadStart)
				{
					_currentHeadstartTime += Time.deltaTime;
				}
				if (_activateTurboHeadstart)
				{
					ExtraHeadstart();
					_activateTurboHeadstart = false;
				}
				_game.HandleControls();
				_character.z += _jetpackSpeed * Time.deltaTime;
				if (_turboHeadstart == 1 && _newflyHeight < _turboHeadstart1FlyHeight)
				{
					_newflyHeight = Mathf.Clamp(_character.transform.position.y + Time.deltaTime * changeHeightSpeed, 0f, _turboHeadstart1FlyHeight);
					_character.transform.position = _track.GetPosition(_character.x, _character.z) + Vector3.up * _newflyHeight;
				}
				else if (_turboHeadstart == 1 && _newflyHeight == _turboHeadstart1FlyHeight)
				{
					_character.transform.position = _track.GetPosition(_character.x, _character.z) + Vector3.up * _turboHeadstart1FlyHeight;
				}
				else if (_turboHeadstart == 2 && _newflyHeight < _turboHeadstart2FlyHeight)
				{
					_newflyHeight = Mathf.Clamp(_character.transform.position.y + Time.deltaTime * changeHeightSpeed, 0f, _turboHeadstart2FlyHeight);
					_character.transform.position = _track.GetPosition(_character.x, _character.z) + Vector3.up * _newflyHeight;
				}
				else if (_turboHeadstart == 2 && _newflyHeight == _turboHeadstart2FlyHeight)
				{
					_character.transform.position = _track.GetPosition(_character.x, _character.z) + Vector3.up * _turboHeadstart2FlyHeight;
				}
				else
				{
					_character.transform.position = _track.GetPosition(_character.x, _character.z) + Vector3.up * flyHeight;
				}
				smoothCameraX.Update();
				smoothCameraX.Target = _character.transform.position.x * 0.9f;
				Vector3 characterPosition = _character.transform.position;
				_characterCamera.Position = characterPosition + cameraOffset;
				_characterCamera.Position.x = smoothCameraX.Value;
				characterPosition.x = smoothCameraX.Value;
				_characterCamera.Target = characterPosition + Vector3.up * cameraAimOffset;
				_game.UpdateMeters();
				_game.LayTrackChunks();
				yield return null;
			}
			_canActivateLongHeadstart = false;
			_activateTurboHeadstart = false;
			isActive = false;
			NotifyOnStop();
			_characterCamera.ResetShake();
			_characterController.detectCollisions = true;
			_character.verticalSpeed = _character.CalculateJumpVerticalSpeed(0f);
			RevertFOV();
			_inAirCoinsManager.ReleaseCoins();
			CoinStats.New.NotifyDynamicCoinsDespawned(_dynamicCoinsSpawned);
			_dynamicCoinsSpawned = 0;
			if (_isHeadStart && _turboHeadstart == 0)
			{
				_game.ChangeCurrentSpeed(_game.speed.rampUpDuration * PlayerInfo.Instance.GetPowerupLandSpeed(PowerupType.headstart));
			}
			else if (_turboHeadstart == 1)
			{
				_game.ChangeCurrentSpeed(_game.speed.rampUpDuration * PlayerInfo.Instance.GetPowerupLandSpeed(PowerupType.turboHeadstart1));
			}
			else if (_turboHeadstart == 2)
			{
				_game.ChangeCurrentSpeed(_game.speed.rampUpDuration * PlayerInfo.Instance.GetPowerupLandSpeed(PowerupType.turboHeadstart2));
			}
			_turboHeadstart = 0;
			_currentHeadstartTime = 0f;
			_game.ChangeState(_game.RunningState);
			_game.Modifiers.Resume();
		}

		private float GetRatioRemainingForFlyAhead()
		{
			return (Time.time - _startTime) / flyAheadDuration;
		}

		private float GetRatioRemainingForFlying()
		{
			return (Time.time - _startTime) / _jetpackDurationExtended;
		}

		private void UpdatePowerupTimeLeft(float newTimeLeft)
		{
			_powerupTimeLeftMax = (Powerup.timeLeft = newTimeLeft);
		}

		public void PlacePickups(float z)
		{
			if (!_isHeadStart)
			{
				return;
			}
			float num = 0f;
			float z2 = z;
			if (_turboHeadstart == 0)
			{
				num = flyHeight - 5f;
				z2 = z - 30f;
			}
			else if (_turboHeadstart == 1)
			{
				num = _turboHeadstart1FlyHeight - 5f;
				z2 = z - 75f;
			}
			else if (_turboHeadstart == 2)
			{
				num = _turboHeadstart2FlyHeight - 5f;
				z2 = z - 70f;
			}
			int[] array = new int[3] { -1, -1, -1 };
			for (int i = 0; i < _endOfFlightPickups.Length; i++)
			{
				SetActiveEndOfFlightPickups(i, true);
				if (PickupsMigration.Migration_IsActivePickupSystemVersion2014)
				{
					_endOfFlightPickups[i].transform.position = new Vector3(-20 + 20 * i, num, z2);
				}
				int num2 = -1;
				while (num2 == array[0] || num2 == array[1] || num2 == array[2])
				{
					num2 = UnityEngine.Random.Range(0, _endOfFlightPickups.Length);
					if (num2 != array[0] && num2 != array[1] && num2 != array[2])
					{
						array[i] = num2;
						break;
					}
				}
				for (int j = 0; j < _pickupLists.Count; j++)
				{
					if (j == num2)
					{
						if (PickupsMigration.Migration_IsActivePickupSystemVersion2015)
						{
							PickupType pickupType = PickupsMigration.Migration_ConvertFromPickupTypeName2014(_pickupLists[i][j].name);
							PickupSpawnPoint2 component = _pickupSpawnPointPrefabInstances[i].GetComponent<PickupSpawnPoint2>();
							component.Migration_Replace2014SpawnPointWith2015SpawnPointForced(_pickupSpawnPointPrefabInstances[i], pickupType, SpawnPointDifficulty.Easy, false, true);
							SingletonMonobehavior<PickupManager2>.Instance.CallPickerCollectionUpdate();
							_pickupSpawnPointPrefabInstances[i].transform.position = new Vector3(-20 + 20 * i, num + 10f, z2);
							SetActivePickupSpawnPointPrefabInstances(i, true);
						}
						if (PickupsMigration.Migration_IsActivePickupSystemVersion2014)
						{
							_pickupLists[i][j].Activate();
						}
					}
					else
					{
						_pickupLists[i][j].Deactivate();
					}
				}
			}
		}

		private void SetActivePickupSpawnPointPrefabInstances(int i, bool isActive)
		{
			_pickupSpawnPointPrefabInstances[i].SetActive(isActive);
		}

		private void SetActiveEndOfFlightPickups(int i, bool isActive)
		{
			_endOfFlightPickups[i].SetActive(isActive);
		}

		private void SpawnCoins(float startZ, float length, float height)
		{
			if (SingletonMonobehavior<CoinManager2>.Instance.QA_SpawnOldDynamicCoins && SingletonMonobehavior<CoinManager2>.Instance.QA_SpawnNewDynamicCoins)
			{
				StartCoroutine(SpawnCoinCurve(_inAirCoinsManager.Curve, startZ, length, height));
				return;
			}
			AnimationCurve animationCurve = new AnimationCurve();
			int num = 1;
			for (float num2 = startZ; num2 < startZ + length; num2 += characterChangeTrackLength + 60f)
			{
				animationCurve.AddKey(new Keyframe(num2, _track.GetTrackX(num)));
				animationCurve.AddKey(new Keyframe(num2 + 60f, _track.GetTrackX(num)));
				num = Mathf.Clamp(num + UnityEngine.Random.Range(-1, 2), 0, _track.numberOfTracks - 1);
				animationCurve.AddKey(new Keyframe(num2 + 60f + characterChangeTrackLength, _track.GetTrackX(num)));
			}
			StartCoroutine(SpawnCoinCurve(animationCurve, startZ, length, height));
		}

		private IEnumerator SpawnCoinCurve(AnimationCurve curve, float startZ, float length, float height)
		{
			CoinManager2 coinManager = SingletonMonobehavior<CoinManager2>.Instance;
			if (Globals.DEBUG_LOG_COINS)
			{
				int coinsSpawned = 0;
				for (float logZ = startZ; logZ < startZ + length; logZ += 30f)
				{
					coinsSpawned++;
				}
				CoinSystemSpawnLog.New.LogSpawnPoint(base.gameObject, coinsSpawned);
			}
			float z = startZ;
			while (z < startZ + length)
			{
				Vector3 p = Vector3.up * height + _track.GetPosition(curve.Evaluate(z), z);
				if (coinManager.QA_SpawnOldDynamicCoins && coinManager.QA_SpawnNewDynamicCoins)
				{
					p.x += 5f;
				}
				coinManager.SpawnCoin(p);
				z += 30f;
				CoinStats.New.NotifyDynamicCoinSpawned();
				_dynamicCoinsSpawned++;
				yield return null;
			}
		}

		private void ResetPickups()
		{
			for (int i = 0; i < _endOfFlightPickups.Length; i++)
			{
				SetActiveEndOfFlightPickups(i, false);
			}
			if (PickupsMigration.Migration_IsActivePickupSystemVersion2015)
			{
				for (int j = 0; j < _pickupSpawnPointPrefabInstances.Length; j++)
				{
					SetActivePickupSpawnPointPrefabInstances(j, false);
				}
			}
		}

		private void HandleOnStageMenuSequence()
		{
			ResetPickups();
		}

		private void UpdateHeadstart(PowerupType powerType, float height, int version)
		{
			float num = PlayerInfo.Instance.GetPowerupDuration(powerType) - flyAheadDuration;
			_headStartTopSpeed = PlayerInfo.Instance.GetPowerupSpeed(powerType);
			_headStartDistance = num * _headStartTopSpeed;
			_startTime = Time.time;
			_jetpackSpeed = _headStartTopSpeed;
			float num2 = _headStartDistance;
			if (version == 1)
			{
				num2 = _track.LayTurboHeadstartChunks(_character.z, num2, _turboHeadstart - 1) - stopBeforeLandingChunkDistance1 * Game.Instance.NormalizedGameSpeed;
			}
			if (version == 2)
			{
				num2 = _track.LayTurboHeadstartChunks(_character.z, num2, _turboHeadstart - 1) - stopBeforeLandingChunkDistance2 * Game.Instance.NormalizedGameSpeed;
			}
			_jetpackDurationExtended = num2 / _headStartTopSpeed;
		}

		private void SetFOV()
		{
			float fov_start = _characterCamera.Camera.fieldOfView;
			float fov_end = (RunningState.Instance.cameraFOV + megaHeadstartFOV) / _characterCamera.Camera.aspect;
			StartCoroutine(pTween.To(trasitionTime, delegate(float t)
			{
				_characterCamera.Camera.fieldOfView = Mathf.Lerp(fov_start, fov_end, t);
			}));
		}

		private void RevertFOV()
		{
			float fov_start = _characterCamera.Camera.fieldOfView;
			float fov_end = 0f;
			if (Hoverboard.Instance.isActive && Hoverboard.Instance.IsHighSpeedBoard)
			{
				fov_end = (RunningState.Instance.cameraFOV + _game.HighSpeedFOV) / _characterCamera.Camera.aspect;
			}
			else if (Hoverboard.Instance.isActive && Hoverboard.Instance.IsSpeedBoard)
			{
				fov_end = (RunningState.Instance.cameraFOV + _game.SpeedFOV) / _characterCamera.Camera.aspect;
			}
			else
			{
				fov_end = RunningState.Instance.cameraFOV / _characterCamera.Camera.aspect;
			}
			StartCoroutine(pTween.To(trasitionTime, delegate(float t)
			{
				if (Game.Instance.CharacterState == Game.Instance.RunningState || Game.Instance.CharacterState == Game.Instance.Jetpack)
				{
					_characterCamera.Camera.fieldOfView = Mathf.Lerp(fov_start, fov_end, t);
				}
				else
				{
					_characterCamera.Camera.fieldOfView = RunningState.Instance.cameraFOV / _characterCamera.Camera.aspect;
				}
			}));
		}

		private void NotifyOnStart(bool isHeadstart)
		{
			if (OnStart != null)
			{
				OnStart(isHeadstart);
			}
		}

		private void NotifyOnStop()
		{
			if (OnStop != null)
			{
				OnStop();
			}
		}
	}
}
