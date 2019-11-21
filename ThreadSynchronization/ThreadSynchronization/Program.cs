using System;
using System.Diagnostics;
using System.Threading;

namespace ThreadSynchronization
{
    class Program
    {
        static void Main(string[] args)
        {
            var processorsX = Convert.ToInt32("10101000", 2);
            if (args.Length != 3)
            {
                Console.WriteLine("需要傳入引數 : 多執行緒模式 計算方式 使用CPU模式");
                Console.WriteLine("多執行緒模式 : Yes , No");
                Console.WriteLine("計算方式 : NoLock , UserModeLock , UsingNETLock , NoLockByLocal");
                Console.WriteLine("使用CPU模式 : 1000000 , 11000000 , 10100000 , 11110000 , 10101000");
                return;
            }
            var multiThread = args[0];
            var type = args[1];
            var cpu = args[2];
            var processors = Convert.ToInt32(cpu, 2);

            AddSubAction addSubAction = (AddSubAction)Enum.Parse(typeof(AddSubAction), type);
            Process.GetCurrentProcess().ProcessorAffinity =
                (IntPtr)processors;

            if (multiThread.ToLower() == "no")
            {
                SyncAddSub();
            }
            else
            {
                AsyncAddSub(addSubAction);
            }
        }
        static void AsyncAddSub(AddSubAction addSubAction)
        {
            WaitHandle[] waitHandles = new WaitHandle[]
            {
                new AutoResetEvent(false),
                new AutoResetEvent(false)
            };
            AddSub addSub = new AddSub();
            Thread thread1 = new Thread(x =>
            {
                addSub.Adds(addSubAction);
                (waitHandles[0] as AutoResetEvent).Set();
            });
            Thread thread2 = new Thread(x =>
            {
                addSub.Subs(addSubAction);
                (waitHandles[1] as AutoResetEvent).Set();
            });

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            thread1.Start();
            thread2.Start();

            WaitHandle.WaitAll(waitHandles);
            stopwatch.Stop();
            Console.WriteLine($"Counter={AddSub.counter}, {stopwatch.ElapsedMilliseconds:N0}ms");
        }
        static void SyncAddSub()
        {
            AddSub addSub = new AddSub();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            addSub.Adds();
            addSub.Subs();
            stopwatch.Stop();
            Console.WriteLine($"Counter={AddSub.counter}, {stopwatch.ElapsedMilliseconds:N0}ms");
        }
    }

    class AddSub
    {
        public static int counter = 0;
        public static object locker = new object();
        public void Adds(AddSubAction addSubAction = AddSubAction.NoLock)
        {
            int localCounter = 0;
            for (int i = 0; i < int.MaxValue; i++)
            {
                if (addSubAction == AddSubAction.NoLock)
                {
                    counter++;
                }
                else if (addSubAction == AddSubAction.UserModeLock)
                {
                    Interlocked.Add(ref counter, 1);
                }
                else if (addSubAction == AddSubAction.UsingNETLock)
                {
                    lock (locker)
                    {
                        counter++;
                    }
                }
                else if (addSubAction == AddSubAction.NoLockByLocal)
                {
                    localCounter++;
                }
            }
            if (addSubAction == AddSubAction.NoLockByLocal)
            {
                Interlocked.Add(ref counter, localCounter);
            }
        }
        public void Subs(AddSubAction addSubAction = AddSubAction.NoLock)
        {
            int localCounter = 0;
            for (int i = 0; i < int.MaxValue; i++)
            {
                if (addSubAction == AddSubAction.NoLock)
                {
                    counter--;
                }
                else if (addSubAction == AddSubAction.UserModeLock)
                {
                    Interlocked.Add(ref counter, -1);
                }
                else if (addSubAction == AddSubAction.UsingNETLock)
                {
                    lock (locker)
                    {
                        counter--;
                    }
                }
                else if (addSubAction == AddSubAction.NoLockByLocal)
                {
                    localCounter--;
                }
            }
            if (addSubAction == AddSubAction.NoLockByLocal)
            {
                Interlocked.Add(ref counter, localCounter);
            }
        }
    }
    enum AddSubAction
    {
        NoLock,
        UserModeLock,
        NoLockByLocal,
        UsingNETLock
    }
}
