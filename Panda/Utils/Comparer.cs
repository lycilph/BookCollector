using System;
using System.Collections.Generic;

namespace Panda.Utils
{
    // Found here: https://stackoverflow.com/questions/6277760/can-i-use-linqs-except-with-a-lambda-expression-comparer
    public class Comparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> equality_comparer;

        public Comparer(Func<T, T, bool> equality_comparer)
        {
            this.equality_comparer = equality_comparer;
        }

        public bool Equals(T first, T second)
        {
            return equality_comparer(first, second);
        }

        public int GetHashCode(T value)
        {
            /*
             If you just return 0 for the hash the Equals comparer will kick in. 
             The underlying evaluation checks the hash and then short circuits the evaluation if it is false.
             Otherwise, it checks the Equals. If you force the hash to be true (by assuming 0 for both objects), 
             you will always fall through to the Equals check which is what we are always going for.
            */
            return 0;
        }
    }
}
