using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace MultiThreadSingleProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            Process.GetCurrentProcess().ProcessorAffinity =
                (IntPtr)0b00000001;
            Thread thread = new Thread(x =>
            {
                while (true)
                {
                    Thread.Sleep(30);
                }
            });
            thread.Priority = ThreadPriority.Highest;
            thread.IsBackground = true;
            thread.Start();
            AsyncAddSub(1);
        }
        static void AsyncAddSub(int times)
        {
            AddSub addSub = new AddSub();
            List<WaitHandle> waitHandles = new List<WaitHandle>();
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < times; i++)
            {
                waitHandles.Add(new AutoResetEvent(false));
                waitHandles.Add(new AutoResetEvent(false));
            }
            Random random = new Random();
            int maxLoop = random.Next(int.MaxValue-5, int.MaxValue);
            for (int i = 0; i < times; i++)
            {
                int idx = i;
                threads.Add(new Thread(x =>
                {
                    Console.WriteLine($"The {idx * 2} Thread {Thread.CurrentThread.ManagedThreadId} Begin");
                    addSub.Adds(maxLoop);
                    (waitHandles[idx * 2] as AutoResetEvent).Set();
                    Console.WriteLine($"The {idx * 2} Thread {Thread.CurrentThread.ManagedThreadId} Complete");
                }));
                threads.Add(new Thread(x =>
                {
                    Console.WriteLine($"The {idx * 2 + 1} Thread {Thread.CurrentThread.ManagedThreadId} Begin");
                    addSub.Subs(maxLoop);
                    (waitHandles[idx * 2 + 1] as AutoResetEvent).Set();
                    Console.WriteLine($"The {idx * 2 + 1} Thread {Thread.CurrentThread.ManagedThreadId} Complete");
                }));
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < times; i++)
            {
                threads[i * 2].Start();
                threads[i * 2 + 1].Start();
            }

            WaitHandle.WaitAll(waitHandles.ToArray());
            stopwatch.Stop();
            Console.WriteLine($"Counter={AddSub.counter}, {stopwatch.ElapsedMilliseconds:N0}ms");
        }
    }
    class AddSub
    {
        public static long counter = 0;
        public void Adds(int maxLoop)
        {
            for (int i = 0; i < maxLoop; i++)
            {
                counter = counter + 1;
                counter = counter + 1;
                counter = counter + 1;
            }
        }
        public void Subs(int maxLoop)
        {
            for (int i = 0; i < maxLoop; i++)
            {
                counter = counter - 1;
                counter = counter - 1;
                counter = counter - 1;
            }
        }
    }
}