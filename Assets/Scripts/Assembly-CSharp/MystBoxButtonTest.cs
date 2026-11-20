public class MystBoxButtonTest : UIBasicButton
{
	public bool doubleBox;

	protected override void Send()
	{
		RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.MysteryBox);
		if (doubleBox)
		{
			RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.MysteryBox);
		}
	}
}
