using System;
using UnityEngine;

namespace Kiloo.Monetization.Offerwall.TapjoyOfferwall
{
	public class TapjoyOffer : IDisplayableOffer
	{
		public enum TapjoyOfferType
		{
			None = 0,
			ActionOffer = 1,
			Announcement = 2,
			App = 3,
			DeeplinkOffer = 4,
			EmailOffer = 5,
			GenericOffer = 6,
			MraidOffer = 7,
			OfferpalOffer = 8,
			RatingOffer = 9,
			ReengagementOffer = 10,
			SurveyOffer = 11,
			VideoOffer = 12
		}

		private string _name = string.Empty;

		private string _amount = string.Empty;

		private string _cost = string.Empty;

		private string _payout = string.Empty;

		private string _iconURL = string.Empty;

		private string _iconURLMedium = string.Empty;

		private string _redirectURL = string.Empty;

		private string _storeID = string.Empty;

		private string _type = string.Empty;

		private Texture2D _texture;

		public string title
		{
			get
			{
				return _name;
			}
		}

		public string payoutAmount
		{
			get
			{
				return _amount;
			}
		}

		public string iconToDisplayURL
		{
			get
			{
				return _iconURLMedium;
			}
		}

		public string actionURL
		{
			get
			{
				return _redirectURL;
			}
		}

		public string costForInstall
		{
			get
			{
				return _cost;
			}
		}

		public string offerType
		{
			get
			{
				return _type;
			}
		}

		public Texture2D texture
		{
			get
			{
				return _texture;
			}
			set
			{
				if (_texture != value)
				{
					_texture = value;
					if (this.textureChanged != null)
					{
						this.textureChanged();
					}
				}
			}
		}

		public event Action textureChanged;

		public TapjoyOffer(string name, string amount, string cost, string payout, string iconURL, string iconURLMedium, string redirectURL, string storeID, string type)
		{
			_name = name;
			_amount = amount;
			_cost = cost;
			_payout = payout;
			_iconURL = iconURL;
			_iconURLMedium = iconURLMedium;
			_redirectURL = redirectURL;
			_storeID = storeID;
			_type = type;
		}

		public string FormatToString()
		{
			return string.Format("Tapjoy Offer content: name= {0}, cost= {1}, amount= {2}, payout= {3}, redirectURL= {4}, iconURL= {5}, iconURLMedium= {6}, storeID= {7}, type= {8}", _name, _cost, _amount, _payout, _redirectURL, _iconURL, _iconURLMedium, _storeID, _type);
		}
	}
}
