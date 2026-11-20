using UnityEngine;

public class SpecialAssetAttachment : CharacterCustomizationProp
{
	public enum AttachmentPoint
	{
		LeftHand = 0,
		RightHand = 1,
		Head = 2,
		AttachmentPoint1 = 3,
		AttachmentPoint2 = 4,
		AttachmentPoint3 = 5
	}

	[SerializeField]
	private AttachmentPoint attachmentPoint;

	public override void Initialize(CharacterRig characterRig, bool alternateSetup)
	{
		if (attachmentPoint == AttachmentPoint.LeftHand)
		{
			base.transform.parent = characterRig.LeftFoot;
		}
		else if (attachmentPoint == AttachmentPoint.RightHand)
		{
			base.transform.parent = characterRig.RightHand;
		}
		else if (attachmentPoint == AttachmentPoint.Head)
		{
			base.transform.parent = characterRig.HeadJoint;
		}
		else if (attachmentPoint == AttachmentPoint.AttachmentPoint1)
		{
			base.transform.parent = characterRig.AttachPoint1;
		}
		else if (attachmentPoint == AttachmentPoint.AttachmentPoint2)
		{
			base.transform.parent = characterRig.AttachPoint2;
		}
		else if (attachmentPoint == AttachmentPoint.AttachmentPoint3)
		{
			base.transform.parent = characterRig.AttachPoint3;
		}
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		base.transform.localScale = Vector3.one;
	}
}
