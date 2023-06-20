using MediatR;
using Microsoft.AspNetCore.Mvc;
using Respirar.Authentication.BackEnd.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Respirar.Authentication.BackEnd.Application.Commands
{
    public class UserDeleteCommand : IRequest<ValueResult<bool>>
    {
        [FromHeader(Name = "access-token")]
        [Required]
        public string accessToken { get; set; }
    }
}
