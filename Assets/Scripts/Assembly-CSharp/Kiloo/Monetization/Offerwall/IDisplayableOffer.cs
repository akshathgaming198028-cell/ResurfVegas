using System;
using UnityEngine;

namespace Kiloo.Monetization.Offerwall
{
	public interface IDisplayableOffer
	{
		string title { get; }

		string payoutAmount { get; }

		string iconToDisplayURL { get; }

		string actionURL { get; }

		Texture2D texture { get; set; }

		string costForInstall { get; }

		string offerType { get; }

		event Action textureChanged;
	}
}
