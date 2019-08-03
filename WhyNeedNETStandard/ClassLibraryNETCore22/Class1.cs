using System;
using System.IO;
using System.Threading.Tasks;

namespace ClassLibraryNETCore22
{
    public class Class1
    {
        public async Task DoSomething()
        {
            // 在 .NET Core 2.2 下，可以有底下兩種同步與非同步的 WriteAllText 方法可以選擇
            // void WriteAllText(string path, string contents) 與
            // Task WriteAllTextAsync(string path, string contents, CancellationToken cancellationToken = default);
            //
            await File.WriteAllTextAsync("MyFile", "MyContent");
        }
    }
}
