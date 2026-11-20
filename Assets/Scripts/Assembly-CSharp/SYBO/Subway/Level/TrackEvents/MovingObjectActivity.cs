using PathologicalGames;
using SYBO.Core.Components.Singleton;
using SYBO.Subway.Level.Placeholders.Lifecycles;
using SYBO.Subway.Managers;
using UnityEngine;

namespace SYBO.Subway.Level.TrackEvents
{
	public class MovingObjectActivity : Activity
	{
		[SerializeField]
		private PlaceholderLifecycle _movingObject;

		private float _speed;

		private float _desiredSpeed;

		private Transform _initialParent;

		private TrackChunk _trackChunk;

		public static SpawnPoolType SpawnPoolType
		{
			get
			{
				return SpawnPoolType.Default;
			}
		}

		public float Speed
		{
			get
			{
				return _speed;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		public override void Prepare(IActivityInitializer initializer)
		{
			CheckInitialize();
			IMoveObjectActivityInitializer moveObjectActivityInitializer = (IMoveObjectActivityInitializer)initializer;
			_initialParent = moveObjectActivityInitializer.Transform;
			_transform.parent = moveObjectActivityInitializer.Transform;
			_transform.localPosition = Vector3.zero;
			_transform.localRotation = Quaternion.identity;
			_desiredSpeed = moveObjectActivityInitializer.Speed;
			_speed = _desiredSpeed;
			_trackChunk = moveObjectActivityInitializer.TrackChunk;
			_trackChunk.OnDeactivate += TrackChunk_OnDeactivate;
			_movingObject.Spawn();
		}

		protected override void Execute()
		{
			UpdateCurrentChunk();
			_transform.localPosition += _speed * Vector3.forward * Time.deltaTime;
		}

		private void UpdateCurrentChunk()
		{
			TrackChunk trackChunk = null;
			if (_transform.position.z < _trackChunk.ZWorldStart)
			{
				trackChunk = _trackChunk.Previous;
			}
			else
			{
				if (!(_trackChunk.ZWorldEnd < _transform.position.z))
				{
					return;
				}
				trackChunk = _trackChunk.Next;
			}
			if (trackChunk != null)
			{
				_trackChunk.OnDeactivate -= TrackChunk_OnDeactivate;
				_trackChunk = trackChunk;
				if (trackChunk.Active)
				{
					_transform.parent = _trackChunk.transform;
					_trackChunk.OnDeactivate += TrackChunk_OnDeactivate;
				}
				else
				{
					Stop();
				}
			}
		}

		public override void Stop()
		{
			if (!_initialized)
			{
				Debug.LogError("Stop called on uninitialized MovingCarActivity");
				return;
			}
			base.Stop();
			_trackChunk.OnDeactivate -= TrackChunk_OnDeactivate;
			_movingObject.Despawn();
			SpawnPool spawnPool = SingletonMonobehavior<SpawnPoolManager>.Instance.GetSpawnPool(SpawnPoolType);
			spawnPool.Despawn(_transform);
		}

		private void TrackChunk_OnDeactivate()
		{
			if (_running)
			{
				Stop();
			}
		}
	}
}
