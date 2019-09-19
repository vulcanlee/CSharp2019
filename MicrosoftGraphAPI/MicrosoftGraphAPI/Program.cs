using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftGraphAPI
{
    class Program
    {
        static string clientId = "應用程式 (用戶端) 識別碼";
        static string authority = "目錄 (租用戶) 識別碼";
        static string account = "Office 365 使用者的電子郵件信箱";
        static string password = "Office 365 使用者的密碼";

        static void Main(string[] args)
        {
            GetMicrosoftGraphAccessTokeyAsync().Wait();
        }

        static async Task GetMicrosoftGraphAccessTokeyAsync()
        {
            string[] scopes = new string[] { "user.read" };
            IPublicClientApplication app;
            app = PublicClientApplicationBuilder.Create(clientId)
                  .WithAuthority($"https://login.microsoftonline.com/{authority}")
                  .Build();

            AuthenticationResult result = null;

            try
            {
                #region 將所提供的密碼，使用 SecureString 以加密的方式儲存
                // SecureString 代表應該將文字保密，例如於不再使用時將它從電腦記憶體刪除。 
                var securePassword = new SecureString();
                foreach (char c in password)
                    securePassword.AppendChar(c);
                #endregion

                // 使用使用者的帳號與密碼憑證，來獲取存取權杖
                result = await app.AcquireTokenByUsernamePassword(scopes, account, securePassword)
                                   .ExecuteAsync();
            }
            catch (MsalException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine(result.Account.Username);
            Console.WriteLine($"Access Token : {result.AccessToken}");
            foreach (var item in result.Scopes)
            {
                Console.WriteLine($"Scope :{item}");
            }


            var graphServiceClient = new GraphServiceClient(new DelegateAuthenticationProvider((requestMessage) =>
            {
                requestMessage
                    .Headers
                    .Authorization = new AuthenticationHeaderValue("bearer", result.AccessToken);

                return Task.FromResult(0);
            }));

            List<Option> requestOptions = new List<Option>();

            // 指定事件開始與結束時間
            DateTimeTimeZone startTime = new DateTimeTimeZone
            {
                DateTime = DateTime.Now.AddDays(3).ToString("o"),
                TimeZone = TimeZoneInfo.Local.Id
            };
            DateTimeTimeZone endTime = new DateTimeTimeZone
            {
                DateTime = DateTime.Now.AddDays(5).AddHours(1).ToString("o"),
                TimeZone = TimeZoneInfo.Local.Id
            };


            // 新增這個事件
            Event createdEvent = await graphServiceClient.Me.Events.Request(requestOptions)
                .AddAsync(new Event
                {
                    Subject = "自動同步行事曆測試" + Guid.NewGuid().ToString(),
                    Start = startTime,
                    End = endTime
                });

            if (createdEvent != null)
            {
                Console.WriteLine($"新的行事曆事件已經建立成功");
            }

        }
    }
}
