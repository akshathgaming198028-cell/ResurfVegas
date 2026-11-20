using UnityEngine;

public class ContentChange : MonoBehaviour
{
	private bool foldedOut;

	public GameObject button;

	public GameObject openButton;

	public GameObject descriptionButton;

	[SerializeField]
	private UILabel descriptionText;

	private GameObject _descriptionTextGameObject;

	[SerializeField]
	private BoxCollider buyButtonCollider;

	[SerializeField]
	private UISprite buyButtonSprite;

	[SerializeField]
	private UILabel buyButtonLabel;

	[SerializeField]
	private GameObject tweensContainer;

	[SerializeField]
	private BoxCollider descButtonCollider;

	[SerializeField]
	private UISprite descButtonSprite;

	[SerializeField]
	private UILabel descButtonLabel;

	[SerializeField]
	private GameObject expandedContent;

	[SerializeField]
	private UISprite openButtonSprite;

	[SerializeField]
	private TableElementAlwaysVisible tableElement;

	public PowerupType powerupType;

	private UIPlayTween[] _tweens;

	private UpgradeHelper _upgradeHelper;

	private bool _hasInited;

	private bool _wasForceOpened;

	private void Start()
	{
		_descriptionTextGameObject = descriptionText.gameObject;
		foldedOut = false;
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.BiggerJackpot);
		if (tweensContainer != null)
		{
			_tweens = tweensContainer.GetComponents<UIPlayTween>();
			_upgradeHelper = tweensContainer.GetComponent<UpgradeHelper>();
		}
		if (powerupType == PowerupType.mysterybox && activeEventManager != null)
		{
			if (UIScreenController.Instance.GetTopScreenName() != "GameoverUI")
			{
				ForceExpandContent(true);
			}
		}
		else
		{
			ContentActivation(false);
		}
		_hasInited = true;
	}

	private void ForceExpandContent(bool expand)
	{
		_wasForceOpened = expand;
		tableElement = base.gameObject.transform.parent.GetComponent<TableElementAlwaysVisible>();
		if (tableElement != null)
		{
			tableElement.ToggleButtonScale();
		}
		if (_tweens != null)
		{
			UIPlayTween[] tweens = _tweens;
			foreach (UIPlayTween uIPlayTween in tweens)
			{
				uIPlayTween.Play(expand);
			}
		}
		_upgradeHelper.AnimationStarted();
	}

	private void OnEnable()
	{
		if (!_hasInited)
		{
			return;
		}
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.BiggerJackpot);
		if (powerupType == PowerupType.mysterybox && activeEventManager != null)
		{
			if (!foldedOut && UIScreenController.Instance.GetTopScreenName() != "GameoverUI")
			{
				ForceExpandContent(true);
			}
		}
		else if (_wasForceOpened)
		{
			ForceExpandContent(false);
		}
		else
		{
			ContentActivation(foldedOut);
		}
	}

	public void TriggerContent()
	{
		if (foldedOut)
		{
			ContentActivation(foldedOut);
		}
	}

	public void FoldClicked()
	{
		if (!foldedOut)
		{
			foldedOut = true;
			return;
		}
		foldedOut = false;
		ContentActivation(false);
	}

	private void ContentActivation(bool foldedOut)
	{
		descriptionText.enabled = foldedOut;
		_descriptionTextGameObject.SetActive(foldedOut);
		if (buyButtonSprite != null)
		{
			buyButtonCollider.center = new Vector3(buyButtonCollider.center.x, buyButtonCollider.center.y, -15f);
			buyButtonCollider.enabled = foldedOut;
			buyButtonSprite.enabled = foldedOut;
			buyButtonLabel.enabled = foldedOut;
		}
		if (button != null)
		{
			NGUITools.SetActive(button, foldedOut);
		}
		expandedContent.SetActive(foldedOut);
		openButtonSprite.enabled = !foldedOut;
	}
}
