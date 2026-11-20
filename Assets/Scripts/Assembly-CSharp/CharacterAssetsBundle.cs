using UnityEngine;

public class CharacterAssetsBundle : MonoBehaviour
{
	public Mesh MainMesh;

	public Mesh HeadMesh;

	public int EyeMaterialID = 1;

	public bool AnimatedEyesHaveDoubleMaterial = true;

	public AvatarEyeAnimation _avatarEyeAnimation;

	public AnimationClip Breath;

	public AnimationClip ScrollbarPose;

	public AnimationClip ScrollbarHeadPose;

	public AnimationClip EventScreenPose;

	public AnimationClip NameEventScreenIdle;

	public AnimationClip RunIdle;

	private void OnValidate()
	{
		AssertIsNotNull(MainMesh, "Mainmesh null for " + base.name);
		AssertIsNotNull(Breath, "Breath animation null for " + base.name);
		AssertIsNotNull(ScrollbarPose, "Scrollbar animation null for " + base.name);
		AssertIsNotNull(EventScreenPose, "Event screen animation null for " + base.name);
		AssertIsNotNull(NameEventScreenIdle, "Name event idle animation null for " + base.name);
	}

	private void AssertIsNotNull(Object obj, string errorString)
	{
		if (obj == null)
		{
			Debug.LogError(errorString);
		}
	}
}
