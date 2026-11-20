using System;
using UnityEngine;

[Serializable]
public class MuzzleFlashAnimate : MonoBehaviour
{
	public virtual void Update()
	{
		transform.localScale = Vector3.one * UnityEngine.Random.Range(0.5f, 1.5f);
		float z = UnityEngine.Random.Range(0f, 90f);
		Vector3 localEulerAngles = transform.localEulerAngles;
		float num = (localEulerAngles.z = z);
		Vector3 vector = (transform.localEulerAngles = localEulerAngles);
	}

	public virtual void Main()
	{
	}
}
