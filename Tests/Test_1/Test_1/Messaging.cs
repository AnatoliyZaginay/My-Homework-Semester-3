using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Test_1
{
    /// <summary>
    /// Static class for messaging between network streams.
    /// </summary>
    public static class Messaging
    {
        /// <summary>
        /// Sends a message.
        /// </summary>
        public static async Task Send(NetworkStream stream, string name)
        {
            using var writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            while (true)
            {
                Console.Write($"{name}: ");
                var message = Console.ReadLine();
                await writer.WriteLineAsync(message);

                if (message == "exit")
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Receives a message.
        /// </summary>
        public static async Task Receive(NetworkStream stream, string name)
        {
            using var reader = new StreamReader(stream);

            while (true)
            {
                var message = await reader.ReadLineAsync();
                Console.WriteLine($"{name}: {message}");

                if (message == "exit")
                {
                    break;
                }
            }
        }
    }
}