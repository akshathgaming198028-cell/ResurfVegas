using System;

namespace Kiloo.HTTP.Internal
{
	internal class HTTPRequestCanceledException : Exception
	{
		public HTTPRequestCanceledException(string msg)
			: base(msg)
		{
		}
	}
}
