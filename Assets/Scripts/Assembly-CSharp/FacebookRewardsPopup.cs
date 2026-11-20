using System.Collections;
using SYBO.Subway.Info;
using UnityEngine;

public class FacebookRewardsPopup : UIScreen
{
	[SerializeField]
	private UILabel title;

	[SerializeField]
	private UILabel subtitle;

	[SerializeField]
	private UISprite background;

	[SerializeField]
	private UISprite outline;

	[SerializeField]
	private GameObject exitButton;

	[SerializeField]
	private GameObject connectButton;

	[SerializeField]
	private GameObject connectButtonAnchor;

	[SerializeField]
	private UILabel connectButtonLabel;

	[SerializeField]
	private GameObject retrieveButton;

	[SerializeField]
	private GameObject retrieveButtonAnchor;

	[SerializeField]
	private UILabel retrieveButtonLabel;

	private int fontSizeBig = 25;

	private int fontSizeSmall = 20;

	private float sizeOffset1 = 0.5f;

	private float sizeOffset2 = 2f;

	private float sizeOffset3 = 5f;

	[SerializeField]
	private UISprite container1Bg;

	[SerializeField]
	private UISprite container1Check;

	[SerializeField]
	private UISprite[] container1Rays;

	[SerializeField]
	private UISprite container1Glow;

	[SerializeField]
	private UILabel container1Amount;

	[SerializeField]
	private GameObject container1Prefab;

	[SerializeField]
	private UIAnchor container1Anchor;

	private GameObject container1Model;

	private Vector3 offset1 = new Vector3(-3f, 2f, 0f);

	private Vector3 rotation1 = new Vector3(-1f, 46f, -24f);

	private Vector3 scale1 = new Vector3(13f, 13f, 13f);

	[SerializeField]
	private UISprite container2Bg;

	[SerializeField]
	private UISprite container2Check;

	[SerializeField]
	private UISprite[] container2Rays;

	[SerializeField]
	private UISprite container2Glow;

	[SerializeField]
	private UILabel container2Amount;

	[SerializeField]
	private GameObject container2Prefab;

	[SerializeField]
	private UIAnchor container2Anchor;

	private GameObject container2Model;

	private Vector3 offset2 = new Vector3(-3f, 0f, 0f);

	private Vector3 rotation2 = new Vector3(0f, 27f, -34f);

	private Vector3 scale2 = new Vector3(13f, 13f, 13f);

	[SerializeField]
	private UISprite container3Bg;

	[SerializeField]
	private UISprite container3Check;

	[SerializeField]
	private UISprite[] container3Rays;

	[SerializeField]
	private UISprite container3Glow;

	[SerializeField]
	private UILabel container3Amount;

	[SerializeField]
	private GameObject container3Prefab;

	[SerializeField]
	private UIAnchor container3Anchor;

	private GameObject container3Model;

	private Vector3 offset3 = new Vector3(0f, 6f, 0f);

	private Vector3 rotation3 = new Vector3(-22f, -48f, 54f);

	private Vector3 scale3 = new Vector3(14f, 14f, 14f);

	[SerializeField]
	private UISprite container4Bg;

	[SerializeField]
	private UISprite container4Check;

	[SerializeField]
	private UISprite[] container4Rays;

	[SerializeField]
	private UISprite container4Glow;

	[SerializeField]
	private UILabel container4Amount;

	[SerializeField]
	private GameObject container4Prefab;

	[SerializeField]
	private UIAnchor container4Anchor;

	private GameObject container4Model;

	private Vector3 offset4 = new Vector3(0f, 0f, 0f);

	private Vector3 rotation4 = new Vector3(0f, 18.5f, 0f);

	private Vector3 scale4 = new Vector3(7f, 7f, 7f);

	[SerializeField]
	private UISprite container5Bg;

	[SerializeField]
	private UISprite container5Check;

	[SerializeField]
	private UISprite[] container5Rays;

	[SerializeField]
	private UISprite container5Glow;

	[SerializeField]
	private UILabel container5Line1;

	[SerializeField]
	private UILabel container5Line2;

	[SerializeField]
	private UIAnchor container5Anchor;

	private GameObject container5Model;

	[SerializeField]
	private GameObject stars1;

	[SerializeField]
	private GameObject stars2;

	[SerializeField]
	private GameObject stars3;

	public override void Show()
	{
		base.Show();
		bool facebookIsLoggedIn = SocialManager.instance.facebookIsLoggedIn;
		InitTitles(facebookIsLoggedIn);
		InitButtons(facebookIsLoggedIn);
		InitCheckmarks(facebookIsLoggedIn);
		if (!FacebookRewardsInfo.isCharacterRewarded)
		{
			if (!PlayerInfo.Instance.hasPayedOutFacebook)
			{
				SetupPack();
			}
			else
			{
				SetupPackWithoutCoins();
			}
		}
		else if (!PlayerInfo.Instance.hasPayedOutFacebook)
		{
			SetupPackAndCharacter();
		}
		else if (!PlayerInfo.Instance.hasPayedOutFacebookPack)
		{
			SetupPackAndCharacterWithoutCoins();
		}
		else if (!PlayerInfo.Instance.hasPayedOutFacebookCharacter)
		{
			SetupCharacter();
		}
		else
		{
			UIScreenController.Instance.CloseActivePopups(screenName);
		}
	}

	public override void Hide()
	{
		base.Hide();
		Show3DElements(false);
	}

	public override void GainFocus()
	{
		Show3DElements(true);
	}

	public override void LooseFocus()
	{
		Show3DElements(false);
	}

	private void Show3DElements(bool show)
	{
		if (container1Model != null)
		{
			container1Model.SetActive(show);
		}
		if (container2Model != null)
		{
			container2Model.SetActive(show);
		}
		if (container3Model != null)
		{
			container3Model.SetActive(show);
		}
		if (container4Model != null)
		{
			container4Model.SetActive(show);
		}
		if (container5Model != null)
		{
			container5Model.SetActive(show);
		}
	}

	private void SetupPack()
	{
		ResizePopup(309, 392);
		Vector2 rewardSize = new Vector2(135f, 103f);
		Vector2 pixelOffset = new Vector2(-73f, 161f);
		SetupContainer(rewardSize, pixelOffset, container1Anchor, container1Amount, FacebookRewardsInfo.FacebookRewardType.Keys, container1Bg, container1Glow, container1Rays, ref container1Model, container1Prefab, offset1, rotation1, scale1 + Vector3.one * sizeOffset3);
		Vector2 pixelOffset2 = new Vector2(67f, 161f);
		SetupContainer(rewardSize, pixelOffset2, container2Anchor, container2Amount, FacebookRewardsInfo.FacebookRewardType.Headstarts, container2Bg, container2Glow, container2Rays, ref container2Model, container2Prefab, offset2, rotation2, scale2 + Vector3.one * sizeOffset3);
		Vector3 vector = new Vector3(-73f, 53f);
		SetupContainer(rewardSize, vector, container3Anchor, container3Amount, FacebookRewardsInfo.FacebookRewardType.Hoverboards, container3Bg, container3Glow, container3Rays, ref container3Model, container3Prefab, offset3, rotation3, scale3 + Vector3.one * sizeOffset3);
		Vector2 pixelOffset3 = new Vector2(67f, 53f);
		SetupContainer(rewardSize, pixelOffset3, container4Anchor, container4Amount, FacebookRewardsInfo.FacebookRewardType.Coins, container4Bg, container4Glow, container4Rays, ref container4Model, container4Prefab, offset4, rotation4, scale4 + Vector3.one * (sizeOffset3 - 3f));
		DisableContainer(container5Anchor, container5Model);
		SetupLabelSize(fontSizeBig);
	}

	private void SetupPackWithoutCoins()
	{
		ResizePopup(309, 287);
		Vector2 rewardSize = new Vector2(88f, 103f);
		Vector2 pixelOffset = new Vector2(-96f, 58f);
		SetupContainer(rewardSize, pixelOffset, container1Anchor, container1Amount, FacebookRewardsInfo.FacebookRewardType.Keys, container1Bg, container1Glow, container1Rays, ref container1Model, container1Prefab, offset1, rotation1, scale1 + Vector3.one * sizeOffset2);
		Vector2 pixelOffset2 = new Vector2(90f, 58f);
		SetupContainer(rewardSize, pixelOffset2, container2Anchor, container2Amount, FacebookRewardsInfo.FacebookRewardType.Headstarts, container2Bg, container2Glow, container2Rays, ref container2Model, container2Prefab, offset2, rotation2, scale2 + Vector3.one * sizeOffset2);
		Vector3 vector = new Vector3(-3f, 58f);
		SetupContainer(rewardSize, vector, container3Anchor, container3Amount, FacebookRewardsInfo.FacebookRewardType.Hoverboards, container3Bg, container3Glow, container3Rays, ref container3Model, container3Prefab, offset3, rotation3, scale3 + Vector3.one * sizeOffset2);
		DisableContainer(container4Anchor, container4Model);
		DisableContainer(container5Anchor, container5Model);
		SetupLabelSize(fontSizeBig);
	}

	private void SetupPackAndCharacter()
	{
		ResizePopup(309, 363);
		Vector2 rewardSize = new Vector2(75f, 88f);
		Vector2 pixelOffset = new Vector2(18f, 140f);
		SetupContainer(rewardSize, pixelOffset, container1Anchor, container1Amount, FacebookRewardsInfo.FacebookRewardType.Keys, container1Bg, container1Glow, container1Rays, ref container1Model, container1Prefab, offset1, rotation1, scale1);
		Vector2 pixelOffset2 = new Vector2(98f, 140f);
		SetupContainer(rewardSize, pixelOffset2, container2Anchor, container2Amount, FacebookRewardsInfo.FacebookRewardType.Headstarts, container2Bg, container2Glow, container2Rays, ref container2Model, container2Prefab, offset2, rotation2, scale2);
		Vector3 vector = new Vector3(18f, 48f);
		SetupContainer(rewardSize, vector, container3Anchor, container3Amount, FacebookRewardsInfo.FacebookRewardType.Hoverboards, container3Bg, container3Glow, container3Rays, ref container3Model, container3Prefab, offset3, rotation3, scale3);
		Vector2 pixelOffset3 = new Vector2(98f, 48f);
		SetupContainer(rewardSize, pixelOffset3, container4Anchor, container4Amount, FacebookRewardsInfo.FacebookRewardType.Coins, container4Bg, container4Glow, container4Rays, ref container4Model, container4Prefab, offset4, rotation4, scale4);
		Vector2 rewardSize2 = new Vector2(117f, 181f);
		Vector3 positionOffset = new Vector3(0f, -88f, 0f);
		Vector3 rotationOffset = new Vector3(0f, 170f, 0f);
		Vector3 characterScale = Vector3.one * 26f;
		Vector2 pixelOffset4 = new Vector2(-83f, 94f);
		SetupContainer5(rewardSize2, pixelOffset4, positionOffset, rotationOffset, characterScale);
		ActivateStars(1);
		SetupCharacterTitle(false);
		SetupLabelSize(fontSizeSmall);
	}

	private void SetupPackAndCharacterWithoutCoins()
	{
		ResizePopup(309, 400);
		Vector2 rewardSize = new Vector2(111f, 70f);
		Vector2 pixelOffset = new Vector2(79f, 186f);
		SetupContainer(rewardSize, pixelOffset, container1Anchor, container1Amount, FacebookRewardsInfo.FacebookRewardType.Keys, container1Bg, container1Glow, container1Rays, ref container1Model, container1Prefab, offset1, rotation1 + new Vector3(1f, 14f, -4f), scale1 + Vector3.one * sizeOffset1);
		Vector2 pixelOffset2 = new Vector2(79f, 36f);
		SetupContainer(rewardSize, pixelOffset2, container2Anchor, container2Amount, FacebookRewardsInfo.FacebookRewardType.Headstarts, container2Bg, container2Glow, container2Rays, ref container2Model, container2Prefab, offset2, rotation2 + new Vector3(22f, -1f, -6f), scale2 + Vector3.one * sizeOffset1);
		Vector3 vector = new Vector3(79f, 111f);
		SetupContainer(rewardSize, vector, container3Anchor, container3Amount, FacebookRewardsInfo.FacebookRewardType.Hoverboards, container3Bg, container3Glow, container3Rays, ref container3Model, container3Prefab, offset3 + new Vector3(0f, -5f, 0f), rotation3 + new Vector3(-16f, 8f, -2f), scale3 + Vector3.one * sizeOffset1);
		DisableContainer(container4Anchor, container4Model);
		Vector2 rewardSize2 = new Vector2(158f, 220f);
		Vector2 pixelOffset3 = new Vector2(-61f, 111f);
		Vector3 positionOffset = new Vector3(0f, -115f, 0f);
		Vector3 rotationOffset = new Vector3(0f, 180f, 0f);
		Vector3 characterScale = Vector3.one * 31f;
		SetupContainer5(rewardSize2, pixelOffset3, positionOffset, rotationOffset, characterScale);
		ActivateStars(2);
		SetupCharacterTitle(false);
		SetupLabelSize(fontSizeSmall);
	}

	private void SetupCharacter()
	{
		ResizePopup(309, 363);
		DisableContainer(container1Anchor, container1Model);
		DisableContainer(container2Anchor, container2Model);
		DisableContainer(container3Anchor, container3Model);
		DisableContainer(container4Anchor, container4Model);
		Vector2 rewardSize = new Vector2(235f, 182f);
		Vector2 pixelOffset = new Vector2(0f, 95f);
		Vector3 positionOffset = new Vector3(-43f, -150f, 0f);
		Vector3 rotationOffset = new Vector3(0f, 180f, 0f);
		Vector3 characterScale = Vector3.one * 32f;
		SetupContainer5(rewardSize, pixelOffset, positionOffset, rotationOffset, characterScale);
		ActivateStars(3);
		SetupCharacterTitle(true);
		container5Check.gameObject.SetActive(false);
	}

	private void SetupLabelSize(int fontSize)
	{
		container1Amount.fontSize = fontSize;
		container2Amount.fontSize = fontSize;
		container3Amount.fontSize = fontSize;
		container4Amount.fontSize = fontSize;
	}

	private void SetupCharacterTitle(bool isFullscreen)
	{
		UIAnchor component = container5Line2.GetComponent<UIAnchor>();
		component.pixelOffset = ((!isFullscreen) ? new Vector2(0f, 5f) : new Vector2(46f, 27f));
	}

	private void SetupContainer(Vector2 rewardSize, Vector2 pixelOffset, UIAnchor containerAnchor, UILabel containerAmount, FacebookRewardsInfo.FacebookRewardType rewardType, UISprite containerBg, UISprite containerGlow, UISprite[] containerRays, ref GameObject containerModel, GameObject containerPrefab, Vector3 modelOffset, Vector3 modelRotation, Vector3 modelScale)
	{
		containerAnchor.gameObject.SetActive(true);
		containerAmount.text = "x" + FacebookRewardsInfo.facebookRewardPack[rewardType];
		ResizeReward(containerBg, containerGlow, containerRays, rewardSize);
		if (containerModel != null)
		{
			containerModel.SetActive(true);
		}
		else
		{
			containerModel = NGUITools.AddChild(UIModelController.Instance.TutorialPopupAnchor, containerPrefab);
			SetLayerRecursively(containerModel, Layers.Instance._3DGUI);
		}
		PlaceModel(containerBg.gameObject, ref containerModel, modelOffset, modelScale, modelRotation);
		containerAnchor.pixelOffset = pixelOffset;
	}

	private void SetupContainer5(Vector2 rewardSize, Vector2 pixelOffset, Vector3 positionOffset, Vector3 rotationOffset, Vector3 characterScale)
	{
		container5Anchor.gameObject.SetActive(true);
		container5Line1.text = string.Empty;
		container5Line2.text = Strings.Get(StringID.FACEBOOK_REWARD_POPUP_CHARACTER_LINE_2);
		ResizeReward(container5Bg, container5Glow, container5Rays, rewardSize);
		SetupCharacterModel(positionOffset, rotationOffset, characterScale);
		container5Anchor.pixelOffset = pixelOffset;
	}

	private void SetupCharacterModel(Vector3 positionOffset, Vector3 rotationOffset, Vector3 characterScale)
	{
		CharacterModelPreviewFactory.Instance.GetFacebookScreenModel(FacebookRewardsInfo.awardedCharacter.ToString(), delegate(GameObject model)
		{
			container5Model = model;
			if (container5Model == null)
			{
				Debug.LogWarning("Can not instantiate limited character from Online Settings");
			}
			else
			{
				GameObject modelToPosition = PlaceModelUnderAnchor(container5Model, UIModelController.Instance.TutorialPopupAnchor.transform, 31);
				if (modelToPosition != null)
				{
					PlaceModel(container5Bg.gameObject, ref modelToPosition, positionOffset, characterScale, rotationOffset);
				}
			}
		});
	}

	private GameObject PlaceModelUnderAnchor(GameObject model, Transform anchor, int layer)
	{
		NGUITools.SetLayer(model, layer);
		model.transform.parent = anchor;
		model.transform.localPosition = Vector3.zero;
		model.transform.localEulerAngles = Vector3.zero;
		model.transform.localScale = Vector3.one;
		return model;
	}

	private void DisableContainer(UIAnchor containerAnchor, GameObject containerModel)
	{
		containerAnchor.gameObject.SetActive(false);
		if (containerModel != null)
		{
			containerModel.SetActive(false);
		}
	}

	private void ResizePopup(int width, int height)
	{
		outline.width = width;
		outline.height = height;
		background.width = width - 11;
		background.height = height - 11;
	}

	private void ResizeReward(UISprite bg, UISprite glow, UISprite[] rays, Vector2 newSize)
	{
		bg.width = (int)newSize.x;
		bg.height = (int)newSize.y;
		glow.width = (int)newSize.x;
		glow.height = (int)newSize.y;
		foreach (UISprite uISprite in rays)
		{
			uISprite.width = (int)newSize.y / 2;
			uISprite.height = (int)newSize.x / 2;
		}
	}

	private void InitTitles(bool isLoggedIn)
	{
		if (!isLoggedIn)
		{
			title.text = Strings.Get(StringID.FACEBOOK_REWARDS_POPUP_TITLE_NOT_LOGGED_IN);
			subtitle.text = Strings.Get(StringID.FACEBOOK_REWARDS_POPUP_SUBTITLE_NOT_LOGGED_IN);
		}
		else
		{
			title.text = Strings.Get(StringID.FACEBOOK_REWARDS_POPUP_TITLE_LOGGED_IN);
			subtitle.text = Strings.Get(StringID.FACEBOOK_REWARDS_POPUP_SUBTITLE_LOGGED_IN);
		}
	}

	private void InitButtons(bool isLoggedIn)
	{
		connectButtonLabel.text = Strings.Get(StringID.FACEBOOK_REWARDS_POPUP_CONNECT_BUTTON_TEXT);
		retrieveButtonLabel.text = Strings.Get(StringID.FACEBOOK_REWARDS_POPUP_RETRIEVE_BUTTON_TEXT);
		exitButton.SetActive(!isLoggedIn);
		connectButton.SetActive(!isLoggedIn);
		retrieveButton.SetActive(isLoggedIn);
	}

	private void InitCheckmarks(bool facebookLoggedIn)
	{
		container1Check.gameObject.SetActive(facebookLoggedIn);
		container2Check.gameObject.SetActive(facebookLoggedIn);
		container3Check.gameObject.SetActive(facebookLoggedIn);
		container4Check.gameObject.SetActive(facebookLoggedIn);
		container5Check.gameObject.SetActive(facebookLoggedIn);
	}

	public void PlaceModel(GameObject modelHost, ref GameObject modelToPosition, Vector3 offset, Vector3 scale, Vector3 rotation)
	{
		StartCoroutine(DelayPositioning(modelHost, modelToPosition, offset, scale, rotation));
	}

	private IEnumerator DelayPositioning(GameObject modelHost, GameObject modelToPosition, Vector3 offset, Vector3 scale, Vector3 rotation)
	{
		yield return new WaitForEndOfFrame();
		Vector3 trackerPos = modelHost.transform.position;
		Vector3 pixelPos = UIScreenController.Instance.Camera2DNormal.WorldToScreenPoint(trackerPos);
		pixelPos = new Vector3(pixelPos.x, pixelPos.y, 3f);
		ResetTransform(newPosition: UIScreenController.Instance.Camera3D.ScreenToWorldPoint(pixelPos), trans: modelToPosition.transform, positionOffset: offset, newRotation: rotation, newScale: scale);
	}

	private void ResetTransform(Transform trans, Vector3 newPosition, Vector3 positionOffset, Vector3 newRotation, Vector3 newScale)
	{
		trans.position = newPosition;
		trans.localPosition += positionOffset;
		trans.localRotation = Quaternion.Euler(newRotation);
		trans.localScale = newScale;
	}

	private void SetLayerRecursively(GameObject target, int layer)
	{
		target.layer = layer;
		if (target.transform.childCount == 0)
		{
			return;
		}
		foreach (Transform item in target.transform)
		{
			SetLayerRecursively(item.gameObject, layer);
		}
	}

	public void OnConnectClicked()
	{
		if (SocialManager.instance.FacebookLogin(OnFacebookLoginResult))
		{
			UIScreenController.Instance.FacebookLogIn(true);
		}
		UIScreenController.Instance.CloseActivePopups(screenName);
	}

	public void OnRetrieveClicked()
	{
		DeliverRemainingFacebookRewards();
		UIScreenController.Instance.CloseActivePopups(screenName);
	}

	public void OnBackBtnClicked()
	{
		if (retrieveButton.activeSelf && retrieveButton.activeInHierarchy)
		{
			DeliverRemainingFacebookRewards();
		}
	}

	private void OnFacebookLoginResult(bool loggedIn)
	{
		if (!loggedIn)
		{
			UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Facebook, UISlideInFacebook.FacebookSlideInState.ConnectFailed));
		}
	}

	private void DeliverRemainingFacebookRewards()
	{
		if (!PlayerInfo.Instance.hasPayedOutFacebook)
		{
			PlayerInfo.Instance.amountOfCoins += FacebookRewardsInfo.facebookRewardPack[FacebookRewardsInfo.FacebookRewardType.Coins];
			PlayerInfo.Instance.hasPayedOutFacebook = true;
		}
		if (!PlayerInfo.Instance.hasPayedOutFacebookPack)
		{
			PlayerInfo.Instance.amountOfKeys += FacebookRewardsInfo.facebookRewardPack[FacebookRewardsInfo.FacebookRewardType.Keys];
			PlayerInfo.Instance.IncreaseUpgradeAmount(PowerupType.hoverboard, FacebookRewardsInfo.facebookRewardPack[FacebookRewardsInfo.FacebookRewardType.Hoverboards]);
			PlayerInfo.Instance.IncreaseUpgradeAmount(PowerupType.headstart2000, FacebookRewardsInfo.facebookRewardPack[FacebookRewardsInfo.FacebookRewardType.Headstarts]);
			PlayerInfo.Instance.hasPayedOutFacebookPack = true;
		}
		if (FacebookRewardsInfo.isCharacterRewarded && !PlayerInfo.Instance.hasPayedOutFacebookCharacter)
		{
			Characters.CharacterType awardedCharacter = FacebookRewardsInfo.awardedCharacter;
			int characterTheme = FacebookRewardsInfo.characterTheme;
			Characters.Model model = Characters.characterData[awardedCharacter];
			CharacterTheme? themeForCharacter = CharacterThemes.GetThemeForCharacter(awardedCharacter, characterTheme);
			bool hasValue = themeForCharacter.HasValue;
			int price = model.Price;
			if (hasValue)
			{
				price = themeForCharacter.Value.price;
			}
			CelebrationReward celebrationReward = new CelebrationReward();
			celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.CharacterUnlock;
			celebrationReward.rewardType = CelebrationRewardType.character;
			celebrationReward.characterType = awardedCharacter;
			celebrationReward.characterOutfitIndex = characterTheme;
			RewardManager.AddRewardToUnlock(celebrationReward);
			if (hasValue)
			{
				PlayerInfo.Instance.UnlockTheme(awardedCharacter, characterTheme);
			}
			else
			{
				PlayerInfo.Instance.CollectToken(awardedCharacter, price);
			}
			CharacterScreenManager.Instance.OnFacebookCharacterUnlocked();
			UIScreenController.Instance.QueuePopupRightAfterCurrentActivePopup("CelebrationPopup");
			PlayerInfo.Instance.hasPayedOutFacebookCharacter = true;
		}
		PlayerInfo.Instance.SaveIfDirty();
	}

	private void ActivateStars(int set)
	{
		switch (set)
		{
		case 1:
			stars1.SetActive(true);
			stars2.SetActive(false);
			stars3.SetActive(false);
			break;
		case 2:
			stars1.SetActive(false);
			stars2.SetActive(true);
			stars3.SetActive(false);
			break;
		case 3:
			stars1.SetActive(false);
			stars2.SetActive(false);
			stars3.SetActive(true);
			break;
		default:
			stars1.SetActive(false);
			stars2.SetActive(false);
			stars3.SetActive(false);
			break;
		}
	}
}
