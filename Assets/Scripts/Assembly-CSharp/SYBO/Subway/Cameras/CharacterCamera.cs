using UnityEngine;

namespace SYBO.Subway.Cameras
{
	public class CharacterCamera : MonoBehaviour
	{
		[SerializeField]
		private EnvironmentBackground _environmentBackground;

		public Vector3 Position;

		public Vector3 Target;

		public float Speed;

		private Camera _camera;

		private float _lastZ;

		private Vector3 _shake = Vector3.zero;

		private Vector3 _diff = Vector3.zero;

		private bool _fovGipping;

		private static CharacterCamera instance;

		public Camera Camera
		{
			get
			{
				return _camera;
			}
		}

		public static CharacterCamera Instance
		{
			get
			{
				return instance ?? (instance = Object.FindObjectOfType(typeof(CharacterCamera)) as CharacterCamera);
			}
		}

		protected void Awake()
		{
			_lastZ = base.transform.position.z;
			_camera = GetComponent<Camera>();
		}

		protected void Start()
		{
		}

		protected void LateUpdate()
		{
			base.transform.position = Position + _shake;
			base.transform.LookAt(Target + _shake);
			Speed = (Position.z - _lastZ) / Time.deltaTime;
			if (float.IsNaN(Speed))
			{
				Speed = 0f;
			}
			_lastZ = Position.z;
		}

		protected void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(Position, Target);
		}

		public void Shake()
		{
			_diff = Vector3.zero;
			float amplitude = 100f;
			StartCoroutine(pTween.To(0.3f, delegate(float t)
			{
				_diff += Random.insideUnitSphere;
				_shake = (1f - t) * _diff * amplitude * Time.deltaTime;
			}));
		}

		public void ResetShake()
		{
			_shake = Vector3.zero;
		}

		public void Shake(float intensity)
		{
			_shake = Random.insideUnitSphere * intensity * Time.deltaTime;
		}

		public void FOVGip(float time, float multiplier)
		{
			if (_fovGipping)
			{
				return;
			}
			_fovGipping = true;
			float startFOV = _camera.fieldOfView;
			StartCoroutine(pTween.To(time, delegate(float t)
			{
				if ((double)t < 0.5)
				{
					_camera.fieldOfView = Mathf.Lerp(startFOV, startFOV * multiplier, t * 2f);
				}
				else
				{
					_camera.fieldOfView = Mathf.Lerp(startFOV * multiplier, startFOV, t * 2f - 1f);
				}
				if (t == 1f)
				{
					_fovGipping = false;
				}
			}));
		}

		public void Game_OnCharacterStateChanged(object sender, CharacterStateChangedEventArgs characterStateChangedEventArgs)
		{
		}
	}
}
