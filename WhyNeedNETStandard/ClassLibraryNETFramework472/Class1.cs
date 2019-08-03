using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryNETFramework472
{
    public class Class1
    {
        public void DoSomething()
        {
            // 在 .NET Framework 4.7.2 下，可以僅有底下同步 WriteAllText 方法可以使用
            // void WriteAllText(string path, string contents);
            //
            File.WriteAllText("MyFile", "MyContent");
            // 底下的敘述可以在 .NET Framework 4.7.2 下運行
            // 可是在 .NET Core 2.2 下運行卻會有例外異常拋出
            var foo = System.Text.Encoding.GetEncoding(1252);

        }
        public void DoWebSomething()
        {
            // 在 .NET Framework 下，HttpClient 需要在 .NET Framework 4.5以上才有支援
            // void WriteAllText(string path, string contents);
            //
            HttpClient client = new HttpClient();
        }
    }
}
