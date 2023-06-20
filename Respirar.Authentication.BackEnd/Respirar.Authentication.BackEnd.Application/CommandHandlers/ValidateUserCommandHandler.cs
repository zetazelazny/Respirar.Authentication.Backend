using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Respirar.Authentication.BackEnd.Application.Commands;
using Respirar.Authentication.BackEnd.Application.DTOs;

namespace Respirar.Authentication.BackEnd.Application.CommandHandlers
{

    public class ValidateUserCommandHandler : IRequestHandler<ValidateUserCommand, ValueResult<bool>>
    {
        private readonly IDistributedCache _cache;

        public ValidateUserCommandHandler(IDistributedCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(_cache));
        }

        public async Task<ValueResult<bool>> Handle(ValidateUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                byte[] cachedData = await _cache.GetAsync(command.Id);

                //return ValueResult<bool>.Ok(true);

                if (cachedData != null)
                {
                    await _cache.RemoveAsync(command.Id);
                    return ValueResult<bool>.Ok(true);
                }
                else
                    return ValueResult<bool>.Error("El usuario no fue encontrado.");
            }
            catch (Exception ex)
            {
                return ValueResult<bool>.Error(ex.Message);
            }
        }
    }
}
