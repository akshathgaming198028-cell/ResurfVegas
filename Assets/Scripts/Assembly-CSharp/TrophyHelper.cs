using System;
using UnityEngine;

public class TrophyHelper : MonoBehaviour
{
	private OldTrophies.Trophy _trophy;

	public UILabel title;

	public UILabel description;

	public UISprite image;

	private bool _hasInited;

	private void OnEnable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.OnTrophyUnlocked = (Action<OldTrophies.Trophy>)Delegate.Combine(instance.OnTrophyUnlocked, new Action<OldTrophies.Trophy>(onTrophyUnlocked));
		updateDisplay();
	}

	private void OnDisable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.OnTrophyUnlocked = (Action<OldTrophies.Trophy>)Delegate.Remove(instance.OnTrophyUnlocked, new Action<OldTrophies.Trophy>(onTrophyUnlocked));
	}

	private void onTrophyUnlocked(OldTrophies.Trophy trophy)
	{
		updateDisplay();
	}

	public void setTrophy(OldTrophies.Trophy trophy)
	{
		_trophy = trophy;
		_hasInited = true;
		updateDisplay();
	}

	public void updateDisplay()
	{
		if (!_hasInited)
		{
			return;
		}
		TrophyData trophyData = OldTrophies.trophyData[_trophy];
		title.text = Strings.Get(trophyData.name);
		description.text = Strings.Get(trophyData.description);
		if ((bool)image)
		{
			if (PlayerInfo.Instance.shouldSpawnTrophy(_trophy))
			{
				image.spriteName = trophyData.spriteUnlocked;
				image.color = Color.white;
			}
			else
			{
				image.spriteName = trophyData.spriteLocked;
				image.color = Color.white;
			}
			image.MakePixelPerfect();
		}
	}
}
