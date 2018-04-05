using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hong.Algorithms
{
    public class Search
    {
        /// <summary>二分查找
        /// </summary>
        public int BinarySearch(string[] array, string key)
        {
            int lo = 0;
            int hi = array.Length - 1;
            int mid = 0;
            int compareResult = 0;

            while (lo <= hi)
            {
                mid = (lo + hi) / 2;
                compareResult = key.CompareTo(array[mid]);

                if (compareResult > 0)
                {
                    lo = mid + 1;
                }
                else if (compareResult < 0)
                {
                    hi = mid - 1;
                }
                else
                {
                    return mid;
                }
            }

            return -1;
        }
    }
}
