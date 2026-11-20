public class UISlideInMissionHelper : UISlideIn
{
	public UILabel line1;

	public void SetupSlideInMission(string message)
	{
		base.gameObject.SetActive(true);
		line1.text = message;
		SlideIn();
	}
}
