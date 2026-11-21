using System;
using System.Diagnostics;
using JetBrains.Annotations;
using SYBO.Core.Utilities;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Common
{
	public class ApplicationState : MonoBehaviour
	{
		private static ApplicationState _instance;

		private static bool _isQuitting;

		private Action _resumed;

		private Action _paused;

		private Action _focused;

		private Action _unfocused;

		private Action _quit;

		private bool _isPaused;

		private bool _hasFocus = true;

		public static bool isPaused
		{
			get
			{
				return instance != null && instance._isPaused;
			}
		}

		public static bool hasFocus
		{
			get
			{
				return instance != null && instance._hasFocus;
			}
		}

		public static bool isQuitting
		{
			get
			{
				return instance == null;
			}
		}

		private static ApplicationState instance
		{
			get
			{
				if (!Application.isPlaying)
				{
					throw new Exception("Can only be used in play mode");
				}
				if (_instance == null)
				{
					if (Application.isEditor)
					{
						ApplicationState applicationState;
						while ((applicationState = UnityEngine.Object.FindObjectOfType<ApplicationState>()) != null)
						{
							UnityEngine.Object.DestroyImmediate(applicationState.gameObject);
						}
					}
					if (_isQuitting)
					{
						LogWarning("Cannot create instance while quitting");
					}
					else
					{
						_instance = new GameObject(typeof(ApplicationState).Name).AddComponent<ApplicationState>();
						UnityEngine.Object.DontDestroyOnLoad(_instance.gameObject);
						_instance.gameObject.hideFlags = HideFlags.DontSave;
						HierarchyUtility.ReparentByHierarchyPathType(_instance.transform, HierarchyPathType.DynamicManagers);
					}
				}
				return _instance;
			}
		}

		public static event Action paused
		{
			add
			{
				if (instance != null)
				{
					ApplicationState applicationState = instance;
					applicationState._paused = (Action)Delegate.Combine(applicationState._paused, value);
				}
			}
			remove
			{
				if (instance != null)
				{
					ApplicationState applicationState = instance;
					applicationState._paused = (Action)Delegate.Remove(applicationState._paused, value);
				}
			}
		}

		public static event Action resumed
		{
			add
			{
				if (instance != null)
				{
					ApplicationState applicationState = instance;
					applicationState._resumed = (Action)Delegate.Combine(applicationState._resumed, value);
				}
			}
			remove
			{
				if (instance != null)
				{
					ApplicationState applicationState = instance;
					applicationState._resumed = (Action)Delegate.Remove(applicationState._resumed, value);
				}
			}
		}

		public static event Action focused
		{
			add
			{
				if (instance != null)
				{
					ApplicationState applicationState = instance;
					applicationState._focused = (Action)Delegate.Combine(applicationState._focused, value);
				}
			}
			remove
			{
				if (instance != null)
				{
					ApplicationState applicationState = instance;
					applicationState._focused = (Action)Delegate.Remove(applicationState._focused, value);
				}
			}
		}

		public static event Action unfocused
		{
			add
			{
				if (instance != null)
				{
					ApplicationState applicationState = instance;
					applicationState._unfocused = (Action)Delegate.Combine(applicationState._unfocused, value);
				}
			}
			remove
			{
				if (instance != null)
				{
					ApplicationState applicationState = instance;
					applicationState._unfocused = (Action)Delegate.Remove(applicationState._unfocused, value);
				}
			}
		}

		public static event Action quit
		{
			add
			{
				if (instance != null)
				{
					ApplicationState applicationState = instance;
					applicationState._quit = (Action)Delegate.Combine(applicationState._quit, value);
				}
			}
			remove
			{
				if (instance != null)
				{
					ApplicationState applicationState = instance;
					applicationState._quit = (Action)Delegate.Remove(applicationState._quit, value);
				}
			}
		}

		[UsedImplicitly]
		private void OnApplicationPause(bool paused)
		{
			_isPaused = paused;
			Action action = ((!_isPaused) ? _resumed : _paused);
			if (action != null)
			{
				action();
			}
		}

		[UsedImplicitly]
		private void OnApplicationFocus(bool inFocus)
		{
			_hasFocus = inFocus;
			Action action = ((!_hasFocus) ? _unfocused : _focused);
			if (action != null)
			{
				action();
			}
		}

		[UsedImplicitly]
		private void OnApplicationQuit()
		{
			_isQuitting = true;
			Action action = _quit;
			if (action != null)
			{
				action();
			}
			if (Application.isEditor)
			{
				UnityEngine.Object.DestroyImmediate(base.gameObject);
			}
		}

		private static void LogWarning(string msg)
		{
			Debug.LogWarning("[ApplicationState] " + msg);
		}

		[Conditional("APPLICATION_STATE_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[ApplicationState] " + msg);
		}
	}
}
