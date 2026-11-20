using UnityEngine;

public class UISlideInMissionSetHelper : UISlideIn
{
	[SerializeField]
	private UILabel line1;

	[SerializeField]
	private UILabel lineReward;

	[SerializeField]
	private UILabel lineRewardShadow;

	[SerializeField]
	private UISprite superMysteryBox;

	[SerializeField]
	private UISprite mysteryBox;

	[SerializeField]
	private UISprite eggHuntIcon;

	[SerializeField]
	private Transform glow;

	[SerializeField]
	private UISprite background;

	[SerializeField]
	private Transform backgroundDark;

	[SerializeField]
	private UISprite coinIcon;

	[SerializeField]
	private UILabel coinLabel;

	private int _displayedMultiplier;

	private int _queuedMissionSetSlideIns;

	private bool _multiplierIsIncrementing;

	private int _missionsInStoryLineSet;

	private void Awake()
	{
		_missionsInStoryLineSet = MissionsManager.Instance.missionSetStoryCount + 1;
	}

	private int CalculateTheDisplayedMultiplierNumber(int multiplier)
	{
		if (multiplier < _missionsInStoryLineSet)
		{
			_queuedMissionSetSlideIns = UIScreenController.Instance.NumberOfMissionSetSlideIns();
			_multiplierIsIncrementing = true;
		}
		else if (_displayedMultiplier == 0)
		{
			_queuedMissionSetSlideIns = 0;
		}
		else
		{
			_queuedMissionSetSlideIns = _missionsInStoryLineSet - (_displayedMultiplier + 1);
		}
		return multiplier - _queuedMissionSetSlideIns;
	}

	public void SetupSlideInMissionSet(int multiplier)
	{
		base.gameObject.SetActive(true);
		if (_displayedMultiplier == _missionsInStoryLineSet)
		{
			_multiplierIsIncrementing = false;
		}
		if (PlayerInfo.Instance.missionCompletedSum > _missionsInStoryLineSet && !_multiplierIsIncrementing)
		{
			superMysteryBox.enabled = true;
			superMysteryBox.transform.localPosition = new Vector3(-61f, -31f, 0f);
			lineReward.enabled = false;
			lineRewardShadow.enabled = false;
		}
		else
		{
			lineReward.enabled = true;
			lineRewardShadow.enabled = true;
			_displayedMultiplier = CalculateTheDisplayedMultiplierNumber(multiplier);
			lineReward.text = "x" + _displayedMultiplier;
			superMysteryBox.enabled = false;
		}
		line1.text = "Mission Set\ncomplete";
		line1.transform.localPosition = new Vector3(-16f, -30f, -1f);
		EnableCoinLabel(false);
		glow.localPosition = new Vector3(-64f, -30f, 0f);
		background.width = 200;
		backgroundDark.localPosition = new Vector3(-100f, 0f, 0f);
		eggHuntIcon.enabled = false;
		mysteryBox.enabled = false;
		SlideIn();
	}

	public void SetupCoin()
	{
		base.gameObject.SetActive(true);
		superMysteryBox.enabled = false;
		mysteryBox.enabled = false;
		lineReward.enabled = false;
		lineRewardShadow.enabled = false;
		eggHuntIcon.enabled = false;
		EnableCoinLabel(true);
		line1.text = "Daily Challenge \ncomplete";
		line1.transform.localPosition = new Vector3(-32f, -30f, -1f);
		glow.localPosition = new Vector3(-84f, -30f, 0f);
		background.width = 240;
		backgroundDark.localPosition = new Vector3(-120f, 0f, 0f);
		SlideIn();
	}

	public void SetupMysteryBox(MysteryBox.Type mysteryBoxType)
	{
		base.gameObject.SetActive(true);
		lineReward.enabled = false;
		lineRewardShadow.enabled = false;
		eggHuntIcon.enabled = false;
		EnableCoinLabel(false);
		if (mysteryBoxType == MysteryBox.Type.Super)
		{
			superMysteryBox.enabled = true;
			superMysteryBox.transform.localPosition = new Vector3(-71f, -31f, 0f);
			mysteryBox.enabled = false;
		}
		else
		{
			superMysteryBox.enabled = false;
			mysteryBox.enabled = true;
			mysteryBox.transform.localPosition = new Vector3(-71f, -31f, 0f);
		}
		line1.text = "Daily Challenge \ncomplete";
		line1.transform.localPosition = new Vector3(-31f, -30f, -1f);
		glow.localPosition = new Vector3(-74f, -30f, 0f);
		background.width = 220;
		backgroundDark.localPosition = new Vector3(-110f, 0f, 0f);
		SlideIn();
	}

	public void SetupEggHunt()
	{
		base.gameObject.SetActive(true);
		lineReward.enabled = false;
		lineRewardShadow.enabled = false;
		superMysteryBox.enabled = false;
		mysteryBox.enabled = false;
		EnableCoinLabel(false);
		eggHuntIcon.enabled = true;
		eggHuntIcon.transform.localPosition = new Vector3(-71f, -31f, 0f);
		line1.text = "Egg Hunt \ncomplete";
		line1.transform.localPosition = new Vector3(-15f, -30f, -1f);
		glow.localPosition = new Vector3(-74f, -30f, 0f);
		background.width = 190;
		backgroundDark.localPosition = new Vector3(-110f, 0f, 0f);
		SlideIn();
	}

	public void NewCoinAmount(int coinAmount)
	{
		NewCoinAmount(coinAmount.ToString());
	}

	public void NewCoinAmount(string coinAmount)
	{
		base.gameObject.SetActive(true);
		coinLabel.text = coinAmount;
		coinIcon.MakePixelPerfect();
		base.gameObject.SetActive(false);
	}

	public void EnableCoinLabel(bool enable)
	{
		coinIcon.gameObject.SetActive(enable);
		coinLabel.gameObject.SetActive(enable);
	}
}
