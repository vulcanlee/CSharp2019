using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientCallJWTAPI.DTOs
{
    public class LoginRequestDTO
    {
        public string Account { get; set; }
        public string Password { get; set; }
    }
}
