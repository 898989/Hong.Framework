using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hong.Test.AwaitAsync
{
    public class TestThreadStatic
    {
        [ThreadStatic]
        private static string Secret;

        void Main(string[] args)
        {
            Start().Wait();
            Console.ReadKey();
        }

        private static async Task Start()
        {
            Secret = "moo moo";
            Console.WriteLine("Started on thread [{0}]", Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("Secret is [{0}]", Secret);

            await Sleepy();

            Console.WriteLine("Finished on thread [{0}]", Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("Secret is [{0}]", Secret);
        }

        private static async Task Sleepy()
        {
            Console.WriteLine("Was on thread [{0}]", Thread.CurrentThread.ManagedThreadId);
            await Task.Delay(1000);
            Console.WriteLine("Now on thread [{0}]", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
