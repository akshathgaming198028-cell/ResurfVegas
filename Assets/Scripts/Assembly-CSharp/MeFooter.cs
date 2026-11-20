using UnityEngine;

public class MeFooter : MonoBehaviour
{
	[SerializeField]
	private UILabel menuLabel;

	[SerializeField]
	private UILabel charactersLabel;

	[SerializeField]
	private UILabel boarsLabel;

	[SerializeField]
	private UILabel awardsLabel;

	[SerializeField]
	private UILabel boardsGiftTopLabel;

	[SerializeField]
	private UILabel boardsGiftBottomLabel;

	private void Awake()
	{
		menuLabel.text = Strings.Get(StringID.ME_FOOTER_MENU);
		charactersLabel.text = Strings.Get(StringID.ME_FOOTER_CHARACTERS);
		boarsLabel.text = Strings.Get(StringID.ME_FOOTER_BOARDS);
		awardsLabel.text = Strings.Get(StringID.ME_FOOTER_AWARDS);
		boardsGiftTopLabel.text = Strings.Get(StringID.ME_FOOTER_BOARDS_GIFT_TOP);
		boardsGiftBottomLabel.text = Strings.Get(StringID.ME_FOOTER_BOARDS_GIFT_BOTTOM);
	}
}
