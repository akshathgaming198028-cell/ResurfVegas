using System;
using System.Collections;
using System.Collections.Generic;
using SYBO.Subway.Info;
using UnityEngine;

public class CharacterNameEventPopup : UIScreen
{
	private enum HorizontalScrollDirections
	{
		left = -1,
		right = 1
	}

	private const float DEFAULT_SCALE_FACTOR = 9.3f;

	private const float FOCUSED_SCALE_FACTOR = 13f;

	private const float DEFAULT_Y_POSITION_ON_SCROLL = 0f;

	private const float FOCUSED_Y_POSITION_ON_SCROLL = -9f;

	private const float DEFAULT_FREEZE_VALUE = 1f;

	private const float FOCUSED_FREEZE_VALUE = 0f;

	private const float ANIMATION_SPEED_FACTOR = 2f;

	private const float SPACING_BETWEEN_CHARACTERS = 77f;

	[SerializeField]
	private UIWidget outline;

	[SerializeField]
	private GameObject characterScrollAnchor;

	[SerializeField]
	private BoxCollider skipButtonCollider;

	[SerializeField]
	private GameObject skipButtonWidget;

	[SerializeField]
	private GameObject completeSignWidget;

	[SerializeField]
	private GameObject startArrowWidget;

	[SerializeField]
	private GameObject endArrowWidget;

	[SerializeField]
	private GameObject centerAnchor;

	[SerializeField]
	private GameObject overlayClipAnchor;

	[SerializeField]
	private GameObject selectButtonPrefab;

	[SerializeField]
	private GameObject eventDescriptionBox;

	[SerializeField]
	private UILabel eventDescriptionBoxName;

	[SerializeField]
	private GameObject eventDescriptionBoxActive;

	[SerializeField]
	private UILabel eventDescriptionBoxTitleInactive;

	[SerializeField]
	private UILabel eventDescriptionTitleActive;

	[SerializeField]
	private UILabel eventDescriptionBoxNameActive;

	[SerializeField]
	private UILabel eventDescriptionBoxNameActiveShadow;

	[SerializeField]
	private UILabel firstTitle;

	[SerializeField]
	private UILabel secondTitle;

	[SerializeField]
	private UILabel skipButtonKeyAmount;

	[SerializeField]
	private UILabel timerLabel;

	[SerializeField]
	private UILabel startArrowLabel;

	[SerializeField]
	private UILabel endArrowLabel;

	private List<KeyValuePair<Characters.CharacterType, CharacterScreenController>> _characterControllers = new List<KeyValuePair<Characters.CharacterType, CharacterScreenController>>();

	private int _scrollPopulationStartFromSlot;

	private int _currentlyFocusedIndex;

	private bool _isFirstFocused;

	private bool _isLastFocused;

	private CharacterNameEventManager _characterNameEventManager;

	private CharacterScreenSelectButton _selectButton;

	public override void Init()
	{
		base.Init();
		InitializeCoinbox(outline, false, true, true, 17f);
		InitializeSelectButton();
	}

	public override void GainFocus()
	{
		base.GainFocus();
		overlayClipAnchor.SetActive(true);
		characterScrollAnchor.SetActive(true);
		ClearScrollListOfCharacters();
		InitScrollWithCharacterModels();
		EnableUIElementsForList();
	}

	public override void LooseFocus()
	{
		base.LooseFocus();
		overlayClipAnchor.SetActive(false);
		characterScrollAnchor.SetActive(false);
	}

	private void SetActiveCharacterNameEventManager()
	{
		_characterNameEventManager = SSEventsManagerController.instance.GetTypeOfActiveEventManager<CharacterNameEventManager>();
	}

	private void InitializeSelectButton()
	{
		GameObject gameObject = NGUITools.AddChild(centerAnchor, selectButtonPrefab);
		_selectButton = gameObject.GetComponent<CharacterScreenSelectButton>();
		if (_selectButton != null)
		{
			Utility.SetLayerRecursively(_selectButton.transform, 30);
			_selectButton.transform.localPosition = new Vector3(0f, -173f, 0f);
			_selectButton.InitButton(CharacterScreenSelectButton.ButtonContext.characterNameEventPopup);
		}
		else
		{
			Debug.LogError("Failed to initialized select button");
		}
	}

	private void ReloadSelectButton(Characters.CharacterType charType, int themeIndex)
	{
		RefreshDescriptionBox();
		if (_characterNameEventManager != null)
		{
			skipButtonWidget.SetActive(_characterNameEventManager.IsAbleToSkip());
		}
		_selectButton.ReloadButton();
	}

	private void OnSelectCharacter()
	{
		RefreshDescriptionBox();
		_selectButton.ReloadButton();
	}

	public override void Show()
	{
		base.Show();
		SetActiveCharacterNameEventManager();
		SSEventsManagerController.OnSSEventFinishedHandler += HidePopupIfNeeded;
		CharacterScreenManager.Instance.AddOnCharacterUnlockedListener(ReloadSelectButton);
		CharacterScreenManager.OnCharacterSelected += OnSelectCharacter;
		UIScreenController.Instance.RecalculateViewportRectForClipCamera(UIScreenController.Instance.Camera3DClip, true);
		UIScreenController.Instance.SetCameraToOrtographic(UIScreenController.Instance.Camera3DClip, false);
		UIScreenController.Instance.CalculateCameraFieldOfView(UIScreenController.Instance.Camera3DClip, 640f, 450f);
		UIScreenController.OnApplicationResumed += HidePopupIfNeeded;
		ClearScrollListOfCharacters();
		InitScrollWithCharacterModels();
		EnableUIElementsForList();
		firstTitle.text = Strings.Get(StringID.EVENTS_POPUP_WEEKEND_EVENT_TITLE_LINE_1);
		secondTitle.text = Strings.Get(StringID.EVENTS_POPUP_WEEKEND_EVENT_TITLE_LINE_2);
		startArrowLabel.text = Strings.Get(StringID.EVENTS_POPUP_START_ARROW_TEXT);
		endArrowLabel.text = Strings.Get(StringID.EVENTS_POPUP_END_ARROW_TEXT);
		RefreshDescriptionBox();
		if (skipButtonCollider != null)
		{
			skipButtonCollider.enabled = true;
		}
		_selectButton.ReloadButton(true, _characterControllers[_currentlyFocusedIndex].Key);
	}

	public override void Hide()
	{
		base.Hide();
		SSEventsManagerController.OnSSEventFinishedHandler -= HidePopupIfNeeded;
		CharacterScreenManager.Instance.RemoveOnCharacterUnlockedListener(ReloadSelectButton);
		CharacterScreenManager.OnCharacterSelected -= OnSelectCharacter;
		ClearScrollListOfCharacters();
		UIScreenController.Instance.RecalculateViewportRectForClipCamera(UIScreenController.Instance.Camera3DClip, false);
		UIScreenController.Instance.SetCameraToOrtographic(UIScreenController.Instance.Camera3DClip, true);
		UIScreenController.OnApplicationResumed -= HidePopupIfNeeded;
	}

	private void ClearScrollListOfCharacters()
	{
		foreach (KeyValuePair<Characters.CharacterType, CharacterScreenController> characterController in _characterControllers)
		{
			if (characterController.Value != null && characterController.Value.gameObject != null)
			{
				UnityEngine.Object.Destroy(characterController.Value.gameObject);
			}
		}
		_characterControllers.Clear();
	}

	private void InitScrollWithCharacterModels()
	{
		characterScrollAnchor.transform.localPosition = new Vector3(-77f, characterScrollAnchor.transform.localPosition.y, characterScrollAnchor.transform.localPosition.z);
		if (_characterNameEventManager != null)
		{
			List<Characters.CharacterType> charactersToDisplay = _characterNameEventManager.GetCharactersToDisplay();
			_isFirstFocused = _characterNameEventManager.IsShowingFirstElement();
			_isLastFocused = _characterNameEventManager.IsShowingLastElement();
			bool flag = _characterNameEventManager.IsFirstCharacterSelected();
			if (_characterNameEventManager.HasCompletedCollectionForSelectedCharacter())
			{
				_currentlyFocusedIndex = ((!flag) ? 1 : 0);
				_scrollPopulationStartFromSlot = (flag ? 1 : 0);
				Invoke("ScrollToNextCharacter", 0.3f);
			}
			else
			{
				_currentlyFocusedIndex = ((!_isFirstFocused) ? 1 : 0);
				_scrollPopulationStartFromSlot = (_isFirstFocused ? 1 : 0);
			}
			for (int i = 0; i < charactersToDisplay.Count; i++)
			{
				InsertCharacterIntoModelsList(charactersToDisplay[i], _currentlyFocusedIndex);
			}
			Utility.SetLayerRecursively(characterScrollAnchor.transform, 29);
		}
	}

	private void InsertCharacterIntoModelsList(Characters.CharacterType characterType, int focusedIndex)
	{
		if (_characterNameEventManager == null)
		{
			return;
		}
		int lastSelectedThemeForCharacterType = _characterNameEventManager.GetLastSelectedThemeForCharacterType(characterType);
		CharacterModelPreviewFactory.Instance.GetCharacterEventScreenModel(characterType.ToString(), lastSelectedThemeForCharacterType, delegate(CharacterScreenController currentCharacterModelController)
		{
			if (currentCharacterModelController == null)
			{
				Debug.LogError(string.Concat("CharacterName Event Popup: GetCharacterEventScreenModel for character ", characterType, " has failed. Creating default model."));
			}
			_characterControllers.Add(new KeyValuePair<Characters.CharacterType, CharacterScreenController>(characterType, currentCharacterModelController));
			if (currentCharacterModelController != null)
			{
				currentCharacterModelController.name = string.Format("{0:000}{1}", _characterControllers.Count, characterType.ToString());
				Transform transform = currentCharacterModelController.transform;
				transform.parent = characterScrollAnchor.transform;
				transform.localPosition = new Vector3((float)_scrollPopulationStartFromSlot * 77f, 0f, 0f);
				transform.localEulerAngles = new Vector3(0f, 180f, 0f);
				if (_characterControllers.Count - 1 == focusedIndex)
				{
					transform.localScale = Vector3.one * 13f;
					Vector3 localPosition = transform.localPosition;
					localPosition.y = -9f;
					transform.localPosition = localPosition;
					currentCharacterModelController.Freeze = 0f;
				}
				else
				{
					transform.localScale = Vector3.one * 9.3f;
					Vector3 localPosition2 = transform.localPosition;
					localPosition2.y = 0f;
					transform.localPosition = localPosition2;
					currentCharacterModelController.Freeze = 1f;
				}
				_scrollPopulationStartFromSlot++;
				Utility.SetLayerRecursively(transform, 29);
			}
		});
	}

	private void EnableUIElementsForList()
	{
		if (_characterNameEventManager != null)
		{
			startArrowWidget.SetActive(_characterNameEventManager.IsShowingFirstElement());
			endArrowWidget.SetActive(_isLastFocused);
			completeSignWidget.SetActive(_characterNameEventManager.ShouldShowTheCompletedSign());
			skipButtonWidget.SetActive(_characterNameEventManager.IsAbleToSkip());
			skipButtonKeyAmount.text = _characterNameEventManager.GetNumberOfKeysToSkip().ToString();
			_selectButton.ReloadButton(true, _characterControllers[_currentlyFocusedIndex].Key);
		}
	}

	private void RefreshDescriptionBox()
	{
		if (_characterNameEventManager != null)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			_characterNameEventManager.GetPopupDescription(out empty, out empty2);
			if (_characterNameEventManager.SelectedCharacterMatchesFocusedCharacter())
			{
				SetActiveCharacterBox(empty, empty2);
			}
			else
			{
				SetInactiveCharacterBox(empty, empty2);
			}
		}
	}

	private void SetInactiveCharacterBox(string descriptionLine1, string descriptionLine2)
	{
		eventDescriptionBox.SetActive(true);
		eventDescriptionBoxActive.SetActive(false);
		eventDescriptionBoxTitleInactive.text = descriptionLine1;
		eventDescriptionBoxName.text = descriptionLine2;
	}

	private void SetActiveCharacterBox(string descriptionLine1, string descriptionLine2)
	{
		eventDescriptionBox.SetActive(false);
		eventDescriptionBoxActive.SetActive(true);
		string text = NGUIText.EncodeColor(GlobalColors.CHARACTERWORDEVENT_POPUP_NAME_ACTIVE);
		string text2 = NGUIText.EncodeColor(GlobalColors.CHARACTERWORDEVENT_POPUP_NAME_INACTIVE);
		string text3 = NGUIText.EncodeColor(GlobalColors.CHARACTERWORDEVENT_POPUP_NAME_SHADOW_ACTIVE);
		string text4 = NGUIText.EncodeColor(GlobalColors.CHARACTERWORDEVENT_POPUP_NAME_SHADOW_INACTIVE);
		int num = 0;
		CharacterNameEventManager typeOfActiveEventManager = SSEventsManagerController.instance.GetTypeOfActiveEventManager<CharacterNameEventManager>();
		if (typeOfActiveEventManager != null)
		{
			num = typeOfActiveEventManager.GetAmountOfCollectedLetters();
		}
		char[] array = descriptionLine2.ToCharArray();
		string text5 = string.Empty;
		string text6 = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			if (IsFilteredCharacter(array[i]))
			{
				num++;
			}
			else if (num > i)
			{
				text5 = text5 + "[" + text + "]";
				text6 = text6 + "[" + text3 + "]";
			}
			else
			{
				text5 = text5 + "[" + text2 + "]";
				text6 = text6 + "[" + text4 + "]";
			}
			text5 = text5 + array[i] + " ";
			text6 = text6 + array[i] + " ";
		}
		if (array.Length > 0)
		{
			eventDescriptionBoxNameActive.text = text5.Substring(0, text5.Length - 1);
			eventDescriptionBoxNameActiveShadow.text = text6.Substring(0, text6.Length - 1);
		}
		eventDescriptionTitleActive.text = descriptionLine1;
	}

	private void Update()
	{
		SetTheClock();
	}

	private void SetTheClock()
	{
		long num = SSEventsManagerController.instance.TimeLeftInSeconds();
		if (num > 0)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(num);
			timerLabel.text = string.Format(Strings.Get(StringID.EVENTS_POPUP_CLOCK), string.Format("{0:00}:{1:00}:{2:00}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds));
		}
		else
		{
			timerLabel.text = Strings.Get(StringID.EVENT_POPUP_EVENT_EXPIRED);
		}
	}

	public void OnSkipClicked()
	{
		if (_characterNameEventManager != null)
		{
			if (_characterNameEventManager.CanAffordToSkip())
			{
				_characterNameEventManager.UseKeysToSkip();
				ScrollToNextCharacter();
			}
			else
			{
				PurchaseHandler.Instance.PurchaseKeysIfNeeded(_characterNameEventManager.GetNumberOfKeysToSkip());
			}
		}
	}

	private void ScrollToNextCharacter()
	{
		if (_characterNameEventManager != null && !_isLastFocused)
		{
			_isFirstFocused = false;
			_characterNameEventManager.SkipCurrentCharacter();
			Characters.CharacterType? nextCharacterToDisplay = _characterNameEventManager.GetNextCharacterToDisplay();
			_currentlyFocusedIndex++;
			if (nextCharacterToDisplay.HasValue)
			{
				InsertCharacterIntoModelsList(nextCharacterToDisplay.Value, _currentlyFocusedIndex);
			}
			else
			{
				_isLastFocused = true;
			}
			ScrollListTo(_currentlyFocusedIndex);
			RefreshDescriptionBox();
		}
	}

	private void ScrollListTo(int currentCharacterIndex)
	{
		EnableUIElementsForList();
		if (currentCharacterIndex <= _characterControllers.Count)
		{
			StartCoroutine(AnimateScrollBar(HorizontalScrollDirections.left, 77f, 2f));
			StartCoroutine(AnimateCharacter(_characterControllers[currentCharacterIndex - 1].Value, 13f, 9.3f, -9f, 0f, 0f, 1f, 2f));
			StartCoroutine(AnimateCharacter(_characterControllers[currentCharacterIndex].Value, 9.3f, 13f, 0f, -9f, 1f, 0f, 2f));
		}
	}

	private IEnumerator AnimateScrollBar(HorizontalScrollDirections direction, float amount, float speedFactor)
	{
		float animationLerpFactor = 0f;
		float temporaryX = 0f;
		Transform characterScrollTrans = characterScrollAnchor.transform;
		Vector3 characterScrollLocalPosition = characterScrollTrans.localPosition;
		float startValue = characterScrollLocalPosition.x;
		float endValue = startValue + amount * (float)direction;
		if (skipButtonCollider != null)
		{
			skipButtonCollider.enabled = false;
		}
		while (animationLerpFactor < 1f)
		{
			animationLerpFactor += Time.deltaTime * speedFactor;
			temporaryX = Mathf.Lerp(startValue, endValue, animationLerpFactor);
			characterScrollTrans.localPosition = new Vector3(temporaryX, characterScrollLocalPosition.y, characterScrollLocalPosition.z);
			yield return null;
		}
		if (skipButtonCollider != null)
		{
			skipButtonCollider.enabled = true;
		}
	}

	private IEnumerator AnimateCharacter(CharacterScreenController characterController, float scaleStartValue, float scaleEndValue, float positionStartValue, float positionEndValue, float freezeStart, float freezeEnd, float speedFactor)
	{
		float animationLerpFactor = 0f;
		float newScale = 0f;
		float newYPosition = 0f;
		if (characterController != null)
		{
			Transform characterModelTrans = characterController.transform;
			Vector3 characterModelPosition = characterModelTrans.localPosition;
			while (animationLerpFactor < 1f)
			{
				animationLerpFactor += Time.deltaTime * speedFactor;
				characterController.Freeze = Mathf.Lerp(freezeStart, freezeEnd, animationLerpFactor);
				newScale = Mathf.Lerp(scaleStartValue, scaleEndValue, animationLerpFactor);
				newYPosition = Mathf.Lerp(positionStartValue, positionEndValue, animationLerpFactor);
				characterModelTrans.localScale = new Vector3(newScale, newScale, newScale);
				characterModelTrans.localPosition = new Vector3(characterModelPosition.x, newYPosition, characterModelPosition.z);
				yield return null;
			}
			characterController.Freeze = freezeEnd;
		}
	}

	private bool IsFilteredCharacter(char newChar)
	{
		char[] fILTERED_CHARACTERS = CharacterNameEventManager.FILTERED_CHARACTERS;
		foreach (char c in fILTERED_CHARACTERS)
		{
			if (newChar == c)
			{
				return true;
			}
		}
		return false;
	}

	public void HidePopupIfNeeded()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (_characterNameEventManager == null || activeEventManager == null || activeEventManager.GetEventID() != _characterNameEventManager.GetEventID())
		{
			UIScreenController.Instance.CloseActivePopups(screenName);
		}
	}
}
