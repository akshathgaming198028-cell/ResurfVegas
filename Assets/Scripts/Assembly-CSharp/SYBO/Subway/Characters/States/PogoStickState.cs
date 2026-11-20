using System;
using System.Collections;
using System.Linq;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities;
using SYBO.Subway.Cameras;
using SYBO.Subway.Coins;
using SYBO.Subway.Pickups;
using SYBO.Subway.Pickups.Migration;
using UnityEngine;

namespace SYBO.Subway.Characters.States
{
	public class PogoStickState : CharacterState
	{
		[Serializable]
		public class pickupWeight
		{
			public string pickupName;

			public PowerupType type;
		}

		[Serializable]
		public class MovementCurve
		{
			public AnimationCurve curve;
		}

		public delegate void OnStartDelegate();

		public delegate void OnReachedTopDelegate();

		public delegate void OnStopDelegate();

		[SerializeField]
		private int rows = 5;

		[SerializeField]
		private int startRowPosition = 1;

		[SerializeField]
		private int endRowPosition = 1;

		[SerializeField]
		private pickupWeight[] weightList;

		[SerializeField]
		private GameObject _endOfFlightPickupPrefab;

		private GameObject _endOfFlightPickupPrefabInstance;

		public bool isActive;

		public AudioClipInfo powerDownSound;

		public float cameraAimOffset = 20f;

		public float jumpHeight = 95f;

		public float jumpDistance = 800f;

		public float characterChangeTrackLength = 60f;

		public PowerupType powerType = PowerupType.pogostick;

		public ActivePowerup Powerup;

		public MovementCurve jumpCurve;

		public MovementCurve cameraCurve;

		public MovementCurve cameraAimFollowCurve;

		public float smoothCameraXDuration = 0.05f;

		public float topPosition = 0.5f;

		public float fadeInPosition = 0.1f;

		public float hangtimePosition = 0.5f;

		public float finalJumpSpeed;

		public OnStartDelegate OnStart;

		public OnStartDelegate OnHangtime;

		public OnStopDelegate OnStop;

		private int _dynamicCoinsSpawned;

		private Game _game;

		private Track _track;

		private Character _character;

		private CharacterController _characterController;

		private Transform _characterTransform;

		private CharacterCamera _characterCamera;

		private Transform _characterCameraTransform;

		private Animation _characterAnimation;

		private CopyTransform _copyTransform;

		[HideInInspector]
		public PogostickCoinsManager PogostickCoinsManager;

		private CoinLineManager _coinLineManager;

		private PickupDefault[] _pickupDefaultWeightedList;

		private PickupDefault[] _pickupDefaultList;

		private float _characterAimOffset;

		private bool _reachedHangtime;

		private GameObject _pickupSpawnPointPrefabInstance;

		private static PogoStickState instance;

		public override bool PauseActiveModifiers
		{
			get
			{
				return true;
			}
		}

		public static PogoStickState Instance
		{
			get
			{
				return instance ?? (instance = UnityEngine.Object.FindObjectOfType(typeof(PogoStickState)) as PogoStickState);
			}
		}

		protected void Awake()
		{
			_game = Game.Instance;
			Game game = _game;
			game.OnStageMenuSequence = (Game.OnStageMenuSequenceDelegate)Delegate.Combine(game.OnStageMenuSequence, new Game.OnStageMenuSequenceDelegate(HandleOnStageMenu));
			_game.OnCharacterModelRecreated += Game_OnCharacterModelRecreated;
			_track = Track.Instance;
			_character = Character.Instance;
			_characterController = _character.characterController;
			_characterTransform = _characterController.transform;
			_characterCamera = CharacterCamera.Instance;
			_characterCameraTransform = _characterCamera.transform;
			PogostickCoinsManager = this.FindObject<PogostickCoinsManager>();
			_coinLineManager = CoinLineManager.Instance;
			if (!(_endOfFlightPickupPrefab != null))
			{
				return;
			}
			_endOfFlightPickupPrefabInstance = UnityEngine.Object.Instantiate(_endOfFlightPickupPrefab);
			HierarchyUtility.ReparentByHierarchyPathType(_endOfFlightPickupPrefabInstance.transform, HierarchyPathType.Dynamic, new Vector3(0f, -200f, 0f));
			_pickupDefaultList = _endOfFlightPickupPrefabInstance.GetComponentsInChildren<PickupDefault>();
			int num = 0;
			pickupWeight[] array = weightList;
			foreach (pickupWeight pickupWeight2 in array)
			{
				num += Upgrades.upgrades[pickupWeight2.type].spawnProbability;
			}
			_pickupDefaultWeightedList = new PickupDefault[num];
			int num2 = 0;
			for (int j = 0; j < weightList.Length; j++)
			{
				for (int k = 0; k < _pickupDefaultList.Length; k++)
				{
					if (weightList[j].pickupName == _pickupDefaultList[k].gameObject.name)
					{
						for (int l = 0; l < Upgrades.upgrades[weightList[j].type].spawnProbability; l++)
						{
							_pickupDefaultWeightedList[num2] = _pickupDefaultList[k];
							num2++;
						}
					}
				}
			}
			SetActiveForEndOfFlightPickup(false);
		}

		private void Game_OnCharacterModelRecreated()
		{
		}

		public override IEnumerator Begin()
		{
			isActive = true;
			_coinLineManager.ToggleLines(true);
			_game.ResetEnemy();
			_character.inAirJump = false;
			_character.IsGrounded.Value = false;
			GameStats.Instance.pickedUpPowerups++;
			Powerup = GameStats.Instance.TriggerPowerup(powerType);
			_game.Modifiers.PauseInJetpackMode();
			if (_character.IsStumbling)
			{
				_character.StopStumble();
			}
			_characterController.detectCollisions = false;
			NotifyOnStart();
			Vector3 startPosition = _characterTransform.position;
			Vector3 endPosition = startPosition + Vector3.forward * jumpDistance;
			StartCoroutine(RemovePickups(endPosition.z));
			float speed = _game.currentSpeed;
			Vector3 cameraPositionStart = _characterCamera.Position;
			Vector3 CameraTargetStartOffset = _characterCamera.Position - _characterCamera.Target;
			Vector3 cameraOffset = new Vector3(0f, 33f, -33f);
			Vector3 startCameraOffset = _characterCameraTransform.position - _characterTransform.position;
			SmoothDampFloat smoothCameraX = new SmoothDampFloat(_characterCameraTransform.position.x, smoothCameraXDuration);
			float coinOffsetZPosition = jumpDistance / (float)rows;
			if (Globals.DEBUG_LOG_COINS)
			{
				int coinsSpawned = 0;
				for (int i = 0; i < endRowPosition; i++)
				{
					if (i > startRowPosition)
					{
						coinsSpawned += 3;
					}
				}
				CoinSystemSpawnLog.Old.LogSpawnPoint(base.gameObject, coinsSpawned);
				CoinSystemSpawnLog.New.LogSpawnPoint(base.gameObject, coinsSpawned);
			}
			for (int j = 0; j < endRowPosition; j++)
			{
				if (j > startRowPosition)
				{
					Vector3 rowPlacement = new Vector3(0f, jumpCurve.curve.Evaluate((float)j / (float)rows) * jumpHeight, _character.z + (float)j * coinOffsetZPosition);
					if (SingletonMonobehavior<CoinManager2>.Instance.QA_SpawnOldDynamicCoins)
					{
						PogostickCoinsManager.placeRow(rowPlacement.z, rowPlacement.y);
					}
					if (SingletonMonobehavior<CoinManager2>.Instance.QA_SpawnNewDynamicCoins)
					{
						SpawnCoinRow(rowPlacement.z, rowPlacement.y);
					}
				}
			}
			int randomX = UnityEngine.Random.Range(0, 3);
			float xPosition = 0f;
			switch (randomX)
			{
			case 0:
				xPosition = -20f;
				break;
			case 1:
				xPosition = 0f;
				break;
			default:
				xPosition = 20f;
				break;
			}
			if (_endOfFlightPickupPrefabInstance != null)
			{
				SetActiveForEndOfFlightPickup(true);
				PickupDefault nextPickup = Enumerable.ElementAt(index: UnityEngine.Random.Range(0, _pickupDefaultWeightedList.Length), source: _pickupDefaultWeightedList);
				Vector3 newWorldPosition2014 = new Vector3(xPosition, jumpCurve.curve.Evaluate(1f) * jumpHeight, endPosition.z + 20f);
				_endOfFlightPickupPrefabInstance.transform.position = newWorldPosition2014;
				if (PickupsMigration.Migration_IsActivePickupSystemVersion2015)
				{
					Vector3 newWorldPosition2015 = newWorldPosition2014 + new Vector3(0f, 10f, 0f);
					if (_pickupSpawnPointPrefabInstance != null && SingletonMonobehavior<PickupManager2>.Instance.IsSpawned(_pickupSpawnPointPrefabInstance.transform))
					{
						SingletonMonobehavior<PickupManager2>.Instance.Despawn(_pickupSpawnPointPrefabInstance.transform);
						_pickupSpawnPointPrefabInstance = null;
					}
					PickupType pickupType = PickupDefault.ConvertSubclassInstanceToPickupType(nextPickup);
					_pickupSpawnPointPrefabInstance = SingletonMonobehavior<PickupManager2>.Instance.SpawnPickupSpawnPoint();
					HierarchyUtility.ReparentByHierarchyPathType(_pickupSpawnPointPrefabInstance.transform, HierarchyPathType.Dynamic, newWorldPosition2015);
					_pickupSpawnPointPrefabInstance.GetComponent<PickupSpawnPoint2>().Migration_Replace2014SpawnPointWith2015SpawnPointForced(_endOfFlightPickupPrefabInstance, pickupType, SpawnPointDifficulty.Easy, false, true);
					SingletonMonobehavior<PickupManager2>.Instance.CallPickerCollectionUpdate();
					SetActiveForPickupPrefabInstance(true);
				}
				if (PickupsMigration.Migration_IsActivePickupSystemVersion2014)
				{
					PickupDefault[] pickupDefaultList = _pickupDefaultList;
					foreach (PickupDefault element in pickupDefaultList)
					{
						if (element == nextPickup)
						{
							element.Activate();
						}
						else
						{
							element.Deactivate();
						}
					}
				}
			}
			while (_character.z < endPosition.z)
			{
				_game.HandleControls();
				_character.z += speed * Time.deltaTime;
				float normalizedPosition = (_character.z - startPosition.z) / jumpDistance;
				Vector3 pivot = _track.GetPosition(_character.x, _character.z) + Vector3.up * jumpCurve.curve.Evaluate(normalizedPosition) * jumpHeight;
				if (normalizedPosition <= fadeInPosition)
				{
					pivot.y = Mathf.Lerp(startPosition.y, pivot.y, normalizedPosition / fadeInPosition);
				}
				if (normalizedPosition > hangtimePosition && !_reachedHangtime)
				{
					NotifyHangtime();
					_reachedHangtime = true;
				}
				_characterTransform.position = pivot;
				_characterCamera.Target = pivot;
				_characterCamera.Position = pivot + cameraOffset;
				Vector3 position = _character.transform.position;
				smoothCameraX.Update();
				smoothCameraX.Target = position.x * 0.75f;
				float warpedCameraRatio = cameraCurve.curve.Evaluate(normalizedPosition);
				Vector3 currentCameraOffset = Vector3.Lerp(startCameraOffset, cameraOffset, Mathf.SmoothStep(0f, 1f, normalizedPosition));
				Vector3 cameraPositionEnd = new Vector3(smoothCameraX.Value, pivot.y, _character.z) + currentCameraOffset;
				Vector3 cameraPositionNew = new Vector3(cameraPositionEnd.x, Mathf.Lerp(cameraPositionStart.y, cameraPositionEnd.y, warpedCameraRatio), cameraPositionEnd.z)
				{
					x = smoothCameraX.Value
				};
				Vector3 cameraTargetNew = Vector3.Lerp(cameraPositionNew - CameraTargetStartOffset, _character.transform.position + Vector3.up * cameraAimOffset, cameraAimFollowCurve.curve.Evaluate(normalizedPosition));
				cameraTargetNew.x = smoothCameraX.Value;
				_characterCamera.Position = cameraPositionNew;
				_characterCamera.Target = cameraTargetNew;
				_game.UpdateMeters();
				_game.LayTrackChunks();
				yield return null;
			}
			EndPogo();
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
			case SwipeDir.Down:
				EndPogo();
				_character.Roll();
				break;
			}
		}

		public void StopHack()
		{
			isActive = false;
			NotifyOnStop();
		}

		private void EndPogo()
		{
			isActive = false;
			if (_game.pogoJumpsLeft == 0)
			{
				NotifyOnStop();
			}
			_characterController.detectCollisions = true;
			RunningState.Instance.transitionFromPogostick = true;
			_game.ChangeState(_game.RunningState);
			if (!_character.IsGliderboard)
			{
				_character.verticalSpeed = _character.CalculateJumpVerticalSpeed(finalJumpSpeed);
			}
			_game.Modifiers.Resume();
			_reachedHangtime = false;
		}

		private IEnumerator RemovePickups(float endPosition)
		{
			while (_character.z < endPosition + 100f)
			{
				yield return null;
			}
			if (_pickupSpawnPointPrefabInstance != null && SingletonMonobehavior<PickupManager2>.Instance.IsSpawned(_pickupSpawnPointPrefabInstance.transform))
			{
				SingletonMonobehavior<PickupManager2>.Instance.Despawn(_pickupSpawnPointPrefabInstance.transform);
				_pickupSpawnPointPrefabInstance = null;
			}
			PogostickCoinsManager.ReleaseCoins();
			CoinStats.New.NotifyDynamicCoinsDespawned(_dynamicCoinsSpawned);
			_dynamicCoinsSpawned = 0;
			SetActiveForEndOfFlightPickup(false);
			SetActiveForPickupPrefabInstance(false);
			ResetPickup();
		}

		private void ResetPickup()
		{
			if (_pickupDefaultList == null)
			{
				return;
			}
			PickupDefault[] pickupDefaultList = _pickupDefaultList;
			foreach (PickupDefault pickupDefault in pickupDefaultList)
			{
				if (pickupDefault != null)
				{
					pickupDefault.gameObject.SetActive(true);
				}
			}
		}

		private void HandleOnStageMenu()
		{
			SetActiveForEndOfFlightPickup(false);
			SetActiveForPickupPrefabInstance(false);
		}

		private void SetActiveForEndOfFlightPickup(bool isActive)
		{
			_endOfFlightPickupPrefabInstance.SetActive(isActive);
		}

		private void SetActiveForPickupPrefabInstance(bool isActive)
		{
			if (PickupsMigration.Migration_IsActivePickupSystemVersion2015 && _pickupSpawnPointPrefabInstance != null)
			{
				_pickupSpawnPointPrefabInstance.SetActive(isActive);
			}
		}

		private void SpawnCoinRow(float z, float height)
		{
			CoinManager2 coinManager = SingletonMonobehavior<CoinManager2>.Instance;
			for (int i = 0; i < 3; i++)
			{
				Vector3 position = new Vector3(_track.GetTrackX(i - 1), height, z);
				if (coinManager.QA_SpawnNewDynamicCoins && coinManager.QA_SpawnOldDynamicCoins)
				{
					position.x += 5f;
				}
				coinManager.SpawnCoin(position);
				CoinStats.New.NotifyDynamicCoinSpawned();
				_dynamicCoinsSpawned++;
			}
		}

		private void NotifyOnStart()
		{
			if (OnStart != null)
			{
				OnStart();
			}
		}

		private void NotifyHangtime()
		{
			if (OnHangtime != null)
			{
				OnHangtime();
			}
		}

		private void NotifyOnStop()
		{
			if (OnStop != null)
			{
				OnStop();
			}
		}

		public static bool IsAPogoStickPickup(GameObject go)
		{
			return go.name.ToLower().Contains("pogo");
		}
	}
}
