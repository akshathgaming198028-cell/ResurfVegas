using System.Collections.Generic;
using PathologicalGames;
using SYBO.Core.Components.Singleton;
using SYBO.Subway.Characters;
using SYBO.Subway.Managers;
using UnityEngine;

namespace SYBO.Subway.Level.TrackEvents
{
	[RequireComponent(typeof(TrackObject))]
	[RequireComponent(typeof(BoxCollider))]
	public class MovingCarTrackEvent : CharacterTriggeredTrackEvent, IActivityInitializer, IMoveObjectActivityInitializer
	{
		[SerializeField]
		private MovingCarActivity _activityPrefab;

		private Transform _activityPrefabTransform;

		[Range(0f, 1000f)]
		[SerializeField]
		private float _speed = 200f;

		[SerializeField]
		private bool _randomize;

		[SerializeField]
		[Range(0f, 1000f)]
		[Tooltip("Used as the max speed value when randomizing speed")]
		private float _speedMax = 400f;

		private List<Collider> _collidersInSpawnArea = new List<Collider>();

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

		private void OnTriggerEnter(Collider otherCollider)
		{
			_collidersInSpawnArea.Add(otherCollider);
		}

		private void OnTriggerExit(Collider otherCollider)
		{
			_collidersInSpawnArea.Remove(otherCollider);
		}

		protected override void Launch()
		{
			if (!(0f < (float)_collidersInSpawnArea.Count))
			{
				SpawnPool spawnPool = _spawnPoolManager.GetSpawnPool(SpawnPoolType.Default);
				Transform transform = spawnPool.Spawn(_activityPrefabTransform);
				MovingCarActivity component = transform.GetComponent<MovingCarActivity>();
				_trackChunk = GetComponentInParent<TrackChunk>();
				component.Prepare(this);
				component.Run();
			}
		}

		protected override void TrackObject_OnActivate()
		{
			_collidersInSpawnArea.Clear();
			Character instance = Character.Instance;
			if (instance != null && _trigger.EndPosition < instance.z && !_launched)
			{
				Launch();
			}
		}
	}
}
