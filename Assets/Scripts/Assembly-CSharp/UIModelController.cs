using System;
using System.Collections;
using SYBO.Subway;
using SYBO.Subway.Info;
using UnityEngine;

public class UIModelController : MonoBehaviour
{
	public enum ModelScreen
	{
		Character = 0,
		GameOver = 1,
		TutorialPopup = 2,
		Popup = 3,
		Boards = 4,
		CelebrationCharacterUnlock = 5,
		CelebrationBoardUnlock = 6,
		CelebrationHighScore = 7,
		DuelResultPopup = 8,
		CelebrationTopRun = 9,
		LimitedCharacterPopup = 10,
		LimitedBoardPopup = 11
	}

	private const string RUNNING_ANIMATION_NAME = "run";

	private const string CELEBRATION_STRIPE_COROUTINE_NAME = "AnimateUnlockBackground";

	private const string BRAG_CELEBRATION_ANIMATION_COROUTINE = "AnimateRunningCharacterForBragCelebration";

	private const float RUNNING_SPEED_LOW = 0.45f;

	private const float DURATION = 3.5f;

	private const float PARTICLE_ANIMATION_SPEED_FACTOR_LOW = 0.5f;

	private const float PARTICLE_ANIMATION_SPEED_FACTOR_HIGH = 3.5f;

	public GameObject CharacterAnchor;

	public GameObject GameOverAnchor;

	public GameObject CelebrationPopupAnchor;

	public GameObject TutorialPopupAnchor;

	public GameObject HuntPrizeAnchor;

	public GameObject PauseScreenAnchor;

	public GameObject ModelPrefab;

	public CelebrationBackgroundHelper celebrationBackgroundHelper;

	private CharacterModel _cachedActiveModel;

	private bool celebrationShouldAnimateStripes = true;

	private bool _isCelebrationCharacterScreen;

	private bool _isCelebrationBackgroundRotating;

	private Vector3 _discoBgRotation = new Vector3(35f, 180f, 0f);

	private Vector3 _tunnerBgRotationNormal = new Vector3(0f, 0f, 180f);

	private ModelScreen _currentActivatedScreenModel;

	private bool isShownInMysteryBox;

	private Hoverboards.BoardType currentMysteryBoxBoardType;

	private int _currentlyShownThemeIndex;

	private Characters.CharacterType _currentlyShownModel;

	private Vector3 CHAR_TO_POS = new Vector3(-60f, -150f, -100f);

	private Vector3 CHAR_FROM_POS = new Vector3(382f, -125f, 750f);

	private static UIModelController _instance;

	public GameObject limitedCharacterPopupAnchor { get; set; }

	public GameObject limitedBoardPopupAnchor { get; set; }

	public int currentlyShownThemeIndex
	{
		get
		{
			return _currentlyShownThemeIndex;
		}
	}

	public Characters.CharacterType currentlyShownModel
	{
		get
		{
			return _currentlyShownModel;
		}
	}

	public static UIModelController Instance
	{
		get
		{
			return _instance ?? (_instance = UnityEngine.Object.FindObjectOfType(typeof(UIModelController)) as UIModelController);
		}
	}

	public static event Action OnChangedCurrentlyShownCharacter;

	public static event Action OnChangedCurrentlyShownHoverboard;

	public void ActivateGameOverModel()
	{
		_ActivateModel((Characters.CharacterType)PlayerInfo.Instance.currentCharacter, PlayerInfo.Instance.currentThemeIndex, ModelScreen.GameOver);
	}

	public GameObject ShowCharacterInCelebration(Characters.CharacterType charType, int themeIndex, ModelScreen screen)
	{
		isShownInMysteryBox = true;
		return _ActivateModel(charType, themeIndex, screen);
	}

	public void DeactivateCelebrationPopupModels()
	{
		StopCoroutine("AnimateUnlockBackground");
		StopCoroutine("AnimateRunningCharacterForBragCelebration");
		isShownInMysteryBox = false;
		foreach (Transform item in TutorialPopupAnchor.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		celebrationBackgroundHelper.Hide();
	}

	public void ActivateCelebrationBoardWithStripes(Hoverboards.BoardType boardType, ModelScreen screen)
	{
		isShownInMysteryBox = true;
		currentMysteryBoxBoardType = boardType;
		if (screen == ModelScreen.CelebrationHighScore)
		{
			_ActivateModel((Characters.CharacterType)PlayerInfo.Instance.currentCharacter, PlayerInfo.Instance.currentThemeIndex, screen);
		}
		else
		{
			_ActivateModel(Instance.currentlyShownModel, PlayerInfo.Instance.currentThemeIndex, screen);
		}
	}

	public void SetDiscoLightBackground()
	{
		celebrationBackgroundHelper.Show(Quaternion.Euler(_discoBgRotation), false, true);
	}

	public void ActivateCelebrationTunnel()
	{
		celebrationBackgroundHelper.Show(Quaternion.Euler(_tunnerBgRotationNormal), false, false);
	}

	public void DeactivateCelebrationTunnel()
	{
		celebrationBackgroundHelper.Hide();
	}

	public void SelectThemeForCurrentModel(int themeVersion)
	{
		_currentlyShownThemeIndex = themeVersion;
		SelectCurrentModelForPlay();
	}

	public void SelectCurrentModelForPlay()
	{
		SelectCharacterForPlay(currentlyShownModel, currentlyShownThemeIndex);
	}

	public void SelectCharacterForPlay(Characters.CharacterType characterType, int themeIndex)
	{
		PlayerInfo.Instance.currentCharacter = (int)characterType;
		PlayerInfo.Instance.currentThemeIndex = themeIndex;
		if (ShouldCreateABrandNewModel())
		{
			Game.Instance.RecreateCharacterModel();
		}
		else if (Game.Instance != null)
		{
			string modelName = Characters.characterData[(Characters.CharacterType)PlayerInfo.Instance.currentCharacter].modelName;
			Game.Instance.Character.characterModel.ChangeCharacterModel(modelName, PlayerInfo.Instance.currentThemeIndex, false, true);
		}
		Action onChangedCurrentlyShownCharacter = UIModelController.OnChangedCurrentlyShownCharacter;
		if (onChangedCurrentlyShownCharacter != null)
		{
			onChangedCurrentlyShownCharacter();
		}
	}

	public void ShowMenuModel(Characters.CharacterType charType, int themeVersion, ModelScreen screen = ModelScreen.Character)
	{
		if (_currentlyShownModel != charType || _currentlyShownThemeIndex != themeVersion || _cachedActiveModel == null)
		{
			_currentlyShownModel = charType;
			_currentlyShownThemeIndex = themeVersion;
			_SwitchModel(charType, themeVersion, screen);
		}
	}

	public void ActivateHoverboardModel(ModelScreen screen = ModelScreen.Boards)
	{
		_currentlyShownModel = (Characters.CharacterType)PlayerInfo.Instance.currentCharacter;
		_currentlyShownThemeIndex = PlayerInfo.Instance.currentThemeIndex;
		_ActivateModel(_currentlyShownModel, _currentlyShownThemeIndex, screen);
		Action onChangedCurrentlyShownHoverboard = UIModelController.OnChangedCurrentlyShownHoverboard;
		if (onChangedCurrentlyShownHoverboard != null)
		{
			onChangedCurrentlyShownHoverboard();
		}
	}

	public void ShowHoverboardMenuModel(Hoverboards.BoardType currentlyShownBoard, bool willUpdateAnimation, bool willAllowSignatureAnimation, Hoverboard.BoardAbility boardAbility)
	{
		GameObject hoverboardGameObject = _cachedActiveModel.GetHoverboardGameObject();
		HoverboardModelPreviewFactory.Instance.ChangeHoverboard(currentlyShownBoard, hoverboardGameObject, _cachedActiveModel.CharacterAnimation, willUpdateAnimation, willAllowSignatureAnimation, boardAbility);
		Action onChangedCurrentlyShownHoverboard = UIModelController.OnChangedCurrentlyShownHoverboard;
		if (onChangedCurrentlyShownHoverboard != null)
		{
			onChangedCurrentlyShownHoverboard();
		}
	}

	public void SelectCurrentlyShownBoard(Hoverboards.BoardType boardType)
	{
		PlayerInfo.Instance.currentHoverboard = boardType;
		Action onChangedCurrentlyShownHoverboard = UIModelController.OnChangedCurrentlyShownHoverboard;
		if (onChangedCurrentlyShownHoverboard != null)
		{
			onChangedCurrentlyShownHoverboard();
		}
	}

	public CharacterModel GetActiveCharacterModel()
	{
		return _cachedActiveModel;
	}

	private GameObject _ActivateModel(Characters.CharacterType characterName, int modelVersion, ModelScreen screen, Characters.CharacterType? friendsCharacterName = null, int friendsModelVersion = -1)
	{
		if (_cachedActiveModel != null && !isShownInMysteryBox)
		{
			ClearModels();
		}
		else
		{
			isShownInMysteryBox = false;
		}
		string modelName = Characters.characterData[characterName].modelName;
		_currentActivatedScreenModel = screen;
		switch (screen)
		{
		case ModelScreen.Character:
		{
			GameObject gameObject7 = UnityEngine.Object.Instantiate(ModelPrefab);
			gameObject7.transform.parent = CharacterAnchor.transform;
			gameObject7.transform.localPosition = new Vector3(0f, 0f, 0f);
			Utility.SetLayerRecursively(gameObject7.transform, CharacterAnchor.layer);
			gameObject7.transform.localScale = new Vector3(21f, 21f, 21f);
			gameObject7.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
			CharacterModel component7 = gameObject7.GetComponent<CharacterModel>();
			component7.ChangeCharacterModel(modelName, modelVersion, true, true);
			component7.HideAllPowerups();
			component7.StartIdleAnimations(screen);
			_cachedActiveModel = component7;
			return gameObject7;
		}
		case ModelScreen.GameOver:
		{
			GameObject gameObject4 = UnityEngine.Object.Instantiate(ModelPrefab);
			gameObject4.transform.parent = GameOverAnchor.transform;
			gameObject4.transform.localPosition = new Vector3(0f, 0f, 0f);
			Utility.SetLayerRecursively(gameObject4.transform, GameOverAnchor.layer);
			gameObject4.transform.localScale = new Vector3(18f, 18f, 18f);
			gameObject4.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
			CharacterModel component4 = gameObject4.GetComponent<CharacterModel>();
			component4.ChangeCharacterModel(modelName, modelVersion, true, false);
			component4.HideAllPowerups();
			component4.StartIdleAnimations(screen);
			_cachedActiveModel = component4;
			return gameObject4;
		}
		case ModelScreen.Boards:
		{
			GameObject gameObject6 = UnityEngine.Object.Instantiate(ModelPrefab);
			gameObject6.transform.parent = CharacterAnchor.transform;
			gameObject6.transform.localPosition = new Vector3(-30f, 0f, 100f);
			Utility.SetLayerRecursively(gameObject6.transform, CharacterAnchor.layer);
			gameObject6.transform.localScale = new Vector3(21f, 21f, 21f);
			gameObject6.transform.localRotation = Quaternion.Euler(new Vector3(20f, 154.5f, 358f));
			CharacterModel component6 = gameObject6.GetComponent<CharacterModel>();
			component6.ChangeCharacterModel(modelName, modelVersion, false, true);
			component6.HideAllPowerups();
			component6.StartEyeAnimations(screen);
			_cachedActiveModel = component6;
			return gameObject6;
		}
		case ModelScreen.CelebrationCharacterUnlock:
		case ModelScreen.CelebrationTopRun:
		{
			Vector3 localScale = new Vector3(21f, 21f, 21f);
			Vector3 localPosition = new Vector3(-85f, -110f, -200f);
			Vector3 euler = new Vector3(0f, 180f, 0f);
			GameObject gameObject5 = UnityEngine.Object.Instantiate(ModelPrefab);
			gameObject5.transform.parent = TutorialPopupAnchor.transform;
			gameObject5.transform.localPosition = localPosition;
			Utility.SetLayerRecursively(gameObject5.transform, TutorialPopupAnchor.layer);
			if (characterName == Characters.CharacterType.halloweenCharacter01)
			{
				gameObject5.transform.localScale = new Vector3(20f, 20f, 20f);
			}
			else
			{
				gameObject5.transform.localScale = localScale;
			}
			gameObject5.transform.localRotation = Quaternion.Euler(euler);
			CharacterModel component5 = gameObject5.GetComponent<CharacterModel>();
			component5.ChangeCharacterModel(modelName, modelVersion, (screen != ModelScreen.CelebrationTopRun) ? true : false, true);
			component5.HideAllPowerups();
			component5.StartEyeAnimations(ModelScreen.CelebrationTopRun);
			switch (screen)
			{
			case ModelScreen.CelebrationCharacterUnlock:
				component5.StartIdleAnimations(screen);
				celebrationBackgroundHelper.Show(Quaternion.Euler(_discoBgRotation), false, true);
				celebrationShouldAnimateStripes = false;
				_isCelebrationBackgroundRotating = true;
				break;
			case ModelScreen.CelebrationTopRun:
			{
				TopRunIdleAnimation topRunIdleAnimation = component5.TopRunIdleAnimation;
				if (topRunIdleAnimation != null)
				{
					topRunIdleAnimation.TR_ActivateIdle();
					break;
				}
				Debug.Log("default to idle animation");
				component5.StartIdleAnimations(screen);
				break;
			}
			}
			_isCelebrationCharacterScreen = true;
			return gameObject5;
		}
		case ModelScreen.CelebrationBoardUnlock:
		case ModelScreen.CelebrationHighScore:
		{
			GameObject gameObject3 = UnityEngine.Object.Instantiate(ModelPrefab);
			gameObject3.transform.parent = TutorialPopupAnchor.transform;
			gameObject3.transform.localPosition = new Vector3(-80f, -100f, 0f);
			gameObject3.transform.localScale = new Vector3(21f, 21f, 21f);
			gameObject3.transform.localRotation = Quaternion.Euler(new Vector3(30f, 210f, 10f));
			CharacterModel component3 = gameObject3.GetComponent<CharacterModel>();
			component3.ChangeCharacterModel(modelName, modelVersion, false, false);
			component3.HideAllPowerups();
			if (screen == ModelScreen.CelebrationBoardUnlock)
			{
				GameObject hoverboardGameObject = component3.GetHoverboardGameObject();
				bool willAllowSignatureAnimation = false;
				HoverboardModelPreviewFactory.Instance.ChangeHoverboard(currentMysteryBoxBoardType, hoverboardGameObject, component3.CharacterAnimation, true, willAllowSignatureAnimation, Hoverboard.BoardAbility.None);
			}
			Utility.SetLayerRecursively(gameObject3.transform, TutorialPopupAnchor.layer);
			celebrationShouldAnimateStripes = true;
			_isCelebrationCharacterScreen = false;
			_isCelebrationBackgroundRotating = false;
			StartCoroutine("AnimateUnlockBackground", gameObject3);
			if (screen == ModelScreen.CelebrationHighScore)
			{
				StartCoroutine("AnimateRunningCharacterForBragCelebration", gameObject3);
			}
			return gameObject3;
		}
		case ModelScreen.LimitedCharacterPopup:
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(ModelPrefab);
			gameObject2.transform.parent = limitedCharacterPopupAnchor.transform;
			gameObject2.transform.localPosition = new Vector3(0f, -56f, 0f);
			Utility.SetLayerRecursively(gameObject2.transform, CharacterAnchor.layer);
			gameObject2.transform.localScale = new Vector3(21f, 21f, 21f);
			gameObject2.transform.localRotation = Quaternion.Euler(new Vector3(-8f, 180f, 0f));
			CharacterModel component2 = gameObject2.GetComponent<CharacterModel>();
			component2.ChangeCharacterModel(modelName, modelVersion, true, true);
			component2.HideAllPowerups();
			component2.StartIdleAnimations(screen);
			_cachedActiveModel = component2;
			return gameObject2;
		}
		case ModelScreen.LimitedBoardPopup:
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(ModelPrefab);
			gameObject.transform.parent = limitedBoardPopupAnchor.transform;
			gameObject.transform.localPosition = new Vector3(-30f, -56f, 100f);
			Utility.SetLayerRecursively(gameObject.transform, CharacterAnchor.layer);
			gameObject.transform.localScale = new Vector3(21f, 21f, 21f);
			gameObject.transform.localRotation = Quaternion.Euler(new Vector3(12f, 154.5f, 358f));
			CharacterModel component = gameObject.GetComponent<CharacterModel>();
			component.ChangeCharacterModel(modelName, modelVersion, false, true);
			component.HideAllPowerups();
			component.StartEyeAnimations(screen);
			_cachedActiveModel = component;
			return gameObject;
		}
		default:
			return null;
		}
	}

	private CharacterModel InstantiateCharacterModelForPopup(Characters.CharacterType characterName, int modelVersion, ModelScreen screen, Vector3 position, Vector3 rotation, Vector3 scale)
	{
		string modelName = Characters.characterData[characterName].modelName;
		GameObject gameObject = UnityEngine.Object.Instantiate(ModelPrefab);
		gameObject.transform.parent = TutorialPopupAnchor.transform;
		Utility.SetLayerRecursively(gameObject.transform, TutorialPopupAnchor.layer);
		gameObject.transform.localPosition = position;
		gameObject.transform.localScale = scale;
		gameObject.transform.localRotation = Quaternion.Euler(rotation);
		CharacterModel component = gameObject.GetComponent<CharacterModel>();
		component.ChangeCharacterModel(modelName, modelVersion, true, true);
		component.HideAllPowerups();
		component.StartIdleAnimations(screen);
		return component;
	}

	private IEnumerator AnimateRunningCharacterForBragCelebration(GameObject model)
	{
		CharacterAnimationControl _charAnimCtrl = model.GetComponentInChildren<CharacterAnimationControl>();
		model.transform.localPosition = CHAR_FROM_POS;
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / 3.5f;
			factor = Mathf.Clamp01(factor);
			float animCurveFactor = celebrationBackgroundHelper.GetBragCelebrationAnimationCurveValue(factor);
			float inverseFactor = 1f - factor;
			celebrationBackgroundHelper.SetParticleAnimationSpeed(Mathf.Lerp(0.5f, 3.5f, inverseFactor));
			float runFactor = Mathf.Clamp(inverseFactor, 0.45f, 1f);
			_charAnimCtrl.Play();
			_charAnimCtrl.Speed(runFactor);
			model.transform.localPosition = Vector3.Lerp(CHAR_FROM_POS, CHAR_TO_POS, animCurveFactor);
			yield return null;
		}
	}

	private IEnumerator AnimateUnlockBackground(GameObject go)
	{
		Vector3 charWithBoardPosition = new Vector3(-80f, -100f, 0f);
		Vector3 rotation1CharWithBoard = new Vector3(27f, 205f, 16f);
		Vector3 rotation2CharWithBoard = new Vector3(350f, 140f, 10f);
		Vector3 rotation3CharWithBoard = new Vector3(5f, 210f, 0f);
		Vector3 offset1PosBoard = new Vector3(0f, 0f, -30f);
		Vector3 offset2PosBoard = new Vector3(0f, 0f, -170f);
		Vector3 offset3PosBoard = new Vector3(0f, 0f, -100f);
		Vector3 charPosition = new Vector3(-85f, -110f, 0f);
		Vector3 rotation1character = new Vector3(0f, 130f, 0f);
		Vector3 rotation2character = new Vector3(0f, 180f, 0f);
		Vector3 rotation3character = new Vector3(0f, 240f, 0f);
		Vector3 offset1PosCharacter = new Vector3(-10f, 0f, -180f);
		Vector3 offset2PosCharacter = new Vector3(0f, 0f, -200f);
		Vector3 offset3PosCharacter = new Vector3(5f, 0f, -200f);
		Vector3 offset1BgRotationCharScreen = new Vector3(35f, 20f, 0f);
		Vector3 offset2BgRotationCharScreen = new Vector3(35f, 0f, 0f);
		Vector3 offset3BgRotationCharScreen = new Vector3(35f, -30f, 0f);
		int index = 0;
		Vector3 currentRotation = new Vector3(0f, 0f, 0f);
		Vector3 currentOffset = new Vector3(0f, 0f, 0f);
		Vector3 currentBackgroundRotationOffset = new Vector3(0f, 0f, 0f);
		Vector3 currentCharPosition = new Vector3(0f, 0f, 0f);
		if (_currentActivatedScreenModel == ModelScreen.CelebrationHighScore)
		{
			currentBackgroundRotationOffset = new Vector3(0f, 0f, 350f);
			UpdateCelebrationRotation(go, rotation3CharWithBoard, charWithBoardPosition, offset3PosBoard, currentBackgroundRotationOffset, celebrationShouldAnimateStripes, _isCelebrationBackgroundRotating);
			yield break;
		}
		while (true)
		{
			switch (index % 3)
			{
			case 0:
				if (_isCelebrationCharacterScreen)
				{
					currentRotation = rotation1character;
					currentOffset = offset1PosCharacter;
					currentBackgroundRotationOffset = offset1BgRotationCharScreen;
					currentCharPosition = charPosition;
				}
				else
				{
					currentRotation = rotation1CharWithBoard;
					currentOffset = offset1PosBoard;
					currentCharPosition = charWithBoardPosition;
				}
				break;
			case 1:
				if (_isCelebrationCharacterScreen)
				{
					currentRotation = rotation2character;
					currentOffset = offset2PosCharacter;
					currentBackgroundRotationOffset = offset2BgRotationCharScreen;
					currentCharPosition = charPosition;
				}
				else
				{
					currentRotation = rotation2CharWithBoard;
					currentOffset = offset2PosBoard;
					currentCharPosition = charWithBoardPosition;
				}
				break;
			default:
				if (_isCelebrationCharacterScreen)
				{
					currentRotation = rotation3character;
					currentOffset = offset3PosCharacter;
					currentBackgroundRotationOffset = offset3BgRotationCharScreen;
					currentCharPosition = charPosition;
				}
				else
				{
					currentRotation = rotation3CharWithBoard;
					currentOffset = offset3PosBoard;
					currentCharPosition = charWithBoardPosition;
				}
				break;
			}
			UpdateCelebrationRotation(go, currentRotation, currentCharPosition, currentOffset, currentBackgroundRotationOffset, celebrationShouldAnimateStripes, _isCelebrationBackgroundRotating);
			index++;
			yield return new WaitForSeconds(2.5f);
		}
	}

	private void UpdateCelebrationRotation(GameObject go, Vector3 charRotation, Vector3 charPosition, Vector3 charPositionOffset, Vector3 backgroundRotationOffset, bool shouldAnimateStripes, bool rotateBackground)
	{
		if (go != null)
		{
			Vector3 euler = charRotation + backgroundRotationOffset;
			Quaternion rotation = Quaternion.Euler(euler);
			Quaternion localRotation = Quaternion.Euler(charRotation);
			go.transform.localRotation = localRotation;
			celebrationBackgroundHelper.Show(rotation, shouldAnimateStripes, rotateBackground);
			Vector3 localPosition = charPosition;
			localPosition += charPositionOffset;
			go.transform.localPosition = localPosition;
		}
	}

	private bool ShouldCreateABrandNewModel()
	{
		bool result = false;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			bool flag = SystemInfo.operatingSystem.Contains("iPhone OS 7") || SystemInfo.operatingSystem.Contains("iOS 7");
			bool flag2 = SystemInfo.graphicsDeviceName.Contains("Apple A7");
			result = flag && flag2;
		}
		return result;
	}

	private void ClearScreenAnchor(ModelScreen screen)
	{
		if (_cachedActiveModel != null)
		{
			_cachedActiveModel = null;
		}
		if (screen != ModelScreen.Character)
		{
			ClearScreenAnchor(ModelScreen.Character);
		}
		GameObject screenAnchor = GetScreenAnchor(screen);
		if (!screenAnchor)
		{
			return;
		}
		Transform transform = screenAnchor.transform;
		foreach (Transform item in transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
	}

	private GameObject GetScreenAnchor(ModelScreen screen)
	{
		GameObject gameObject = null;
		switch (screen)
		{
		case ModelScreen.Character:
		case ModelScreen.Boards:
			return CharacterAnchor;
		case ModelScreen.GameOver:
			return GameOverAnchor;
		case ModelScreen.TutorialPopup:
			return TutorialPopupAnchor;
		case ModelScreen.LimitedCharacterPopup:
			return limitedCharacterPopupAnchor;
		case ModelScreen.LimitedBoardPopup:
			return limitedBoardPopupAnchor;
		case ModelScreen.CelebrationCharacterUnlock:
		case ModelScreen.CelebrationBoardUnlock:
		case ModelScreen.CelebrationHighScore:
		case ModelScreen.CelebrationTopRun:
			return TutorialPopupAnchor;
		default:
			return CharacterAnchor;
		}
	}

	private void _SwitchModel(Characters.CharacterType characterName, int themeVersion, ModelScreen screen)
	{
		if (ShouldCreateABrandNewModel())
		{
			ClearScreenAnchor(screen);
		}
		if (_cachedActiveModel != null)
		{
			string modelName = Characters.characterData[characterName].modelName;
			_cachedActiveModel.ChangeCharacterModel(modelName, themeVersion, true, true);
			_cachedActiveModel.HideAllPowerups();
			_cachedActiveModel.StartIdleAnimations(screen);
		}
		else
		{
			_ActivateModel(characterName, themeVersion, screen);
		}
	}

	public void ClearModels()
	{
		if (_cachedActiveModel != null)
		{
			_cachedActiveModel = null;
		}
		foreach (Transform item in CharacterAnchor.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (Transform item2 in GameOverAnchor.transform)
		{
			UnityEngine.Object.Destroy(item2.gameObject);
		}
		foreach (Transform item3 in TutorialPopupAnchor.transform)
		{
			UnityEngine.Object.Destroy(item3.gameObject);
		}
		foreach (Transform item4 in CelebrationPopupAnchor.transform)
		{
			UnityEngine.Object.Destroy(item4.gameObject);
		}
		if ((bool)limitedCharacterPopupAnchor)
		{
			foreach (Transform item5 in limitedCharacterPopupAnchor.transform)
			{
				UnityEngine.Object.Destroy(item5.gameObject);
			}
		}
		if (!limitedBoardPopupAnchor)
		{
			return;
		}
		foreach (Transform item6 in limitedBoardPopupAnchor.transform)
		{
			UnityEngine.Object.Destroy(item6.gameObject);
		}
	}

	private void ClearCharacterAnchor()
	{
		if (_cachedActiveModel != null)
		{
			_cachedActiveModel = null;
		}
		foreach (Transform item in CharacterAnchor.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
	}

	public void ClearTutorialPopup()
	{
		foreach (Transform item in TutorialPopupAnchor.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
	}
}
