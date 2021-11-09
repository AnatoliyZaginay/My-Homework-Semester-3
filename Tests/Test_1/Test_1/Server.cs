using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Test_1
{
    /// <summary>
    /// Server class.
    /// </summary>
    public class Server
    {
        private TcpListener listener;

        /// <summary>
        /// Creates a new server.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public Server(IPAddress ip, int port)
        {
            listener = new TcpListener(ip, port);
        }

        /// <summary>
        /// Starts server.
        /// </summary>
        public async Task Run()
        {
            listener.Start();
            var client = await listener.AcceptTcpClientAsync();
            using var stream = client.GetStream();

            var send = Task.Run(async () => await Messaging.Send(stream, "Server"));
            var receive = Task.Run(async () => await Messaging.Receive(stream, "Client"));
            Task.WaitAny(send, receive);

            listener.Stop();
        }
    }
}