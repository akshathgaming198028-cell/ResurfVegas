using System.Collections;
using UnityEngine;

public class FriendHelperBrag : MonoBehaviour
{
	private Color localPlayerColor = new Color(1f / 15f, 0.39607844f, 0.6156863f, 1f);

	public UILabel friendRank;

	public UILabel friendName;

	public UILabel friendScore;

	public UITexture friendPicture;

	public UISprite friendBackground;

	public Texture2D dummyImage;

	private bool _isLocalUser;

	private Friend _friend;

	private bool _initialized;

	public void InitLocalUser(int ranking, bool backgroundActive = false)
	{
		if (!backgroundActive)
		{
			friendBackground.alpha = 0f;
		}
		else
		{
			friendBackground.alpha = 0.2f;
		}
		friendRank.text = ranking.ToString();
		friendRank.color = localPlayerColor;
		friendName.text = SocialManager.instance.localUserName;
		friendName.color = localPlayerColor;
		friendScore.text = PlayerInfo.Instance.highestScore.ToString();
		friendScore.color = localPlayerColor;
		if (SocialManager.instance.localUserImage != null)
		{
			friendPicture.mainTexture = SocialManager.instance.localUserImage;
		}
		else
		{
			friendPicture.mainTexture = dummyImage;
		}
		friendPicture.shader = Shader.Find("Unlit/Transparent Colored (AlphaClip)");
		SocialManager.instance.AddLocalUserUpdatedHandler(OnLocalUserUpdated);
		_initialized = true;
		_isLocalUser = true;
	}

	public void InitFriend(Friend friend, int ranking, bool backgroundActive = false)
	{
		_friend = friend;
		if (!backgroundActive)
		{
			friendBackground.alpha = 0f;
		}
		else
		{
			friendBackground.alpha = 0.2f;
		}
		friendRank.text = ranking.ToString();
		friendName.text = _friend.name;
		friendScore.text = _friend.score.ToString();
		if (_friend.image != null)
		{
			friendPicture.mainTexture = _friend.image;
		}
		else
		{
			friendPicture.mainTexture = dummyImage;
		}
		friendPicture.shader = Shader.Find("Unlit/Transparent Colored (AlphaClip)");
		_friend.AddUpdatedHandler(OnFriendUpdatedHandler);
		_initialized = true;
	}

	private void OnDestroy()
	{
		if (_initialized)
		{
			if (!_isLocalUser)
			{
				_friend.RemoveUpdatedHandler(OnFriendUpdatedHandler);
			}
			else
			{
				SocialManager.instance.RemoveLocalUserUpdatedHandler(OnLocalUserUpdated);
			}
		}
	}

	private void OnFriendUpdatedHandler()
	{
		if (_friend.image != null)
		{
			friendPicture.mainTexture = _friend.image;
			UIPanel panel = friendPicture.panel;
			if (panel != null)
			{
				StartCoroutine(DisableDepthPass());
			}
		}
	}

	private void OnLocalUserUpdated()
	{
		if (SocialManager.instance.localUserImage != null)
		{
			friendPicture.mainTexture = SocialManager.instance.localUserImage;
			UIPanel panel = friendPicture.panel;
			if (panel != null)
			{
				StartCoroutine(DisableDepthPass());
			}
		}
	}

	private IEnumerator DisableDepthPass()
	{
		yield return null;
		UIPanel friendPanel = friendPicture.panel;
		if (!(friendPanel != null))
		{
		}
	}
}
