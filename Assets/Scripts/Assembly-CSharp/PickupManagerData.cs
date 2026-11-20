using System;
using System.Collections.Generic;
using SYBO.Core.Utilities;
using SYBO.Subway;
using SYBO.Subway.Pickups;
using SYBO.Subway.Pickups.Migration;
using UnityEngine;

[Serializable]
public class PickupManagerData
{
	public enum SpawnPrefabType
	{
		HuntToken = 0,
		SuperMysteryBox = 1,
		None = 2
	}

	public GameObject prefab;

	public bool canSpawn = true;

	public float SPAWN_MINIMUM_TIME;

	public float SPAWN_MAXIMUM_TIME;

	public float SPAWN_PROBABILTY;

	public SpawnPrefabType SPAWN_PREFAB_TYPE = SpawnPrefabType.None;

	private float lastSpawnPosition;

	private List<Transform> _activePrefabs = new List<Transform>();

	private List<Transform> _inActivePrefabs = new List<Transform>();

	private Transform _inactiveObjectsRoot;

	public void Initialize(Transform activeObjectsRoot, Transform inactiveObjectsRoot)
	{
		_inactiveObjectsRoot = inactiveObjectsRoot;
		Game.OnGameStarted = (Action)Delegate.Combine(Game.OnGameStarted, new Action(OnNewGameStarted));
	}

	private bool CalculateSpawn(Transform position)
	{
		bool result = false;
		float num = Game.Instance.currentSpeed;
		if (num <= 0f)
		{
			num = Game.Instance.speed.min;
		}
		if (position.position.z < lastSpawnPosition + SPAWN_MINIMUM_TIME * num)
		{
			result = false;
		}
		else if (position.position.z > lastSpawnPosition + SPAWN_MAXIMUM_TIME * num)
		{
			result = true;
		}
		else if (UnityEngine.Random.value <= SPAWN_PROBABILTY)
		{
			result = true;
		}
		return result;
	}

	private void InstatiateSpawn(Transform position)
	{
		GameObject gameObject = null;
		if (_inActivePrefabs.Count == 0)
		{
			if (prefab != null)
			{
				gameObject = UnityEngine.Object.Instantiate(prefab, position.position, position.rotation) as GameObject;
				gameObject.transform.parent = position;
				gameObject.transform.ResetLocalPosition();
				_activePrefabs.Add(gameObject.transform);
				gameObject.GetComponent<PickupController>().Activate();
			}
			else
			{
				Debug.LogWarning("The hunt prefab is corrupt!");
			}
		}
		else
		{
			gameObject = _inActivePrefabs[0].gameObject;
			gameObject.transform.parent = position;
			gameObject.transform.ResetLocalPosition();
			_inActivePrefabs.Remove(gameObject.transform);
			_activePrefabs.Add(gameObject.transform);
			gameObject.GetComponent<PickupController>().Activate();
		}
		switch (SPAWN_PREFAB_TYPE)
		{
		case SpawnPrefabType.HuntToken:
			PickupsMigration.Log2014PickupSpawnInNewLogger(PickupType.TokenHunt);
			break;
		case SpawnPrefabType.SuperMysteryBox:
			PickupsMigration.Log2014PickupSpawnInNewLogger(PickupType.SuperMysteryBox);
			break;
		case SpawnPrefabType.None:
			Debug.LogError("Legacy Pickup Spawn System spawning wrong type. 1");
			break;
		default:
			Debug.LogError("Legacy Pickup Spawn System spawning wrong type. 2");
			break;
		}
		if (Globals.DEBUG_PRINT_SPAWNED_POWER_UPS)
		{
			SpawnedObjects.countUP(gameObject.name);
		}
		lastSpawnPosition = position.position.z;
	}

	public bool CheckSpawn(Transform newSpawnPosition, List<Transform> _activePickupPoints, List<Transform> _usedPickupPoints)
	{
		bool result = false;
		bool flag = false;
		if (!_usedPickupPoints.Contains(newSpawnPosition))
		{
			if (_activePrefabs.Count != 0)
			{
				foreach (Transform activePrefab in _activePrefabs)
				{
					if (newSpawnPosition.position.z < activePrefab.position.z)
					{
						flag = true;
					}
					else
					{
						result = false;
					}
				}
				if (flag)
				{
					if (CalculateSpawn(newSpawnPosition))
					{
						result = true;
						_usedPickupPoints.Add(newSpawnPosition);
						InstatiateSpawn(newSpawnPosition);
						int count = _activePrefabs.Count;
						foreach (Transform activePrefab2 in _activePrefabs)
						{
							activePrefab2.Translate(0f, -1000f, 0f);
							_inActivePrefabs.Add(activePrefab2);
						}
						_activePrefabs.Clear();
						for (int i = 0; i < count; i++)
						{
							foreach (Transform _activePickupPoint in _activePickupPoints)
							{
								if (!_usedPickupPoints.Contains(_activePickupPoint))
								{
									_usedPickupPoints.Add(_activePickupPoint);
									InstatiateSpawn(_activePickupPoint);
								}
							}
						}
					}
					else
					{
						result = false;
					}
					flag = false;
				}
				else if (CalculateSpawn(newSpawnPosition))
				{
					result = true;
					_usedPickupPoints.Add(newSpawnPosition);
					InstatiateSpawn(newSpawnPosition);
				}
				else
				{
					result = false;
				}
			}
			else if (CalculateSpawn(newSpawnPosition))
			{
				result = true;
				_usedPickupPoints.Add(newSpawnPosition);
				InstatiateSpawn(newSpawnPosition);
			}
			else
			{
				result = false;
			}
		}
		return result;
	}

	public void RemovePoint(Transform position)
	{
		Transform transform = null;
		foreach (Transform activePrefab in _activePrefabs)
		{
			if (activePrefab.position == position.position)
			{
				transform = activePrefab;
			}
		}
		if (transform != null)
		{
			transform.GetComponent<PickupController>().Deactivate();
			_activePrefabs.Remove(transform);
			transform.transform.parent = _inactiveObjectsRoot;
			_inActivePrefabs.Add(transform);
		}
	}

	private void OnNewGameStarted()
	{
		lastSpawnPosition = 0f;
	}
}
