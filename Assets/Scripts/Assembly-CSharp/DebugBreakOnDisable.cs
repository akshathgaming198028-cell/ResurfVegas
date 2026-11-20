using UnityEngine;

public class DebugBreakOnDisable : MonoBehaviour
{
	private void OnDisable()
	{
		Debug.Log("I was disabled!!");
		Debug.Break();
	}
}
