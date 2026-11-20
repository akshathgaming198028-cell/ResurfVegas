using System;
using UnityEngine;

public class ThrowCustomException : MonoBehaviour
{
	private class CustomException : Exception
	{
	}

	public void Start()
	{
		throw new CustomException();
	}
}
