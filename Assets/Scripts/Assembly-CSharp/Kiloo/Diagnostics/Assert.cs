using System;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Diagnostics
{
	public static class Assert
	{
		public class AssertFailedException : Exception
		{
		}

		private static void FailAssert()
		{
			Debug.Break();
			throw new AssertFailedException();
		}

		[ContractAnnotation("halt <= obj: null")]
		[Conditional("ASSERTS_ENABLED")]
		public static void IsNotNull(object obj, string paramName, UnityEngine.Object context)
		{
			if (obj == null)
			{
				Debug.LogError("Assert failed: " + paramName + " is null!", context);
				FailAssert();
			}
		}

		[Conditional("ASSERTS_ENABLED")]
		[ContractAnnotation("halt <= obj: null")]
		public static void IsNotNull(object obj, string paramName)
		{
			if (obj == null)
			{
				Debug.LogError("Assert failed: " + paramName + " is null!");
				FailAssert();
			}
		}

		[Conditional("ASSERTS_ENABLED")]
		[ContractAnnotation("halt <= condition: false")]
		public static void IsTrue(bool condition, string message, UnityEngine.Object context)
		{
			if (!condition)
			{
				Debug.LogError("Assert failed: " + message, context);
				FailAssert();
			}
		}

		[ContractAnnotation("halt <= condition: true")]
		[Conditional("ASSERTS_ENABLED")]
		public static void IsFalse(bool condition, string message, UnityEngine.Object context)
		{
			if (condition)
			{
				Debug.LogError("Assert failed: " + message, context);
				FailAssert();
			}
		}

		[ContractAnnotation("halt <= condition: false")]
		[Conditional("ASSERTS_ENABLED")]
		public static void IsTrue(bool condition)
		{
			if (!condition)
			{
				Debug.LogError("Assert failed!");
				FailAssert();
			}
		}

		[ContractAnnotation("halt <= condition: true")]
		[Conditional("ASSERTS_ENABLED")]
		public static void IsFalse(bool condition)
		{
			if (condition)
			{
				Debug.LogError("Assert failed!");
				FailAssert();
			}
		}

		[ContractAnnotation("halt <= condition: false")]
		[Conditional("ASSERTS_ENABLED")]
		public static void IsTrue(bool condition, string message)
		{
			if (!condition)
			{
				Debug.LogError("Assert failed: " + message);
				FailAssert();
			}
		}

		[ContractAnnotation("halt <= condition: true")]
		[Conditional("ASSERTS_ENABLED")]
		public static void IsFalse(bool condition, string message)
		{
			if (condition)
			{
				Debug.LogError("Assert failed: " + message);
				FailAssert();
			}
		}
	}
}
