using UnityEngine;

public class HoverboardPopup : UIScreen
{
	[SerializeField]
	private UIWidget outline;

	[SerializeField]
	private UILabel popupTitle;

	[SerializeField]
	private UILabel hoverboardButtonPriceLabel;

	[SerializeField]
	private UILabel hoverboardDescription1;

	[SerializeField]
	private UILabel hoverboardDescription2;

	private void Awake()
	{
		popupTitle.text = Strings.Get(StringID.HOVERBOARD_POPUP);
		hoverboardButtonPriceLabel.text = Strings.Get(StringID.HOVERBOARD_POPUP_PRICE);
		hoverboardDescription1.text = Strings.Get(StringID.HOVERBOARD_POPUP_DESCRIPTION_1);
		hoverboardDescription2.text = Strings.Get(StringID.HOVERBOARD_POPUP_DESCRIPTION_2);
	}

	public override void Init()
	{
		base.Init();
		InitializeCoinbox(outline, true, true, true);
	}
}
