using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Respirar.Authentication.BackEnd.Application.ApiClient;
using Respirar.Authentication.BackEnd.Application.Commands;
using Respirar.Authentication.BackEnd.Application.DTOs;
using System.Threading.Tasks;

namespace Respirar.Authentication.BackEnd.Application.CommandHandlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, ValueResult<LoginResult>>
    {
        private readonly IKeyrockApiClient _keyrockApiClient;
        private readonly IDistributedCache _cache;

        public LoginCommandHandler(IKeyrockApiClient keyrockApiClient, IDistributedCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _keyrockApiClient = keyrockApiClient ?? throw new ArgumentNullException(nameof(keyrockApiClient));
        }
        public async Task<ValueResult<LoginResult>> Handle(LoginCommand command, CancellationToken cancellationToken)
        {

            var user = await _keyrockApiClient.GetUserWithEmail(command.name, cancellationToken);
            if (user.IsSuccess) {
                var code = await _cache.GetAsync(user.Result.Id);                
                if (code == null)
                {
                    return await _keyrockApiClient.Login(command, cancellationToken);
                }
                else
                    return ValueResult<LoginResult>.Error("Debe validar el mail");
            }            
            else {
                  return ValueResult<LoginResult>.Error("El usuario y/o la clave es incorrecta");
            }

        }

    }
}