using System.Net.Sockets;
using System.Threading.Tasks;

namespace Test_1
{
    /// <summary>
    /// Client class.
    /// </summary>
    public class Client
    {
        private TcpClient client;

        /// <summary>
        /// Creaters a new client.
        /// </summary>
        public Client(string host, int port)
        {
            client = new TcpClient(host, port);
        }

        /// <summary>
        /// Starts client.
        /// </summary>
        public void Run()
        {
            var stream = client.GetStream();

            var send = Task.Run(async () => await Messaging.Send(stream, "Client"));
            var receive = Task.Run(async () => await Messaging.Receive(stream, "Server"));
            Task.WaitAny(send, receive);
        }
    }
}