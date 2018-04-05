using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Hong.Test.Public
{
    public class RunInThread
    {
        public static void Start(Action action, int threadCount)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            List<Task> task = new List<Task>();
            for (var index = 0; index < threadCount; index++)
            {
                task.Add(Task.Factory.StartNew(() => ThreadAction(action), TaskCreationOptions.HideScheduler));
            }

            Task.WaitAll(task.ToArray());
            watch.Stop();

            Debug.WriteLine("单方法并发线程任务执行完成 =>并发数:" + threadCount + "|总耗时: " + watch.Elapsed.TotalMilliseconds + "毫秒");
        }

        public static void Start(Action[] action, int threadCount)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Task masterTask = new Task(() =>
             {
                 for (var index = 0; index < threadCount; index++)
                 {
                     for (var actionIndex = 0; actionIndex < action.Length; actionIndex++)
                     {
                         var a = action[actionIndex];
                         new Task(() => ThreadAction(a), TaskCreationOptions.AttachedToParent).Start();
                     }
                 }
             });

            masterTask.Start();
            masterTask.Wait();
            watch.Stop();

            Debug.WriteLine("多方法交叉运行并发线程任务执行完成 =>并发数:" + threadCount + "|总耗时: " + watch.Elapsed.TotalMilliseconds + "毫秒");
        }

        static void ThreadAction(Action action)
        {
            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            //var hashCode = Thread.CurrentThread.GetHashCode();
            //Debug.WriteLine("RunInThread =>线程:" + hashCode + "开始");
            action();
            //watch.Stop();
            //Debug.WriteLine("RunInThread =>线程:" + hashCode + "结束, 用时:"+ watch.Elapsed.TotalMilliseconds);
        }
    }
}
