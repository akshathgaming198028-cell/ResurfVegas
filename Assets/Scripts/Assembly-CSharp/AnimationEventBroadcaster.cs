using UnityEngine;

public class AnimationEventBroadcaster : MonoBehaviour
{
	public delegate void SendDoKeyframeAnimationDelegate(int soundIndex);

	public delegate void SendPlayKeyframeAnimationDelegate(int soundIndex);

	public delegate void SendSetAnimationSpeedEventDelegate(AnimationEvent animEvent);

	public event SendDoKeyframeAnimationDelegate SendDoKeyframeAnimation;

	public event SendPlayKeyframeAnimationDelegate SendPlayKeyframeAnimation;

	public event SendSetAnimationSpeedEventDelegate SendSetAnimationSpeedEvent;

	private void DoKeyframeAnimation(int soundIndex)
	{
		if (this.SendDoKeyframeAnimation != null)
		{
			this.SendDoKeyframeAnimation(soundIndex);
		}
	}

	private void PlayKeyframeAnimation(int soundIndex)
	{
		if (this.SendPlayKeyframeAnimation != null)
		{
			this.SendPlayKeyframeAnimation(soundIndex);
		}
	}

	private void SetAnimationSpeedEvent(AnimationEvent animEvent)
	{
		if (this.SendSetAnimationSpeedEvent != null)
		{
			this.SendSetAnimationSpeedEvent(animEvent);
		}
	}
}
