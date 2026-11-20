using UnityEngine;

public class UIWidgetResizer : MonoBehaviour
{
	private const int _pixelsOutsideTheBackground = 84;

	[SerializeField]
	private UIWidget _anchorBackground;

	private void OnEnable()
	{
		AdjustAnchorBackgroundToResolution();
	}

	public void AdjustAnchorBackgroundToResolution()
	{
		UIRoot uIRoot = null;
		if (UIScreenController.Instance != null)
		{
			uIRoot = UIScreenController.Instance.root;
		}
		if (uIRoot == null)
		{
			uIRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
		}
		Vector2 vector = new Vector2(_anchorBackground.width, _anchorBackground.height);
		vector.y = uIRoot.manualHeight - 84;
		_anchorBackground.width = Mathf.RoundToInt(vector.x);
		_anchorBackground.height = Mathf.RoundToInt(vector.y);
	}
}
