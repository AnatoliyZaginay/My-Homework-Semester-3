using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_2
{
    public class MultithreadedLazy<T> : ILazy<T>
    {
        private Func<T> supplier;

        private T calculationResult;

        private volatile bool isCalculated = false;

        private object lockObject = new();

        public MultithreadedLazy(Func<T> supplier)
        {
            this.supplier = supplier;
        }

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