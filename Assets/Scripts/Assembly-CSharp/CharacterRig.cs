using UnityEngine;

public class CharacterRig : MonoBehaviour
{
	[SerializeField]
	private string modelName;

	[SerializeField]
	private string outfitModelName;

	[SerializeField]
	private string headModelName;

	[SerializeField]
	private string meshSuperSneakerName;

	[SerializeField]
	private string doubleJumpersName;

	[SerializeField]
	private string rocketSkatesName;

	[SerializeField]
	private string spineTransformName;

	[SerializeField]
	private string rightShoulderTransformName;

	[SerializeField]
	private string attachPoint1Name;

	[SerializeField]
	private string attachPoint2Name;

	[SerializeField]
	private string attachPoint3Name;

	[SerializeField]
	private string hipName;

	[SerializeField]
	private string leftFootName;

	[SerializeField]
	private string rightFootName;

	[SerializeField]
	private string headJointName;

	[SerializeField]
	private string rightHandName;

	[SerializeField]
	private string eyeFocusPointOriginName;

	[SerializeField]
	private string eyeFocusPointTargetName;

	[SerializeField]
	private string eyeSwitchName;

	[SerializeField]
	private string blendShapeControllerName;

	[SerializeField]
	private Material powerupMaterial;

	[OptionalField]
	private Animation characterAnimation;

	[OptionalField]
	private AvatarAnimations avatarAnimations;

	[OptionalField]
	private CharacterCustomization characterCustomization;

	[OptionalField]
	private SkinnedMeshRenderer model;

	[OptionalField]
	private SkinnedMeshRenderer outfitModel;

	[OptionalField]
	private SkinnedMeshRenderer headModel;

	[OptionalField]
	private SkinnedMeshRenderer meshSuperSneaker;

	[OptionalField]
	private SkinnedMeshRenderer doubleJumpers;

	[OptionalField]
	private SkinnedMeshRenderer rocketSkates;

	[OptionalField]
	private Transform spineTransform;

	[OptionalField]
	private Transform rightShoulderTransform;

	[OptionalField]
	private Transform attachPoint1;

	[OptionalField]
	private Transform attachPoint2;

	[OptionalField]
	private Transform attachPoint3;

	[OptionalField]
	private Transform hip;

	[OptionalField]
	private Transform leftFoot;

	[OptionalField]
	private Transform rightFoot;

	[OptionalField]
	private Transform headJoint;

	[OptionalField]
	private Transform rightHand;

	[OptionalField]
	private Transform eyeFocusTarget;

	[OptionalField]
	private Transform eyeFocusOrigin;

	[OptionalField]
	private Transform eyeSwitch;

	[OptionalField]
	private Transform blendShapeController;

	public Animation CharacterAnimation
	{
		get
		{
			return characterAnimation;
		}
	}

	public AvatarAnimations AvatarAnimations
	{
		get
		{
			return avatarAnimations;
		}
	}

	public CharacterCustomization CharacterCustomization
	{
		get
		{
			return characterCustomization;
		}
	}

	public SkinnedMeshRenderer Model
	{
		get
		{
			return model;
		}
	}

	public SkinnedMeshRenderer OutfitModel
	{
		get
		{
			return outfitModel;
		}
	}

	public SkinnedMeshRenderer HeadModel
	{
		get
		{
			return headModel;
		}
	}

	public SkinnedMeshRenderer MeshSuperSneaker
	{
		get
		{
			return meshSuperSneaker;
		}
	}

	public SkinnedMeshRenderer DoubleJumpers
	{
		get
		{
			return doubleJumpers;
		}
	}

	public SkinnedMeshRenderer RocketSkates
	{
		get
		{
			return rocketSkates;
		}
	}

	public Transform SpineTransform
	{
		get
		{
			return spineTransform;
		}
	}

	public Transform ShoulderTransform
	{
		get
		{
			return rightShoulderTransform;
		}
	}

	public Transform AttachPoint1
	{
		get
		{
			return attachPoint1;
		}
	}

	public Transform AttachPoint2
	{
		get
		{
			return attachPoint2;
		}
	}

	public Transform AttachPoint3
	{
		get
		{
			return attachPoint3;
		}
	}

	public Transform Hip
	{
		get
		{
			return hip;
		}
	}

	public Transform LeftFoot
	{
		get
		{
			return leftFoot;
		}
	}

	public Transform RightFoot
	{
		get
		{
			return rightFoot;
		}
	}

	public Transform HeadJoint
	{
		get
		{
			return headJoint;
		}
	}

	public Transform RightHand
	{
		get
		{
			return rightHand;
		}
	}

	public Transform EyeFocusTarget
	{
		get
		{
			return eyeFocusTarget;
		}
	}

	public Transform EyeFocusOrigin
	{
		get
		{
			return eyeFocusOrigin;
		}
	}

	public Transform EyeSwitch
	{
		get
		{
			return eyeSwitch;
		}
	}

	public Transform BlendShapeController
	{
		get
		{
			return blendShapeController;
		}
	}

	private void Awake()
	{
		characterAnimation = GetComponentInChildren<Animation>();
		characterCustomization = GetComponent<CharacterCustomization>();
		AnimationEventBroadcaster componentInChildren = base.gameObject.GetComponentInChildren<AnimationEventBroadcaster>();
		if (componentInChildren == null)
		{
			characterAnimation.gameObject.AddComponent<AnimationEventBroadcaster>();
		}
		SkinnedMeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		SkinnedMeshRenderer[] array = componentsInChildren;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
		{
			if (skinnedMeshRenderer.name == modelName)
			{
				model = skinnedMeshRenderer;
			}
			if (skinnedMeshRenderer.name == outfitModelName)
			{
				outfitModel = skinnedMeshRenderer;
			}
			if (skinnedMeshRenderer.name == headModelName)
			{
				headModel = skinnedMeshRenderer;
			}
			if (skinnedMeshRenderer.name == meshSuperSneakerName)
			{
				meshSuperSneaker = skinnedMeshRenderer;
				meshSuperSneaker.material = powerupMaterial;
			}
			if (skinnedMeshRenderer.name == doubleJumpersName)
			{
				doubleJumpers = skinnedMeshRenderer;
				doubleJumpers.material = powerupMaterial;
			}
			if (skinnedMeshRenderer.name == rocketSkatesName)
			{
				rocketSkates = skinnedMeshRenderer;
				rocketSkates.material = powerupMaterial;
			}
		}
		Transform[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Transform>();
		Transform[] array2 = componentsInChildren2;
		foreach (Transform transform in array2)
		{
			if (transform.name == spineTransformName)
			{
				spineTransform = transform;
			}
			if (transform.name == rightShoulderTransformName)
			{
				rightShoulderTransform = transform;
			}
			if (transform.name == attachPoint1Name)
			{
				attachPoint1 = transform;
			}
			if (transform.name == attachPoint2Name)
			{
				attachPoint2 = transform;
			}
			if (transform.name == attachPoint3Name)
			{
				attachPoint3 = transform;
			}
			if (transform.name == hipName)
			{
				hip = transform;
			}
			if (transform.name == leftFootName)
			{
				leftFoot = transform;
			}
			if (transform.name == rightFootName)
			{
				rightFoot = transform;
			}
			if (transform.name == headJointName)
			{
				headJoint = transform;
			}
			if (transform.name == rightHandName)
			{
				rightHand = transform;
			}
			if (transform.name == eyeFocusPointOriginName)
			{
				eyeFocusOrigin = transform;
			}
			if (transform.name == eyeFocusPointTargetName)
			{
				eyeFocusTarget = transform;
			}
			if (transform.name == eyeSwitchName)
			{
				eyeSwitch = transform;
			}
			if (transform.name == blendShapeControllerName)
			{
				blendShapeController = transform;
			}
		}
		avatarAnimations = base.gameObject.GetComponent<AvatarAnimations>();
		avatarAnimations.Target = characterAnimation;
		avatarAnimations.Head = headModel;
		avatarAnimations.BlendshapeController = blendShapeController;
	}
}
