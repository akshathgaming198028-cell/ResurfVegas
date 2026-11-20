using System;
using UnityEngine;

[Serializable]
public class MechAnimationTest : MonoBehaviour
{
	public float turning;

	public float walking;

	public float turnOffset;

	public Rigidbody rigid;

	public AnimationClip idle;

	public AnimationClip walk;

	public AnimationClip turnLeft;

	public AnimationClip turnRight;

	public SignalSender footstepSignals;

	public virtual void OnEnable()
	{
		GetComponent<Animation>()[idle.name].layer = 0;
		GetComponent<Animation>()[idle.name].weight = 1f;
		GetComponent<Animation>()[idle.name].enabled = true;
		GetComponent<Animation>()[walk.name].layer = 1;
		GetComponent<Animation>()[turnLeft.name].layer = 1;
		GetComponent<Animation>()[turnRight.name].layer = 1;
		GetComponent<Animation>()[walk.name].weight = 1f;
		GetComponent<Animation>()[turnLeft.name].weight = 0f;
		GetComponent<Animation>()[turnRight.name].weight = 0f;
		GetComponent<Animation>()[walk.name].enabled = true;
		GetComponent<Animation>()[turnLeft.name].enabled = true;
		GetComponent<Animation>()[turnRight.name].enabled = true;
	}

	public virtual void FixedUpdate()
	{
		GetComponent<Animation>()[walk.name].speed = Mathf.Lerp(1f, GetComponent<Animation>()[walk.name].length / GetComponent<Animation>()[turnLeft.name].length, Mathf.Abs(turning));
		GetComponent<Animation>()[turnLeft.name].time = GetComponent<Animation>()[walk.name].time + turnOffset;
		GetComponent<Animation>()[turnRight.name].time = GetComponent<Animation>()[walk.name].time + turnOffset;
		rigid.velocity = rigid.transform.forward * 2.5f * walking;
		rigid.angularVelocity = Vector3.up * turning * 100f * ((float)Math.PI / 180f);
		float num = rigid.angularVelocity.y * 57.29578f / 100f;
		float value = rigid.velocity.magnitude / 2.5f;
		GetComponent<Animation>()[turnLeft.name].weight = Mathf.Clamp01(0f - num);
		GetComponent<Animation>()[turnRight.name].weight = Mathf.Clamp01(num);
		GetComponent<Animation>()[walk.name].weight = Mathf.Clamp01(value);
	}

	public virtual void OnGUI()
	{
		GUILayout.Label("Walking (0 to 1): " + walking.ToString("0.00"));
		walking = GUILayout.HorizontalSlider(walking, 0f, 1f, GUILayout.Width(100f));
		if (GUI.changed)
		{
			turning = Mathf.Clamp(Mathf.Abs(turning), 0f, 1f - walking) * Mathf.Sign(turning);
			GUI.changed = false;
		}
		GUILayout.Label("Turning (-1 to 1): " + turning.ToString("0.00"));
		turning = GUILayout.HorizontalSlider(turning, -1f, 1f, GUILayout.Width(100f));
		if (!(Mathf.Abs(turning) >= 0.1f))
		{
			turning = 0f;
		}
		if (GUI.changed)
		{
			walking = Mathf.Clamp(walking, 0f, 1f - Mathf.Abs(turning));
			GUI.changed = false;
		}
		GUILayout.Label("Offset to turning anims (-0.5 to 0.5): " + turnOffset.ToString("0.00"));
		turnOffset = GUILayout.HorizontalSlider(turnOffset, -0.5f, 0.5f, GUILayout.Width(100f));
		if (!(Mathf.Abs(turnOffset) >= 0.05f))
		{
			turnOffset = 0f;
		}
	}

	public virtual void Main()
	{
	}
}
