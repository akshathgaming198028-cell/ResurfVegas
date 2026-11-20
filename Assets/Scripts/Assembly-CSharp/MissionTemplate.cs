using UnityEngine;

public struct MissionTemplate
{
	public StringID description;

	public StringID descriptionSingle;

	public StringID ultraShortDescription;

	public StringID ultraShortDescriptionSingle;

	public MissionTarget missionTarget;

	public bool singleRun;

	public bool completeIfLess;

	public bool completeIfEqual;

	public bool resetDuringRun;
}
