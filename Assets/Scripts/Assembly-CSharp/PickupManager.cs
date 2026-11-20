using System.Collections.Generic;
using SYBO.Subway.Characters;
using SYBO.Subway.Pickups.Migration;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
	[SerializeField]
	private PickupManagerData[] spawnSetups;

	[SerializeField]
	private float _minSpawnDistance;

	[SerializeField]
	private bool _debug;

	private List<Transform> _activePickupPoints = new List<Transform>();

	private List<Transform> _usedPickupPoints = new List<Transform>();

	private static PickupManager instance;

	private GameObject _activeObjects;

	private GameObject _inactiveObjects;

	public static PickupManager Instance
	{
		get
		{
			return instance ?? (instance = Utils.FindObject<PickupManager>());
		}
	}

	private void Awake()
	{
		if (PickupsMigration.Migration_IsActivePickupSystemVersion2014)
		{
			instance = this;
			_activeObjects = new GameObject("ActiveObjects");
			_inactiveObjects = new GameObject("InactiveObjects");
			_activeObjects.transform.parent = base.transform;
			_inactiveObjects.transform.parent = base.transform;
			PickupManagerData[] array = spawnSetups;
			foreach (PickupManagerData pickupManagerData in array)
			{
				pickupManagerData.Initialize(_activeObjects.transform, _inactiveObjects.transform);
			}
			CheckSpawns();
			OnlineSettings.instance.AddOnFinishedDownloadingOnlineSettingsHandler(CheckSpawns);
		}
	}

	private void OnEnable()
	{
		WeeklyHuntManager.ItemCountUpdated += CheckSpawns;
	}

	private void OnDisable()
	{
		WeeklyHuntManager.ItemCountUpdated -= CheckSpawns;
	}

	public void CheckSpawns()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.SuperMysteryBox);
		PickupManagerData[] array = spawnSetups;
		foreach (PickupManagerData pickupManagerData in array)
		{
			if (pickupManagerData.SPAWN_PREFAB_TYPE == PickupManagerData.SpawnPrefabType.SuperMysteryBox)
			{
				pickupManagerData.canSpawn = activeEventManager != null;
			}
			if (pickupManagerData.SPAWN_PREFAB_TYPE == PickupManagerData.SpawnPrefabType.HuntToken)
			{
				pickupManagerData.canSpawn = WeeklyHuntManager.IsAnyPeriodActive();
			}
		}
	}

	private void _UpdateSpawning(Transform point, bool add)
	{
		if (_debug)
		{
			PickupManagerData[] array = spawnSetups;
			foreach (PickupManagerData pickupManagerData in array)
			{
				pickupManagerData.canSpawn = true;
			}
		}
		if (add)
		{
			for (int j = 0; j < _activePickupPoints.Count; j++)
			{
				bool flag = false;
				if (!(point.position.z > Character.Instance.z + _minSpawnDistance))
				{
					continue;
				}
				for (int k = 0; k < spawnSetups.Length; k++)
				{
					if (!flag && spawnSetups[k].canSpawn)
					{
						flag = spawnSetups[k].CheckSpawn(point, _activePickupPoints, _usedPickupPoints);
					}
				}
			}
		}
		else
		{
			for (int l = 0; l < spawnSetups.Length; l++)
			{
				spawnSetups[l].RemovePoint(point);
			}
		}
	}

	public void AddPoint(Transform point)
	{
		if (_activePickupPoints.Count == 0)
		{
			_activePickupPoints.Add(point);
		}
		else
		{
			for (int i = 0; i < _activePickupPoints.Count; i++)
			{
				if (point.position.z < _activePickupPoints[i].position.z)
				{
					if (!_activePickupPoints.Contains(point))
					{
						_activePickupPoints.Insert(i, point);
						break;
					}
				}
				else if (!_activePickupPoints.Contains(point))
				{
					_activePickupPoints.Add(point);
				}
			}
		}
		CheckSpawns();
		_UpdateSpawning(point, true);
	}

	public void RemovePoint(Transform point)
	{
		_activePickupPoints.Remove(point);
		if (_usedPickupPoints.Contains(point))
		{
			_usedPickupPoints.Remove(point);
		}
		_UpdateSpawning(point, false);
	}

	public void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			CheckSpawns();
		}
	}
}
