﻿using System;
using System.Diagnostics;

namespace Task_1
{
    class Program
    {
        static void Main(string[] args)
        {
            var firstMatrix = MatrixOperations.Generate(500, 500, -8, 10);
            var secondMatrix = MatrixOperations.Generate(500, 500, -8, 10);

            var timer = new Stopwatch();

            timer.Start();
            var firstResult = MatrixOperations.Multiply(firstMatrix, secondMatrix);
            timer.Stop();
            Console.WriteLine($"{timer.ElapsedMilliseconds}\n\n");

            timer.Restart();
            var secondResult = MatrixOperations.MultiplyInParallel(firstMatrix, secondMatrix);
            timer.Stop();
            Console.WriteLine($"{timer.ElapsedMilliseconds}\n\n");
        }
    }
}
