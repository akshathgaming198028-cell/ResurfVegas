using System;
using SYBO.Core.Components.Singleton;
using SYBO.Subway.Characters;
using SYBO.Subway.Pickups;
using UnityEngine;

namespace SYBO.Subway.Coins
{
	[RequireComponent(typeof(Pickup))]
	[RequireComponent(typeof(CoinRenderer))]
	[RequireComponent(typeof(TrackObject))]
	[RequireComponent(typeof(PickupRotate))]
	public class Coin2 : MonoBehaviour
	{
		private class CoinMagnetMovement
		{
			private Game _game;

			private Character _character;

			private Transform _coinTransform;

			private Transform _magnetTransform;

			private bool _active;

			private Vector3 _startPosition;

			private Vector3 _endPositionOffset;

			private float _startTime;

			private float _duration;

			public event Action OnComplete;

			public CoinMagnetMovement(Transform coinTransform, Transform magnetTransform, Vector3 endPositionOffset)
			{
				_coinTransform = coinTransform;
				_endPositionOffset = endPositionOffset;
				_game = Game.Instance;
				_character = Character.Instance;
				_magnetTransform = magnetTransform;
			}

			public void Activate(float pullSpeed)
			{
				_startPosition = _coinTransform.position;
				_startTime = Time.time;
				float magnitude = (_startPosition - _character.transform.position).magnitude;
				_duration = magnitude / (pullSpeed * _game.NormalizedGameSpeed);
				_active = true;
			}

			public void Update()
			{
				if (!_active)
				{
					return;
				}
				float num = (Time.time - _startTime) / _duration;
				_coinTransform.position = Vector3.Lerp(_startPosition, _magnetTransform.position + _endPositionOffset, num * num);
				if (num >= 1f)
				{
					if (this.OnComplete != null)
					{
						this.OnComplete();
					}
					_active = false;
				}
			}

			public void Reset()
			{
				_active = false;
			}
		}

		public const float QA_X_OFFSET = 5f;

		private const float DEACTIVATION_Z_DISTANCE = 10f;

		private const float DEACTIVATION_INTERVAL = 1.5f;

		[SerializeField]
		private TrackObject _trackObject;

		[SerializeField]
		private PickupRotate _pickupRotate;

		[SerializeField]
		private Pickup _pickup;

		[SerializeField]
		private PickupDefault _pickupDefault;

		[SerializeField]
		private CoinRenderer _coinRenderer;

		[SerializeField]
		private Collider _collider;

		private bool _active;

		private Character _character;

		private CoinMagnetMovement _coinMagnetMovement;

		private CoinMagnetMovement _superSneakerMagnetMovement;

		private float _deactivationCheckCooldown;

		private bool _isInitialized;

		public event EventHandler OnPickedUp;

		public event EventHandler OnDeactivated;

		protected void Start()
		{
			Initialize();
		}

		public void Update()
		{
			if (_pickupDefault.IsVisible)
			{
				_pickupRotate.PhasedRotate();
			}
			_coinMagnetMovement.Update();
			_superSneakerMagnetMovement.Update();
			if (_deactivationCheckCooldown <= 0f)
			{
				_deactivationCheckCooldown = 1.5f;
				if (base.transform.position.z <= _character.transform.position.z - 10f)
				{
					Deactivate();
				}
			}
			_deactivationCheckCooldown -= Time.unscaledDeltaTime;
		}

		public void OnDestroy()
		{
			if (SingletonMonobehavior<CoinManager2>.IsInstantiated())
			{
				SingletonMonobehavior<CoinManager2>.Instance.NotifyCoinDestroyed(this);
			}
			CoinStats.New.NotifyCoinDestroyed();
		}

		public void ToggleRenderer(bool enabled)
		{
			_coinRenderer.ToggleRenderers(enabled);
		}

		public void Deactivate(bool trackObjectDeactivated = false)
		{
			if (_active)
			{
				_active = false;
				if (!trackObjectDeactivated)
				{
					_trackObject.Deactivate();
				}
				SingletonMonobehavior<CoinManager2>.Instance.NotifyCoinDeactivated(this);
				_coinMagnetMovement.Reset();
				_superSneakerMagnetMovement.Reset();
				RaiseOnDeactivated();
				CoinStats.New.NotifyCoinDeactivated();
			}
		}

		public void Activate(bool trackObjectActivated = false)
		{
			if (!_active)
			{
				_active = true;
				if (!trackObjectActivated)
				{
					_trackObject.Activate();
				}
				_collider.enabled = true;
				ToggleRenderer(true);
				SingletonMonobehavior<CoinManager2>.Instance.NotifyCoinActivated(this);
				CoinStats.New.NotifyCoinActivated();
				if (Globals.DEBUG_LOG_COINS)
				{
					CoinSystemSpawnLog.New.LogCoin(base.gameObject);
				}
			}
		}

		public void MoveToMagnet(float pullSpeed)
		{
			_collider.enabled = false;
			_coinMagnetMovement.Activate(pullSpeed);
		}

		public void MoveToSuperSneakers(float pullSpeed)
		{
			_collider.enabled = false;
			_superSneakerMagnetMovement.Activate(pullSpeed);
		}

		public override int GetHashCode()
		{
			return GetInstanceID();
		}

		public void Initialize()
		{
			if (!_isInitialized)
			{
				_isInitialized = true;
				if (_pickupRotate == null)
				{
					_pickupRotate = GetComponent<PickupRotate>();
				}
				if (_pickup == null)
				{
					_pickup = GetComponent<Pickup>();
				}
				Pickup pickup = _pickup;
				pickup.OnPickup = (Pickup.OnPickupDelegate)Delegate.Combine(pickup.OnPickup, new Pickup.OnPickupDelegate(Pickup_OnPickup));
				if (_pickupDefault == null)
				{
					_pickupDefault = GetComponent<PickupDefault>();
				}
				if (_coinRenderer == null)
				{
					_coinRenderer = GetComponent<CoinRenderer>();
				}
				if (_collider == null)
				{
					_collider = GetComponent<Collider>();
				}
				if (_trackObject == null)
				{
					_trackObject = GetComponent<TrackObject>();
				}
				_trackObject.OnActivate += TrackObject_OnActivate;
				_trackObject.OnDeactivate += TrackObject_OnDeactivate;
				_character = Character.Instance;
				_coinMagnetMovement = new CoinMagnetMovement(base.transform, _character.CharacterRendering.CharacterModel.MeshCoinMagnet.transform, Vector3.down * 6f);
				_superSneakerMagnetMovement = new CoinMagnetMovement(base.transform, _character.CharacterRendering.CharacterModel.MeshSuperSneaker.transform, Vector3.down * 6f);
				_coinMagnetMovement.OnComplete += MagnetMovement_OnComplete;
				_superSneakerMagnetMovement.OnComplete += SuperSneakersMagnetMovement_OnComplete;
				CoinStats.New.NotifyCoinCreated();
			}
		}

		private void TrackObject_OnActivate()
		{
			Activate(true);
		}

		private void TrackObject_OnDeactivate()
		{
			Deactivate(true);
		}

		private void Pickup_OnPickup(CharacterPickupParticles pickupParticles)
		{
			RaiseOnPickedUp();
			pickupParticles.PickedUpCoin(_pickup);
			Deactivate();
		}

		private void MagnetMovement_OnComplete()
		{
			_collider.enabled = true;
			GameStats.Instance.coinsCoinMagnet++;
			Character.Instance.NotifyPickup(_pickup, null);
		}

		private void SuperSneakersMagnetMovement_OnComplete()
		{
			_collider.enabled = true;
			Character.Instance.NotifyPickup(_pickup, null);
		}

		private void RaiseOnPickedUp()
		{
			if (this.OnPickedUp != null)
			{
				this.OnPickedUp(this, EventArgs.Empty);
			}
		}

		private void RaiseOnDeactivated()
		{
			if (this.OnDeactivated != null)
			{
				this.OnDeactivated(this, EventArgs.Empty);
			}
		}
	}
}
