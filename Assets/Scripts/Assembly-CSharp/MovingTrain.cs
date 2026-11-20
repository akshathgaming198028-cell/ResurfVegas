using System;
using System.Collections.Generic;
using SYBO.Subway;
using SYBO.Subway.Characters;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(TrackObject))]
public class MovingTrain : MonoBehaviour
{
	private Game _game;

	private static CharacterController _characterController;

	private Transform _characterControllerTransform;

	[SerializeField]
	private float speed = 1f;

	[SerializeField]
	private float trainCount = 3f;

	[SerializeField]
	private static float autoPilotActivationDistance = 200f;

	[SerializeField]
	private AudioClip trianPassClip;

	private static List<MovingTrain> _activeTrains = new List<MovingTrain>();

	private Transform _transform;

	private Transform _train;

	private BoxCollider _trainCollider;

	private Vector3 _trainColliderInitialSize;

	private TrackObject _trackObject;

	private AudioSource _trainPassSource;

	private bool _startSound;

	private bool _isPaused;

	private bool _autoPilot;

	private bool _initalized;

	public float Speed
	{
		get
		{
			return speed;
		}
		set
		{
			if (speed != value)
			{
				speed = value;
				if (Application.isPlaying)
				{
					CheckInitialize();
					UpdateTrainCollider();
				}
			}
		}
	}

	private void Awake()
	{
		CheckInitialize();
	}

	private void CheckInitialize()
	{
		if (!_initalized)
		{
			_transform = base.transform;
			_game = Game.Instance;
			if (_transform.childCount == 0)
			{
				Debug.Log("No train child");
			}
			_train = _transform.GetChild(0);
			_train.localPosition = -Vector3.up * 200f;
			_trainCollider = GetComponent<BoxCollider>();
			_trainColliderInitialSize = _trainCollider.size;
			base.enabled = false;
			_trackObject = GetComponent<TrackObject>();
			_trackObject.OnActivate += OnActivate;
			_trackObject.OnDeactivate += OnDeactivate;
			_trainPassSource = base.gameObject.AddComponent<AudioSource>();
			_trainPassSource.minDistance = 20f;
			_trainPassSource.maxDistance = 50f;
			_trainPassSource.playOnAwake = false;
			_trainPassSource.loop = true;
			_trainPassSource.clip = trianPassClip;
			_trainPassSource.spatialBlend = 1f;
			UpdateTrainCollider();
			_initalized = true;
		}
	}

	private void OnDestroy()
	{
		_trackObject.OnActivate -= OnActivate;
		_trackObject.OnDeactivate -= OnDeactivate;
	}

	private void HandleOnPauseChange(bool pause)
	{
		if (pause)
		{
			if (_trainPassSource.isPlaying)
			{
				_trainPassSource.Pause();
			}
			_isPaused = true;
		}
		else
		{
			if (_isPaused)
			{
				_trainPassSource.Play();
			}
			_isPaused = false;
		}
	}

	private void UpdateTrainCollider()
	{
		_trainCollider.size = new Vector3(_trainColliderInitialSize.x, _trainColliderInitialSize.y, _trainColliderInitialSize.z / (1f + speed));
		_trainCollider.center = new Vector3(0f, 15.2f, (30f * trainCount + 1f) / (1f + speed));
	}

	private void OnActivate()
	{
		if (_characterController == null)
		{
			_characterController = Character.Instance.characterController;
		}
		_activeTrains.Add(this);
		base.enabled = true;
		_autoPilot = false;
		Game game = _game;
		game.OnPauseChange = (Game.OnPauseChangeDelegate)Delegate.Combine(game.OnPauseChange, new Game.OnPauseChangeDelegate(HandleOnPauseChange));
		_train.localPosition = new Vector3(0f, 0f, (_transform.position.z - _characterController.transform.position.z) * speed);
		_startSound = true;
	}

	private void Update()
	{
		if (_startSound)
		{
			_trainPassSource.pitch = UnityEngine.Random.Range(0.8f, 1.1f);
			_trainPassSource.volume = UnityEngine.Random.Range(0.1f, 0.6f);
			_trainPassSource.timeSamples = UnityEngine.Random.Range(0, _trainPassSource.timeSamples);
			_trainPassSource.Play();
			_startSound = false;
		}
		if (_autoPilot)
		{
			_train.position -= Vector3.forward * Time.deltaTime * _game.currentSpeed * speed;
			return;
		}
		Vector3 position = new Vector3(0f, 0f, (_transform.position.z - _characterController.transform.position.z) * speed);
		Vector3 position2 = base.transform.TransformPoint(position);
		_train.position = position2;
	}

	private void OnDeactivate()
	{
		Game game = _game;
		game.OnPauseChange = (Game.OnPauseChangeDelegate)Delegate.Remove(game.OnPauseChange, new Game.OnPauseChangeDelegate(HandleOnPauseChange));
		_trainPassSource.Stop();
		_activeTrains.Remove(this);
		base.enabled = false;
		_train.localPosition = -100f * Vector3.up;
	}

	private void OnDrawGizmos()
	{
		if (_train != null)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(_train.position, _transform.position);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(_transform.position, 5f);
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(_train.position, 5f);
		}
	}

	public static void ActivateAutoPilot()
	{
		foreach (MovingTrain activeTrain in _activeTrains)
		{
			if (activeTrain.GetComponent<Collider>().bounds.min.z - _characterController.transform.position.z < autoPilotActivationDistance)
			{
				activeTrain._autoPilot = true;
			}
		}
	}
}
