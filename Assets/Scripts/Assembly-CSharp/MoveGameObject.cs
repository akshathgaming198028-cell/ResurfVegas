using SYBO.Subway.Cameras;
using UnityEngine;

public class MoveGameObject : MonoBehaviour
{
	public Vector3 vector = Vector3.zero;

	public float scaleFactor = 1f;

	private CharacterCamera characterCamera;

	[OptionalField]
	public Transform model;

	public float startScale = 1f;

	private void Update()
	{
		if (characterCamera == null)
		{
			characterCamera = CharacterCamera.Instance;
		}
		if (characterCamera.Speed > 0f)
		{
			base.transform.Translate(vector * characterCamera.Speed * Time.deltaTime);
			float num = startScale + base.transform.localPosition.x * scaleFactor;
			model.localScale = new Vector3(num, num, num);
		}
	}
}
