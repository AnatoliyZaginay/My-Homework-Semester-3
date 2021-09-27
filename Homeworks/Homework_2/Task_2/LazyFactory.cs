using System;

namespace Task_2
{
    /// <summary>
    /// Static class for creating lazy.
    /// </summary>
    public static class LazyFactory
    {
        /// <summary>
        /// Creates a single-threaded lazy.
        /// </summary>
        /// <typeparam name="T">Type of the supplier's return value.</typeparam>
        /// <param name="supplier">Supplied function.</param>
        /// <returns>Single-threaded lazy.</returns>
        public static ILazy<T> CreateSinglethreadedLazy<T>(Func<T> supplier)
            => new SinglethreadedLazy<T>(supplier);

        /// <summary>
        /// Creates a multithreaded lazy.
        /// </summary>
        /// <typeparam name="T">Type of the supplier's return value.</typeparam>
        /// <param name="supplier">Supplied function.</param>
        /// <returns>Multithreaded lazy.</returns>
        public static ILazy<T> CreateMultithreadedLazy<T>(Func<T> supplier)
            => new MultithreadedLazy<T>(supplier);
    }
}