using UnityEngine;

public class ScrollListPopupCellHelper : MonoBehaviour
{
	public UILabel Title;

	public GameObject OffIcon;

	public GameObject OnIcon;

	public string cellID;

	private bool currentIconState;

	public void UpdateTitle(string title)
	{
		Title.text = title;
	}

	public void SetToggleIcon(bool selected)
	{
		bool flag = selected;
		OffIcon.SetActive(!flag);
		OnIcon.SetActive(flag);
		currentIconState = flag;
	}

	public void ToggleIcon()
	{
		currentIconState = !currentIconState;
		OffIcon.SetActive(!currentIconState);
		OnIcon.SetActive(currentIconState);
	}
}
