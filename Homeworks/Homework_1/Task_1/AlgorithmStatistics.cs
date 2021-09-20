using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Task_1
{
    public static class AlgorithmStatistics
    {
        private static long GetAlgotithmRunningTime(int[,] firstMatrix, int[,] secondMatrix, Func<int[,], int[,], int[,]> matrixMultiplyFunction)
        {
            var timer = new Stopwatch();
            timer.Start();
            matrixMultiplyFunction(firstMatrix, secondMatrix);
            timer.Stop();
            return timer.ElapsedMilliseconds;
        }

        public static (double, double) GetAlgorithmAverageStats(int matrixSize, int minNumber, int maxNumber,
            int numberOfIterations, Func<int[,], int[,], int[,]> matrixMultiplyFunction)
        {
            var results = new List<long>();

            for (int i = 0; i < numberOfIterations; ++i)
            {
                var firstMatrix = MatrixOperations.Generate(matrixSize, matrixSize, minNumber, maxNumber);
                var secondMatrix = MatrixOperations.Generate(matrixSize, matrixSize, minNumber, maxNumber);
                var result = GetAlgotithmRunningTime(firstMatrix, secondMatrix, matrixMultiplyFunction);
                results.Add(result);
            }

            var mathematicalExpectation = results.Average();
            var variance = results.Select(x => Math.Pow(x - mathematicalExpectation, 2)).Average();
            var standardDeviation = Math.Sqrt(variance);

            return (mathematicalExpectation, standardDeviation);
        }
    }
}