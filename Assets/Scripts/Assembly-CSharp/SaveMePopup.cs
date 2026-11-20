using System.Collections;
using UnityEngine;

public class SaveMePopup : UIScreen
{
	private const float ANIMATION_DURATION = 3f;

	[SerializeField]
	private UIWidget _outline;

	public SaveMeAnimateClock saveMeAnimateClock;

	private float timeLeft;

	[SerializeField]
	private UILabel saveMeButtonLabel;

	public override void Init()
	{
		base.Init();
		InitializeCoinbox(_outline, false, false, true, UIScreenController.SAVEME_POPUP_POSITION.x, UIScreenController.SAVEME_POPUP_POSITION.y);
	}

	private void Awake()
	{
		saveMeButtonLabel.text = Strings.Get(StringID.SAVE_ME_POPUP_BUTTON_LABEL);
	}

	private void OnEnable()
	{
		StartCoroutine(startClockAnimation());
		UIScreenController instance = UIScreenController.Instance;
		IngameScreen ingameScreen = instance.GetScreenFromCache(instance.GetTopScreenName()) as IngameScreen;
		if (ingameScreen == null)
		{
			Debug.LogError("IngameScreen == NULL");
		}
		else
		{
			ingameScreen.SetPauseButtonVisibility(false);
		}
	}

	public float getAnimationTimeLeft()
	{
		return timeLeft;
	}

	public float getAnimationDuration()
	{
		return 3f;
	}

	private IEnumerator startClockAnimation()
	{
		timeLeft = 3f;
		while (timeLeft > -0.2f)
		{
			float spriteAmount = Mathf.Clamp01(timeLeft / 3f);
			saveMeAnimateClock.FillSpriteAmount(spriteAmount);
			timeLeft -= Time.deltaTime;
			yield return null;
		}
		UIScreenController.Instance.CloseActivePopups(screenName);
		Revive.Instance.SendSkipRevive();
		SaveMeManager.ResetSaveMeForNewRun();
	}
}
