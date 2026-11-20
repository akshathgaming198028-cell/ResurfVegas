using System;
using PathologicalGames;
using SYBO.Core;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Math;
using SYBO.Subway.Level.Placeholders.Lifecycles;
using SYBO.Subway.Managers;
using UnityEngine;

namespace SYBO.Subway.Level.TrackEvents
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(BoxCollider))]
	public class MovingCarActivity : Activity
	{
		[SerializeField]
		private PlaceholderLifecycle _carPlaceholderLifecycle;

		[SerializeField]
		private CollisionNotifier _brakingTrigger;

		private float _speed;

		private float _desiredSpeed;

		private float _acceleration;

		private float _brakeStartSpeed;

		private float _brakeEndSpeed;

		private float _brakeStartDistance;

		private bool _braking;

		private bool _carAheadMoveOpposite;

		private MovingCarActivity _carAhead;

		private Transform _carAheadTransforn;

		private Transform _initialParent;

		private BoxCollider _collider;

		private TrackChunk _trackChunk;

		private Collider _turnTrigger;

		private bool _turning;

		private Transform _turnTarget;

		private Vector3 _turnStartPos;

		private Quaternion _turnStartRotation;

		private Vector3 _vectStartToTarget;

		private Vector3 _turnStartPoint;

		private Vector3 _turnTargetPoint;

		private Vector3 _turnTargetDirectionForward;

		private Vector3 _turnTargetDirectionUp;

		private Quaternion _turnTargetRotation;

		private Vector3 _turnHalfWayPoint;

		private Vector3 _turnHalfWayDirectionForward;

		private Vector3 _turnHalfWayDirectionUp;

		private Quaternion _turnHalfWayRotation;

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
			_collider = GetComponent<Collider>() as BoxCollider;
			_brakingTrigger.TriggerEnter += BrakingTrigger_OnEnter;
			_brakingTrigger.TriggerExit += BrakingTrigger_OnExit;
		}

		private void OnTriggerEnter(Collider trigger)
		{
			if (!trigger.CompareTag("TrackEvent"))
			{
				return;
			}
			MovingCarDirectionChange component = trigger.GetComponent<MovingCarDirectionChange>();
			if (component != null && !(_turnTrigger == trigger) && !_turning)
			{
				_turning = true;
				_turnTrigger = trigger;
				_turnTarget = component.TargetDirection;
				_turnStartPoint = _transform.position;
				_turnStartRotation = _transform.rotation;
				_turnTargetPoint = _turnTarget.position;
				_turnTargetDirectionForward = _turnTarget.forward;
				_turnTargetDirectionUp = _turnTarget.up;
				_turnTargetRotation = _turnTarget.rotation;
				_vectStartToTarget = _turnTarget.position - _transform.position;
				Vector3 normalized = _vectStartToTarget.normalized;
				_turnHalfWayDirectionUp = (_transform.up + _turnTarget.up).normalized;
				float num = Quaternion.Angle(_transform.rotation, _turnTarget.rotation);
				bool flag = 90f < num;
				bool flag2 = Vector3.Dot(_transform.forward, _vectStartToTarget.normalized) < 0f;
				if (flag && flag2)
				{
					_turnHalfWayDirectionForward = normalized;
					_turnHalfWayRotation = Quaternion.LookRotation(_turnHalfWayDirectionForward, _turnHalfWayDirectionUp);
					_turnHalfWayPoint = _transform.position + 0.5f * _vectStartToTarget;
					return;
				}
				float num2 = 0.5f * _vectStartToTarget.magnitude / Mathf.Sin(0.5f * num * ((float)Math.PI / 180f));
				float num3 = num2 * Mathf.Cos(0.5f * num * ((float)Math.PI / 180f));
				float num4 = num2 - num3;
				_turnHalfWayDirectionForward = (normalized - 0.2f * _turnTargetDirectionForward).normalized;
				_turnHalfWayRotation = Quaternion.LookRotation(_turnHalfWayDirectionForward, _turnHalfWayDirectionUp);
				Vector3 rightVector = Math3D.GetRightVector(_turnHalfWayRotation);
				_turnHalfWayPoint = _transform.position + 0.5f * _vectStartToTarget + num4 * rightVector;
			}
		}

		protected void OnDrawGizmosSelected()
		{
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
			_running = false;
			_turning = false;
			_braking = false;
			_turnTrigger = null;
			_carPlaceholderLifecycle.Spawn();
		}

		protected override void Execute()
		{
			UpdateCurrentChunk();
			if (_turning)
			{
				Turn();
			}
			if (_braking)
			{
				Brake();
			}
			_transform.position += _speed * _transform.forward * Time.deltaTime;
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
			_turning = false;
			_braking = false;
			_turnTrigger = null;
			_carAhead = null;
			_trackChunk.OnDeactivate -= TrackChunk_OnDeactivate;
			_carPlaceholderLifecycle.Despawn();
			SpawnPool spawnPool = SingletonMonobehavior<SpawnPoolManager>.Instance.GetSpawnPool(SpawnPoolType.Default);
			spawnPool.Despawn(_transform);
		}

		private void Turn()
		{
			Vector3 lhs = _transform.position - _turnStartPoint;
			float num = Vector3.Dot(lhs, _vectStartToTarget.normalized) / _vectStartToTarget.magnitude;
			if (0.99f < num)
			{
				num = 1f;
			}
			if (num < 0.5f)
			{
				float t = num * 2f;
				_transform.rotation = Quaternion.Lerp(_turnStartRotation, _turnHalfWayRotation, t);
			}
			else
			{
				float t2 = (num - 0.5f) * 2f;
				_transform.rotation = Quaternion.Lerp(_turnHalfWayRotation, _turnTargetRotation, t2);
			}
			if (1f <= num)
			{
				_turning = false;
				_turnTrigger = null;
			}
		}

		private void Brake()
		{
			if (!_carAhead._running)
			{
				_braking = false;
			}
			Vector3 vector = GapStartPoint();
			Vector3 vector2 = GapEndPoint();
			Debug.DrawLine(vector + Vector3.up, vector2 + Vector3.up, Color.magenta);
			float num = Vector3.Distance(vector, vector2);
			_speed = Mathf.Lerp(_carAhead.Speed, _brakeStartSpeed, Mathf.Clamp01(num / _brakeStartDistance));
			if (_speed < 1f)
			{
				_speed = 0f;
				_braking = false;
			}
			else if (_speed - _carAhead.Speed < 1f || num < 1f)
			{
				_speed = _carAhead.Speed;
				_braking = false;
			}
		}

		private Vector3 GapStartPoint()
		{
			return _transform.position + new Vector3(0f, 0f, _collider.center.z) + _transform.forward * 0.5f * (_collider.size.z + _collider.size.x);
		}

		private Vector3 GapEndPoint()
		{
			float num = ((!_carAheadMoveOpposite) ? (-1f) : 1f);
			return _carAhead._transform.position + num * _carAhead._transform.forward * _collider.size.z * 0.5f;
		}

		private void TrackChunk_OnDeactivate()
		{
			if (_running)
			{
				Stop();
			}
		}

		private void BrakingTrigger_OnEnter(Collider otherCollider)
		{
			MovingCarActivity component = otherCollider.GetComponent<MovingCarActivity>();
			if (component != null && !(component == this))
			{
				_carAhead = component;
				_carAheadTransforn = component.transform;
				_carAheadMoveOpposite = Vector3.Dot(_transform.forward, _carAheadTransforn.forward) < 0f;
				_braking = true;
				_brakeStartDistance = Vector3.Distance(GapStartPoint(), GapEndPoint());
				_brakeStartSpeed = _speed;
				_brakeEndSpeed = ((!_carAheadMoveOpposite) ? _carAhead.Speed : 0f);
			}
		}

		private void BrakingTrigger_OnExit(Collider otherCollider)
		{
			MovingCarDirectionChange component = otherCollider.GetComponent<MovingCarDirectionChange>();
			MovingCarActivity component2 = otherCollider.GetComponent<MovingCarActivity>();
			if (component == null && component2 != this && component2 != null)
			{
				_braking = false;
			}
		}
	}
}
