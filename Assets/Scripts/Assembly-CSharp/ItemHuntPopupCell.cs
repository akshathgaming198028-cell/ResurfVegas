using System;
using System.Collections;
using UnityEngine;

public class ItemHuntPopupCell : MonoBehaviour
{
	public UISprite cellBackground;

	public GameObject activeLabels;

	public UILabel activePeriodTitle;

	public UILabel activeMainTitle;

	public GameObject inactiveLabels;

	public UILabel inactivePeriodTitle;

	public UILabel inactiveMainTitle;

	public GameObject iconsGroup;

	public UISprite spriteCircleBg;

	public UISprite spriteCirclePrizeCompleted;

	public UISprite spriteCirclePrizeFailed;

	public UISprite spriteCirclePrizeUnknown;

	public GameObject activeProgressSlider;

	public UISprite activeProgressBg;

	public UISprite activeProgressFg;

	public UILabel activeProgressText;

	public UISprite activeProgressIcon;

	public GameObject inactiveProgressGroup;

	public UILabel inactiveProgressText;

	public UISprite inactiveProgressIcon;

	public GameObject stickerGroup;

	public UISprite stickerSprite;

	public UILabel stickerTextTop;

	public UILabel stickerTextBot;

	[NonSerialized]
	public GameObject prizeIcon;

	[NonSerialized]
	public GameObject presentIcon;

	private bool stickerTopInitialized;

	private Vector3 _prizeRotation = Vector3.zero;

	private Vector3 _prizeScale = Vector3.zero;

	private Vector3 _prizePositionOffset = Vector3.zero;

	private bool _hidePrize;

	private TimeSpan expireTime = TimeSpan.Zero;

	private int _totalSeconds = -1;

	public GameObject iconPrefab { get; set; }

	public void PlaceModel(Vector3 offset, Vector3 scale, Vector3 rotation, bool hidePrize = false)
	{
		_prizePositionOffset = offset;
		_prizeScale = scale;
		_prizeRotation = rotation;
		_hidePrize = hidePrize;
		StartCoroutine("DelayPositioning");
	}

	private IEnumerator DelayPositioning()
	{
		yield return new WaitForEndOfFrame();
		Vector3 trackerPos = spriteCircleBg.transform.position;
		Vector3 pixelPos = UIScreenController.Instance.Camera2DNormal.WorldToScreenPoint(trackerPos);
		pixelPos = new Vector3(pixelPos.x, pixelPos.y, 3f);
		Vector3 worldPos = Vector3.zero;
		worldPos = (_hidePrize ? UIScreenController.Instance.Camera3DClip.ScreenToWorldPoint(pixelPos) : UIScreenController.Instance.Camera3D.ScreenToWorldPoint(pixelPos));
		if (_hidePrize)
		{
			ResetTransform(presentIcon.transform, worldPos, _prizePositionOffset, _prizeRotation, _prizeScale);
		}
		else
		{
			ResetTransform(prizeIcon.transform, worldPos, _prizePositionOffset, _prizeRotation, _prizeScale);
		}
	}

	private void ResetTransform(Transform trans, Vector3 newPosition, Vector3 positionOffset, Vector3 newRotation, Vector3 newScale)
	{
		trans.position = newPosition;
		trans.localPosition += positionOffset;
		trans.localRotation = Quaternion.Euler(newRotation);
		trans.localScale = newScale;
	}

	public void HidePrizeIcon()
	{
		if (prizeIcon != null)
		{
			prizeIcon.SetActive(false);
		}
		if (presentIcon != null)
		{
			presentIcon.SetActive(false);
		}
	}

	public void RefreshProgressBar(int currentItemAmount, int totalItemAmount)
	{
		UISlider component = activeProgressSlider.GetComponent<UISlider>();
		if (totalItemAmount != 0)
		{
			component.value = (float)currentItemAmount / (float)totalItemAmount;
		}
		else
		{
			component.value = 1f;
		}
		activeProgressText.text = string.Format("{0} / {1}", currentItemAmount, totalItemAmount);
	}

	public void SetTheStickerContent(TimeSpan timeLeft)
	{
		if (!stickerTopInitialized)
		{
			stickerTopInitialized = true;
		}
		expireTime = timeLeft;
		if (timeLeft.Days < 1)
		{
			UpdateStickerCounter();
			return;
		}
		stickerTextTop.text = ((timeLeft.Days != 1) ? string.Format(Strings.Get(StringID.ITEM_HUNT_N_DAYS), timeLeft.Days) : Strings.Get(StringID.ITEM_HUNT_ONE_DAY));
		stickerTextBot.text = Strings.Get(StringID.ITEM_HUNT_LEFT);
	}

	private void UpdateStickerCounter()
	{
		if (stickerTextTop.gameObject.activeInHierarchy && stickerTextTop.enabled)
		{
			if (expireTime < TimeSpan.Zero)
			{
				expireTime = TimeSpan.Zero;
			}
			if (_totalSeconds != (int)expireTime.TotalSeconds)
			{
				stickerTextTop.text = string.Format("{0:00}:{1:00}:{2:00}", expireTime.Hours, expireTime.Minutes, expireTime.Seconds);
				stickerTextBot.text = Strings.Get(StringID.ITEM_HUNT_LEFT);
				_totalSeconds = (int)expireTime.TotalSeconds;
			}
		}
	}
}
