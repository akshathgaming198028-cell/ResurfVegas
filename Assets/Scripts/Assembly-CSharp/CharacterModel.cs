using System.Collections.Generic;
using SYBO.Subway.Info;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterModel : MonoBehaviour
{
	[FormerlySerializedAs("rigPrefab")]
	[SerializeField]
	private GameObject _rigPrefab;

	[SerializeField]
	[FormerlySerializedAs("ShadowPrefab")]
	private GameObject _shadowPrefab;

	[SerializeField]
	[FormerlySerializedAs("rigController")]
	private GameObject _rigController;

	[FormerlySerializedAs("focusPoint")]
	[SerializeField]
	private GameObject _focusPoint;

	[FormerlySerializedAs("sprayCanPrefab")]
	[SerializeField]
	private GameObject _sprayCanPrefab;

	[FormerlySerializedAs("coinMagnetPrefab")]
	[SerializeField]
	private GameObject _coinMagnetPrefab;

	[SerializeField]
	[FormerlySerializedAs("jetpackPrefab")]
	private GameObject _jetpackPrefab;

	[FormerlySerializedAs("pogostickPrefab")]
	[SerializeField]
	private GameObject _pogostickPrefab;

	[FormerlySerializedAs("topRunIdleAnimation")]
	[SerializeField]
	private TopRunIdleAnimation _topRunIdleAnimation;

	[OptionalField]
	private GameObject _sprayCanModel;

	[OptionalField]
	private MeshRenderer _meshCoinMagnet;

	[OptionalField]
	private MeshRenderer _meshJetpack;

	[OptionalField]
	private MeshRenderer _meshPogostick;

	[OptionalField]
	private Transform _jetpackCloudPositionL;

	[OptionalField]
	private Transform _jetpackCloudPositionR;

	[OptionalField]
	private Transform[] _jetpackThrusters;

	[OptionalField]
	private Transform _attachPoint1;

	[OptionalField]
	private Transform _attachPoint2;

	[OptionalField]
	private Transform _attachPoint3;

	[OptionalField]
	private SkinnedMeshRenderer _model;

	[OptionalField]
	private SkinnedMeshRenderer _outfitModel;

	[OptionalField]
	private SkinnedMeshRenderer _headModel;

	[OptionalField]
	private SkinnedMeshRenderer _meshSuperSneaker;

	[OptionalField]
	private SkinnedMeshRenderer _doubleJumpers;

	[OptionalField]
	private SkinnedMeshRenderer _rocketSkates;

	[OptionalField]
	private Animation _characterAnimation;

	[OptionalField]
	private Transform _spineTransform;

	[OptionalField]
	private Transform _shoulderTransform;

	[OptionalField]
	private Transform _hip;

	[OptionalField]
	private Transform _leftFoot;

	[OptionalField]
	private Transform _rightFoot;

	[OptionalField]
	private Transform _headJoint;

	[OptionalField]
	private Transform _rightHand;

	private CharacterAssetsBundle _currentModelBundle;

	private MeshRenderer _meshBlobShadow;

	private GameObject _currentHoverboard;

	private List<GameObject> _currentCustomBoardModels = new List<GameObject>();

	private List<Renderer> _currentCustomBoardModelsRenderes = new List<Renderer>();

	private Dictionary<string, SkinnedMeshRenderer> _modelLookupTable;

	private string[] _modelNames;

	private CharacterCustomization _characterCustomization;

	private CharacterAssetFactory _charaterAssetFactory;

	private AvatarEyeAnimation _animEye;

	private AvatarAnimations _anim;

	private CharacterRig _characterRig;

	public MeshRenderer MeshBlobShadow
	{
		get
		{
			return _meshBlobShadow;
		}
	}

	public GameObject CurrentHoverboard
	{
		get
		{
			return _currentHoverboard;
		}
	}

	public List<GameObject> CurrentCustomBoardModels
	{
		get
		{
			return _currentCustomBoardModels;
		}
	}

	public List<Renderer> CurrentCustomBoardModelsRenderes
	{
		get
		{
			return _currentCustomBoardModelsRenderes;
		}
	}

	public GameObject SprayCanModel
	{
		get
		{
			return GetSprayCanModel();
		}
	}

	public MeshRenderer MeshCoinMagnet
	{
		get
		{
			return GetCoinMagnet();
		}
	}

	public MeshRenderer MeshJetpack
	{
		get
		{
			return GetJetpack();
		}
	}

	public MeshRenderer MeshPogostick
	{
		get
		{
			return GetPogostick();
		}
	}

	public Transform JetpackCloudPositionL
	{
		get
		{
			return _jetpackCloudPositionL;
		}
	}

	public Transform JetpackCloudPositionR
	{
		get
		{
			return _jetpackCloudPositionR;
		}
	}

	public Transform[] JetpackThrusters
	{
		get
		{
			return _jetpackThrusters;
		}
	}

	public Transform AttachPoint1
	{
		get
		{
			return _attachPoint1;
		}
	}

	public Transform AttachPoint2
	{
		get
		{
			return _attachPoint2;
		}
	}

	public Transform AttachPoint3
	{
		get
		{
			return _attachPoint3;
		}
	}

	public SkinnedMeshRenderer Model
	{
		get
		{
			return _model;
		}
	}

	public SkinnedMeshRenderer OutfitModel
	{
		get
		{
			return _outfitModel;
		}
	}

	public SkinnedMeshRenderer HeadModel
	{
		get
		{
			return _headModel;
		}
	}

	public SkinnedMeshRenderer MeshSuperSneaker
	{
		get
		{
			return _meshSuperSneaker;
		}
	}

	public SkinnedMeshRenderer DoubleJumpers
	{
		get
		{
			return _doubleJumpers;
		}
	}

	public SkinnedMeshRenderer RocketSkates
	{
		get
		{
			return _rocketSkates;
		}
	}

	public Animation CharacterAnimation
	{
		get
		{
			return _characterAnimation;
		}
	}

	public Transform SpineTransform
	{
		get
		{
			return _spineTransform;
		}
	}

	public Transform ShoulderTransform
	{
		get
		{
			return _shoulderTransform;
		}
	}

	public Transform Hip
	{
		get
		{
			return _hip;
		}
	}

	public Transform LeftFoot
	{
		get
		{
			return _leftFoot;
		}
	}

	public Transform RightFoot
	{
		get
		{
			return _rightFoot;
		}
	}

	public Transform HeadJoint
	{
		get
		{
			return _headJoint;
		}
	}

	public Transform RightHand
	{
		get
		{
			return _rightHand;
		}
	}

	public TopRunIdleAnimation TopRunIdleAnimation
	{
		get
		{
			return _topRunIdleAnimation;
		}
	}

	private void Awake()
	{
		BuildRig();
		GameObject gameObject = Object.Instantiate(_shadowPrefab);
		gameObject.transform.parent = base.transform;
		_meshBlobShadow = gameObject.GetComponent<MeshRenderer>();
		gameObject.GetComponent<PointConstraint>().master = Hip;
		GetSprayCanModel();
		base.gameObject.GetComponentInChildren<KeyframeBehaviour>().TargetObjects = _sprayCanModel.GetComponentsInChildren<ParticleSystem>();
		_sprayCanModel.SetActive(false);
	}

	private void BuildRig()
	{
		GameObject gameObject = Object.Instantiate(_rigPrefab);
		gameObject.transform.parent = _rigController.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		_characterRig = gameObject.GetComponent<CharacterRig>();
		_characterAnimation = _characterRig.CharacterAnimation;
		_attachPoint1 = _characterRig.AttachPoint1;
		_attachPoint2 = _characterRig.AttachPoint2;
		_attachPoint3 = _characterRig.AttachPoint3;
		_model = _characterRig.Model;
		_outfitModel = _characterRig.OutfitModel;
		_outfitModel.enabled = false;
		_headModel = _characterRig.HeadModel;
		_headModel.enabled = false;
		_meshSuperSneaker = _characterRig.MeshSuperSneaker;
		_meshSuperSneaker.enabled = false;
		_doubleJumpers = _characterRig.DoubleJumpers;
		_doubleJumpers.enabled = false;
		_rocketSkates = _characterRig.RocketSkates;
		_rocketSkates.enabled = false;
		_characterAnimation = _characterRig.CharacterAnimation;
		_spineTransform = _characterRig.SpineTransform;
		_shoulderTransform = _characterRig.ShoulderTransform;
		_hip = _characterRig.Hip;
		_leftFoot = _characterRig.LeftFoot;
		_rightFoot = _characterRig.RightFoot;
		_headJoint = _characterRig.HeadJoint;
		_rightHand = _characterRig.RightHand;
	}

	public void InitializeCharacter()
	{
		int currentCharacter = PlayerInfo.Instance.currentCharacter;
		Characters.CharacterType key = (Characters.CharacterType)currentCharacter;
		if (!Characters.characterData.ContainsKey(key))
		{
			Debug.LogError(string.Format("This character index ({0}) does not exist on this branch. Stop the scene. In the menu bar press \"Tools -> Data files -> Delete PlayerInfo\" and try play the scene again.", currentCharacter));
		}
		string modelName = Characters.characterData[key].modelName;
		int currentThemeIndex = PlayerInfo.Instance.currentThemeIndex;
		CheckForCharactersToBeInSyncWithStaticData();
		SetCharacterModel(modelName, currentThemeIndex, false, true);
		_outfitModel = _characterRig.OutfitModel;
		Hoverboard.Instance.intialize();
		base.gameObject.GetComponentInChildren<RunnerAnimPlayer>().Initialize();
		base.gameObject.GetComponentInChildren<KeyframeBehaviour>().Initialize();
	}

	public void RecreateCharacter()
	{
		string modelName = Characters.characterData[(Characters.CharacterType)PlayerInfo.Instance.currentCharacter].modelName;
		int currentThemeIndex = PlayerInfo.Instance.currentThemeIndex;
		ChangeCharacterModel(modelName, currentThemeIndex, false, true);
		_outfitModel = _characterRig.OutfitModel;
		base.gameObject.GetComponentInChildren<RunnerAnimPlayer>().Initialize();
		base.gameObject.GetComponentInChildren<KeyframeBehaviour>().Initialize();
	}

	public void ChangeCharacterModel(string name, int customizedVersion, bool useSpecialAsset, bool deleteSpecialAsset)
	{
		StopIdleAnimations();
		if (_currentModelBundle != null)
		{
			Object.Destroy(_currentModelBundle.gameObject);
		}
		SetCharacterModel(name, customizedVersion, useSpecialAsset, deleteSpecialAsset);
	}

	public void SetCharacterModel(string name, int customizedVersion, bool useSpecialAsset, bool deleteSpecialAsset)
	{
		CharacterAssetFactory.GetAssetPrefab(name, delegate(CharacterAssetsBundle bundle)
		{
			SetupModel(bundle, name, customizedVersion, useSpecialAsset, deleteSpecialAsset);
		});
	}

	private void SetupModel(CharacterAssetsBundle characterAssetBundle, string name, int customizedVersion, bool useSpecialAsset, bool deleteSpecialAsset)
	{
		if (characterAssetBundle == null)
		{
			Debug.LogWarning(string.Format("Could not find character by the name:- {0}", name));
			return;
		}
		_currentModelBundle = characterAssetBundle;
		_currentModelBundle.gameObject.name = name;
		_currentModelBundle.transform.parent = base.transform;
		_model.sharedMesh = characterAssetBundle.MainMesh;
		_anim = _characterRig.AvatarAnimations;
		_headModel.enabled = false;
		_headModel.sharedMesh = null;
		_headModel.sharedMesh = characterAssetBundle.HeadMesh;
		_headModel.GetComponent<Renderer>().enabled = true;
		_anim.Initialize(characterAssetBundle);
		_animEye = characterAssetBundle._avatarEyeAnimation;
		if (_animEye != null)
		{
			_animEye.Initialize(_focusPoint, _headJoint, _characterRig.EyeSwitch, _characterRig.EyeFocusOrigin, _characterRig.EyeFocusTarget, _characterRig.BlendShapeController, _headModel, characterAssetBundle.EyeMaterialID);
		}
		_characterRig.CharacterCustomization.Customize(name, customizedVersion, _model, _outfitModel, _headModel, _characterRig, useSpecialAsset, deleteSpecialAsset, false);
	}

	private GameObject GetSprayCanModel()
	{
		if (_sprayCanModel == null)
		{
			_sprayCanModel = Object.Instantiate(_sprayCanPrefab);
			_sprayCanModel.transform.parent = _rightHand;
			_sprayCanModel.transform.localPosition = Vector3.zero;
			_sprayCanModel.transform.localRotation = Quaternion.identity;
			_sprayCanModel.transform.localScale = Vector3.one;
		}
		return _sprayCanModel;
	}

	private MeshRenderer GetCoinMagnet()
	{
		if (_meshCoinMagnet == null)
		{
			GameObject gameObject = Object.Instantiate(_coinMagnetPrefab);
			CoinMagnet.Instance.Initialize();
			gameObject.transform.parent = _rightHand;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			_meshCoinMagnet = gameObject.GetComponentInChildren<MeshRenderer>();
			_meshCoinMagnet.enabled = false;
		}
		return _meshCoinMagnet;
	}

	private MeshRenderer GetJetpack()
	{
		if (_meshJetpack == null)
		{
			GameObject gameObject = Object.Instantiate(_jetpackPrefab);
			gameObject.transform.parent = _spineTransform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			JetpackModel componentInChildren = gameObject.GetComponentInChildren<JetpackModel>();
			_meshJetpack = componentInChildren.Mesh;
			_meshJetpack.enabled = false;
			_jetpackCloudPositionL = componentInChildren.JetpackCloudPositionL;
			_jetpackCloudPositionR = componentInChildren.JetpackCloudPositionR;
			_jetpackThrusters = componentInChildren.JetpackThrusters;
		}
		return _meshJetpack;
	}

	private MeshRenderer GetPogostick()
	{
		if (_meshPogostick == null)
		{
			GameObject gameObject = Object.Instantiate(_pogostickPrefab);
			gameObject.transform.parent = _attachPoint1;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			_meshPogostick = gameObject.GetComponent<MeshRenderer>();
			_meshPogostick.enabled = false;
		}
		return _meshPogostick;
	}

	public GameObject GetHoverboardGameObject()
	{
		if (_attachPoint1 != null)
		{
			return _attachPoint1.gameObject;
		}
		return null;
	}

	public GameObject SetNewHoverboard(GameObject root, GameObject newBoard, string boardName)
	{
		if (_currentHoverboard != null)
		{
			Object.Destroy(_currentHoverboard);
		}
		if (_currentCustomBoardModels.Count > 0)
		{
			foreach (GameObject currentCustomBoardModel in _currentCustomBoardModels)
			{
				Object.Destroy(currentCustomBoardModel);
			}
			_currentCustomBoardModels.Clear();
			_currentCustomBoardModelsRenderes.Clear();
		}
		GameObject gameObject = Object.Instantiate(newBoard);
		gameObject.transform.parent = root.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		gameObject.name = boardName;
		Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.gameObject.layer = Layers.Instance._3DGUI;
		}
		_currentHoverboard = gameObject;
		return gameObject;
	}

	public GameObject AddHoverboardModel()
	{
		GameObject gameObject = (_currentHoverboard = Object.Instantiate(Hoverboard.Instance.GetActiveHoverboard()));
		gameObject.transform.parent = _attachPoint1;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		return gameObject;
	}

	public Renderer AddCustomHoverboardModel(GameObject modelPrefab, bool isMenu, Transform parent)
	{
		Renderer result = null;
		if (modelPrefab != null)
		{
			GameObject gameObject = Object.Instantiate(modelPrefab);
			result = gameObject.GetComponentInChildren<Renderer>();
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			_currentCustomBoardModels.Add(gameObject);
			_currentCustomBoardModelsRenderes.Add(gameObject.GetComponentInChildren<Renderer>());
			if (isMenu)
			{
				gameObject.layer = Layers.Instance._3DGUI;
				Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
				Renderer[] array = componentsInChildren;
				foreach (Renderer renderer in array)
				{
					renderer.gameObject.layer = Layers.Instance._3DGUI;
				}
			}
		}
		return result;
	}

	public void AddCustomHoverboardVanityMenuFX(GameObject modelPrefab, Transform parent)
	{
		if (modelPrefab != null)
		{
			GameObject gameObject = Object.Instantiate(modelPrefab);
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			_currentCustomBoardModels.Add(gameObject);
		}
	}

	public void DeleteBoardModels()
	{
		if (_currentCustomBoardModels.Count <= 0)
		{
			return;
		}
		foreach (GameObject currentCustomBoardModel in _currentCustomBoardModels)
		{
			Object.Destroy(currentCustomBoardModel);
		}
		_currentCustomBoardModels.Clear();
		_currentCustomBoardModelsRenderes.Clear();
	}

	public void HideAllPowerups()
	{
		if (_currentHoverboard != null)
		{
			_currentHoverboard.SetActive(false);
		}
		if (_meshJetpack != null)
		{
			_meshJetpack.enabled = false;
		}
		if (_meshSuperSneaker != null)
		{
			_meshSuperSneaker.enabled = false;
		}
		if (_meshCoinMagnet != null)
		{
			_meshCoinMagnet.enabled = false;
		}
		if (_sprayCanModel != null)
		{
			_sprayCanModel.SetActive(false);
		}
		ParticleSystem[] componentsInChildren = _sprayCanModel.GetComponentsInChildren<ParticleSystem>(true);
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			ParticleSystem.EmissionModule emission = particleSystem.emission;
			emission.enabled = false;
		}
	}

	public void HideBlobShadow()
	{
		_meshBlobShadow.enabled = false;
	}

	public void StartEyeAnimations(UIModelController.ModelScreen modelScreen)
	{
		if (!(_model == null) && _animEye != null)
		{
			_animEye.AnimateEyes(modelScreen);
		}
	}

	public void StopEyeAnimations()
	{
		if (!(_model == null) && _animEye != null)
		{
			_animEye.StopAnimatingEyes();
		}
	}

	public void StartIdleAnimations(UIModelController.ModelScreen modelScreen)
	{
		if (!(_model == null))
		{
			if (_anim != null)
			{
				_anim.StartIdleAnimations();
			}
			if (_animEye != null)
			{
				_animEye.AnimateEyes(modelScreen);
			}
		}
	}

	public void StartIdlePopupAnimations()
	{
		if (!(_model == null))
		{
			if (_anim != null)
			{
				_anim.StartIdlePopupAnimations();
			}
			if (_animEye != null)
			{
				_animEye.AnimateEyes(UIModelController.ModelScreen.Popup);
			}
		}
	}

	public void StopIdleAnimations()
	{
		if (!(_model == null))
		{
			if (_anim != null)
			{
				_anim.StopIdleAnimations();
			}
			if (_animEye != null)
			{
				_animEye.StopAnimatingEyes();
			}
		}
	}

	public void PauseIdleAnimations()
	{
		if (!(_model == null) && _anim != null)
		{
			_anim.PauseIdleAnimations();
		}
	}

	public void ResumeIdleAnimations()
	{
		if (!(_model == null) && _anim != null)
		{
			_anim.ResumeIdleAnimations();
		}
	}

	private void CheckForCharactersToBeInSyncWithStaticData()
	{
	}

	private void OnDestroy()
	{
		Object.Destroy(_focusPoint);
	}
}
