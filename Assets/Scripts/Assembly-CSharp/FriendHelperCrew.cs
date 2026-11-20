using System;
using UnityEngine;

public class FriendHelperCrew : MonoBehaviour
{
	private const string PROGRESS_STRING = "{0} / {1} runs";

	private const int RUNS_TO_COLLECT = 50;

	private const int REWARD_MIN = 50;

	private const int REWARD_MAX = 350;

	private const string DUMMY_FRIEND_NAME = "Friend Bonus";

	public GameObject collectButton;

	public GameObject progress;

	public UILabel friendName;

	public UITexture friendPicture;

	public UISprite friendBackground;

	public FriendCrewPokeHelper pokeHelper;

	public Texture2D dummyImage;

	private Friend _friend;

	private CollectCoinsDummyData.CollectCoinsDummy _dummyFriend;

	private Color32 CELL_BACKGROUND_COLOR_CLEAR = new Color32(184, 215, 237, 127);

	private Color32 CELL_BACKGROUND_COLOR_DARK = new Color32(120, 170, 206, 127);

	private bool _isDummyFriend;

	private bool _hasAddedFriendUpdateHandler;

	public Texture2D dummyFriendImage;

	private readonly Vector3 COLLECT_POSITION = new Vector3(75f, -22f, 0f);

	public void InitDummyFriend(CollectCoinsDummyData.CollectCoinsDummy dummyFriend, bool backgroundActive = false)
	{
		_isDummyFriend = true;
		_dummyFriend = dummyFriend;
		if (!backgroundActive)
		{
			friendBackground.color = CELL_BACKGROUND_COLOR_CLEAR;
		}
		else
		{
			friendBackground.color = CELL_BACKGROUND_COLOR_DARK;
		}
		friendName.text = dummyFriend.name;
		friendPicture.mainTexture = dummyFriendImage;
		friendPicture.shader = Shader.Find("Unlit/Transparent Colored (AlphaClip)");
		if (!dummyFriend.hasBeenCollected)
		{
			progress.SetActive(false);
			collectButton.SetActive(true);
			collectButton.GetComponent<UIButtonMessage>().target = base.gameObject;
			collectButton.transform.localPosition = COLLECT_POSITION;
			pokeHelper.DeactivatePoke();
		}
		else
		{
			progress.SetActive(true);
			collectButton.SetActive(false);
			FriendProgressHelper component = progress.GetComponent<FriendProgressHelper>();
			component.label.text = string.Format("{0} / {1} runs", _dummyFriend.fakeProgress, 50);
			component.slider.value = (float)_dummyFriend.fakeProgress / 50f;
			pokeHelper.DeactivatePoke();
		}
	}

	public void InitFriend(Friend friend, bool backgroundActive = false)
	{
		_isDummyFriend = false;
		_friend = friend;
		if (!backgroundActive)
		{
			friendBackground.color = CELL_BACKGROUND_COLOR_CLEAR;
		}
		else
		{
			friendBackground.color = CELL_BACKGROUND_COLOR_DARK;
		}
		friendName.text = _friend.name;
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
		_hasAddedFriendUpdateHandler = true;
		if (_friend.gamesToCashIn >= 50)
		{
			progress.SetActive(false);
			collectButton.SetActive(true);
			collectButton.GetComponent<UIButtonMessage>().target = base.gameObject;
			collectButton.transform.localPosition = COLLECT_POSITION;
			pokeHelper.DeactivatePoke();
			return;
		}
		progress.SetActive(true);
		collectButton.SetActive(false);
		FriendProgressHelper component = progress.GetComponent<FriendProgressHelper>();
		component.label.text = string.Format("{0} / {1} runs", Mathf.Clamp(_friend.gamesToCashIn, 0, 50), 50);
		component.slider.value = (float)_friend.gamesToCashIn / 50f;
		if (friend.shouldBePoked)
		{
			pokeHelper.ActivatePoke(friend);
			return;
		}
		if (_friend.status.lastPokeTime == DateTime.MinValue)
		{
			SocialManager.instance.SetPokeFirstTime(_friend);
		}
		pokeHelper.DeactivatePoke();
	}

	public void CollectReward()
	{
		PlayerInfo.Instance.hasCollectedFromFriends = true;
		if (_isDummyFriend)
		{
			Debug.Log("Collecting reward from dummy friend");
			int num = PayoutAndReturnReward();
			progress.SetActive(true);
			collectButton.SetActive(false);
			FriendProgressHelper component = progress.GetComponent<FriendProgressHelper>();
			component.label.text = string.Format("{0} / {1} runs", 0, 50);
			component.slider.value = 0f;
			UIScreenController.Instance.SpawnCollectText(component.GetCoinPouchGlobalPosition(), num.ToString());
			PlayerInfo.Instance.CollectCoinsDummyData.DummyCollected(_dummyFriend.index);
			PlayerInfo.Instance.dummyFriendCollected = true;
		}
		else if (_friend != null)
		{
			Debug.Log("Collecting reward");
			MissionsManager.Instance.PlayerDidThis(MissionTarget.CollectCoinPouch);
			SocialManager.instance.CollectFriendReward(_friend);
			int num2 = PayoutAndReturnReward();
			progress.SetActive(true);
			collectButton.SetActive(false);
			FriendProgressHelper component2 = progress.GetComponent<FriendProgressHelper>();
			component2.label.text = string.Format("{0} / {1} runs", _friend.gamesToCashIn, 50);
			component2.slider.value = _friend.gamesToCashIn / 50;
			UIScreenController.Instance.SpawnCollectText(component2.GetCoinPouchGlobalPosition(), num2.ToString());
			SocialManager.instance.Save();
			Flurry.LogEvent("Friend reward collect");
		}
		else
		{
			Debug.LogError("FriendHelperCrew.CollectReward called for non-dummy friend, but _friend is not set", this);
		}
	}

	private int PayoutAndReturnReward()
	{
		int num = UnityEngine.Random.Range(50, 350);
		PlayerInfo.Instance.amountOfCoins += num;
		Statistics stats;
		Statistics statistics = (stats = PlayerInfo.Instance.stats);
		Stat stat2;
		Stat stat = (stat2 = Stat.CoinsFriends);
		int num2 = stats[stat2];
		statistics[stat] = num2 + num;
		return num;
	}

	public void Refresh()
	{
		OnFriendUpdatedHandler();
	}

	private void OnFriendUpdatedHandler()
	{
		if (_friend != null)
		{
			if (_friend.image != null)
			{
				friendPicture.mainTexture = _friend.image;
			}
			if (_friend.name != null && !string.IsNullOrEmpty(_friend.name) && friendName != null)
			{
				friendName.text = _friend.name;
			}
		}
		else
		{
			Debug.LogError("FriendHelperCrew.OnFriendUpdatedHandler called but _friend is not set", this);
		}
	}

	private void OnDestroy()
	{
		if (_hasAddedFriendUpdateHandler)
		{
			if (_friend != null)
			{
				_friend.RemoveUpdatedHandler(OnFriendUpdatedHandler);
				_hasAddedFriendUpdateHandler = false;
			}
			else
			{
				Debug.LogError("FriendHelperCrew.OnDestroy called but _friend is not set and we have a handler to remove", this);
			}
		}
	}
}
