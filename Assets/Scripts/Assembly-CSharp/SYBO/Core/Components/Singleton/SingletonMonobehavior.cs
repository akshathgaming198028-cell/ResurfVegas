using System;
using System.Reflection;
using UnityEngine;

namespace SYBO.Core.Components.Singleton
{
	public abstract class SingletonMonobehavior<T> : MonoBehaviour where T : MonoBehaviour
	{
		public delegate void OnInstantiateCompletedDelegate(T instance);

		private static T _Instance;

		public static OnInstantiateCompletedDelegate OnInstantiateCompleted;

		public static T Instance
		{
			get
			{
				if (!IsInstantiated())
				{
					Instantiate();
				}
				return _Instance;
			}
			set
			{
				_Instance = value;
			}
		}

		public static bool IsInstantiated()
		{
			if (_Instance == null && Application.isEditor && !Application.isPlaying)
			{
				_Instance = UnityEngine.Object.FindObjectOfType<T>();
			}
			return _Instance != null;
		}

		public static T Instantiate()
		{
			if (!IsInstantiated())
			{
				if (Utils.HasObject<T>())
				{
					_Instance = Utils.FindObject<T>();
				}
				if (_Instance == null)
				{
					GameObject gameObject = null;
					Type typeFromHandle = typeof(T);
					if (Attribute.IsDefined(typeFromHandle, typeof(PrefabPathAttribute)))
					{
						PrefabPathAttribute prefabPathAttribute = (PrefabPathAttribute)Attribute.GetCustomAttribute(typeFromHandle, typeof(PrefabPathAttribute));
						string text = prefabPathAttribute.Name;
						try
						{
							gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load(text, typeof(GameObject)));
						}
						catch (Exception ex)
						{
							Debug.LogError("Could not instantiate prefab of " + text + ": " + ex.Message + "\n" + ex.StackTrace);
							Debug.LogWarning("Creating empty game object for singleton MonoBehavior " + typeof(T).ToString());
							gameObject = new GameObject();
						}
					}
					else
					{
						gameObject = new GameObject();
					}
					_Instance = gameObject.GetComponent<T>();
					if (_Instance == null)
					{
						_Instance = gameObject.AddComponent<T>();
					}
					gameObject.name = _Instance.GetType().Name;
					if (Application.isEditor && !Application.isPlaying)
					{
						gameObject.hideFlags = HideFlags.DontSave;
					}
					if (Application.isPlaying)
					{
						UnityEngine.Object.DontDestroyOnLoad(gameObject);
					}
					// Direct virtual call. Reflection on the base type ran the empty base method under IL2CPP, so subclass setup (SpawnPoolManager) never ran.
					((SingletonMonobehavior<T>)_Instance).SingletonMonobehavior_OnInstantiateCompleted();
					if (OnInstantiateCompleted != null)
					{
						OnInstantiateCompleted(_Instance);
					}
				}
			}
			return _Instance;
		}

		public static void Destroy()
		{
			if (IsInstantiated())
			{
				UnityEngine.Object.DestroyImmediate(_Instance.gameObject);
				_Instance = (T)null;
			}
		}

		protected virtual void SingletonMonobehavior_OnInstantiateCompleted()
		{
		}
	}
}
