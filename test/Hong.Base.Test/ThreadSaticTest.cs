using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hong.Test
{
    public class ThreadSaticTest
    {
        [ThreadStatic]
        private static string Secret = "init";
        private static AsyncLocal<int> asyncLocal = new AsyncLocal<int>();
        private static ThreadLocal<int> threadLocal = new ThreadLocal<int>();

        public static void Main()
        {
            var random = new Random();
            threadLocal.Value = random.Next(0,100);
            asyncLocal.Value = random.Next(101,200);

            Console.WriteLine("I on thread [{0}]\tSecret:{1}\tAsyncLocal:{2}\tThreadLocal:{3}",  Thread.CurrentThread.ManagedThreadId, Secret, asyncLocal.Value, threadLocal.Value);

            Start().Wait();
            Start().Wait();

            Console.ReadKey();
        }

        private static async Task<int> Start()
        {
            Secret = "moo";

            Console.WriteLine("S on thread [{0}]\tSecret:{1}\tAsyncLocal:{2}\tThreadLocal:{3}",  Thread.CurrentThread.ManagedThreadId, Secret, asyncLocal.Value, threadLocal.Value);

            await Sleepy();

            Console.WriteLine("F on thread [{0}]\tSecret:{1}\tAsyncLocal:{2}\tThreadLocal:{3}", Thread.CurrentThread.ManagedThreadId, Secret, asyncLocal.Value, threadLocal.Value);
            Console.WriteLine("--------------------------------");
            return 1;
        }

        private static async Task<int> Sleepy()
        {
            Console.WriteLine("W on thread [{0}]\tSecret:{1}\tAsyncLocal:{2}\tThreadLocal:{3}", Thread.CurrentThread.ManagedThreadId, Secret, asyncLocal.Value, threadLocal.Value);
            await Task.Delay(1000);
            Console.WriteLine("N on thread [{0}]\tSecret:{1}\tAsyncLocal:{2}\tThreadLocal:{3}", Thread.CurrentThread.ManagedThreadId, Secret, asyncLocal.Value, threadLocal.Value);
            return 1;
        }
    }
}
