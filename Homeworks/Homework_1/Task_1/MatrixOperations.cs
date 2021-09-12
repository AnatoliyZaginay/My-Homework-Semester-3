using System;
using System.IO;
using System.Threading;

namespace Task_1
{
    public static class MatrixOperations
    {
        public static int[,] Generate(int rowsNumber, int columnsNumber, int minNumber, int maxNumber)
        {
            if (minNumber > maxNumber)
            {
                throw new ArgumentException("The minimum number must be less than the maximum");
            }

            var newMatrix = new int[rowsNumber, columnsNumber];
            var random = new Random();

            for (int i = 0; i < rowsNumber; ++i)
            {
                for (int j = 0; j < columnsNumber; ++j)
                {
                    newMatrix[i, j] = random.Next(minNumber, maxNumber);
                }
            }

            return newMatrix;
        }

        public static int[,] ReadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new ArgumentException("File not exists");
            }

            var lines = File.ReadAllLines(path);
            var matrix = new int[lines.Length, lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Length];

            for (int i = 0; i < lines.Length; ++i)
            {
                var line = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (matrix.GetLength(1) != line.Length)
                {
                    throw new ArgumentException("The number of elements in the rows of the matrix must be the same");
                }

                for (int j = 0; j < line.Length; ++j)
                {
                    if (!int.TryParse(line[j], out matrix[i, j]))
                    {
                        throw new ArgumentException("The elements of the matrix must be integers");
                    }
                }
            }

            return matrix;
        }

        public static void WriteToFile(int[,] matrix, string path)
        {
            using var file = new StreamWriter(path);

            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                for (int j = 0; j < matrix.GetLength(1); ++j)
                {
                    file.Write(j < matrix.GetLength(1) - 1 ? $"{matrix[i, j]} " : matrix[i, j]);
                }

                if (i < matrix.GetLength(0) - 1)
                {
                    file.Write("\n");
                }
            }
        }

        public static int[,] Multiply(int[,] firstMatrix, int[,] secondMatrix)
        {
            if (firstMatrix.GetLength(1) != secondMatrix.GetLength(0))
            {
                throw new ArgumentException("The number of columns of the first matrix must match the number of rows of the second matrix");
            }

            var result = new int[firstMatrix.GetLength(0), secondMatrix.GetLength(1)];

            for (int i = 0; i < result.GetLength(0); ++i)
            {
                for (int j = 0; j < result.GetLength(1); ++j)
                {
                    for (int k = 0; k < firstMatrix.GetLength(1); ++k)
                    {
                        result[i, j] += firstMatrix[i, k] * secondMatrix[k, j];
                    }
                }
            }

            return result;
        }

        public static void MultiplyMatrixChunk(int[,] firstMatrix, int[,] secondMatrix, int[,] result, int chunkSize, int chunkNumber)
        {
            for (int i = chunkNumber * chunkSize; i < Math.Min((chunkNumber + 1) * chunkSize, result.GetLength(0)); ++i)
            {
                for (int j = 0; j < result.GetLength(1); ++j)
                {
                    for (int k = 0; k < firstMatrix.GetLength(1); ++k)
                    {
                        result[i, j] += firstMatrix[i, k] * secondMatrix[k, j];
                    }
                }
            }
        }

        public static int[,] MultiplyInParallel(int[,] firstMatrix, int[,] secondMatrix)
        {
            if (firstMatrix.GetLength(1) != secondMatrix.GetLength(0))
            {
                throw new ArgumentException("The number of columns of the first matrix must match the number of rows of the second matrix");
            }

            var result = new int[firstMatrix.GetLength(0), secondMatrix.GetLength(1)];

            var threads = new Thread[Math.Min(Environment.ProcessorCount, result.GetLength(0))];
            var chunkSize = result.GetLength(0) / threads.Length + 1;

            for (int i = 0; i < threads.Length; ++i)
            {
                var localI = i;
                threads[i] = new Thread(() => MultiplyMatrixChunk(firstMatrix, secondMatrix, result, chunkSize, localI));
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            return result;
        }
    }
}
