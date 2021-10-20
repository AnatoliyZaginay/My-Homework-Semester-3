using NUnit.Framework;
using System;
using System.Threading;

namespace Task_3.Tests
{
    [TestFixture]
    public class Tests
    {
        private volatile int actualNumberOfThreads;
        private int numberOfThreads = Environment.ProcessorCount;

        [Test]
        public void NumberOfThreadsTest()
        {
            var tasks = new IMyTask<int>[numberOfThreads + 1];

            var countDownEvent = new CountdownEvent(1);
            var countDownEventForThreads = new CountdownEvent(numberOfThreads);

            var threadPool = new MyThreadPool(numberOfThreads);

            for (var i = 0; i < tasks.Length; ++i)
            {
                tasks[i] = threadPool.Submit(() =>
                {
                    Interlocked.Increment(ref actualNumberOfThreads);

                    countDownEventForThreads.Signal();
                    countDownEvent.Wait();

                    return 0;
                });
            }

            countDownEventForThreads.Wait();

            Assert.AreEqual(numberOfThreads, actualNumberOfThreads);

            countDownEvent.Signal();

            threadPool.Shutdown();
        }

        [Test]
        public void ThreadPoolShouldThrowExceptionIfNumberOfThreadsIsIncorrect()
        {
            Assert.Throws<ArgumentException>(() => new MyThreadPool(0));
            Assert.Throws<ArgumentException>(() => new MyThreadPool(-2));
        }

        [Test]
        public void TaskResultShouldBeCorrect()
        {
            var tasks = new IMyTask<int>[numberOfThreads * 2];
            var threadPool = new MyThreadPool(numberOfThreads);

            for (var i = 0; i < tasks.Length; ++i)
            {
                var localI = i;
                tasks[i] = threadPool.Submit(() => localI * 2);
            }

            for (int i = 0; i < tasks.Length; ++i)
            {
                Assert.AreEqual(i * 2, tasks[i].Result);
            }

            threadPool.Shutdown();
        }

        [Test]
        public void TaskShouldThrowExceptionIfFunctionIsNull()
        {
            var threadPool = new MyThreadPool(numberOfThreads);
            Assert.Throws<ArgumentNullException>(() => threadPool.Submit<int>(null));
            threadPool.Shutdown();
        }

        [Test]
        public void TaskResultShouldThrowAggregateExceptionIfFunctionThrowException()
        {
            var threadPool = new MyThreadPool(numberOfThreads);
            var x = 2;
            var task = threadPool.Submit(() => x / 0);
            Assert.Throws<AggregateException>(() => x = task.Result);
            threadPool.Shutdown();
        }

        [Test]
        public void ThreadPoolShouldWorkCorrecltyWithTasksAfterShutdown()
        {
            var tasks = new IMyTask<int>[numberOfThreads * 2];
            var threadPool = new MyThreadPool(numberOfThreads);

            for (var i = 0; i < tasks.Length; ++i)
            {
                var localI = i;
                tasks[i] = threadPool.Submit(() => localI * 2);
            }

            threadPool.Shutdown();

            for (int i = 0; i < numberOfThreads; ++i)
            {
                Assert.Throws<InvalidOperationException>(() => threadPool.Submit(() => 0));
            }

            for (int i = 0; i < tasks.Length; ++i)
            {
                Assert.AreEqual(i * 2, tasks[i].Result);
            }
        }

        [Test]
        public void TaskContinuationResultShouldBeCorrect()
        {
            var taskContinuations = new IMyTask<string>[numberOfThreads * 2];
            var threadPool = new MyThreadPool(numberOfThreads);

            for (var i = 0; i < taskContinuations.Length; ++i)
            {
                var localI = i;
                taskContinuations[i] = threadPool.Submit(() => localI * 2).ContinueWith(x => x - 1).ContinueWith(x => x.ToString());
            }

            for (int i = 0; i < taskContinuations.Length; ++i)
            {
                Assert.AreEqual($"{i * 2 - 1}", taskContinuations[i].Result);
            }

            threadPool.Shutdown();
        }

        [Test]
        public void ThreadPoolShouldWorkCorrecltyWithTaskContinuationsAfterShutdown()
        {
            var tasks = new IMyTask<int>[numberOfThreads * 2];
            var taskContinuations = new IMyTask<string>[numberOfThreads * 2];
            var threadPool = new MyThreadPool(numberOfThreads);

            for (var i = 0; i < tasks.Length; ++i)
            {
                var localI = i;
                tasks[i] = threadPool.Submit(() => localI * 2);
                taskContinuations[i] = tasks[localI].ContinueWith(x => x.ToString());
            }

            threadPool.Shutdown();

            for (int i = 0; i < numberOfThreads; ++i)
            {
                Assert.Throws<InvalidOperationException>(() => threadPool.Submit(() => 0).ContinueWith(x => x.ToString()));
            }

            for (int i = 0; i < tasks.Length; ++i)
            {
                Assert.AreEqual(i * 2, tasks[i].Result);
                Assert.AreEqual($"{i * 2}", taskContinuations[i].Result);
            }
        }
    }
}