using System;
using System.Threading.Tasks;
using System.Net;

namespace MyFTPClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Enter the IP: ");
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
            }

            var client = new MyClient(ipString, port);
        }
    }
}
