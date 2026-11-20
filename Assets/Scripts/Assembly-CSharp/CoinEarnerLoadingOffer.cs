using System;
using System.Collections;
using Kiloo.Monetization.Offerwall;
using UnityEngine;

public class CoinEarnerLoadingOffer : MonoBehaviour
{
	public enum OffersState
	{
		Loading = 0,
		NoOffersAvailable = 1,
		Offer = 2
	}

	private const string BG_SPRITENAME_ACTIVE_DEFAULT = "button_fill_shopItem_main";

	private const string BG_SPRITENAME_OVERLAY_DEFAULT = "button_fill_shopItem_highlight";

	private const string BG_SPRITENAME_INACTIVE = "button_generic_1";

	[SerializeField]
	private GameObject _loadingGameObject;

	[SerializeField]
	private GameObject _noOffersAvailableGameObject;

	[SerializeField]
	private GameObject _offersGameObject;

	[SerializeField]
	private UISprite _fillOverlay;

	[SerializeField]
	private UISprite _backgroundFill;

	[SerializeField]
	private GameObject _coinEarnerPrefab;

	[SerializeField]
	private UISprite _freeSticker;

	[SerializeField]
	private UILabel _priceLabel;

	[SerializeField]
	private UIAnchor _priceAnchor;

	[SerializeField]
	private UILabel _titleLabel;

	[SerializeField]
	private UILabel _descriptionLabel;

	[SerializeField]
	private UITexture _picture;

	[SerializeField]
	private UILabel _loadingAppOffers;

	[SerializeField]
	private UILabel _couldNotLoad;

	[SerializeField]
	private UILabel _tryAgain;

	private UIButtonColor _btColor;

	private OffersState _currentState;

	private static int _timeLimit = 10;

	private bool _isLodingTimerStarted;

	private IDisplayableOffer _offer;

	private Action _onClickHandler;

	public void Init(IDisplayableOffer offer, Action onClickDelegate)
	{
		if (_offer != null)
		{
			return;
		}
		_offer = offer;
		_offer.textureChanged += OfferImageDownloaded;
		if (onClickDelegate != null)
		{
			_onClickHandler = (Action)Delegate.Combine(_onClickHandler, onClickDelegate);
		}
		int result = 0;
		if (int.TryParse(offer.payoutAmount, out result))
		{
			if (result > 0)
			{
				_priceLabel.text = result.ToString();
			}
			_priceAnchor.pixelOffset = new Vector2(_priceLabel.width / 2 + 8, -10f);
		}
		_titleLabel.text = _offer.title;
		_descriptionLabel.text = Strings.Get(StringID.TAPJOY_OFFER_DESCRIPTION);
		if (_offer.texture != null)
		{
			OfferImageDownloaded();
		}
	}

	private void Awake()
	{
		SetOffersState(OffersState.Loading);
		checkIfNullAndSetTextWhereIsTheCase();
	}

	private void OnDisable()
	{
		if (_offer != null)
		{
			_offer.textureChanged -= OfferImageDownloaded;
		}
	}

	public void SetOffersState(OffersState state)
	{
		_currentState = state;
		switch (state)
		{
		case OffersState.Loading:
			SetStateGameObjectActive(_loadingGameObject);
			break;
		case OffersState.NoOffersAvailable:
			SetStateGameObjectActive(_noOffersAvailableGameObject);
			break;
		case OffersState.Offer:
			SetStateGameObjectActive(_offersGameObject);
			break;
		default:
			SetStateGameObjectActive(_loadingGameObject);
			break;
		}
	}

	public OffersState getOffersState()
	{
		return _currentState;
	}

	private void SetStateGameObjectActive(GameObject go)
	{
		if (_loadingGameObject != null)
		{
			_loadingGameObject.SetActive(false);
		}
		else
		{
			Debug.LogError("Loading Game object not connected in CoinEarnerLoadingOffer");
		}
		if (_noOffersAvailableGameObject != null)
		{
			_noOffersAvailableGameObject.SetActive(false);
		}
		else
		{
			Debug.LogError("No Offers available Game object not connected in CoinEarnerLoadingOffer");
		}
		if (_offersGameObject != null)
		{
			_offersGameObject.SetActive(false);
		}
		else
		{
			Debug.LogError("Offers Game object not connected in CoinEarnerLoadingOffer");
		}
		if (!(go != null))
		{
			return;
		}
		if (go == _noOffersAvailableGameObject || go == _offersGameObject)
		{
			_backgroundFill.spriteName = "button_fill_shopItem_main";
			_backgroundFill.color = GlobalColors.COLOR_WHITE;
			_fillOverlay.spriteName = "button_fill_shopItem_highlight";
			_fillOverlay.gameObject.SetActive(true);
			AddButtonColor(_backgroundFill);
			_coinEarnerPrefab.GetComponent<Collider>().enabled = true;
			StopCoroutine("StartLoadingCoroutine");
			_isLodingTimerStarted = false;
		}
		else if (go == _loadingGameObject)
		{
			if (_btColor != null)
			{
				UnityEngine.Object.Destroy(_btColor);
			}
			_fillOverlay.gameObject.SetActive(false);
			_coinEarnerPrefab.GetComponent<Collider>().enabled = false;
			_backgroundFill.spriteName = "button_generic_1";
			_backgroundFill.color = GlobalColors.PRIMARY_ACTION_COLOR_UNAVAILABLE;
			if (!_isLodingTimerStarted)
			{
				StartCoroutine("StartLoadingCoroutine");
			}
		}
		go.SetActive(true);
	}

	private void OnClick()
	{
		if (_noOffersAvailableGameObject.activeInHierarchy)
		{
			OfferWallManager.Instance.RefreshOffers();
			SetOffersState(OffersState.Loading);
		}
		if (_offersGameObject.activeInHierarchy)
		{
			OfferWallManager.Instance.OfferClicked(_offer);
			Application.OpenURL(_offer.actionURL);
			if (_onClickHandler != null)
			{
				_onClickHandler();
			}
		}
	}

	private void AddButtonColor(UISprite tweenTarget)
	{
		if (_btColor != null)
		{
			UnityEngine.Object.Destroy(_btColor);
		}
		_btColor = base.gameObject.AddComponent<UIButtonColor>();
		_btColor.tweenTarget = tweenTarget.gameObject;
		_btColor.hover = GlobalColors.COLOR_WHITE;
		_btColor.duration = 0.05f;
	}

	private void OfferImageDownloaded()
	{
		if (_offer != null && _offer.texture != null)
		{
			_picture.mainTexture = _offer.texture;
		}
	}

	private IEnumerator StartLoadingCoroutine()
	{
		_isLodingTimerStarted = true;
		yield return new WaitForSeconds(_timeLimit);
		_isLodingTimerStarted = false;
		SetOffersState(OffersState.NoOffersAvailable);
	}

	private void checkIfNullAndSetTextWhereIsTheCase()
	{
		if (_loadingAppOffers == null)
		{
			Debug.LogError("_loadingAppOffers not set in the Coin earner Offer Button Helper prefab");
		}
		else
		{
			_loadingAppOffers.text = Strings.Get(StringID.OFFERWALL_BUTTON_LOADING);
		}
		if (_couldNotLoad == null)
		{
			Debug.LogError("_couldNotLoad not set in the Coin earner Offer Button Helper prefab");
		}
		else
		{
			_couldNotLoad.text = Strings.Get(StringID.OFFERWALL_BUTTON_COULD_NOT_LOAD);
		}
		if (_tryAgain == null)
		{
			Debug.LogError("_couldNotLoad not set in the Coin earner Offer Button Helper prefab");
		}
		else
		{
			_tryAgain.text = Strings.Get(StringID.OFFERWALL_BUTTON_TRY_AGAIN);
		}
		if (_freeSticker == null)
		{
			Debug.LogError("Free sticker not set in the Coin earner Offer Button Helper prefab");
		}
		if (_priceLabel == null)
		{
			Debug.LogError("Price Label not set in the Coin earner Offer Button Helper prefab");
		}
		if (_titleLabel == null)
		{
			Debug.LogError("Title Label not set in the Coin earner Offer Button Helper prefab");
		}
		if (_descriptionLabel == null)
		{
			Debug.LogError("Description label not set in the Coin earner Offer Button Helper prefab");
		}
		if (_picture == null)
		{
			Debug.LogError("Picture not set in the Coin earner Offer Button Helper prefab");
		}
	}
}
