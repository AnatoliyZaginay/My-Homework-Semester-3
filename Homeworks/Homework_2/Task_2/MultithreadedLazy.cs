using System;

namespace Task_2
{
    /// <summary>
    /// Multithreaded implementation of lazy.
    /// </summary>
    /// <typeparam name="T">Type of the supplier's return value.</typeparam>
    public class MultithreadedLazy<T> : ILazy<T>
    {
        private Func<T> supplier;

        private T calculationResult;

        private volatile bool isCalculated = false;

        private object lockObject = new();

        /// <summary>
        /// Creates a multithreaded lazy.
        /// </summary>
        /// <param name="supplier">Supplied function.</param>
        public MultithreadedLazy(Func<T> supplier)
        {
            if (supplier == null)
            {
                throw new ArgumentNullException("Supplier must not be null");
            }

            this.supplier = supplier;
        }

        /// <summary>
        /// Returns supplier's return value (calculates it only once)
        /// </summary>
        public T Get()
        {
            if (isCalculated)
            {
                return calculationResult;
            }

            lock (lockObject)
            {
                if (!isCalculated)
                {
                    calculationResult = supplier();
                    isCalculated = true;
                }
            }

            return calculationResult;
        }
    }
}