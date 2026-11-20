using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Kiloo.HTTP
{
	public interface IRequest
	{
		string method { get; }

		string protocol { get; }

		byte[] bytes { get; set; }

		Uri uri { get; set; }

		IResponse response { get; set; }

		uint maxRedirectCount { get; set; }

		bool acceptGzip { get; set; }

		Exception exception { get; set; }

		RequestState state { get; set; }

		bool ignoreContentMD5 { get; set; }

		bool ignoreCertificateErrors { get; set; }

		float timeout { get; set; }

		float minBackOffTime { get; set; }

		float maxBackOffTime { get; set; }

		Headers headers { get; }

		bool isFinished { get; }

		bool isSuccessful { get; }

		[NotNull]
		string statusMessage { get; }

		float simulatedRequestLatency { get; set; }

		IEnumerable<SimulatedRequestResult> simulatedRequestResults { get; set; }

		void FetchResponse();

		void FetchResponseAsync();
	}
}
