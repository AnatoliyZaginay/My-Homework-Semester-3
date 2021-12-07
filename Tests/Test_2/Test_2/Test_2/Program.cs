using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Test_2
{
    class Program
    {
        private static long GetRunningTime(Func<string, byte[]> checkSum, string path)
        {
            var timer = new Stopwatch();
            timer.Start();
            checkSum(path);
            timer.Stop();
            return timer.ElapsedMilliseconds;
        }

        private static (double, double) GetAverageStats(int numberOfIterations, Func<string, byte[]> checkSum, string path)
        {
            var results = new List<long>();

            for (int i = 0; i < numberOfIterations; ++i)
            {
                var result = GetRunningTime(checkSum, path);
                results.Add(result);
            }

            var mathematicalExpectation = results.Average();
            var variance = results.Select(x => Math.Pow(x - mathematicalExpectation, 2)).Average();
            var standardDeviation = Math.Sqrt(variance);

            return (mathematicalExpectation, standardDeviation);
        }

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Enter a directory path");
                return;
            }

            try
            {
                var firstResult = GetAverageStats(20, CheckSum.GetCheckSumSinglethreaded, args[0]);
                var secondResult = GetAverageStats(20, CheckSum.GetCheckSumMiltithreaded, args[0]);

                Console.WriteLine($"Singlethreaded stats:\n{firstResult.Item1}\n{firstResult.Item2}");
                Console.WriteLine($"Multithreaded stats:\n{secondResult.Item1}\n{secondResult.Item2}");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Directory not exists");
            }
        }
    }
}
