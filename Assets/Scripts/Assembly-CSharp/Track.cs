using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SYBO.Core;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities;
using SYBO.Subway.Characters;
using SYBO.Subway.Characters.States;
using SYBO.Subway.Level;
using UnityEngine;

public class Track : MonoBehaviour, ChunkStateChangePositionProvider
{
	private struct SelectorWrapper
	{
		public GameObject GameObject;

		public float z;

		public Selector Selector;

		public float Z
		{
			get
			{
				return z;
			}
		}

		public SelectorWrapper(Selector selector, GameObject gameObject)
		{
			Selector = selector;
			GameObject = gameObject;
			z = gameObject.transform.position.z;
		}
	}

	public const float TrackAheadActivatedDistance = 1000f;

	private const int NumSelectGameObjectsPerFrame = 15;

	private const int NumActivateGameObjectsPerFrame = 40;

	private const int NumActivateTrackObjectsPerFrame = 10;

	public Transform trackLeft;

	public Transform trackRight;

	public int numberOfTracks = 3;

	public float cleanUpDistance = 400f;

	public float trackAheadDistance = 1500f;

	[OptionalField]
	public Transform levelChunksParent;

	[OptionalField]
	public TrackChunk jetpackLandingChunk;

	[OptionalField]
	public TrackChunk[] TurboHeadstartLandingChunk;

	public bool tutorial;

	[OptionalField]
	public TrackChunk tutorialTrackChunk;

	[SerializeField]
	private bool _environmentDebugChanges;

	[SerializeField]
	private bool _environmentRepeat = true;

	[SerializeField]
	private EnvironmentTypeRepeatSettings[] environmentRepeatSettings;

	private Dictionary<EnvironmentType, float> environmentRepeatSettingsDict = new Dictionary<EnvironmentType, float>();

	private bool firstTrackChunk = true;

	private TrackChunkCollection trackChunks;

	private float trackSpacing;

	private float trackChunkZ;

	private List<TrackChunk> chunksOnTrack = new List<TrackChunk>(10);

	private Character _character;

	private Hoverboard hoverboard;

	private static Track instance;

	private JetpackState jetpack;

	private EnvironmentType _previousEnvirnomentType;

	private EnvironmentType _currentEnvirnomentType;

	private float _currentEnvironmentTypeStartZ;

	private float _currentEnvironmentTypeEndZ;

	private float _environmentRepeatDistance = 3000f;

	public bool IsRunningOnTutorialTrack { get; set; }

	public float EnvironmentRepeatDistance
	{
		get
		{
			return _environmentRepeatDistance;
		}
	}

	public bool EnvironmentRepeat
	{
		get
		{
			return _environmentRepeat;
		}
	}

	public float RequiredActivationZ
	{
		get
		{
			return 1000f + ((!(_character != null)) ? 0f : _character.z);
		}
	}

	public float RequiredDeactivationZ
	{
		get
		{
			return (!(_character != null)) ? 0f : (_character.z - 2000f);
		}
	}

	public float TrackBuiltDistance
	{
		get
		{
			TrackChunk trackChunk = chunksOnTrack.LastOrDefault();
			return (!(trackChunk == null)) ? trackChunk.transform.position.z : 0f;
		}
	}

	public static Track Instance
	{
		get
		{
			return instance ?? (instance = Object.FindObjectOfType(typeof(Track)) as Track);
		}
	}

	public void Awake()
	{
		trackSpacing = (trackRight.position - trackLeft.position).magnitude / (float)(numberOfTracks - 1);
		trackChunks = new TrackChunkCollection();
		hoverboard = Hoverboard.Instance;
		_character = Character.Instance;
		jetpack = JetpackState.Instance;
		tutorial = !PlayerInfo.Instance.IsTutorialCompleted;
		for (int i = 0; i < environmentRepeatSettings.Length; i++)
		{
			EnvironmentTypeRepeatSettings environmentTypeRepeatSettings = environmentRepeatSettings[i];
			environmentRepeatSettingsDict[environmentTypeRepeatSettings.Environment] = environmentTypeRepeatSettings.RepeatDistance;
		}
		SingletonMonobehavior<WorkLoadQueue>.Instantiate();
	}

	public Vector3 GetPosition(float x, float z)
	{
		return Vector3.forward * z + trackLeft.position + x * Vector3.right;
	}

	public float GetTrackX(int trackIndex)
	{
		return trackSpacing * (float)trackIndex;
	}

	public float LayJetpackChunks(float characterZ, float flyLength)
	{
		LayTracksUpTo(characterZ, flyLength, true, 0);
		float result = trackChunkZ - characterZ;
		LayTrackChunk(jetpackLandingChunk);
		return result;
	}

	public float LayTurboHeadstartChunks(float characterZ, float flyLength, int version)
	{
		LayTracksUpTo(characterZ, flyLength, false, version + 1);
		float result = trackChunkZ - characterZ;
		LayTrackChunk(TurboHeadstartLandingChunk[version]);
		return result;
	}

	public void LayEmptyChunks(float characterZ, float removeDistance)
	{
		RemoveChunkObstacles(characterZ + removeDistance);
	}

	private void RemoveChunkObstacles(float removeDistance)
	{
		foreach (TrackChunk item in chunksOnTrack)
		{
			item.DeactivateObstacles(removeDistance);
		}
	}

	public void LayTrackChunks(float characterZ, bool activate = true)
	{
		LayTracksUpTo(characterZ, trackAheadDistance, false, 0, activate);
	}

	private void LayTracksUpTo(float characterZ, float trackAheadDistance, bool isJetpack, int turboHeadstart, bool activate = true)
	{
		if (trackChunks.CanDeliver())
		{
			float num = characterZ + trackAheadDistance;
			float num2 = 200f;
			Debug.DrawLine(Vector3.forward * num + Vector3.left * num2, Vector3.forward * num + -Vector3.left * num2, Color.white);
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			while (trackChunkZ < num)
			{
				trackChunks.FilterAvailableForward(trackChunkZ);
				TrackChunk trackChunk;
				if (firstTrackChunk && tutorial)
				{
					trackChunk = tutorialTrackChunk;
					firstTrackChunk = false;
					if (trackChunk.CheckPoints.Count > 0)
					{
						IsRunningOnTutorialTrack = true;
					}
					hoverboard.IsAllowed = false;
				}
				else if (isJetpack)
				{
					trackChunk = trackChunks.GetJetPakChunk(num3);
					num3++;
				}
				else
				{
					switch (turboHeadstart)
					{
					case 1:
						trackChunk = trackChunks.GetTurboHeadstart1Chunk(num4);
						num4++;
						break;
					case 2:
						trackChunk = trackChunks.GetTurboHeadstart2Chunk(num5);
						num5++;
						break;
					default:
					{
						trackChunk = trackChunks.GetRandom();
						int num6 = 0;
						while ((chunksOnTrack.Contains(trackChunk) || trackChunk.Deactivating) && num6 < 1000)
						{
							trackChunk = trackChunks.GetRandom();
							num6++;
						}
						if (num6 == 1000)
						{
							Debug.Log("active track chunks");
							Debug.Log("active: " + string.Join(", ", chunksOnTrack.ConvertAll((TrackChunk chunk) => chunk.gameObject.name).ToArray()));
							Debug.LogError("infinite loop. not track chunks to select.");
						}
						break;
					}
					}
				}
				LayTrackChunk(trackChunk, activate);
			}
		}
		if (0 < chunksOnTrack.Count && chunksOnTrack[0].ZWorldEnd < characterZ && turboHeadstart == 0)
		{
			RemoveTrackChunks(characterZ);
		}
	}

	private void LayTrackChunk(TrackChunk trackChunk, bool activate = true)
	{
		TrackChunk trackChunk2 = null;
		if (0 < chunksOnTrack.Count)
		{
			trackChunk2 = chunksOnTrack[chunksOnTrack.Count - 1];
		}
		if (trackChunk2 != null)
		{
			trackChunk2.Next = trackChunk;
		}
		trackChunk.Previous = trackChunk2;
		chunksOnTrack.Add(trackChunk);
		trackChunk.Transform.position = Vector3.forward * trackChunkZ;
		trackChunkZ += trackChunk.zSize;
		if (trackChunk.zMaximum == -1f)
		{
			jetpack.PlacePickups(trackChunk.Transform.position.z);
		}
		trackChunk.RestoreHiddenObstacles();
		SingletonMonobehavior<WorkLoadQueue>.Instance.EnqueueWork(ChunkRecursiveSelectAndActivateAsync(trackChunk, activate));
	}

	private IEnumerator ChunkRecursiveSelectAndActivateAsync(TrackChunk trackChunk, bool activate = true)
	{
		yield return PerformRecursiveSelection(trackChunk.gameObject, activate);
		if (activate)
		{
			yield return trackChunk.Activate(10, this);
		}
	}

	public void Activate()
	{
		SingletonMonobehavior<WorkLoadQueue>.Instance.EnqueueWork(ActivateTrackChunksAsync());
	}

	private IEnumerator ActivateTrackChunksAsync()
	{
		for (int i = 0; i < chunksOnTrack.Count; i++)
		{
			TrackChunk trackChunk = chunksOnTrack[i];
			yield return trackChunk.Activate(10);
		}
	}

	private IEnumerator PerformRecursiveSelection(GameObject parent, bool checkRequiredActivationPos = true, bool sortSpawnPoints = true)
	{
		List<GameObjectPositionWrapper> objectsToActivate = new List<GameObjectPositionWrapper>();
		List<GameObject> objectsToVisit = new List<GameObject>();
		List<SelectorWrapper> spawnPoints = new List<SelectorWrapper>();
		int numSelected = 0;
		objectsToVisit.Add(parent);
		GameObject currGameObject = null;
		GameObject prevGameObject = null;
		string prevGameObjectName = null;
		while (0 < objectsToVisit.Count)
		{
			prevGameObject = currGameObject;
			if (prevGameObject != null)
			{
				prevGameObjectName = prevGameObject.name;
			}
			currGameObject = objectsToVisit[0];
			objectsToVisit.RemoveAt(0);
			if (currGameObject == null)
			{
				if (Application.isEditor)
				{
					string prevGO = ((!(prevGameObject != null)) ? (prevGameObjectName + " (destroyed already)") : prevGameObject.GetLongName());
					Debug.LogWarning("Got destroyed object in selection list. Previous was - " + prevGO);
				}
				continue;
			}
			Selector selector = currGameObject.GetComponent(typeof(Selector)) as Selector;
			if (sortSpawnPoints)
			{
				SpawnPoint spawnPoint = selector as SpawnPoint;
				if (spawnPoint != null)
				{
					spawnPoints.Add(new SelectorWrapper(spawnPoint, currGameObject));
					continue;
				}
				SpawnPointEgg eggSpawn = selector as SpawnPointEgg;
				if (eggSpawn != null)
				{
					spawnPoints.Add(new SelectorWrapper(eggSpawn, eggSpawn.gameObject));
					continue;
				}
			}
			RandomizeOffset randomizeOffset = currGameObject.GetComponent(typeof(RandomizeOffset)) as RandomizeOffset;
			if (randomizeOffset != null)
			{
				randomizeOffset.ChooseRandomOffset();
			}
			Transform gameObjectTransform = currGameObject.transform;
			objectsToActivate.Add(new GameObjectPositionWrapper(currGameObject));
			if (selector != null)
			{
				selector.PerformSelection(objectsToVisit);
				numSelected++;
				if (15 <= numSelected)
				{
					yield return null;
					numSelected = 0;
				}
			}
			else
			{
				for (int i = 0; i < gameObjectTransform.childCount; i++)
				{
					GameObject child = gameObjectTransform.GetChild(i).gameObject;
					objectsToVisit.Add(child);
				}
			}
		}
		int j = 0;
		foreach (GameObjectPositionWrapper goWrapper in objectsToActivate)
		{
			if (goWrapper.GameObject != null)
			{
				goWrapper.GameObject.SetActive(true);
			}
			j++;
			bool yieldActivation = 40 <= j;
			if (checkRequiredActivationPos && goWrapper.Position.z < RequiredActivationZ)
			{
				yieldActivation = false;
			}
			if (yieldActivation)
			{
				yield return null;
				j = 0;
			}
		}
		if (0 >= spawnPoints.Count)
		{
			yield break;
		}
		spawnPoints.Sort((SelectorWrapper x, SelectorWrapper y) => x.Z.CompareTo(y.Z));
		objectsToVisit.Clear();
		foreach (SelectorWrapper item in spawnPoints)
		{
			item.Selector.PerformSelection(objectsToVisit);
		}
		foreach (GameObject o in objectsToVisit)
		{
			o.SetActive(true);
		}
	}

	private bool IsLowPriority(GameObject g)
	{
		return g.layer != Layers.Instance.KeepOnHoverboard;
	}

	public void RemoveTrackChunks(float characterZ)
	{
		float cleanupZ = characterZ - cleanUpDistance;
		SingletonMonobehavior<WorkLoadQueue>.Instance.EnqueueWork(RemoveTrackChunksAsync(cleanupZ));
	}

	private IEnumerator RemoveTrackChunksAsync(float cleanupZ)
	{
		int i = 0;
		while (i < chunksOnTrack.Count && chunksOnTrack[i].ZWorldEnd < cleanupZ && !chunksOnTrack[i].Deactivating)
		{
			yield return chunksOnTrack[i].DeactivateAsync(this);
			chunksOnTrack.RemoveAt(i);
		}
	}

	public void RemoveTrackChunks()
	{
		StopAllCoroutines();
		for (int i = 0; i < chunksOnTrack.Count; i++)
		{
			TrackChunk trackChunk = chunksOnTrack[i];
			trackChunk.Deactivate();
		}
		chunksOnTrack.Clear();
		trackChunkZ = 0f;
		firstTrackChunk = true;
	}

	public void Reset()
	{
		RemoveTrackChunks();
		trackChunks.FilterAvailableInitial(0f);
		_currentEnvironmentTypeStartZ = 0f;
		_currentEnvironmentTypeEndZ = 0f;
	}

	private TrackObject[] _DoRemoveNullValuesFromTrackObjectArray(TrackObject[] trackObjects)
	{
		List<TrackObject> list = new List<TrackObject>();
		foreach (TrackObject trackObject in trackObjects)
		{
			if (trackObject != null)
			{
				list.Add(trackObject);
			}
		}
		return Enumerable.ToArray(list);
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Vector3 center = new Vector3(0f, 200f, RequiredActivationZ);
		Vector3 size = new Vector3(400f, 400f, 0f);
		Gizmos.DrawWireCube(center, size);
	}

	public float GetLastCheckPoint(float characterZ)
	{
		foreach (TrackChunk item in chunksOnTrack)
		{
			if (IsRunningOnTutorialTrack && item == tutorialTrackChunk)
			{
				return item.GetLastCheckPoint(characterZ);
			}
		}
		Debug.Log("No checkpoints in track");
		return 0f;
	}

	public EnvironmentType GetEnvironmentType(float startPosition)
	{
		if (startPosition < _currentEnvironmentTypeStartZ)
		{
			return _previousEnvirnomentType;
		}
		if (startPosition < _currentEnvironmentTypeEndZ)
		{
			return _currentEnvirnomentType;
		}
		return EnvironmentType.None;
	}

	public EnvironmentType GetPreviousEnvironmentType(float startPosition)
	{
		if (startPosition < _currentEnvironmentTypeStartZ)
		{
			return EnvironmentType.None;
		}
		if (startPosition < _currentEnvironmentTypeEndZ)
		{
			return _previousEnvirnomentType;
		}
		return _currentEnvirnomentType;
	}

	public void SetEnvironmentType(float startPositionZ, EnvironmentType environment)
	{
		if (_environmentRepeat && _currentEnvirnomentType != EnvironmentType.None && startPositionZ < _currentEnvironmentTypeEndZ)
		{
			if (Application.isEditor)
			{
				Vector3 vector = new Vector3(0f, 0f, startPositionZ);
				Debug.DrawLine(vector - Vector3.right * 500f, vector + Vector3.right * 500f, Color.cyan, 10000f);
				Debug.LogError(string.Concat("Tried to change environment to ", environment, " type at ", startPositionZ, " before current ", _currentEnvirnomentType, " has ended at ", _currentEnvironmentTypeStartZ, " - ", _currentEnvironmentTypeEndZ, " !. Change only if current type is None"));
			}
		}
		else
		{
			_previousEnvirnomentType = _currentEnvirnomentType;
			_currentEnvirnomentType = environment;
			_currentEnvironmentTypeStartZ = startPositionZ;
			if (_currentEnvirnomentType != EnvironmentType.None)
			{
				_currentEnvironmentTypeEndZ = startPositionZ + environmentRepeatSettingsDict[_currentEnvirnomentType];
			}
			else
			{
				_currentEnvironmentTypeEndZ = float.MaxValue;
			}
			if (Application.isEditor)
			{
				Vector3 vector2 = new Vector3(0f, 0f, startPositionZ);
				Debug.DrawLine(vector2, vector2 + Vector3.up * 400f, Color.white, 10000f);
			}
			if (_previousEnvirnomentType == EnvironmentType.None)
			{
				_previousEnvirnomentType = _currentEnvirnomentType;
				_currentEnvironmentTypeStartZ = startPositionZ;
				if (Application.isEditor)
				{
					Vector3 vector3 = new Vector3(0f, 0f, startPositionZ);
					Debug.DrawLine(vector3, vector3 + Vector3.up * 500f, Color.red, 10000f);
				}
			}
		}
		if (Application.isEditor && _environmentDebugChanges)
		{
			Debug.LogFormat("Change Environment from {0} to {1} at {2} range {3} - {4}", _previousEnvirnomentType, _currentEnvirnomentType, startPositionZ, _currentEnvironmentTypeStartZ, _currentEnvironmentTypeEndZ);
		}
	}
}
