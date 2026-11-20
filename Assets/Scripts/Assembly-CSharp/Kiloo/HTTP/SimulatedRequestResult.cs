using System.Net.Sockets;

namespace Kiloo.HTTP
{
	public class SimulatedRequestResult
	{
		public float chance;

		public SocketError socketError;

		public IResponse response;
	}
}
