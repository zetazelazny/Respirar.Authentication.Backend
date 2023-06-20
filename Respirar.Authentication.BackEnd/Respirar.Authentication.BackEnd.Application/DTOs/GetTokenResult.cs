using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respirar.Authentication.BackEnd.Application.DTOs
{
    public class GetTokenResult
    {
        public string access_token { get; set; }
        public DateTime expires { get; set; }
        public bool valid { get; set; }
        public UserDTO User { get; set; }

        public class UserDTO
        {
            public string id { get; set; }
            public string username { get; set; }
            public string email { get; set; }
            public DateTime date_password { get; set; }
            public bool enabled { get; set; }
            public bool admin { get; set; }
        }
    }
}

