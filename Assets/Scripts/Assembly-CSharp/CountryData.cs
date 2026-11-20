using System.Collections.Generic;

public static class CountryData
{
	private static List<Country> listOfCountries;

	public static Country GetMotherCountryForCountryID(string countryID)
	{
		switch (countryID)
		{
		case "as":
			return new Country("United States", "United-States", "us");
		case "ai":
			return new Country("United Kingdom", "United-Kingdom", "gb");
		case "aw":
			return new Country("Netherlands", "Netherlands", "nl");
		case "bm":
			return new Country("United Kingdom", "United-Kingdom", "gb");
		case "ky":
			return new Country("United Kingdom", "United-Kingdom", "gb");
		case "ck":
			return new Country("New Zealand", "New-Zealand", "nz");
		case "fk":
			return new Country("United Kingdom", "United-Kingdom", "gb");
		case "fo":
			return new Country("Denmark", "Denmark", "dk");
		case "gf":
			return new Country("France", "France", "fr");
		case "pf":
			return new Country("France", "France", "fr");
		case "gi":
			return new Country("United Kingdom", "United-Kingdom", "gb");
		case "gl":
			return new Country("Denmark", "Denmark", "dk");
		case "gp":
			return new Country("France", "France", "fr");
		case "gu":
			return new Country("United States", "United-States", "us");
		case "va":
			return new Country("Italy", "Italy", "it");
		case "mo":
			return new Country("China", "China", "cn");
		case "mq":
			return new Country("France", "France", "fr");
		case "yt":
			return new Country("France", "France", "fr");
		case "mc":
			return new Country("France", "France", "fr");
		case "ms":
			return new Country("United Kingdom", "United-Kingdom", "gb");
		case "an":
			return new Country("Netherlands", "Netherlands", "nl");
		case "nc":
			return new Country("France", "France", "fr");
		case "nu":
			return new Country("New Zealand", "New-Zealand", "nz");
		case "mp":
			return new Country("United States", "United-States", "us");
		case "pr":
			return new Country("United States", "United-States", "us");
		case "re":
			return new Country("France", "France", "fr");
		case "sh":
			return new Country("United Kingdom", "United-Kingdom", "gb");
		case "pm":
			return new Country("France", "France", "fr");
		case "sm":
			return new Country("Italy", "Italy", "it");
		case "sx":
			return new Country("Netherlands", "Netherlands", "nl");
		case "tk":
			return new Country("New Zealand", "New-Zealand", "nz");
		case "tc":
			return new Country("United Kingdom", "United-Kingdom", "gb");
		case "vg":
			return new Country("United Kingdom", "United-Kingdom", "gb");
		case "vi":
			return new Country("United States", "United-States", "us");
		case "wf":
			return new Country("France", "France", "fr");
		default:
			return null;
		}
	}

	public static List<Country> getListOfCountries()
	{
		if (listOfCountries == null)
		{
			List<Country> list = new List<Country>();
			list.Add(new Country("Afghanistan", "Afghanistan", "af"));
			list.Add(new Country("Albania", "Albania", "al"));
			list.Add(new Country("Algeria", "Algeria", "dz"));
			list.Add(new Country("Angola", "Angola", "ao"));
			list.Add(new Country("Antigua & Barbuda", "Antigua-and-Barbuda", "ag"));
			list.Add(new Country("Argentina", "Argentina", "ar"));
			list.Add(new Country("Armenia", "Armenia", "am"));
			list.Add(new Country("Australia", "Australia", "au"));
			list.Add(new Country("Austria", "Austria", "at"));
			list.Add(new Country("Azerbaijan", "Azerbaijan", "az"));
			list.Add(new Country("Bahamas", "Bahamas", "bs"));
			list.Add(new Country("Bahrain", "Bahrain", "bh"));
			list.Add(new Country("Bangladesh", "Bangladesh", "bd"));
			list.Add(new Country("Barbados", "Barbados", "bb"));
			list.Add(new Country("Belarus", "Belarus", "by"));
			list.Add(new Country("Belgium", "Belgium", "be"));
			list.Add(new Country("Belize", "Belize", "bz"));
			list.Add(new Country("Benin", "Benin", "bj"));
			list.Add(new Country("Bolivia", "Bolivia", "bo"));
			list.Add(new Country("Bosnia & Herzegovina", "Bosnia-and-Herzegovina", "ba"));
			list.Add(new Country("Brazil", "Brazil", "br"));
			list.Add(new Country("Brunei", "Brunei", "bn"));
			list.Add(new Country("Bulgaria", "Bulgaria", "bg"));
			list.Add(new Country("Burkina Faso", "Burkina-Faso", "bf"));
			list.Add(new Country("Cambodia", "Cambodia", "kh"));
			list.Add(new Country("Canada", "Canada", "ca"));
			list.Add(new Country("Central African Republic", "Central-African-Republic", "cf"));
			list.Add(new Country("Chile", "Chile", "cl"));
			list.Add(new Country("China", "China", "cn"));
			list.Add(new Country("Colombia", "Colombia", "co"));
			list.Add(new Country("Costa Rica", "Costa-Rica", "cr"));
			list.Add(new Country("Côte d'Ivoire", "Cote-d'Ivoire", "ci"));
			list.Add(new Country("Croatia", "Croatia", "hr"));
			list.Add(new Country("Cuba", "Cuba", "cu"));
			list.Add(new Country("Cyprus", "Cyprus", "cy"));
			list.Add(new Country("Czech Republic", "Czech-Republic", "cz"));
			list.Add(new Country("Denmark", "Denmark", "dk"));
			list.Add(new Country("Dominican Republic", "Dominican-Republic", "do"));
			list.Add(new Country("Ecuador", "Ecuador", "ec"));
			list.Add(new Country("Egypt", "Egypt", "eg"));
			list.Add(new Country("El Salvador", "El-Salvador", "sv"));
			list.Add(new Country("Eritrea", "Eritrea", "er"));
			list.Add(new Country("Estonia", "Estonia", "ee"));
			list.Add(new Country("Ethiopia", "Ethiopia", "et"));
			list.Add(new Country("Fiji", "Fiji", "fj"));
			list.Add(new Country("Finland", "Finland", "fi"));
			list.Add(new Country("France", "France", "fr"));
			list.Add(new Country("Georgia", "Georgia", "ge"));
			list.Add(new Country("Germany", "Germany", "de"));
			list.Add(new Country("Ghana", "Ghana", "gh"));
			list.Add(new Country("Greece", "Greece", "gr"));
			list.Add(new Country("Guatemala", "Guatemala", "gt"));
			list.Add(new Country("Guyana", "Guyana", "gy"));
			list.Add(new Country("Haiti", "Haiti", "ht"));
			list.Add(new Country("Honduras", "Honduras", "hn"));
			list.Add(new Country("Hong Kong", "Hong-Kong", "hk"));
			list.Add(new Country("Hungary", "Hungary", "hu"));
			list.Add(new Country("Iceland", "Iceland", "is"));
			list.Add(new Country("India", "India", "in"));
			list.Add(new Country("Indonesia", "Indonesia", "id"));
			list.Add(new Country("Iran", "Iran", "ir"));
			list.Add(new Country("Iraq", "Iraq", "iq"));
			list.Add(new Country("Ireland", "Ireland", "ie"));
			list.Add(new Country("Israel", "Israel", "il"));
			list.Add(new Country("Italy", "Italy", "it"));
			list.Add(new Country("Jamaica", "Jamaica", "jm"));
			list.Add(new Country("Japan", "Japan", "jp"));
			list.Add(new Country("Jordan", "Jordan", "jo"));
			list.Add(new Country("Kazakhstan", "Kazakhstan", "kz"));
			list.Add(new Country("Kenya", "Kenya", "ke"));
			list.Add(new Country("Kiribati", "Kiribati", "ki"));
			list.Add(new Country("Kuwait", "Kuwait", "kw"));
			list.Add(new Country("Kyrgyzstan", "Kyrgyzstan", "kg"));
			list.Add(new Country("Laos", "Laos", "la"));
			list.Add(new Country("Latvia", "Latvia", "lv"));
			list.Add(new Country("Lebanon", "Lebanon", "lb"));
			list.Add(new Country("Libya", "Libya", "ly"));
			list.Add(new Country("Liechtenstein", "Liechtenstein", "li"));
			list.Add(new Country("Lithuania", "Lithuania", "lt"));
			list.Add(new Country("Luxembourg", "Luxembourg", "lu"));
			list.Add(new Country("Macedonia", "Macedonia", "mk"));
			list.Add(new Country("Malaysia", "Malaysia", "my"));
			list.Add(new Country("Maldives", "Maldives", "mv"));
			list.Add(new Country("Mali", "Mali", "ml"));
			list.Add(new Country("Malta", "Malta", "mt"));
			list.Add(new Country("Mauritania", "Mauritania", "mr"));
			list.Add(new Country("Mauritius", "Mauritius", "mu"));
			list.Add(new Country("Mexico", "Mexico", "mx"));
			list.Add(new Country("Moldova", "Moldova", "md"));
			list.Add(new Country("Mongolia", "Mongolia", "mn"));
			list.Add(new Country("Montenegro", "Montenegro", "me"));
			list.Add(new Country("Morocco", "Morocco", "ma"));
			list.Add(new Country("Mozambique", "Mozambique", "mz"));
			list.Add(new Country("Myanmar", "Myanmar", "mm"));
			list.Add(new Country("Namibia", "Namibia", "na"));
			list.Add(new Country("Nauru", "Nauru", "nr"));
			list.Add(new Country("Nepal", "Nepal", "np"));
			list.Add(new Country("Netherlands", "Netherlands", "nl"));
			list.Add(new Country("New Zealand", "New-Zealand", "nz"));
			list.Add(new Country("Nicaragua", "Nicaragua", "ni"));
			list.Add(new Country("Nigeria", "Nigeria", "ng"));
			list.Add(new Country("North Korea", "North-Korea", "kp"));
			list.Add(new Country("Norway", "Norway", "no"));
			list.Add(new Country("Oman", "Oman", "om"));
			list.Add(new Country("Pakistan", "Pakistan", "pk"));
			list.Add(new Country("Palestine", "Palestine", "ps"));
			list.Add(new Country("Panama", "Panama", "pa"));
			list.Add(new Country("Paraguay", "Paraguay", "py"));
			list.Add(new Country("Peru", "Peru", "pe"));
			list.Add(new Country("Philippines", "Philippines", "ph"));
			list.Add(new Country("Poland", "Poland", "pl"));
			list.Add(new Country("Portugal", "Portugal", "pt"));
			list.Add(new Country("Qatar", "Qatar", "qa"));
			list.Add(new Country("Romania", "Romania", "ro"));
			list.Add(new Country("Russia", "Russia", "ru"));
			list.Add(new Country("Saint Kitts & Nevis", "Saint-Kitts-and-Nevis", "kn"));
			list.Add(new Country("Saint Lucia", "Saint-Lucia", "lc"));
			list.Add(new Country("St. Vincent & Grenadines", "Saint-Vincent-and-the-Grenadines", "vc"));
			list.Add(new Country("Saudi Arabia", "Saudi-Arabia", "sa"));
			list.Add(new Country("Senegal", "Senegal", "sn"));
			list.Add(new Country("Serbia", "Serbia", "rs"));
			list.Add(new Country("Singapore", "Singapore", "sg"));
			list.Add(new Country("Slovakia", "Slovakia", "sk"));
			list.Add(new Country("Slovenia", "Slovenia", "si"));
			list.Add(new Country("South Africa", "South-Africa", "za"));
			list.Add(new Country("South Korea", "South-Korea", "kr"));
			list.Add(new Country("South Sudan", "South-Sudan", "ss"));
			list.Add(new Country("Spain", "Spain", "es"));
			list.Add(new Country("Sri Lanka", "Sri-Lanka", "lk"));
			list.Add(new Country("Sudan", "Sudan", "sd"));
			list.Add(new Country("Suriname", "Suriname", "sr"));
			list.Add(new Country("Sweden", "Sweden", "se"));
			list.Add(new Country("Switzerland", "Switzerland", "ch"));
			list.Add(new Country("Syria", "Syria", "sy"));
			list.Add(new Country("Taiwan", "Taiwan", "tw"));
			list.Add(new Country("Tajikistan", "Tajikistan", "tj"));
			list.Add(new Country("Tanzania", "Tanzania", "tz"));
			list.Add(new Country("Thailand", "Thailand", "th"));
			list.Add(new Country("Trinidad & Tobago", "Trinidad-and-Tobago", "tt"));
			list.Add(new Country("Tunisia", "Tunisia", "tn"));
			list.Add(new Country("Turkey", "Turkey", "tr"));
			list.Add(new Country("Turkmenistan", "Turkmenistan", "tm"));
			list.Add(new Country("Tuvalu", "Tuvalu", "tv"));
			list.Add(new Country("Uganda", "Uganda", "ug"));
			list.Add(new Country("Ukraine", "Ukraine", "ua"));
			list.Add(new Country("United Arab Emirates", "United-Arab-Emirates", "ae"));
			list.Add(new Country("United Kingdom", "United-Kingdom", "gb"));
			list.Add(new Country("United States", "United-States", "us"));
			list.Add(new Country("Uruguay", "Uruguay", "uy"));
			list.Add(new Country("Uzbekistan", "Uzbekistan", "uz"));
			list.Add(new Country("Venezuela", "Venezuela", "ve"));
			list.Add(new Country("Vietnam", "Vietnam", "vn"));
			list.Add(new Country("Yemen", "Yemen", "ye"));
			list.Add(new Country("Zambia", "Zambia", "zm"));
			list.Add(new Country("Zimbabwe", "Zimbabwe", "zw"));
			listOfCountries = list;
		}
		return listOfCountries;
	}

	public static Country GetCountryForID(string id)
	{
		string text = id;
		if (!string.IsNullOrEmpty(text))
		{
			text = text.ToLower();
			foreach (Country listOfCountry in getListOfCountries())
			{
				if (listOfCountry.countryCode == text)
				{
					return listOfCountry;
				}
			}
		}
		return null;
	}
}
