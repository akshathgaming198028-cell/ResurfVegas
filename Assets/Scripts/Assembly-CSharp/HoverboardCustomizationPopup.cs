using System.Collections;
using UnityEngine;

public class HoverboardCustomizationPopup : UIScreen
{
	[SerializeField]
	private UILabel title1;

	[SerializeField]
	private UILabel title2;

	[SerializeField]
	private UISprite container1;

	[SerializeField]
	private UISprite container1Arrow;

	[SerializeField]
	private UISprite container1Glow;

	[SerializeField]
	private UISprite container2;

	[SerializeField]
	private UISprite container2Footer;

	[SerializeField]
	private UILabel board1Power;

	[SerializeField]
	private UISprite container2Arrow;

	[SerializeField]
	private UISprite container2Glow;

	[SerializeField]
	private UISprite container3;

	[SerializeField]
	private UISprite container3Footer;

	[SerializeField]
	private UILabel board2Power;

	[SerializeField]
	private UISprite container3Arrow;

	[SerializeField]
	private UISprite container3Glow;

	[SerializeField]
	private UISprite container4;

	[SerializeField]
	private UISprite container4Glow;

	[SerializeField]
	private GameObject boardModelPrefab;

	[SerializeField]
	private GameObject theme1ModelPrefab;

	[SerializeField]
	private GameObject theme2ModelPrefab;

	[SerializeField]
	private GameObject bothThemesModelPrefab;

	[SerializeField]
	private UISprite[] lightRays;

	private GameObject _boardModel;

	private GameObject _theme1Model;

	private GameObject _theme2Model;

	private GameObject _bothThemesModel;

	private static readonly Vector3 BOARD_MODEL_POSITION_OFFSET = new Vector3(0f, 0f, 0f);

	private static readonly Vector3 BOARD_MODEL_ROTATION = new Vector3(35f, -120f, 49f);

	private static readonly Vector3 BOARD_MODEL_SCALE = new Vector3(18f, 18f, 18f);

	private static readonly Vector3 THEME1_POSITION_OFFSET = new Vector3(0f, 4f, 0f);

	private static readonly Vector3 THEME1_ROTATION = new Vector3(30f, -133f, 43f);

	private static readonly Vector3 THEME1_SCALE = new Vector3(20f, 20f, 20f);

	private static readonly Vector3 THEME2_POSITION_OFFSET = new Vector3(0f, 4f, 0f);

	private static readonly Vector3 THEME2_ROTATION = new Vector3(31f, -119f, 45.5f);

	private static readonly Vector3 THEME2_SCALE = new Vector3(20f, 20f, 20f);

	private static readonly Vector3 BOTH_THEMES_MODEL_POSITION_OFFSET = new Vector3(0f, 11f, 0f);

	private static readonly Vector3 BOTH_THEMES_MODEL_ROTATION = new Vector3(22f, -131f, 30f);

	private static readonly Vector3 BOTH_THEMES_MODEL_SCALE = new Vector3(29f, 29f, 29f);

	public override void Show()
	{
		title1.text = Strings.Get(StringID.HOVERBOARD_CUSTOMIZATION_POPUP_TITLE1);
		title2.text = Strings.Get(StringID.HOVERBOARD_CUSTOMIZATION_POPUP_TITLE2);
		container1.color = GlobalColors.HOVERBOARD_CUSTOMIZATION_POPUP_BOARD_THEME_FOOTER_COLOR;
		container1Arrow.color = GlobalColors.HOVERBOARD_CUSTOMIZATION_POPUP_BOARD_THEME_FOOTER_COLOR;
		container2.color = GlobalColors.HOVERBOARD_CUSTOMIZATION_POPUP_BOARD_THEME_CONTAINER_COLOR;
		container2Footer.color = GlobalColors.HOVERBOARD_CUSTOMIZATION_POPUP_BOARD_THEME_FOOTER_COLOR;
		board1Power.text = Strings.Get(StringID.HOVERBOARD_THEME_HERO_TELEPORT_TITLE);
		container2Arrow.color = GlobalColors.HOVERBOARD_CUSTOMIZATION_POPUP_BOARD_THEME_FOOTER_COLOR;
		container3.color = GlobalColors.HOVERBOARD_CUSTOMIZATION_POPUP_BOARD_THEME_CONTAINER_COLOR;
		container3Footer.color = GlobalColors.HOVERBOARD_CUSTOMIZATION_POPUP_BOARD_THEME_FOOTER_COLOR;
		board2Power.text = Strings.Get(StringID.HOVERBOARD_THEME_HERO_SUPER_JUMP_TITLE);
		container3Arrow.color = GlobalColors.HOVERBOARD_CUSTOMIZATION_POPUP_BOARD_THEME_FOOTER_COLOR;
		container4.color = GlobalColors.HOVERBOARD_CUSTOMIZATION_POPUP_BOARD_THEME_CONTAINER_COLOR;
		UISprite[] array = lightRays;
		foreach (UISprite uISprite in array)
		{
			uISprite.color = GlobalColors.HOVERBOARD_CUSTOMIZATION_POPUP_BOARD_THEME_CONTAINER_COLOR_LIGHT;
		}
		Add3DModels();
	}

	private void Add3DModels()
	{
		if (_boardModel != null)
		{
			_boardModel.SetActive(true);
		}
		else
		{
			_boardModel = NGUITools.AddChild(UIModelController.Instance.TutorialPopupAnchor, boardModelPrefab);
		}
		_boardModel.layer = UIModelController.Instance.TutorialPopupAnchor.layer;
		PlaceModel(container1Glow.gameObject, _boardModel, BOARD_MODEL_POSITION_OFFSET, BOARD_MODEL_SCALE, BOARD_MODEL_ROTATION);
		if (_theme1Model != null)
		{
			_theme1Model.SetActive(true);
		}
		else
		{
			_theme1Model = NGUITools.AddChild(UIModelController.Instance.TutorialPopupAnchor, theme1ModelPrefab);
		}
		SetLayerRecursively(_theme1Model.gameObject, UIModelController.Instance.TutorialPopupAnchor.layer);
		PlaceModel(container2Glow.gameObject, _theme1Model, THEME1_POSITION_OFFSET, THEME1_SCALE, THEME1_ROTATION);
		if (_theme2Model != null)
		{
			_theme2Model.SetActive(true);
		}
		else
		{
			_theme2Model = NGUITools.AddChild(UIModelController.Instance.TutorialPopupAnchor, theme2ModelPrefab);
		}
		SetLayerRecursively(_theme2Model.gameObject, UIModelController.Instance.TutorialPopupAnchor.layer);
		PlaceModel(container3Glow.gameObject, _theme2Model, THEME2_POSITION_OFFSET, THEME2_SCALE, THEME2_ROTATION);
		if (_bothThemesModel != null)
		{
			_bothThemesModel.SetActive(true);
		}
		else
		{
			_bothThemesModel = NGUITools.AddChild(UIModelController.Instance.TutorialPopupAnchor, bothThemesModelPrefab);
		}
		SetLayerRecursively(_bothThemesModel.gameObject, UIModelController.Instance.TutorialPopupAnchor.layer);
		PlaceModel(container4Glow.gameObject, _bothThemesModel, BOTH_THEMES_MODEL_POSITION_OFFSET, BOTH_THEMES_MODEL_SCALE, BOTH_THEMES_MODEL_ROTATION);
	}

	public void PlaceModel(GameObject modelHost, GameObject modelToPosition, Vector3 offset, Vector3 scale, Vector3 rotation)
	{
		StartCoroutine(DelayPositioning(modelHost, modelToPosition, offset, scale, rotation));
	}

	private IEnumerator DelayPositioning(GameObject modelHost, GameObject modelToPosition, Vector3 offset, Vector3 scale, Vector3 rotation)
	{
		yield return new WaitForEndOfFrame();
		Vector3 trackerPos = modelHost.transform.position;
		Vector3 pixelPos = UIScreenController.Instance.Camera2DNormal.WorldToScreenPoint(trackerPos);
		pixelPos = new Vector3(pixelPos.x, pixelPos.y, 3f);
		ResetTransform(newPosition: UIScreenController.Instance.Camera3D.ScreenToWorldPoint(pixelPos), trans: modelToPosition.transform, positionOffset: offset, newRotation: rotation, newScale: scale);
	}

	private void ResetTransform(Transform trans, Vector3 newPosition, Vector3 positionOffset, Vector3 newRotation, Vector3 newScale)
	{
		trans.position = newPosition;
		trans.localPosition += positionOffset;
		trans.localRotation = Quaternion.Euler(newRotation);
		trans.localScale = newScale;
	}

	public override void Hide()
	{
		base.Hide();
		_boardModel.SetActive(false);
		_theme1Model.SetActive(false);
		_theme2Model.SetActive(false);
		_bothThemesModel.SetActive(false);
	}

	public void OKClicked()
	{
		PlayerInfo.Instance.hasShownHoverboardCustmoizationPopup = true;
		BoardScreenManager.Instance.forceCenterOnSpecificBoard = true;
		BoardScreenManager.Instance.comingFromHoverboardCustomizationPopup = true;
		UIScreenController.Instance.ignoreLastPopupClosed = true;
		UIScreenController.Instance.CloseActivePopups(screenName);
		UIScreenController.Instance.PushScreen("BoardScreen");
	}

	private void SetLayerRecursively(GameObject obj, int newLayer)
	{
		if (null == obj)
		{
			return;
		}
		obj.layer = newLayer;
		foreach (Transform item in obj.transform)
		{
			if (!(null == item))
			{
				SetLayerRecursively(item.gameObject, newLayer);
			}
		}
	}
}
