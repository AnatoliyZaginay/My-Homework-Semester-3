using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MyFTPClient
{
    /// <summary>
    /// Implementation of the FTP server client.
    /// </summary>
    public class MyClient
    {
        private TcpClient client;

        /// <summary>
        /// Creates new FTP server client.
        /// </summary>
        public MyClient(string host, int port)
        {
            client = new(host, port);
        }

        /// <summary>
        /// Returns a list of files and directories in the specified directory.
        /// </summary>
        /// <param name="directoryPath">Path to the directory.</param>
        public async Task<(int size, List<(string, bool)> list)> List(string directoryPath)
        {
            using var stream = client.GetStream();
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            await writer.WriteLineAsync($"1 {directoryPath}");
            var response = await reader.ReadLineAsync();

            if (response == "-1")
            {
                throw new ArgumentException("Incorrect directory");
            }

            var data = response.Split(' ');
            var size = int.Parse(data[0]);

            var list = new List<(string, bool)>();

            for (int i = 1; i < data.Length; i += 2)
            {
                list.Add((data[i], bool.Parse(data[i + 1])));
            }

            return (size, list);
        }

        /// <summary>
        /// Downloads the specified file, and returns its size.
        /// </summary>
        /// <param name="destination">The path to download the file.</param>
        /// <param name="source">Path to the specified file.</param>
        public async Task<int> Get(string destination, string source)
        {
            using var stream = client.GetStream();
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            await writer.WriteLineAsync($"2 {source}");

            var response = await reader.ReadLineAsync();

            if (response == "-1")
            {
                throw new ArgumentException("File not exists");
            }

            var size = int.Parse(response);
            var bytes = new byte[size];
            await reader.BaseStream.ReadAsync(bytes);

            using var fileStream = File.Create(destination);
            await fileStream.WriteAsync(bytes);

            return size;
        }
    }
}