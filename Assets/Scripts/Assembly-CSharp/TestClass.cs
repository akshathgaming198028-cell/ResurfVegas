using System;
using SYBO.Core.Utilities.Flags;
using UnityEngine;

public class TestClass : MonoBehaviour
{
	[Flags]
	public enum Fun
	{
		ONE = 0,
		TWO = 1
	}

	public string testString = "hello";

	[EnumFlagProperty]
	public Fun MyFun;

	private void Start()
	{
	}

	private void Update()
	{
	}
}
