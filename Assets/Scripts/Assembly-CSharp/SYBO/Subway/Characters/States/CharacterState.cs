using System.Collections;
using UnityEngine;

namespace SYBO.Subway.Characters.States
{
	public abstract class CharacterState : MonoBehaviour
	{
		public virtual bool PauseActiveModifiers
		{
			get
			{
				return false;
			}
		}

		public virtual IEnumerator Begin()
		{
			yield break;
		}

		public virtual void HandleCriticalHit()
		{
		}

		public virtual void HandleDoubleTap()
		{
		}

		public virtual void HandleSwipe(SwipeDir swipeDir)
		{
		}
	}
}
