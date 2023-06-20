using MediatR;
using Microsoft.AspNetCore.Mvc;
using Respirar.Authentication.BackEnd.Application.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respirar.Authentication.BackEnd.Application.Commands
{
    public class RecoverPasswordCommand : IRequest<ValueResult<bool>>
    {
        public string email { get; set; }
    }
}
