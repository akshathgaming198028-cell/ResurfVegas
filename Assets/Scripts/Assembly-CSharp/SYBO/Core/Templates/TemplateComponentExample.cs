using System;
using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Core.Templates
{
	public class TemplateComponentExample : MonoBehaviour
	{
		public enum Importance
		{
			None = 0,
			Low = 1,
			Medium = 2,
			High = 3
		}

		public struct Data
		{
			public Importance Importance;

			public GameObject GameObject;
		}

		public class OnCreatedGameObjectEventArgs : EventArgs
		{
			public readonly GameObject GameObject;

			public OnCreatedGameObjectEventArgs(GameObject gameObject)
			{
				GameObject = gameObject;
			}
		}

		public const int MaxChildCount = 5;

		public static readonly string NamePrefix = "StandardizedSpawner_";

		[SerializeField]
		private int _childCount;

		[SerializeField]
		private string _childName;

		[Range(0f, 1f)]
		[SerializeField]
		private float _childVerticalOffset;

		private bool _initialized;

		protected List<GameObject> _children;

		public static int CountOfInstances;

		private static TemplateComponentExample _lastCreatedInstance;

		public bool IsDrawingGizmos { get; set; }

		public int ChildCount
		{
			get
			{
				return _childCount;
			}
		}

		public string Name
		{
			get
			{
				return base.name;
			}
			set
			{
				base.name = NamePrefix + value;
			}
		}

		public int ActiveChildrenCount
		{
			get
			{
				int num = 0;
				foreach (Transform item in base.transform)
				{
					if (item.gameObject.activeSelf)
					{
						num++;
					}
				}
				return num;
			}
		}

		public static bool IsLastInstanceAlive
		{
			get
			{
				return _lastCreatedInstance != null;
			}
		}

		public event EventHandler OnCreatingGameObject;

		public event EventHandler<OnCreatedGameObjectEventArgs> OnCreatedGameObject;

		public event Action<TemplateComponentExample> OnDestructing;

		protected void Awake()
		{
		}

		protected virtual void Initialize()
		{
			_initialized = true;
		}

		protected void Start()
		{
		}

		protected void Update()
		{
		}

		protected void OnDrawGizmos()
		{
		}

		public void ShowBestPractices()
		{
			switch (0)
			{
			}
			for (int i = 0; i < 10; i++)
			{
			}
		}

		public void CreateGameObject()
		{
			RaiseOnCreatingGameObject(EventArgs.Empty);
			GameObject gameObject = new GameObject();
			RaiseOnCreatedGameObject(new OnCreatedGameObjectEventArgs(gameObject));
		}

		protected virtual void RaiseOnCreatingGameObject(EventArgs args)
		{
			if (this.OnCreatingGameObject != null)
			{
				this.OnCreatingGameObject(this, args);
			}
		}

		protected virtual void RaiseOnCreatedGameObject(OnCreatedGameObjectEventArgs args)
		{
			if (this.OnCreatedGameObject != null)
			{
				this.OnCreatedGameObject(this, args);
			}
		}

		private void StandardizedSpawner_OnCreatedGameObject(object sender, OnCreatedGameObjectEventArgs args)
		{
			Debug.Log(args.GameObject.name);
		}

		public static void LogLastInstanceName()
		{
			if (_lastCreatedInstance != null)
			{
				Debug.Log(_lastCreatedInstance.name);
			}
		}
	}
}
