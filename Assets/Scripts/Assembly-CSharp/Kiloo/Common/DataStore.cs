using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Kiloo.Common
{
	public class DataStore<TEnum> where TEnum : struct
	{
		public abstract class BaseValue
		{
			public const byte TYPE_BOOL = 0;

			public const byte TYPE_BOOL_ARRAY = 1;

			public const byte TYPE_FLOAT = 2;

			public const byte TYPE_FLOAT_ARRAY = 3;

			public const byte TYPE_ENUM = 4;

			public const byte TYPE_ENUM_ARRAY = 5;

			public const byte TYPE_INT = 6;

			public const byte TYPE_INT_ARRAY = 7;

			public const byte TYPE_STRING = 8;

			public const byte TYPE_STRING_ARRAY = 9;

			public const byte TYPE_DATETIME = 10;

			public TEnum key;

			public Action<TEnum> onValueChanged;

			public abstract void WriteTo(BinaryWriter writer);

			public abstract void SetValueFrom(BaseValue other);

			public abstract object GetValue();

			public abstract void SetValue(object value);

			public static BaseValue ReadFrom(BinaryReader reader)
			{
				string value = reader.ReadString();
				TEnum? val = null;
				try
				{
					val = (TEnum)Enum.Parse(typeof(TEnum), value);
				}
				catch (Exception)
				{
				}
				switch (reader.ReadByte())
				{
				case 0:
				{
					bool value4 = reader.ReadBoolean();
					if (val.HasValue)
					{
						return new Value<bool>(val.Value, value4);
					}
					break;
				}
				case 2:
				{
					float value6 = reader.ReadSingle();
					if (val.HasValue)
					{
						return new Value<float>(val.Value, value6);
					}
					break;
				}
				case 6:
				{
					int value3 = reader.ReadInt32();
					if (val.HasValue)
					{
						return new Value<int>(val.Value, value3);
					}
					break;
				}
				case 8:
				{
					string value5 = reader.ReadString();
					if (val.HasValue)
					{
						return new Value<string>(val.Value, value5);
					}
					break;
				}
				case 10:
				{
					long ticks = (long)reader.ReadUInt64();
					if (val.HasValue)
					{
						return new Value<DateTime>(val.Value, new DateTime(ticks));
					}
					break;
				}
				case 4:
				{
					value = reader.ReadString();
					string text2 = reader.ReadString();
					if (!val.HasValue)
					{
						break;
					}
					Type type3 = Type.GetType(value);
					object obj = null;
					try
					{
						if (type3 != null)
						{
							obj = Enum.Parse(type3, text2);
						}
					}
					catch (Exception ex3)
					{
						DataStore<TEnum>.LogError("Failed to parse enum value: " + value + " = " + text2 + ": " + ex3);
						return null;
					}
					Type genericTypeDefinition2 = typeof(Value<bool>).GetGenericTypeDefinition();
					if (genericTypeDefinition2 != null)
					{
						Type type4 = genericTypeDefinition2.MakeGenericType(typeof(TEnum), type3);
						return (BaseValue)Activator.CreateInstance(type4, val.Value, obj);
					}
					break;
				}
				case 1:
				{
					int num2 = reader.ReadInt32();
					bool[] array3 = new bool[num2];
					for (int k = 0; k < num2; k++)
					{
						array3[k] = reader.ReadBoolean();
					}
					if (val.HasValue)
					{
						return new Value<bool[]>(val.Value, array3);
					}
					break;
				}
				case 3:
				{
					int num4 = reader.ReadInt32();
					float[] array5 = new float[num4];
					for (int m = 0; m < num4; m++)
					{
						array5[m] = reader.ReadSingle();
					}
					if (val.HasValue)
					{
						return new Value<float[]>(val.Value, array5);
					}
					break;
				}
				case 7:
				{
					int num3 = reader.ReadInt32();
					int[] array4 = new int[num3];
					for (int l = 0; l < num3; l++)
					{
						array4[l] = reader.ReadInt32();
					}
					if (val.HasValue)
					{
						return new Value<int[]>(val.Value, array4);
					}
					break;
				}
				case 9:
				{
					int num5 = reader.ReadInt32();
					string[] array6 = new string[num5];
					for (int n = 0; n < num5; n++)
					{
						array6[n] = reader.ReadString();
					}
					if (val.HasValue)
					{
						return new Value<string[]>(val.Value, array6);
					}
					break;
				}
				case 5:
				{
					string text = reader.ReadString();
					int num = reader.ReadInt32();
					string[] array = new string[num];
					for (int i = 0; i < num; i++)
					{
						array[i] = reader.ReadString();
					}
					if (!val.HasValue)
					{
						break;
					}
					Type type = Type.GetType(text);
					if (type == null)
					{
						break;
					}
					Array array2 = Array.CreateInstance(type, num);
					for (int j = 0; j < num; j++)
					{
						object value2;
						try
						{
							value2 = Enum.Parse(type, array[j]);
						}
						catch (Exception ex2)
						{
							DataStore<TEnum>.LogError("Failed to parse enum type: " + text + "\n" + ex2);
							return null;
						}
						array2.SetValue(value2, j);
					}
					Type genericTypeDefinition = typeof(Value<bool>).GetGenericTypeDefinition();
					if (genericTypeDefinition != null)
					{
						Type type2 = genericTypeDefinition.MakeGenericType(typeof(TEnum), array2.GetType());
						return (BaseValue)Activator.CreateInstance(type2, val.Value, array2);
					}
					break;
				}
				default:
					throw new Exception("Settings.Value.ReadFrom: Invalid type field encountered");
				}
				return null;
			}
		}

		public class Value<T> : BaseValue
		{
			public T value;

			public Value()
			{
			}

			public Value(Value<T> copyFrom)
				: this(copyFrom.key, copyFrom.value)
			{
			}

			public Value(TEnum key, T value)
			{
				Type type = value.GetType();
				if (type == typeof(bool) || type == typeof(bool[]) || type == typeof(int) || type == typeof(int[]) || type == typeof(string) || type == typeof(string[]) || type == typeof(float) || type == typeof(float[]) || type == typeof(DateTime) || type.IsEnum || (type.IsArray && type.GetElementType().IsEnum))
				{
					base.key = key;
					ICloneable cloneable = value as ICloneable;
					if (cloneable != null)
					{
						this.value = (T)cloneable.Clone();
					}
					else
					{
						this.value = value;
					}
					return;
				}
				throw new ArgumentException("Value has unsupported type");
			}

			public void SetValueFrom(Value<T> other)
			{
				if (!key.Equals(other.key))
				{
					throw new ArgumentException();
				}
				Type type = value.GetType();
				if (type != other.value.GetType())
				{
					throw new ArgumentException();
				}
				if (type.IsArray)
				{
					Array array = (Array)(object)other.value;
					Array array2 = (Array)(object)value;
					Array.Copy(array, array2, (array.Length >= array2.Length) ? array2.Length : array.Length);
				}
				else if (!value.Equals(other.value))
				{
					ICloneable cloneable = other.value as ICloneable;
					if (cloneable != null)
					{
						value = (T)cloneable.Clone();
					}
					else
					{
						value = other.value;
					}
				}
			}

			public override void SetValueFrom(BaseValue other)
			{
				SetValueFrom((Value<T>)other);
			}

			public override object GetValue()
			{
				return value;
			}

			public override void SetValue(object v)
			{
				if (v is T)
				{
					value = (T)v;
					return;
				}
				throw new ArgumentException("value has wrong type");
			}

			public override bool Equals(object obj)
			{
				if (obj is Value<T>)
				{
					Value<T> value = (Value<T>)obj;
					return key.Equals(value.key) && this.value.Equals(value.value);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return key.GetHashCode() + value.GetHashCode();
			}

			public override string ToString()
			{
				return string.Format("Value<{0}>({1}, {2})", value.GetType(), key, value);
			}

			public override void WriteTo(BinaryWriter writer)
			{
				writer.Write(key.ToString());
				Type type = value.GetType();
				if (type.IsArray)
				{
					Type elementType = type.GetElementType();
					if (elementType == typeof(bool))
					{
						writer.Write((byte)1);
						bool[] array = (bool[])(object)value;
						writer.Write(array.Length);
						bool[] array2 = array;
						foreach (bool flag in array2)
						{
							writer.Write(flag);
						}
					}
					else if (elementType == typeof(float))
					{
						writer.Write((byte)3);
						float[] array3 = (float[])(object)value;
						writer.Write(array3.Length);
						float[] array4 = array3;
						foreach (float num in array4)
						{
							writer.Write(num);
						}
					}
					else if (elementType == typeof(int))
					{
						writer.Write((byte)7);
						int[] array5 = (int[])(object)value;
						writer.Write(array5.Length);
						int[] array6 = array5;
						foreach (int num2 in array6)
						{
							writer.Write(num2);
						}
					}
					else if (elementType == typeof(string))
					{
						writer.Write((byte)9);
						string[] array7 = (string[])(object)value;
						writer.Write(array7.Length);
						string[] array8 = array7;
						foreach (string text in array8)
						{
							writer.Write(text);
						}
					}
					else
					{
						if (!elementType.IsEnum)
						{
							return;
						}
						Array array9 = (Array)(object)value;
						writer.Write((byte)5);
						writer.Write(elementType.FullName);
						writer.Write(array9.Length);
						foreach (object item in array9)
						{
							writer.Write(item.ToString());
						}
					}
				}
				else if (type == typeof(bool))
				{
					writer.Write((byte)0);
					writer.Write((bool)(object)value);
				}
				else if (type == typeof(float))
				{
					writer.Write((byte)2);
					writer.Write((float)(object)value);
				}
				else if (type == typeof(int))
				{
					writer.Write((byte)6);
					writer.Write((int)(object)value);
				}
				else if (type == typeof(string))
				{
					writer.Write((byte)8);
					writer.Write((string)(object)value);
				}
				else if (type.IsEnum)
				{
					writer.Write((byte)4);
					writer.Write(type.FullName);
					writer.Write(value.ToString());
				}
				else if (type == typeof(DateTime))
				{
					writer.Write((byte)10);
					writer.Write(((DateTime)(object)value).Ticks);
				}
			}
		}

		public delegate int Convert(TEnum e);

		private const string BASE_SIGNATURE = "DS1.";

		private readonly BaseValue[] _defaults;

		private BaseValue[] _values;

		private bool _dirty;

		private readonly Convert _convert;

		public string signature
		{
			get
			{
				return "DS1." + typeof(TEnum).FullName;
			}
		}

		public bool isDirty
		{
			get
			{
				return _dirty;
			}
		}

		public DataStore(BaseValue[] defaults, Convert convert)
		{
			if (convert == null)
			{
				convert = DefaultConvert;
			}
			if (!ValidateValues(defaults, convert))
			{
				throw new ArgumentException("DataStore: Default settings are malformed");
			}
			_defaults = defaults;
			_convert = convert;
			ApplyValues(new BaseValue[0]);
		}

		static DataStore()
		{
			if (!typeof(TEnum).IsEnum)
			{
				throw new Exception("DataStore: Generic type TEnum must be an enum type");
			}
		}

		private int DefaultConvert(TEnum e)
		{
			return (int)(object)e;
		}

		public void ClearDirty()
		{
			_dirty = false;
		}

		public T GetValue<T>(TEnum key)
		{
			Value<T> value = (Value<T>)_values[_convert(key)];
			return value.value;
		}

		public T GetArrayValue<T>(TEnum key, int index)
		{
			Value<T[]> value = (Value<T[]>)_values[_convert(key)];
			return value.value[index];
		}

		public void SetValue<T>(TEnum key, T value)
		{
			Value<T> value2 = (Value<T>)_values[_convert(key)];
			if (!value2.value.Equals(value))
			{
				value2.value = value;
				_dirty = true;
				Action<TEnum> onValueChanged = value2.onValueChanged;
				if (onValueChanged != null)
				{
					onValueChanged(key);
				}
			}
		}

		public void SetArrayValue<T>(TEnum key, int index, T value)
		{
			Value<T[]> value2 = (Value<T[]>)_values[_convert(key)];
			T[] value3 = value2.value;
			if (!value3[index].Equals(value))
			{
				value3[index] = value;
				_dirty = true;
				Action<TEnum> onValueChanged = value2.onValueChanged;
				if (onValueChanged != null)
				{
					onValueChanged(key);
				}
			}
		}

		public bool Toggle(TEnum key)
		{
			bool flag = !GetValue<bool>(key);
			SetValue(key, flag);
			return flag;
		}

		public void AddChangedHandler(TEnum key, Action<TEnum> handler)
		{
			BaseValue obj = _values[_convert(key)];
			obj.onValueChanged = (Action<TEnum>)Delegate.Combine(obj.onValueChanged, handler);
		}

		public void RemoveChangedHandler(TEnum key, Action<TEnum> handler)
		{
			BaseValue obj = _values[_convert(key)];
			obj.onValueChanged = (Action<TEnum>)Delegate.Remove(obj.onValueChanged, handler);
		}

		public void WriteTo(Stream stream)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			binaryWriter.Write(signature);
			binaryWriter.Write(_values.Length);
			BaseValue[] values = _values;
			foreach (BaseValue baseValue in values)
			{
				baseValue.WriteTo(binaryWriter);
			}
		}

		public void ReadFrom(Stream stream)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			string text = binaryReader.ReadString();
			if (text != signature)
			{
				throw new IOException("Wrong signature");
			}
			int num = binaryReader.ReadInt32();
			BaseValue[] array = new BaseValue[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = BaseValue.ReadFrom(binaryReader);
			}
			ApplyValues(array);
		}

		public static DataStore<TEnum> ReadFrom(Stream stream, BaseValue[] defaults, Convert convert)
		{
			DataStore<TEnum> dataStore = new DataStore<TEnum>(defaults, convert);
			dataStore.ReadFrom(stream);
			dataStore.ClearDirty();
			return dataStore;
		}

		public void ApplyValues(BaseValue[] newValues)
		{
			if (_values == null)
			{
				_values = new BaseValue[Enum.GetValues(typeof(TEnum)).Length];
				for (int i = 0; i < _values.Length; i++)
				{
					_values[i] = (BaseValue)Activator.CreateInstance(_defaults[i].GetType(), _defaults[i]);
				}
			}
			foreach (BaseValue baseValue in newValues)
			{
				if (baseValue == null)
				{
					continue;
				}
				int num = _convert(baseValue.key);
				if (!baseValue.Equals(_values[num]))
				{
					_values[num].SetValueFrom(baseValue);
					Action<TEnum> onValueChanged = _values[num].onValueChanged;
					if (onValueChanged != null)
					{
						onValueChanged(baseValue.key);
					}
				}
			}
		}

		private static bool ValidateValues(BaseValue[] values, Convert convert)
		{
			if (values != null)
			{
				TEnum[] array = (TEnum[])Enum.GetValues(typeof(TEnum));
				if (array.Length == values.Length)
				{
					for (int i = 0; i < values.Length; i++)
					{
						BaseValue baseValue = values[i];
						if (baseValue != null)
						{
							if (i != convert(baseValue.key))
							{
								LogError(string.Concat("ValidateValues: ", baseValue.key, " is at incorrect index in values: ", i, ", should be ", convert(baseValue.key)));
								return false;
							}
							continue;
						}
						LogError("ValidateValues: Value at index " + i + " is null");
						return false;
					}
					return true;
				}
				LogError("ValidateValues: Wrong number of settings");
				return false;
			}
			LogError("ValidateValues: values is null");
			return false;
		}

		[Conditional("UNITY_EDITOR")]
		private static void Assert(bool condition, string msg)
		{
			if (!condition)
			{
				throw new Exception(msg);
			}
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[DataStore] " + msg);
		}
	}
}
