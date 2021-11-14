using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace MyFTPServer
{
    /// <summary>
    /// Implementation of the FTP server.
    /// </summary>
    public class MyServer
    {
        private CancellationTokenSource cancellationTokenSource;
        private IPAddress ip;
        private int port;

        /// <summary>
        /// Creates new FTP server.
        /// </summary>
        public MyServer(IPAddress ip, int port)
        {
            this.ip = ip;
            this.port = port;
            cancellationTokenSource = new();
        }

        private async Task List(StreamWriter writer, string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                await writer.WriteLineAsync("-1");
                return;
            }

            var files = Directory.GetFiles(directoryPath);
            var directories = Directory.GetDirectories(directoryPath);

            var size = files.Length + directories.Length;
            var result = new StringBuilder($"{size}");

            foreach (var file in files)
            {
                result.Append($" {file} false");
            }

            foreach (var directory in directories)
            {
                result.Append($" {directory} true");
            }

            await writer.WriteLineAsync(result);
        }

        private async Task Get(StreamWriter writer, string filePath)
        {
            if (!File.Exists(filePath))
            {
                await writer.WriteLineAsync("-1");
                return;
            }

            var size = new FileInfo(filePath).Length;
            await writer.WriteLineAsync($"{size}");

            await File.OpenRead(filePath).CopyToAsync(writer.BaseStream);
        }

        private async Task Work(Socket socket)
        {
            using (socket)
            {
                using var stream = new NetworkStream(socket);
                using var reader = new StreamReader(stream);
                using var writer = new StreamWriter(stream);
                writer.AutoFlush = true;

                var request = await reader.ReadLineAsync();
                var arguments = request.Split(' ');

                if (arguments.Length != 2 || (arguments[0] != "1" && arguments[0] != "2"))
                {
                    await writer.WriteLineAsync("Incorrect request");
                    return;
                }

                switch (arguments[0])
                {
                    case "1":
                        await List(writer, arguments[1]);
                        return;
                    case "2":
                        await Get(writer, arguments[1]);
                        return;
                }
            }
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        public async Task Run()
        {
            var listener = new TcpListener(ip, port);
            listener.Start();

            var clients = new List<Task>();

            while (!cancellationTokenSource.IsCancellationRequested)
            {
                var socket = await listener.AcceptSocketAsync();
                var client = Task.Run(() => Work(socket));
                clients.Add(client);
            }

            Task.WaitAll(clients.ToArray());
            listener.Stop();
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void Stop()
            => cancellationTokenSource.Cancel();
    }
}