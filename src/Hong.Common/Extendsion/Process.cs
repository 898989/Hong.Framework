using System;

namespace Hong.Common.Extendsion
{
    public class Process
    {
        //        System.Diagnostics.Process p = Process.GetCurrentProcess();
        //        p.ProcessorAffinity = (IntPtr)0x0001; 

        //Process.ProcessorAffinity 设置当前CPU的屏蔽字，0x0001表示选用一号CPU，0x0002表示选用2号CPU。
        public static void Main(string[] args)
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();
            process.ProcessorAffinity = (IntPtr)0x0001;

            const int SAMPLING_COUNT = 200;
            const double PI = 3.14159;
            const int TOTAL_AMPLITUDE = 250; // the length of each time piece 
                                             //const int system_busy = 10; // take the system cpu consume into consideration 

            double[] busySpan = new double[SAMPLING_COUNT];
            int amplitude = (TOTAL_AMPLITUDE) / 2;
            double radian = 0.0;
            double radianIncreament = 2.0 / SAMPLING_COUNT;

            for (int i = 0; i < SAMPLING_COUNT; i++)
            {
                busySpan[i] = amplitude + Math.Sin(PI * radian) * amplitude;
                radian += radianIncreament;
            }
            
            int startTick = Environment.TickCount;

            for (int j = 0; ; j = (j + 1) % SAMPLING_COUNT)
            {
                startTick = Environment.TickCount;
                while ((Environment.TickCount - startTick) < busySpan[j])
                {
                    // 
                }

                System.Threading.Thread.Sleep(TOTAL_AMPLITUDE - (int)busySpan[j]);
            }
        }
    }
}
