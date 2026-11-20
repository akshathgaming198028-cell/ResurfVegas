using System;
using UnityEngine;

namespace SYBO.Core.UnityIssueTracker
{
	public class UnityIssueTracker_LegacyAnimationScale : MonoBehaviour
	{
		[SerializeField]
		private Animation _testAnimation;

		protected void Awake()
		{
			if (_testAnimation == null)
			{
				throw new Exception("Null Not Ok: " + _testAnimation);
			}
		}

		protected void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				PlayAnimationName("down");
			}
			if (Input.GetMouseButtonUp(0))
			{
				PlayAnimationName("up");
			}
		}

		private void PlayAnimationName(string animationName)
		{
			Debug.Log("UnityIssueTracker_LegacyAnimationScale.PlayAnimationName(" + animationName + ")");
			_testAnimation.Play(animationName);
		}
	}
}
