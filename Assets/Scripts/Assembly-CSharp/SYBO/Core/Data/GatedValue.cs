using System;
using SYBO.Core.Utilities;
using UnityEngine;

namespace SYBO.Core.Data
{
	public class GatedValue
	{
		private float _minimumValue;

		private float _maximumValue;

		private float _value;

		private float[] _gateValues;

		public float MinimumValue
		{
			get
			{
				return _minimumValue;
			}
		}

		public float MaximumValue
		{
			get
			{
				return _maximumValue;
			}
		}

		public float Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				_value = Mathf.Min(_value, _maximumValue);
				_value = Mathf.Max(_value, _minimumValue);
			}
		}

		public float[] GateValues
		{
			get
			{
				return _gateValues;
			}
			set
			{
				_gateValues = value;
				int num = EnumUtility.ConvertEnumToIntArray<Gate>().Length - 3;
				if (num != _gateValues.Length)
				{
					throw new Exception("GatedValue.GatedValue() gatePercentages.Length of '" + _gateValues.Length + "' must be '" + num + "'");
				}
				Array.Sort(_gateValues);
			}
		}

		public Gate Gate
		{
			get
			{
				if (_value <= _minimumValue)
				{
					return Gate.Empty;
				}
				if (_value >= _maximumValue)
				{
					return Gate.Full;
				}
				for (int i = 0; i < _gateValues.Length; i++)
				{
					if (_value < _gateValues[i])
					{
						return EnumUtility.ConvertIndexToEnumValue<Gate>(i + 1);
					}
				}
				return Gate.High;
			}
		}

		public GatedValue(float minimumValue, float maximumValue, float defaultValue, float[] gateValues)
		{
			_minimumValue = minimumValue;
			_maximumValue = maximumValue;
			_value = defaultValue;
			GateValues = gateValues;
		}
	}
}
