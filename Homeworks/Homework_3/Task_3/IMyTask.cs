using System;

namespace Task_3
{
    /// <summary>
    /// Interface of the task that returns the result of the function.
    /// </summary>
    /// <typeparam name="TResult">Result type.</typeparam>
    public interface IMyTask<TResult>
    {
        /// <summary>
        /// True if the task is completed, otherwise false.
        /// </summary>
        public bool IsCompleted { get; }

        /// <summary>
        /// Result of the task.
        /// </summary>
        public TResult Result { get; }

        /// <summary>
        /// Returns a task that uses the result of this task.
        /// </summary>
        /// <typeparam name="TNewResult">New result type.</typeparam>
        /// <param name="function">Function of new task.</param>
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> function);
    }
}