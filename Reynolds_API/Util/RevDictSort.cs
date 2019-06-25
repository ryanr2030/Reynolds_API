using System;
using System.Collections.Generic;

namespace MiscUtil.Collections
/*
 * Utility class to overide the original compare function for sorted  
 * dictionaries to maintain O(log(n)) time complexity
 */
{

    public sealed class RevComparer<T> : IComparer<T>
    {
        
        readonly IComparer<T> comparer;

        //getter to access the original comparer
        public IComparer<T> Comparer
        {
            get { return comparer; }
        }

        public RevComparer(IComparer<T> original)
        {
            if (original == null)
                throw new ArgumentNullException("original");
            this.comparer = original;
        }

        //use the orginal comparer passing the values in, in reversed order
        public int Compare(T x, T y)
        {
            return comparer.Compare(y, x);
        }
    }
}