using System;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace MyFTPClient
{
    class Program
    {
        private static void Help()
        {
            Console.WriteLine("Write the IP address and port to start the client");
            Console.WriteLine("Commands: ");
            Console.WriteLine("--list (directoryPath) - list of files in the directory");
            Console.WriteLine("--get (source file) (destination file) - downloads the specified file");
        }

        static async Task Main(string[] args)
        {
            if (args.Length < 4 || args.Length > 5)
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

            var client = new MyClient(args[0], port);

            try
            {
                var command = Console.ReadLine();
                switch (args[2])
                {
                    case "--list":
                        {
                            var response = await client.List(args[3], CancellationToken.None);
                            Console.WriteLine("Response:");
                            Console.WriteLine($"Size: {response.Count}");
                            foreach (var (name, isDir) in response)
                            {
                                Console.WriteLine($"{name} {isDir}");
                            }
                            break;
                        }

                    case "--get":
                        {
                            if (args.Length != 5)
                            {
                                Help();
                                return;
                            }

                            var response = await client.Get(args[4], args[3], CancellationToken.None);
                            Console.WriteLine($"File size: {response}");
                            Console.WriteLine("File successfully downloaded.");
                            break;
                        }
                    default:
                        {
                            Help();
                            return;
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
