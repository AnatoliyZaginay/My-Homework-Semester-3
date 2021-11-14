using System;
using System.Threading.Tasks;
using System.Net;

namespace MyFTPServer
{
    class Program
    {
        private static void Help()
        {
            Console.WriteLine("Write the IP address and port to start the server");
        }

        static async Task Main(string[] args)
        {
            if (args.Length != 2)
            {
                Help();
                return;
            }

            if (!IPAddress.TryParse(args[0], out var ip))
            {
                Console.WriteLine("Incorrect IP");
                return;
            }

            if (!int.TryParse(args[1], out var port) || port < 0 || port > 65535)
            {
                Console.WriteLine("Incorrect port");
                return;
            }

            var server = new MyServer(ip, port);
            Console.WriteLine("Server is running");
            await server.Run();
        }
    }
}