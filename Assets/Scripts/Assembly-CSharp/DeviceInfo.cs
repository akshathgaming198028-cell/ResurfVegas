using System.Diagnostics;
using Kiloo.Common.Performance;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DeviceInfo
{
	public enum FormFactor
	{
		iPhone = 0,
		iPad = 1,
		small = 2,
		medium = 3,
		large = 4,
		iPhone5 = 5,
		iPhone6 = 6,
		iPhone6Plus = 7
	}

	public enum PerformanceLevel
	{
		Low = 0,
		Medium = 1,
		High = 2
	}

	private const int DEFAULT_ANDROID_PERFORMANCE_LEVEL_THRESHOLD_VALUE = 1000;

	private const int DEFAULT_WP8_PERFORMANCE_LEVEL_THRESHOLD_VALUE = 1400;

	private const int DEFAULT_KINDLE_PERFORMANCE_LEVEL_THRESHOLD_VALUE = 900;

	public PerformanceLevel performanceLevel;

	public string deviceModel;

	private static DeviceInfo _instance;

	public float dpi { get; private set; }

	public FormFactor formFactor { get; private set; }

	public bool isHighres { get; private set; }

	public int gpuScore { get; private set; }

	public GPUCategory gpuCategory { get; private set; }

	public static DeviceInfo Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new DeviceInfo();
			}
			return _instance;
		}
	}

	private DeviceInfo()
	{
		deviceModel = SystemInfo.deviceModel;
		isHighres = (float)Screen.height > 480f;
		CategoryThresholds thresholds = default(CategoryThresholds);
		switch (Application.platform)
		{
		case RuntimePlatform.Android:
			if (isKindleDevice())
			{
				thresholds.high = 900f;
			}
			else
			{
				thresholds.high = 1000f;
			}
			break;
		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.WP8Player:
			thresholds.high = 1400f;
			break;
		default:
			thresholds.high = 2.1474836E+09f;
			break;
		}
		AndroidGPUCategorizer.Initialize(thresholds, LogUnknownGPU);
		gpuScore = AndroidGPUCategorizer.gpuScore;
		gpuCategory = AndroidGPUCategorizer.gpuCategory;
		if (Screen.height < 500)
		{
			formFactor = FormFactor.small;
		}
		else if (Screen.height < 900)
		{
			formFactor = FormFactor.medium;
		}
		else
		{
			formFactor = FormFactor.large;
		}
		if (isTablet())
		{
			formFactor = FormFactor.iPad;
		}
		if (Screen.height >= 500 && Screen.width > 320)
		{
			isHighres = true;
		}
		else
		{
			isHighres = false;
		}
		dpi = Screen.dpi;
		if (dpi <= 0f)
		{
			dpi = 300f;
		}
		GPUCategory gPUCategory = gpuCategory;
		if (gPUCategory == GPUCategory.High)
		{
			performanceLevel = PerformanceLevel.High;
		}
		else
		{
			performanceLevel = PerformanceLevel.Low;
		}
		int graphicsOverride = Settings.optionGraphics;
		if (graphicsOverride == 1)
		{
			performanceLevel = PerformanceLevel.High;
		}
		else if (graphicsOverride == 2)
		{
			performanceLevel = PerformanceLevel.Low;
		}
	}

	public static bool isKindleDevice()
	{
		return false;
	}

	private bool isTablet()
	{
		float f = ((!(Screen.dpi > 0f)) ? ((float)Screen.width) : ((float)Screen.width / Screen.dpi));
		float f2 = ((!(Screen.dpi > 0f)) ? ((float)Screen.height) : ((float)Screen.height / Screen.dpi));
		double num = Mathf.Sqrt(Mathf.Pow(f, 2f) + Mathf.Pow(f2, 2f));
		return num >= 6.0;
	}

	private void SetDeviceInfoForUnknownIphone()
	{
		if ((float)Screen.height > 960f)
		{
			performanceLevel = PerformanceLevel.High;
		}
		else
		{
			performanceLevel = PerformanceLevel.Low;
		}
		formFactor = FormFactor.iPhone;
		isHighres = (float)Screen.height > 480f;
		dpi = 300f;
	}

	private void SetDeviceInfoForUnknownIpad()
	{
		if ((float)Screen.height > 1024f)
		{
			performanceLevel = PerformanceLevel.High;
		}
		else
		{
			performanceLevel = PerformanceLevel.Low;
		}
		formFactor = FormFactor.iPad;
		isHighres = (float)Screen.height > 480f;
		dpi = 264f;
	}

	private void LogUnknownGPU(string gpuName)
	{
		if (AndroidGPUCategorizer.IsSupportedOnPlatform())
		{
			Flurry.LogEventWithAParameter("Unknown GPU", "GPU name", gpuName);
		}
	}

	public bool IsDeviceCapablePerformance(PerformanceLevel perfLevel)
	{
		return perfLevel <= performanceLevel;
	}

	private static void LogError(string msg, Object context = null)
	{
		Debug.LogError("DeviceInfo: " + msg, context);
	}

	private static void LogWarning(string msg, Object context = null)
	{
		Debug.LogWarning("DeviceInfo: " + msg, context);
	}

	[Conditional("DEVICE_INFO_DEBUG_LOGS")]
	private static void Log(string msg, Object context = null)
	{
		Debug.Log("DeviceInfo: " + msg, context);
	}
}
