using UnityEngine;

public class ListTitleComponentHelper : MonoBehaviour
{
	[SerializeField]
	private UILabel label;

	[SerializeField]
	private UIWidget outline;

	private Vector3 _outlinePosition = Vector3.zero;

	public void Setup(string text, Color color, float offsetFromLeft)
	{
		label.text = text;
		label.color = color;
		_outlinePosition = outline.transform.localPosition;
		_outlinePosition.x += offsetFromLeft;
		outline.transform.localPosition = _outlinePosition;
	}
}
