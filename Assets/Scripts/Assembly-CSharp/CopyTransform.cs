using UnityEngine;

public class CopyTransform : MonoBehaviour
{
	[SerializeField]
	private Transform transformToCopy;

	private void LateUpdate()
	{
		base.transform.localPosition = transformToCopy.localPosition;
		base.transform.localRotation = transformToCopy.localRotation;
	}
}
