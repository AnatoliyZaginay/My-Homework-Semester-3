using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;

namespace MyFTPClient
{
    /// <summary>
    /// Implementation of the FTP server client.
    /// </summary>
    public class MyClient
    {
        private string host;
        private int port;

        /// <summary>
        /// Creates new FTP server client.
        /// </summary>
        public MyClient(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        /// <summary>
        /// Returns a list of files and directories in the specified directory.
        /// </summary>
        /// <param name="directoryPath">Path to the directory.</param>
        public async Task<List<(string, bool)>> List(string directoryPath, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var client = new TcpClient();
            await client.ConnectAsync(host, port, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            using var stream = client.GetStream();
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            cancellationToken.ThrowIfCancellationRequested();

            await writer.WriteLineAsync($"1 {directoryPath}");
            var response = await reader.ReadLineAsync();

            if (response == "-1")
            {
                throw new DirectoryNotFoundException("Directory not found");
            }

            var data = response.Split(' ');

            var list = new List<(string, bool)>();

            for (int i = 1; i < data.Length; i += 2)
            {
                cancellationToken.ThrowIfCancellationRequested();
                list.Add((data[i], bool.Parse(data[i + 1])));
            }

            cancellationToken.ThrowIfCancellationRequested();
            return list;
        }

        /// <summary>
        /// Downloads the specified file, and returns its size.
        /// </summary>
        /// <param name="destination">The path to download the file.</param>
        /// <param name="source">Path to the specified file.</param>
        public async Task<int> Get(string destination, string source, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var client = new TcpClient();
            await client.ConnectAsync(host, port, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            using var stream = client.GetStream();
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            cancellationToken.ThrowIfCancellationRequested();

            await writer.WriteLineAsync($"2 {source}");
            var response = await reader.ReadLineAsync();

            if (response == "-1")
            {
                throw new FileNotFoundException("File not found");
            }

            var size = int.Parse(response);

            cancellationToken.ThrowIfCancellationRequested();

            using var fileStream = File.Create(destination);
            await stream.CopyToAsync(fileStream, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            return size;
        }
    }
}