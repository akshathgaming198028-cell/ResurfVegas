using JetBrains.Annotations;
using TopRun;
using UnityEngine;

public class TopRunResultsButton : MonoBehaviour
{
	[SerializeField]
	private UILabel weekLabel;

	[UsedImplicitly]
	private void OnEnable()
	{
		SetWeekLabel();
	}

	private void SetWeekLabel()
	{
		weekLabel.text = "WEEK " + PlayerInfo.Instance.pendingTopRunResultsWeek;
	}

	[UsedImplicitly]
	private void OnClick()
	{
		IManager manager = TopRunRoot.manager;
		if (manager != null)
		{
			if (TopRunRewardHelper.ShouldShowResults())
			{
				SetBeatTopRunFriendsAwardProgression();
				TopRunRewardHelper.PrepareAndCallCelebrationScreen(manager);
			}
			else
			{
				Debug.LogError("Torunament is null or doesn't have a result. Please make sure this button is never shown unless tournament results are available.");
			}
		}
		else
		{
			Debug.LogError("TopRunManager is null. Please make sure this button is never shown unless maanger data is available.");
		}
	}

	private void SetBeatTopRunFriendsAwardProgression()
	{
		for (int i = 0; i < PlayerInfo.Instance.pendingTopRunBeatenFriendsAward; i++)
		{
			Statistics stats;
			Statistics statistics = (stats = PlayerInfo.Instance.stats);
			Stat stat2;
			Stat stat = (stat2 = Stat.TopRunFriendsBeaten);
			int num = stats[stat2];
			statistics[stat] = num + 1;
		}
	}
}
