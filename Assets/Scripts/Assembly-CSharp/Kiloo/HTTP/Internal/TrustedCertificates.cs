using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.HTTP.Internal
{
	internal static class TrustedCertificates
	{
		private static X509Certificate2Collection _trusted;

		public static void Init()
		{
			if (_trusted == null)
			{
				_trusted = LoadTrusted();
			}
		}

		public static bool VerifyTrust(X509Certificate2 certificate)
		{
			if (_trusted == null)
			{
				_trusted = LoadTrusted();
			}
			X509Chain x509Chain = new X509Chain();
			x509Chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
			x509Chain.ChainPolicy.ExtraStore.AddRange(_trusted);
			x509Chain.Build(certificate);
			bool result = true;
			bool flag = false;
			X509ChainStatus[] chainStatus = x509Chain.ChainStatus;
			for (int i = 0; i < chainStatus.Length; i++)
			{
				X509ChainStatus x509ChainStatus = chainStatus[i];
				if (x509ChainStatus.Status == X509ChainStatusFlags.UntrustedRoot)
				{
					flag = true;
				}
				else if (x509ChainStatus.Status != X509ChainStatusFlags.NoError)
				{
					result = false;
				}
			}
			if (flag)
			{
				X509ChainElementEnumerator enumerator = x509Chain.ChainElements.GetEnumerator();
				while (enumerator.MoveNext())
				{
					X509ChainElement current = enumerator.Current;
					X509ChainStatus[] chainElementStatus = current.ChainElementStatus;
					foreach (X509ChainStatus x509ChainStatus2 in chainElementStatus)
					{
						if (x509ChainStatus2.Status == X509ChainStatusFlags.UntrustedRoot && !_trusted.Contains(current.Certificate))
						{
							result = false;
						}
					}
				}
			}
			return result;
		}

		[NotNull]
		private static X509Certificate2Collection LoadTrusted()
		{
			X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
			TextAsset[] array = Resources.LoadAll<TextAsset>("trusted_roots");
			TextAsset[] array2 = array;
			foreach (TextAsset textAsset in array2)
			{
				try
				{
					x509Certificate2Collection.Import(textAsset.bytes);
				}
				catch (Exception ex)
				{
					LogError("LoadTrusted: Failed to import trusted root certificate \"" + textAsset.name + "\": " + ex);
				}
			}
			return x509Certificate2Collection;
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[TrustedCertificates] " + msg);
		}

		[Conditional("HTTP_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[TrustedCertificates] " + msg);
		}
	}
}
