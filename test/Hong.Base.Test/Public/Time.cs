using System;
using System.Diagnostics;

namespace Hong.Test.Public
{
    public class Time
    {
        public static double UserTime(Action action)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            action();
            watch.Stop();

            return watch.Elapsed.TotalMilliseconds; 
        }

        public static double WhileUseTime(Action action,int whileCount)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for(var index = 0; index < whileCount; index++)
            {
                action();
            }
            watch.Stop();

            return watch.Elapsed.TotalMilliseconds;
        }
    }
}
