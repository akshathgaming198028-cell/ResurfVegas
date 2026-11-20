using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Kiloo.Plugins.DeviceUtility;
using UnityEngine;

public class MysteryBoxDoubleUpButton : MonoBehaviour
{
	private const string ONLINESETTINGS_MYSTERY_BOX_DOUBLE_REWARD_CHANCE_KEY = "mystery_box_double_reward_chance";

	private const int DEFAULT_MYSTERY_BOX_DOUBLE_REWARD_CHANCE = 100;

	private const string ONLINESETTINGS_MYSTERY_BOX_DOUBLE_REWARD_ALLOW_KEYS_KEY = "mystery_box_double_reward_allow_keys";

	private const bool DEFAULT_MYSTERY_BOX_DOUBLE_REWARD_ALLOW_KEYS = false;

	[SerializeField]
	private GameObject _doubleButton;

	private CelebrationReward _rewardToDouble;

	private bool _wasRewardDoubled;

	private bool _isButtonEnabled;

	private Dictionary<CelebrationRewardType, Action> _doubleRewardCallbacks = new Dictionary<CelebrationRewardType, Action>(0);

	private int chance
	{
		get
		{
			return OnlineSettings.instance.GetValue("mystery_box_double_reward_chance", 100);
		}
	}

	private bool ChanceRoll()
	{
		return chance > UnityEngine.Random.Range(0, 100);
	}

	private bool IsVideoAvailable()
	{
		return VideoAdsManager.instance.IsAnyVideoAvailable();
	}

	private bool IsDoublable(CelebrationReward reward)
	{
		bool result = false;
		switch (reward.rewardType)
		{
		case CelebrationRewardType.keys:
			result = OnlineSettings.instance.GetValue("mystery_box_double_reward_allow_keys", false);
			break;
		case CelebrationRewardType.token:
			result = PlayerInfo.Instance.IsTokenUseful(reward.characterType, reward, true);
			break;
		case CelebrationRewardType.coins:
			result = true;
			break;
		case CelebrationRewardType.powerup:
			switch (reward.powerupType)
			{
			case PowerupType.hoverboard:
				result = true;
				break;
			case PowerupType.headstart2000:
				result = true;
				break;
			case PowerupType.scorebooster:
				result = true;
				break;
			}
			break;
		default:
			result = false;
			break;
		}
		return result;
	}

	private bool IsValidOrigin(CelebrationReward reward)
	{
		switch (reward.CelebrationRewardOrigin)
		{
		case CelebrationRewardOrigin.MysteryBox:
		case CelebrationRewardOrigin.SuperMysteryBox:
		case CelebrationRewardOrigin.WeeklyChallengeMysteryBox:
		case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
		case CelebrationRewardOrigin.CharacterEventSuperMysteryBox:
			return true;
		default:
			return false;
		}
	}

	public bool TryEnable(CelebrationReward rewardToDouble, Dictionary<CelebrationRewardType, Action> doubleRewardCallbacks)
	{
		_wasRewardDoubled = false;
		bool flag = IsValidOrigin(rewardToDouble);
		bool flag2 = IsDoublable(rewardToDouble);
		bool flag3 = IsVideoAvailable();
		bool flag4 = ChanceRoll();
		bool flag5 = (_isButtonEnabled = flag && flag2 && flag3 && flag4);
		_doubleButton.SetActive(flag5);
		if (flag5)
		{
			_rewardToDouble = rewardToDouble;
			_doubleRewardCallbacks = doubleRewardCallbacks;
			StartCoroutine(AnimateAlpha(_doubleButton.GetComponent<UIWidget>(), 0.2f, 0f, 1f));
			Flurry.LogEvent("Double up button shown");
		}
		if (flag && flag2)
		{
			string arg = ((!flag4) ? "No Chance" : "Chance");
			string arg2 = ((!flag3) ? "No Fill" : "Fill");
			string argValue = string.Format("{0} - {1}", arg, arg2);
			Flurry.LogEventWithAParameter("Attempted to show double up button", "Result", argValue);
			if (!flag3)
			{
				VideoAdsManager.VideoState currentVideoState = VideoAdsManager.instance.currentVideoState;
				VideoAdProvider currentProvider = VideoAdsManager.instance.GetCurrentProvider();
				string text = string.Empty;
				if (currentProvider != null)
				{
					text = currentProvider.id;
				}
				string text2 = text + currentVideoState;
				string contryCodeFromLocale = Utils.GetContryCodeFromLocale(DeviceUtility.GetLocale());
				Flurry.LogEventWithSeveralParameters("No fill for double button", "State;Country", text2 + ";" + contryCodeFromLocale);
			}
		}
		if (!flag3)
		{
			VideoAdsManager.VideoState currentVideoState2 = VideoAdsManager.instance.GetCurrentVideoState();
			if (VideoAdsManager.instance.isInitialized)
			{
				VideoAdsManager.instance.ResumeProvider();
				if (currentVideoState2 == VideoAdsManager.VideoState.NoVideos || currentVideoState2 == VideoAdsManager.VideoState.Offline)
				{
					VideoAdsManager.instance.TryCachingVideosAgain();
				}
			}
		}
		return flag5;
	}

	public void DisableButton()
	{
		if (_isButtonEnabled && _rewardToDouble != null && !_wasRewardDoubled)
		{
			FlurryLogDoubleUpResultAs(false);
		}
		_wasRewardDoubled = false;
		_isButtonEnabled = false;
		_doubleButton.SetActive(false);
	}

	public void OnRelease()
	{
		_wasRewardDoubled = true;
		Flurry.LogEvent("Double up button clicked");
		DisableButton();
		if (!VideoAdsManager.instance.PlayVideoIfAvailable(OnDoubleReward))
		{
			UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get(StringID.SLIDEIN_VIDEO_PLAY_ERROR));
		}
		else
		{
			UIScreenController.Instance.ShowVideoAdsOverlay();
		}
	}

	private void OnDoubleReward()
	{
		UIScreenController.Instance.HideVideoAdsOverlay();
		if (_rewardToDouble != null && IsDoublable(_rewardToDouble))
		{
			Action value;
			if (_doubleRewardCallbacks.TryGetValue(_rewardToDouble.rewardType, out value) && value != null)
			{
				value();
			}
			FlurryLogDoubleUpResultAs(true);
		}
		_rewardToDouble = null;
	}

	private void OnEnable()
	{
		DisableButton();
		_rewardToDouble = null;
	}

	private IEnumerator AnimateAlpha(UIWidget widget, float duration, float from, float to)
	{
		widget.alpha = from;
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			widget.alpha = Mathf.Lerp(from, to, factor);
			yield return null;
		}
	}

	public void FlurryLogDoubleUpResultAs(bool doubledUp)
	{
		string arg = ((_rewardToDouble.rewardType != CelebrationRewardType.powerup) ? _rewardToDouble.rewardType.ToString() : _rewardToDouble.powerupType.ToString());
		string arg2 = ((_rewardToDouble.rewardType != CelebrationRewardType.coins) ? _rewardToDouble.amount.ToString() : GroupCoinsForFlurryLogging(_rewardToDouble.amount));
		string arg3 = ((!doubledUp) ? "Not Doubled" : "Doubled");
		string argValue = string.Format("{0} {1} {2}", arg2, arg, arg3);
		Flurry.LogEventWithAParameter("Double up choice", "Result", argValue);
	}

	public string GroupCoinsForFlurryLogging(int originalCoinAmount)
	{
		if (originalCoinAmount < 500)
		{
			int num = originalCoinAmount / 10 * 10;
			int num2 = num + 9;
			return num + "-" + num2;
		}
		return originalCoinAmount.ToString();
	}

	[Conditional("MYSTERY_BOX_DOUBLE_REWARD_LOGS")]
	private void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log("[MysteryBoxDoubleUp] " + msg, context);
	}
}
