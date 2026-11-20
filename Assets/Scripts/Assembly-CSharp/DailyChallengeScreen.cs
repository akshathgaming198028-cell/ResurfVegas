using UnityEngine;

public class DailyChallengeScreen : UIScreen
{
	private bool _hasInited;

	[SerializeField]
	private DayHelper[] dayHelpers = new DayHelper[0];

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
	private UILabel topTitleLabel;

	[SerializeField]
	private UILabel collectLettersLabel;

	[SerializeField]
	private UILabel nextChallengeLabel;

	[SerializeField]
	private UILabel comeBackLabel;

	[SerializeField]
	private UILabel mysteryBoxRewardLabel;

	[SerializeField]
	private UILabel comeBackForMoreLabel;

	private void Awake()
	{
		topTitleLabel.text = Strings.Get(StringID.DAILY_CHALLENGE_TOP_TITLE);
		collectLettersLabel.text = Strings.Get(StringID.DAILY_CHALLENGE_COLLECT);
		nextChallengeLabel.text = Strings.Get(StringID.DAILY_CHALLENGE_NEXT_CHALLENGE);
		comeBackLabel.text = Strings.Get(StringID.DAILY_CHALLENGE_COME_BACK);
		mysteryBoxRewardLabel.text = Strings.Get(StringID.DAILY_CHALLENGE_SUPER_MYSTERY_BOX);
		comeBackForMoreLabel.text = Strings.Get(StringID.DAILY_CHALLENGE_COME_BACK_FOR_MORE);
	}

	public override void Init()
	{
		base.Init();
		_hasInited = true;
		for (int i = 0; i < dayHelpers.Length; i++)
		{
			dayHelpers[i].Init(i);
		}
	}

	public override void Show()
	{
		base.Show();
		RefreshDailyChallengeScreen();
	}

	private void RefreshDailyChallengeScreen()
	{
		for (int i = 0; i < dayHelpers.Length; i++)
		{
			dayHelpers[i].RefreshDay();
		}
		bool mostRecentIsToday;
		int num = Mathf.Clamp(PlayerInfo.Instance.GetDailyWordDaysInARow(out mostRecentIsToday), 0, 5);
		int num2 = 0;
		num2 = num;
		if (num2 > DailyWord.REWARDS.Length - 1)
		{
			num2 = DailyWord.REWARDS.Length - 1;
		}
		for (int j = 0; j < rewardIcons.Length; j++)
		{
			UISprite[] componentsInChildren = rewardIcons[j].GetComponentsInChildren<UISprite>();
			foreach (UISprite uISprite in componentsInChildren)
			{
				uISprite.enabled = j == num2;
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
		for (int l = 0; l < challengeActiveWidgets.Length; l++)
		{
			challengeActiveWidgets[l].enabled = !mostRecentIsToday;
		}
		for (int m = 0; m < challengeCompletedWidgets.Length; m++)
		{
			challengeCompletedWidgets[m].enabled = mostRecentIsToday;
		}
	}
}
