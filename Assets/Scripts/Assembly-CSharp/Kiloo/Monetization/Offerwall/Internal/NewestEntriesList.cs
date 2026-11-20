using System;
using System.Collections.Generic;
using System.Linq;

namespace Kiloo.Monetization.Offerwall.Internal
{
	public class NewestEntriesList<TValue> : List<TValue>
	{
		private Dictionary<TValue, DateTime> values = new Dictionary<TValue, DateTime>();

		public int MaxItems { get; set; }

		public new void Add(TValue value)
		{
			DateTime now = DateTime.Now;
			if (values.Count >= MaxItems && values.Count > 0)
			{
				KeyValuePair<TValue, DateTime> keyValuePair = values.OrderBy((KeyValuePair<TValue, DateTime> kvp) => kvp.Value).First();
				values.Remove(keyValuePair.Key);
			}
			values.Add(value, now);
			base.Add(value);
		}
	}
}
