using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleNETFramework472
{
    class Program
    {
        static void Main(string[] args)
        {
            var foo = new ClassLibraryNETFramework472.Class1();
            foo.DoSomething();
            var bar = new ClassLibraryNETFramework472.Class1();
            bar.DoSomething();
        }
    }
}
