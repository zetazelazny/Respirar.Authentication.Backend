using MediatR;
using Respirar.Authentication.BackEnd.Application.ApiClient;
using Respirar.Authentication.BackEnd.Application.Commands;
using Respirar.Authentication.BackEnd.Application.DTOs;
using System.Threading.Tasks;

namespace Respirar.Authentication.BackEnd.Application.CommandHandlers
{
    public class UserDeleteCommandHandler : IRequestHandler<UserDeleteCommand, ValueResult<bool>>
    {
        private readonly IKeyrockApiClient _keyrockApiClient;

        public UserDeleteCommandHandler(IKeyrockApiClient keyrockApiClient)
        {
            _keyrockApiClient = keyrockApiClient ?? throw new ArgumentNullException(nameof(keyrockApiClient)); ;
        }
        public async Task<ValueResult<bool>> Handle(UserDeleteCommand command, CancellationToken cancellationToken)
        {
            return await _keyrockApiClient.UserDelete(command, cancellationToken);
        }
    }
}
