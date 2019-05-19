using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TraceHttpClient
{
    #region DTO 型別宣告
    public class LoginRequestDTO
    {
        public string Account { get; set; }
        public string Password { get; set; }
    }
    public class LoginResponseDTO
    {
        public int Id { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public int TokenExpireMinutes { get; set; }
        public string RefreshToken { get; set; }
        public int RefreshTokenExpireDays { get; set; }
        public string Image { get; set; }
    }
    /// <summary>
    /// 呼叫 API 回傳的制式格式
    /// </summary>
    public class APIResult
    {
        /// <summary>
        /// 此次呼叫 API 是否成功
        /// </summary>
        public bool Status { get; set; } = false;
        /// <summary>
        /// 呼叫 API 失敗的錯誤訊息
        /// </summary>
        public string Message { get; set; } = "";
        /// <summary>
        /// 呼叫此API所得到的其他內容
        /// </summary>
        public object Payload { get; set; }
    }
    #endregion

    #region 建立一個 HttpHandler ，用來記錄下當時 HTTP Request & Response 內容
    public class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine("HTTP Request 內容:");
            Console.WriteLine(new String('-', 40));
            Console.WriteLine(request.ToString());
            if (request.Content != null)
            {
                Console.WriteLine(await request.Content.ReadAsStringAsync());
            }
            Console.WriteLine();
            Console.WriteLine();

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            Console.WriteLine("HTTP Response 內容:");
            Console.WriteLine(new String('-', 40));
            Console.WriteLine(response.ToString());
            if (response.Content != null)
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            Console.WriteLine();
            Console.WriteLine();

            return response;
        }
    }
    #endregion

    #region 用來寫入檔案與讀取檔案的公用方法
    public class StorageUtility
    {
        /// <summary>
        /// 將所指定的字串寫入到指定目錄的檔案內
        /// </summary>
        /// <param name="folderName">目錄名稱</param>
        /// <param name="filename">檔案名稱</param>
        /// <param name="content">所要寫入的文字內容</param> 
        /// <returns></returns>
        public static async Task WriteToDataFileAsync(string rootFolder, string folderName, string filename, string content)
        {
            string rootPath = rootFolder;

            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentNullException(nameof(folderName));
            }

            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException(nameof(content));
            }

            try
            {
                #region 建立與取得指定路徑內的資料夾
                string fooPath = Path.Combine(rootPath, folderName);
                if (Directory.Exists(fooPath) == false)
                {
                    Directory.CreateDirectory(fooPath);
                }
                fooPath = Path.Combine(fooPath, filename);
                #endregion

                byte[] encodedText = Encoding.UTF8.GetBytes(content);

                using (FileStream sourceStream = new FileStream(fooPath,
                    FileMode.Create, FileAccess.Write, FileShare.None,
                    bufferSize: 4096, useAsync: true))
                {
                    await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
            }
        }

        /// <summary>
        /// 從指定目錄的檔案內將文字內容讀出
        /// </summary>
        /// <param name="folderName">目錄名稱</param>
        /// <param name="filename">檔案名稱</param>
        /// <returns>文字內容</returns>
        public static async Task<string> ReadFromDataFileAsync(string rootFolder, string folderName, string filename)
        {
            string content = "";
            string rootPath = rootFolder;

            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentNullException(nameof(folderName));
            }

            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            try
            {
                #region 建立與取得指定路徑內的資料夾
                string fooPath = Path.Combine(rootPath, folderName);
                if (Directory.Exists(fooPath) == false)
                {
                    Directory.CreateDirectory(fooPath);
                }
                fooPath = Path.Combine(fooPath, filename);
                #endregion

                if (File.Exists(fooPath) == false)
                {
                    return content;
                }

                using (FileStream sourceStream = new FileStream(fooPath,
                    FileMode.Open, FileAccess.Read, FileShare.Read,
                    bufferSize: 4096, useAsync: true))
                {
                    StringBuilder sb = new StringBuilder();

                    byte[] buffer = new byte[0x1000];
                    int numRead;
                    while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        string text = Encoding.UTF8.GetString(buffer, 0, numRead);
                        sb.Append(text);
                    }

                    content = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
            }

            return content.Trim();
        }
    }
    #endregion

    class Program
    {
        static async Task Main(string[] args)
        {
            await ShowHttpClientRequestResponse();

            Console.WriteLine("讀取檔案內容並轉換成為物件的範例程式碼");
            string fileContent = await StorageUtility.ReadFromDataFileAsync("", "MyDataFolder", "MyFilename.txt");
            LoginResponseDTO loginResponseDTO = JsonConvert.DeserializeObject<LoginResponseDTO>(fileContent);
            Console.WriteLine($"{Environment.NewLine}JWT Token{Environment.NewLine}");
            Console.WriteLine($"{loginResponseDTO.Token}");

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }

        private static async Task ShowHttpClientRequestResponse()
        {
            string url = "https://lobworkshop.azurewebsites.net/api/Login";
            LoginRequestDTO loginRequestDTO = new LoginRequestDTO()
            {
                Account = "user1",
                Password = "password1"
            };
            var httpJsonPayload = JsonConvert.SerializeObject(loginRequestDTO);
            HttpClient client = new HttpClient(new LoggingHandler(new HttpClientHandler()));
            HttpResponseMessage response = await client.PostAsync(url,
                new StringContent(httpJsonPayload, System.Text.Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"已經登入成功，將結果寫入到檔案中");
                String strResult = await response.Content.ReadAsStringAsync();
                APIResult apiResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                if (apiResult.Status == true)
                {
                    string itemJsonContent = apiResult.Payload.ToString();
                    LoginResponseDTO item = JsonConvert.DeserializeObject<LoginResponseDTO>(itemJsonContent, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });

                    string content = JsonConvert.SerializeObject(item);
                    await StorageUtility.WriteToDataFileAsync("", "MyDataFolder", "MyFilename.txt", content);
                }
            }
        }
    }
}
