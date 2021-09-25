using NUnit.Framework;
using System;
using System.Threading;

namespace Task_2.Tests
{
    [TestFixture]
    public class LazyTests
    {
        [Test]
        public void Test1()
        {
            var random = new Random();
            var a = LazyFactory.CreateMultithreadedLazy(() =>
            {
                var random = new Random();
                return random.Next(1, 10);
            });

            var b = LazyFactory.CreateSinglethreadedLazy(() =>
            {
                var random = new Random();
                return random.Next(1, 10);
            });

            var res = b.Get();
            var res2 = b.Get();
            var res3 = b.Get();

            var threads = new Thread[100];
            var resA = new int[100];

            for (int i = 0; i < 100; ++i)
            {
                var localI = i;
                threads[localI] = new Thread(() =>
                {
                    resA[localI] = a.Get();
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
        }
    }
}