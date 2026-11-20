using Kiloo.Plugins.DeviceUtility;
using UnityEngine;

[RequireComponent(typeof(UILabel))]
public class UIVersionLabel : MonoBehaviour
{
	private void Start()
	{
		if (!DeviceUtility.isFlexionDevice)
		{
			GetComponent<UILabel>().text = "v" + DeviceUtility.GetBundleVersion();
		}
		else
		{
			GetComponent<UILabel>().text = "v" + DeviceUtility.GetBundleVersion() + "f";
		}
	}
}
