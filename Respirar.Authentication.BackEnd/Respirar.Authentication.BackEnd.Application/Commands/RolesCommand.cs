using MediatR;
using Microsoft.AspNetCore.Mvc;
using Respirar.Authentication.BackEnd.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Respirar.Authentication.BackEnd.Application.Commands
{
    public class RolesCommand : IRequest<ValueResult<RoleResult>>
    {
        [FromHeader(Name = "access-token")]
        [Required]
        public string accessToken { get; set; }
    }
}
