using TopRun;
using UnityEngine;

public class FriendHelperHighScore : MonoBehaviour
{
	private const int SCORE_SPACING = 10;

	private Color _localPlayerColor = new Color(1f, 1f, 1f, 1f);

	private Color _friendColor = new Color(0.043137256f, 0.16862746f, 0.3254902f, 1f);

	public UILabel friendRank;

	public UILabel friendName;

	public UILabel friendScore;

	public UITexture friendPicture;

	public UISprite friendBackground;

	[SerializeField]
	private UISprite medal;

	[SerializeField]
	private UISprite shadow;

	[SerializeField]
	private UISprite flag;

	private Color32 CELL_BACKGROUND_COLOR_LOCAL_USER = new Color32(11, 43, 83, 204);

	private Color32 CELL_BACKGROUND_COLOR_CLEAR = new Color32(184, 215, 237, 127);

	private Color32 CELL_BACKGROUND_COLOR_DARK = new Color32(120, 170, 206, 127);

	public Texture2D dummyImage;

	private bool _isLocalUser;

	private bool _isDummy;

	private Friend _friend;

	private bool _initialized;

	public void InitLocalUser(int ranking, bool backgroundActive = false, bool isPLayerRanked = true)
	{
		_isLocalUser = true;
		friendBackground.color = CELL_BACKGROUND_COLOR_LOCAL_USER;
		friendRank.text = ranking.ToString();
		friendRank.color = _localPlayerColor;
		friendName.text = SocialManager.instance.localUserName;
		friendName.color = _localPlayerColor;
		friendScore.text = PlayerInfo.Instance.highestScore.ToString();
		friendScore.color = _localPlayerColor;
		if (!isPLayerRanked)
		{
			friendRank.text = "?";
		}
		if (SocialManager.instance.localUserImage != null)
		{
			friendPicture.mainTexture = SocialManager.instance.localUserImage;
		}
		else
		{
			friendPicture.mainTexture = dummyImage;
		}
		friendPicture.shader = Shader.Find("Unlit/Transparent Colored (AlphaClip)");
		InitializeMedal();
		ResizeNameIfNeeded();
		SocialManager.instance.AddLocalUserUpdatedHandler(OnLocalUserUpdated);
		_initialized = true;
	}

	public void InitFriend(Friend friend, int ranking, bool backgroundActive = false)
	{
		_friend = friend;
		if (!backgroundActive)
		{
			friendBackground.color = CELL_BACKGROUND_COLOR_CLEAR;
		}
		else
		{
			friendBackground.color = CELL_BACKGROUND_COLOR_DARK;
		}
		friendRank.text = ranking.ToString();
		friendName.text = _friend.name;
		friendName.color = _friendColor;
		friendScore.text = friend.score.ToString();
		friendScore.color = _friendColor;
		if (_friend.image != null)
		{
			friendPicture.mainTexture = friend.image;
		}
		else
		{
			friendPicture.mainTexture = dummyImage;
		}
		friendPicture.shader = Shader.Find("Unlit/Transparent Colored (AlphaClip)");
		InitializeMedal();
		ResizeNameIfNeeded();
		_friend.AddUpdatedHandler(OnFriendUpdatedHandler);
		_initialized = true;
	}

	public void InitDummy(LeaderboardDummyData.LeaderboardDummy dummy, int ranking, bool backgroundActive)
	{
		_isDummy = true;
		if (!backgroundActive)
		{
			friendBackground.color = CELL_BACKGROUND_COLOR_CLEAR;
		}
		else
		{
			friendBackground.color = CELL_BACKGROUND_COLOR_DARK;
		}
		friendRank.text = ranking.ToString();
		friendName.text = dummy.name;
		friendName.color = _friendColor;
		friendScore.text = dummy.score.ToString();
		friendScore.color = _friendColor;
		friendPicture.mainTexture = dummyImage;
		friendPicture.shader = Shader.Find("Unlit/Transparent Colored (AlphaClip)");
		InitializeMedal();
		ResizeNameIfNeeded();
		_initialized = true;
	}

	private void ResizeNameIfNeeded()
	{
		float num = friendScore.transform.localPosition.x - (float)friendScore.width;
		float num2 = friendName.transform.localPosition.x + (float)friendName.width;
		float num3 = num - 10f;
		float num4 = num2;
		if (medal.enabled)
		{
			num4 += (float)medal.width;
		}
		if (flag.enabled)
		{
			num4 += (float)flag.width;
		}
		if (num3 <= num4)
		{
			float f = num4 - num3;
			friendName.overflowMethod = UILabel.Overflow.ClampContent;
			friendName.width -= Mathf.RoundToInt(f);
			friendName.text = friendName.processedText;
			friendName.overflowMethod = UILabel.Overflow.ResizeFreely;
		}
	}

	private void InitializeMedal()
	{
		IManager manager = TopRunRoot.manager;
		if (manager != null)
		{
			ITournament tournament = manager.tournament;
			if (tournament != null)
			{
				IRank rank = null;
				if (_isLocalUser)
				{
					rank = TopRunRoot.manager.rank;
					if (tournament.isFinished && tournament.hasResult && !TopRunRewardHelper.ShouldShowResults())
					{
						SetTemporaryMedal(tournament.GetRankForScore(manager.score));
					}
					else if (TopRunRewardHelper.ShouldShowResults())
					{
						HideMedal();
					}
					else if (rank != null && rank.rank < 4 && rank.rank != -1)
					{
						SetMedalForRank(rank, manager);
					}
					else
					{
						HideMedal();
					}
				}
				else if (_friend != null)
				{
					if (_friend.fbProfile != null)
					{
						rank = TopRunRoot.manager.GetPlayerRank(_friend.fbProfile.id);
						if (rank != null && rank.rank < 4 && rank.rank != -1)
						{
							SetMedalForRank(rank, manager);
						}
						else
						{
							HideMedal();
						}
					}
					else
					{
						HideMedal();
					}
				}
				else
				{
					HideMedal();
				}
			}
			else
			{
				HideMedal();
			}
		}
		else
		{
			HideMedal();
		}
	}

	private void HideMedal()
	{
		flag.enabled = false;
		shadow.enabled = false;
		medal.enabled = false;
	}

	private void SetMedalForRank(IRank rankData, IManager manager)
	{
		int rank = rankData.rank;
		Country countryForID = CountryData.GetCountryForID(rankData.country);
		string text = ((countryForID != null) ? countryForID.flag : null);
		if (!string.IsNullOrEmpty(text) && rank >= 0 && rank <= 3)
		{
			flag.spriteName = text;
			if (string.Equals(manager.tournament.country.ToLower(), rankData.country.ToLower()))
			{
				flag.enabled = false;
				shadow.enabled = false;
			}
			else
			{
				flag.enabled = true;
				shadow.enabled = true;
			}
		}
		switch (rank)
		{
		case 0:
			medal.spriteName = "icon_diamond_medal";
			medal.enabled = true;
			break;
		case 1:
			medal.spriteName = "icon_gold_medal";
			medal.enabled = true;
			break;
		case 2:
			medal.spriteName = "icon_silver_medal";
			medal.enabled = true;
			break;
		case 3:
			medal.spriteName = "icon_bronze_medal";
			medal.enabled = true;
			break;
		default:
			Debug.Log("No need for a medal.");
			medal.enabled = false;
			flag.enabled = false;
			shadow.enabled = false;
			break;
		}
	}

	private void SetTemporaryMedal(int rank)
	{
		medal.enabled = true;
		flag.enabled = false;
		shadow.enabled = false;
		switch (rank)
		{
		case 0:
			medal.spriteName = "icon_diamond_medal";
			break;
		case 1:
			medal.spriteName = "icon_gold_medal";
			break;
		case 2:
			medal.spriteName = "icon_silver_medal";
			break;
		case 3:
			medal.spriteName = "icon_bronze_medal";
			break;
		default:
			medal.enabled = false;
			break;
		}
	}

	private void OnDestroy()
	{
		if (_initialized)
		{
			if (_isLocalUser)
			{
				SocialManager.instance.RemoveLocalUserUpdatedHandler(OnLocalUserUpdated);
			}
			else if (!_isDummy)
			{
				_friend.RemoveUpdatedHandler(OnFriendUpdatedHandler);
			}
		}
	}

	public void Refresh()
	{
		OnFriendUpdatedHandler();
	}

	private void OnFriendUpdatedHandler()
	{
		if (_friend.image != null)
		{
			friendPicture.mainTexture = _friend.image;
		}
	}

	private void OnLocalUserUpdated()
	{
		if (SocialManager.instance.localUserImage != null)
		{
			friendPicture.mainTexture = SocialManager.instance.localUserImage;
		}
	}
}
