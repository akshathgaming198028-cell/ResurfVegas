using UnityEngine;

public class UISlideInItemHunt : UISlideIn
{
	[SerializeField]
	private UILabel _textLabel;

	public void SetupSlideInOffer(string sum)
	{
		_textLabel.text = Strings.Get(StringID.WEEKLY_HUNT_SLIDEIN);
		base.gameObject.SetActive(true);
		SlideIn();
	}
}
