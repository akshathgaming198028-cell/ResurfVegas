using SYBO.Subway.Characters;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
	private GameStats stats;

	private Character character;

	public void Start()
	{
		stats = GameStats.Instance;
		character = GetComponent<Character>();
		character.OnChangeTrack += OnChangeTrack;
		character.OnJump += OnJump;
		character.OnRoll += OnRoll;
		character.OnPassedObstacle += OnPassedObstacle;
		character.OnJumpOverTrain += OnJumpOverTrain;
		character.OnCriticalHit += OnCriticalHit;
		character.OnStumble += OnStumble;
	}

	private void OnJump()
	{
		stats.jumps++;
	}

	private void OnRoll()
	{
		stats.rolls++;
		if (character.TrackIndex == 0)
		{
			stats.rollsLeftTrack++;
		}
		if (character.TrackIndex == 1)
		{
			stats.rollsCenterTrack++;
		}
		if (character.TrackIndex == 2)
		{
			stats.rollsRightTrack++;
		}
	}

	private void OnChangeTrack(Character.OnChangeTrackDirection direction)
	{
		stats.trackChanges++;
	}

	private void OnPassedObstacle(Character.ObstacleType type)
	{
		switch (type)
		{
		case Character.ObstacleType.JumpBarrier:
			stats.jumpBarrier++;
			break;
		case Character.ObstacleType.JumpHighBarrier:
			stats.jumpBarrier++;
			stats.jumpHighBarrier++;
			break;
		case Character.ObstacleType.JumpTrain:
			stats.jumpsOverTrains++;
			break;
		case Character.ObstacleType.RollBarrier:
			stats.dodgeBarrier++;
			break;
		}
	}

	private void OnJumpOverTrain()
	{
		stats.jumpsOverTrains++;
	}

	private void OnCriticalHit(Character.CriticalHitType type)
	{
		switch (type)
		{
		case Character.CriticalHitType.Train:
			stats.trainHit++;
			break;
		case Character.CriticalHitType.Barrier:
			stats.barrierHit++;
			break;
		case Character.CriticalHitType.MovingTrain:
			stats.movingTrainHit++;
			break;
		}
	}

	private void OnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName)
	{
		if (colliderName.Contains("lightSignal"))
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.BumpLightSignal);
		}
		else if (colliderName.Contains("bush") || colliderName.Contains("powerbox"))
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.BumpBush);
		}
		else if (colliderName.Contains("ramp_side"))
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.BumpTrain);
		}
		else if (colliderName.Contains("blocker_jump"))
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.BumpBarrier);
		}
		else if (colliderName.Contains("blocker_roll"))
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.BumpBarrier);
		}
		else if (colliderName.Contains("blocker_standard"))
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.BumpBarrier);
		}
		else
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.BumpTrain);
		}
	}
}
