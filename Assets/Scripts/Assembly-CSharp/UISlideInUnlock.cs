public class UISlideInUnlock : UISlideIn
{
	public UILabel UnlockName;

	public void SetupSlideInUnlock(string message)
	{
		base.gameObject.SetActive(true);
		UnlockName.text = message;
		SlideIn();
	}
}
