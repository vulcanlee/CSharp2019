using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ROPCAuthentication
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
        }
    }
}
