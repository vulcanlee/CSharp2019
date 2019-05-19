using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace UsingJWTToken
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
    public class InvoiceRequestDTO
    {
        public int Id { get; set; }
        public string InvoiceNo { get; set; }
        public UserDTO user { get; set; }
        public DateTime Date { get; set; }
        public string Memo { get; set; }
    }
    public class InvoiceResponseDTO
    {
        public int Id { get; set; }
        public string InvoiceNo { get; set; }
        public UserDTO user { get; set; }
        public DateTime Date { get; set; }
        public string Memo { get; set; }
    }
    public class UserDTO
    {
        public int Id { get; set; }
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
            // 登入系統，取得 JTW Token
            await LoginAsync();

            // 從檔案中取得 JWT 權杖 Token
            string fileContent = await StorageUtility.ReadFromDataFileAsync("", "MyDataFolder", "MyFilename.txt");
            LoginResponseDTO loginResponseDTO = JsonConvert.DeserializeObject<LoginResponseDTO>(fileContent);

            #region CRUD => Retrive 取得該使用者的發票資料
            List<InvoiceResponseDTO> invoiceResponseDTOs = await QueryInvoiceAsync(loginResponseDTO);
            PrintAllInvoice(invoiceResponseDTOs);
            #endregion

            #region CRUD => Create 新增發票資料
            InvoiceResponseDTO invoiceResponseDTO = await CreateInvoiceAsync(loginResponseDTO);
            #endregion

            #region CRUD => Retrive 取得該使用者的發票資料
            invoiceResponseDTOs = await QueryInvoiceAsync(loginResponseDTO);
            PrintAllInvoice(invoiceResponseDTOs);
            #endregion

            #region CRUD => Update 修改發票資料
            invoiceResponseDTO = await UpdateInvoiceAsync(loginResponseDTO, invoiceResponseDTO);
            #endregion

            #region CRUD => Retrive 取得該使用者的發票資料
            invoiceResponseDTOs = await QueryInvoiceAsync(loginResponseDTO);
            PrintAllInvoice(invoiceResponseDTOs);
            #endregion
            
            #region CRUD => Delete 刪除發票資料
            foreach (var item in invoiceResponseDTOs)
            {
                await DeleteInvoiceAsync(loginResponseDTO, item.Id);
            }
            #endregion

            #region CRUD => Retrive 取得該使用者的發票資料
            invoiceResponseDTOs = await QueryInvoiceAsync(loginResponseDTO);
            PrintAllInvoice(invoiceResponseDTOs);
            #endregion

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }

        private static void PrintAllInvoice(List<InvoiceResponseDTO> invoiceResponseDTOs)
        {
            Console.WriteLine("所有的發票");
            foreach (var item in invoiceResponseDTOs)
            {
                Console.WriteLine($"Id = {item.Id}");
                Console.WriteLine($"InvoiceNo = {item.InvoiceNo}");
                Console.WriteLine($"Memo = {item.Memo}");
                Console.WriteLine($"Date = {item.Date}");
                Console.WriteLine();
            }
        }

        private static async Task<List<InvoiceResponseDTO>> QueryInvoiceAsync(LoginResponseDTO loginResponseDTO)
        {
            List<InvoiceResponseDTO> invoiceResponseDTOs = new List<InvoiceResponseDTO>();
            string url = "https://lobworkshop.azurewebsites.net/api/Invoices";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponseDTO.Token);
            HttpResponseMessage response = await client.GetAsync(url);

            String strResult = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                APIResult apiResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                if (apiResult.Status == true)
                {
                    string itemJsonContent = apiResult.Payload.ToString();
                    Console.WriteLine($"成功查詢發票 : {itemJsonContent}");
                    invoiceResponseDTOs = JsonConvert.DeserializeObject<List<InvoiceResponseDTO>>(itemJsonContent, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                }
            }
            return invoiceResponseDTOs;
        }

        private static async Task<InvoiceResponseDTO> CreateInvoiceAsync(LoginResponseDTO loginResponseDTO)
        {
            InvoiceResponseDTO invoiceResponseDTO = new InvoiceResponseDTO();
            string url = "https://lobworkshop.azurewebsites.net/api/Invoices";
            InvoiceRequestDTO invoiceRequestDTO = new InvoiceRequestDTO()
            {
                InvoiceNo = "123",
                Memo = "查理王",
                Date = new DateTime(2019, 05, 20),
                user = new UserDTO()
                {
                    Id = loginResponseDTO.Id
                }
            };
            var httpJsonPayload = JsonConvert.SerializeObject(invoiceRequestDTO);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponseDTO.Token);
            HttpResponseMessage response = await client.PostAsync(url,
                new StringContent(httpJsonPayload, System.Text.Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                String strResult = await response.Content.ReadAsStringAsync();
                APIResult apiResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                if (apiResult.Status == true)
                {
                    string itemJsonContent = apiResult.Payload.ToString();
                    Console.WriteLine($"成功新增一筆發票 : {itemJsonContent}");
                    invoiceResponseDTO = JsonConvert.DeserializeObject<InvoiceResponseDTO>(itemJsonContent, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                }
            }
            return invoiceResponseDTO;
        }

        private static async Task<InvoiceResponseDTO> UpdateInvoiceAsync(LoginResponseDTO loginResponseDTO, InvoiceResponseDTO UpdateItem)
        {
            InvoiceResponseDTO invoiceResponseDTO = new InvoiceResponseDTO();
            string url = $"https://lobworkshop.azurewebsites.net/api/Invoices/{UpdateItem.Id}";
            InvoiceRequestDTO invoiceRequestDTO = new InvoiceRequestDTO()
            {
                Id = UpdateItem.Id,
                InvoiceNo = UpdateItem.InvoiceNo,
                Memo = "修正" +UpdateItem.Memo,
                Date = UpdateItem.Date.AddDays(5),
                user = UpdateItem.user
            };
            var httpJsonPayload = JsonConvert.SerializeObject(invoiceRequestDTO);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponseDTO.Token);
            HttpResponseMessage response = await client.PutAsync(url,
                new StringContent(httpJsonPayload, System.Text.Encoding.UTF8, "application/json"));

            String strResult = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                APIResult apiResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                if (apiResult.Status == true)
                {
                    string itemJsonContent = apiResult.Payload.ToString();
                    Console.WriteLine($"成功修改一筆發票 : {itemJsonContent}");
                    invoiceResponseDTO = JsonConvert.DeserializeObject<InvoiceResponseDTO>(itemJsonContent, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                }
            }
            return invoiceResponseDTO;
        }

        private static async Task<InvoiceResponseDTO> DeleteInvoiceAsync(LoginResponseDTO loginResponseDTO, int Id)
        {
            InvoiceResponseDTO invoiceResponseDTO = new InvoiceResponseDTO();
            string url = $"https://lobworkshop.azurewebsites.net/api/Invoices/{Id}";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponseDTO.Token);
            HttpResponseMessage response = await client.DeleteAsync(url);

            String strResult = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                APIResult apiResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                if (apiResult.Status == true)
                {
                    string itemJsonContent = apiResult.Payload.ToString();
                    Console.WriteLine($"成功刪除一筆發票 : {itemJsonContent}");
                    invoiceResponseDTO = JsonConvert.DeserializeObject<InvoiceResponseDTO>(itemJsonContent, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                }
            }
            return invoiceResponseDTO;
        }

        private static async Task LoginAsync()
        {
            string url = "https://lobworkshop.azurewebsites.net/api/Login";
            LoginRequestDTO loginRequestDTO = new LoginRequestDTO()
            {
                Account = "user50",
                Password = "password50"
            };
            var httpJsonPayload = JsonConvert.SerializeObject(loginRequestDTO);
            HttpClient client = new HttpClient();
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
