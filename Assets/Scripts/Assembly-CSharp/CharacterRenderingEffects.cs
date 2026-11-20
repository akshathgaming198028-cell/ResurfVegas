using System;
using SYBO.Subway;
using SYBO.Subway.Cameras;
using SYBO.Subway.Characters;
using UnityEngine;

internal class CharacterRenderingEffects : MonoBehaviour
{
	public delegate void OnDoubleJumpDelegate();

	[SerializeField]
	private Renderer leftJumpThruster;

	[SerializeField]
	private Renderer rightJumpThruster;

	[SerializeField]
	private float thrusterPlayTime = 0.2f;

	[SerializeField]
	private bool _showSnow;

	[SerializeField]
	private ParticleSystem _snow;

	[SerializeField]
	private ParticleSystem _snowLow;

	[SerializeField]
	private bool _showStars;

	[SerializeField]
	private Transform _stars;

	[SerializeField]
	private float _zOffset;

	[SerializeField]
	private Transform _scoreBoosterRoot;

	[SerializeField]
	private Animation _scoreBoostAnimation;

	[SerializeField]
	private ParticleSystem _scoreBoostParticle;

	[SerializeField]
	private GameObject jetpackParticles;

	[SerializeField]
	private GameObject pogostickParticles;

	public ParticleFollow pogostickParticleCloud;

	public ParticleFollow jetpackParticleCloudL;

	public ParticleFollow jetpackParticleCloudR;

	private AnimationState leftJumpThrusterState;

	private AnimationState rightJumpThrusterState;

	private Vector3 _starsOriginalPosition;

	private ParticleSystem _currentSnow;

	private Transform _snowTransform;

	private Vector3 _snowOriginalPosition;

	private bool _snowActive;

	private Character _character;

	private CharacterModel _characterModel;

	private CharacterCamera characterCamera;

	private Hoverboard _hoverboard;

	private Game _game;

	public GameObject JetpackParticles
	{
		get
		{
			return jetpackParticles;
		}
	}

	public GameObject PogostickParticles
	{
		get
		{
			return pogostickParticles;
		}
	}

	public event OnDoubleJumpDelegate OnDoubleJump;

	private void Awake()
	{
		if (DeviceInfo.Instance.performanceLevel == DeviceInfo.PerformanceLevel.Low && _showSnow)
		{
			_snow.gameObject.SetActive(false);
			_currentSnow = _snowLow;
		}
		else if (DeviceInfo.Instance.performanceLevel == DeviceInfo.PerformanceLevel.High && _showSnow)
		{
			_snowLow.gameObject.SetActive(false);
			_currentSnow = _snow;
		}
		else
		{
			_snow.gameObject.SetActive(false);
			_snowLow.gameObject.SetActive(false);
			_currentSnow = _snowLow;
		}
		if (_showStars)
		{
			_stars.gameObject.SetActive(true);
			_starsOriginalPosition = _stars.position;
		}
		else
		{
			_stars.gameObject.SetActive(false);
		}
	}

	public void Initialize(CharacterModel characterModel)
	{
		_game = Game.Instance;
		Game game = _game;
		game.OnStageMenuSequence = (Game.OnStageMenuSequenceDelegate)Delegate.Combine(game.OnStageMenuSequence, new Game.OnStageMenuSequenceDelegate(DeactivateSnow));
		Game game2 = _game;
		game2.OnIntroRun = (Game.OnIntroRunDelegate)Delegate.Combine(game2.OnIntroRun, new Game.OnIntroRunDelegate(ActivateSnow));
		_character = Character.Instance;
		_characterModel = characterModel;
		characterCamera = CharacterCamera.Instance;
		_hoverboard = Hoverboard.Instance;
		InitializeModel(characterModel);
		SetupScoorboosterFX();
	}

	public void InitializeModel(CharacterModel characterModel)
	{
		SetupDoubleJumpThrusters(characterModel.LeftFoot, characterModel.RightFoot);
		SetupSnow(characterModel.transform);
	}

	public void RecreateModel(CharacterModel characterModel)
	{
		_characterModel = characterModel;
		SetupDoubleJumpThrusters(characterModel.LeftFoot, characterModel.RightFoot);
		SetupSnow(characterModel.transform);
	}

	private void SetupScoorboosterFX()
	{
		_scoreBoosterRoot.parent = _character.transform;
		_scoreBoosterRoot.localPosition = Vector3.zero;
		_scoreBoosterRoot.localRotation = Quaternion.identity;
		_scoreBoosterRoot.localScale = Vector3.one;
	}

	private void SetupDoubleJumpThrusters(Transform leftFoot, Transform rightFoot)
	{
		leftJumpThruster.transform.parent = leftFoot;
		leftJumpThruster.enabled = false;
		leftJumpThruster.transform.localPosition = new Vector3(0f, 0.25f, -0.8f);
		leftJumpThruster.transform.localRotation = Quaternion.Euler(0f, 90f, 35f);
		leftJumpThrusterState = leftJumpThruster.GetComponent<Animation>()["POOF"];
		leftJumpThrusterState.enabled = true;
		leftJumpThrusterState.weight = 1f;
		rightJumpThruster.transform.parent = rightFoot;
		rightJumpThruster.enabled = false;
		rightJumpThruster.transform.localPosition = new Vector3(0f, -0.25f, 0.8f);
		rightJumpThruster.transform.localRotation = Quaternion.Euler(0f, 270f, 325f);
		rightJumpThrusterState = rightJumpThruster.GetComponent<Animation>()["POOF"];
		rightJumpThrusterState.enabled = true;
		rightJumpThrusterState.weight = 1f;
	}

	private void SetupSnow(Transform root)
	{
		_snowTransform = _currentSnow.gameObject.transform;
		_snowOriginalPosition = _snowTransform.position;
	}

	private void ActivateSnow()
	{
		_currentSnow.Play();
		_snowActive = true;
	}

	private void DeactivateSnow()
	{
		_snowActive = false;
		_currentSnow.Stop();
	}

	public void PlayScoreboostFX(int version)
	{
		if (version == 1)
		{
			_scoreBoostAnimation.Play("ScoreBoosterFirst");
		}
		if (version == 2)
		{
			_scoreBoostAnimation.Play("ScoreBooster");
		}
		if (version == 3)
		{
			_scoreBoostAnimation.Play("ScoreBooster");
			_scoreBoostParticle.Play();
			characterCamera.Shake();
		}
		if (version < 1 || version > 3)
		{
			Debug.LogWarning("PlayScoreBooster method is not using a supported version number, use 0,1 or 2");
		}
	}

	public void PlayDoubleJumpThruster()
	{
		if (!_hoverboard.isActive)
		{
			leftJumpThruster.enabled = true;
			rightJumpThruster.enabled = true;
			StartCoroutine(pTween.To(thrusterPlayTime, delegate(float t)
			{
				leftJumpThrusterState.normalizedTime = t;
				rightJumpThrusterState.normalizedTime = t;
				if (t == 1f)
				{
					leftJumpThruster.enabled = false;
					rightJumpThruster.enabled = false;
				}
			}));
		}
		else if (this.OnDoubleJump != null)
		{
			this.OnDoubleJump();
		}
	}

	public void SetRightAndLeftParticlesActive(bool active)
	{
		if (jetpackParticleCloudL.Target == null || jetpackParticleCloudR.Target == null)
		{
			jetpackParticleCloudL.Target = _characterModel.JetpackCloudPositionL;
			jetpackParticleCloudR.Target = _characterModel.JetpackCloudPositionR;
		}
		if (active)
		{
			jetpackParticles.SetActive(active);
			jetpackParticleCloudL.gameObject.SetActive(active);
			jetpackParticleCloudL.enabled = active;
			jetpackParticleCloudR.gameObject.SetActive(active);
			jetpackParticleCloudR.enabled = active;
			return;
		}
		jetpackParticleCloudL.enabled = active;
		jetpackParticleCloudR.enabled = active;
		StartCoroutine(pTween.To(0.15f, delegate(float t)
		{
			if (t < 1f)
			{
				jetpackParticles.transform.position += new Vector3(0f, -0.05f, 0.02f) * characterCamera.Speed * t;
			}
			if (t == 1f)
			{
				jetpackParticles.SetActive(active);
				jetpackParticleCloudL.gameObject.SetActive(active);
				jetpackParticleCloudR.gameObject.SetActive(active);
			}
		}));
	}

	public void SetPogostickParticlesActive(bool active)
	{
		if (pogostickParticleCloud.Target == null)
		{
			pogostickParticleCloud.Target = _characterModel.AttachPoint1;
		}
		if (active)
		{
			pogostickParticles.SetActive(active);
			pogostickParticleCloud.gameObject.SetActive(active);
			pogostickParticleCloud.enabled = active;
			return;
		}
		pogostickParticleCloud.enabled = active;
		StartCoroutine(pTween.To(0.15f, delegate(float t)
		{
			if (t < 1f)
			{
				pogostickParticles.transform.position += new Vector3(0f, -0.01f, 0.02f) * characterCamera.Speed * t;
			}
			if (t == 1f)
			{
				pogostickParticles.SetActive(active);
				pogostickParticleCloud.gameObject.SetActive(active);
			}
		}));
	}

	private void Update()
	{
		if (_snowActive)
		{
			_snowTransform.position = new Vector3(_snowOriginalPosition.x, _snowOriginalPosition.y, _character.z + _zOffset);
			ParticleSystem.EmissionModule emission = _currentSnow.emission;
			emission.enabled = !_character.IsInsideSubway;
		}
		if (_showStars)
		{
			_stars.position = new Vector3(_starsOriginalPosition.x, _starsOriginalPosition.y, _character.z + _starsOriginalPosition.z);
		}
	}
}
