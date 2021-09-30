using System;

namespace Task_2
{
    /// <summary>
    /// Lazy interface.
    /// </summary>
    /// <typeparam name="T">Type of the supplier's return value.</typeparam>
    public interface ILazy<out T>
    {
        /// <summary>
        /// Returns supplier's return value (calculates it only once)
        /// </summary>
        public T Get();
    }
}