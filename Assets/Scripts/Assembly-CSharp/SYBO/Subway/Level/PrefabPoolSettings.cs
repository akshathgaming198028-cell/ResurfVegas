using SYBO.Subway.Managers;
using UnityEngine;

namespace SYBO.Subway.Level
{
	[DisallowMultipleComponent]
	public class PrefabPoolSettings : MonoBehaviour
	{
		public SpawnPoolType SpawnPoolType;

		public int PreloadAmount = 1;

		public bool PreloadTime;

		public int PreloadFrames = 2;

		public float PreloadDelay;

		public bool LimitInstances;

		public int LimitAmount = 100;

		public bool LimitFIFO;

		public bool CullDespawned;

		public int CullAbove = 50;

		public int CullDelay = 60;

		public int CullMaxPerPass = 5;

		public bool SendLifecycleMessages;

		public bool LogMessages;
	}
}
