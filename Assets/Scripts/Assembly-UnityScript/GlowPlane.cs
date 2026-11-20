using System;
using UnityEngine;

[Serializable]
public class GlowPlane : MonoBehaviour
{
	public Transform playerTransform;

	private Vector3 pos;

	private Vector3 scale;

	public float minGlow;

	public float maxGlow;

	public Color glowColor;

	private Material mat;

	public GlowPlane()
	{
		minGlow = 0.2f;
		maxGlow = 0.5f;
		glowColor = Color.white;
	}

	public virtual void Start()
	{
		if (!playerTransform)
		{
			playerTransform = GameObject.FindWithTag("Player").transform;
		}
		pos = transform.position;
		scale = transform.localScale;
		mat = GetComponent<Renderer>().material;
		enabled = false;
	}

	public virtual void OnDrawGizmos()
	{
		Gizmos.color = glowColor;
		float a = maxGlow * 0.25f;
		Color color = Gizmos.color;
		float num = (color.a = a);
		Color color2 = (Gizmos.color = color);
		Gizmos.matrix = transform.localToWorldMatrix;
		Vector3 size = 5f * Vector3.Scale(Vector3.one, new Vector3(1f, 0f, 1f));
		Gizmos.DrawCube(Vector3.zero, size);
		Gizmos.matrix = Matrix4x4.identity;
	}

	public virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = glowColor;
		float a = maxGlow;
		Color color = Gizmos.color;
		float num = (color.a = a);
		Color color2 = (Gizmos.color = color);
		Gizmos.matrix = transform.localToWorldMatrix;
		Vector3 size = 5f * Vector3.Scale(Vector3.one, new Vector3(1f, 0f, 1f));
		Gizmos.DrawCube(Vector3.zero, size);
		Gizmos.matrix = Matrix4x4.identity;
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
		Vector3 vector = pos - playerTransform.position;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		transform.localScale = Vector3.Lerp(Vector3.one * minGlow, scale, Mathf.Clamp01(magnitude * 0.35f));
		mat.SetColor("_TintColor", glowColor * Mathf.Clamp(magnitude * 0.1f, minGlow, maxGlow));
	}

	public virtual void Main()
	{
	}
}
