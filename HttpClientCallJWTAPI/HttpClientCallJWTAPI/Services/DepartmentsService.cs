using HttpClientCallJWTAPI.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientCallJWTAPI.Services
{
    public class DepartmentsManager : BaseWebAPI<DepartmentResponseDTO>
    {
        public DepartmentsManager()
            : base()
        {
            this.restURL = "/Departments";
            this.host = Constants.HostAPI;
            IsCollectionType = true;
            EncodingType = EnctypeMethod.JSON;
            NeedSaveData = true;
        }

        public async Task<APIResult> GetAsync(string authenticationHeaderBearerTokenValue, CancellationToken ctoken = default(CancellationToken))
        {
            AuthenticationHeaderBearerTokenValue = authenticationHeaderBearerTokenValue;

            #region 要傳遞的參數
            HTTPPayloadDictionary dic = new HTTPPayloadDictionary();

            // ---------------------------- 另外兩種建立 QueryString的方式
            //dic.Add(Global.getName(() => memberSignIn_QS.app), memberSignIn_QS.app);
            //dic.AddItem<string>(() => 查詢資料QueryString.strHospCode);
            //dic.Add("Price", SetMemberSignUpVM.Price.ToString());
            //dic.Add(LOBGlobal.JSONDataKeyName, JsonConvert.SerializeObject(exceptionRecordRequestDTO));
            #endregion

            var mr = await this.SendAsync(dic, HttpMethod.Get, ctoken);

            //mr.Success = false;
            //mr.Message = "測試用的錯誤訊息";
            return mr;
        }
    }
}
