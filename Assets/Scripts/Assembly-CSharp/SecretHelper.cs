using UnityEngine;

public class SecretHelper : MonoBehaviour
{
	public UILabel SecretValue;

	public UILabel SecretInfo;

	public UILabel SecretTitle;

	public void UpdateInfo(string info, string value)
	{
		SecretInfo.text = info;
		SecretValue.text = value;
	}

	public void UpdateTitle(string title)
	{
		SecretTitle.text = title;
	}
}
