using System;
using Kiloo.Plugins.DeviceUtility;
using UnityEngine;

[RequireComponent(typeof(UILabel))]
public class UIVersionLabel : MonoBehaviour
{
	private void Start()
	{
		string arch = IntPtr.Size == 8 ? "ARM v8a (64 bit)" : "ARM v7a (32 bit)";
		string flexion = DeviceUtility.isFlexionDevice ? "f" : "";
		GetComponent<UILabel>().text = "v" + DeviceUtility.GetBundleVersion() + flexion + " " + arch;
	}
}
