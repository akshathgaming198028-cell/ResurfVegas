using System;
using UnityEngine;

namespace SYBO.Core.Utilities.Flags
{
	public class EnumFlagPropertyAttribute : PropertyAttribute
	{
		public string DisplayName;

		public Type ClassType;

		public EnumFlagPropertyAttribute()
		{
		}

		public EnumFlagPropertyAttribute(string name)
		{
			DisplayName = name;
		}

		public EnumFlagPropertyAttribute(Type classType)
		{
			ClassType = classType;
		}
	}
}
