using UnityEngine;

public class NotebookScreen : UIScreen
{
	private const int _pixelsOutsideTheBackground = 84;

	private const int _pixelsOutsideTheDecal = 130;

	[SerializeField]
	private UITexture blurredBackground;

	[SerializeField]
	private UISprite background;

	[SerializeField]
	private UISprite decal;

	[SerializeField]
	private UISprite spiral;

	[SerializeField]
	private UIAnchor _topDecalAnchor;

	[SerializeField]
	private UIAnchor _centerBackrenderAnchor;

	private bool isOnLoadScreen;

	public override void Show()
	{
		base.Show();
		ShowBackground();
	}

	public override void Hide()
	{
		base.Hide();
		HideBackground();
	}

	public override void AdjustToResolution()
	{
		UIRoot uIRoot = null;
		if (UIScreenController.Instance != null)
		{
			uIRoot = UIScreenController.Instance.root;
		}
		else
		{
			isOnLoadScreen = true;
		}
		if (uIRoot == null)
		{
			uIRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
		}
		if (!isOnLoadScreen && UIScreen.IsScreenHeightOutOfProportion())
		{
			if (background != null && decal != null)
			{
				Vector2 vector = new Vector2(background.width, background.height);
				vector.y = uIRoot.manualHeight - 84;
				background.width = Mathf.RoundToInt(vector.x);
				background.height = Mathf.RoundToInt(vector.y);
				Vector2 vector2 = new Vector2(decal.width, decal.height);
				vector2.y = uIRoot.manualHeight - 130;
				decal.width = Mathf.RoundToInt(vector2.x);
				decal.height = Mathf.RoundToInt(vector2.y);
			}
			else
			{
				Debug.Log("Set background and decal in NotebookScreen. They are null!");
			}
		}
		if (isOnLoadScreen)
		{
			Debug.Log("IS on LoadScene");
			ScaleForLoadScene();
		}
	}

	private void ScaleForLoadScene()
	{
		Debug.Log("Scale for load scene------>>>");
		_topDecalAnchor.transform.localPosition = new Vector3(_topDecalAnchor.transform.localPosition.x, _topDecalAnchor.transform.localPosition.y, 240f);
		_centerBackrenderAnchor.transform.localPosition = new Vector3(_centerBackrenderAnchor.transform.localPosition.x, _centerBackrenderAnchor.transform.localPosition.y, 240f);
	}

	public void ShowBackground()
	{
		blurredBackground.enabled = true;
	}

	public void HideBackground()
	{
		blurredBackground.enabled = false;
	}
}
