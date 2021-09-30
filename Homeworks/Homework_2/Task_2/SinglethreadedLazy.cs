using System;

namespace Task_2
{
    /// <summary>
    /// Single-threaded implementation of lazy.
    /// </summary>
    /// <typeparam name="T">Type of the supplier's return value.</typeparam>
    public class SinglethreadedLazy<T> : ILazy<T>
    {
        private Func<T> supplier;

        private T calculationResult;

        private bool isCalculated = false;

        /// <summary>
        /// Creates a single-threaded lazy.
        /// </summary>
        /// <param name="supplier">Supplied function.</param>
        public SinglethreadedLazy(Func<T> supplier)
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
            if (!isCalculated)
            {
                calculationResult = supplier();
                supplier = null;
                isCalculated = true;
            }

            return calculationResult;
        }
    }
}