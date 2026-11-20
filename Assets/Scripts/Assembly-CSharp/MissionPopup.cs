using System.Collections;
using SYBO.Subway;
using UnityEngine;

public class MissionPopup : UIScreen
{
	[SerializeField]
	private UILabel missionsLabel;

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
	private UILabel missionGoalLabelLine2;

	[SerializeField]
	private UISprite MissionGoalSprite;

	[SerializeField]
	private UILabel MissionSetLabel;

	[SerializeField]
	private UILabel RewardExplanationLine1;

	[SerializeField]
	private UILabel RewardExplanationLine2;

	[SerializeField]
	private UILabel popupMainTitleLabel;

	[SerializeField]
	private UILabel missionsDescriptionLabel;

	[SerializeField]
	private UILabel rewardLabel;

	private MissionInfo[] _cachedMissionInfo = new MissionInfo[3];

	private int _cachedMissionSet;

	private bool hasCached;

	private MissionInfo[] _currentMissions;

	private int oneOfEach
	{
		get
		{
			int num = 5;
			if (Game.Instance.IsInGame.Value)
			{
				if (GameStats.Instance.doubleMultiplierPickups != 0)
				{
					num--;
				}
				if (GameStats.Instance.coinMagnetsPickups != 0)
				{
					num--;
				}
				if (GameStats.Instance.superSneakerPickups != 0)
				{
					num--;
				}
				if (GameStats.Instance.jetpackPickups != 0)
				{
					num--;
				}
				if (GameStats.Instance.powerJumperPickups != 0)
				{
					num--;
				}
			}
			return num;
		}
	}

	public override void Show()
	{
		base.Show();
		PlayerInfo.Instance.CheckIfWeShouldRemoveProgressForDailyQuestInRow();
		RefreshContents();
	}

	private void Awake()
	{
		popupMainTitleLabel.text = Strings.Get(StringID.MISSION_POPUP_MAIN_TITLE);
		missionsDescriptionLabel.text = Strings.Get(StringID.MISSION_POPUP_DESCRIPTION);
		rewardLabel.text = Strings.Get(StringID.MISSION_POPUP_REWARD);
	}

	private void RefreshContents()
	{
		_currentMissions = MissionsManager.Instance.GetMissionInfo();
		if (hasCached)
		{
			bool flag = true;
			for (int i = 0; i < 3; i++)
			{
				if (_cachedMissionInfo[i].progress != _currentMissions[i].progress || _cachedMissionInfo[i].complete != _currentMissions[i].complete)
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
		UIPanel component = GetComponent<UIPanel>();
		if (component != null)
		{
			component.widgetsAreStatic = false;
			StartCoroutine(_changeMissionCheckboxes(1));
			UIPanelStaticDelayer component2 = GetComponent<UIPanelStaticDelayer>();
			if (component2 != null)
			{
				component2.SetStaticDelayed(3, component);
			}
		}
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
		progressLabel.text = string.Format(Strings.Get(StringID.MISSION_POPUP_PROGRESS_DONE), (float)num / 3f);
		MissionSetLabel.text = Strings.Get(StringID.MISSION_POPUP_MISSION_SET_NUMBER) + " " + PlayerInfo.Instance.missionCompletedSum;
		if (PlayerInfo.Instance.rawMultiplier == 30)
		{
			RewardExplanationLine1.text = Strings.Get(StringID.MISSION_POPUP_SUPER);
			RewardExplanationLine2.text = Strings.Get(StringID.MISSION_POPUP_MYSTERY_BOX);
		}
		else
		{
			RewardExplanationLine1.text = Strings.Get(StringID.MISSION_POPUP_SCORE);
			RewardExplanationLine2.text = Strings.Get(StringID.MISSION_POPUP_MULTIPLIER);
		}
		if (PlayerInfo.Instance.rawMultiplier == 30)
		{
			missionGoalLabel.text = string.Empty;
			missionGoalLabelLine2.text = string.Empty;
			MissionGoalSprite.enabled = true;
		}
		else
		{
			missionGoalLabel.text = "x" + (PlayerInfo.Instance.rawMultiplier + 1);
			missionGoalLabelLine2.text = Strings.Get(StringID.MISSION_POPUP_SCORE);
			MissionGoalSprite.enabled = false;
		}
	}

	private IEnumerator _changeMissionCheckboxes(int delayFrames)
	{
		int num = 0;
		while (num < delayFrames)
		{
			num++;
			yield return null;
		}
		MissionCheckBox1.value = _currentMissions[0].complete;
		MissionCheckBox2.value = _currentMissions[1].complete;
		MissionCheckBox3.value = _currentMissions[2].complete;
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
}
