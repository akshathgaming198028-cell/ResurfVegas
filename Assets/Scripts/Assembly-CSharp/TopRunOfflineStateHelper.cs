using TopRun;
using UnityEngine;

public class TopRunOfflineStateHelper : MonoBehaviour
{
	public enum OfflineState
	{
		TryAgain = 0,
		Loading = 1
	}

	[SerializeField]
	private UILabel _offlineText;

	[SerializeField]
	private UISprite _offlineOverlay;

	[SerializeField]
	private UILabel _connectionLabel;

	[SerializeField]
	private UILabel _tryAgainText;

	[SerializeField]
	private UISprite _tryAgainBackground;

	private TopRunOnlineScreen _topRunOnlineScreen;

	private TopRunOnlineGameOverScreen _topRunOnlineGameOverScreen;

	private int tryAgainOfflineLabelOffset = 20;

	private int loadingOfflineLabelOffset = 70;

	public void SetState(OfflineState newState, TopRunOnlineScreen topRunScreen = null, TopRunOnlineGameOverScreen topRunGOScreen = null)
	{
		_topRunOnlineScreen = topRunScreen;
		_topRunOnlineGameOverScreen = topRunGOScreen;
		switch (newState)
		{
		case OfflineState.TryAgain:
			SetTryAgainState();
			break;
		case OfflineState.Loading:
			SetLoadingState();
			break;
		default:
			Debug.LogError(string.Concat("No state found for ", newState, "."));
			break;
		}
	}

	private void SetTryAgainState()
	{
		_offlineText.GetComponent<UIAnchor>().pixelOffset = new Vector2(tryAgainOfflineLabelOffset, 0f);
		_offlineOverlay.gameObject.SetActive(true);
		_tryAgainBackground.gameObject.SetActive(true);
		_tryAgainText.gameObject.SetActive(true);
		_connectionLabel.text = Strings.Get(StringID.TOPRUN_OFFLINE_BUTTON_TEXT);
		_connectionLabel.pivot = UIWidget.Pivot.Left;
		UIAnchor component = _connectionLabel.GetComponent<UIAnchor>();
		component.side = UIAnchor.Side.Left;
		component.pixelOffset = new Vector2(20f, 0f);
		base.gameObject.GetComponent<UIButtonOverlayOff>().enabled = true;
	}

	private void SetLoadingState()
	{
		_offlineText.GetComponent<UIAnchor>().pixelOffset = new Vector2(loadingOfflineLabelOffset, 0f);
		_offlineOverlay.gameObject.SetActive(false);
		_tryAgainBackground.gameObject.SetActive(false);
		_tryAgainText.gameObject.SetActive(false);
		_connectionLabel.text = Strings.Get(StringID.TOPRUN_OFFLINE_BUTTON_TEXT_LOADING);
		_connectionLabel.pivot = UIWidget.Pivot.Center;
		UIAnchor component = _connectionLabel.GetComponent<UIAnchor>();
		component.side = UIAnchor.Side.Center;
		component.pixelOffset = new Vector2(-12f, 0f);
		base.gameObject.GetComponent<UIButtonOverlayOff>().enabled = false;
	}

	private void OnClick()
	{
		EnableLoadingOverlay();
		SetState(OfflineState.Loading);
		IManager manager = TopRunRoot.manager;
		if (manager != null)
		{
			manager.RefreshTournament();
		}
	}

	private void EnableLoadingOverlay()
	{
		if (_topRunOnlineScreen != null)
		{
			_topRunOnlineScreen.loadingOverlay.SetActive(true);
		}
		else if (_topRunOnlineGameOverScreen != null)
		{
			_topRunOnlineGameOverScreen.loadingOverlay.SetActive(true);
		}
	}
}
