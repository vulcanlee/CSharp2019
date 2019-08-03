using System;
using System.Threading;

namespace ConsoleNETCore22
{
    class Program
    {
        static void Main(string[] args)
        {
            var foo = new ClassLibraryNETCore22.Class1();
            foo.DoSomething().Wait();
            var bar = new ClassLibraryNETFramework472.Class1();
            bar.DoSomething();
        }
    }
}
