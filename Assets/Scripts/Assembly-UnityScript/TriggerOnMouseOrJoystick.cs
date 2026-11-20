using System;
using UnityEngine;

[Serializable]
public class TriggerOnMouseOrJoystick : MonoBehaviour
{
	public SignalSender mouseDownSignals;

	public SignalSender mouseUpSignals;

	private bool state;

	private Joystick[] joysticks;

	public virtual void Start()
	{
		joysticks = ((Joystick[])UnityEngine.Object.FindObjectsOfType(typeof(Joystick))) as Joystick[];
	}

	public virtual void Update()
	{
		if (!state && joysticks[0].tapCount > 0)
		{
			mouseDownSignals.SendSignals(this);
			state = true;
		}
		else if (joysticks[0].tapCount <= 0)
		{
			mouseUpSignals.SendSignals(this);
			state = false;
		}
	}

	public virtual void Main()
	{
	}
}
