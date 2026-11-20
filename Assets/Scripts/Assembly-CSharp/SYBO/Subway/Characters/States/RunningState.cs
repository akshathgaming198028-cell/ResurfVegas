using System.Collections;
using System.Collections.Generic;
using SYBO.Subway.Cameras;
using UnityEngine;

namespace SYBO.Subway.Characters.States
{
	public class RunningState : CharacterState
	{
		public enum RunPositions
		{
			ground = 0,
			station = 1,
			train = 2,
			movingTrain = 3,
			air = 4
		}

		public float slowDownDuration = 0.625f;

		public float slowDownRatio = 1f;

		public float tunnelDelta = -20f;

		public Vector3 cameraOffset = new Vector3(0f, 33f, -33f);

		public float cameraOffsetSmoothDuration = 0.5f;

		public float cameraAimOffset = 20f;

		public float cameraFOV = 60f;

		public float smoothCameraXDuration = 0.05f;

		public float ySmoothDuration = 0.1f;

		public float characterChangeTrackLength = 30f;

		public float ySmoothDurationPogostick = 0.1f;

		public float transitionTimeMax = 1f;

		public float speedupTransitionTime = 5f;

		public float speedupTransitionTimeLow = 10f;

		public AnimationCurve transitionFromJetpackY;

		public AnimationCurve transitionFromJetpackZ;

		public float transitionTimeJetpack = 1f;

		public bool transitionFromHeight;

		public bool transitionFromPogostick;

		private float tunnelStartZ;

		private Curve offsetDeltaCurve;

		private Game game;

		private Character character;

		private CharacterRendering characterRendering;

		private Transform characterTransform;

		private CharacterController characterController;

		private CharacterCamera characterCamera;

		private Transform characterCameraTransform;

		private Animation characterAnimation;

		public RunPositions currentRunPosition;

		private Queue<Collider> GrindedTrains = new Queue<Collider>();

		private int GrindedTrainsBufferSize = 5;

		private static RunningState instance;

		public static RunningState Instance
		{
			get
			{
				return instance ?? (instance = Object.FindObjectOfType(typeof(RunningState)) as RunningState);
			}
		}

		public void Awake()
		{
			game = Game.Instance;
			character = Character.Instance;
			characterRendering = CharacterRendering.Instance;
			characterTransform = character.transform;
			characterController = character.characterController;
			characterCamera = CharacterCamera.Instance;
			characterCameraTransform = characterCamera.transform;
			character.OnStumble += delegate
			{
				character.characterCamera.Shake();
			};
			character.OnLanding += UpdateGroundTag;
		}

		public override IEnumerator Begin()
		{
			transitionFromHeight = false;
			bool transitionWithRoll = false;
			float transitionTimeLength = 0f;
			if (characterTransform.position.y > 70f && characterTransform.position.y < 125f)
			{
				transitionTimeLength = transitionTimeJetpack;
				transitionFromHeight = true;
			}
			if (transitionFromPogostick)
			{
				transitionTimeLength = transitionTimeMax;
				transitionFromHeight = true;
				if (characterTransform.position.y < 155f)
				{
					transitionWithRoll = true;
				}
			}
			character.characterTrigger.enabled = true;
			character.characterCamera.enabled = true;
			SmoothDampVector3 currentCameraOffset = new SmoothDampVector3(cameraOffset, cameraOffsetSmoothDuration)
			{
				Target = cameraOffset
			};
			SmoothDampFloat smoothCameraX = new SmoothDampFloat(characterCameraTransform.position.x, smoothCameraXDuration);
			SmoothDampFloat currentCameraAimOffset = new SmoothDampFloat(cameraAimOffset, cameraOffsetSmoothDuration);
			offsetDeltaCurve = new Curve();
			character.lastGroundedY = character.transform.position.y;
			AnimationEvent setAnimationSpeedEvent = new AnimationEvent
			{
				functionName = "SetAnimationSpeedEvent",
				time = 0.1f,
				messageOptions = SendMessageOptions.RequireReceiver
			};
			Globals.TryAddAnimationEvent(characterRendering.CharacterAnimation, "run", setAnimationSpeedEvent);
			Globals.TryAddAnimationEvent(characterRendering.CharacterAnimation, "run2", setAnimationSpeedEvent);
			Globals.TryAddAnimationEvent(characterRendering.CharacterAnimation, "run3", setAnimationSpeedEvent);
			Globals.TryAddAnimationEvent(characterRendering.CharacterAnimation, "run4_long", setAnimationSpeedEvent);
			ParticleSystem[] componentsInChildren = characterRendering.CharacterModel.SprayCanModel.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem ps in componentsInChildren)
			{
				ParticleSystem.EmissionModule em = ps.emission;
				em.enabled = false;
			}
			float transitionTime = 0f;
			Vector3 cameraPositionStart = characterCamera.Position;
			Vector3 cameraPositionStartOffset = character.transform.position - cameraPositionStart;
			Vector3 cameraTargetStartOffset = characterCamera.Position - characterCamera.Target;
			characterController.Move(Vector3.down * 2f);
			SmoothDampFloat y = null;
			y = ((!transitionFromHeight) ? new SmoothDampFloat(characterTransform.position.y, ySmoothDuration) : new SmoothDampFloat(characterTransform.position.y, ySmoothDurationPogostick));
			while (true)
			{
				game.LayTrackChunks();
				game.currentSpeed = game.currentLevelSpeed;
				game.HandleControls();
				character.ApplyGravity();
				character.MoveForward();
				if (character.IsGrounded.Value && transitionFromHeight && !transitionWithRoll)
				{
					if (game.pogoJumpsLeft > 0)
					{
						game.pogoJumpsLeft--;
						game.StartPogostick();
					}
					else
					{
						transitionTimeLength -= Time.deltaTime * speedupTransitionTime;
					}
					transitionTimeLength -= Time.deltaTime * speedupTransitionTime;
				}
				else if (transitionWithRoll)
				{
					transitionTimeLength -= Time.deltaTime * speedupTransitionTimeLow;
				}
				Vector3 position = character.transform.position;
				if (character.IsJumpingHigher && !transitionFromHeight && !character.IsDoubleJumping)
				{
					y.Target = 0.5f * (character.lastGroundedY + position.y);
				}
				if (character.IsDoubleJumping)
				{
					if (!character.IsJumpingHigher)
					{
						y.Target = 0.5f * (character.lastGroundedY + position.y);
					}
					else
					{
						y.Target = 0.8f * position.y;
					}
				}
				if (character.IsJumpingHigher && transitionFromHeight)
				{
					y.Target = position.y;
				}
				else if (characterController.isGrounded)
				{
					y.Target = character.lastGroundedY;
				}
				else if (position.y < character.lastGroundedY)
				{
					y.Target = position.y;
				}
				y.Update();
				currentCameraOffset.Update();
				currentCameraAimOffset.Update();
				smoothCameraX.Update();
				Vector3 offsetDelta = offsetDeltaCurve.Evaluate(character.z - tunnelStartZ);
				smoothCameraX.Target = position.x * 0.75f;
				Vector3 cameraPositionEnd = new Vector3(smoothCameraX.Value, y.Value, position.z) + currentCameraOffset.Value + offsetDelta;
				Vector3 cameraTargetEnd = new Vector3(smoothCameraX.Value, y.Value, position.z) + Vector3.up * currentCameraAimOffset.Value + offsetDelta * 0.5f;
				Vector3 offset = cameraTargetEnd - cameraPositionEnd;
				if (character.IsInsideSubway && !transitionFromHeight)
				{
					float cameraMaxYinSubway = character.subwayMaxY;
					cameraPositionEnd.y = Mathf.Min(cameraMaxYinSubway, cameraPositionEnd.y);
					cameraTargetEnd = cameraPositionEnd + offset;
					Debug.DrawLine(cameraPositionEnd, cameraTargetEnd, Color.blue, 10f);
				}
				if (transitionFromHeight)
				{
					float transitionRatio = Mathf.Clamp01(transitionTime / transitionTimeLength);
					if (!transitionWithRoll)
					{
						if (game.pogoJumpsLeft == 0)
						{
							transitionFromPogostick = false;
						}
						characterCamera.Position = new Vector3(cameraPositionEnd.x, Mathf.SmoothStep(character.transform.position.y - cameraPositionStartOffset.y, cameraPositionEnd.y, transitionRatio), cameraPositionEnd.z);
						characterCamera.Target = new Vector3(cameraTargetEnd.x, Mathf.Lerp(characterCamera.Position.y - cameraTargetStartOffset.y, cameraTargetEnd.y, transitionRatio), cameraTargetEnd.z);
					}
					else
					{
						if (characterCamera.Position.y - character.transform.position.y >= cameraOffset.y)
						{
							characterCamera.Position = new Vector3(cameraPositionEnd.x, character.transform.position.y + cameraOffset.y, cameraPositionEnd.z);
						}
						else
						{
							characterCamera.Position = new Vector3(cameraPositionEnd.x, cameraPositionStart.y, cameraPositionEnd.z);
						}
						if (characterCamera.Target.y - character.transform.position.y >= cameraAimOffset)
						{
							characterCamera.Target = new Vector3(cameraTargetEnd.x, character.transform.position.y + cameraAimOffset, character.transform.position.z);
						}
						else
						{
							characterCamera.Target = new Vector3(cameraTargetEnd.x, cameraPositionStart.y - cameraTargetStartOffset.y, character.transform.position.z);
						}
					}
					if (character.IsGrounded.Value || (character.inAirJump && characterController.isGrounded))
					{
						transitionFromHeight = false;
						transitionFromPogostick = false;
						transitionWithRoll = false;
						y = new SmoothDampFloat(characterTransform.position.y, ySmoothDuration);
						characterCamera.Position = cameraPositionEnd;
						characterCamera.Target = cameraTargetEnd;
						character.ForceLeaveSubway();
					}
					transitionTime += Time.deltaTime;
				}
				else
				{
					characterCamera.Position = cameraPositionEnd;
					characterCamera.Target = cameraTargetEnd;
				}
				character.CheckInAirJump();
				game.UpdateMeters();
				UpdateInAirRunPosition();
				UpdateRunStateMeters();
				yield return null;
			}
		}

		private void UpdateRunStateMeters()
		{
			float num = game.currentSpeed * Time.deltaTime;
			GameStats gameStats = GameStats.Instance;
			if (currentRunPosition != RunPositions.air)
			{
				if (character.TrackIndex == 0)
				{
					gameStats.metersRunLeftTrack += num;
				}
				if (character.TrackIndex == 1)
				{
					gameStats.metersRunCenterTrack += num;
				}
				if (character.TrackIndex == 2)
				{
					gameStats.metersRunRightTrack += num;
				}
			}
			if (currentRunPosition == RunPositions.ground)
			{
				GameStats.Instance.metersRunGround += num;
			}
			if (currentRunPosition == RunPositions.air)
			{
				GameStats.Instance.metersFly += num;
			}
			if (currentRunPosition == RunPositions.station)
			{
				GameStats.Instance.metersRunStation += num;
			}
			if (currentRunPosition == RunPositions.train)
			{
				GameStats.Instance.metersRunTrain += num;
			}
			if (currentRunPosition == RunPositions.movingTrain)
			{
				GameStats.Instance.metersRunTrain += num;
			}
		}

		private void UpdateInAirRunPosition()
		{
			if (!characterController.isGrounded)
			{
				currentRunPosition = RunPositions.air;
			}
		}

		private void LandedOnTrain(Collider trainCollider)
		{
			if (character.hoverboard.enabled && !GrindedTrains.Contains(trainCollider))
			{
				if (GrindedTrains.Count > GrindedTrainsBufferSize)
				{
					GrindedTrains.Dequeue();
				}
				GrindedTrains.Enqueue(trainCollider);
				GameStats.Instance.grindedTrains++;
			}
			MissionsManager.Instance.PlayerDidThis(MissionTarget.LandOnTrainInRow);
		}

		private void UpdateGroundTag(Transform characterTransform)
		{
			Ray ray = new Ray(character.characterRoot.position, -Vector3.up);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo))
			{
				switch (hitInfo.collider.tag)
				{
				case "Ground":
					currentRunPosition = RunPositions.ground;
					MissionsManager.Instance.RemoveProgressForThis(MissionTarget.LandOnTrainInRow);
					break;
				case "HitTrain":
					currentRunPosition = RunPositions.train;
					LandedOnTrain(hitInfo.collider);
					break;
				case "HitMovingTrain":
					currentRunPosition = RunPositions.movingTrain;
					LandedOnTrain(hitInfo.collider);
					break;
				case "Station":
					currentRunPosition = RunPositions.station;
					break;
				}
			}
		}

		public override void HandleCriticalHit()
		{
			character.characterCamera.Shake();
			game.Die();
		}

		public override void HandleSwipe(SwipeDir swipeDir)
		{
			switch (swipeDir)
			{
			case SwipeDir.None:
				break;
			case SwipeDir.Left:
				if (character.IsTeleboard)
				{
					character.ChangeTrack(-1, 20f / game.currentSpeed);
				}
				else
				{
					character.ChangeTrack(-1, characterChangeTrackLength / game.currentSpeed);
				}
				break;
			case SwipeDir.Right:
				if (character.IsTeleboard)
				{
					character.ChangeTrack(1, 20f / game.currentSpeed);
				}
				else
				{
					character.ChangeTrack(1, characterChangeTrackLength / game.currentSpeed);
				}
				break;
			case SwipeDir.Up:
				character.Jump();
				break;
			case SwipeDir.Down:
				character.Roll();
				break;
			}
		}

		public void DebugSuperJump()
		{
			character.DebugSuperJump();
		}

		public override void HandleDoubleTap()
		{
			int upgradeAmount = PlayerInfo.Instance.GetUpgradeAmount(PowerupType.hoverboard);
			if (PlayerInfo.Instance.IsTutorialCompleted)
			{
				if (game.Modifiers.Hoverboard.IsAllowed && !game.modifiers.IsActive(game.modifiers.Hoverboard) && !Hoverboard.Instance.IsARewardFromMysteryPowerup)
				{
					if (upgradeAmount > 0)
					{
						game.Modifiers.Add(game.Modifiers.Hoverboard);
					}
					if (!game.Modifiers.Hoverboard.IsInCooldown)
					{
						LogEventForHoverboardStartAttempt(upgradeAmount > 0);
					}
				}
			}
			else if (game.Modifiers.Hoverboard.IsAllowed && !game.modifiers.IsActive(game.modifiers.Hoverboard))
			{
				if (upgradeAmount > 0)
				{
					game.Modifiers.Add(game.Modifiers.Hoverboard);
				}
				if (!game.Modifiers.Hoverboard.IsInCooldown)
				{
					LogEventForHoverboardStartAttempt(upgradeAmount > 0);
				}
			}
		}

		public void StartTunnel(float tunnelLength)
		{
			tunnelStartZ = character.z;
			offsetDeltaCurve = new Curve();
			offsetDeltaCurve.AddKey(0f, Vector3.zero, -Vector3.up, -Vector3.up);
			offsetDeltaCurve.AddKey(tunnelLength / 2f, Vector3.up * tunnelDelta);
			offsetDeltaCurve.AddKey(tunnelLength, Vector3.zero, Vector3.up * 0.001f, Vector3.up * 0.001f);
		}

		public void EndTunnel()
		{
		}

		private void LogEventForHoverboardStartAttempt(bool successful)
		{
			if (successful)
			{
				Flurry.LogEventWithAParameter("Boost hoverboard activated", "successfully", "yes");
			}
			else
			{
				Flurry.LogEventWithAParameter("Boost hoverboard activated", "successfully", "no");
			}
		}
	}
}
