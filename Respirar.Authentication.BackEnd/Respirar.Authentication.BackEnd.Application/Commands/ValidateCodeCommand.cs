using MediatR;
using Respirar.Authentication.BackEnd.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respirar.Authentication.BackEnd.Application.Commands
{
    public class ValidateCodeCommand : IRequest<ValueResult<bool>>
    {
        public string code { get; set; }
    }
}
