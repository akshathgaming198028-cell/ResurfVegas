using System;
using System.Collections.Generic;
using SYBO.Core.Components.Singleton;
using UnityEngine;

public class AssetBundleFactory<T> where T : UnityEngine.Object
{
	private Dictionary<string, int> _requests = new Dictionary<string, int>();

	private Dictionary<string, List<Disposable<T>>> _disposableReferences = new Dictionary<string, List<Disposable<T>>>();

	private bool _isComponent;

	public AssetBundleFactory()
	{
		_isComponent = typeof(Component).IsAssignableFrom(typeof(T));
		Factories instance = SingletonMonobehavior<Factories>.Instance;
		instance.OnLateUpdate = (Action)Delegate.Combine(instance.OnLateUpdate, new Action(HandleOnLateUpdate));
	}

	public void RequestItemInstance(string name, Action<T> onSuccess)
	{
		name = name.ToLower();
		if (!_requests.ContainsKey(name))
		{
			_requests.Add(name, 0);
		}
		Dictionary<string, int> requests;
		Dictionary<string, int> dictionary = (requests = _requests);
		string key2;
		string key = (key2 = name);
		int num = requests[key2];
		dictionary[key] = num + 1;
		AssetBundle assetBundle = AssetBundleManager.Instance.GetAssetBundle(name);
		if (assetBundle == null)
		{
			onSuccess((T)null);
		}
		else
		{
			LoadAssetFromBundles(assetBundle, name, onSuccess);
		}
	}

	public void LoadAssetFromBundles(AssetBundle bundle, string name, Action<T> onSucces)
	{
		T original = ((!_isComponent) ? bundle.LoadAsset<T>(name) : bundle.LoadAsset<GameObject>(name).GetComponent<T>());
		Disposable<T> disposable = new Disposable<T>(UnityEngine.Object.Instantiate(original));
		if (!_disposableReferences.ContainsKey(name))
		{
			_disposableReferences.Add(name, new List<Disposable<T>>());
		}
		_disposableReferences[name].Add(disposable);
		Dictionary<string, int> requests;
		Dictionary<string, int> dictionary = (requests = _requests);
		string key2;
		string key = (key2 = name);
		int num = requests[key2];
		dictionary[key] = num - 1;
		if (onSucces != null)
		{
			onSucces(disposable.ReferencedObject);
		}
	}

	private void HandleOnLateUpdate()
	{
		List<string> list = new List<string>();
		foreach (string key in _disposableReferences.Keys)
		{
			List<Disposable<T>> list2 = _disposableReferences[key];
			int num = 0;
			while (num < list2.Count)
			{
				if (list2[num].ReferencedObject == null)
				{
					list2.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
			if (list2.Count == 0 && _requests[key] == 0)
			{
				list.Add(key);
			}
		}
		foreach (string item in list)
		{
			_disposableReferences.Remove(item);
			AssetBundleManager.Instance.UnloadAssetBundle(item, true);
		}
	}
}
