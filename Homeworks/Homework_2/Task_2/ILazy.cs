using System;

namespace Task_2
{
    public interface ILazy<T>
    {
        public T Get();
    }
}