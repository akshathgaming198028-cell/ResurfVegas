using SYBO.Subway.Pickups;
using UnityEngine;

namespace SYBO.Subway.Coins
{
	[RequireComponent(typeof(Coin2))]
	[RequireComponent(typeof(PickupRotate))]
	public class CoinRenderer : MonoBehaviour
	{
		[SerializeField]
		private Renderer _renderer;

		[SerializeField]
		private Glow _glow;

		private PickupRotate _pickupRotate;

		public bool RenderersEnabled { get; private set; }

		public void ToggleRenderers(bool enabled)
		{
			_renderer.enabled = enabled;
			_glow.SetVisible(enabled);
			RenderersEnabled = enabled;
		}
	}
}
