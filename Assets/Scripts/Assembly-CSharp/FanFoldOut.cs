using System.Collections;
using UnityEngine;

public class FanFoldOut : CharacterCustomizationProp
{
	[SerializeField]
	private float _flipTime = 0.2f;

	[SerializeField]
	private Renderer _foldIn;

	[SerializeField]
	private Renderer _foldOut;

	public override void Initialize(CharacterRig characterRig, bool alternateSetup)
	{
		base.transform.parent = characterRig.RightHand;
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		base.transform.localScale = Vector3.one;
		if (!alternateSetup)
		{
			StartCoroutine(Play());
		}
		else
		{
			_foldOut.enabled = true;
		}
	}

	private IEnumerator Play()
	{
		yield return new WaitForSeconds(_flipTime);
		_foldIn.enabled = false;
		_foldOut.enabled = true;
	}
}
