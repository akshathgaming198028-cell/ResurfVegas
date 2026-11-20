using UnityEngine;

public class UIFooterLoader : MonoBehaviour
{
	public GameObject FooterPrefab;

	private UIFooterHandler _footerHandler;

	public int selectedButton;

	private void Awake()
	{
		GameObject gameObject = NGUITools.AddChild(base.gameObject, FooterPrefab);
		_footerHandler = gameObject.GetComponent<UIFooterHandler>();
		switch (selectedButton)
		{
		case 1:
			_footerHandler.button1.GoToSelectedState();
			break;
		case 2:
			_footerHandler.button2.GoToSelectedState();
			break;
		case 3:
			_footerHandler.button3.GoToSelectedState();
			break;
		default:
			Debug.Log("No button was selected in the footer?", this);
			break;
		}
	}
}
