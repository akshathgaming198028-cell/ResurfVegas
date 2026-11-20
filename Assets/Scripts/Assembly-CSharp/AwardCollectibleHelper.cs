using UnityEngine;

public class AwardCollectibleHelper : MonoBehaviour
{
	[SerializeField]
	private UISprite _itemIconSprite;

	[SerializeField]
	private GameObject _completeIcon;

	[SerializeField]
	private GameObject _glitterOverlay;

	[SerializeField]
	private UISprite _background;

	public void SetupAwardCollectible(string backgroundToUse, string itemIconName, bool useGlitter, bool isItemCollected = false)
	{
		_itemIconSprite.transform.localRotation = Quaternion.Euler(Vector3.zero);
		_itemIconSprite.spriteName = itemIconName;
		_completeIcon.SetActive(isItemCollected);
		_background.spriteName = backgroundToUse;
		if (_glitterOverlay != null)
		{
			_glitterOverlay.gameObject.SetActive(useGlitter);
		}
	}
}
