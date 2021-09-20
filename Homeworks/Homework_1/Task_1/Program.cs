using System;

namespace Task_1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Write("Specify the path to the file with the first matrix: ");
                var firstFilePath = Console.ReadLine();
                var firstMatrix = MatrixOperations.ReadFromFile(firstFilePath);

                Console.Write("Specify the path to the file with the second matrix: ");
                var secondFilePath = Console.ReadLine();
                var secondMatrix = MatrixOperations.ReadFromFile(secondFilePath);

                var resultMatrix = MatrixOperations.MultiplyInParallel(firstMatrix, secondMatrix);
                Console.Write("Specify the path to the file where the result of matrix multiplication will be written: ");
                var resultFilePath = Console.ReadLine();
                MatrixOperations.WriteToFile(resultMatrix, resultFilePath);

                Console.WriteLine("The result of matrix multiplication has been successfully written to the file");
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}