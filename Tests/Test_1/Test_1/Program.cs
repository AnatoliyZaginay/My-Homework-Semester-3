using System;
using System.Net;
using System.Threading.Tasks;

namespace Test_1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            switch (args.Length)
            {
                case 1:
                    {
                        if (!int.TryParse(args[0], out var port))
                        {
                            Console.WriteLine("Incorrect port");
                            break;
                        }

                        var server = new Server(IPAddress.Any, port);
                        await server.Run();
                        break;
                    }
                case 2:
                    {
                        if (!IPAddress.TryParse(args[0], out var ip))
                        {
                            Console.WriteLine("Incorrect ip");
                            break;
                        }
                        if (!int.TryParse(args[1], out var port))
                        {
                            Console.WriteLine("Incorrect port");
                            break;
                        }

                        var client = new Client(args[0], port);
                        client.Run();
                        break;
                    }
                default:
                    Console.WriteLine("Not enough arguments");
                    break;
            }
        }
    }
}