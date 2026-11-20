using System;
using SYBO.Subway.Managers;
using UnityEngine;

namespace SYBO.Subway.Level.Placeholders
{
	[Serializable]
	public struct PrefabSpawnInfo
	{
		public Transform PrefabTransform;

		public SpawnPoolType SpawnPoolType;

		public PrefabSpawnInfo(Transform prefabTransform, SpawnPoolType spawnPoolType)
		{
			PrefabTransform = prefabTransform;
			SpawnPoolType = spawnPoolType;
		}
	}
}
