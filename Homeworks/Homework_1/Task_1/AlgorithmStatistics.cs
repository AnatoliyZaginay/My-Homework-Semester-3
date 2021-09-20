using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Task_1
{
    /// <summary>
    /// A static class for getting statistics about the execution time of the matrix multiplication algorithm.
    /// </summary>
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

        /// <summary>
        /// Gets statistics on the time of the matrix multiplication algorithm.
        /// </summary>
        /// <param name="matrixSize">The size of the generated matrices.</param>
        /// <param name="minNumber">The minimum random number that can be contained in the generated matrix.</param>
        /// <param name="maxNumber">The maximum random number that can be contained in the generated matrix.</param>
        /// <param name="numberOfIterations">The number of iterations.</param>
        /// <param name="matrixMultiplicationFunction">Matrix multiplication function.</param>
        /// <returns>The mathematical expectation and standard deviation of the algorithm's running time, expressed in milliseconds.</returns>
        public static (double, double) GetAlgorithmAverageStats(int matrixSize, int minNumber, int maxNumber,
            int numberOfIterations, Func<int[,], int[,], int[,]> matrixMultiplicationFunction)
        {
            var results = new List<long>();

            for (int i = 0; i < numberOfIterations; ++i)
            {
                var firstMatrix = MatrixOperations.Generate(matrixSize, matrixSize, minNumber, maxNumber);
                var secondMatrix = MatrixOperations.Generate(matrixSize, matrixSize, minNumber, maxNumber);
                var result = GetAlgotithmRunningTime(firstMatrix, secondMatrix, matrixMultiplicationFunction);
                results.Add(result);
            }

            var mathematicalExpectation = results.Average();
            var variance = results.Select(x => Math.Pow(x - mathematicalExpectation, 2)).Average();
            var standardDeviation = Math.Sqrt(variance);

            return (mathematicalExpectation, standardDeviation);
        }
    }
}

/*
    Для тестирования использовалась функция GetAlgorithmAverageStats, возвращающая матожидание и среднеквадратичное отклонение времени работы алгоритма.
    
    В матрицах генерировались числа в диапазоне -10000 : 10000
    Количество итераций: 100
 
    Результаты:
    64 x 64:    однопоточный алгоритм: матожидание - 2,07 ms, среднеквадратичное отклонение - 0,380919939094818 ms
                многопоточный алгоритм: матожидание - 1,2 ms, среднеквадратичное отклонение - 0,509901951359278 ms

    128 x 128:  однопоточный алгоритм: матожидание - 18,48 ms, среднеквадратичное отклонение - 2,1188676221038443 ms
                многопоточный алгоритм: матожидание - 5,23 ms, среднеквадратичное отклонение - 1,2235603785673994 ms

    256 x 256:  однопоточный алгоритм: матожидание - 149,56 ms, среднеквадратичное отклонение - 8,444311694862995 ms
                многопоточный алгоритм: матожидание - 40,18 ms, среднеквадратичное отклонение - 4,661287375822262 ms

    512 x 512:  однопоточный алгоритм: матожидание - 1623,46 ms, среднеквадратичное отклонение - 45,152944532998056 ms
                многопоточный алгоритм: матожидание - 379,24 ms, среднеквадратичное отклонение - 23,651689157436508 ms

    Видно, что при размерах матрицы 512 x 512 разница уже довольно существенна, и многопоточный алогритм работает заметно быстрее однопоточного.
    
    При размерах матрицы ~ 60 x 60 алгоритмы примерно одинаковы по скорости работы.
    При меньших размерах матрицы - однопоточный алгоритм быстрее.
 */