using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Hong.Test.Algorithms
{
    public class AlgorithmsSearch
    {
        [Fact]
        public void BinnarySearch()
        {
            string[] keyWords = new string[] {
                "张","习","李","江","王","周","台","西","法","美","日","民","华","邓","小","大","中"
            };

            Array.Sort(keyWords);

            int a = BinarySearch(keyWords, "江");
        }

        public int BinarySearch(string[] array, string key)
        {
            var lo = 0;
            var hi = array.Length - 1;
            var mid = 0;

            while (lo <= hi)
            {
                mid = (lo+hi) / 2;
                int a = key.CompareTo(array[mid]);
                if (a > 0)
                {
                    lo = mid + 1;
                }
                else if (a < 0)
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
