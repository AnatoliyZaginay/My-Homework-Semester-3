using System;
using System.Diagnostics;

namespace Task_1
{
    class Program
    {
        static void Main(string[] args)
        {
            var statsM = AlgorithmStatistics.GetAlgorithmAverageStats(50, -10000, 10000, 100, MatrixOperations.Multiply);
            var statsP = AlgorithmStatistics.GetAlgorithmAverageStats(50, -10000, 10000, 100, MatrixOperations.MultiplyInParallel);

            Console.WriteLine($"{statsM.Item1} {statsM.Item2}\n");

            Console.WriteLine($"{statsP.Item1} {statsP.Item2}\n");
        }
    }
}