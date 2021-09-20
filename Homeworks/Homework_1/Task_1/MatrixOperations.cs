using System;
using System.IO;
using System.Threading;

namespace Task_1
{
    /// <summary>
    /// Static class for working with matrices.
    /// </summary>
    public static class MatrixOperations
    {
        /// <summary>
        /// Generates a matrix based on the specified parameters.
        /// </summary>
        /// <param name="rowsNumber">Number of rows in the matrix.</param>
        /// <param name="columnsNumber">Number of columns in the matrix.</param>
        /// <param name="minNumber">The minimum random number that can be contained in the generated matrix.</param>
        /// <param name="maxNumber">The maximum random number that can be contained in the generated matrix.</param>
        public static int[,] Generate(int rowsNumber, int columnsNumber, int minNumber, int maxNumber)
        {
            if (minNumber > maxNumber)
            {
                throw new ArgumentException("The minimum number must be less than the maximum");
            }

            if (rowsNumber <= 0 || columnsNumber <= 0)
            {
                throw new ArgumentException("Incorrect matrix sizes");
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

        /// <summary>
        /// Reads a matrix from a specified file.
        /// </summary>
        /// <param name="path">The path to the file containing the matrix.</param>
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

        /// <summary>
        /// Writes the matrix to the specified file.
        /// </summary>
        /// <param name="matrix">The matrix that will be written.</param>
        /// <param name="path">The path to the specified file.</param>
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

        /// <summary>
        /// Compares two matrices.
        /// </summary>
        /// <param name="firstMatrix">The first matrix.</param>
        /// <param name="secondMatrix">The second matrix.</param>
        /// <returns>Returns true if the matrices are the same, otherwise returns false.</returns>
        public static bool Compare(int[,] firstMatrix, int[,] secondMatrix)
        {
            if (firstMatrix.GetLength(0) != secondMatrix.GetLength(0) ||
                firstMatrix.GetLength(1) != secondMatrix.GetLength(1))
            {
                return false;
            }

            for (int i = 0; i < firstMatrix.GetLength(0); ++i)
            {
                for (int j = 0; j < firstMatrix.GetLength(1); ++j)
                {
                    if (firstMatrix[i, j] != secondMatrix[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Multiplies matrices in single-threaded mode.
        /// </summary>
        /// <param name="firstMatrix">The first matrix.</param>
        /// <param name="secondMatrix">The second matrix.</param>
        public static int[,] Multiply(int[,] firstMatrix, int[,] secondMatrix)
        {
            if (firstMatrix.GetLength(1) != secondMatrix.GetLength(0))
            {
                throw new ArgumentException("The number of columns of the first matrix must match the number of rows of the second matrix");
            }

            if (firstMatrix.GetLength(0) == 0 || firstMatrix.GetLength(1) == 0 ||
                secondMatrix.GetLength(0) == 0 || secondMatrix.GetLength(1) == 0)
            {
                throw new ArgumentException("The number of rows or columns in the matrix must not be zero");
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

        private static void MultiplyMatrixChunk(int[,] firstMatrix, int[,] secondMatrix, int[,] result, int chunkSize, int chunkNumber)
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

        /// <summary>
        /// Multiplies matrices in multithreaded mode.
        /// </summary>
        /// <param name="firstMatrix">The first matrix.</param>
        /// <param name="secondMatrix">The second matrix.</param>
        public static int[,] MultiplyInParallel(int[,] firstMatrix, int[,] secondMatrix)
        {
            if (firstMatrix.GetLength(1) != secondMatrix.GetLength(0))
            {
                throw new ArgumentException("The number of columns of the first matrix must match the number of rows of the second matrix");
            }

            if (firstMatrix.GetLength(0) == 0 || firstMatrix.GetLength(1) == 0 ||
                secondMatrix.GetLength(0) == 0 || secondMatrix.GetLength(1) == 0)
            {
                throw new ArgumentException("The number of rows or columns in the matrix must not be zero");
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