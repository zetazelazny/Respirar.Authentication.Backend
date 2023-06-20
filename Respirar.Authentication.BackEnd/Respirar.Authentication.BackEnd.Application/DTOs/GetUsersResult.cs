using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respirar.Authentication.BackEnd.Application.DTOs
{
    public class GetUsersResult
    {
        public List<User> Users { get; set; }
    }

    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool Enabled { get; set; }
        public bool Gravatar { get; set; }
        public DateTime Date_password { get; set; }
        public object Description { get; set; }
        public object Website { get; set; }
    }
}
