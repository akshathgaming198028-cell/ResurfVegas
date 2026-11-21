using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.HTTP.Internal
{
	internal class Request : IRequest
	{
		private const string EOL = "\r\n";

		private readonly Headers _headers = new Headers();

		private DateTime _startTime;

		private static System.Random _random;

		public string method { get; private set; }

		public string protocol { get; private set; }

		public byte[] bytes { get; set; }

		public Uri uri { get; set; }

		public IResponse response { get; set; }

		public uint maxRedirectCount { get; set; }

		public bool acceptGzip { get; set; }

		public static string userAgent { get; set; }

		public Exception exception { get; set; }

		public RequestState state { get; set; }

		public bool ignoreCertificateErrors { get; set; }

		public bool ignoreContentMD5 { get; set; }

		public float timeout { get; set; }

		public float minBackOffTime { get; set; }

		public float maxBackOffTime { get; set; }

		public Headers headers
		{
			get
			{
				return _headers;
			}
		}

		public bool isFinished
		{
			get
			{
				return state == RequestState.Done || state == RequestState.Error || state == RequestState.Canceled || state == RequestState.TimedOut;
			}
		}

		public bool isSuccessful
		{
			get
			{
				return state == RequestState.Done && response.category == ResponseCategory.Successful;
			}
		}

		public string statusMessage
		{
			get
			{
				if (exception != null)
				{
					return string.Format("{0}: {1}", exception.GetType().Name, exception.Message);
				}
				if (response != null)
				{
					return string.Format("{0}: {1}", response.status, response.message);
				}
				return state.ToString();
			}
		}

		private bool useSsl
		{
			get
			{
				return uri.Scheme.ToLower() == "https";
			}
		}

		public float simulatedRequestLatency { get; set; }

		public IEnumerable<SimulatedRequestResult> simulatedRequestResults { get; set; }

		private Request()
		{
			protocol = "HTTP/1.1";
			maxRedirectCount = 8u;
			acceptGzip = true;
			ignoreCertificateErrors = false;
			simulatedRequestResults = null;
			minBackOffTime = 1f;
			_headers = new Headers();
		}

		public Request([NotNull] string method, [NotNull] Uri uri)
			: this()
		{
			this.method = method.ToUpperInvariant();
			this.uri = uri;
		}

		public Request([NotNull] string method, [NotNull] Uri uri, byte[] bytes)
			: this(method, uri)
		{
			this.bytes = bytes;
		}

		public Request([NotNull] string method, [NotNull] string uri)
			: this(method, new Uri(uri))
		{
		}

		public Request([NotNull] string method, [NotNull] string uri, byte[] bytes)
			: this(method, new Uri(uri), bytes)
		{
		}

		public override string ToString()
		{
			return Response.ContentAsString(bytes, _headers);
		}

		private void AssertNotCanceled(int step)
		{
			if (timeout > 0f && (DateTime.Now - _startTime).TotalSeconds > (double)timeout)
			{
				state = RequestState.TimedOut;
			}
			if (isFinished)
			{
				if (state == RequestState.Canceled)
				{
					throw new HTTPRequestCanceledException("Request was canceled (at step " + step + ")");
				}
				if (state == RequestState.TimedOut)
				{
					throw new HTTPRequestCanceledException("Request was canceled because it failed to complete in time (at step " + step + ")");
				}
				throw new HTTPException("Unexpected request state (at step " + step + ")");
			}
		}

		private Stream Connect()
		{
			int num = 0;
			if (timeout > 0f)
			{
				num = (int)(timeout * 1000f);
				if (num == 0)
				{
					num = 1;
				}
			}
			try
			{
				TcpClient tcpClient = new TcpClient();
				tcpClient.SendTimeout = num;
				tcpClient.ReceiveTimeout = num;
				tcpClient.Connect(uri.Host, uri.Port);
				AssertNotCanceled(1);
				Stream stream = tcpClient.GetStream();
				if (useSsl)
				{
					TrustedCertificates.Init();
					SslStream sslStream = new SslStream(stream, false, ValidateServerCertificate);
					sslStream.AuthenticateAsClient(uri.Host);
					return sslStream;
				}
				return stream;
			}
			catch (AuthenticationException ex)
			{
				throw new HTTPSecurityException(ex.Message, ex);
			}
		}

		public void CheckBackOff()
		{
			double backOffSeconds = Hosts.GetBackOffSeconds(uri.Host);
			if (backOffSeconds > 0.0)
			{
				throw new HTTPBackOffException(uri.Host + " is busy, cannot make request at this time (waiting " + backOffSeconds + " more seconds)");
			}
		}

		public void UpdateBackOffError()
		{
			if (maxBackOffTime > 0f)
			{
				Hosts.BackOff(uri.Host, minBackOffTime, maxBackOffTime);
			}
		}

		public void UpdateBackOff(Response response)
		{
			string host = uri.Host;
			int status = response.status;
			if (status == 500 || status == 503)
			{
				bool flag = false;
				if (response.headers.Contains("Retry-After"))
				{
					DateTime dateTime = response.headers.Get("Retry-After", DateTime.MinValue, true);
					if (dateTime > DateTime.UtcNow)
					{
						Hosts.BackOff(host, dateTime);
						flag = true;
					}
				}
				if (!flag && maxBackOffTime > 0f)
				{
					Hosts.BackOff(uri.Host, minBackOffTime, maxBackOffTime);
				}
			}
			else
			{
				Hosts.DontBackOff(host);
			}
		}

		public void FetchResponse()
		{
			_startTime = DateTime.Now;
			state = RequestState.Waiting;
			try
			{
				if (acceptGzip)
				{
					headers.Set("Accept-Encoding", "gzip");
				}
				if (!string.IsNullOrEmpty(userAgent) && !headers.Contains("User-Agent"))
				{
					headers.Set("User-Agent", userAgent);
				}
				headers.Set("Date", DateTime.UtcNow);
				IResponse response = null;
				if (response == null)
				{
					uint num = 0u;
					while (num++ < maxRedirectCount)
					{
						CheckBackOff();
						headers.Set("Host", uri.Host);
						using (Stream stream = Connect())
						{
							AssertNotCanceled(2);
							WriteToStream(stream);
							stream.Flush();
							AssertNotCanceled(3);
							Response response2 = new Response();
							state = RequestState.Reading;
							response2.ReadFromStream(stream, ignoreContentMD5);
							response = response2;
							UpdateBackOff(response2);
						}
						int status = response.status;
						if (status == 301 || status == 302 || status == 307)
						{
							uri = new Uri(response.headers.Get("Location"));
						}
						else
						{
							num = maxRedirectCount;
						}
					}
				}
				AssertNotCanceled(4);
				this.response = response;
				state = RequestState.Done;
			}
			catch (HTTPRequestCanceledException ex)
			{
				LogError(ex.ToString());
				if (state == RequestState.TimedOut)
				{
					UpdateBackOffError();
				}
			}
			catch (Exception ex2)
			{
				LogError(ex2.ToString());
				exception = ex2;
				this.response = null;
				state = RequestState.Error;
				if (!(ex2 is HTTPBackOffException))
				{
					UpdateBackOffError();
				}
			}
		}

		public void FetchResponseAsync()
		{
			state = RequestState.Waiting;
			try
			{
				TrustedCertificates.Init();
				if (!ThreadPool.QueueUserWorkItem(delegate
				{
					FetchResponse();
				}))
				{
					throw new HTTPException("Could not queue asynchronous request worker thread");
				}
			}
			catch (Exception ex)
			{
				exception = ex;
				state = RequestState.Error;
			}
		}

		public bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			bool result = true;
			if (sslPolicyErrors != SslPolicyErrors.None)
			{
				if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) != SslPolicyErrors.None)
				{
					result = ignoreCertificateErrors;
				}
				else if (TrustedCertificates.VerifyTrust(new X509Certificate2(certificate)))
				{
					sslPolicyErrors = SslPolicyErrors.None;
				}
				else
				{
					result = ignoreCertificateErrors;
				}
			}
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
			}
			return result;
		}

		private void WriteToStream(Stream outputStream)
		{
			StreamWriter streamWriter = new StreamWriter(outputStream, ASCIIEncoding.ASCII);
			streamWriter.Write(method);
			streamWriter.Write(' ');
			streamWriter.Write(uri.PathAndQuery);
			streamWriter.Write(' ');
			streamWriter.Write(protocol);
			streamWriter.Write("\r\n");
			foreach (KeyValuePair<string, string> header in headers)
			{
				streamWriter.Write(header.Key.Trim());
				streamWriter.Write(':');
				streamWriter.Write(header.Value.Trim());
				streamWriter.Write("\r\n");
			}
			int num = ((bytes != null) ? bytes.Length : 0);
			if ((num > 0 || method == "POST" || method == "PUT") && headers.Get("Content-Length", string.Empty) == string.Empty)
			{
				streamWriter.Write("Content-Length: ");
				streamWriter.Write(num);
				streamWriter.Write("\r\n");
			}
			streamWriter.Write("\r\n");
			streamWriter.Flush();
			if (bytes != null)
			{
				outputStream.Write(bytes, 0, bytes.Length);
			}
		}

		private float GetSimulatedRequestResponseChanceTotal()
		{
			if (simulatedRequestResults != null)
			{
				float num = 0f;
				{
					foreach (SimulatedRequestResult simulatedRequestResult in simulatedRequestResults)
					{
						if (simulatedRequestResult.chance > 0f)
						{
							num += simulatedRequestResult.chance;
						}
					}
					return num;
				}
			}
			return 0f;
		}

		private SimulatedRequestResult GetRandomSimulatedRequestResult()
		{
			float simulatedRequestResponseChanceTotal = GetSimulatedRequestResponseChanceTotal();
			if (simulatedRequestResponseChanceTotal > 0f)
			{
				if (_random == null)
				{
					_random = new System.Random();
				}
				double num = _random.NextDouble() * (double)simulatedRequestResponseChanceTotal;
				if (num < (double)simulatedRequestResponseChanceTotal)
				{
					foreach (SimulatedRequestResult simulatedRequestResult in simulatedRequestResults)
					{
						if (!(simulatedRequestResult.chance >= 0f))
						{
							continue;
						}
						num -= (double)simulatedRequestResult.chance;
						if (num <= 0.0)
						{
							if (simulatedRequestResult.socketError == SocketError.Success && simulatedRequestResult.response == null)
							{
								return null;
							}
							return simulatedRequestResult;
						}
					}
				}
			}
			return null;
		}

		[Conditional("HTTP_SIMULATION_ENABLED")]
		private void SimulateResponseIfApplicable(ref IResponse resultResponse)
		{
			CheckBackOff();
			IResponse response = null;
			SimulatedRequestResult randomSimulatedRequestResult = GetRandomSimulatedRequestResult();
			if (randomSimulatedRequestResult != null)
			{
				float num = 0.5f;
				Exception ex = null;
				if (randomSimulatedRequestResult.socketError != SocketError.Success)
				{
					ex = new SocketException((int)randomSimulatedRequestResult.socketError);
					if (randomSimulatedRequestResult.socketError == SocketError.TimedOut)
					{
						if (timeout > 0f)
						{
							num = timeout;
						}
						else
						{
							ex = null;
						}
					}
				}
				num += simulatedRequestLatency;
				if (num > 0f)
				{
					Thread.Sleep(TimeSpan.FromSeconds(num));
				}
				if (ex != null)
				{
					throw ex;
				}
				if (randomSimulatedRequestResult.response != null)
				{
					response = randomSimulatedRequestResult.response;
				}
			}
			if (simulatedRequestLatency > 0f)
			{
				Thread.Sleep(TimeSpan.FromSeconds(simulatedRequestLatency));
			}
			resultResponse = response;
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[Request] " + msg);
		}

		[Conditional("HTTP_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[Request] " + msg);
		}
	}
}
