using HttpClientCallJWTAPI.DTOs;
using HttpClientCallJWTAPI.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientCallJWTAPI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            LoginManager loginManager = new LoginManager();
            Output("進行登入身分驗證");
            APIResult result = await loginManager.PostAsync(new LoginRequestDTO()
            {
                Account = "user50",
                Password = "password50"
            });
            if (result.Status == true)
            {
                Console.WriteLine($"登入成功");
                Console.WriteLine($"{result.Payload}");
            }
            else
            {
                Console.WriteLine($"登入失敗");
                Console.WriteLine($"{result.Message}");
            }
            Thread.Sleep(2000);

            Output("利用取得的 JTW Token 呼叫取得部門資訊 Web API");
            DepartmentsManager departmentsManager = new DepartmentsManager();
            result = await departmentsManager.GetAsync(loginManager.SingleItem.Token);
            if (result.Status == true)
            {
                Console.WriteLine($"取得部門資料成功");
                Console.WriteLine($"{result.Payload}");
            }
            else
            {
                Console.WriteLine($"取得部門資料失敗");
                Console.WriteLine($"{result.Message}");
            }

            Console.WriteLine("等候10秒鐘，等待 JWT Token 失效");
            await Task.Delay(10000);

            departmentsManager = new DepartmentsManager();
            Output("再次呼叫取得部門資訊 Web API，不過，該 JWT Token已經失效了");
            result = await departmentsManager.GetAsync(loginManager.SingleItem.Token);
            if (result.Status == true)
            {
                Console.WriteLine($"取得部門資料成功");
                Console.WriteLine($"{result.Payload}");
            }
            else
            {
                Console.WriteLine($"取得部門資料失敗");
                Console.WriteLine($"{result.Message}");
            }
            Thread.Sleep(2000);

            RefreshTokenService refreshTokenService = new RefreshTokenService();
            Output("呼叫更新 JWT Token API，取得更新的 JWT Token");
            result = await refreshTokenService.GetAsync(loginManager.SingleItem.RefreshToken);
            if (result.Status == true)
            {
                Console.WriteLine($"更新 JWT Token 成功");
                Console.WriteLine($"{result.Payload}");
            }
            else
            {
                Console.WriteLine($"更新 JWT Token 失敗");
                Console.WriteLine($"{result.Message}");
            }
            Thread.Sleep(2000);

            departmentsManager = new DepartmentsManager();
            Output("再次呼叫取得部門資訊 Web API，不過，使用剛剛取得的更新 JWT Token");
            result = await departmentsManager.GetAsync(refreshTokenService.SingleItem.Token);
            if (result.Status == true)
            {
                Console.WriteLine($"取得部門資料成功");
                Console.WriteLine($"{result.Payload}");
            }
            else
            {
                Console.WriteLine($"取得部門資料失敗");
                Console.WriteLine($"{result.Message}");
            }
            Thread.Sleep(2000);

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }

        public static void Output(string message)
        {
            Console.WriteLine(message);
            Thread.Sleep(2000);
        }
    }






    #region Constants
    public static class Constants
    {
        public static string HostAPI = "https://lobworkshop.azurewebsites.net/api";
        public static string JSONDataKeyName = "JSON";
    }
    #endregion

    #region StorageUtility
    /// <summary>
    /// Storage 相關的 API
    /// http://www.nudoq.org/#!/Packages/PCLStorage/PCLStorage/FileSystem
    /// </summary>
    public class StorageUtility
    {
        /// <summary>
        /// 將所指定的字串寫入到指定目錄的檔案內
        /// </summary>
        /// <param name="folderName">目錄名稱</param>
        /// <param name="filename">檔案名稱</param>
        /// <param name="content">所要寫入的文字內容</param> 
        /// <returns></returns>
        public static async Task WriteToDataFileAsync(string folderName, string filename, string content)
        {
            //string rootPath = FileSystem.AppDataDirectory;
            string rootPath = Environment.CurrentDirectory;

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
        public static async Task<string> ReadFromDataFileAsync(string folderName, string filename)
        {
            string content = "";
            //string rootPath = FileSystem.AppDataDirectory;
            string rootPath = Environment.CurrentDirectory;

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

    #region APIPayloadHelper
    public static class APIPayloadHelper
    {
        /// <summary>
        /// 取得變數名稱
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static string getName(Expression<Func<string>> expr)
        {
            return ((MemberExpression)expr.Body).Member.Name;
        }

        public static string getStringValue(Expression<Func<string>> expr)
        {
            return ((MemberExpression)expr.Body).Member.Name;
        }

        /// <summary>
        /// 取得變數名稱
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static string getName(Expression<Func<int>> expr)
        {
            return ((MemberExpression)expr.Body).Member.Name;
        }

        /// <summary>
        /// 取得呼叫的方法名稱
        /// </summary>
        /// <param name="caller"></param>
        /// <returns></returns>
        public static string getCallerName([CallerMemberName]string caller = "")
        {
            return caller;
        }

        /// <summary>
        /// Get使用(將字典轉換成QueryString)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string ToQueryString(this Dictionary<string, string> dic)
        {
            string queryString = "?";

            foreach (string key in dic.Keys)
            {
                queryString += key + "=" + dic[key] + "&";
            }

            queryString = queryString.Remove(queryString.Length - 1, 1);

            return queryString;
        }

        /// <summary>
        /// Post使用(將字典轉換成Multipart)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static MultipartFormDataContent ToMultipartFormDataContent(this Dictionary<string, string> dic)
        {
            MultipartFormDataContent form = new MultipartFormDataContent();

            foreach (string key in dic.Keys)
            {
                form.Add(new StringContent(dic[key]), String.Format("\"{0}\"", key));
            }

            return form;
        }

        /// <summary>
        /// Post使用(將字典轉換成 FormUrlEncoded)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static FormUrlEncodedContent ToFormUrlEncodedContent(this Dictionary<string, string> dic)
        {
            FormUrlEncodedContent form = new FormUrlEncodedContent(dic);

            return form;
        }
    }
    #endregion

    #region HTTPPayloadDictionary
    /// <summary>
    /// Dictionary<String, String>的資料型態，主要用於 Http的 Get & Post 的傳遞資料儲存地方
    /// </summary>
    public class HTTPPayloadDictionary : Dictionary<string, string>
    {

        /// <summary>
        /// 傳遞屬性的Lambda表示式，自動取出該屬性的名稱與值出來，並且加入到Dictionary內
        /// </summary>
        /// <typeparam name="T">傳遞屬性變數的資料類型</typeparam>
        /// <param name="expr">Lambda表示式，該屬性名稱即為要傳遞的參數名稱</param>
        public void AddItem<T>(Expression<Func<T>> expr)
        {
            string propertyName = ((MemberExpression)expr.Body).Member.Name;
            T propertyValue = expr.Compile()();
            this.Add(propertyName, propertyValue.ToString());
        }

        /// <summary>
        /// 傳遞屬性的Lambda表示式(無須指定屬性的資料型別)，自動取出該屬性的名稱與值出來，並且加入到Dictionary內
        /// </summary>
        /// <param name="expr">Lambda表示式，該屬性名稱即為要傳遞的參數名稱</param>
        public void AddStringItem(Expression<Func<object>> expr)
        {
            string propertyName = ((MemberExpression)expr.Body).Member.Name;
            object propertyValue = expr.Compile()();
            this.Add(propertyName, propertyValue.ToString());
        }
        public void AddStringItem(string propertyName, string propertyValue)
        {
            this.Add(propertyName, propertyValue.ToString());
        }
    }
    #endregion

    #region BaseWebAPI
    /// <summary>
    /// 存取Http服務的Base Class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseWebAPI<T>
    {
        #region Field
        /// <summary>
        /// WebAPI主機位置
        /// </summary>
        public string host = Constants.HostAPI;

        /// <summary>
        /// WebAPI方法網址 
        /// </summary>
        public string restURL = "";
        //public string AuthenticationHeaderBearerTokenValue = "";
        /// <summary>
        /// 指定 HTTP 標頭 Bearer 內，要放置的 JWT Token 值
        /// </summary>
        public string AuthenticationHeaderBearerTokenValue { get; set; }
        /// <summary>
        /// 要呼叫 REST API 的額外路由路徑
        /// </summary>
        public string RouteURL { get; set; } = "";
        /// <summary>
        /// 成功呼叫完成 API 之後，是否要儲存到本機檔案系統內
        /// </summary>
        public bool NeedSaveData { get; set; }
        /// <summary>
        /// 是否為集合型別的物件
        /// </summary>
        public bool IsCollectionType { get; set; } = true;
        /// <summary>
        /// 要傳遞的 HTTP Payload 使用的編碼格式
        /// </summary>
        public EnctypeMethod EncodingType;
        /// <summary>
        /// 資料夾名稱
        /// </summary>
        //public string CurrentFolderName = "";
        public string CurrentFolderName { get; set; } = "";
        public string SubFolderName { get; set; } = "";
        public string TopDataFolderName { get; set; } = "Data";

        /// <summary>
        /// 檔案名稱
        /// </summary>
        public string DataFileName { get; set; } = "";


        #region 系統用到的訊息字串
        public static readonly string APIInternalError = "System Exception = null, Result = null";
        #endregion

        #endregion

        // =========================================================================================================

        #region protected

        #endregion

        // =========================================================================================================

        #region Public
        /// <summary>
        /// 透過Http取得的資料，也許是一個物件，也許是List
        /// </summary>
        public List<T> Items { get; set; }
        public T SingleItem { get; set; }
        /// <summary>
        /// 此次呼叫的處理結果
        /// </summary>
        public APIResult ManagerResult { get; set; }

        #endregion

        // =========================================================================================================

        /// <summary>
        /// 建構子，經由繼承後使用反射取得類別的名稱當作，檔案名稱及WebAPI的方法名稱
        /// </summary>
        public BaseWebAPI()
        {
            CurrentFolderName = TopDataFolderName;
            restURL = "";
            DataFileName = this.GetType().Name;
            //子資料夾名稱 = 資料檔案名稱;
            this.ManagerResult = new APIResult();
            EncodingType = EnctypeMethod.JSON;
        }

        ///// <summary>
        ///// 建立存取 Web 服務的參數
        ///// </summary>
        ///// <param name="_url">存取服務的URL</param>
        ///// <param name="_DataFileName">儲存資料的名稱</param>
        ///// <param name="_DataFolderName">資料要儲存的目錄</param>
        ///// <param name="_className">類別名稱</param>
        //public void SetWebAccessCondition(string _url, string _DataFileName, string _DataFolderName, string _className = "")
        //{
        //    string className = _className;

        //    this.restURL = string.Format("{0}{1}", _url, _className);
        //    this.資料檔案名稱 = _DataFileName;
        //    this.現在資料夾名稱 = _DataFolderName;
        //    this.managerResult = new APIResult();
        //}

        /// <summary>
        /// 從網路取得相對應WebAPI的資料
        /// </summary>
        /// <param name="dic">所要傳遞的參數 Dictionary </param>
        /// <param name="httpMethod">Get Or Post</param>
        /// <returns></returns>
        protected virtual async Task<APIResult> SendAsync(Dictionary<string, string> dic, HttpMethod httpMethod,
            CancellationToken token = default(CancellationToken))
        {
            this.ManagerResult = new APIResult();
            APIResult mr = this.ManagerResult;
            string jsonPayload = "";

            //檢查網路狀態
            //if (UtilityHelper.IsConnected() == false)
            //{
            //    mr.Status = false;
            //    mr.Message = "無網路連線可用，請檢查網路狀態";
            //    return mr;
            //}

            if (dic.ContainsKey(Constants.JSONDataKeyName))
            {
                jsonPayload = dic[Constants.JSONDataKeyName];
                dic.Remove(Constants.JSONDataKeyName);
            }

            HttpClientHandler handler = new HttpClientHandler();

            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    //client.Timeout = TimeSpan.FromMinutes(3);
                    string fooQueryString = dic.ToQueryString();
                    string fooUrl = $"{host}{restURL}{RouteURL}" + fooQueryString;
                    UriBuilder ub = new UriBuilder(fooUrl);
                    HttpResponseMessage response = null;

                    #region 檢查是否要將 JWT Token 放入 HTTP 標頭 Bearer 內
                    if (string.IsNullOrEmpty(this.AuthenticationHeaderBearerTokenValue) == false)
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", this.AuthenticationHeaderBearerTokenValue);
                    }
                    #endregion

                    #region  執行 HTTP 動詞 (Action) : Get, Post, Put, Delete
                    if (httpMethod == HttpMethod.Get)
                    {
                        // 使用 Get 方式來呼叫
                        response = await client.GetAsync(ub.Uri, token);
                    }
                    else if (httpMethod == HttpMethod.Post)
                    {
                        // 使用 Post 方式來呼叫
                        switch (EncodingType)
                        {
                            case EnctypeMethod.MULTIPART:
                                // 使用 MULTIPART 方式來進行傳遞資料的編碼
                                response = await client.PostAsync(ub.Uri, dic.ToMultipartFormDataContent(), token);
                                break;
                            case EnctypeMethod.FORMURLENCODED:
                                // 使用 FormUrlEncoded 方式來進行傳遞資料的編碼
                                response = await client.PostAsync(ub.Uri, dic.ToFormUrlEncodedContent(), token);
                                break;
                            case EnctypeMethod.JSON:
                                client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                                response = await client.PostAsync(ub.Uri, new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
                                break;
                            default:
                                throw new Exception("不正確的 HTTP Payload 編碼設定");
                                break;
                        }
                    }
                    else if (httpMethod == HttpMethod.Put)
                    {
                        // 使用 Post 方式來呼叫
                        switch (EncodingType)
                        {
                            case EnctypeMethod.MULTIPART:
                                // 使用 MULTIPART 方式來進行傳遞資料的編碼
                                response = await client.PutAsync(ub.Uri, dic.ToMultipartFormDataContent(), token);
                                break;
                            case EnctypeMethod.FORMURLENCODED:
                                // 使用 FormUrlEncoded 方式來進行傳遞資料的編碼
                                response = await client.PutAsync(ub.Uri, dic.ToFormUrlEncodedContent(), token);
                                break;
                            case EnctypeMethod.JSON:
                                client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                                response = await client.PutAsync(ub.Uri, new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
                                break;
                            default:
                                throw new Exception("不正確的 HTTP Payload 編碼設定");
                                break;
                        }
                    }
                    else if (httpMethod == HttpMethod.Delete)
                    {
                        response = await client.DeleteAsync(ub.Uri, token);
                    }
                    else
                    {
                        throw new NotImplementedException("Not Found HttpMethod");
                    }
                    #endregion

                    #region Response
                    if (response != null)
                    {
                        String strResult = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode == true)
                        {
                            mr = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                            if (mr.Status == true)
                            {
                                var fooDataString = mr.Payload.ToString();
                                if (IsCollectionType == false)
                                {
                                    SingleItem = JsonConvert.DeserializeObject<T>(fooDataString, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                                    if (NeedSaveData == true)
                                    {
                                        if (SingleItem == null)
                                        {
                                            SingleItem = (T)Activator.CreateInstance(typeof(T));
                                        }
                                        await this.WriteToFileAsync();
                                    }
                                }
                                else
                                {
                                    Items = JsonConvert.DeserializeObject<List<T>>(fooDataString, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                                    if (NeedSaveData == true)
                                    {
                                        if (Items == null)
                                        {
                                            Items = (List<T>)Activator.CreateInstance(typeof(List<T>));
                                        }
                                        await this.WriteToFileAsync();
                                    }
                                }
                            }
                        }
                        else
                        {
                            APIResult fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                            if (fooAPIResult != null)
                            {
                                mr = fooAPIResult;
                            }
                            else
                            {
                                mr.Status = false;
                                mr.Message = string.Format("Error Code:{0}, Error Message:{1}", response.StatusCode, response.Content);
                            }
                        }
                    }
                    else
                    {
                        mr.Status = false;
                        mr.Message = APIInternalError;
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    mr.Status = false;
                    mr.Message = ex.Message;
                }
            }

            return mr;
        }

        /// <summary>
        /// 將物件資料從檔案中讀取出來
        /// </summary>
        public virtual async Task ReadFromFileAsync()
        {
            #region 先將建立該資料模型的物件，避免檔案讀取不出來之後， Items / SingleItem 的物件值為 null
            if (IsCollectionType == false)
            {
                SingleItem = (T)Activator.CreateInstance(typeof(T));
            }
            else
            {
                Items = (List<T>)Activator.CreateInstance(typeof(List<T>));
            }
            #endregion

            string data = await StorageUtility.ReadFromDataFileAsync(this.CurrentFolderName, this.DataFileName);
            if (string.IsNullOrEmpty(data) == true)
            {

            }
            else
            {
                try
                {
                    if (IsCollectionType == false)
                    {
                        this.SingleItem = JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                    }
                    else
                    {
                        this.Items = JsonConvert.DeserializeObject<List<T>>(data, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

        }

        /// <summary>
        /// 將物件資料寫入到檔案中
        /// </summary>
        public virtual async Task WriteToFileAsync()
        {
            string data = "";
            if (IsCollectionType == false)
            {
                data = JsonConvert.SerializeObject(this.SingleItem);
            }
            else
            {
                data = JsonConvert.SerializeObject(this.Items);
            }
            await StorageUtility.WriteToDataFileAsync(this.CurrentFolderName, this.DataFileName, data);
        }

    }
    #endregion

    #region EnctypeMethod
    /// <summary>
    /// 呼叫 HTTP 動詞的時候，將要傳遞的參數，使用何種方式來進行編碼
    /// </summary>
    public enum EnctypeMethod
    {
        /// <summary>
        /// 使用 multipart/form-data 方式來進行傳遞參數的編碼
        /// </summary>
        MULTIPART,
        /// <summary>
        /// 使用 application/x-www-form-urlencoded 方式來進行傳遞參數的編碼
        /// </summary>
        FORMURLENCODED,
        /// <summary>
        /// 使用 application/json 方式來進行傳遞參數的編碼
        /// </summary>
        JSON
    }
    #endregion
}
