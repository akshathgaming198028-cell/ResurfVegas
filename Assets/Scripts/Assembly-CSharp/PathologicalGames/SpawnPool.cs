using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PathologicalGames
{
	[AddComponentMenu("Path-o-logical/PoolManager/SpawnPool")]
	public sealed class SpawnPool : MonoBehaviour, IEnumerable, IList<Transform>, ICollection<Transform>, IEnumerable<Transform>
	{
		private sealed class GetEnumerator_003Ec__Iterator0 : IDisposable, IEnumerator, IEnumerator<object>
		{
			internal int _003Ci_003E__0;

			internal int _0024PC;

			internal object _0024current;

			internal SpawnPool _003C_003Ef__this;

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return _0024current;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return _0024current;
				}
			}

			public bool MoveNext()
			{
				uint num = (uint)_0024PC;
				_0024PC = -1;
				switch (num)
				{
				case 0u:
					_003Ci_003E__0 = 0;
					goto IL_0063;
				case 1u:
					{
						_003Ci_003E__0++;
						goto IL_0063;
					}
					IL_0063:
					if (_003Ci_003E__0 < _003C_003Ef__this._spawned.Count)
					{
						_0024current = _003C_003Ef__this._spawned[_003Ci_003E__0];
						_0024PC = 1;
						return true;
					}
					_0024PC = -1;
					break;
				}
				return false;
			}

			[DebuggerHidden]
			public void Dispose()
			{
				_0024PC = -1;
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}
		}

		private Transform _transform;

		private Transform _despawnedGroup;

		internal Dictionary<GameObject, PrefabPool> _dictSpawnedObjToPool = new Dictionary<GameObject, PrefabPool>();

		internal Dictionary<GameObject, PrefabPool> _dictDespawnedObjToPool = new Dictionary<GameObject, PrefabPool>();

		private Dictionary<GameObject, PrefabPool> _dictPrefabToPool = new Dictionary<GameObject, PrefabPool>();

		internal static readonly Vector3 DEFAULT_GROUP_LOCAL_POSITION = Vector3.zero;

		public string poolName = string.Empty;

		public bool matchPoolScale;

		public bool matchPoolLayer;

		public bool dontReparent;

		public bool _dontDestroyOnLoad;

		public bool logMessages;

		public List<PrefabPool> _perPrefabPoolOptions = new List<PrefabPool>();

		public Dictionary<object, bool> prefabsFoldOutStates = new Dictionary<object, bool>();

		public int itemCount;

		public float maxParticleDespawnTime = 300f;

		public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();

		private List<PrefabPool> _prefabPools = new List<PrefabPool>();

		internal List<Transform> _spawned = new List<Transform>();

		public bool dontDestroyOnLoad
		{
			get
			{
				return _dontDestroyOnLoad;
			}
			set
			{
				_dontDestroyOnLoad = value;
				if (group != null)
				{
					UnityEngine.Object.DontDestroyOnLoad(group.gameObject);
				}
			}
		}

		public Transform group { get; private set; }

		public Dictionary<string, PrefabPool> prefabPools
		{
			get
			{
				Dictionary<string, PrefabPool> dictionary = new Dictionary<string, PrefabPool>();
				for (int i = 0; i < _prefabPools.Count; i++)
				{
					dictionary[_prefabPools[i].prefabGO.name] = _prefabPools[i];
				}
				return dictionary;
			}
		}

		public Transform this[int index]
		{
			get
			{
				return _spawned[index];
			}
			set
			{
				throw new NotImplementedException("Read-only.");
			}
		}

		public int Count
		{
			get
			{
				return _spawned.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			GetEnumerator_003Ec__Iterator0 getEnumerator_003Ec__Iterator = new GetEnumerator_003Ec__Iterator0();
			getEnumerator_003Ec__Iterator._003C_003Ef__this = this;
			return getEnumerator_003Ec__Iterator;
		}

		bool ICollection<Transform>.Remove(Transform item)
		{
			throw new NotImplementedException();
		}

		private void Awake()
		{
			if (_dontDestroyOnLoad)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			_transform = base.transform;
			group = new GameObject("spawned").transform;
			group.parent = _transform;
			group.localPosition = DEFAULT_GROUP_LOCAL_POSITION;
			_despawnedGroup = new GameObject("despawned").transform;
			_despawnedGroup.parent = _transform;
			_despawnedGroup.localPosition = Vector3.zero;
			if (poolName == string.Empty)
			{
				poolName = base.name.Replace("Pool", string.Empty);
				poolName = poolName.Replace("(Clone)", string.Empty);
			}
			if (logMessages)
			{
				Debug.Log(string.Format("SpawnPool {0}: Initializing..", poolName));
			}
			for (int i = 0; i < _perPrefabPoolOptions.Count; i++)
			{
				if (_perPrefabPoolOptions[i].prefab == null)
				{
					Debug.LogWarning(string.Format("Initialization Warning: Pool '{0}' contains a PrefabPool with no prefab reference. Skipping.", poolName));
					continue;
				}
				_perPrefabPoolOptions[i].inspectorInstanceConstructor();
				CreatePrefabPool(_perPrefabPoolOptions[i]);
			}
			PoolManager.Pools.Add(this);
		}

		private void Update()
		{
			itemCount = _spawned.Count;
		}

		private void OnDestroy()
		{
			if (logMessages)
			{
				Debug.Log(string.Format("SpawnPool {0}: Destroying...", poolName));
			}
			PoolManager.Pools.Remove(this);
			StopAllCoroutines();
			_spawned.Clear();
			foreach (PrefabPool prefabPool in _prefabPools)
			{
				prefabPool.SelfDestruct();
			}
			_prefabPools.Clear();
		}

		public void CreatePrefabPool(PrefabPool prefabPool)
		{
			PrefabPool prefabPool2 = GetPrefabPool(prefabPool.prefab);
			bool flag = prefabPool2 != null;
			if (!flag)
			{
				prefabPool.spawnPool = this;
				_prefabPools.Add(prefabPool);
				_dictPrefabToPool.Add(prefabPool.prefabGO, prefabPool);
			}
			if (flag && prefabPool != prefabPool2)
			{
				Debug.LogWarning("Attempting to create a second pool of same prefab - " + prefabPool.prefab.name);
			}
			else if (!prefabPool.preloaded)
			{
				if (logMessages)
				{
					Debug.Log(string.Format("SpawnPool {0}: Preloading {1} {2}", poolName, prefabPool.preloadAmount, prefabPool.prefab.name));
				}
				prefabPool.PreloadInstances();
			}
		}

		public bool ContainsPool(GameObject prefab)
		{
			return _dictPrefabToPool.ContainsKey(prefab);
		}

		public void Add(Transform instance, string prefabName, bool despawn, bool parent)
		{
			for (int i = 0; i < _prefabPools.Count; i++)
			{
				if (_prefabPools[i].prefabGO == null)
				{
					Debug.LogError("Unexpected Error: PrefabPool.prefabGO is null");
					return;
				}
				if (_prefabPools[i].prefabGO.name == prefabName)
				{
					_prefabPools[i].AddUnpooled(instance, despawn);
					if (logMessages)
					{
						Debug.Log(string.Format("SpawnPool {0}: Adding previously unpooled instance {1}", poolName, instance.name));
					}
					if (parent)
					{
						instance.parent = group;
					}
					if (!despawn)
					{
						_spawned.Add(instance);
					}
					return;
				}
			}
			Debug.LogError(string.Format("SpawnPool {0}: PrefabPool {1} not found.", poolName, prefabName));
		}

		public void Add(Transform item)
		{
			string message = "Use SpawnPool.Spawn() to properly add items to the pool.";
			throw new NotImplementedException(message);
		}

		public void Remove(Transform item)
		{
			string message = "Use Despawn() to properly manage items that should remain in the pool but be deactivated.";
			throw new NotImplementedException(message);
		}

		public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot, Transform parent)
		{
			PrefabPool value;
			Transform transform;
			if (_dictPrefabToPool.TryGetValue(prefab.gameObject, out value))
			{
				transform = value.SpawnInstance(pos, rot);
				if (transform == null)
				{
					return null;
				}
				if (parent != null)
				{
					transform.parent = parent;
				}
				else if (!dontReparent && transform.parent != group)
				{
					transform.parent = group;
				}
				_spawned.Add(transform);
				if (value.sendLifecycleMessages)
				{
					transform.gameObject.BroadcastMessage("OnSpawned", this, SendMessageOptions.DontRequireReceiver);
				}
				return transform;
			}
			PrefabPool prefabPool = new PrefabPool(prefab);
			CreatePrefabPool(prefabPool);
			transform = prefabPool.SpawnInstance(pos, rot);
			if (parent != null)
			{
				transform.parent = parent;
			}
			else
			{
				transform.parent = group;
			}
			_spawned.Add(transform);
			if (prefabPool.sendLifecycleMessages)
			{
				transform.gameObject.BroadcastMessage("OnSpawned", this, SendMessageOptions.DontRequireReceiver);
			}
			return transform;
		}

		public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot)
		{
			Transform transform = Spawn(prefab, pos, rot, null);
			if (transform == null)
			{
				return null;
			}
			return transform;
		}

		public Transform Spawn(Transform prefab)
		{
			return Spawn(prefab, Vector3.zero, Quaternion.identity);
		}

		public Transform Spawn(Transform prefab, Transform parent)
		{
			return Spawn(prefab, Vector3.zero, Quaternion.identity, parent);
		}

		public AudioSource Spawn(AudioSource prefab, Vector3 pos, Quaternion rot)
		{
			return Spawn(prefab, pos, rot, null);
		}

		public AudioSource Spawn(AudioSource prefab)
		{
			return Spawn(prefab, Vector3.zero, Quaternion.identity, null);
		}

		public AudioSource Spawn(AudioSource prefab, Transform parent)
		{
			return Spawn(prefab, Vector3.zero, Quaternion.identity, parent);
		}

		public AudioSource Spawn(AudioSource prefab, Vector3 pos, Quaternion rot, Transform parent)
		{
			Transform transform = Spawn(prefab.transform, pos, rot, parent);
			if (transform == null)
			{
				return null;
			}
			AudioSource component = transform.GetComponent<AudioSource>();
			component.Play();
			StartCoroutine(ListForAudioStop(component));
			return component;
		}

		public ParticleSystem Spawn(ParticleSystem prefab, Vector3 pos, Quaternion rot)
		{
			return Spawn(prefab, pos, rot, null);
		}

		public ParticleSystem Spawn(ParticleSystem prefab, Vector3 pos, Quaternion rot, Transform parent)
		{
			Transform transform = Spawn(prefab.transform, pos, rot, parent);
			if (transform == null)
			{
				return null;
			}
			ParticleSystem component = transform.GetComponent<ParticleSystem>();
			StartCoroutine(ListenForEmitDespawn(component));
			return component;
		}

		public ParticleEmitter Spawn(ParticleEmitter prefab, Vector3 pos, Quaternion rot)
		{
			Transform transform = Spawn(prefab.transform, pos, rot);
			if (transform == null)
			{
				return null;
			}
			ParticleAnimator component = transform.GetComponent<ParticleAnimator>();
			if (component != null)
			{
				component.autodestruct = false;
			}
			ParticleEmitter component2 = transform.GetComponent<ParticleEmitter>();
			component2.emit = true;
			StartCoroutine(ListenForEmitDespawn(component2));
			return component2;
		}

		public ParticleEmitter Spawn(ParticleEmitter prefab, Vector3 pos, Quaternion rot, string colorPropertyName, Color color)
		{
			Transform transform = Spawn(prefab.transform, pos, rot);
			if (transform == null)
			{
				return null;
			}
			ParticleAnimator component = transform.GetComponent<ParticleAnimator>();
			if (component != null)
			{
				component.autodestruct = false;
			}
			ParticleEmitter component2 = transform.GetComponent<ParticleEmitter>();
			component2.GetComponent<Renderer>().material.SetColor(colorPropertyName, color);
			component2.emit = true;
			StartCoroutine(ListenForEmitDespawn(component2));
			return component2;
		}

		public void Despawn(Transform instance)
		{
			bool flag = false;
			PrefabPool value;
			if (_dictSpawnedObjToPool.TryGetValue(instance.gameObject, out value))
			{
				flag = value.DespawnInstance(instance);
			}
			else if (_dictDespawnedObjToPool.TryGetValue(instance.gameObject, out value))
			{
				Debug.LogError(string.Format("SpawnPool {0}: {1} has already been despawned. You cannot despawn something more than once!", poolName, GetLongName(instance)));
				return;
			}
			if (!flag)
			{
				Debug.LogError(string.Format("SpawnPool {0}: {1} not found in SpawnPool", poolName, instance.name));
				return;
			}
			Vector3 localScale = instance.localScale;
			Quaternion localRotation = instance.localRotation;
			instance.parent = _despawnedGroup;
			instance.localScale = localScale;
			instance.localRotation = localRotation;
			instance.localPosition = Vector3.zero;
			_spawned.Remove(instance);
			if (value.sendLifecycleMessages)
			{
				instance.gameObject.BroadcastMessage("OnDespawnedCompleted", value, SendMessageOptions.DontRequireReceiver);
			}
		}

		public void Despawn(Transform instance, Transform parent)
		{
			instance.parent = parent;
			Despawn(instance);
		}

		public void Despawn(Transform instance, float seconds)
		{
			StartCoroutine(DoDespawnAfterSeconds(instance, seconds, false, null));
		}

		public void Despawn(Transform instance, float seconds, Transform parent)
		{
			StartCoroutine(DoDespawnAfterSeconds(instance, seconds, true, parent));
		}

		private IEnumerator DoDespawnAfterSeconds(Transform instance, float seconds, bool useParent, Transform parent)
		{
			GameObject go = instance.gameObject;
			while (seconds > 0f)
			{
				yield return null;
				if (!go.activeInHierarchy)
				{
					yield break;
				}
				seconds -= Time.deltaTime;
			}
			if (IsSpawned(instance))
			{
				if (useParent)
				{
					Despawn(instance, parent);
				}
				else
				{
					Despawn(instance);
				}
			}
		}

		public void DespawnAll()
		{
			List<Transform> list = new List<Transform>(_spawned);
			for (int i = 0; i < list.Count; i++)
			{
				Despawn(list[i]);
			}
		}

		public bool IsSpawned(Transform instance)
		{
			return _spawned.Contains(instance);
		}

		public PrefabPool GetPrefabPool(Transform prefab)
		{
			return GetPrefabPool(prefab.gameObject);
		}

		public PrefabPool GetPrefabPool(GameObject prefab)
		{
			for (int i = 0; i < _prefabPools.Count; i++)
			{
				if (_prefabPools[i].prefabGO == null)
				{
					Debug.LogError(string.Format("SpawnPool {0}: PrefabPool.prefabGO is null", poolName));
				}
				if (_prefabPools[i].prefabGO == prefab)
				{
					return _prefabPools[i];
				}
			}
			return null;
		}

		public Transform GetPrefab(Transform instance)
		{
			PrefabPool value;
			if (_dictSpawnedObjToPool.TryGetValue(instance.gameObject, out value))
			{
				return value.prefab;
			}
			return null;
		}

		private IEnumerator ListForAudioStop(AudioSource src)
		{
			yield return null;
			while (src.isPlaying)
			{
				yield return null;
			}
			Despawn(src.transform);
		}

		private IEnumerator ListenForEmitDespawn(ParticleEmitter emitter)
		{
			yield return null;
			yield return new WaitForEndOfFrame();
			float safetimer = 0f;
			while (emitter.particleCount > 0)
			{
				safetimer += Time.deltaTime;
				if (safetimer > maxParticleDespawnTime)
				{
					Debug.LogWarning(string.Format("SpawnPool {0}: Timed out while listening for all particles to die. Waited for {1}sec.", poolName, maxParticleDespawnTime));
				}
				yield return null;
			}
			emitter.emit = false;
			if (IsSpawned(emitter.transform))
			{
				Despawn(emitter.transform);
			}
		}

		private IEnumerator ListenForEmitDespawn(ParticleSystem emitter)
		{
			yield return new WaitForSeconds(emitter.startDelay + 0.25f);
			float safetimer = 0f;
			while (emitter.IsAlive(true))
			{
				if (!PoolManagerUtils.activeInHierarchy(emitter.gameObject))
				{
					emitter.Clear(true);
					yield break;
				}
				safetimer += Time.deltaTime;
				if (safetimer > maxParticleDespawnTime)
				{
					Debug.LogWarning(string.Format("SpawnPool {0}: Timed out while listening for all particles to die. Waited for {1}sec.", poolName, maxParticleDespawnTime));
				}
				yield return null;
			}
			if (IsSpawned(emitter.transform))
			{
				Despawn(emitter.transform);
			}
		}

		public override string ToString()
		{
			List<string> list = new List<string>();
			foreach (Transform item in _spawned)
			{
				list.Add(item.name);
			}
			return string.Join(", ", list.ToArray());
		}

		public bool Contains(Transform item)
		{
			string message = "Use IsSpawned(Transform instance) instead.";
			throw new NotImplementedException(message);
		}

		public void CopyTo(Transform[] array, int arrayIndex)
		{
			_spawned.CopyTo(array, arrayIndex);
		}

		public IEnumerator<Transform> GetEnumerator()
		{
			for (int i = 0; i < _spawned.Count; i++)
			{
				yield return _spawned[i];
			}
		}

		public int IndexOf(Transform item)
		{
			throw new NotImplementedException();
		}

		public void Insert(int index, Transform item)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		private static string GetLongName(Transform transform)
		{
			return (!(transform == null)) ? (GetLongName(transform.parent) + "/" + transform.name) : string.Empty;
		}
	}
}
