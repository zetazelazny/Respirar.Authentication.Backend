using MediatR;
using Respirar.Authentication.BackEnd.Application.ApiClient;
using Respirar.Authentication.BackEnd.Application.Commands;
using Respirar.Authentication.BackEnd.Application.DTOs;
using static Respirar.Authentication.BackEnd.Application.CommandHandlers.UserRegisterCommandHandler;

namespace Respirar.Authentication.BackEnd.Application.CommandHandlers
{
    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, ValueResult<UpdatePasswordResult>>
    {
        private readonly IKeyrockApiClient _keyrockApiClient;

        public UpdatePasswordCommandHandler(IKeyrockApiClient keyrockApiClient)
        {
            _keyrockApiClient = keyrockApiClient ?? throw new ArgumentNullException(nameof(keyrockApiClient)); ;
        }
        public async Task<ValueResult<UpdatePasswordResult>> Handle(UpdatePasswordCommand command, CancellationToken cancellationToken)
        {
            if (command.cambioPass)
            {
                var loginCommand = new LoginCommand()
                {
                    name = command.username,
                    password = command.currentPassword
                };

                var passwordConfirmationResult = await _keyrockApiClient.Login(loginCommand, cancellationToken);

                if (passwordConfirmationResult.IsSuccess)
                {
                    return await _keyrockApiClient.UpdatePassword(command, cancellationToken);
                }
                else
                    return ValueResult<UpdatePasswordResult>.Error("La validacion del usuario fallo");
            }
            else
            {
                return await _keyrockApiClient.UpdatePassword(command, cancellationToken);
            }
        }
    }
}

