using UnityEngine;
using com.kiloo.awards;

public class AwardFinishedCellHelper : MonoBehaviour
{
	private const string AWARD_COMPLETE_DESCRIPTION_HEX_COLOR = "[549133]";

	private const string AWARD_COMPLETE_VALUE_HEX_COLOR = "[015A01]";

	private const string END_COLOR_SYMBOL = "[-]";

	private const string COMPLETED_ITEM_BACKGROUND_SPRITE_NAME = "icon_award_bg_completed_small";

	private const string COMPLETED_GOAL_ICON_BACKGROUND_SPRITE_NAME = "icon_award_bg_completed";

	private static readonly Color32 COMPLETED_TITLE_TEXT_COLOR = new Color32(1, 90, 1, byte.MaxValue);

	private static readonly Color32 COMPLETED_TITLE_TEXT_SHADOW_COLOR = new Color32(232, 248, 228, byte.MaxValue);

	private static readonly Color32 COMPLETE_SEGMENT_BACKGROUND_COLOR = new Color32(146, 231, 127, 180);

	[SerializeField]
	private UILabel _missionTitle;

	[SerializeField]
	private UILabel _missionTextDescription;

	[SerializeField]
	private GameObject _missionIconsDescriptionParent;

	[SerializeField]
	private UISprite _cellBackground;

	[SerializeField]
	private AwardCollectibleHelper _mainGoalElementsHelper;

	[SerializeField]
	private UISprite _mysteryBoxIcon;

	[SerializeField]
	private UILabel _inMysteryBoxLabel;

	private string[] _mysteryBoxIconNames = new string[2] { "icon_small_mysteryBox", "icon_small_superMysteryBox" };

	private Award _currentAward;

	public void InitCell(Award award)
	{
		if (award != null)
		{
			_currentAward = award;
		}
		SetupCellStructure();
		SetupCellContent();
		SetupCellColors();
	}

	private void SetupCellStructure()
	{
		_missionTextDescription.gameObject.SetActive(_currentAward.trophyData.hasDescription);
		_missionIconsDescriptionParent.SetActive(!_currentAward.trophyData.hasDescription);
	}

	private void SetupCellContent()
	{
		AwardGoal[] goals = _currentAward.goals;
		_missionTitle.text = Strings.Get(_currentAward.trophyData.title);
		_mainGoalElementsHelper.SetupAwardCollectible(FindBackgroundToUse(true), "icon_gold_can", true, true);
		if (_currentAward.trophyData.hasDescription)
		{
			StringID key = ((!_currentAward.trophyData.hasAnotherDescriptionForOneItemGoal || goals[0].goal != 1) ? _currentAward.trophyData.description : _currentAward.trophyData.descriptionForOneItemGoal);
			_missionTextDescription.text = string.Format(Strings.Get(key), "[549133]", "[-]", "[015A01]", goals[0].goal, "[-]", "[549133]", "[-]");
		}
		else
		{
			if (_currentAward.trophyData.id == AwardId.COLLECT_OLD_TROPHY_ITEMS_MB)
			{
				_mysteryBoxIcon.spriteName = _mysteryBoxIconNames[0];
			}
			else if (_currentAward.trophyData.id == AwardId.COLLECT_OLD_TROPHY_ITEMS_SMB)
			{
				_mysteryBoxIcon.spriteName = _mysteryBoxIconNames[1];
			}
			_inMysteryBoxLabel.text = string.Format(Strings.Get(StringID.AWARD_IN_MYSTERYBOX_TEXT), "[549133]", "[-]");
		}
	}

	private void SetupCellColors()
	{
		_missionTextDescription.color = Color.white;
		_cellBackground.color = COMPLETE_SEGMENT_BACKGROUND_COLOR;
		_missionTitle.color = COMPLETED_TITLE_TEXT_COLOR;
		_missionTitle.effectColor = COMPLETED_TITLE_TEXT_SHADOW_COLOR;
	}

	private string FindBackgroundToUse(bool mainGoalIcon)
	{
		return (!mainGoalIcon) ? "icon_award_bg_completed_small" : "icon_award_bg_completed";
	}
}
