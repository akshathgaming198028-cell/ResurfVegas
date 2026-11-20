using UnityEngine;

public class AwardsPopup : UIScreen
{
	[SerializeField]
	private UILabel titleLine;

	[SerializeField]
	private UILabel line1Big;

	[SerializeField]
	private UILabel line1Small;

	[SerializeField]
	private UILabel line1Dot;

	[SerializeField]
	private UILabel line2Big;

	[SerializeField]
	private UILabel line2Small;

	[SerializeField]
	private UILabel line2Dot;

	[SerializeField]
	private UILabel line3Big;

	[SerializeField]
	private UILabel line3Small;

	[SerializeField]
	private UILabel line3Dot;

	[SerializeField]
	private UILabel buttonText;

	[SerializeField]
	private GameObject goldCanGO;

	[SerializeField]
	private GameObject silverCanGO;

	[SerializeField]
	private GameObject bronzeCanGO;

	private UIModelController uimodelController;

	private GameObject goldCan;

	private GameObject silverCan;

	private GameObject bronzeCan;

	private static readonly Vector3 BRONZE_POSITION = new Vector3(-126f, 7f, 0f);

	private static readonly Vector3 BRONZE_ROTATION = new Vector3(9f, 180f, 350f);

	private static readonly Vector3 BRONZE_SCALE = new Vector3(60f, 60f, 60f);

	private static readonly Vector3 GOLD_POSITION = new Vector3(-92f, 3.5f, -30f);

	private static readonly Vector3 GOLD_ROTATION = new Vector3(10f, 180f, 0f);

	private static readonly Vector3 GOLD_SCALE = new Vector3(63f, 63f, 63f);

	private static readonly Vector3 SILVER_POSITION = new Vector3(-57f, 6f, 0f);

	private static readonly Vector3 SILVER_ROTATION = new Vector3(9f, 180f, 10f);

	private static readonly Vector3 SILVER_SCALE = new Vector3(60f, 60f, 60f);

	public override void Show()
	{
		base.Show();
		uimodelController = UIModelController.Instance;
		titleLine.text = Strings.Get(StringID.POPUP_AWARDS_TITLE_LINE_1);
		buttonText.text = Strings.Get(StringID.OK);
		line1Big.text = Strings.Get(StringID.POPUP_AWARDS_LINE1BIG);
		line1Small.text = Strings.Get(StringID.POPUP_AWARDS_LINE1SMALL);
		line2Big.text = Strings.Get(StringID.POPUP_AWARDS_LINE2BIG);
		line2Small.text = Strings.Get(StringID.POPUP_AWARDS_LINE2SMALL);
		line3Big.text = Strings.Get(StringID.POPUP_AWARDS_LINE3BIG);
		line3Small.text = Strings.Get(StringID.POPUP_AWARDS_LINE3SMALL);
		PlayerInfo.Instance.hasShownAwardsIntroductionPopup = true;
		InsertCans();
	}

	public override void Hide()
	{
		base.Hide();
		goldCan.SetActive(false);
		silverCan.SetActive(false);
		bronzeCan.SetActive(false);
	}

	private void OkClicked()
	{
		UIScreenController.Instance.ignoreLastPopupClosed = true;
		UIScreenController.Instance.PushScreen("AwardsScreen");
		UIScreenController.Instance.CloseActivePopups(screenName);
	}

	private void InsertCans()
	{
		if (goldCan != null)
		{
			goldCan.SetActive(true);
		}
		else
		{
			goldCan = NGUITools.AddChild(uimodelController.TutorialPopupAnchor, goldCanGO);
		}
		goldCan.layer = UIModelController.Instance.TutorialPopupAnchor.layer;
		Transform transform = goldCan.transform;
		transform.localPosition = GOLD_POSITION;
		transform.localRotation = Quaternion.Euler(GOLD_ROTATION);
		transform.localScale = GOLD_SCALE;
		if (silverCan != null)
		{
			silverCan.SetActive(true);
		}
		else
		{
			silverCan = NGUITools.AddChild(uimodelController.TutorialPopupAnchor, silverCanGO);
		}
		silverCan.layer = UIModelController.Instance.TutorialPopupAnchor.layer;
		Transform transform2 = silverCan.transform;
		transform2.localPosition = SILVER_POSITION;
		transform2.localRotation = Quaternion.Euler(SILVER_ROTATION);
		transform2.localScale = SILVER_SCALE;
		if (bronzeCan != null)
		{
			bronzeCan.SetActive(true);
		}
		else
		{
			bronzeCan = NGUITools.AddChild(uimodelController.TutorialPopupAnchor, bronzeCanGO);
		}
		bronzeCan.layer = UIModelController.Instance.TutorialPopupAnchor.layer;
		Transform transform3 = bronzeCan.transform;
		transform3.localPosition = BRONZE_POSITION;
		transform3.localRotation = Quaternion.Euler(BRONZE_ROTATION);
		transform3.localScale = BRONZE_SCALE;
	}
}
