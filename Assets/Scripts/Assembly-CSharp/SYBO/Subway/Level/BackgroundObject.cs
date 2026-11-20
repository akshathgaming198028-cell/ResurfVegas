using UnityEngine;

namespace SYBO.Subway.Level
{
	public class BackgroundObject : MonoBehaviour
	{
		[SerializeField]
		private bool _alwaysVisible;

		[SerializeField]
		private bool _movable;

		public bool AlwaysVisible
		{
			get
			{
				return _alwaysVisible;
			}
		}

		public bool Movable
		{
			get
			{
				return _movable;
			}
		}
	}
}
