using System.Collections;
using SYBO.Subway;
using SYBO.Subway.Characters;
using UnityEngine;

public class PauseScreen : UIScreen
{
	[SerializeField]
	private GameObject[] rewardIcons = new GameObject[0];

	[SerializeField]
	private UILabel coinRewardLabel;

	[SerializeField]
	private UISprite coinRewardSprite;

	[SerializeField]
	private UIWidget[] challengeActiveWidgets = new UIWidget[0];

	[SerializeField]
	private UIWidget[] challengeCompletedWidgets = new UIWidget[0];

	[SerializeField]
	private GameObject dailyWordActiveGameObject;

	[SerializeField]
	private GameObject dailyWordCompletedGameObject;

	[SerializeField]
	private GameObject dailyWordRewardsGameObject;

	[SerializeField]
	private GameObject topRunTimerGameObject;

	[SerializeField]
	private UILabel countryLabel;

	[SerializeField]
	private UISprite countryFlagSprite;

	[SerializeField]
	private UITopRunTimer topRunTimer;

	[SerializeField]
	private ResumeButtonHelper resumeBtnHelper;

	[SerializeField]
	private UIAnchor missionHelperAnchor;

	[SerializeField]
	private UISprite resumeButtonFill;

	[SerializeField]
	private UILabel topTitleLabel;

	[SerializeField]
	private UILabel collectLettersLabel;

	[SerializeField]
	private UILabel missionsLabel;

	[SerializeField]
	private UILabel homeButtonLabel;

	[SerializeField]
	private UILabel settingsButtonLabel;

	[SerializeField]
	private UILabel resumeButtonLabel;

	[SerializeField]
	private UILabel nextChallengeLable;

	[SerializeField]
	private UILabel comeBackLabel;

	[SerializeField]
	private GameObject _dailyBubbleSprite;

	[SerializeField]
	private GameObject _topRunBubbleSprite;

	[SerializeField]
	private GameObject _diamondMedalPrefab;

	[SerializeField]
	private GameObject _goldMedalPrefab;

	[SerializeField]
	private GameObject _silverMedalPrefab;

	[SerializeField]
	private GameObject _bronzeMedalPrefab;

	private GameObject[] topRunMedals = new GameObject[4];

	private GameObject _diamondMedal;

	private GameObject _goldMedal;

	private GameObject _silverMedal;

	private GameObject _bronzeMedal;

	private static readonly Vector3 DIAMOND_POSITION = new Vector3(-50f, 30f, 0f);

	private static readonly Vector3 DIAMOND_ROTATION = new Vector3(0f, -180f, -15f);

	private static readonly Vector3 DIAMOND_SCALE = new Vector3(13.8f, 13.8f, 13.8f);

	private static readonly Vector3 GOLD_POSITION = new Vector3(2f, 32f, 12f);

	private static readonly Vector3 GOLD_ROTATION = new Vector3(0f, 180f, 13f);

	private static readonly Vector3 GOLD_SCALE = new Vector3(11f, 11f, 11f);

	private static readonly Vector3 SILVER_POSITION = new Vector3(44f, 20f, 24f);

	private static readonly Vector3 SILVER_ROTATION = new Vector3(0f, 180f, 22f);

	private static readonly Vector3 SILVER_SCALE = new Vector3(9.5f, 9.5f, 9.5f);

	private static readonly Vector3 BRONZE_POSITION = new Vector3(72f, -5f, 36f);

	private static readonly Vector3 BRONZE_ROTATION = new Vector3(0f, 180f, 35f);

	private static readonly Vector3 BRONZE_SCALE = new Vector3(7.6f, 7.6f, 7.6f);

	[SerializeField]
	private GameObject _topAnchorParent;

	[SerializeField]
	private UIToggle MissionCheckBox1;

	[SerializeField]
	private UIToggle MissionCheckBox2;

	[SerializeField]
	private UIToggle MissionCheckBox3;

	[SerializeField]
	private UILabel MissionLabel1;

	[SerializeField]
	private UILabel MissionLabel2;

	[SerializeField]
	private UILabel MissionLabel3;

	[SerializeField]
	private UILabel MissionNumber1;

	[SerializeField]
	private UILabel MissionNumber2;

	[SerializeField]
	private UILabel MissionNumber3;

	[SerializeField]
	private UISlider missionProgressSlider;

	[SerializeField]
	private UILabel progressLabel;

	[SerializeField]
	private UILabel missionGoalLabel;

	[SerializeField]
	private UISprite MissionGoalSprite;

	private MissionInfo[] _cachedMissionInfo = new MissionInfo[3];

	private int _cachedMissionSet;

	private bool hasCached;

	private MissionInfo[] _currentMissions;

	private int oneOfEach
	{
		get
		{
			int num = 0;
			foreach (PowerupType key in GameStats.Instance.MissionEachPowerupPickupStatus.Keys)
			{
				if (!GameStats.Instance.MissionEachPowerupPickupStatus[key])
				{
					num++;
				}
			}
			return num;
		}
	}

	private void Awake()
	{
		topTitleLabel.text = Strings.Get(StringID.PAUSE_UI_TITLE);
		collectLettersLabel.text = Strings.Get(StringID.PAUSE_UI_COLLECT_LETTERS);
		missionsLabel.text = Strings.Get(StringID.PAUSE_UI_MISSIONS);
		homeButtonLabel.text = Strings.Get(StringID.PAUSE_UI_HOME);
		settingsButtonLabel.text = Strings.Get(StringID.PAUSE_UI_SETTINGS);
		resumeButtonLabel.text = Strings.Get(StringID.PAUSE_UI_RESUME);
		nextChallengeLable.text = Strings.Get(StringID.PAUSE_UI_NEXT_CHALLENGE);
		comeBackLabel.text = Strings.Get(StringID.PAUSE_UI_COME_BACK);
	}

	public override void Show()
	{
		base.Show();
		if (Game.Instance != null)
		{
			Game.Instance.TriggerPause(true);
			int magnitude = (int)(Time.time - Character.Instance.sameLaneTimeStamp);
			Character.Instance.sameLaneTimeStamp = Time.time;
			MissionsManager.Instance.PlayerDidThis(MissionTarget.StayInOneLane, magnitude);
		}
		PlayerInfo.Instance.CheckIfWeShouldRemoveProgressForDailyQuestInRow();
		if (_topAnchorParent.activeSelf)
		{
			if (PlayerInfo.Instance.isDailyWordComplete())
			{
				DisableDailyChallenge();
				RefreshTopRunTimer();
			}
			else
			{
				DisableTopRunTimer();
				RefreshDailyChallenge();
			}
		}
		PlayerInfo.Instance.CheckIfWeShouldRemoveProgressForDailyQuestInRow();
		_UpdateMissions();
		if (resumeBtnHelper != null)
		{
			resumeBtnHelper.EnableButton();
		}
	}

	public override void Hide()
	{
		base.Hide();
		Destroy3DMedals();
	}

	public override void AdjustToResolution()
	{
		base.AdjustToResolution();
		if (UIScreenController.Instance.root == null)
		{
			Debug.LogWarning("Root is not set in UIScreenController");
		}
		float num = UIScreenController.Instance.root.manualHeight;
		if (num > 480f)
		{
			float num2 = num - 480f;
			float num3 = num2 / 70f;
			float y = missionHelperAnchor.relativeOffset.y - 0.01f * num3;
			missionHelperAnchor.relativeOffset.y = y;
		}
	}

	private void RefreshDailyChallenge()
	{
		dailyWordActiveGameObject.SetActive(true);
		dailyWordCompletedGameObject.SetActive(true);
		dailyWordRewardsGameObject.SetActive(true);
		topTitleLabel.text = Strings.Get(StringID.PAUSE_UI_TITLE);
		bool mostRecentIsToday;
		int num = Mathf.Clamp(PlayerInfo.Instance.GetDailyWordDaysInARow(out mostRecentIsToday), 0, 5);
		int num2 = 0;
		num2 = num;
		if (num2 > DailyWord.REWARDS.Length - 1)
		{
			num2 = DailyWord.REWARDS.Length - 1;
		}
		for (int i = 0; i < rewardIcons.Length; i++)
		{
			UISprite[] componentsInChildren = rewardIcons[i].GetComponentsInChildren<UISprite>();
			foreach (UISprite uISprite in componentsInChildren)
			{
				uISprite.enabled = i == num2;
			}
		}
		if (DailyWord.REWARDS[num2].type == DailyWord.DailyWordRewardType.Coins)
		{
			coinRewardLabel.enabled = true;
			coinRewardSprite.enabled = true;
			coinRewardLabel.text = DailyWord.GetRewardForDay(num2).coins.ToString();
		}
		else if (DailyWord.REWARDS[num2].type == DailyWord.DailyWordRewardType.MysteryBox)
		{
			coinRewardLabel.enabled = false;
			coinRewardSprite.enabled = false;
		}
		else
		{
			Debug.LogWarning("DailyChallengeScreen - Unsupported DailyWordRewardType!");
		}
		for (int k = 0; k < challengeActiveWidgets.Length; k++)
		{
			challengeActiveWidgets[k].enabled = !mostRecentIsToday;
		}
		for (int l = 0; l < challengeCompletedWidgets.Length; l++)
		{
			challengeCompletedWidgets[l].enabled = mostRecentIsToday;
		}
		_dailyBubbleSprite.SetActive(true);
	}

	private void DisableDailyChallenge()
	{
		dailyWordActiveGameObject.SetActive(false);
		dailyWordCompletedGameObject.SetActive(false);
		dailyWordRewardsGameObject.SetActive(false);
	}

	private void RefreshTopRunTimer()
	{
		topRunTimerGameObject.SetActive(true);
		topTitleLabel.text = Strings.Get(StringID.TOPRUN_SCREEN_TITLE);
		countryFlagSprite.spriteName = CountryManager.instance.GetCurrentTournamentCountry().flag;
		countryLabel.text = CountryManager.instance.GetCurrentTournamentCountry().name.ToUpper();
		Add3DModels();
		topRunTimer.DoTimer();
	}

	private void DisableTopRunTimer()
	{
		topRunTimerGameObject.SetActive(false);
		Destroy3DMedals();
	}

	private void Add3DModels()
	{
		if (_diamondMedal != null)
		{
			_diamondMedal.SetActive(true);
		}
		else
		{
			_diamondMedal = NGUITools.AddChild(UIModelController.Instance.PauseScreenAnchor, _diamondMedalPrefab);
			topRunMedals[0] = _diamondMedal;
		}
		_diamondMedal.layer = UIModelController.Instance.PauseScreenAnchor.layer;
		StartCoroutine(DelayMedalIconPositioning(_diamondMedal, DIAMOND_POSITION, DIAMOND_ROTATION, DIAMOND_SCALE));
		if (_goldMedal != null)
		{
			_goldMedal.SetActive(true);
		}
		else
		{
			_goldMedal = NGUITools.AddChild(UIModelController.Instance.PauseScreenAnchor, _goldMedalPrefab);
			topRunMedals[1] = _goldMedal;
		}
		_goldMedal.layer = UIModelController.Instance.PauseScreenAnchor.layer;
		StartCoroutine(DelayMedalIconPositioning(_goldMedal, GOLD_POSITION, GOLD_ROTATION, GOLD_SCALE));
		if (_silverMedal != null)
		{
			_silverMedal.SetActive(true);
		}
		else
		{
			_silverMedal = NGUITools.AddChild(UIModelController.Instance.PauseScreenAnchor, _silverMedalPrefab);
			topRunMedals[2] = _silverMedal;
		}
		_silverMedal.layer = UIModelController.Instance.PauseScreenAnchor.layer;
		StartCoroutine(DelayMedalIconPositioning(_silverMedal, SILVER_POSITION, SILVER_ROTATION, SILVER_SCALE));
		if (_bronzeMedal != null)
		{
			_bronzeMedal.SetActive(true);
		}
		else
		{
			_bronzeMedal = NGUITools.AddChild(UIModelController.Instance.PauseScreenAnchor, _bronzeMedalPrefab);
			topRunMedals[3] = _bronzeMedal;
		}
		_bronzeMedal.layer = UIModelController.Instance.PauseScreenAnchor.layer;
		StartCoroutine(DelayMedalIconPositioning(_bronzeMedal, BRONZE_POSITION, BRONZE_ROTATION, BRONZE_SCALE));
	}

	public void Destroy3DMedals()
	{
		GameObject[] array = topRunMedals;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				Object.Destroy(gameObject);
			}
		}
	}

	private IEnumerator DelayMedalIconPositioning(GameObject medalGameObject, Vector3 position, Vector3 rotation, Vector3 scale)
	{
		yield return new WaitForEndOfFrame();
		if (medalGameObject != null)
		{
			Vector3 trackerPos = _topRunBubbleSprite.transform.position;
			Vector3 pixelPos = UIScreenController.Instance.Camera2DNormal.WorldToScreenPoint(trackerPos);
			pixelPos = new Vector3(pixelPos.x, pixelPos.y, 3f);
			ResetTransform(newPosition: UIScreenController.Instance.Camera3D.ScreenToWorldPoint(pixelPos), trans: medalGameObject.transform, positionOffset: position, newRotation: rotation, newScale: scale);
		}
	}

	private void ResetTransform(Transform trans, Vector3 newPosition, Vector3 positionOffset, Vector3 newRotation, Vector3 newScale)
	{
		trans.position = newPosition;
		trans.localPosition += positionOffset;
		trans.localRotation = Quaternion.Euler(newRotation);
		trans.localScale = newScale;
	}

	public void FetchingTopRunDataTimedOut()
	{
		DisableTopRunTimer();
		RefreshDailyChallenge();
	}

	private void _UpdateMissions()
	{
		_currentMissions = MissionsManager.Instance.GetMissionInfo();
		if (hasCached)
		{
			bool flag = true;
			for (int i = 0; i < 3; i++)
			{
				if (_cachedMissionInfo[i].progress != _currentMissions[i].progress || _cachedMissionInfo[i].complete != _currentMissions[i].complete || _currentMissions[i].mission.type == MissionType.OneOfEachPowerup)
				{
					flag = false;
				}
			}
			if (_cachedMissionSet != MissionsManager.Instance.currentMissionSet)
			{
				flag = false;
			}
			if (flag)
			{
				return;
			}
		}
		MissionLabel2.text = string.Format(Strings.Get(_currentMissions[1].template.description), _currentMissions[1].mission.goal, _currentMissions[1].mission.goal - _currentMissions[1].progress);
		MissionLabel3.text = string.Format(Strings.Get(_currentMissions[2].template.description), _currentMissions[2].mission.goal, _currentMissions[2].mission.goal - _currentMissions[2].progress);
		MissionCheckBox1.value = _currentMissions[0].complete;
		MissionCheckBox2.value = _currentMissions[1].complete;
		MissionCheckBox3.value = _currentMissions[2].complete;
		hasCached = true;
		LabelAndNumberUpdate(0, MissionLabel1, MissionNumber1);
		LabelAndNumberUpdate(1, MissionLabel2, MissionNumber2);
		LabelAndNumberUpdate(2, MissionLabel3, MissionNumber3);
		MissionLabel1.alpha = ((!_currentMissions[0].complete) ? 1f : 0.5f);
		MissionLabel2.alpha = ((!_currentMissions[1].complete) ? 1f : 0.5f);
		MissionLabel3.alpha = ((!_currentMissions[2].complete) ? 1f : 0.5f);
		_cachedMissionInfo[0] = _currentMissions[0];
		_cachedMissionInfo[1] = _currentMissions[1];
		_cachedMissionInfo[2] = _currentMissions[2];
		_cachedMissionSet = MissionsManager.Instance.currentMissionSet;
		int num = 0;
		for (int j = 0; j < 3; j++)
		{
			if (_currentMissions[j].complete)
			{
				num++;
			}
		}
		missionProgressSlider.value = (float)num / 3f;
		progressLabel.text = string.Format(Strings.Get(StringID.PAUSE_SCREEN_PROGRESS_DONE), (float)num / 3f);
		if (PlayerInfo.Instance.rawMultiplier == 30)
		{
			missionGoalLabel.text = string.Empty;
			MissionGoalSprite.enabled = true;
		}
		else
		{
			missionGoalLabel.text = "x" + (PlayerInfo.Instance.rawMultiplier + 1);
			MissionGoalSprite.enabled = false;
		}
	}

	private void LabelAndNumberUpdate(int missionArrayNr, UILabel sendMissionLabel, UILabel sendMissionNumber)
	{
		if (_currentMissions[missionArrayNr].complete)
		{
			string format = ((_currentMissions[missionArrayNr].mission.goal != 1) ? Strings.Get(_currentMissions[missionArrayNr].template.ultraShortDescription) : Strings.Get(_currentMissions[missionArrayNr].template.ultraShortDescriptionSingle));
			sendMissionLabel.text = string.Format(format, _currentMissions[missionArrayNr].mission.goal);
			sendMissionNumber.text = string.Empty;
			return;
		}
		string format2 = ((_currentMissions[missionArrayNr].mission.goal != 1) ? Strings.Get(_currentMissions[missionArrayNr].template.description) : Strings.Get(_currentMissions[missionArrayNr].template.descriptionSingle));
		if (_currentMissions[missionArrayNr].mission.type == MissionType.TimeDeath)
		{
			if (UIScreenController.Instance.GetTopScreenName() == "PauseUI")
			{
				sendMissionLabel.text = string.Format(format2, _currentMissions[missionArrayNr].mission.goal, (int)Game.Instance.GetDuration());
				hasCached = false;
			}
			else
			{
				sendMissionLabel.text = string.Format(format2, _currentMissions[missionArrayNr].mission.goal, 0);
			}
		}
		else if (_currentMissions[missionArrayNr].mission.type == MissionType.OneOfEachPowerup)
		{
			sendMissionLabel.text = string.Format(format2, _currentMissions[missionArrayNr].mission.goal, oneOfEach);
		}
		else
		{
			sendMissionLabel.text = string.Format(format2, _currentMissions[missionArrayNr].mission.goal, _currentMissions[missionArrayNr].mission.goal - _currentMissions[missionArrayNr].progress);
		}
		sendMissionNumber.text = missionArrayNr + 1 + string.Empty;
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused && _topAnchorParent.activeSelf)
		{
			if (PlayerInfo.Instance.isDailyWordComplete())
			{
				DisableDailyChallenge();
				RefreshTopRunTimer();
			}
			else
			{
				DisableTopRunTimer();
				RefreshDailyChallenge();
			}
		}
	}

	public bool IsWaiting()
	{
		if (resumeBtnHelper == null)
		{
			Debug.Log("Resume button Helper not set in Pause Screen.");
		}
		return !resumeBtnHelper.isButtonEnabled;
	}
}
