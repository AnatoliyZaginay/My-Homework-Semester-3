using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Task_2.Tests
{
    [TestFixture]
    public class LazyTests
    {
        private int value;

        private const int expectedValue = 1;

        private const int iterationsNumber = 100;

        [SetUp]
        public void Setup()
        {
            value = 0;
        }

        [TestCaseSource(nameof(TestCases))]
        public void SinglethreadedLazyTest(Func<Func<int>, ILazy<int>> createLazy)
        {
            var lazy = createLazy(() => ++value);
            
            for (int i = 0; i < iterationsNumber; ++i)
            {
                var result = lazy.Get();
                Assert.AreEqual(expectedValue, result);
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public void CreateLazyShouldThrowExceptionIfSupplierIsNull(Func<Func<int>, ILazy<int>> createLazy)
            => Assert.Throws<ArgumentNullException>(() => createLazy(null));

        [Test]
        public void MultithreadedLazyTest()
        {
            var lazy = LazyFactory.CreateMultithreadedLazy(() => Interlocked.Increment(ref value));
            var threads = new Thread[Environment.ProcessorCount];
            var results = new int[threads.Length, iterationsNumber];

            for (int i = 0; i < threads.Length; ++i)
            {
                var localI = i;
                threads[i] = new Thread(() =>
                {
                    for (int j = 0; j < iterationsNumber; ++j)
                    {
                        results[localI, j] = lazy.Get();
                    }
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            foreach (var result in results)
            {
                Assert.AreEqual(expectedValue, result);
            }
        }

        [Test]
        public void AnotherMultithreadedLazyTest()
        {
            var lazy = LazyFactory.CreateMultithreadedLazy(() =>
            {
                var random = new Random();
                return random.Next(10000);
            });
            var threads = new Thread[Environment.ProcessorCount];
            var results = new int[threads.Length, iterationsNumber];

            for (int i = 0; i < threads.Length; ++i)
            {
                var localI = i;
                threads[i] = new Thread(() =>
                {
                    for (int j = 0; j < iterationsNumber; ++j)
                    {
                        results[localI, j] = lazy.Get();
                    }
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            foreach (var result in results)
            {
                Assert.AreEqual(results[0, 0], result);
            }
        }

        private static IEnumerable<Func<Func<int>, ILazy<int>>> TestCases()
        {
            yield return LazyFactory.CreateSinglethreadedLazy;
            yield return LazyFactory.CreateMultithreadedLazy;
        }
    }
}