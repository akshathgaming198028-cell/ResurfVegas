using SYBO.Subway;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditorLoadFromLazyLoadScene : MonoBehaviour
{
	private void Start()
	{
		if (!Game.HasLoaded)
		{
			Debug.Log("LoadLevel Level Loades " + Time.frameCount);
			SceneManager.LoadScene("LoadScene");
		}
	}

	private void Update()
	{
	}
}
