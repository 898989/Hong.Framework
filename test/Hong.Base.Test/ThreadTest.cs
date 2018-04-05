using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hong.Test
{
    public class ThreadTest
    {
        [Xunit.Fact]
        public void ThreadTestMain()
        {
            // Thread-Local variable that yields a name for a thread
            System.Threading.ThreadLocal<string> ThreadName = new System.Threading.ThreadLocal<string>(() =>
            {
                return "Thread" + System.Threading.Thread.CurrentThread.ManagedThreadId;
            });

            // Action that prints out ThreadName for the current thread
            Action action = () =>
            {
                // If ThreadName.IsValueCreated is true, it means that we are not the
                // first action to run on this thread.
                bool repeat = ThreadName.IsValueCreated;
                //if (!repeat)
                //{
                //    ThreadName.Value = "新的值";
                //}

                Console.WriteLine("ThreadName = {0} {1}", ThreadName.Value, repeat ? "(repeat)" : "");
            };

            ThreadName.Value = "主线程";

            // Launch eight of them.  On 4 cores or less, you should see some repeat ThreadNames
            Parallel.Invoke(action, action, action, action, action, action, action, action);

            // Dispose when you are done
            ThreadName.Dispose();
        }
    }
}
