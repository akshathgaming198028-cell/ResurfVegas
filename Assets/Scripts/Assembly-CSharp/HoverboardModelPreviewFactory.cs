using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverboardModelPreviewFactory : MonoBehaviour
{
	[Serializable]
	public class HoverboardModelSetup
	{
		public string name;

		public Vector3 eulerAngles;

		public float menuYPosition;

		public AnimationClip clipHangtime;

		public AnimationClip clipRun;

		public GameObject hoverboardPrefab;

		public GameObject hoverboardPrefabScrollMenu;
	}

	[SerializeField]
	private HoverboardModelSetup[] hoverboards;

	[SerializeField]
	private AnimationClip jumperSignature;

	[SerializeField]
	private AnimationClip speederSignature;

	[SerializeField]
	private AnimationClip doublejumperSignature;

	[SerializeField]
	private AnimationClip lowriderSignature;

	[SerializeField]
	private AnimationClip lowriderRunIdle;

	[SerializeField]
	private AnimationClip teleporterSignature;

	[SerializeField]
	private AnimationClip gliderSignature;

	[SerializeField]
	private AnimationClip vanitySignature;

	public Action OnOpenGlider;

	public Action OnCloseGlider;

	public Action OnSpeederStart;

	public Action OnSpeederEnd;

	private Dictionary<string, HoverboardModelSetup> name2character = new Dictionary<string, HoverboardModelSetup>();

	private string currentIdleClip;

	[SerializeField]
	private GameObject teleboardWarpPrefab;

	[SerializeField]
	private Material teleboardFXMaterial;

	private GameObject teleboardWarp;

	private AnimationState teleboardAnimation;

	private Vector3 teleboardWarpPosition;

	private GliderAnimation gliderAnimation;

	private readonly int id_Teleboard = Animator.StringToHash("Base Layer.HoverboardMenuTeleport");

	private readonly int id_Glider = Animator.StringToHash("Base Layer.HoverboardMenuGlider");

	private Animator animator;

	private Vector3 intialScale = new Vector3(-1f, -1f, -1f);

	private Transform root;

	private Material cachedModelMaterial;

	private Material cachedCustomizationMaterial;

	private Material[] cachedHeadMaterials;

	private Renderer cachedHoverboard;

	private Material cachedHoverboardMaterial;

	private Material cachedSupersneakersMaterial;

	private Material cachedDoubleJumperMaterial;

	private Renderer[] cachedCustomHoverboardModels;

	private Material[] cachedCustomHoverboardModelMaterials;

	private CharacterModel characterModel;

	private bool cachedCustomizationState;

	private bool teleportFxActive;

	private static HoverboardModelPreviewFactory instance;

	public static HoverboardModelPreviewFactory Instance
	{
		get
		{
			return instance ?? (instance = UnityEngine.Object.FindObjectOfType(typeof(HoverboardModelPreviewFactory)) as HoverboardModelPreviewFactory);
		}
	}

	public void Awake()
	{
		animator = base.gameObject.GetComponent<Animator>();
		UIScreenController.OnNavigateToScreen += HandleOnNavigateToScreen;
		UIScreenController.Instance.OnPopupShown += HandleOnPopupShown;
		UIScreenController.OnApplicationResumed += UIScreenController_OnApplicationResumed;
		HoverboardModelSetup[] array = hoverboards;
		foreach (HoverboardModelSetup hoverboardModelSetup in array)
		{
			name2character.Add(hoverboardModelSetup.name, hoverboardModelSetup);
		}
	}

	private void UIScreenController_OnApplicationResumed()
	{
		ResetAnimator();
		ResetTeleportFX();
	}

	private void HandleOnPopupShown()
	{
		ResetAnimator();
	}

	private void HandleOnNavigateToScreen(string obj)
	{
		if (obj == "FrontUI")
		{
			ResetAnimator();
		}
	}

	public GameObject GetHoverboardModelPreview(string name, Hoverboards.BoardType boardType)
	{
		HoverboardModelSetup value;
		if (name2character.TryGetValue(name, out value))
		{
			GameObject gameObject = new GameObject("Hoverboard: " + name);
			GameObject gameObject2 = ((!(value.hoverboardPrefabScrollMenu != null)) ? UnityEngine.Object.Instantiate(value.hoverboardPrefab) : UnityEngine.Object.Instantiate(value.hoverboardPrefabScrollMenu));
			gameObject2.transform.parent = gameObject.transform;
			Renderer[] componentsInChildren = gameObject2.GetComponentsInChildren<Renderer>();
			Hoverboard.HoverboardSelection.CustomSet[] activeCustomSets = Hoverboard.Instance.GetActiveCustomSets(boardType);
			if (activeCustomSets != null)
			{
				bool flag = true;
				if (HoverboardManager.Instance.IsCustomSetEquippedForBoard(boardType, 0) && !activeCustomSets[1].useDefaultmodel)
				{
					flag = false;
				}
				if (HoverboardManager.Instance.IsCustomSetEquippedForBoard(boardType, 1) && !activeCustomSets[2].useDefaultmodel)
				{
					flag = false;
				}
				if (flag && activeCustomSets.Length > 0 && activeCustomSets[0].modelPrefab != null)
				{
					AddCustomModel(activeCustomSets[0].modelPrefab, gameObject2.transform);
				}
				if (activeCustomSets.Length > 1 && HoverboardManager.Instance.IsCustomSetEquippedForBoard(boardType, 0))
				{
					if (activeCustomSets[1].baseModelPrefab != null)
					{
						AddCustomModel(activeCustomSets[1].baseModelPrefab, gameObject2.transform);
						Renderer[] array = componentsInChildren;
						foreach (Renderer renderer in array)
						{
							renderer.enabled = false;
						}
					}
					if (activeCustomSets[1].modelPrefab != null)
					{
						AddCustomModel(activeCustomSets[1].modelPrefab, gameObject2.transform);
					}
				}
				if (activeCustomSets.Length > 2 && HoverboardManager.Instance.IsCustomSetEquippedForBoard(boardType, 1))
				{
					if (activeCustomSets[2].baseModelPrefab != null)
					{
						AddCustomModel(activeCustomSets[2].baseModelPrefab, gameObject2.transform);
						Renderer[] array2 = componentsInChildren;
						foreach (Renderer renderer2 in array2)
						{
							renderer2.enabled = false;
						}
					}
					if (activeCustomSets[2].modelPrefab != null)
					{
						AddCustomModel(activeCustomSets[2].modelPrefab, gameObject2.transform);
					}
				}
			}
			gameObject2.transform.localRotation = Quaternion.Euler(value.eulerAngles) * Quaternion.Euler(0f, 180f, 0f);
			Vector3 localPosition = gameObject2.transform.localPosition;
			gameObject2.transform.localPosition = localPosition + new Vector3(0f, value.menuYPosition, 0f);
			GliderAnimation componentInChildren = gameObject2.GetComponentInChildren<GliderAnimation>();
			if (componentInChildren != null)
			{
				componentInChildren.RemoveEventHandling();
			}
			return gameObject;
		}
		Debug.LogError("GetHoverboardModelPreview() could not find preview character model for '" + name + "'.");
		return null;
	}

	private void AddCustomModel(GameObject prefab, Transform root)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
		gameObject.transform.parent = root;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		gameObject.layer = Layers.Instance._3DGUI;
		Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.gameObject.layer = Layers.Instance._3DGUI;
		}
	}

	public Quaternion GetHoverboardDefaultRotation(string name)
	{
		HoverboardModelSetup value;
		if (name2character.TryGetValue(name, out value))
		{
			return Quaternion.Euler(value.eulerAngles) * Quaternion.Euler(0f, 180f, 0f);
		}
		return Quaternion.Euler(194f, 110.5f, 180f);
	}

	private CharacterModel FindCharacterModel(GameObject characterAnimation)
	{
		CharacterModel characterModel = null;
		GameObject gameObject = characterAnimation;
		while (characterModel == null)
		{
			characterModel = gameObject.GetComponentInParent<CharacterModel>();
			Transform parent = gameObject.transform.parent;
			if (parent == null)
			{
				return null;
			}
			gameObject = parent.gameObject;
		}
		return characterModel;
	}

	public void ChangeHoverboard(Hoverboards.BoardType boardType, GameObject hoverboardGO, Animation characterAnimation, bool willUpdateAnimation, bool willAllowSignatureAnimation, Hoverboard.BoardAbility boardAbility)
	{
		if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == id_Teleboard && willUpdateAnimation)
		{
			StopAllCoroutines();
			ResetAnimator();
		}
		else if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == id_Glider)
		{
			ResetAnimator();
		}
		if (teleportFxActive)
		{
			ResetTeleportFX();
		}
		string boardModelName = Hoverboards.boardData[boardType].boardModelName;
		characterModel = FindCharacterModel(characterAnimation.gameObject);
		if (characterModel == null)
		{
			Debug.LogError("HoverboardModelPreviewFactory - characterModel was null!");
			return;
		}
		if (intialScale == new Vector3(-1f, -1f, -1f))
		{
			intialScale = characterModel.transform.localScale;
		}
		HoverboardModelSetup value;
		if (!name2character.TryGetValue(boardModelName, out value))
		{
			return;
		}
		if (boardModelName == "Jumpboard")
		{
			characterModel.MeshSuperSneaker.enabled = true;
		}
		else
		{
			characterModel.MeshSuperSneaker.enabled = false;
		}
		string boardName = "hoverboard";
		hoverboardGO = characterModel.SetNewHoverboard(hoverboardGO, value.hoverboardPrefab, boardName);
		cachedHoverboard = hoverboardGO.GetComponent<Renderer>();
		if (boardModelName == "gliderboard")
		{
			gliderAnimation = hoverboardGO.GetComponentInChildren<GliderAnimation>();
			if (gliderAnimation != null)
			{
				gliderAnimation.AddEvents(this);
			}
		}
		if (boardModelName == "teleboard")
		{
			CachMaterials();
		}
		Hoverboard.HoverboardSelection.CustomSet[] activeCustomSets = Hoverboard.Instance.GetActiveCustomSets(boardType);
		if (activeCustomSets == null)
		{
			return;
		}
		bool flag = true;
		if (activeCustomSets.Length > 1 && HoverboardManager.Instance.ShouldCustomSetBeDisplayed(0) && !activeCustomSets[1].useDefaultmodel)
		{
			flag = false;
		}
		if (activeCustomSets.Length > 2 && HoverboardManager.Instance.ShouldCustomSetBeDisplayed(1) && !activeCustomSets[2].useDefaultmodel)
		{
			flag = false;
		}
		if (flag == activeCustomSets.Length > 0)
		{
			characterModel.AddCustomHoverboardModel(activeCustomSets[0].modelPrefab, true, hoverboardGO.transform);
		}
		if (activeCustomSets.Length == 1 && willAllowSignatureAnimation)
		{
			if (activeCustomSets[0].ability != Hoverboard.BoardAbility.None)
			{
				if (activeCustomSets[0].ability == Hoverboard.BoardAbility.Teleport)
				{
					CachMaterials();
				}
				if (activeCustomSets[0].VanityMenuFxPrefab != null)
				{
					characterModel.AddCustomHoverboardVanityMenuFX(activeCustomSets[0].VanityMenuFxPrefab, hoverboardGO.transform);
				}
				if (activeCustomSets[0].ability == Hoverboard.BoardAbility.Lowrider)
				{
					PlaySignatureMove(activeCustomSets[0].ability, characterAnimation, true);
				}
				else
				{
					PlaySignatureMove(activeCustomSets[0].ability, characterAnimation, false);
				}
			}
			return;
		}
		if (HoverboardManager.Instance.ShouldCustomSetBeDisplayed(0))
		{
			AddCustomModelsAndFX(activeCustomSets, 1, hoverboardGO.transform);
		}
		if (HoverboardManager.Instance.ShouldCustomSetBeDisplayed(1))
		{
			AddCustomModelsAndFX(activeCustomSets, 2, hoverboardGO.transform);
		}
		if (!willUpdateAnimation)
		{
			return;
		}
		bool flag2 = false;
		bool flag3 = false;
		for (int i = 0; i < activeCustomSets.Length; i++)
		{
			if (i == 0 && activeCustomSets[i].ability == Hoverboard.BoardAbility.Lowrider)
			{
				flag2 = true;
			}
			if (i >= 1 && activeCustomSets[i].ability == Hoverboard.BoardAbility.Lowrider && HoverboardManager.Instance.ShouldCustomSetBeDisplayed(i - 1))
			{
				flag2 = true;
			}
			if (i == 0 && activeCustomSets[i].ability == Hoverboard.BoardAbility.Glider)
			{
				flag3 = true;
			}
		}
		if (boardAbility == Hoverboard.BoardAbility.None)
		{
			if (characterAnimation[value.clipHangtime.name] == null)
			{
				characterAnimation.AddClip(value.clipHangtime, value.clipHangtime.name);
			}
			characterAnimation[value.clipHangtime.name].wrapMode = WrapMode.Once;
			characterAnimation.Play(value.clipHangtime.name);
			if (flag2)
			{
				if (characterAnimation[lowriderRunIdle.name] == null)
				{
					characterAnimation.AddClip(lowriderRunIdle, lowriderRunIdle.name);
				}
				currentIdleClip = lowriderRunIdle.name;
				characterAnimation.CrossFadeQueued(lowriderRunIdle.name, 0.2f);
			}
			else
			{
				if (characterAnimation[value.clipRun.name] == null)
				{
					characterAnimation.AddClip(value.clipRun, value.clipRun.name);
				}
				currentIdleClip = value.clipRun.name;
				characterAnimation.CrossFadeQueued(value.clipRun.name, 0.2f);
			}
			if (flag3 && animator.GetCurrentAnimatorStateInfo(0).fullPathHash != id_Glider)
			{
				animator.Play(id_Glider);
			}
		}
		else if (willAllowSignatureAnimation)
		{
			PlaySignatureMove(boardAbility, characterAnimation, flag2);
		}
		else
		{
			Debug.LogWarning("Ok? willAllowSignatureAnimation is : " + willAllowSignatureAnimation);
		}
	}

	private void AddCustomModelsAndFX(Hoverboard.HoverboardSelection.CustomSet[] customSets, int i, Transform hoverboardRoot)
	{
		if (customSets.Length <= i)
		{
			return;
		}
		if (customSets[i].ability == Hoverboard.BoardAbility.Teleport)
		{
			CachMaterials();
		}
		if (customSets[i].baseModelPrefab != null)
		{
			cachedHoverboard.enabled = false;
			cachedHoverboard = characterModel.AddCustomHoverboardModel(customSets[i].baseModelPrefab, true, hoverboardRoot);
		}
		characterModel.AddCustomHoverboardModel(customSets[i].modelPrefab, true, hoverboardRoot);
		if (customSets[i].ability == Hoverboard.BoardAbility.Glider)
		{
			gliderAnimation = hoverboardRoot.gameObject.GetComponentInChildren<GliderAnimation>();
			if (gliderAnimation != null)
			{
				gliderAnimation.AddEvents(this);
			}
		}
		if (customSets[i].VanityMenuFxPrefab != null)
		{
			characterModel.AddCustomHoverboardVanityMenuFX(customSets[i].VanityMenuFxPrefab, hoverboardRoot);
		}
	}

	public void PlaySignatureMove(Hoverboard.BoardAbility ability, Animation characterAnimation, bool useLowrider)
	{
		if (teleportFxActive)
		{
			ResetTeleportFX();
		}
		switch (ability)
		{
		case Hoverboard.BoardAbility.DoubleJump:
			if (characterAnimation[doublejumperSignature.name] == null)
			{
				characterAnimation.AddClip(doublejumperSignature, doublejumperSignature.name);
			}
			if (characterAnimation.isPlaying)
			{
				characterAnimation.CrossFade(doublejumperSignature.name, 0.2f);
			}
			else
			{
				characterAnimation.Play(doublejumperSignature.name);
			}
			break;
		case Hoverboard.BoardAbility.Glider:
			if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash != id_Glider)
			{
				animator.Play(id_Glider);
			}
			if (characterAnimation[gliderSignature.name] == null)
			{
				characterAnimation.AddClip(gliderSignature, gliderSignature.name);
			}
			if (characterAnimation.isPlaying)
			{
				characterAnimation.CrossFade(gliderSignature.name, 0.2f);
			}
			else
			{
				characterAnimation.Play(gliderSignature.name);
			}
			break;
		case Hoverboard.BoardAbility.HighSpeed:
		case Hoverboard.BoardAbility.Speed:
			StartCoroutine(SpeederEvents(speederSignature.length));
			if (characterAnimation[speederSignature.name] == null)
			{
				characterAnimation.AddClip(speederSignature, speederSignature.name);
			}
			if (characterAnimation.isPlaying)
			{
				characterAnimation.CrossFade(speederSignature.name, 0.2f);
			}
			else
			{
				characterAnimation.Play(speederSignature.name);
			}
			break;
		case Hoverboard.BoardAbility.Jumper:
			if (characterAnimation[jumperSignature.name] == null)
			{
				characterAnimation.AddClip(jumperSignature, jumperSignature.name);
			}
			if (characterAnimation.isPlaying)
			{
				characterAnimation.CrossFade(jumperSignature.name, 0.2f);
			}
			else
			{
				characterAnimation.Play(jumperSignature.name);
			}
			break;
		case Hoverboard.BoardAbility.Lowrider:
			if (characterAnimation[lowriderSignature.name] == null)
			{
				characterAnimation.AddClip(lowriderSignature, lowriderSignature.name);
			}
			if (characterAnimation.isPlaying)
			{
				characterAnimation.CrossFade(lowriderSignature.name, 0.2f);
			}
			else
			{
				characterAnimation.Play(lowriderSignature.name);
			}
			break;
		case Hoverboard.BoardAbility.Teleport:
			root = characterModel.Hip;
			if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash != id_Teleboard)
			{
				animator.Play(id_Teleboard);
			}
			if (characterAnimation[teleporterSignature.name] == null)
			{
				characterAnimation.AddClip(teleporterSignature, teleporterSignature.name);
			}
			if (characterAnimation.isPlaying)
			{
				characterAnimation.CrossFade(teleporterSignature.name, 0.2f);
			}
			else
			{
				characterAnimation.Play(teleporterSignature.name);
			}
			break;
		case Hoverboard.BoardAbility.Vanity:
			if (characterAnimation[vanitySignature.name] == null)
			{
				characterAnimation.AddClip(vanitySignature, vanitySignature.name);
			}
			if (characterAnimation.isPlaying)
			{
				characterAnimation.CrossFade(vanitySignature.name, 0.2f);
			}
			else
			{
				characterAnimation.Play(vanitySignature.name);
			}
			break;
		}
		if (useLowrider)
		{
			if (characterAnimation[lowriderRunIdle.name] == null)
			{
				characterAnimation.AddClip(lowriderRunIdle, lowriderRunIdle.name);
			}
			characterAnimation.CrossFadeQueued(lowriderRunIdle.name, 0.2f);
		}
		else
		{
			characterAnimation.CrossFadeQueued(currentIdleClip, 0.2f);
		}
	}

	private IEnumerator SpeederEvents(float waitTime)
	{
		if (OnSpeederStart != null)
		{
			OnSpeederStart();
		}
		yield return new WaitForSeconds(waitTime);
		if (OnSpeederEnd != null)
		{
			OnSpeederEnd();
		}
	}

	private void CachMaterials()
	{
		if (characterModel != null)
		{
			Material sharedMaterial = characterModel.Model.sharedMaterial;
			if (sharedMaterial != teleboardFXMaterial)
			{
				cachedModelMaterial = sharedMaterial;
			}
			Material[] sharedMaterials = characterModel.HeadModel.sharedMaterials;
			if (sharedMaterials[0] != teleboardFXMaterial)
			{
				cachedHeadMaterials = sharedMaterials;
			}
			Material sharedMaterial2 = characterModel.OutfitModel.sharedMaterial;
			if (sharedMaterial2 != teleboardFXMaterial)
			{
				cachedCustomizationMaterial = sharedMaterial2;
			}
			cachedCustomizationState = characterModel.OutfitModel.enabled;
		}
	}

	private void TeleboardStart(float duration)
	{
		if (teleportFxActive)
		{
			ResetTeleportFX();
			ResetAnimator();
			StopCoroutine("TeleportFXStartScale");
		}
		teleportFxActive = true;
		if (characterModel == null)
		{
			return;
		}
		if (teleboardWarp == null)
		{
			teleboardWarp = UnityEngine.Object.Instantiate(teleboardWarpPrefab);
			float num = 0.07f;
			teleboardWarp.transform.localScale = new Vector3(num, num, num);
			teleboardWarp.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			teleboardWarp.GetComponentInChildren<Renderer>().gameObject.layer = Layers.Instance._3DGUI;
			teleboardAnimation = teleboardWarp.GetComponent<Animation>()["teleboardWarp"];
			teleboardAnimation.normalizedTime = 0f;
			teleboardAnimation.weight = 1f;
			teleboardAnimation.enabled = true;
			teleboardAnimation.speed = 0f;
			teleboardWarp.SetActive(false);
		}
		characterModel.Model.GetComponent<Renderer>().material = teleboardFXMaterial;
		Material[] array = new Material[characterModel.HeadModel.materials.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = teleboardFXMaterial;
		}
		characterModel.HeadModel.materials = array;
		characterModel.OutfitModel.material = teleboardFXMaterial;
		Material sharedMaterial = cachedHoverboard.sharedMaterial;
		if (sharedMaterial != teleboardFXMaterial)
		{
			cachedHoverboardMaterial = sharedMaterial;
		}
		cachedHoverboard.material = teleboardFXMaterial;
		Material sharedMaterial2 = characterModel.MeshSuperSneaker.GetComponent<Renderer>().sharedMaterial;
		if (sharedMaterial2 != teleboardFXMaterial)
		{
			cachedSupersneakersMaterial = sharedMaterial2;
		}
		characterModel.MeshSuperSneaker.GetComponent<Renderer>().material = teleboardFXMaterial;
		Material sharedMaterial3 = characterModel.DoubleJumpers.GetComponent<Renderer>().sharedMaterial;
		if (sharedMaterial3 != teleboardFXMaterial)
		{
			cachedDoubleJumperMaterial = sharedMaterial3;
		}
		characterModel.DoubleJumpers.GetComponent<Renderer>().material = teleboardFXMaterial;
		characterModel.StopEyeAnimations();
		if (characterModel.CurrentCustomBoardModels.Count > 0)
		{
			cachedCustomHoverboardModelMaterials = new Material[characterModel.CurrentCustomBoardModels.Count];
			cachedCustomHoverboardModels = new Renderer[characterModel.CurrentCustomBoardModels.Count];
			for (int j = 0; j < characterModel.CurrentCustomBoardModels.Count; j++)
			{
				cachedCustomHoverboardModels[j] = characterModel.CurrentCustomBoardModels[j].GetComponentInChildren<Renderer>();
				Material material = null;
				if (cachedCustomHoverboardModels[j] != null)
				{
					material = cachedCustomHoverboardModels[j].sharedMaterial;
				}
				if (material != teleboardFXMaterial && material != null)
				{
					cachedCustomHoverboardModelMaterials[j] = material;
				}
			}
			for (int k = 0; k < characterModel.CurrentCustomBoardModels.Count; k++)
			{
				if (cachedCustomHoverboardModels[k] != null)
				{
					cachedCustomHoverboardModels[k].material = teleboardFXMaterial;
				}
			}
		}
		StartCoroutine(TeleportFXStartScale(duration));
	}

	private IEnumerator TeleportFXStartScale(float duration)
	{
		float time = 0f;
		float t = 0f;
		while (time < duration)
		{
			time += Time.deltaTime;
			t = Mathf.Clamp01(time / duration);
			if (characterModel == null)
			{
				break;
			}
			characterModel.transform.localScale = new Vector3((1f - 0.5f * t) * intialScale.x, (1f + 0.5f * t) * intialScale.y, (1f - t) * intialScale.z);
			if (t == 1f && teleportFxActive)
			{
				characterModel.transform.localScale = Vector3.zero;
				cachedHoverboard.enabled = false;
				characterModel.Model.enabled = false;
				characterModel.HeadModel.enabled = false;
				SetEnabledWithinCurrentCustomHoverboardModels(false);
				characterModel.OutfitModel.enabled = false;
				if (characterModel.CurrentHoverboard != null)
				{
					characterModel.CurrentHoverboard.SetActive(false);
				}
			}
			yield return null;
		}
	}

	private void TeleboardEnd(float duration)
	{
		if (characterModel == null)
		{
			return;
		}
		characterModel.transform.localScale = intialScale;
		if (duration > 0f)
		{
			teleboardWarp.SetActive(true);
			teleboardAnimation.normalizedTime = 1f;
			teleboardAnimation.weight = 1f;
			teleboardAnimation.speed = -1.8f;
			teleboardAnimation.enabled = true;
		}
		if (root != null)
		{
			teleboardWarp.transform.position = root.position;
		}
		StartCoroutine(pTween.To(teleboardAnimation.length, delegate(float t)
		{
			if (t == 1f)
			{
				teleboardWarp.SetActive(false);
				animator.StopPlayback();
			}
		}));
		StartCoroutine(pTween.To(duration, delegate(float t)
		{
			if (t == 1f && teleportFxActive)
			{
				ResetTeleportFX();
			}
		}));
	}

	private IEnumerator TeleboardEndTimer(float duration)
	{
		float time = 0f;
		float t = 0f;
		while (time < duration)
		{
			time += Time.deltaTime;
			t = Mathf.Clamp01(time / duration);
			if (t == 1f && teleportFxActive)
			{
				teleboardWarp.SetActive(false);
				animator.StopPlayback();
				ResetTeleportFX();
			}
			yield return null;
		}
	}

	public void ResetTeleportFX()
	{
		if (characterModel == null || !teleportFxActive)
		{
			return;
		}
		characterModel.Model.enabled = true;
		characterModel.HeadModel.enabled = true;
		cachedHoverboard.enabled = true;
		SetEnabledWithinCurrentCustomHoverboardModels(true);
		characterModel.OutfitModel.enabled = cachedCustomizationState;
		if (characterModel.CurrentHoverboard != null)
		{
			characterModel.CurrentHoverboard.SetActive(true);
		}
		if (cachedModelMaterial != null)
		{
			characterModel.Model.GetComponent<Renderer>().material = cachedModelMaterial;
		}
		if (characterModel.HeadModel != null && characterModel.HeadModel.materials != null && cachedHeadMaterials != null)
		{
			characterModel.HeadModel.materials = cachedHeadMaterials;
		}
		characterModel.OutfitModel.material = cachedCustomizationMaterial;
		characterModel.MeshSuperSneaker.material = cachedSupersneakersMaterial;
		characterModel.DoubleJumpers.material = cachedDoubleJumperMaterial;
		characterModel.StartEyeAnimations(UIModelController.ModelScreen.Boards);
		if (cachedHoverboard != null)
		{
			cachedHoverboard.material = cachedHoverboardMaterial;
		}
		if (cachedCustomHoverboardModelMaterials != null && cachedCustomHoverboardModelMaterials.Length > 0)
		{
			for (int i = 0; i < characterModel.CurrentCustomBoardModels.Count; i++)
			{
				if (cachedCustomHoverboardModelMaterials.Length - 1 >= i)
				{
					characterModel.CurrentCustomBoardModelsRenderes[i].material = cachedCustomHoverboardModelMaterials[i];
				}
			}
		}
		characterModel.transform.localScale = intialScale;
		teleportFxActive = false;
	}

	private void SetEnabledWithinCurrentCustomHoverboardModels(bool isEnabled)
	{
		if (cachedCustomHoverboardModels == null)
		{
			return;
		}
		Renderer[] array = cachedCustomHoverboardModels;
		foreach (Renderer renderer in array)
		{
			if (renderer != null)
			{
				renderer.enabled = isEnabled;
			}
		}
	}

	private void OpenGlider()
	{
		if (OnOpenGlider != null)
		{
			OnOpenGlider();
		}
	}

	private void CloseGlider()
	{
		if (OnCloseGlider != null)
		{
			OnCloseGlider();
		}
	}

	public void ResetAnimator()
	{
		if (animator != null)
		{
			animator.Play("empty");
		}
	}

	private void OnDestroy()
	{
		instance = null;
	}
}
