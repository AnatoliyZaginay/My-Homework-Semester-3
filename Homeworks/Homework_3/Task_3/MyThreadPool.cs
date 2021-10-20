using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;

namespace Task_3
{
    public class MyThreadPool
    {
        private Thread[] threads;
        private ConcurrentQueue<Action> taskQueue = new ConcurrentQueue<Action>();
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private AutoResetEvent isTaskAvailaible = new AutoResetEvent(false);

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
            private ManualResetEvent resulatIsCalculated = new ManualResetEvent(false);
            private Exception caughtExcpetion;

            public MyTask(Func<TResult> function, MyThreadPool threadPool)
            {
                this.function = function;
                this.threadPool = threadPool;
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

                IsCompleted = true;
                resulatIsCalculated.Set();
                function = null;
            }

            public bool IsCompleted { get; private set; }

            public TResult Result
            {
                get
                {
                    resulatIsCalculated.WaitOne();
                    if (caughtExcpetion != null)
                    {
                        throw new AggregateException(caughtExcpetion);
                    }
                    return result;
                }
            }

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> function)
            {

            }
        }

        private void ExecuteTasks(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested || !taskQueue.IsEmpty)
            {
                if (taskQueue.TryDequeue(out var taskRun))
                {
                    taskRun();
                }
                else
                {
                    isTaskAvailaible.WaitOne();
                }
            }
        }

        public IMyTask<TResult> Submit<TResult>(Func<TResult> function)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                throw new InvalidOperationException("Thread pool is shutdown");
            }

            if (function == null)
            {
                throw new ArgumentNullException("The function is null");
            }

            var task = new MyTask<TResult>(function, this);
            taskQueue.Enqueue(task.Run);
            isTaskAvailaible.Set();

            return task;
        }

        public void Shutdown()
        {
            cancellationTokenSource.Cancel();
            isTaskAvailaible.Set();

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
    }
}
