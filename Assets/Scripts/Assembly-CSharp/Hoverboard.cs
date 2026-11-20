using System;
using System.Collections;
using System.Collections.Generic;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Managers;
using SYBO.Subway;
using SYBO.Subway.Characters;
using UnityEngine;

public class Hoverboard : CharacterModifier
{
	public enum BoardAbility
	{
		None = 0,
		Jumper = 1,
		Lowrider = 2,
		HighSpeed = 3,
		Speed = 4,
		DoubleJump = 5,
		Glider = 6,
		Teleport = 7,
		Vanity = 8
	}

	[Serializable]
	public class HoverboardSelection
	{
		[Serializable]
		public class CustomSet
		{
			public BoardAbility ability;

			public GameObject baseModelPrefab;

			public GameObject modelPrefab;

			public GameObject VanityFxPrefab;

			public GameObject VanityMenuFxPrefab;

			public bool useDefaultmodel = true;

			public bool useSuperSneakers = true;
		}

		public Hoverboards.BoardType boardType;

		public GameObject hoverboardPrefab;

		public CustomSet[] customSets;
	}

	public delegate void OnSwitchToHoverboardDelegate(GameObject hoverbooard, BoardAbility[] abilities);

	public delegate void OnEndHoverboardDelegate();

	public delegate void OnHoverboardJumpDelegate();

	public delegate void OnRunDelegate();

	public delegate void OnSpeedStartDelegate();

	public delegate void OnSpeedEndDelegate();

	private bool isJumper;

	private bool isLowrider;

	private bool isHighSpeedBoard;

	private bool isSpeedBoard;

	private bool isGlider;

	private bool isVTol;

	private bool isTeleporter;

	private bool isDoubleJump;

	private bool isVanity;

	[OptionalField]
	public GameObject currentHoverboard;

	public HoverboardSelection[] hoverboardSelector;

	public AudioClipInfo powerDownSound;

	public float cooldownDstance = 50f;

	public float slowMotionDistance = 90f;

	public float slowDownToScale = 0.3f;

	private GameObject hoverboardRoot;

	public float WaitForParticlesDelay;

	public float RemoveObstaclesDistance = 250f;

	private LongMagnet longMagnet;

	private Character character;

	private Track track;

	[HideInInspector]
	public bool isActive;

	public bool _isInCooldown;

	private bool __isAllowed = true;

	[HideInInspector]
	private bool _isARewardFromMysteryPowerup;

	public AudioClipInfo CrashSound;

	public AudioClipInfo StartSound;

	public ActivePowerup Powerup;

	private static Hoverboard instance;

	private Hoverboards.BoardType _currentRandomBoard;

	private Dictionary<Hoverboards.BoardType, GameObject> hoverboardThatMatchBoardType = new Dictionary<Hoverboards.BoardType, GameObject>();

	private Dictionary<Hoverboards.BoardType, HoverboardSelection.CustomSet[]> BoardtypeCustomSets = new Dictionary<Hoverboards.BoardType, HoverboardSelection.CustomSet[]>();

	public bool IsJumper
	{
		get
		{
			return isJumper;
		}
	}

	public bool IsLowrider
	{
		get
		{
			return isLowrider;
		}
	}

	public bool IsHighSpeedBoard
	{
		get
		{
			return isHighSpeedBoard;
		}
	}

	public bool IsSpeedBoard
	{
		get
		{
			return isSpeedBoard;
		}
	}

	public bool IsGlider
	{
		get
		{
			return isGlider;
		}
	}

	public bool IsVTol
	{
		get
		{
			return isVTol;
		}
	}

	public bool IsTeleporter
	{
		get
		{
			return isTeleporter;
		}
	}

	public bool IsDoubleJump
	{
		get
		{
			return isDoubleJump;
		}
	}

	public bool IsVanity
	{
		get
		{
			return isVanity;
		}
	}

	public bool IsInCooldown
	{
		get
		{
			return _isInCooldown;
		}
	}

	public bool IsAllowed
	{
		get
		{
			return __isAllowed;
		}
		set
		{
			__isAllowed = value;
		}
	}

	public bool IsARewardFromMysteryPowerup
	{
		get
		{
			return _isARewardFromMysteryPowerup;
		}
		set
		{
			_isARewardFromMysteryPowerup = value;
		}
	}

	public override bool ShouldPauseInJetpack
	{
		get
		{
			return true;
		}
	}

	public static Hoverboard Instance
	{
		get
		{
			return instance ?? (instance = UnityEngine.Object.FindObjectOfType(typeof(Hoverboard)) as Hoverboard);
		}
	}

	public event OnSwitchToHoverboardDelegate OnSwitchToHoverboard;

	public event OnEndHoverboardDelegate OnEndHoverboard;

	public event OnHoverboardJumpDelegate OnJump;

	public event OnRunDelegate OnRun;

	public event OnSpeedStartDelegate OnSpeedStart;

	public event OnSpeedEndDelegate OnSpeedEnd;

	public void Awake()
	{
		instance = this;
	}

	public void intialize()
	{
		Game.Instance.OnCharacterModelRecreated += Game_OnCharacterModelRecreated;
		character = Character.Instance;
		hoverboardRoot = character.characterModel.AttachPoint1.gameObject;
		longMagnet = base.gameObject.GetComponent<LongMagnet>();
		track = Track.Instance;
		HoverboardSelection[] array = hoverboardSelector;
		foreach (HoverboardSelection hoverboardSelection in array)
		{
			if (!hoverboardThatMatchBoardType.ContainsKey(hoverboardSelection.boardType))
			{
				hoverboardThatMatchBoardType.Add(hoverboardSelection.boardType, hoverboardSelection.hoverboardPrefab);
				BoardtypeCustomSets.Add(hoverboardSelection.boardType, hoverboardSelection.customSets);
				continue;
			}
			throw new Exception("There are more hoverboards assigned to the hoverboard selection");
		}
	}

	private void Game_OnCharacterModelRecreated()
	{
		hoverboardRoot = character.characterModel.AttachPoint1.gameObject;
	}

	public GameObject GetActiveHoverboard()
	{
		GameObject value;
		if (hoverboardThatMatchBoardType.TryGetValue((!_isARewardFromMysteryPowerup) ? GetCurrentActiveHoverboard() : _currentRandomBoard, out value))
		{
			return value;
		}
		Debug.Log("You need to make a hoverboard that matches the selection");
		return null;
	}

	public HoverboardSelection.CustomSet[] GetActiveCustomSets(Hoverboards.BoardType board)
	{
		HoverboardSelection.CustomSet[] value;
		if (BoardtypeCustomSets.TryGetValue(board, out value))
		{
			return value;
		}
		return null;
	}

	private GameObject GetRandomHoverboard()
	{
		List<Hoverboards.BoardType> randomBoardList = MysteryPowerup.GetRandomBoardList();
		int index = UnityEngine.Random.Range(0, randomBoardList.Count);
		HoverboardSelection[] array = hoverboardSelector;
		foreach (HoverboardSelection hoverboardSelection in array)
		{
			if (randomBoardList.Count > 0)
			{
				if (hoverboardSelection.boardType == randomBoardList[index])
				{
					_currentRandomBoard = hoverboardSelection.boardType;
					return hoverboardSelection.hoverboardPrefab;
				}
				continue;
			}
			Debug.LogError("The eligible board list is empty.");
			break;
		}
		Debug.LogError("No eligible board found. Make sure the list contains at least 1 eligible board.");
		return GetActiveHoverboard();
	}

	public override void Reset()
	{
		character.immuneToCriticalHit = false;
		character.characterController.enabled = true;
		character.characterTrigger.enabled = true;
		hoverboardRoot.SetActive(false);
		isActive = false;
		SingletonMonobehavior<TimeScaleManager>.Instance.TimeScale = SingletonMonobehavior<TimeScaleManager>.Instance.TimeScaleInGameDefault;
		character.hoverboardCrashParticleSystem.gameObject.SetActive(false);
	}

	public BoardAbility[] SetupAbilitys(GameObject newBoard)
	{
		Renderer[] componentsInChildren = newBoard.GetComponentsInChildren<Renderer>();
		List<BoardAbility> list = new List<BoardAbility>();
		HoverboardSelection.CustomSet[] array = ((!_isARewardFromMysteryPowerup) ? GetActiveCustomSets(PlayerInfo.Instance.currentHoverboard) : GetActiveCustomSets(_currentRandomBoard));
		if (array.Length > 0)
		{
			bool flag = true;
			if (array.Length > 1 && HoverboardManager.Instance.IsCustomSetEquippedForBoard(PlayerInfo.Instance.currentHoverboard, 0) && !array[1].useDefaultmodel)
			{
				flag = false;
			}
			if (array.Length > 2 && HoverboardManager.Instance.IsCustomSetEquippedForBoard(PlayerInfo.Instance.currentHoverboard, 1) && !array[2].useDefaultmodel)
			{
				flag = false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if ((i != 0 || !flag) && (i != 1 || !HoverboardManager.Instance.IsCustomSetEquippedForBoard(PlayerInfo.Instance.currentHoverboard, 0)) && (i != 2 || !HoverboardManager.Instance.IsCustomSetEquippedForBoard(PlayerInfo.Instance.currentHoverboard, 1)))
				{
					continue;
				}
				list.Add(array[i].ability);
				if (array[i].baseModelPrefab != null)
				{
					character.characterModel.AddCustomHoverboardModel(array[i].baseModelPrefab, false, newBoard.transform);
					Renderer[] array2 = componentsInChildren;
					foreach (Renderer renderer in array2)
					{
						renderer.enabled = false;
					}
				}
				if (array[i].modelPrefab != null)
				{
					character.characterModel.AddCustomHoverboardModel(array[i].modelPrefab, false, newBoard.transform);
				}
				if (array[i].VanityFxPrefab != null)
				{
					character.characterModel.AddCustomHoverboardModel(array[i].VanityFxPrefab, false, newBoard.transform);
				}
				if (array[i].ability == BoardAbility.Jumper)
				{
					isJumper = true;
					SuperSneakers.Instance.Pause();
					if (array[i].useSuperSneakers)
					{
						character.characterModel.MeshSuperSneaker.enabled = true;
					}
					longMagnet.Activate();
					playThruster componentInChildren = newBoard.GetComponentInChildren<playThruster>();
					if (componentInChildren != null && componentInChildren.jumpStyle == playThruster.JumpStyle.Super)
					{
						componentInChildren.Initialize();
					}
				}
				else if (array[i].ability == BoardAbility.Lowrider)
				{
					isLowrider = true;
					character.SqueezeCollider.Add(this);
				}
				else if (array[i].ability == BoardAbility.HighSpeed)
				{
					if (this.OnSpeedStart != null)
					{
						this.OnSpeedStart();
					}
					isHighSpeedBoard = true;
					Game.Instance.ActivateHighSpeed("highSpeedboard");
				}
				else if (array[i].ability == BoardAbility.Speed)
				{
					if (this.OnSpeedStart != null)
					{
						this.OnSpeedStart();
					}
					isSpeedBoard = true;
					Game.Instance.ActivateHighSpeed("speedboard");
				}
				else if (array[i].ability == BoardAbility.Glider)
				{
					isGlider = true;
					longMagnet.Activate();
					GliderAnimation componentInChildren2 = newBoard.GetComponentInChildren<GliderAnimation>();
					if (componentInChildren2 != null)
					{
						componentInChildren2.AddEvents();
					}
				}
				else if (array[i].ability == BoardAbility.Teleport)
				{
					isTeleporter = true;
				}
				else if (array[i].ability == BoardAbility.DoubleJump)
				{
					isDoubleJump = true;
					playThruster componentInChildren3 = newBoard.GetComponentInChildren<playThruster>();
					if (componentInChildren3 != null && componentInChildren3.jumpStyle == playThruster.JumpStyle.Double)
					{
						componentInChildren3.Initialize();
					}
				}
				else if (array[i].ability == BoardAbility.Vanity)
				{
					isVanity = true;
				}
			}
		}
		return list.ToArray();
	}

	public GameObject GetCustomModel(Hoverboards.BoardType boardtype, int set)
	{
		GameObject result = null;
		HoverboardSelection.CustomSet[] value;
		if (BoardtypeCustomSets.TryGetValue(boardtype, out value))
		{
			result = value[set].modelPrefab;
		}
		return result;
	}

	public void ResetAbilitys()
	{
		if (isJumper)
		{
			if (!longMagnet.ShouldBeActive)
			{
				longMagnet.Deactivate();
			}
			if (!SuperSneakers.Instance.isActive)
			{
				character.characterModel.MeshSuperSneaker.enabled = false;
			}
			SuperSneakers.Instance.Resume();
			isJumper = false;
		}
		if (isLowrider)
		{
			character.SqueezeCollider.Remove(this);
			isLowrider = false;
		}
		if (isHighSpeedBoard)
		{
			if (this.OnSpeedEnd != null)
			{
				this.OnSpeedEnd();
			}
			Game.Instance.DeactivateHighSpeed();
			isHighSpeedBoard = false;
		}
		if (isSpeedBoard)
		{
			if (this.OnSpeedEnd != null)
			{
				this.OnSpeedEnd();
			}
			Game.Instance.DeactivateHighSpeed();
			isSpeedBoard = false;
		}
		if (isGlider)
		{
			isGlider = false;
			if (!longMagnet.ShouldBeActive)
			{
				longMagnet.Deactivate();
			}
		}
		if (isTeleporter)
		{
			isTeleporter = false;
		}
		if (isDoubleJump)
		{
			isDoubleJump = false;
		}
		character.characterModel.DeleteBoardModels();
	}

	public override IEnumerator Begin()
	{
		if (!IsAllowed || _isInCooldown)
		{
			yield break;
		}
		hoverboardRoot.SetActive(true);
		if (!_isARewardFromMysteryPowerup)
		{
			PlayerInfo.Instance.UseUpgrade(PowerupType.hoverboard);
		}
		else
		{
			ResetAbilitys();
			GetRandomHoverboard();
			UIScreenController.Instance.messageHelper.ShowMessage(Strings.Get(Hoverboards.boardData[_currentRandomBoard].name) + " " + Strings.Get(StringID.MYSTERY_POWERUP_RANDOM_HOVERBOARD_TEXT), true);
		}
		MissionsManager.Instance.PlayerDidThis(MissionTarget.HoverBoard);
		Powerup = GameStats.Instance.TriggerPowerup(PowerupType.hoverboard);
		base.Paused = false;
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		isActive = true;
		GameObject newBoard = character.characterModel.AddHoverboardModel();
		BoardAbility[] abilities = SetupAbilitys(newBoard);
		if (this.OnSwitchToHoverboard != null)
		{
			this.OnSwitchToHoverboard(newBoard, abilities);
		}
		So.Instance.playSound(StartSound);
		character.CharacterPickupParticleSystem.PickedUpDefaultPowerUp();
		character.immuneToCriticalHit = true;
		stop = StopSignal.DONT_STOP;
		while (Powerup.timeLeft > 0f && stop == StopSignal.DONT_STOP)
		{
			yield return null;
		}
		character.immuneToCriticalHit = false;
		isActive = false;
		ResetAbilitys();
		if (stop == StopSignal.DONT_STOP)
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.HoverBoardExpire);
			So.Instance.playSound(powerDownSound);
			_isInCooldown = true;
			_Trigger_OnEndHoverboard();
			if (character.IsFalling || character.IsJumping)
			{
				if (this.OnJump != null)
				{
					this.OnJump();
				}
			}
			else if (this.OnRun != null)
			{
				this.OnRun();
			}
			UnityEngine.Object.Destroy(newBoard);
		}
		if (stop != StopSignal.STOP)
		{
			yield break;
		}
		isActive = false;
		character.immuneToCriticalHit = false;
		_isInCooldown = true;
		character.hoverboardCrashParticleSystem.gameObject.SetActive(true);
		character.hoverboardCrashParticleSystem.Play();
		PlayCrashSound();
		_Trigger_OnEndHoverboard();
		if (this.OnJump != null)
		{
			this.OnJump();
		}
		UnityEngine.Object.Destroy(newBoard);
		float timeLeft = WaitForParticlesDelay;
		while (timeLeft > 0f)
		{
			timeLeft -= Time.deltaTime;
			yield return null;
		}
		track.LayEmptyChunks(character.z, RemoveObstaclesDistance * Game.Instance.NormalizedGameSpeed);
		character.Revive();
		if (character.IsRolling)
		{
			character.EndRoll();
		}
		float newSlowMotionDistance = slowMotionDistance * Game.Instance.NormalizedGameSpeed;
		float newCoolDownDist = cooldownDstance * Game.Instance.NormalizedGameSpeed;
		float distanceLeft = newSlowMotionDistance;
		bool didStopCooldown = false;
		while (distanceLeft > 0f)
		{
			distanceLeft -= Game.Instance.currentLevelSpeed * Time.deltaTime;
			newCoolDownDist -= Game.Instance.currentLevelSpeed * Time.deltaTime;
			if (newCoolDownDist < 0f && !didStopCooldown)
			{
				character.immuneToCriticalHit = false;
				didStopCooldown = true;
			}
			FinishRandomBoard();
			yield return null;
		}
		character.hoverboardCrashParticleSystem.gameObject.SetActive(false);
	}

	public void PlayCrashSound()
	{
		So.Instance.playSound(CrashSound);
	}

	public override void Pause()
	{
		if (character.characterModel.CurrentHoverboard != null)
		{
			character.characterModel.CurrentHoverboard.SetActive(false);
		}
		bool flag = PlayerInfo.Instance.currentHoverboard == Hoverboards.BoardType.speedboard;
		bool flag2 = PlayerInfo.Instance.currentHoverboard == Hoverboards.BoardType.speedboard;
		if (flag)
		{
			Game.Instance.DeactivateHighSpeed();
		}
		if (flag2)
		{
			Game.Instance.DeactivateHighSpeed();
		}
	}

	public override void Resume()
	{
		hoverboardRoot.SetActive(true);
		if (IsHighSpeedBoard)
		{
			Game.Instance.ActivateHighSpeed("highSpeedboard");
		}
		if (IsSpeedBoard)
		{
			Game.Instance.ActivateHighSpeed("speedboard");
		}
	}

	private void FinishRandomBoard()
	{
		_currentRandomBoard = Hoverboards.BoardType.normal;
	}

	public Hoverboards.BoardType GetCurrentActiveHoverboard()
	{
		if (_isARewardFromMysteryPowerup)
		{
			return _currentRandomBoard;
		}
		return PlayerInfo.Instance.currentHoverboard;
	}

	public override void ExpireSoon()
	{
		Powerup.timeLeft = CharacterModifierCollection.TIME_LEFT_FOR_EXPIRE_SOON;
	}

	public void HardReset()
	{
		isActive = false;
		_isInCooldown = false;
		if (PlayerInfo.Instance.IsTutorialCompleted)
		{
			IsAllowed = true;
		}
		else
		{
			IsAllowed = false;
		}
		_isARewardFromMysteryPowerup = false;
		FinishRandomBoard();
	}

	private void _Trigger_OnEndHoverboard()
	{
		ResetAbilitys();
		if (this.OnEndHoverboard != null)
		{
			this.OnEndHoverboard();
		}
	}
}
