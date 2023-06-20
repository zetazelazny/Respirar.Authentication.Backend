﻿using MediatR;
using Respirar.Authentication.BackEnd.Application.ApiClient;
using Respirar.Authentication.BackEnd.Application.Commands;
using Respirar.Authentication.BackEnd.Application.DTOs;
using System.Threading.Tasks;

namespace Respirar.Authentication.BackEnd.Application.CommandHandlers
{
    public class RolesCommandHandler : IRequestHandler<RolesCommand, ValueResult<RoleResult>>
    {
        private readonly IKeyrockApiClient _keyrockApiClient;

        public RolesCommandHandler(IKeyrockApiClient keyrockApiClient)
        {
            _keyrockApiClient = keyrockApiClient ?? throw new ArgumentNullException(nameof(keyrockApiClient)); ;
        }
        public async Task<ValueResult<RoleResult>> Handle(RolesCommand command, CancellationToken cancellationToken)
        {
            return await _keyrockApiClient.GetRoles(command, cancellationToken);
        }
    }
}
