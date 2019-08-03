using System;
using System.IO;

namespace ClassLibraryNETStandard
{
    public class Class1
    {
        public void DoSomething()
        {
            // 在 .NET Standard 2.0 下，可以僅有底下同步 WriteAllText 方法可以使用
            // void WriteAllText(string path, string contents);
            //
            File.WriteAllText("MyFile", "MyContent");
        }
    }
}
