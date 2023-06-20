using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respirar.Authentication.BackEnd.Application.DTOs
{
    public class UpdatePassword
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
