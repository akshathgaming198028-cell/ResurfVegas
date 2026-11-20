using System;

namespace Kiloo.HTTP
{
	public class HTTPException : Exception
	{
		public HTTPException(string message)
			: base(message)
		{
		}

		public HTTPException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
