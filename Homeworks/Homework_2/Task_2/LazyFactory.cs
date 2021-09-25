using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_2
{
    public static class LazyFactory
    {
        public static ILazy<T> CreateSinglethreadedLazy<T>(Func<T> supplier)
            => new SinglethreadedLazy<T>(supplier);

        public static ILazy<T> CreateMultithreadedLazy<T>(Func<T> supplier)
            => new MultithreadedLazy<T>(supplier);
    }
}