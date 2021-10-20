using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Task_3
{
    /// <summary>
    /// A class that implements a thread pool for performing tasks.
    /// </summary>
    public class MyThreadPool
    {
        private Thread[] threads;

        private ConcurrentQueue<Action> taskQueue = new();

        private CancellationTokenSource cancellationTokenSource = new();

        private AutoResetEvent isTaskAvailable = new(false);

        private object lockObject = new();

        private volatile int numberOfTasks = 0;

        /// <summary>
        /// Creates a new thread pool.
        /// </summary>
        /// <param name="numberOfThreads">Number of threads in the pool.</param>
        public MyThreadPool(int numberOfThreads)
        {
            if (numberOfThreads <= 0)
            {
                throw new ArgumentException("Incorrect number of threads");
            }

            threads = new Thread[numberOfThreads];

            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i] = new Thread(() => ExecuteTasks(cancellationTokenSource.Token));
                threads[i].Start();
            }
        }

        private class MyTask<TResult> : IMyTask<TResult>
        {
            private Func<TResult> function;

            private MyThreadPool threadPool;

            private TResult result;

            private ManualResetEvent resultIsCalculated = new(false);

            private Exception caughtExcpetion;

            private CancellationToken cancellationToken;

            private Queue<Action> localQueue = new();

            private object lockObject = new();

            public MyTask(Func<TResult> function, MyThreadPool threadPool)
            {
                this.function = function;
                this.threadPool = threadPool;
                cancellationToken = threadPool.cancellationTokenSource.Token;
            }

            public void Run()
            {
                try
                {
                    result = function();
                }
                catch (Exception exception)
                {
                    caughtExcpetion = exception;
                }

                lock (lockObject)
                {
                    IsCompleted = true;
                }

                resultIsCalculated.Set();
                function = null;

                foreach (var taskContinuation in localQueue)
                {
                    threadPool.taskQueue.Enqueue(taskContinuation);
                    threadPool.isTaskAvailable.Set();
                }
            }

            public bool IsCompleted { get; private set; }

            public TResult Result
            {
                get
                {
                    resultIsCalculated.WaitOne();
                    if (caughtExcpetion != null)
                    {
                        throw new AggregateException(caughtExcpetion);
                    }
                    return result;
                }
            }

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> function)
            {
                if (function == null)
                {
                    throw new ArgumentNullException("The function is null");
                }

                lock (lockObject)
                {
                    if (IsCompleted)
                    {
                        return threadPool.Submit(() => function(Result));
                    }

                    lock (threadPool.lockObject)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            throw new InvalidOperationException("Thread pool is shutdown");
                        }

                        var taskContinuation = new MyTask<TNewResult>(() => function(Result), threadPool);
                        localQueue.Enqueue(taskContinuation.Run);
                        Interlocked.Increment(ref threadPool.numberOfTasks);

                        return taskContinuation;
                    }
                }
            }
        }

        private void ExecuteTasks(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested || numberOfTasks > 0)
            {
                if (taskQueue.TryDequeue(out var taskRun))
                {
                    Interlocked.Decrement(ref numberOfTasks);
                    taskRun();
                }
                else
                {
                    isTaskAvailable.WaitOne();

                    if (cancellationToken.IsCancellationRequested || taskQueue.Count >= 2)
                    {
                        isTaskAvailable.Set();
                    }
                }
            }
        }

        /// <summary>
        /// Returns the task added to the thread pool.
        /// </summary>
        /// <typeparam name="TResult">Task result type.</typeparam>
        /// <param name="function">Task function.</param>
        public IMyTask<TResult> Submit<TResult>(Func<TResult> function)
        {
            if (function == null)
            {
                throw new ArgumentNullException("The function is null");
            }

            lock (lockObject)
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    throw new InvalidOperationException("Thread pool is shutdown");
                }

                var task = new MyTask<TResult>(function, this);

                taskQueue.Enqueue(task.Run);
                isTaskAvailable.Set();
                Interlocked.Increment(ref numberOfTasks);

                return task;
            }
        }

        /// <summary>
        /// Stops threads work in the thread pool
        /// </summary>
        public void Shutdown()
        {
            lock (lockObject)
            {
                cancellationTokenSource.Cancel();
            }

            isTaskAvailable.Set();

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
    }
}