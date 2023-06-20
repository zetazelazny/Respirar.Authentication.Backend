using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Respirar.Authentication.BackEnd.Application.DTOs.GetUserResult;

namespace Respirar.Authentication.BackEnd.Application.DTOs
{
    public class RoleResult
    {
        public List<Role> roles { get; set; }
    }
}
