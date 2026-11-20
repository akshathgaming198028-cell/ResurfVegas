using System;
using UnityEngine;

[Serializable]
public class SpiderAnimationTest : MonoBehaviour
{
	public Rigidbody rigid;

	public AnimationClip forwardAnim;

	public AnimationClip backAnim;

	public AnimationClip leftAnim;

	public AnimationClip rightAnim;

	public float walking;

	public float angle;

	private Transform tr;

	public virtual void OnEnable()
	{
		tr = rigid.transform;
		GetComponent<Animation>()[forwardAnim.name].layer = 1;
		GetComponent<Animation>()[forwardAnim.name].enabled = true;
		GetComponent<Animation>()[backAnim.name].layer = 1;
		GetComponent<Animation>()[backAnim.name].enabled = true;
		GetComponent<Animation>()[leftAnim.name].layer = 1;
		GetComponent<Animation>()[leftAnim.name].enabled = true;
		GetComponent<Animation>()[rightAnim.name].layer = 1;
		GetComponent<Animation>()[rightAnim.name].enabled = true;
		GetComponent<Animation>().SyncLayer(1);
	}

	public virtual void Update()
	{
		rigid.velocity = Quaternion.Euler(0f, angle, 0f) * rigid.transform.forward * 2.4f * walking;
		Vector3 velocity = rigid.velocity;
		velocity.y = 0f;
		float speed = velocity.magnitude / 2.4f;
		GetComponent<Animation>()[forwardAnim.name].speed = speed;
		GetComponent<Animation>()[rightAnim.name].speed = speed;
		GetComponent<Animation>()[backAnim.name].speed = speed;
		GetComponent<Animation>()[leftAnim.name].speed = speed;
		if (!(velocity == Vector3.zero))
		{
			float num = Mathf.DeltaAngle(HorizontalAngle(tr.forward), HorizontalAngle(rigid.velocity));
			float num2 = default(float);
			if (!(num >= -90f))
			{
				num2 = Mathf.InverseLerp(-180f, -90f, num);
				GetComponent<Animation>()[forwardAnim.name].weight = 0f;
				GetComponent<Animation>()[rightAnim.name].weight = 0f;
				GetComponent<Animation>()[backAnim.name].weight = 1f - num2;
				GetComponent<Animation>()[leftAnim.name].weight = 1f;
			}
			else if (!(num >= 0f))
			{
				num2 = Mathf.InverseLerp(-90f, 0f, num);
				GetComponent<Animation>()[forwardAnim.name].weight = num2;
				GetComponent<Animation>()[rightAnim.name].weight = 0f;
				GetComponent<Animation>()[backAnim.name].weight = 0f;
				GetComponent<Animation>()[leftAnim.name].weight = 1f - num2;
			}
			else if (!(num >= 90f))
			{
				num2 = Mathf.InverseLerp(0f, 90f, num);
				GetComponent<Animation>()[forwardAnim.name].weight = 1f - num2;
				GetComponent<Animation>()[rightAnim.name].weight = num2;
				GetComponent<Animation>()[backAnim.name].weight = 0f;
				GetComponent<Animation>()[leftAnim.name].weight = 0f;
			}
			else
			{
				num2 = Mathf.InverseLerp(90f, 180f, num);
				GetComponent<Animation>()[forwardAnim.name].weight = 0f;
				GetComponent<Animation>()[rightAnim.name].weight = 1f - num2;
				GetComponent<Animation>()[backAnim.name].weight = num2;
				GetComponent<Animation>()[leftAnim.name].weight = 0f;
			}
		}
	}

	public static float HorizontalAngle(Vector3 direction)
	{
		return Mathf.Atan2(direction.x, direction.z) * 57.29578f;
	}

	public virtual void OnGUI()
	{
		GUILayout.Label("Angle (0 to 360): " + angle.ToString("0.00"));
		angle = GUILayout.HorizontalSlider(angle, 0f, 360f, GUILayout.Width(200f));
		for (int i = 0; i <= 360; i += 45)
		{
			if (!(Mathf.Abs(angle - (float)i) >= 10f))
			{
				angle = i;
			}
		}
		GUILayout.Label("Walking (0 to 1): " + walking.ToString("0.00"));
		walking = GUILayout.HorizontalSlider(walking, 0f, 1f, GUILayout.Width(100f));
	}

	public virtual void Main()
	{
	}
}
