using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities;
using SYBO.Subway.Characters;
using SYBO.Subway.Pickups.Pickers;
using UnityEngine;

namespace SYBO.Subway.Pickups
{
	public class PickupSpawnPoint2 : MonoBehaviour
	{
		public delegate void OnPickupSpawnPointDelegate(PickupSpawnPoint2 pickupSpawnPoint);

		public delegate void OnPickupSpawnPointPickupDelegate(PickupSpawnPoint2 pickupSpawnPoint, PickerData2 pickerData2);

		private const string _PICKUP_SPAWN_POINT_PREFAB_RUNTIME_PATH = "Prefabs/Pickups/Refactored/PickupSpawnPoint";

		public const string PICKUP_SPAWN_POINT_PREFAB_EDITTIME_PATH = "Assets/Resources/Prefabs/Pickups/Refactored/PickupSpawnPoint.prefab";

		public const float DefaultHeight = 7f;

		private static bool IsAllowedPositionReset = true;

		[SerializeField]
		internal GameObject _pickupParent;

		[SerializeField]
		internal GameObject _debugBlockEasy;

		[SerializeField]
		internal GameObject _debugBlockMedium;

		[SerializeField]
		internal GameObject _debugBlockForce;

		[SerializeField]
		internal SpawnPointMode __spawnPointMode;

		private SpawnPointMode _spawnPointMode_Original;

		private bool _willIgnoreZPositionBeforeSpawn_Original;

		internal bool __willIgnoreZPositionBeforeSpawn;

		private PickupType _forceSpawnPickupType_Original;

		[SerializeField]
		internal PickupType __forceSpawnPickupType = PickupType.PogoStick;

		[HideInInspector]
		private SpawnPointState __spawnPointState;

		private SpawnPointDifficulty _spawnPointDifficulty_Original;

		[SerializeField]
		internal SpawnPointDifficulty _spawnPointDifficulty = SpawnPointDifficulty.Easy;

		private SpawnPointHeight _spawnPointHeight_Original;

		[SerializeField]
		internal SpawnPointHeight _spawnPointHeight = SpawnPointHeight.OnGround;

		private bool _willInitializeWithOriginalPositionYZ;

		private Vector3 _localPosition_Original;

		private PickupData2 _pickupData;

		private GameObject _spawnedPickupIcon;

		private TrackObject _trackObject;

		private SnapObject _snapObject;

		private static bool _willShowGizmo
		{
			get
			{
				return SingletonMonobehavior<PickupManager2>.Instance.WillShowPickupSpawnPointGizmos;
			}
		}

		public SpawnPointMode SpawnPointMode
		{
			get
			{
				return __spawnPointMode;
			}
			internal set
			{
				__spawnPointMode = value;
			}
		}

		public bool WillIgnoreZPositionBeforeSpawn
		{
			get
			{
				return __willIgnoreZPositionBeforeSpawn;
			}
			internal set
			{
				__willIgnoreZPositionBeforeSpawn = value;
			}
		}

		public PickupType ForceSpawnPickupType
		{
			get
			{
				return __forceSpawnPickupType;
			}
			internal set
			{
				__forceSpawnPickupType = value;
			}
		}

		public SpawnPointState SpawnPointState
		{
			get
			{
				return __spawnPointState;
			}
			private set
			{
				if (__spawnPointState == SpawnPointState.Despawned)
				{
				}
				__spawnPointState = value;
			}
		}

		public SpawnPointDifficulty SpawnPointDifficulty
		{
			get
			{
				return _spawnPointDifficulty;
			}
		}

		public SpawnPointHeight SpawnPointHeight
		{
			get
			{
				return _spawnPointHeight;
			}
		}

		public Vector3 LocalPositionOriginal
		{
			get
			{
				return _localPosition_Original;
			}
		}

		public bool CanTriggerCollisionWithCharacter
		{
			get
			{
				return SpawnPointState == SpawnPointState.Spawned;
			}
		}

		public PickupData2 PickupData
		{
			get
			{
				return _pickupData;
			}
		}

		private bool _IsAvailableAsTrackObject
		{
			get
			{
				return _trackObject.Active;
			}
		}

		private bool _IsZPositionToWorldAcceptableBeforeSpawn
		{
			get
			{
				return Game.ConvertUnityUnitsToGameMeters(base.transform.position.z - Character.Instance.z) > 0f;
			}
		}

		protected bool _IsZPositionToGameStartAcceptableBeforeSpawn
		{
			get
			{
				if (SingletonMonobehavior<PickupManager2>.Instance.SpawnDistanceMinToGameStart > 0f)
				{
					return Game.ConvertUnityUnitsToGameMeters(base.transform.position.z) >= SingletonMonobehavior<PickupManager2>.Instance.SpawnDistanceMinToGameStart;
				}
				return true;
			}
		}

		public GameObject SpawnedPickupIcon
		{
			get
			{
				return _spawnedPickupIcon;
			}
		}

		public event OnPickupSpawnPointDelegate OnRegistered;

		public event OnPickupSpawnPointDelegate OnDeregistered;

		public event OnPickupSpawnPointDelegate OnPickupDespawned;

		public event OnPickupSpawnPointDelegate OnPickupCollected;

		public event OnPickupSpawnPointPickupDelegate OnPickupSpawned;

		private void SetSpawnPointDifficultyAndOriginalTo(SpawnPointDifficulty spawnPointDifficulty)
		{
			_spawnPointDifficulty = spawnPointDifficulty;
			_spawnPointDifficulty_Original = spawnPointDifficulty;
		}

		private void SetForceSpawnPickupTypeyAndOriginalTo(PickupType pickupType)
		{
			ForceSpawnPickupType = (_forceSpawnPickupType_Original = pickupType);
		}

		private void SetSpawnPointModeAndOriginalTo(SpawnPointMode spawnPointMode)
		{
			SpawnPointMode = (_spawnPointMode_Original = spawnPointMode);
		}

		private void SetWillIgnoreZPositionBeforeSpawnAndOriginalTo(bool willIgnoreZPositionBeforeSpawn)
		{
			WillIgnoreZPositionBeforeSpawn = (_willIgnoreZPositionBeforeSpawn_Original = willIgnoreZPositionBeforeSpawn);
		}

		public void SetLocalPositionAndOriginalTo(Vector3 newPosition)
		{
			_localPosition_Original = newPosition;
			SetLocalPosition(_localPosition_Original);
		}

		public void SetWorldPosition(Vector3 newPosition)
		{
			base.transform.position = newPosition;
		}

		public void SetLocalPosition(Vector3 newPosition)
		{
			base.transform.localPosition = newPosition;
		}

		public void SetLocalPositionToOriginalPositionYZ()
		{
			Vector3 localPosition = new Vector3(base.transform.localPosition.x, _localPosition_Original.y, _localPosition_Original.z);
			SetLocalPosition(localPosition);
			_snapObject.Snap();
		}

		private void ValidatePosition()
		{
		}

		public bool IsAvailableForSpawning(bool willShowDebugLog = false)
		{
			if (WillIgnoreZPositionBeforeSpawn)
			{
				return _IsAvailableForSpawning_State() && _IsAvailableAsTrackObject;
			}
			return _IsAvailableForSpawning_State(willShowDebugLog) && _IsAvailableAsTrackObject && _IsZPositionToWorldAcceptableBeforeSpawn && _IsZPositionToCharacterAcceptableBeforeSpawn(willShowDebugLog) && _IsZPositionToGameStartAcceptableBeforeSpawn;
		}

		public void DebugLogFor_IsAvailableForSpawning()
		{
			if (WillIgnoreZPositionBeforeSpawn)
			{
				Debug.Log("\t1 _IsAvailableForSpawning_State: " + _IsAvailableForSpawning_State(true), base.gameObject);
				Debug.Log("\t2 _IsAvailableAsTrackObject: " + _IsAvailableAsTrackObject, base.gameObject);
				return;
			}
			Debug.Log("\t3 _IsAvailableForSpawning_State: " + _IsAvailableForSpawning_State(true), base.gameObject);
			Debug.Log("\t2 _IsAvailableAsTrackObject: " + _IsAvailableAsTrackObject, base.gameObject);
			Debug.Log("\t4 _IsZPositionToWorldAcceptableBeforeSpawn: " + _IsZPositionToWorldAcceptableBeforeSpawn, base.gameObject);
			Debug.Log("\t5: _IsZPositionToCharacterAcceptableBeforeSpawn: " + _IsZPositionToCharacterAcceptableBeforeSpawn(true), base.gameObject);
			Debug.Log("\t6: _IsZPositionToGameStartAcceptableBeforeSpawn: " + _IsZPositionToGameStartAcceptableBeforeSpawn, base.gameObject);
		}

		private bool _IsAvailableForSpawning_State(bool willShowDebugLog = false)
		{
			if (willShowDebugLog && !_IsAvailableForSpawning_State())
			{
				Debug.Log("_IsAvailableForSpawning_State : " + (SpawnPointState == SpawnPointState.UnspawnedAndAvailable) + " because SpawnPointState: " + SpawnPointState, base.gameObject);
			}
			return SpawnPointState == SpawnPointState.UnspawnedAndAvailable;
		}

		private bool _IsZPositionToCharacterAcceptableBeforeSpawn(bool willShowDebugLog = false)
		{
			if (PickupManager2.SpawnDistanceMinToCharacter > 0f)
			{
				if (base.transform.position.z >= Character.Instance.z)
				{
					if (willShowDebugLog)
					{
						Debug.Log("\t\tdist to char, dist:" + Game.ConvertUnityUnitsToGameMeters(base.transform.position.z - Character.Instance.z) + " >=? min:" + PickupManager2.SpawnDistanceMinToCharacter);
						Debug.Log("\t\ttransform.position.z : " + base.transform.position.z, base.transform);
						Debug.Log("\t\tCharacter.Instance.z : " + Character.Instance.z, Character.Instance.transform);
					}
					return Game.ConvertUnityUnitsToGameMeters(base.transform.position.z - Character.Instance.z) >= PickupManager2.SpawnDistanceMinToCharacter;
				}
				return false;
			}
			return true;
		}

		protected void Awake()
		{
			_trackObject = GetComponent<TrackObject>();
			_trackObject.OnActivate += TrackObject_OnActivate;
			_trackObject.OnDeactivate += TrackObject_OnDeactivate;
			_snapObject = GetComponent<SnapObject>();
			_spawnPointMode_Original = SpawnPointMode;
			_forceSpawnPickupType_Original = ForceSpawnPickupType;
			_spawnPointDifficulty_Original = _spawnPointDifficulty;
			_spawnPointHeight_Original = _spawnPointHeight;
			InitializeAllValues();
		}

		protected void Start()
		{
			CheckForGizmos();
			ValidatePosition();
		}

		protected void OnDestroy()
		{
		}

		public void Migration_Replace2014SpawnPointWith2015SpawnPointForced(GameObject go, PickupType pickupType, SpawnPointDifficulty spawnPointDifficulty, bool willOffsetYPosition, bool willIgnoreZPositionBeforeSpawn)
		{
			InitializeAllValues();
			SetWillIgnoreZPositionBeforeSpawnAndOriginalTo(willIgnoreZPositionBeforeSpawn);
			SetSpawnPointModeAndOriginalTo(SpawnPointMode.WillForcePickupType);
			SetForceSpawnPickupTypeyAndOriginalTo(pickupType);
			if (SpawnPointMode == SpawnPointMode.WillForcePickupType && ForceSpawnPickupType == PickupType.PogoStick)
			{
				SetGameObjectName("DynamicPogoStick");
			}
			else
			{
				SetGameObjectName("Dynamic");
			}
			Migration_Configure(go, spawnPointDifficulty, willOffsetYPosition);
			_trackObject.Activate();
			ValidatePosition();
		}

		private void SetGameObjectName(string newName)
		{
			base.gameObject.name = "PickupSpawnPoint(" + newName + ")";
		}

		public void Migration_Replace2014SpawnPointWith2015SpawnPoint(GameObject go, SpawnPointDifficulty spawnPointDifficulty)
		{
			Migration_Configure(go, spawnPointDifficulty, true);
			SetGameObjectName("Replacement");
			_willInitializeWithOriginalPositionYZ = true;
			SetLocalPositionAndOriginalTo(base.transform.localPosition);
		}

		private void Migration_Configure(GameObject go, SpawnPointDifficulty spawnPointDifficulty, bool willOffsetYPosition)
		{
			RandomizeOffset component = go.GetComponent<RandomizeOffset>();
			RandomizeOffset component2 = base.gameObject.GetComponent<RandomizeOffset>();
			if (component != null && component2 != null)
			{
				component2.SetRandomOffsetsData(component.RandomOffsets);
			}
			if (willOffsetYPosition)
			{
				SetLocalPosition(base.transform.localPosition + new Vector3(0f, 7f, 0f));
			}
			SetLocalPositionAndOriginalTo(base.transform.localPosition);
			SetSpawnPointDifficultyAndOriginalTo(spawnPointDifficulty);
		}

		private void CheckForGizmos()
		{
			_debugBlockEasy.SetActive(false);
			_debugBlockEasy.GetComponent<Renderer>().enabled = false;
			_debugBlockMedium.SetActive(false);
			_debugBlockMedium.GetComponent<Renderer>().enabled = false;
			_debugBlockForce.SetActive(false);
			_debugBlockForce.GetComponent<Renderer>().enabled = false;
			if (_willShowGizmo)
			{
				if (SpawnPointMode == SpawnPointMode.WillForcePickupType)
				{
					_debugBlockForce.SetActive(true);
					_debugBlockForce.GetComponent<Renderer>().enabled = true;
				}
				else if (SpawnPointDifficulty == SpawnPointDifficulty.Easy)
				{
					_debugBlockEasy.SetActive(true);
					_debugBlockEasy.GetComponent<Renderer>().enabled = true;
				}
				else if (SpawnPointDifficulty == SpawnPointDifficulty.Medium)
				{
					_debugBlockMedium.SetActive(true);
					_debugBlockMedium.GetComponent<Renderer>().enabled = true;
				}
			}
		}

		public void DoRegister()
		{
			if (this.OnRegistered != null)
			{
				this.OnRegistered(this);
			}
		}

		public void DoDeregister()
		{
			DeactivateAndDespawnChild();
			if (this.OnDeregistered != null)
			{
				this.OnDeregistered(this);
			}
		}

		public void DoSpawnAPickup(PickupData2 pickupData, PickerData2 pickerData)
		{
			if (IsAvailableForSpawning())
			{
				SpawnPointState = SpawnPointState.Spawned;
				_pickupData = pickupData;
				Vector3 vector = new Vector3(0f, 0f, 0f);
				_spawnedPickupIcon = SingletonMonobehavior<PickupManager2>.Instance.Spawn(pickupData.PickupIconPrefab.transform, vector, Quaternion.identity).gameObject;
				HierarchyUtility.Reparent(_spawnedPickupIcon.transform, _pickupParent.transform, vector);
				_trackObject.Hack_PropagateActivationStateToChildrenImmediately();
				if (SpawnPointMode == SpawnPointMode.WillForcePickupType)
				{
				}
				if (this.OnPickupSpawned != null)
				{
					this.OnPickupSpawned(this, pickerData);
				}
			}
		}

		private void InitializeAllValues()
		{
			DeactivateAndDespawnChild();
			SpawnPointState = SpawnPointState.UnspawnedAndAvailable;
			SpawnPointMode = _spawnPointMode_Original;
			ForceSpawnPickupType = _forceSpawnPickupType_Original;
			WillIgnoreZPositionBeforeSpawn = _willIgnoreZPositionBeforeSpawn_Original;
			_spawnPointDifficulty = _spawnPointDifficulty_Original;
			_spawnPointHeight = _spawnPointHeight_Original;
			if (SpawnPointMode == SpawnPointMode.WillForcePickupType && ForceSpawnPickupType == PickupType.PogoStick)
			{
				_willInitializeWithOriginalPositionYZ = true;
			}
			if (_willInitializeWithOriginalPositionYZ)
			{
				if (IsAllowedPositionReset)
				{
					SetLocalPositionToOriginalPositionYZ();
				}
				else
				{
					Debug.LogWarning("TEST: Ignoring SetLocalPositionToOriginalPositionYZ(). We expect bugs now :)");
				}
			}
			_pickupData = null;
			CheckForGizmos();
		}

		private void DeactivateAndDespawnChild()
		{
			DeactivateChild();
			DespawnChild();
		}

		private void DeactivateChild()
		{
			if (_spawnedPickupIcon != null)
			{
				_spawnedPickupIcon.GetComponent<TrackObject>().Deactivate();
			}
		}

		private void DespawnChild()
		{
			if (_spawnedPickupIcon != null)
			{
				SingletonMonobehavior<PickupManager2>.Instance.Despawn(_spawnedPickupIcon.transform);
				_spawnedPickupIcon = null;
			}
			SpawnPointState = SpawnPointState.Despawned;
			if (this.OnPickupDespawned != null)
			{
				this.OnPickupDespawned(this);
			}
		}

		public void DoMarkAsUnavailable()
		{
			if (SpawnPointState == SpawnPointState.UnspawnedAndAvailable)
			{
				SpawnPointState = SpawnPointState.UnspawnedAndUnavailable;
				return;
			}
			throw new UnityException("New value not acceptable. Don't call method in this situation.");
		}

		public override string ToString()
		{
			return string.Format("[PickupSpawnPoint2: diff={0}, height={1}, PickupData={2}]", SpawnPointDifficulty, SpawnPointHeight, PickupData);
		}

		private void DebugLog(string message)
		{
			if (SingletonMonobehavior<PickupManager2>.IsInstantiated() && SingletonMonobehavior<PickupManager2>.Instance.WillDebugLogPickers)
			{
				Debug.Log(message);
			}
		}

		public void OnDespawned()
		{
		}

		public void OnSpawned()
		{
			InitializeAllValues();
			CheckForGizmos();
		}

		private void TrackObject_OnActivate()
		{
			InitializeAllValues();
			ValidatePosition();
			if (SingletonMonobehavior<PickupManager2>.IsInstantiated())
			{
				SingletonMonobehavior<PickupManager2>.Instance.RegisterPickupSpawnPoint(this);
			}
		}

		private void TrackObject_OnDeactivate()
		{
			if (SingletonMonobehavior<PickupManager2>.IsInstantiated())
			{
				SingletonMonobehavior<PickupManager2>.Instance.DeregisterPickupSpawnPoint(this);
			}
		}

		public void TriggerCollisionWithCharacter(CharacterPickupParticles characterPickupParticles)
		{
			if (SpawnPointState != SpawnPointState.SpawnedAndCollected)
			{
				SpawnPointState = SpawnPointState.SpawnedAndCollected;
				characterPickupParticles.PickedUpPowerUp();
				if (this.OnPickupCollected != null)
				{
					this.OnPickupCollected(this);
				}
				DeactivateAndDespawnChild();
			}
		}
	}
}
