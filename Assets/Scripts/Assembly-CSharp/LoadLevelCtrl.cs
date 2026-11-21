using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.SceneManagement;

public class LoadLevelCtrl : MonoBehaviour
{
	[SerializeField]
	private GameObject backrender;

	private AsyncOperation asyncMerge;

	private GameObject notebook;

	[SerializeField]
	private UILabel titleTipLabel;

	[SerializeField]
	private UILabel tipLabel;

	[SerializeField]
	private UISlider slider;

	[SerializeField]
	private UISprite foreground;

	[SerializeField]
	private UITexture splash;

	[SerializeField]
	private Transform backgroundAnchor;

	private float sliderSteps;

	private bool sliderStepsIsSet;

	[SerializeField]
	private float inspectorProgress;

	private string[] tipArray = new string[37]
	{
		"Stay on top of trains\nto get a better view\nof what's coming!", "Pick up the 2X Multiplier\nto double the score you earn!", "Jump barriers, dodge trains\n- and don't let\nthe Inspector catch you!", "Surf the trains\nas far as possible\nto set a new highscore!", "Get the Double Coins Booster\nto increase every coin pickup!", "Find your favorite\ncharacters in the Me menu!", "Complete Daily Challenges\neach day and\nincrease your reward!", "Upgrade your preferred\npowerups in the Shop\nto make them last longer!", "Join your friends on Facebook\nand show off your skills!", "Complete missions\nto increase your Multiplier\nup to x30!",
		"Reached Multiplier 30?\nComplete extra missions\nfor Super Mystery Boxes!", "Earn coins for your friends\nby making multiple runs!", "Go to the Friends screen\nto get coin bonuses\nfrom your friends!", "Switch lanes in mid-air\nto dodge trains\nat the last minute!", "Double tap the screen\nto grab a Hoverboard!", "Use Hoverboards\nto avoid crashing!", "Discover secret Trophies\nin Mystery Boxes!", "Reach the top of\nthe leaderboard by\nincreasing your Multiplier!", "Open Mystery Boxes\n- go for the Jackpot!", "Catch those letters\nto complete the Daily Challenge!",
		"Surf the trains\nfor a better view!", "Put on your\npaint powered Jetpack\nto glide with style!", "The 2X Multiplier\ndoubles your Multiplier\ntemporarily!", "Skip Missions in the Shop!", "Use the bouncy\nSuper Sneakers\nto jump over trains!", "Swipe down in mid-air\nto master the Super Sneakers!", "Use Headstarts\nfor a helping hand!", "Unlock Tricky\nby collecting specific\nCharacter Tokens!", "Unlock Fresh\nby collecting specific\nCharacter Tokens!", "Unlock Spike\nby collecting specific\nCharacter Tokens!",
		"Unlock Yutani\nby collecting specific\nCharacter Tokens!", "Reach goals to win\nBronze, Silver and\nGold Awards", "Collect rare Trophies\nfrom Mystery Boxes\nand Super Mystery Boxes", "Swipe down at any time\nduring a Power Jump\nto land immediately", "Join the Wild Wednesday fun\nevery week", "Collect surprising\nSuper Mysterizers\non Mystery Mondays", "Find Awards\nin the Me menu"
	};

	private float loadingBarBundlesPercerntage = 0.5f;

	private float progress;

	private bool sendDebug;

	public static float continueTime = 99999f;

	private float fakeProgress
	{
		get
		{
			float num = 0f;
			num += progress * 0.5f;
			if (progress > 0.74f)
			{
				num += (progress - 0.74f) * 3.7f;
			}
			return num;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.transform.gameObject);
	}

	private void Update()
	{
		LoadBar();
	}

	private string GetTip()
	{
		return tipArray[Random.Range(0, tipArray.Length)];
	}

	private IEnumerator Start()
	{
		titleTipLabel.text = Strings.Get(StringID.LOAD_LEVEL_CTRL_TIP_MESSAGE);
		tipLabel.text = GetTip();
		if (tipLabel.panel != null)
		{
			tipLabel.panel.Refresh();
		}
		AssetBundleManager assetBundleManager = AssetBundleManager.Instance;
		IEnumerator cacheCoroutine = assetBundleManager.CacheNeededAssetBundles();
		progress = 0f;
		while (cacheCoroutine.MoveNext())
		{
			progress = (float)cacheCoroutine.Current * loadingBarBundlesPercerntage;
			yield return null;
		}
		progress = loadingBarBundlesPercerntage;
		asyncMerge = SceneManager.LoadSceneAsync(MergeLevelStartHelp.SCENE_NAME, LoadSceneMode.Additive);
		StartCoroutine(Progress());
		yield return asyncMerge;
	}

	private IEnumerator Progress()
	{
		while (!asyncMerge.isDone)
		{
			if (asyncMerge != null)
			{
				progress += asyncMerge.progress * (1f - loadingBarBundlesPercerntage);
			}
			if (progress > 0.89f && !sendDebug)
			{
				sendDebug = true;
			}
			yield return null;
		}
		continueTime = 0f;
		yield return null;
		Object.Destroy(base.gameObject);
		UnloadAssets();
	}

	public float LoadBar()
	{
		if (!sliderStepsIsSet)
		{
			sliderSteps = slider.backgroundWidget.width;
			sliderStepsIsSet = true;
		}
		slider.value = fakeProgress * sliderSteps / sliderSteps;
		return slider.value;
	}

	private void UnloadAssets()
	{
		Resources.UnloadAsset(splash.mainTexture);
		Object.Destroy(splash);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogError(string msg, Object context = null)
	{
		Debug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogWarning(string msg, Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("ENABLE_LOADLEVEL_LOGS")]
	public static void Log(string msg, Object context = null)
	{
		Debug.Log(msg, context);
	}
}
