using System;
using SYBO.Core.Utilities;
using UnityEngine;

public class CharacterModelPreviewFactory : MonoBehaviour
{
	[Serializable]
	public class ModelSetupObject
	{
		public AnimationClip ScrollbarPose;

		public AnimationClip ScrollbarHeadPose;

		public AnimationClip EventScreenPose;

		public AnimationClip NameEventScreenIdle;

		public GameObject model;
	}

	[SerializeField]
	private GameObject characterModelPreviewPrefab;

	[SerializeField]
	private CharacterAssetFactory characterAssetFactory;

	[SerializeField]
	private AnimationClip facebookIdle;

	[SerializeField]
	private AnimationClip defaultEventScreenPose;

	[SerializeField]
	private AnimationClip defaultEnventScreenIdle;

	[SerializeField]
	private Shader eventScreenShader;

	[SerializeField]
	private Shader eventScreenOutfitShader;

	[SerializeField]
	[Range(0f, 1f)]
	private float _nameEventScreenCharacterFadeFactor = 0.6f;

	[SerializeField]
	[Range(0f, 1f)]
	private float _nameEventScreenShadowFadeFactor = 0.7f;

	private SkinnedMeshRenderer headMesh;

	private static CharacterModelPreviewFactory instance;

	public static CharacterModelPreviewFactory Instance
	{
		get
		{
			return instance ?? (instance = UnityEngine.Object.FindObjectOfType(typeof(CharacterModelPreviewFactory)) as CharacterModelPreviewFactory);
		}
	}

	public void GetCharacterModelPreview(string name, int version, Action<GameObject> onSucess)
	{
		BuildCharacter(name, version, false, false, true, delegate(ModelSetupObject go)
		{
			if (go != null)
			{
				if (go.ScrollbarPose != null)
				{
					Animation componentInChildren = go.model.GetComponentInChildren<Animation>();
					if (componentInChildren.GetClip(go.ScrollbarPose.name) == null)
					{
						componentInChildren.AddClip(go.ScrollbarPose, go.ScrollbarPose.name);
						componentInChildren.Play(go.ScrollbarPose.name);
						componentInChildren[go.ScrollbarPose.name].speed = 0f;
					}
					if (go.ScrollbarHeadPose != null)
					{
						componentInChildren.AddClip(go.ScrollbarHeadPose, go.ScrollbarHeadPose.name);
						componentInChildren[go.ScrollbarHeadPose.name].blendMode = AnimationBlendMode.Blend;
						componentInChildren[go.ScrollbarHeadPose.name].weight = 1f;
						componentInChildren[go.ScrollbarHeadPose.name].layer = 10;
						componentInChildren[go.ScrollbarHeadPose.name].enabled = true;
						componentInChildren[go.ScrollbarHeadPose.name].speed = 0f;
						componentInChildren[go.ScrollbarHeadPose.name].normalizedTime = 0f;
					}
				}
				else
				{
					Debug.LogError("could not find Scrollbar Pose for '" + name + "'.");
				}
			}
			onSucess(go.model);
		});
	}

	public void GetEventScreenModel(string name, int version, Action<GameObject> onSucess)
	{
		BuildCharacter(name, version, false, true, false, delegate(ModelSetupObject MSO)
		{
			if (MSO != null)
			{
				Animation componentInChildren = MSO.model.GetComponentInChildren<Animation>();
				if (MSO.EventScreenPose != null)
				{
					componentInChildren.AddClip(MSO.EventScreenPose, "eventClip");
					componentInChildren.Play("eventClip");
					componentInChildren["eventClip"].speed = 0f;
				}
				else
				{
					componentInChildren.AddClip(defaultEventScreenPose, "defaultEventClip");
					componentInChildren.Play("defaultEventClip");
					componentInChildren["defaultEventClip"].speed = 0f;
				}
			}
			onSucess(MSO.model);
		});
	}

	public void GetFacebookScreenModel(string name, Action<GameObject> onSucess)
	{
		BuildCharacter(name, 0, false, false, false, delegate(ModelSetupObject go)
		{
			if (go != null)
			{
				Animation componentInChildren = go.model.GetComponentInChildren<Animation>();
				if (facebookIdle != null)
				{
					componentInChildren.AddClip(facebookIdle, "facebookClip");
					componentInChildren.Play("facebookClip");
				}
			}
			onSucess(go.model);
		});
	}

	public void GetCharacterEventScreenModel(string name, int version, Action<CharacterScreenController> onSucess)
	{
		BuildCharacter(name, version, true, true, false, delegate(ModelSetupObject mso)
		{
			if (mso != null)
			{
				CharacterScreenController characterScreenController = null;
				characterScreenController = mso.model.AddComponent<CharacterScreenController>();
				AnimationClip idleClip = ((!(mso.NameEventScreenIdle != null)) ? defaultEnventScreenIdle : mso.NameEventScreenIdle);
				characterScreenController.Initialize(idleClip, eventScreenShader, eventScreenOutfitShader, _nameEventScreenCharacterFadeFactor, _nameEventScreenShadowFadeFactor);
				onSucess(characterScreenController);
			}
			else
			{
				onSucess(null);
			}
		});
	}

	private void BuildCharacter(string name, int version, bool useSpecialAsset, bool useAnimatedEyes, bool forceLOD0, Action<ModelSetupObject> onSucess)
	{
		CharacterAssetFactory.GetAssetPrefab(name, delegate(CharacterAssetsBundle modelBundle)
		{
			if (modelBundle != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(characterModelPreviewPrefab);
				modelBundle.transform.parent = gameObject.transform;
				CharacterRig componentInChildren = gameObject.GetComponentInChildren<CharacterRig>();
				componentInChildren.Model.sharedMesh = modelBundle.MainMesh;
				if (modelBundle.HeadMesh != null)
				{
					componentInChildren.HeadModel.sharedMesh = modelBundle.HeadMesh;
				}
				else
				{
					componentInChildren.HeadModel.enabled = false;
				}
				componentInChildren.OutfitModel.enabled = false;
				componentInChildren.RocketSkates.enabled = false;
				componentInChildren.MeshSuperSneaker.enabled = false;
				componentInChildren.DoubleJumpers.enabled = false;
				componentInChildren.CharacterCustomization.Customize(name, version, componentInChildren.Model, componentInChildren.OutfitModel, componentInChildren.HeadModel, componentInChildren, useSpecialAsset, false, forceLOD0);
				if (useAnimatedEyes)
				{
					AvatarEyeAnimation componentInChildren2 = gameObject.GetComponentInChildren<AvatarEyeAnimation>();
					if (componentInChildren2 != null)
					{
						GameObject gameObject2 = new GameObject("FocusPoint");
						HierarchyUtility.ReparentByHierarchyPathType(gameObject2.transform, HierarchyPathType.Dynamic);
						componentInChildren2.Initialize(gameObject2, componentInChildren.HeadJoint, componentInChildren.EyeSwitch, componentInChildren.EyeFocusOrigin, componentInChildren.EyeFocusTarget, componentInChildren.BlendShapeController, componentInChildren.HeadModel, modelBundle.EyeMaterialID);
					}
				}
				ModelSetupObject obj = new ModelSetupObject
				{
					model = gameObject,
					ScrollbarPose = modelBundle.ScrollbarPose,
					ScrollbarHeadPose = modelBundle.ScrollbarHeadPose,
					EventScreenPose = modelBundle.EventScreenPose,
					NameEventScreenIdle = modelBundle.NameEventScreenIdle
				};
				onSucess(obj);
			}
			else
			{
				Debug.LogError("could not find preview character model for '" + name + "'.");
				onSucess(null);
			}
		});
	}

	private void OnDestroy()
	{
		instance = null;
	}
}
