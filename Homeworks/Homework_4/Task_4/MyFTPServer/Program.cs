using System;
using System.Threading.Tasks;
using System.Net;

namespace FtpServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            /*Console.Write("Enter the IP: ");
            var ipString = Console.ReadLine();

            if (!IPAddress.TryParse(ipString, out var ip))
            {
                Console.WriteLine("Incorrect IP");
                return;
            }

            Console.Write("Enter the port: ");
            var portString = Console.ReadLine();

            if (!int.TryParse(portString, out var port) || port < 0 || port > 65535)
            {
                Console.WriteLine("Incorrect port");
                return;
            }*/

            IPAddress.TryParse("127.0.0.1", out var ip);
            var port = 8888;

            var server = new MyServer(ip, port);
            Console.WriteLine("Server is running");
            await server.Run();
        }
    }
}