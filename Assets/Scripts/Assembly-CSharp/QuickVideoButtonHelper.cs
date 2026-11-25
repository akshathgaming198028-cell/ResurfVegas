using System.Collections;
using UnityEngine;

public class QuickVideoButtonHelper : MonoBehaviour
{
	public enum State
	{
		Keys = 0,
		Minibox = 1
	}

	[SerializeField]
	private UILabel keyLabel1;

	[SerializeField]
	private UILabel keyLabel2;

	[SerializeField]
	private UISprite keySprite;

	[SerializeField]
	private UISprite miniboxSprite;

	private void OnEnable()
	{
		VideoAdsManager.instance.OnTakeoverEnded += OnTakeoverEndedHandler;
	}

	private void OnDisable()
	{
		VideoAdsManager.instance.OnTakeoverEnded -= OnTakeoverEndedHandler;
	}

	private void OnClick()
	{
		Flurry.LogEvent("Quick Video Ads button clicked");
		StartCoroutine(StartVideoWithDelay());
	}

	private IEnumerator StartVideoWithDelay()
	{
		yield return new WaitForEndOfFrame();
		if (!VideoAdsManager.instance.PlayVideoIfAvailable())
		{
			UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get(StringID.EARN_COINS_NATIVE_POPUP_TITLE));
		}
		else
		{
			UIScreenController.Instance.ShowVideoAdsOverlay();
		}
	}

	private void OnTakeoverEndedHandler(VideoAdProvider provider)
	{
		base.gameObject.SetActive(false);
		UIScreenController.Instance.HideVideoAdsOverlay();
	}

	public void SetState(State state)
	{
		switch (state)
		{
		case State.Keys:
			miniboxSprite.gameObject.SetActive(false);
			keyLabel1.gameObject.SetActive(true);
			keyLabel2.gameObject.SetActive(true);
			keySprite.gameObject.SetActive(true);
			break;
		case State.Minibox:
			miniboxSprite.gameObject.SetActive(true);
			keyLabel1.gameObject.SetActive(false);
			keyLabel2.gameObject.SetActive(false);
			keySprite.gameObject.SetActive(false);
			break;
		}
	}
}
