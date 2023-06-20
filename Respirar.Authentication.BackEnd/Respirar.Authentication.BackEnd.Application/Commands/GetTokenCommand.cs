using MediatR;
using Respirar.Authentication.BackEnd.Application.DTOs;

namespace Respirar.Authentication.BackEnd.Application.Commands
{
    public class GetTokenCommand : IRequest<ValueResult<GetTokenResult>>
    {
        public string? XSubjectToken { get ; set ; }

    }
}
