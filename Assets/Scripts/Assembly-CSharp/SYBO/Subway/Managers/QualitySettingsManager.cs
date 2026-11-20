using System;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities;
using UnityEngine;

namespace SYBO.Subway.Managers
{
	[PrefabPath("Prefabs/Managers/QualitySettingsManager")]
	public class QualitySettingsManager : SingletonMonobehavior<QualitySettingsManager>
	{
		public class QualityTypeChangedEventArgs : EventArgs
		{
			public QualityType QualityType { get; internal set; }

			public QualityTypeChangedEventArgs(QualityType qualityType)
			{
				QualityType = qualityType;
			}
		}

		private const string _QUALITY_LEVEL_NAME_GOOD = "Good";

		private const string _QUALITY_LEVEL_NAME_DEMO_VIDEO_MODE = "Demo Video Mode";

		public event EventHandler<QualityTypeChangedEventArgs> OnQualityTypeChanged;

		protected override void SingletonMonobehavior_OnInstantiateCompleted()
		{
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.DynamicManagers);
		}

		protected void Awake()
		{
			if (!DemoVideoModeManager.IsEnabled)
			{
			}
		}

		public void SetQualityType(QualityType qualityType)
		{
			switch (qualityType)
			{
			case QualityType.Default:
				SetQualityLevel("Good");
				break;
			case QualityType.DemoVideoMode:
				SetQualityLevel("Demo Video Mode");
				break;
			}
			RaiseOnQualityLevelChanged(new QualityTypeChangedEventArgs(qualityType));
		}

		private int GetQualityLevelIntFromName(string qualityLevelName)
		{
			string[] names = QualitySettings.names;
			for (int i = 0; i < names.Length; i++)
			{
				string text = names[i];
				if (text == qualityLevelName)
				{
					return i;
				}
			}
			return -1;
		}

		private bool HasQualityLevelIntFromName(string qualityLevelName)
		{
			return GetQualityLevelIntFromName(qualityLevelName) != -1;
		}

		private void SetQualityLevel(int qualityLevel)
		{
			QualitySettings.SetQualityLevel(qualityLevel, true);
			DebugLogQualitySettings("SetQualityLevel");
		}

		private void SetQualityLevel(string qualityLevelName)
		{
			if (HasQualityLevelIntFromName(qualityLevelName))
			{
				int qualityLevelIntFromName = GetQualityLevelIntFromName(qualityLevelName);
				SetQualityLevel(qualityLevelIntFromName);
			}
			else
			{
				Debug.LogError("No qualityLevelInt available for qualityLevelName: " + qualityLevelName);
			}
		}

		private void DebugLogQualitySettings(string callingMethodName)
		{
			int qualityLevel = QualitySettings.GetQualityLevel();
			Debug.LogWarning("[QualitySettingsManager]." + callingMethodName + "() QualityName: " + QualitySettings.names[qualityLevel] + " (" + qualityLevel + ")");
		}

		private void RaiseOnQualityLevelChanged(QualityTypeChangedEventArgs e)
		{
			if (this.OnQualityTypeChanged != null)
			{
				this.OnQualityTypeChanged(this, e);
			}
		}
	}
}
