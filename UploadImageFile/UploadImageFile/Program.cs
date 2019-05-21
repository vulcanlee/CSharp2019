using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace UploadImageFile
{
    /// <summary>
    /// 呼叫 API 回傳的制式格式
    /// </summary>
    public class APIResult
    {
        /// <summary>
        /// 此次呼叫 API 是否成功
        /// </summary>
        public bool Status { get; set; } = true;
        public int HTTPStatus { get; set; } = 200;
        public int ErrorCode { get; set; }
        /// <summary>
        /// 呼叫 API 失敗的錯誤訊息
        /// </summary>
        public string Message { get; set; } = "";
        /// <summary>
        /// 呼叫此API所得到的其他內容
        /// </summary>
        public object Payload { get; set; }
    }
    public class UploadImageResponseDTO
    {
        public string ImageUrl { get; set; }
    }
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("使用 HttpClient 上傳檔案範例");
            string path = Directory.GetCurrentDirectory();
            string fileName = "XamarinForms.jpg";
            string imageFileName = Path.Combine(path, fileName);

            HttpClient client = new HttpClient();
            string url = $"http://lobworkshop.azurewebsites.net/api/UploadImage";
            #region 將圖片檔案，上傳到網路伺服器上(使用 Multipart 的規範)
            // 規格說明請參考 https://www.w3.org/Protocols/rfc1341/7_2_Multipart.html
            using (var content = new MultipartFormDataContent())
            {
                // 開啟這個圖片檔案，並且讀取其內容
                using (var fs = File.Open(imageFileName, FileMode.Open))
                {
                    var streamContent = new StreamContent(fs);
                    streamContent.Headers.Add("Content-Type", "application/octet-stream");
                    streamContent.Headers.Add("Content-Disposition", "form-data; name=\"file\"; filename=\"" + fileName + "\"");
                    content.Add(streamContent, "file", fileName);

                    // 上傳到遠端伺服器上
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode == true)
                        {
                            // 取得呼叫完成 API 後的回報內容
                            String strResult = await response.Content.ReadAsStringAsync();
                            APIResult apiResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                            if (apiResult?.Status == true)
                            {
                                UploadImageResponseDTO uploadImageResponseDTO = JsonConvert.DeserializeObject<UploadImageResponseDTO>(apiResult.Payload.ToString());
                                Console.WriteLine("上傳圖片的網址");
                                Console.WriteLine(uploadImageResponseDTO.ImageUrl);
                            }
                        }
                    }
                }
            }
            #endregion

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }
    }
}
