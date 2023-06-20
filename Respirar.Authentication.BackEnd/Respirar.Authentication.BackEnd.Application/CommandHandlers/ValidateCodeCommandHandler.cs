using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Respirar.Authentication.BackEnd.Application.Commands;
using Respirar.Authentication.BackEnd.Application.DTOs;
using Respirar.Authentication.BackEnd.Application.Services;

namespace Respirar.Authentication.BackEnd.Application.CommandHandlers
{
    public class ValidateCodeCommandHandler : IRequestHandler<ValidateCodeCommand, ValueResult<bool>>
    {
        private readonly IDistributedCache _cache;

        public ValidateCodeCommandHandler(IDistributedCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(_cache)); ;
        }

        public async Task<ValueResult<bool>> Handle(ValidateCodeCommand command, CancellationToken cancellationToken)
        {
            var code = await _cache.GetAsync(command.code);

            if(code != null)
            {
                return ValueResult<bool>.Ok(true);
            }
            else
                return ValueResult<bool>.Error("El codigo no pudo ser validado");
        }
    }
}
