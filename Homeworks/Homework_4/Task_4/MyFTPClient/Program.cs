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

            Console.WriteLine("Commands: ");
            Console.WriteLine("1 - list of files in the directory");
            Console.WriteLine("2 - downloads the specified file");

            try
            {
                var command = Console.ReadLine();
                switch (command)
                {
                    case "1":
                        {
                            Console.Write("Enter directory path: ");
                            var directoryPath = Console.ReadLine();

                            var response = await client.List(directoryPath);
                            Console.WriteLine("Response:");
                            Console.WriteLine($"Size: {response.size}");
                            foreach (var (name, isDir) in response.list)
                            {
                                Console.WriteLine($"{name} {isDir}");
                            }
                            break;
                        }

                    case "2":
                        {
                            Console.Write("Enter path: ");
                            var path = Console.ReadLine();
                            Console.Write("Enter destination path: ");
                            var destinationPath = Console.ReadLine();

                            var response = await client.Get(destinationPath, path);
                            Console.WriteLine($"File size: {response}");
                            Console.WriteLine("File successfully downloaded.");
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Incorrect command.");
                            break;
                        }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception: {exception.Message}");
            }
        }
    }
}
