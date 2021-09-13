using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;

namespace Task_1.Tests
{
    [TestFixture]
    public class MatrixOperationsTests
    {
        private int[,] firstMatrix =
        {
            { }
        };

        [TestCaseSource(nameof(CorrectTestDataForGeneration))]
        public void GenerateMatrixTest(int rowsNumber, int columnsNumber, int minNumber, int maxNumber)
        {
            var matrix = MatrixOperations.Generate(rowsNumber, columnsNumber, minNumber, maxNumber);
            Assert.AreEqual(rowsNumber, matrix.GetLength(0));
            Assert.AreEqual(columnsNumber, matrix.GetLength(1));

            foreach (var element in matrix)
            {
                Assert.IsTrue(element >= minNumber && element < maxNumber);
            }
        }

        [TestCaseSource(nameof(IncorrectTestDataForGeneration))]
        public void GenerateShouldThrowExceptionIfDataIsIncorrect(int rowsNumber, int columnsNumber, int minNumber, int maxNumber)
        {
            Assert.Throws<ArgumentException>(() => MatrixOperations.Generate(rowsNumber, columnsNumber, minNumber, maxNumber));
        }

        [TestCaseSource(nameof(TestDataForComparison))]
        public void CompareMatricesTest(int[,] firstMatrix, int[,] secondMatrix, bool expectedResult)
        {
            Assert.AreEqual(expectedResult, MatrixOperations.Compare(firstMatrix, secondMatrix));
        }

        [TestCaseSource(nameof(CorrectTestDataForGeneration))]
        public void WriteAndReadMatrixFromFileTest(int rowsNumber, int columnsNumber, int minNumber, int maxNumber)
        {
            var matrix = MatrixOperations.Generate(rowsNumber, columnsNumber, minNumber, maxNumber);
            MatrixOperations.WriteToFile(matrix, "../../../Matrix.txt");
            var readMatrix = MatrixOperations.ReadFromFile("../../../Matrix.txt");
            Assert.IsTrue(MatrixOperations.Compare(matrix, readMatrix));
            File.Delete("../../../Matrix.txt");
        }

        [TestCaseSource(nameof(IncorrectTestDataForReadingFromFile))]
        public void ReadShouldThrowExceptionIfDataIsIncorrect(string path)
        {
            Assert.Throws<ArgumentException>(() => MatrixOperations.ReadFromFile(path));
        }

        [TestCaseSource(nameof(CorrectTestDataForMultiplication))]
        public void MultiplyMatricesTest(int[,] firstMatrix, int[,] secondMatrix, int[,] expectedResult, Func<int[,], int[,], int[,]> matrixMultiplyFunction)
        {
            var result = matrixMultiplyFunction(firstMatrix, secondMatrix);
            Assert.IsTrue(MatrixOperations.Compare(expectedResult, result));
        }

        [TestCaseSource(nameof(IncorrectTestDataForMultiplication))]
        public void MultiplyMatricesShouldThrowExceptionIfDataIsIncorrect(int[,] firstMatrix, int[,] secondMatrix, Func<int[,], int[,], int[,]> matrixMultiplyFunction)
        {
            Assert.Throws<ArgumentException>(() => matrixMultiplyFunction(firstMatrix, secondMatrix));
        }

        [TestCaseSource(nameof(TestDataForComparisonOfMultiplications))]
        public void ParallelMultiplicationShouldHaveSameResultAsSimpleMultiplication(int rowsNumber, int columnsNumber, int length, int minNumber, int maxNumber)
        {
            var firstMatrix = MatrixOperations.Generate(rowsNumber, length, minNumber, maxNumber);
            var secondMatrix = MatrixOperations.Generate(length, columnsNumber, minNumber, maxNumber);
            var multiplicationResult = MatrixOperations.Multiply(firstMatrix, secondMatrix);
            var parallelMultiplicationResult = MatrixOperations.MultiplyInParallel(firstMatrix, secondMatrix);

            Assert.IsTrue(MatrixOperations.Compare(multiplicationResult, parallelMultiplicationResult));
        }

        private static IEnumerable<int[]> CorrectTestDataForGeneration()
        {
            yield return new int[] { 1, 1, 0, 2 };
            yield return new int[] { 1, 2, -3, 5 };
            yield return new int[] { 2, 3, -3, 5 };
            yield return new int[] { 10, 10, -5, 5 };
            yield return new int[] { 50, 50, -20, 20 };
        }

        private static IEnumerable<int[]> IncorrectTestDataForGeneration()
        {
            yield return new int[] { 1, 1, 2, 1 };
            yield return new int[] { -1, 2, -20, 20 };
            yield return new int[] { 5, 0, -20, 20 };
            yield return new int[] { 0, 2, -20, 20 };
            yield return new int[] { 5, -2, -20, 20 };
            yield return new int[] { -5, -2, -20, 20 };
        }

        private static IEnumerable<object[]> TestDataForComparison()
        {
            yield return new object[] { new int[,] { { 1, 2, 3 }, { 4, 5, 6 } }, new int[,] { { 1, 2, 3 }, { 4, 5, 6 } }, true };
            yield return new object[] { new int[,] { { 1, 2 }, { 4, 5 } }, new int[,] { { 1, 2 }, { 4, 5 } }, true };
            yield return new object[] { new int[,] { { 1, 0, 3 }, { 4, 5, 0 } }, new int[,] { { 1, 2, 3 }, { 4, 5, 6 } }, false };
            yield return new object[] { new int[,] { { 1, 2, 3 }, { 4, 5, 6 } }, new int[,] { { 1, 2, 0 }, { 4, 0, 6 } }, false };
            yield return new object[] { new int[,] { { 1, 2, 3 }, { 4, 5, 6 } }, new int[,] { { 1, 2 }, { 4, 5 } }, false };
            yield return new object[] { new int[,] { { 1, 2, 3 }, { 4, 5, 6 } }, new int[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } }, false };
        }

        private static IEnumerable<string> IncorrectTestDataForReadingFromFile()
        {
            yield return "../../../IncorrectPath.txt";
            yield return "../../../IncorrectMatrix.txt";
            yield return "../../../IncorrectMatrix2.txt";
        }

        private static IEnumerable<object[]> CorrectTestDataForMultiplication()
        {
            var functions = new Func<int[,], int[,], int[,]>[] { (firstMatrix, secondMatrix) => MatrixOperations.Multiply(firstMatrix, secondMatrix),
                                                                 (firstMatrix, secondMatrix) => MatrixOperations.MultiplyInParallel(firstMatrix, secondMatrix) };

            foreach (var fucntion in functions)
            {
                yield return new object[] { new int[,] { { 1, 2, 3 }, { 4, 5, 6 } }, new int[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } },
                                            new int[,] { { 22, 28 }, { 49, 64 } }, fucntion };
                yield return new object[] { new int[,] { { 1, 2 }, { 3, 4 } }, new int[,] { { 1, 2 }, { 3, 4 } },
                                            new int[,] { { 7, 10 }, { 15, 22 } }, fucntion };
                yield return new object[] { new int[,] { { 1, 0 } }, new int[,] { { 1, 2 }, { 3, 4 } },
                                            new int[,] { { 1, 2 } }, fucntion };
            }
        }

        private static IEnumerable<object[]> IncorrectTestDataForMultiplication()
        {
            var functions = new Func<int[,], int[,], int[,]>[] { (firstMatrix, secondMatrix) => MatrixOperations.Multiply(firstMatrix, secondMatrix),
                                                                 (firstMatrix, secondMatrix) => MatrixOperations.MultiplyInParallel(firstMatrix, secondMatrix) };

            foreach (var fucntion in functions)
            {
                yield return new object[] { new int[,] { { 1, 2, 3 }, { 4, 5, 6 } }, new int[,] { { 1, 2 }, { 3, 4 } }, fucntion };
                yield return new object[] { new int[,] { { 1, 2 }, { 3, 4 } }, new int[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } }, fucntion };
                yield return new object[] { new int[,] { { 1, 2 } }, new int[,] { { 1 } }, fucntion };
                yield return new object[] { new int[0, 1], new int[,] { { 1 } }, fucntion };
                yield return new object[] { new int[,] { { 1 } }, new int[1, 0], fucntion };
            }
        }

        private static IEnumerable<int[]> TestDataForComparisonOfMultiplications()
        {
            yield return new int[] { 1, 1, 2, -5, 5 };
            yield return new int[] { 10, 10, 20, -20, 20 };
            yield return new int[] { 30, 30, 30, -100, 100 };
            yield return new int[] { 40, 40, 30, -20, 20 };
        }
    }
}