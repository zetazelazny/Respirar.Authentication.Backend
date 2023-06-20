using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Respirar.Authentication.BackEnd.Application.ApiClient;
using Respirar.Authentication.BackEnd.Application.Commands;
using Respirar.Authentication.BackEnd.Application.DTOs;
using Respirar.Authentication.BackEnd.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respirar.Authentication.BackEnd.Application.CommandHandlers
{
    public class RecoverPasswordCommandHandler : IRequestHandler<RecoverPasswordCommand, ValueResult<bool>>
    {
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;
        private readonly IKeyrockApiClient _keyrockApiClient;

        public RecoverPasswordCommandHandler(IKeyrockApiClient keyrockApiClient,
            IMailService mailService,
            IConfiguration configuration,
            IDistributedCache cache)
        {
            _keyrockApiClient = keyrockApiClient ?? throw new ArgumentNullException(nameof(_keyrockApiClient));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(_mailService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(_configuration));
            _cache = cache ?? throw new ArgumentNullException(nameof(_cache)); ;
        }
        public async Task<ValueResult<bool>> Handle(RecoverPasswordCommand command, CancellationToken cancellationToken)
        {

            ValueResult<User> userResult = await _keyrockApiClient.GetUserWithEmail(command.email, cancellationToken);

            if (!userResult.IsSuccess)
            {
                return ValueResult<bool>.Error(userResult.Errors.FirstOrDefault());
            }

            Random random = new Random();

            string code = random.Next(000000, 999999).ToString();

            try
            {
                var mailData = new MailData()
                {
                    EmailToId = command.email,
                    EmailBody = "Clickee el siguiente link para reestablecer su contraseña.",
                    EmailSubject = "Reestablecimiento de contraseña",
                    EmailToName = command.email,
                    EmailLink = $"{_configuration["MailSettings:RedirectionDomain"]}/recoverpasswordchange?changecode={code}&username={command.email}"
                };

               _mailService.SendMail(mailData);

                var dataToCache = Encoding.UTF8.GetBytes(code);

                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(1200))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1200));

                await _cache.SetAsync(code, dataToCache, options);

                return ValueResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ValueResult<bool>.Error(ex.Message);
            }
        }
    }
}
