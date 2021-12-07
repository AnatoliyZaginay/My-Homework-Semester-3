using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Test_2
{
    /// <summary>
    /// Static class for calculating check sum.
    /// </summary>
    public static class CheckSum
    {
        private static byte[] GetFileHash(FileInfo info)
        {
            using var stream = info.Open(FileMode.Open);
            return MD5.Create().ComputeHash(stream);
        }

        /// <summary>
        /// Singlethreaded variant of checksum implementation.
        /// </summary>
        public static byte[] GetCheckSumSinglethreaded(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new ArgumentException();
            }

            var info = new DirectoryInfo(path);

            var filesInfo = info.GetFiles();
            var filesSum = filesInfo.OrderBy(x => x.Name).Select(x => GetFileHash(x)).ToList();

            var directoriesInfo = info.GetDirectories();
            var directoriesSum = directoriesInfo.OrderBy(x => x.Name).Select(x => GetCheckSumSinglethreaded(x.FullName)).ToList();

            var stringBuilder = new StringBuilder(info.FullName);
            stringBuilder.Append(filesSum);
            stringBuilder.Append(directoriesSum);

            var line = stringBuilder.ToString();

            return MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(line));
        }

        /// <summary>
        /// Multithreaded variant of checksum implementation.
        /// </summary>
        public static byte[] GetCheckSumMiltithreaded(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new ArgumentException();
            }

            var info = new DirectoryInfo(path);

            var filesInfo = info.GetFiles();
            var filesSum = filesInfo.AsParallel().OrderBy(x => x.Name).Select(x => GetFileHash(x)).ToList();

            var directoriesInfo = info.GetDirectories();
            var directoriesSum = directoriesInfo.AsParallel().OrderBy(x => x.Name).Select(x => GetCheckSumSinglethreaded(x.FullName)).ToList();

            var stringBuilder = new StringBuilder(info.FullName);
            stringBuilder.Append(filesSum);
            stringBuilder.Append(directoriesSum);

            var line = stringBuilder.ToString();

            return MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(line));
        }
    }
}
