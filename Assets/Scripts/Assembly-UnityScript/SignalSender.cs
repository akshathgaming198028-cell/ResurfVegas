using System;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class SignalSender
{
	public bool onlyOnce;

	public ReceiverItem[] receivers;

	private bool hasFired;

	public virtual void SendSignals(MonoBehaviour sender)
	{
		if (!hasFired || !onlyOnce)
		{
			for (int i = 0; i < Extensions.get_length((Array)receivers); i++)
			{
				sender.StartCoroutine(receivers[i].SendWithDelay(sender));
			}
			hasFired = true;
		}
	}
}
