public class HiddenSecretButton : UIScreen
{
	private const int STANDARD_NUMBER_OF_CLICKS_TO_OPEN = 3;

	private const int DEBUG_NUMBER_OF_CLICKS_TO_OPEN = 1;

	private int tabs;

	private void OnClick()
	{
		tabs++;
		if (tabs == (Globals.DEBUG ? 1 : 3))
		{
			UIScreenController.Instance.PushPopup("SecretPopup");
			tabs = 0;
		}
	}
}
