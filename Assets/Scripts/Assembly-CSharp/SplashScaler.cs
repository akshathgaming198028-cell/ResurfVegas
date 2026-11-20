using UnityEngine;

[ExecuteInEditMode]
public class SplashScaler : MonoBehaviour
{
	private const float Y_SCALE = 1.775f;

	private bool passed;

	[SerializeField]
	private UITexture _textureToScale;

	[SerializeField]
	private bool repositionNow;

	private void Update()
	{
		if (repositionNow)
		{
			passed = false;
			repositionNow = false;
		}
		if (!passed)
		{
			float f = (float)_textureToScale.width * 1.775f;
			base.transform.localPosition = new Vector3(0f, 0f, 0f);
			_textureToScale.height = Mathf.RoundToInt(f);
			passed = true;
		}
	}
}
