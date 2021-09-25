using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_2
{
    public class SinglethreadedLazy<T> : ILazy<T>
    {
        private Func<T> supplier;

        private T calculationResult;

        private bool isCalculated;

        public SinglethreadedLazy(Func<T> supplier)
        {
            this.supplier = supplier;
            isCalculated = false;
        }

        public T Get()
        {
            if (!isCalculated)
            {
                calculationResult = supplier();
                isCalculated = true;
            }

            return calculationResult;
        }
    }
}