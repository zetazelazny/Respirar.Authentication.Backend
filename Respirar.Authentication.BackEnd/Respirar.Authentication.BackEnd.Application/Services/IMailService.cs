using Respirar.Authentication.BackEnd.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respirar.Authentication.BackEnd.Application.Services
{
    public interface IMailService
    {
        Task<bool> SendMail(MailData mailData);
    }
}
