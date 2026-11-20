using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Kiloo.Monetization.Offerwall.Internal
{
	public class ImageDownloader
	{
		private Action<bool, ImageDownloader> _onDownloadComplete;

		private string _url;

		private Texture2D _image;

		private float _timeout;

		private object _cookie;

		private bool _isDownloading;

		public object cookie
		{
			get
			{
				return _cookie;
			}
		}

		public Texture2D image
		{
			get
			{
				return _image;
			}
		}

		public string url
		{
			get
			{
				return _url;
			}
		}

		public bool isDownloading
		{
			get
			{
				return _isDownloading;
			}
		}

		public ImageDownloader(string url, Action<bool, ImageDownloader> onDownloadComplete, float timeout, object obj)
		{
			_url = url;
			_timeout = timeout;
			_cookie = obj;
			AddOnDownloadCompleteHandler(onDownloadComplete);
		}

		public void AddOnDownloadCompleteHandler(Action<bool, ImageDownloader> handler)
		{
			if (handler != null)
			{
				_onDownloadComplete = (Action<bool, ImageDownloader>)Delegate.Combine(_onDownloadComplete, handler);
			}
		}

		public void RemoveOnDownloadCompleteHandler(Action<bool, ImageDownloader> handler)
		{
			if (handler != null)
			{
				_onDownloadComplete = (Action<bool, ImageDownloader>)Delegate.Remove(_onDownloadComplete, handler);
			}
		}

		public IEnumerator Download()
		{
			float _startedDownloading = Time.realtimeSinceStartup;
			if (_url == null)
			{
				yield break;
			}
			_isDownloading = true;
			WWW www = new WWW(_url);
			while (!www.isDone)
			{
				if (_timeout > 0f && Time.realtimeSinceStartup - _startedDownloading >= _timeout)
				{
					DownloadComplete(false);
					yield break;
				}
				yield return null;
			}
			yield return www;
			if (www.error != null)
			{
				DownloadComplete(false);
				yield break;
			}
			_image = www.texture;
			if (_image == null || (_image.width == 8 && _image.height == 8))
			{
				DownloadComplete(false);
			}
			else
			{
				DownloadComplete(true);
			}
		}

		private void DownloadComplete(bool succes)
		{
			_isDownloading = true;
			Action<bool, ImageDownloader> onDownloadComplete = _onDownloadComplete;
			if (onDownloadComplete != null)
			{
				onDownloadComplete(succes, this);
			}
		}

		[Conditional("OFFERWALL_DEBUG_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
			Debug.Log("ImageDownloader: " + msg, context);
		}
	}
}
