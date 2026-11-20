using System;
using UnityEngine;

[IntegrationTest.Timeout(1f)]
[IntegrationTest.ExcludePlatform(new RuntimePlatform[]
{
	RuntimePlatform.Android,
	RuntimePlatform.LinuxPlayer
})]
[IntegrationTest.ExpectExceptions(false, new Type[] { typeof(ArgumentException) })]
[IntegrationTest.SucceedWithAssertions]
[IntegrationTest.DynamicTest("ExampleIntegrationTests")]
public class DynamicIntegrationTest : MonoBehaviour
{
	public void Start()
	{
		IntegrationTest.Pass(base.gameObject);
	}
}
