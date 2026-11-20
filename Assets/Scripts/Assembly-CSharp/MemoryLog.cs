using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Kiloo.Common;
using Kiloo.HTTP;
using Kiloo.Plugins.DeviceUtility;
using SYBO.Subway;
using UnityEngine;

public class MemoryLog
{
	private const float POPUP_DELAY = 0.2f;

	private const float POPUP_DELAY_CLOSE = 0.2f;

	private const float SCREEN_DELAY = 0.2f;

	private const float SCREEN_DELAY_CLOSE = 0.2f;

	private const int TOTAL_REPEAT_COUNT = 3;

	private const int UI_REPEAT_COUNT = 10;

	private const int UI_LOG_SKIP = 4;

	private const int GAME_LOG_INTERVAL = 6;

	private const int GAME_LOG_COUNT = 30;

	private const char LOG_SPLITTER = ';';

	private const string GOOGLE_SCRIPT_URL = "https://script.google.com/macros/s/AKfycbz2lBaGXHu05CgN1mXAr217sresz20BwK_3YwuHO2sHx_eJu5g/exec";

	private const string GOOGLE_SHEET_ID = "1d4AZJWVKm1c438UX21RZIMHGpzjRd013Bmk10_n5tmU";

	private const string WORKSHEET_NAME = "Memory_Data";

	private const string UPLOAD_SUCCESS_RESPONSE = "Upload Successful";

	private const string CONNECTION_TEST_SUCCESS_RESPONSE = "Connection Established";

	private const string GOOGLE_PARAMETER_KEY = "val";

	private const string SPREADSHEET_ID_PARAMETER_KEY = "ssid";

	private const string WORKSHEET_NAME_PARAMETER_KEY = "sheet";

	private const string ACTION_PARAMETER_KEY = "action";

	private const string ACTION_CONNECT_TEST_PARAMETER_VALUE = "ConnectionTest";

	private const string ACTION_UPLOAD_PARAMETER_VALUE = "POST";

	private readonly bool RUN_UI_BENCHMARK = true;

	private readonly bool RUN_GAME_BENCHMARK = true;

	private readonly bool UPLOAD_TO_GOOGLE = true;

	private readonly bool VERBOSE_LOG;

	private readonly bool INCLUDE_LOG_DETAILS;

	private StringBuilder _logMemory;

	private List<string> _rawDataLogs = new List<string>();

	private int _uiCurrentLogSkipCount;

	private int _startMemory;

	private int _endMemory;

	private int _minMemory;

	private int _maxMemory;

	private float _progressIncreaseGame;

	private float _progressIncreaseUI;

	private float _currentProgress;

	private static MemoryLog _instance;

	public static bool benchmarkInProgress { get; private set; }

	public static MemoryLog Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new MemoryLog();
			}
			return _instance;
		}
	}

	private bool TestGoogleConnection()
	{
		IRequest request = WebClient.CreateGet(WebClient.AddQueryParametersToUri(new Uri("https://script.google.com/macros/s/AKfycbz2lBaGXHu05CgN1mXAr217sresz20BwK_3YwuHO2sHx_eJu5g/exec"), new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("ssid", "1d4AZJWVKm1c438UX21RZIMHGpzjRd013Bmk10_n5tmU"),
			new KeyValuePair<string, string>("sheet", "Memory_Data"),
			new KeyValuePair<string, string>("action", "ConnectionTest")
		})).WithIgnoreCertificateErrors(true).WithTimeout(5f);
		request.acceptGzip = false;
		request = WebClient.SendRequest(request);
		if (request.isSuccessful && request.response.ToString() == "Connection Established")
		{
			return true;
		}
		return false;
	}

	private IEnumerator UploadToGoogleSheets()
	{
		List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("ssid", "1d4AZJWVKm1c438UX21RZIMHGpzjRd013Bmk10_n5tmU"),
			new KeyValuePair<string, string>("sheet", "Memory_Data"),
			new KeyValuePair<string, string>("action", "POST"),
			new KeyValuePair<string, string>("val", DateTime.Now.ToString(CultureInfo.InvariantCulture)),
			new KeyValuePair<string, string>("val", SystemInfo.deviceModel),
			new KeyValuePair<string, string>("val", SystemInfo.deviceName),
			new KeyValuePair<string, string>("val", SystemInfo.operatingSystem),
			new KeyValuePair<string, string>("val", DeviceUtility.GetBundleVersion())
		};
		for (int i = 0; i < _rawDataLogs.Count; i++)
		{
			parameters.Add(new KeyValuePair<string, string>("val", _rawDataLogs[i]));
		}
		IRequest requestPost = WebClient.CreateGet(WebClient.AddQueryParametersToUri(new Uri("https://script.google.com/macros/s/AKfycbz2lBaGXHu05CgN1mXAr217sresz20BwK_3YwuHO2sHx_eJu5g/exec"), parameters)).WithIgnoreCertificateErrors(true).WithTimeout(5f);
		requestPost.acceptGzip = false;
		yield return WebClient.SendRequestAsync(requestPost);
		if (requestPost.isSuccessful && requestPost.response.ToString() == "Upload Successful")
		{
			UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get(StringID.UPLOAD_SUCCESS_MESSAGE));
		}
		else
		{
			UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get(StringID.UPLOAD_FAILED_MESSAGE));
		}
	}

	private Coroutine StartPushPopup(string popup)
	{
		return CoroutineUtil.StartCoroutine(PushPopup(popup));
	}

	private Coroutine StartPushScreen(string screen, float extraDelay = 0f, bool closeAfterwards = true, bool logMemory = true)
	{
		return CoroutineUtil.StartCoroutine(PushScreen(screen, extraDelay, closeAfterwards, logMemory));
	}

	private Coroutine StartWaitForRealSeconds(float delay)
	{
		return CoroutineUtil.StartCoroutine(CoroutineUtil.WaitForRealSeconds(delay));
	}

	public void PromptStartMemoryBenchmark()
	{
		if (!benchmarkInProgress)
		{
			string text = "This benchmark will take around 15 minutes to complete.\nPlease make sure the device has at least 20% battery or is charging.\nAll input will be disabled while the test is running!\n\n";
			string text2 = string.Format("Test details:\nUI Repeat: {0} times\nGame Length: {1}\nTotal repeat: {2} times\nUpload results: {3}", 10, GetFormattedTime(180), 3, (!UPLOAD_TO_GOOGLE || INCLUDE_LOG_DETAILS) ? "No" : "Yes");
			DeviceUtility.ShowNativePopup("Confirm Benchmark", text + text2, "Cancel", "OK", MemoryBenchmarkPopupDismissed);
		}
		else
		{
			UIScreenController.Instance.QueueErrorMessageSlidein("Benchmark already running.");
		}
	}

	private void MemoryBenchmarkPopupDismissed(string popupResult)
	{
		if (popupResult == "OK")
		{
			CoroutineUtil.StartCoroutine(RunMemoryBenchmark());
		}
	}

	private IEnumerator RunMemoryBenchmark()
	{
		if (benchmarkInProgress)
		{
			yield break;
		}
		benchmarkInProgress = true;
		UIScreenController.Instance.ShowMemoryBenchmarkOverlay();
		UIScreenController.Instance.UpdateMemoryBenchmarkPercent(0f);
		yield return StartWaitForRealSeconds(0.2f);
		if (UPLOAD_TO_GOOGLE && !INCLUDE_LOG_DETAILS && !TestGoogleConnection())
		{
			UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get(StringID.CONNECTION_TEST_FAILED_ERROR_MESSAGE));
			benchmarkInProgress = false;
			UIScreenController.Instance.HideMemoryBenchmarkOverlay();
			yield break;
		}
		int originalScreenTimeout = Screen.sleepTimeout;
		Screen.sleepTimeout = -1;
		_rawDataLogs.Clear();
		_logMemory = new StringBuilder();
		_startMemory = LogCurrentMemoryUsage();
		_minMemory = _startMemory;
		_maxMemory = 0;
		_endMemory = 0;
		_progressIncreaseUI = 1f / 30f * ((!RUN_GAME_BENCHMARK) ? 1f : 0.25f);
		_progressIncreaseGame = 1f / 90f * ((!RUN_UI_BENCHMARK) ? 1f : 0.75f);
		_currentProgress = 0f;
		if (VERBOSE_LOG)
		{
		}
		yield return StartPushScreen("FrontUI", 0f, false, false);
		UIScreenController.Instance.ClosePopup();
		UIScreenController.Instance.ClosePopup();
		UIScreenController.Instance.ClosePopup();
		yield return StartWaitForRealSeconds(1f);
		for (int i = 0; i < 3; i++)
		{
			if (RUN_UI_BENCHMARK)
			{
				for (int j = 0; j < 10; j++)
				{
					int startLogIndex = _logMemory.Length;
					yield return CoroutineUtil.StartCoroutine(UIBenchmark());
					int logLength = _logMemory.Length - startLogIndex;
				}
			}
			DumpRawData();
			if (RUN_GAME_BENCHMARK)
			{
				int startLogIndex2 = _logMemory.Length;
				yield return CoroutineUtil.StartCoroutine(GameBenchmark());
				int logLength2 = _logMemory.Length - startLogIndex2;
				string time = GetFormattedTime(180);
			}
			if (i + 1 == 3)
			{
				_endMemory = LogCurrentMemoryUsage();
			}
			DumpRawData();
		}
		StringBuilder logResults = new StringBuilder();
		logResults.AppendFormat("Start: {0} MB", _startMemory);
		logResults.AppendLine();
		logResults.AppendFormat("End: {0}", _endMemory);
		logResults.AppendLine();
		int difference = _maxMemory - _minMemory;
		logResults.AppendFormat("Change: {0}{1}", (difference <= 0) ? string.Empty : "+", difference);
		logResults.AppendLine();
		logResults.AppendFormat("Min: {0}", _minMemory);
		logResults.AppendLine();
		logResults.AppendFormat("Max: {0}", _maxMemory);
		string[] results = logResults.ToString().Split('\n');
		for (int k = 0; k < results.Length; k++)
		{
		}
		for (int l = 0; l < _rawDataLogs.Count; l++)
		{
		}
		DeviceUtility.ShowNativePopup("Benchmark results", logResults.ToString(), "OK");
		if (UPLOAD_TO_GOOGLE && !INCLUDE_LOG_DETAILS)
		{
			yield return CoroutineUtil.StartCoroutine(UploadToGoogleSheets());
		}
		Screen.sleepTimeout = originalScreenTimeout;
		benchmarkInProgress = false;
		UIScreenController.Instance.HideMemoryBenchmarkOverlay();
		UIScreenController.Instance.PushPopup("SettingsPopup");
		UIScreenController.Instance.PushPopup("DebugSettingsPopup");
	}

	private IEnumerator PushPopup(string popup)
	{
		UIScreenController.Instance.PushPopup(popup);
		yield return StartWaitForRealSeconds(0.2f);
		if (!ShouldSkipLog())
		{
			LogCurrentMemoryUsage(popup);
		}
		UIScreenController.Instance.CloseActivePopups(popup);
		yield return StartWaitForRealSeconds(0.2f);
	}

	private IEnumerator PushScreen(string screen, float extraDelay = 0f, bool closeAfterwards = true, bool logMemory = true)
	{
		UIScreenController.Instance.PushScreen(screen);
		yield return StartWaitForRealSeconds(0.2f + extraDelay);
		if (logMemory && !ShouldSkipLog())
		{
			LogCurrentMemoryUsage(screen);
		}
		if (closeAfterwards)
		{
			UIScreenController.Instance.BackToPrevious();
			yield return StartWaitForRealSeconds(0.2f);
		}
		UIScreenController.Instance.ClosePopup();
	}

	private IEnumerator UIBenchmark()
	{
		if (!Globals.DEBUG_USE_CELEBRATION_TESTER && RewardManager.rewardsToUnlockCount <= 0)
		{
			CelebrationReward celebrationReward = new CelebrationReward();
			celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.MysteryBox;
			celebrationReward.amount = 1;
			celebrationReward.rewardType = CelebrationRewardType.coins;
			RewardManager.AddRewardToUnlock(celebrationReward, false);
		}
		yield return StartPushPopup("Mission_popup");
		yield return StartPushPopup("DailyChallengePopup");
		yield return StartPushPopup("ItemHuntPopup");
		yield return StartPushPopup("HoverboardPopup");
		yield return StartPushPopup("SettingsPopup");
		yield return StartPushPopup("EventsPopup");
		yield return StartPushPopup("TopRunPopup");
		yield return StartPushPopup("CelebrationPopup");
		yield return StartPushPopup("CoinsUI_quick");
		yield return StartPushPopup("CountryPopup");
		yield return StartPushPopup("FacebookRewardsPopup");
		yield return StartPushPopup("UpdateAppPopup");
		yield return StartPushScreen("CharacterScreen", 0f);
		yield return StartPushScreen("AwardsScreen", 0f);
		yield return StartPushScreen("BoardScreen", 0f);
		yield return StartPushScreen("CoinsUI_shop", 0f);
		yield return StartPushScreen("EarnCoinsScreen", 0f);
		yield return StartPushScreen("FriendsUI", 0.2f);
		yield return StartPushScreen("GameoverUI", 0f);
		yield return StartPushScreen("LeaderboardUI", 0.2f);
		yield return StartPushScreen("TopRunUI", 0.2f);
		yield return StartPushScreen("UpgradesUI_shop", 0f);
		IncreaseAndUpdateProgress(_progressIncreaseUI);
	}

	private IEnumerator GameBenchmark()
	{
		bool colliderState = Globals.DEBUG_CHARACTER_MOVE_THROUGH_COLLIDERS;
		Physics.IgnoreLayerCollision(Layers.Instance.Character, Layers.Instance.Hit, true);
		Game.Instance.StartNewRun(false);
		UIScreenController.Instance.PushScreen("IngameUI");
		LogCurrentMemoryUsage(GetFormattedTime(0));
		for (int i = 0; i < 30; i++)
		{
			yield return StartWaitForRealSeconds(6f);
			LogCurrentMemoryUsage(GetFormattedTime(6 * (i + 1)));
			IncreaseAndUpdateProgress(_progressIncreaseGame);
		}
		if (Game.Instance.IsInJetpackMode)
		{
			while (Game.Instance.IsInJetpackMode)
			{
				yield return null;
			}
			yield return StartWaitForRealSeconds(2f);
		}
		Game.Instance.ChangeState(null, Game.GameStateCoroutineType.DieSequence);
		yield return StartWaitForRealSeconds(10f);
		yield return StartPushScreen("FrontUI", 0f, false);
		Physics.IgnoreLayerCollision(Layers.Instance.Character, Layers.Instance.Hit, colliderState);
	}

	private string GetFormattedTime(int time)
	{
		return string.Format("{0}m{1}s", Mathf.FloorToInt((float)time / 60f), time % 60);
	}

	private bool ShouldSkipLog()
	{
		if (INCLUDE_LOG_DETAILS || VERBOSE_LOG)
		{
			return false;
		}
		if (_uiCurrentLogSkipCount <= 0)
		{
			_uiCurrentLogSkipCount = 4;
			return false;
		}
		_uiCurrentLogSkipCount--;
		return true;
	}

	private void DumpRawData()
	{
		string item = _logMemory.ToString();
		_rawDataLogs.Add(item);
		_logMemory.Remove(0, _logMemory.Length);
	}

	private void IncreaseAndUpdateProgress(float deltaProgress)
	{
		_currentProgress += deltaProgress;
		UIScreenController.Instance.UpdateMemoryBenchmarkPercent(_currentProgress);
	}

	private int LogCurrentMemoryUsage(string details = null)
	{
		int currentMemoryUsage = DeviceUtility.GetCurrentMemoryUsage();
		if (currentMemoryUsage > _maxMemory)
		{
			_maxMemory = currentMemoryUsage;
		}
		else if (currentMemoryUsage < _minMemory)
		{
			_minMemory = currentMemoryUsage;
		}
		LogMemory(currentMemoryUsage, details);
		return currentMemoryUsage;
	}

	private void LogMemory(int memoryValue, string details = null)
	{
		if (INCLUDE_LOG_DETAILS && !string.IsNullOrEmpty(details))
		{
			_logMemory.AppendFormat("{0}=", details);
		}
		_logMemory.Append(memoryValue);
		_logMemory.Append(';');
		if (!VERBOSE_LOG)
		{
		}
	}

	[Conditional("ENABLE_MEMORY_LOGS")]
	private void Log(string text)
	{
		Debug.Log(string.Format("[Memory Benchmark] {0}", text));
	}
}
