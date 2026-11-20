using System.Collections;
using System.Collections.Generic;
using SYBO.Subway;
using SYBO.Subway.Info;
using UnityEngine;

public class SwapCharacter : CharacterModifier
{
	public float ratio;

	public ActivePowerup Powerup;

	public AudioClipInfo powerDownSound;

	public static bool swapCharacterEnabled;

	private static SwapCharacter instance;

	private string newCharacterName;

	public static SwapCharacter Instance
	{
		get
		{
			return instance ?? (instance = Object.FindObjectOfType(typeof(SwapCharacter)) as SwapCharacter);
		}
	}

	private void Awake()
	{
		instance = this;
	}

	public override IEnumerator Begin()
	{
		GameStats.Instance.pickedUpPowerups++;
		base.Paused = false;
		stop = StopSignal.DONT_STOP;
		Powerup = GameStats.Instance.TriggerPowerup(PowerupType.swapCharacter);
		float duration = Powerup.timeLeft;
		StartSwapCharacter();
		while (Powerup.timeLeft >= 0f && stop == StopSignal.DONT_STOP)
		{
			ratio = Powerup.timeLeft / duration;
			yield return null;
		}
		StopSwapCharacter();
		if (Powerup.timeLeft <= 0f)
		{
			So.Instance.playSound(powerDownSound);
		}
	}

	public override void Reset()
	{
		StopSwapCharacter();
	}

	public void StartSwapCharacter()
	{
		Powerup = GameStats.Instance.TriggerPowerup(PowerupType.swapCharacter);
		swapCharacterEnabled = true;
		DoTheSwap();
		UIScreenController.Instance.messageHelper.ShowMessage(Strings.Get(Upgrades.upgrades[PowerupType.swapCharacter].name) + "\n" + newCharacterName, true);
	}

	public void StopSwapCharacter()
	{
		swapCharacterEnabled = false;
		DoTheSwap();
	}

	private void DoTheSwap()
	{
		if (swapCharacterEnabled)
		{
			List<Characters.Model> list = new List<Characters.Model>();
			foreach (KeyValuePair<Characters.CharacterType, Characters.Model> characterDatum in Characters.characterData)
			{
				if (characterDatum.Value.modelName != Game.Instance.startCharacterName && characterDatum.Value.modelName != "yutani")
				{
					if (!characterDatum.Value.hasOnlineSettings)
					{
						list.Add(characterDatum.Value);
					}
					else if (PlayerInfo.Instance.CharacterExpirationTimespan(characterDatum.Key).TotalSeconds > 0.0)
					{
						list.Add(characterDatum.Value);
					}
				}
			}
			int index = Random.Range(0, list.Count);
			Game.Instance.character.characterModel.ChangeCharacterModel(list[index].modelName, 0, false, true);
			newCharacterName = Strings.Get(list[index].name);
		}
		else
		{
			Game.Instance.character.characterModel.ChangeCharacterModel(Game.Instance.startCharacterName, Game.Instance.startCharacterTheme, false, true);
			newCharacterName = string.Empty;
		}
	}
}
