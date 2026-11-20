using UnityEngine;

public class UISlideInBrag : UISlideIn
{
	[SerializeField]
	private UILabel firstLine;

	[SerializeField]
	private UILabel secondLine;

	public void SetupSlideInBrag()
	{
		base.gameObject.SetActive(true);
		firstLine.text = Strings.Get(StringID.SLIDE_IN_BRAG_FIRST_LINE);
		secondLine.text = Strings.Get(StringID.SLIDE_IN_BRAG_SECOND_LINE);
		SlideIn();
	}
}
