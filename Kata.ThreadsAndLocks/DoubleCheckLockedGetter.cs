using System;

namespace Kata.ThreadsAndLocks
{
    /// <summary>
    ///     See http://en.wikipedia.org/wiki/Double-checked_locking
    /// </summary>
    public class DoubleCheckLockedGetter<T> where T : class
    {
        private readonly object _syncLock = new object();

        public T Get(Func<T> cachedGetter, Func<T> heavyweightGetter)
        {
            T instance = cachedGetter();

            if (instance == null)
            {
                lock (_syncLock)
                {
                    instance = cachedGetter() ?? heavyweightGetter();
                }
            }

            return instance;
        }
    }
}