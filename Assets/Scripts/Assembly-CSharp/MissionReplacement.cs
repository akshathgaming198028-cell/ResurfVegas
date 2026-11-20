public struct MissionReplacement
{
	public int versionNo;

	public int missionSet;

	public int missionNumberInSet;

	public MissionReplacement(int versionNumber, int missionSet, int missionIndexInSet)
	{
		versionNo = versionNumber;
		this.missionSet = missionSet;
		missionNumberInSet = missionIndexInSet;
	}
}
