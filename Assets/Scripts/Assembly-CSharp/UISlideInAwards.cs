using UnityEngine;
using com.kiloo.awards;

public class UISlideInAwards : UISlideIn
{
	private const string CAN_BRONZE = "icon_bronze_can";

	private const string CAN_SILVER = "icon_silver_can";

	private const string CAN_GOLD = "icon_gold_can";

	[SerializeField]
	private UISprite _icon;

	[SerializeField]
	private GameObject _elements;

	[SerializeField]
	private UISprite _mainBg;

	[SerializeField]
	private UILabel _textLine1;

	[SerializeField]
	private UILabel _textLine2;

	private Vector3 INITIAL_ELEM_POS = new Vector3(0f, 0f, 0f);

	private Vector3 INITIAL_BG_SCALE = new Vector3(230f, 64f, 0f);

	private float INITIAL_LINE1_SIZE = 140f;

	public void SetupSlideInAward(string payload, AwardTierType awardTierType)
	{
		base.gameObject.SetActive(true);
		_textLine1.text = payload;
		float num = (float)Mathf.Max(_textLine1.width, _textLine2.width) - INITIAL_LINE1_SIZE;
		_elements.transform.localPosition = new Vector3(INITIAL_ELEM_POS.x - num / 2f, INITIAL_ELEM_POS.y, INITIAL_ELEM_POS.z);
		_mainBg.width = Mathf.RoundToInt(INITIAL_BG_SCALE.x + num);
		_mainBg.height = Mathf.RoundToInt(INITIAL_BG_SCALE.y);
		_textLine2.text = Strings.Get(StringID.AWARD_SLIDEIN_LINE2);
		switch (awardTierType)
		{
		case AwardTierType.Bronze:
			_icon.spriteName = "icon_bronze_can";
			break;
		case AwardTierType.Silver:
			_icon.spriteName = "icon_silver_can";
			break;
		case AwardTierType.Gold:
			_icon.spriteName = "icon_gold_can";
			break;
		}
		SlideIn();
	}
}
