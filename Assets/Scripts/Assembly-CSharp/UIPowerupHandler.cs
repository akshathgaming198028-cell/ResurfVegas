using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIPowerupHandler : MonoBehaviour
{
	public GameObject PowerupPrefab;

	private Vector3 _offScreenPosition = new Vector3(0f, -1000f, 0f);

	private int _screenOffset = 26;

	private int _bottomOffset = 5;

	private List<UIPowerupHelper> _powerupSlots = new List<UIPowerupHelper>();

	private void Update()
	{
		List<ActivePowerup> activePowerups = GameStats.Instance.GetActivePowerups();
		int num = 0;
		for (int num2 = activePowerups.Count - 1; num2 >= 0; num2--)
		{
			if (_powerupSlots.Count < activePowerups.Count)
			{
				int count = _powerupSlots.Count;
				GameObject gameObject = NGUITools.AddChild(base.gameObject, PowerupPrefab);
				gameObject.transform.localPosition = _offScreenPosition;
				_powerupSlots.Add(gameObject.GetComponent<UIPowerupHelper>());
				if (count % 2 == 0)
				{
					_powerupSlots[count].GetComponent<UIAnchor>().side = UIAnchor.Side.BottomLeft;
				}
				else
				{
					_powerupSlots[count].GetComponent<UIAnchor>().side = UIAnchor.Side.BottomRight;
				}
			}
			if (activePowerups[num2].type != PowerupType.hoverboard || !activePowerups.Any((ActivePowerup powerup) => powerup.type == PowerupType.mysteryPowerup))
			{
				_powerupSlots[num].SetPowerupSlot(activePowerups[num2]);
			}
			if (!_powerupSlots[num].gameObject.activeInHierarchy)
			{
				_powerupSlots[num].gameObject.SetActive(true);
			}
			float x = _screenOffset;
			if (_powerupSlots[num].GetComponent<UIAnchor>().side == UIAnchor.Side.BottomRight)
			{
				x = _powerupSlots[num].getSliderWidth() * -1 + _screenOffset / 2;
			}
			float num3 = 0f;
			num3 = (float)getNumberOfSpaces(num2) + getSliderHights(num2, num) + (float)_bottomOffset;
			_powerupSlots[num].setContainerPosition(x, num3, 0f);
			num++;
		}
		for (; num < _powerupSlots.Count; num++)
		{
			if (_powerupSlots[num].gameObject.activeInHierarchy)
			{
				_powerupSlots[num].HidePowerupSlot();
				_powerupSlots[num].transform.localPosition = _offScreenPosition;
				_powerupSlots[num].gameObject.SetActive(false);
			}
		}
	}

	public float getSliderHights(int i, int slotPosition)
	{
		return (float)Mathf.FloorToInt(i / 2) * _powerupSlots[slotPosition].getSliderHeight();
	}

	public int getNumberOfSpaces(int i)
	{
		if (i % 2 == 0)
		{
			return _screenOffset / 4 * (Mathf.CeilToInt((float)i / 2f) + 1);
		}
		return _screenOffset / 4 * Mathf.CeilToInt((float)i / 2f);
	}
}
