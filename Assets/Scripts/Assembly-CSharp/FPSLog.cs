using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Kiloo.Common;
using Kiloo.HTTP;
using Kiloo.Plugins.DeviceUtility;
using SYBO.Subway;
using UnityEngine;

public class FPSLog
{
	private const int MAX_LOG_SIZE = 1000;

	private const string GOOGLE_SCRIPT_URL = "https://script.google.com/macros/s/AKfycbz2lBaGXHu05CgN1mXAr217sresz20BwK_3YwuHO2sHx_eJu5g/exec";

	private const string GOOGLE_SHEET_ID = "1d4AZJWVKm1c438UX21RZIMHGpzjRd013Bmk10_n5tmU";

	private const string WORKSHEET_NAME = "FPS_Data";

	private const string UPLOAD_SUCCESS_RESPONSE = "Upload Successful";

	private const string CONNECTION_TEST_SUCCESS_RESPONSE = "Connection Established";

	private const string GOOGLE_PARAMETER_KEY = "val";

	private const string SPREADSHEET_ID_PARAMETER_KEY = "ssid";

	private const string WORKSHEET_NAME_PARAMETER_KEY = "sheet";

	private const string ACTION_PARAMETER_KEY = "action";

	private const string GROUPBY_PARAMETER_KEY = "groupby";

	private const string ACTION_CONNECT_TEST_PARAMETER_VALUE = "ConnectionTest";

	private const string ACTION_UPLOAD_PARAMETER_VALUE = "POST";

	private const string DATA_DEFAULT_PARAMETER_KEY = "val";

	private const string DATA_DATE_PARAMETER_KEY = "date";

	private const string DATA_DEVICE_PARAMETER_KEY = "device";

	private const string DATA_NAME_PARAMETER_KEY = "name";

	private const string DATA_PERFORMANCE_PARAMETER_KEY = "performance";

	private const string DATA_OS_PARAMETER_KEY = "os";

	private const string DATA_LENGTH_PARAMETER_KEY = "length";

	private const string DATA_SSVERSION_PARAMETER_KEY = "ssversion";

	private const string DATA_RAWDATA_PARAMETER_KEY = "raw";

	private const string DATA_MINIMUM_PARAMETER_KEY = "min";

	private const string DATA_AVERAGE_PARAMETER_KEY = "avg";

	private const string DATA_MAXIMUM_PARAMETER_KEY = "max";

	private const string DATA_TARGETFRAMERATE_PARAMETER_KEY = "targetframerate";

	public const int DEFAULT_PERFORMANCE_DEVICE_FRAMERATE = 60;

	public const int LOW_PERFORMANCE_DEVICE_FRAMERATE = 20;

	public const int HIGH_PERFORMANCE_DEVICE_FRAMERATE = 40;

	public const int HIGH_PERFORMANCE_IOS_DEVICE_FRAMERATE = 30;

	public const int DEFAULT_LOGGING_FREQUENCY = 60;

	private static FPSLog _instance;

	private bool _logging;

	private int _frameCount;

	private int _fps;

	private double _timeElapsed;

	private double _totalTimeElapsed;

	private double _timeSinceLog;

	private double _timeToLog = 60.0;

	private StringBuilder _log = new StringBuilder();

	private int _autoRunLength = 600;

	private bool _useCustomFramerate;

	private int _targetFramerate = 60;

	private bool previousColliderState;

	private bool _listenersSetup;

	private int originalScreenTimeout;

	private IEnumerator _updateCoroutine;

	public static FPSLog instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new FPSLog();
			}
			return _instance;
		}
	}

	public bool IsLoggingFPS { get; private set; }

	public bool IsUploadingFPS { get; private set; }

	public void SetFPSUpload(bool value)
	{
		IsUploadingFPS = value;
	}

	public void SetFPSFramerate(int value = 0)
	{
		if (value == 0)
		{
			_useCustomFramerate = false;
			_targetFramerate = 60;
		}
		else
		{
			_useCustomFramerate = true;
			_targetFramerate = value;
		}
	}

	public void StartAutoFPS()
	{
		CoroutineUtil.StartCoroutine(AutoFPSTest());
		Game.OnMenuScreenShown = (Action)Delegate.Combine(Game.OnMenuScreenShown, new Action(ResetAutoFPS));
	}

	private void ResetAutoFPS()
	{
		Physics.IgnoreLayerCollision(Layers.Instance.Character, Layers.Instance.Hit, previousColliderState);
		SetFPSLogging(false);
		SetFPSUpload(false);
		SetFPSFramerate();
		Game.OnMenuScreenShown = (Action)Delegate.Remove(Game.OnMenuScreenShown, new Action(ResetAutoFPS));
	}

	public void SetFPSLogging(bool shouldEnable, int frequency = 60)
	{
		if (shouldEnable)
		{
			originalScreenTimeout = Screen.sleepTimeout;
			Screen.sleepTimeout = -1;
			IsLoggingFPS = true;
			SetupListeners(true);
			_timeToLog = frequency;
			_updateCoroutine = Update();
			CoroutineUtil.StartCoroutine(_updateCoroutine);
			if (!_useCustomFramerate)
			{
				switch (DeviceInfo.Instance.performanceLevel)
				{
				case DeviceInfo.PerformanceLevel.Low:
					Application.targetFrameRate = 20;
					break;
				case DeviceInfo.PerformanceLevel.High:
					if (Application.platform == RuntimePlatform.IPhonePlayer)
					{
						Application.targetFrameRate = 30;
					}
					else
					{
						Application.targetFrameRate = 40;
					}
					break;
				default:
					Application.targetFrameRate = 60;
					break;
				}
			}
			else
			{
				Application.targetFrameRate = _targetFramerate;
			}
		}
		else
		{
			Screen.sleepTimeout = originalScreenTimeout;
			IsLoggingFPS = false;
			SetupListeners(false);
			CoroutineUtil.StopCoroutine(_updateCoroutine);
			Application.targetFrameRate = 60;
		}
	}

	private void SetupListeners(bool connect)
	{
		if (connect)
		{
			if (!_listenersSetup)
			{
				Game.OnRunStarted = (Action)Delegate.Combine(Game.OnRunStarted, new Action(RunStarted));
				Game.OnMenuScreenShown = (Action)Delegate.Combine(Game.OnMenuScreenShown, new Action(RunEnded));
				Game game = Game.Instance;
				game.OnPauseChange = (Game.OnPauseChangeDelegate)Delegate.Combine(game.OnPauseChange, new Game.OnPauseChangeDelegate(PauseChanged));
				_listenersSetup = true;
			}
		}
		else if (_listenersSetup)
		{
			Game.OnRunStarted = (Action)Delegate.Remove(Game.OnRunStarted, new Action(RunStarted));
			Game.OnMenuScreenShown = (Action)Delegate.Remove(Game.OnMenuScreenShown, new Action(RunEnded));
			Game game2 = Game.Instance;
			game2.OnPauseChange = (Game.OnPauseChangeDelegate)Delegate.Remove(game2.OnPauseChange, new Game.OnPauseChangeDelegate(PauseChanged));
			_listenersSetup = false;
		}
	}

	private IEnumerator AutoFPSTest()
	{
		previousColliderState = Globals.DEBUG_CHARACTER_MOVE_THROUGH_COLLIDERS;
		Physics.IgnoreLayerCollision(Layers.Instance.Character, Layers.Instance.Hit, true);
		SetFPSLogging(true, _autoRunLength);
		SetFPSUpload(true);
		Game.Instance.StartNewRun(false);
		UIScreenController.Instance.PushScreen("IngameUI");
		UIScreenController.Instance.ClosePopup();
		UIScreenController.Instance.ClosePopup();
		yield return CoroutineUtil.StartCoroutine(CoroutineUtil.WaitForRealSeconds(_autoRunLength));
		UIScreenController.Instance.PushScreen("PauseUI");
		UIScreenController.Instance.GoToMainMenuFromGame(null);
	}

	private void RunStarted()
	{
		_log = new StringBuilder();
		_frameCount = 0;
		_fps = 0;
		_timeElapsed = 0.0;
		_totalTimeElapsed = 0.0;
		_timeSinceLog = 0.0;
		_logging = true;
	}

	private void RunEnded()
	{
		_logging = false;
		Log();
		if (IsUploadingFPS)
		{
			CoroutineUtil.StartCoroutine(Send2GoogleSheet(_log.ToString().TrimEnd(';')));
		}
	}

	private void PauseChanged(bool isPaused)
	{
		_logging = !isPaused;
	}

	private IEnumerator Update()
	{
		while (true)
		{
			if (_logging)
			{
				_frameCount++;
				_timeElapsed += Time.unscaledDeltaTime;
				_totalTimeElapsed += Time.unscaledDeltaTime;
				_timeSinceLog += Time.unscaledDeltaTime;
				if (_timeElapsed > 1.0)
				{
					_fps = (int)((double)_frameCount / _timeElapsed);
					_frameCount = 0;
					_timeElapsed = 0.0;
					_log.Append(_fps);
					_log.Append(';');
					if (_timeSinceLog > _timeToLog)
					{
						Log();
					}
				}
			}
			yield return null;
		}
	}

	private void Log()
	{
		_timeSinceLog = 0.0;
		Debug.Log("[FPSLog] RunLength: " + (int)_totalTimeElapsed);
		int num = Mathf.CeilToInt(_log.ToString().Length / 1000);
		for (int i = 0; i <= num; i++)
		{
			int startIndex = i * 1000;
			int num2 = (i + 1) * 1000;
			if (num2 < _log.Length)
			{
				Debug.Log("[FPSLog] " + _log.ToString().Substring(startIndex, 1000));
				continue;
			}
			Debug.Log("[FPSLog] " + _log.ToString().Substring(startIndex).TrimEnd(';'));
		}
	}

	public bool TestGoogleConnection()
	{
		IRequest request = WebClient.CreateGet(WebClient.AddQueryParametersToUri(new Uri("https://script.google.com/macros/s/AKfycbz2lBaGXHu05CgN1mXAr217sresz20BwK_3YwuHO2sHx_eJu5g/exec"), new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("ssid", "1d4AZJWVKm1c438UX21RZIMHGpzjRd013Bmk10_n5tmU"),
			new KeyValuePair<string, string>("sheet", "FPS_Data"),
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

	private IEnumerator Send2GoogleSheet(string dataPoints)
	{
		dataPoints = dataPoints.Remove(0, 2);
		if (dataPoints.Length > 60)
		{
			int[] values = (from x in dataPoints.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList()
				select int.Parse(x)).ToArray();
			IRequest requestPost = WebClient.CreateGet(WebClient.AddQueryParametersToUri(new Uri("https://script.google.com/macros/s/AKfycbz2lBaGXHu05CgN1mXAr217sresz20BwK_3YwuHO2sHx_eJu5g/exec"), new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("ssid", "1d4AZJWVKm1c438UX21RZIMHGpzjRd013Bmk10_n5tmU"),
				new KeyValuePair<string, string>("sheet", "FPS_Data"),
				new KeyValuePair<string, string>("action", "POST"),
				new KeyValuePair<string, string>("groupby", DeviceUtility.GetBundleVersion().Remove(4)),
				new KeyValuePair<string, string>("targetframerate", Application.targetFrameRate.ToString()),
				new KeyValuePair<string, string>("val", DateTime.Now.ToString(CultureInfo.InvariantCulture)),
				new KeyValuePair<string, string>("val", SystemInfo.deviceModel),
				new KeyValuePair<string, string>("val", SystemInfo.deviceName),
				new KeyValuePair<string, string>("val", DeviceInfo.Instance.performanceLevel.ToString()),
				new KeyValuePair<string, string>("val", Application.targetFrameRate.ToString()),
				new KeyValuePair<string, string>("val", SystemInfo.operatingSystem),
				new KeyValuePair<string, string>("val", string.Format("{0:c}", TimeSpan.FromSeconds(_totalTimeElapsed))),
				new KeyValuePair<string, string>("val", DeviceUtility.GetBundleVersion()),
				new KeyValuePair<string, string>("val", dataPoints),
				new KeyValuePair<string, string>("val", Mathf.Min(values).ToString()),
				new KeyValuePair<string, string>("val", ((float)values.ToList().Sum() / (float)values.Length).ToString("0.00")),
				new KeyValuePair<string, string>("val", Mathf.Max(values).ToString())
			})).WithIgnoreCertificateErrors(true).WithTimeout(5f);
			requestPost.acceptGzip = false;
			yield return WebClient.SendRequestAsync(requestPost);
			if (requestPost.response.ToString() == "Upload Successful")
			{
				UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get(StringID.UPLOAD_SUCCESS_MESSAGE));
			}
			else
			{
				UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get(StringID.UPLOAD_FAILED_MESSAGE));
			}
		}
	}
}
