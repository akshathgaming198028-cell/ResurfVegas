using UnityEngine;

public class UnsavedSceneObject : MonoBehaviour
{
	private void Awake()
	{
		base.hideFlags |= HideFlags.DontSave;
	}
}
