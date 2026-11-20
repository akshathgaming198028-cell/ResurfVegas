using UnityEngine;

public class UICameraScreenClipping : MonoBehaviour
{
	private const int SCREEN_WIDTH = 300;

	private const int POPUP_WIDTH = 265;

	private Camera _cam;

	private void Awake()
	{
		_cam = base.gameObject.GetComponent<Camera>();
	}

	private void Start()
	{
		if (_cam == null)
		{
			Debug.LogError("The UICameraScreenClipping script is not attached to a Camera");
		}
		else
		{
			CalculateClipping(false);
		}
	}

	public void CalculateClipping(bool popupSizedClip)
	{
		UIRoot root = UIScreenController.Instance.root;
		if (root == null)
		{
			Debug.LogError("UIRoot not set in UIScreenController");
		}
		if (_cam == null)
		{
			_cam = base.gameObject.GetComponent<Camera>();
		}
		Rect rect = _cam.rect;
		if (!UIScreen.IsScreenHeightOutOfProportion())
		{
			float num = Screen.width * 480 / Screen.height;
			float num2 = (float)((!popupSizedClip) ? 300 : 265) / num;
			float x = 0.5f - num2 / 2f;
			rect.x = x;
			rect.width = num2;
		}
		else if (popupSizedClip)
		{
			rect.x = 0.085f;
			rect.width = 0.83f;
		}
		else
		{
			rect.x = 0.05f;
			rect.width = 0.9f;
		}
		_cam.rect = rect;
	}
}
