using System;

namespace SYBO.Core.Components.Singleton
{
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public class PrefabPathAttribute : Attribute
	{
		private string _name;

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public PrefabPathAttribute()
		{
			_name = string.Empty;
		}

		public PrefabPathAttribute(string name)
		{
			_name = name;
		}
	}
}
