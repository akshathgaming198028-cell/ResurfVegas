using UnityEngine;

public class AwardTierHelper : MonoBehaviour
{
	[SerializeField]
	private GameObject inactiveElementsParent;

	[SerializeField]
	private GameObject activeElement;

	public void TierComplete(bool complete)
	{
		inactiveElementsParent.SetActive(!complete);
		activeElement.SetActive(complete);
	}
}
