using UnityEngine;

public class CharacterAnimationControl : MonoBehaviour
{
	[SerializeField]
	private AnimationClip _runAnimation;

	private Animation _animationComponent;

	private AnimationClip _currentAnimation;

	public void Play()
	{
		CharacterModel componentInParent = base.gameObject.GetComponentInParent<CharacterModel>();
		CharacterAssetsBundle componentInChildren = componentInParent.GetComponentInChildren<CharacterAssetsBundle>();
		AnimationClip runIdle = componentInChildren.RunIdle;
		_currentAnimation = ((!(runIdle != null)) ? _runAnimation : runIdle);
		if (_animationComponent == null)
		{
			_animationComponent = base.gameObject.GetComponentInChildren<Animation>();
		}
		if (!_animationComponent.GetClip(_currentAnimation.name))
		{
			_animationComponent.AddClip(_currentAnimation, _currentAnimation.name);
		}
		_animationComponent.Play(_currentAnimation.name);
	}

	public void Speed(float speed)
	{
		_animationComponent[_currentAnimation.name].speed = speed;
	}
}
