using System;
using UnityEngine;

[Serializable]
public class conveyorBelt : MonoBehaviour
{
	public float scrollSpeed;

	public Material mat;

	public conveyorBelt()
	{
		scrollSpeed = 0.1f;
	}

	public virtual void Start()
	{
		enabled = false;
	}

	public virtual void OnBecameVisible()
	{
		enabled = true;
	}

	public virtual void OnBecameInvisible()
	{
		enabled = false;
	}

	public virtual void Update()
	{
		float num = Time.time * scrollSpeed % 1f;
		mat.SetTextureOffset("_MainTex", new Vector2(0f, 0f - num));
		mat.SetTextureOffset("_BumpMap", new Vector2(0f, 0f - num));
	}

	public virtual void Main()
	{
	}
}
