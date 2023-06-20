using MediatR;
using Respirar.Authentication.BackEnd.Application.DTOs;

namespace Respirar.Authentication.BackEnd.Application.Commands
{
    public class UpdatePasswordCommand : IRequest<ValueResult<UpdatePasswordResult>>
    {
        public string username { get; set; }
        public string currentPassword { get; set; }
        public string newPassword { get; set; }

        public bool cambioPass { get; set; }
    }
}
