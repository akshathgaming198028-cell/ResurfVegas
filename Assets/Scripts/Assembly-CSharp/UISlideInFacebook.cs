using System;
using UnityEngine;

public class UISlideInFacebook : UISlideIn
{
	public enum FacebookSlideInState
	{
		ConnectFailed = 0,
		ConnectSucceeded = 1,
		LoggedOut = 2
	}

	[SerializeField]
	private UILabel _onlineLabel;

	[SerializeField]
	private UILabel _offlineLabel;

	[SerializeField]
	private GameObject _onlineElements;

	[SerializeField]
	private GameObject _offlineElements;

	public void SetupSlideFacebook(FacebookSlideInState state, Action onDidShowCallback)
	{
		base.gameObject.SetActive(true);
		switch (state)
		{
		case FacebookSlideInState.ConnectSucceeded:
			_onlineLabel.text = Strings.Get(StringID.FACEBOOK_ONLINE_SLIDEIN);
			_onlineElements.SetActive(true);
			_offlineElements.SetActive(false);
			break;
		case FacebookSlideInState.ConnectFailed:
			_offlineLabel.text = Strings.Get(StringID.FACEBOOK_OFFLINE_SLIDEIN);
			_onlineElements.SetActive(false);
			_offlineElements.SetActive(true);
			break;
		case FacebookSlideInState.LoggedOut:
			_onlineLabel.text = Strings.Get(StringID.FACEBOOK_LOGOUT_SLIDEIN);
			_onlineElements.SetActive(true);
			_offlineElements.SetActive(false);
			break;
		}
		SlideIn(onDidShowCallback);
	}
}
