using System;

public class SecretCell
{
	public SecretCellType cellType;

	public string cellInfo;

	public string cellValue;

	public string cellTitle;

	public Action OnUpdateCell;

	public void UpdateCell()
	{
		if (OnUpdateCell != null)
		{
			OnUpdateCell();
		}
	}
}
