using System;
using System.Collections.Generic;
using System.Linq;
using Kiloo.Plugins.DeviceUtility;
using UnityEngine;

namespace Kiloo.Monetization.Offerwall.Internal
{
	public static class OfferWallImagesDownloader<T> where T : IDisplayableOffer
	{
		private struct TextureUrl
		{
			public Texture2D texture;

			public string url;
		}

		private const int TIMEOUT = 30;

		private const int MAX_SIMULTANEOUS_ALLOWED_DOWNLOADS = 3;

		private const int MAX_IMAGES_TO_HOLD_IN_MEMORY = 10;

		private static NewestEntriesList<TextureUrl> _cachedImages;

		private static MonoBehaviour _monobehavior = null;

		private static Queue<T> _imagesToDownloadQueue = new Queue<T>();

		private static List<T> _downloadingQueue = new List<T>();

		private static bool _isPaused = false;

		private static bool isSupportedOnPlatform
		{
			get
			{
				return (Application.platform == RuntimePlatform.Android && !DeviceUtility.isKindleDevice) || Application.isEditor;
			}
		}

		public static event Action<bool, T> _onImageDownloaded;

		static OfferWallImagesDownloader()
		{
			OfferWallImagesDownloader<T>._onImageDownloaded = null;
		}

		public static void DownloadImagesForOffers(List<T> offers)
		{
			foreach (T offer in offers)
			{
				DownloadImageForOffer(offer);
			}
		}

		public static void DownloadImageForOffer(T offer)
		{
			if (!isSupportedOnPlatform)
			{
				return;
			}
			int num = -1;
			if (_cachedImages != null)
			{
				num = _cachedImages.FindIndex((TextureUrl textureUrl) => textureUrl.url.Equals(offer.iconToDisplayURL));
			}
			if (num > -1 && _cachedImages != null)
			{
				offer.texture = _cachedImages.ElementAt(num).texture;
				Action<bool, T> onImageDownloaded = OfferWallImagesDownloader<T>._onImageDownloaded;
				if (onImageDownloaded != null)
				{
					onImageDownloaded(true, offer);
				}
			}
			else if (!_isPaused && _downloadingQueue.Count < 3)
			{
				ImageDownloader imageDownloader = new ImageDownloader(offer.iconToDisplayURL, OnImageDownloadComplete, 30f, offer);
				GetDownloadMonoBehaviour().StartCoroutine(imageDownloader.Download());
				_downloadingQueue.Add(offer);
			}
			else
			{
				_imagesToDownloadQueue.Enqueue(offer);
			}
		}

		public static void PauseDownloading()
		{
			if (!isSupportedOnPlatform)
			{
				return;
			}
			_isPaused = true;
			if (_monobehavior != null)
			{
				_monobehavior.StopAllCoroutines();
				UnityEngine.Object.Destroy(_monobehavior.gameObject);
				_monobehavior = null;
			}
			foreach (T item in _downloadingQueue)
			{
				_imagesToDownloadQueue.Enqueue(item);
			}
			_downloadingQueue.Clear();
		}

		public static void ResumeDownloading()
		{
			if (isSupportedOnPlatform)
			{
				_isPaused = false;
				DownloadRemaining();
			}
		}

		private static void DownloadRemaining()
		{
			if (isSupportedOnPlatform && !_isPaused)
			{
				while (_downloadingQueue.Count < 3 && _imagesToDownloadQueue.Count > 0)
				{
					T val = _imagesToDownloadQueue.Dequeue();
					ImageDownloader imageDownloader = new ImageDownloader(val.iconToDisplayURL, OnImageDownloadComplete, 30f, val);
					GetDownloadMonoBehaviour().StartCoroutine(imageDownloader.Download());
					_downloadingQueue.Add(val);
				}
			}
		}

		private static void SaveTextureWithUrl(Texture2D texture, string url)
		{
			TextureUrl value = new TextureUrl
			{
				texture = texture,
				url = url
			};
			if (_cachedImages == null)
			{
				_cachedImages = new NewestEntriesList<TextureUrl>();
				_cachedImages.MaxItems = 10;
			}
			_cachedImages.Add(value);
		}

		private static void OnImageDownloadComplete(bool success, ImageDownloader imgDownloader)
		{
			if (isSupportedOnPlatform)
			{
				T val = (T)imgDownloader.cookie;
				if (success)
				{
					val.texture = imgDownloader.image;
					SaveTextureWithUrl(imgDownloader.image, imgDownloader.url);
				}
				_downloadingQueue.Remove(val);
				Action<bool, T> onImageDownloaded = OfferWallImagesDownloader<T>._onImageDownloaded;
				if (onImageDownloaded != null)
				{
					onImageDownloaded(success, val);
				}
				DownloadRemaining();
			}
		}

		private static MonoBehaviour GetDownloadMonoBehaviour()
		{
			if (_monobehavior == null)
			{
				GameObject gameObject = new GameObject("ImageDownloadManager");
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				_monobehavior = gameObject.AddComponent<MonoBehaviour>();
			}
			return _monobehavior;
		}
	}
}
