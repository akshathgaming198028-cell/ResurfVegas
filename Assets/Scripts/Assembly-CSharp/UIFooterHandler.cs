using UnityEngine;

public class UIFooterHandler : MonoBehaviour
{
	public UIButtonOverlayOff button0;

	public UIButtonOverlayOff button1;

	public UIButtonOverlayOff button2;

	public UIButtonOverlayOff button3;

	[OptionalField]
	public GameObject boostDiscount;

	[OptionalField]
	public GameObject coinshopDiscount;

	[SerializeField]
	private UISprite[] discountStickers;

	[SerializeField]
	private UILabel[] discountStickerLabels;

	public void HandleStickers()
	{
		string topScreenName = UIScreenController.Instance.GetTopScreenName();
		if (!(boostDiscount == null) && !(coinshopDiscount == null))
		{
			for (int i = 0; i < discountStickers.Length; i++)
			{
				discountStickers[i].color = GlobalColorsGenerated.DISCOUNT_STICKER_COLOR;
			}
			for (int j = 0; j < discountStickerLabels.Length; j++)
			{
				discountStickerLabels[j].color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
			}
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			bool flag = activeEventManager == null || activeEventManager.GetEventType() != SSEventType.BiggerJackpot;
			bool active = (topScreenName == "CoinsUI_shop" || topScreenName == "EarnCoinsScreen") && flag && InAppManager.IsDoubleCoinsDiscountActive;
			bool active2 = (topScreenName == "UpgradesUI_shop" || topScreenName == "EarnCoinsScreen") && InAppManager.IsAnyCurrencyDiscountActive;
			boostDiscount.SetActive(active);
			coinshopDiscount.SetActive(active2);
		}
	}
}
