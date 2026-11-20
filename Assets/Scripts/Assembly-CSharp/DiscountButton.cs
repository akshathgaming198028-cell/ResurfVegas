using System;
using UnityEngine;

public class DiscountButton : MonoBehaviour
{
	protected const int SALE_BUTTON_GROWTH = 12;

	protected readonly Color32 titleColor = GlobalColorsGenerated.DISCOUNT_BUTTON_TITLE_COLOR;

	protected readonly Color32 titleColorShadow = GlobalColorsGenerated.DISCOUNT_BUTTON_TITLE_COLOR_SHADOW;

	protected readonly Color32 description1ColorInactive = GlobalColorsGenerated.DISCOUNT_BUTTON_DESCRIPTION1_INACTIVE_COLOR;

	protected readonly Color32 description1ColorShadowInactive = GlobalColorsGenerated.DISCOUNT_BUTTON_DESCRIPTION1_INACTIVE_SHADOW_COLOR;

	protected readonly Color32 description1ColorActive = GlobalColors.DISCOUNT_BUTTON_DESCRIPTION1_ACTIVE_COLOR;

	protected readonly Color32 description1ColorShadowActive = GlobalColors.DISCOUNT_BUTTON_DESCRIPTION1_ACTIVE_SHADOW_COLOR;

	protected readonly Color32 description2Color = GlobalColors.DISCOUNT_BUTTON_DESCRIPTION2_COLOR;

	protected readonly Color32 description2ColorShadow = GlobalColors.DISCOUNT_BUTTON_DESCRIPTION2_SHADOW_COLOR;

	protected readonly Color32 priceColor = GlobalColorsGenerated.DISCOUNT_BUTTON_PRICE_COLOR;

	protected readonly Color32 priceColorShadow = GlobalColorsGenerated.DISCOUNT_BUTTON_PRICE_SHADOW_COLOR;

	protected readonly Color32 limitedTimerColor = GlobalColorsGenerated.DISCOUNT_BUTTON_LIMITED_TIME_COLOR;

	protected readonly Color32 limitedTimerColorShadow = GlobalColorsGenerated.DISCOUNT_BUTTON_LIMITED_TIME_SHADOW_COLOR;

	protected Color32 originalTitleColor;

	protected Color32 originalTitleColorShadow;

	protected Color32 originalDescriptionColor;

	protected Color32 originalDescriptionColorShadow;

	protected Vector3 originalDescriptionPosition;

	protected Vector3 originalPricePosition;

	protected Vector3 originalFillGrayScale;

	protected Vector3 originalFillColorScale;

	protected Vector3 originalOutlineScale;

	protected Vector3 originalContentPosition;

	protected Vector3 originalIconBgPosition;

	protected Vector3 originalIconPosition;

	protected Vector2 titleShadowPosition = new Vector2(1f, 1f);

	protected string originalFillColorSpirteName;

	protected string originalFillGraySpriteName;

	[SerializeField]
	protected UISprite iconBG;

	[SerializeField]
	protected UISprite icon;

	[SerializeField]
	protected UILabel title;

	[SerializeField]
	protected UILabel description1;

	[SerializeField]
	protected UILabel description2;

	[SerializeField]
	protected UISprite lineThrough;

	[SerializeField]
	protected UISprite lineThroughShadow;

	[SerializeField]
	protected UILabel price;

	[SerializeField]
	protected UISprite fillGraySprite;

	[SerializeField]
	protected UISprite fillColorSprite;

	[SerializeField]
	protected UISprite outlineSprite;

	[SerializeField]
	protected Transform content;

	[SerializeField]
	protected UISprite discountSticker;

	[SerializeField]
	protected UILabel discountLabel;

	[SerializeField]
	protected UILabel discountLabelSingleLine;

	[SerializeField]
	protected UILabel limitedTitle;

	[SerializeField]
	protected UILabel limitedTime;

	protected static TimeSpan timeLeft;

	public virtual void Awake()
	{
		originalFillColorSpirteName = fillColorSprite.spriteName;
		originalFillGraySpriteName = fillGraySprite.spriteName;
		originalFillGrayScale = new Vector3(fillGraySprite.width, fillGraySprite.height, 1f);
		originalFillColorScale = new Vector3(fillColorSprite.width, fillColorSprite.height, 1f);
		originalOutlineScale = new Vector3(outlineSprite.width, outlineSprite.height, 1f);
		originalContentPosition = content.localPosition;
		originalDescriptionPosition = description1.cachedTransform.localPosition;
		originalPricePosition = price.cachedTransform.localPosition;
		originalTitleColor = title.color;
		originalTitleColorShadow = title.effectColor;
		originalDescriptionColor = description1.color;
		originalDescriptionColorShadow = description1.effectColor;
	}

	protected virtual void ShowOnPromotion(string productString, string sendDescription, string sendDescription2)
	{
		fillGraySprite.width = Mathf.RoundToInt(originalFillGrayScale.x);
		fillGraySprite.height = Mathf.RoundToInt(originalFillGrayScale.y + 12f);
		fillColorSprite.width = Mathf.RoundToInt(originalFillColorScale.x);
		fillColorSprite.height = Mathf.RoundToInt(originalFillColorScale.y + 12f);
		outlineSprite.width = Mathf.RoundToInt(originalOutlineScale.x);
		outlineSprite.height = Mathf.RoundToInt(originalOutlineScale.y + 12f);
		content.localPosition = originalContentPosition + new Vector3(0f, 6f, 0f);
		fillColorSprite.spriteName = "button_fill_shopItem_highlight_sale";
		fillGraySprite.spriteName = "button_fill_shopItem_main_sale";
		limitedTitle.gameObject.SetActive(true);
		limitedTime.gameObject.SetActive(true);
		limitedTitle.enabled = true;
		limitedTime.enabled = true;
		discountSticker.enabled = true;
		discountSticker.gameObject.SetActive(true);
		discountLabelSingleLine.enabled = true;
		discountLabelSingleLine.gameObject.SetActive(true);
		discountLabelSingleLine.text = Strings.Get(StringID.DISCOUNT_BUTTON_COOL_DEAL_SINGLE_LINE);
		price.transform.localPosition = originalPricePosition + new Vector3(0f, 6f, 0f);
		title.text = OnlineSettings.instance.GetValue("discount_deal_name", Strings.Get(StringID.DISCOUNT_BUTTON_COOL_SPACIAL_DEAL));
		description1.text = sendDescription;
		description1.cachedTransform.localPosition = originalDescriptionPosition;
		description2.enabled = true;
		description2.text = sendDescription2;
		title.color = titleColor;
		title.effectColor = titleColorShadow;
		title.effectDistance = titleShadowPosition;
		description1.color = description1ColorInactive;
		description1.effectColor = description1ColorShadowInactive;
		description2.color = description2Color;
		description2.effectColor = description2ColorShadow;
		price.color = priceColor;
		price.effectColor = priceColorShadow;
		limitedTitle.color = limitedTimerColor;
		limitedTitle.effectColor = limitedTimerColorShadow;
		limitedTime.color = limitedTimerColor;
		limitedTime.effectColor = limitedTimerColorShadow;
		discountSticker.color = GlobalColorsGenerated.DISCOUNT_STICKER_COLOR;
		discountLabel.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
		discountLabelSingleLine.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
		if (lineThrough != null && lineThroughShadow != null)
		{
			lineThrough.enabled = true;
			lineThroughShadow.enabled = true;
			lineThrough.color = GlobalColors.DISCOUNT_BUTTON_LINE_THROUGH_COLOR;
			lineThroughShadow.color = GlobalColors.DISCOUNT_BUTTON_LINE_THROUGH_SHADOW_COLOR;
			lineThrough.width = Mathf.RoundToInt(6 + description1.width);
			lineThroughShadow.width = Mathf.RoundToInt(6 + description1.width);
			if (DeviceInfo.Instance.isHighres)
			{
				lineThrough.spriteName = "sale_strikeover_hi";
				lineThroughShadow.spriteName = "sale_strikeover_hi";
			}
			else
			{
				lineThrough.spriteName = "sale_strikeover_lo";
				lineThroughShadow.spriteName = "sale_strikeover_lo";
			}
		}
		else
		{
			Debug.LogError("There is a missing reference to the \"lineThrough\" variable in the DiscountButton.cs", this);
		}
		iconBG.spriteName = "icon_background_theme";
		Common(productString);
	}

	protected virtual void ShowOnSale(string productString, string sendDescription = "")
	{
		fillGraySprite.width = Mathf.RoundToInt(originalFillGrayScale.x);
		fillGraySprite.height = Mathf.RoundToInt(originalFillGrayScale.y + 12f);
		fillColorSprite.width = Mathf.RoundToInt(originalFillColorScale.x);
		fillColorSprite.height = Mathf.RoundToInt(originalFillColorScale.y + 12f);
		outlineSprite.width = Mathf.RoundToInt(originalOutlineScale.x);
		outlineSprite.height = Mathf.RoundToInt(originalOutlineScale.y + 12f);
		content.localPosition = originalContentPosition + new Vector3(0f, 6f, 0f);
		fillColorSprite.spriteName = "button_fill_shopItem_highlight_sale";
		fillGraySprite.spriteName = "button_fill_shopItem_main_sale";
		limitedTitle.gameObject.SetActive(true);
		limitedTime.gameObject.SetActive(true);
		limitedTitle.enabled = true;
		limitedTime.enabled = true;
		discountSticker.enabled = true;
		discountSticker.gameObject.SetActive(true);
		discountLabel.enabled = true;
		discountLabel.gameObject.SetActive(true);
		discountLabel.text = Mathf.Abs(OnlineSettings.instance.GetValue(InAppData.GetInAppOnlineSettingsId(InAppData.inAppData[productString].NormalOfferId), 0)) + Strings.Get(StringID.DISCOUNT_BUTTON_COOL_OFF);
		price.transform.localPosition = originalPricePosition + new Vector3(0f, 6f, 0f);
		title.text = OnlineSettings.instance.GetValue("discount_deal_name", Strings.Get(StringID.DISCOUNT_BUTTON_COOL_SPACIAL_DEAL));
		description1.text = InAppData.inAppData[productString].DisplayName;
		description1.cachedTransform.localPosition = originalDescriptionPosition;
		description2.enabled = true;
		if (string.IsNullOrEmpty(InAppData.inAppData[productString].DisplayText))
		{
			description2.text = sendDescription;
		}
		else
		{
			description2.text = InAppData.inAppData[productString].DisplayText;
		}
		title.color = titleColor;
		title.effectColor = titleColorShadow;
		title.effectDistance = titleShadowPosition;
		description1.color = description1ColorActive;
		description1.effectColor = description1ColorShadowActive;
		description2.color = description2Color;
		description2.effectColor = description2ColorShadow;
		price.color = priceColor;
		price.effectColor = priceColorShadow;
		limitedTitle.color = limitedTimerColor;
		limitedTitle.effectColor = limitedTimerColorShadow;
		limitedTime.color = limitedTimerColor;
		limitedTime.effectColor = limitedTimerColorShadow;
		discountSticker.color = GlobalColorsGenerated.DISCOUNT_STICKER_COLOR;
		discountLabel.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
		if (lineThrough != null && lineThroughShadow != null)
		{
			lineThrough.enabled = false;
			lineThroughShadow.enabled = false;
		}
		iconBG.spriteName = "icon_background_theme";
		Common(productString);
	}

	protected virtual void ShowNoDiscount(string productString, string backupDescription = "")
	{
		fillGraySprite.width = Mathf.RoundToInt(originalFillGrayScale.x);
		fillGraySprite.height = Mathf.RoundToInt(originalFillGrayScale.y);
		fillColorSprite.width = Mathf.RoundToInt(originalFillColorScale.x);
		fillColorSprite.height = Mathf.RoundToInt(originalFillColorScale.y);
		outlineSprite.width = Mathf.RoundToInt(originalOutlineScale.x);
		outlineSprite.height = Mathf.RoundToInt(originalOutlineScale.y);
		content.localPosition = originalContentPosition;
		fillColorSprite.spriteName = originalFillColorSpirteName;
		fillGraySprite.spriteName = originalFillGraySpriteName;
		limitedTitle.gameObject.SetActive(false);
		limitedTime.gameObject.SetActive(false);
		limitedTitle.enabled = false;
		limitedTime.enabled = false;
		discountSticker.enabled = false;
		discountLabel.enabled = false;
		discountSticker.gameObject.SetActive(false);
		discountLabel.gameObject.SetActive(false);
		discountLabelSingleLine.enabled = false;
		discountLabelSingleLine.gameObject.SetActive(false);
		price.transform.localPosition = originalPricePosition;
		title.text = InAppData.inAppData[productString].DisplayName;
		if (string.IsNullOrEmpty(InAppData.inAppData[productString].DisplayText))
		{
			description1.text = backupDescription;
		}
		else
		{
			description1.text = InAppData.inAppData[productString].DisplayText;
		}
		description1.color = Color.white;
		description1.cachedTransform.localPosition = originalDescriptionPosition;
		description2.enabled = false;
		if (lineThrough != null && lineThroughShadow != null)
		{
			lineThrough.enabled = false;
			lineThroughShadow.enabled = false;
		}
		if (InAppData.inAppData[productString].InAppItemType == InAppItemModel.ItemType.Coins || InAppData.inAppData[productString].InAppItemType == InAppItemModel.ItemType.DoubleCoins)
		{
			iconBG.spriteName = "icon_background_normal";
		}
		else
		{
			iconBG.spriteName = "icon_background_keys";
		}
		title.color = originalTitleColor;
		title.effectColor = originalTitleColorShadow;
		description1.color = originalDescriptionColor;
		description1.effectColor = originalDescriptionColorShadow;
		Common(productString);
	}

	private void Common(string productString)
	{
		icon.spriteName = InAppData.inAppData[productString].Icon;
		price.text = InAppData.inAppData[productString].LocalizedPrice;
	}
}
