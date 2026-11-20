using PathologicalGames;
using SYBO.Core.Components.Singleton;
using SYBO.Subway.Managers;
using UnityEngine;

namespace SYBO.Subway.Level.TrackEvents
{
	public class MovingObjectTrackEvent : TrackEvent, IActivityInitializer, IMoveObjectActivityInitializer
	{
		[SerializeField]
		private MovingObjectActivity _activityPrefab;

		private Transform _activityPrefabTransform;

		[SerializeField]
		[Range(0f, 1000f)]
		private float _speed = 200f;

		[SerializeField]
		private bool _randomize;

		[Tooltip("Used as the max speed value when randomizing speed")]
		[Range(0f, 1000f)]
		[SerializeField]
		private float _speedMax = 400f;

		private SpawnPoolManager _spawnPoolManager;

		private TrackChunk _trackChunk;

		Transform IMoveObjectActivityInitializer.Transform
		{
			get
			{
				return _transform;
			}
		}

		public TrackChunk TrackChunk
		{
			get
			{
				return _trackChunk;
			}
		}

		public float Speed
		{
			get
			{
				return (!_randomize) ? _speed : Random.Range(_speed, _speedMax);
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			_activityPrefabTransform = _activityPrefab.transform;
			_spawnPoolManager = SingletonMonobehavior<SpawnPoolManager>.Instance;
		}

		protected override void Launch()
		{
			SpawnPool spawnPool = _spawnPoolManager.GetSpawnPool(MovingObjectActivity.SpawnPoolType);
			Transform transform = spawnPool.Spawn(_activityPrefabTransform);
			MovingObjectActivity component = transform.GetComponent<MovingObjectActivity>();
			_trackChunk = GetComponentInParent<TrackChunk>();
			component.Prepare(this);
			component.Run();
		}
	}
}
