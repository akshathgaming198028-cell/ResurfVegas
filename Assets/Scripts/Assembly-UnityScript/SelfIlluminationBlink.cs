using System;
using UnityEngine;

[Serializable]
public class SelfIlluminationBlink : MonoBehaviour
{
	public float blink;

	public virtual void OnWillRenderObject()
	{
		GetComponent<Renderer>().sharedMaterial.SetFloat("_SelfIllumStrength", blink);
	}

	public virtual void Blink()
	{
		blink = 1f - blink;
	}

	public virtual void Main()
	{
	}
}
