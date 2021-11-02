using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;

namespace MyFTPServer
{
    /// <summary>
    /// Implementation of the FTP server.
    /// </summary>
    public class MyServer
    {
        private TcpListener listener;
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Creates new FTP server.
        /// </summary>
        public MyServer(IPAddress ip, int port)
        {
            listener = new(ip, port);
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
            var result = $"{size}";

            foreach (var file in files)
            {
                result += $" {file} false";
            }

            foreach (var directory in directories)
            {
                result += $" {directory} true";
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
            await writer.WriteLineAsync();
        }

        private async Task Work(Socket socket)
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

        /// <summary>
        /// Starts the server.
        /// </summary>
        public async Task Run()
        {
            listener.Start();
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                var socket = await listener.AcceptSocketAsync();
                await Task.Run(() => Work(socket));
            }
            listener.Stop();
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void Stop()
            => cancellationTokenSource.Cancel();
    }
}