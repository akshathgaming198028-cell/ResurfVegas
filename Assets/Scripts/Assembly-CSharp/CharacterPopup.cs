using SYBO.Subway.Info;
using UnityEngine;

public class CharacterPopup : UIScreen
{
	private CharacterModel characterModel;

	private CharacterModel secondaryCharacterModel;

	protected UIModelController uimodelController;

	public override void Init()
	{
		base.Init();
		uimodelController = UIModelController.Instance;
	}

	protected void SetCharacter(Characters.CharacterType character, Vector2? offsetFromAnchor = null, Vector3? rotationOffset = null, float scale = 19f, bool posing = false, AnimationClip pose = null)
	{
		uimodelController.ClearTutorialPopup();
		if (characterModel == null)
		{
			Vector3 localPosition = ((!offsetFromAnchor.HasValue) ? ((Vector2)new Vector3(35f, 23f, 50f)) : offsetFromAnchor.Value);
			localPosition.z = 50f;
			characterModel = NGUITools.AddChild(uimodelController.TutorialPopupAnchor, uimodelController.ModelPrefab).GetComponent<CharacterModel>();
			Utility.SetLayerRecursively(characterModel.transform, uimodelController.TutorialPopupAnchor.layer);
			Transform transform = characterModel.transform;
			transform.localPosition = localPosition;
			transform.localScale = new Vector3(scale, scale, scale);
			transform.localEulerAngles = ((!posing) ? new Vector3(50f, 200f, 0f) : rotationOffset.Value);
			characterModel.HideBlobShadow();
		}
		string modelName = Characters.characterData[character].modelName;
		string currentPopupName = UIScreenController.Instance.GetCurrentPopupName();
		characterModel.ChangeCharacterModel(modelName, 0, (!(currentPopupName == "WelcomePopup")) ? true : false, false);
		characterModel.HideAllPowerups();
		if (posing)
		{
			Animation characterAnimation = characterModel.CharacterAnimation;
			characterAnimation.AddClip(pose, pose.name);
			characterAnimation.Play(pose.name);
		}
		else
		{
			characterModel.StartIdlePopupAnimations();
		}
		CharacterScaler component = uimodelController.TutorialPopupAnchor.GetComponent<CharacterScaler>();
		if (component != null)
		{
			component.lookAtCamera = true;
		}
	}

	protected virtual void OnDisable()
	{
		if (!(UIScreenController.Instance == null) && !UIScreenController.Instance.stoppingFromEditor)
		{
			uimodelController.ClearTutorialPopup();
		}
	}
}
