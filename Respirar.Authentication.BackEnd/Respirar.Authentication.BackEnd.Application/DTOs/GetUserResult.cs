using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respirar.Authentication.BackEnd.Application.DTOs
{
    public class GetUserResult
    {
            public List<Organization> organizations { get; set; }
            public string displayName { get; set; }
            public List<Role> roles { get; set; }
            public string app_id { get; set; }
            public bool isGravatarEnabled { get; set; }
            public string email { get; set; }
            public string id { get; set; }
            public string authorization_decision { get; set; }
            public string app_azf_domain { get; set; }
            public string username { get; set; }

        public class Organization
        {
            public string id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public object website { get; set; }
            public List<Role> roles { get; set; }
        }
        public class Role
        {
            public string id { get; set; }
            public string name { get; set; }
        }
    }
}
