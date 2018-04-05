using Hong.Common.Extendsion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Hong.Base.Test.Algorithms
{
    
    public class TestFeaturescs
    {
        [Fact]
        public void HightQueue()
        {
            In(0, 10);
            Out(0, 10);
        }

        [Fact]
        public void GetMD5()
        {
            var str = @"# This file contains the mappings of IP addresses to host names. Each
# entry should be kept on an individual line. The IP address should
# be placed in the first column followed by the corresponding host name.
# The IP address and the host name should be separated by at least one
# space.";
            Stopwatch watch = new Stopwatch();
            watch.Start();

            for(var i = 0; i < 10000; i++)
            {
                Security.GetMD532(str);
            }

            watch.Stop();

            var d = new Dictionary<string, string>();
            //d.Add(str, Security.GetMD532(str));

            for (var i = 0; i < 10000; i++)
            {
                d.Add(str+i, i.ToString());
            }

            str += "1000";
            watch.Reset();
            watch.Start();

            for (var i = 0; i < 10000; i++)
            {
                d.TryGetValue(str, out string md5);
            }
            watch.Stop();
        }

        Hong.Algorithms.Queue<int> fifo = new Hong.Algorithms.Queue<int>(120);
        private void In(int begin, int end)
        {
            for (var v = begin; v < end; v++)
            {
                fifo.Enque(v);
            }
        }

        private void Out(int begin, int end)
        {
            var data = new List<int>();

            for (var v = begin; v < end; v++)
            {
                data.Add(fifo.Deque(-1));
            }
        }
    }
}
