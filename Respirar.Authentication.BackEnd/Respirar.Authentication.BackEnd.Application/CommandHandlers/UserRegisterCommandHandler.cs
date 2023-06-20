using MediatR;
using Respirar.Authentication.BackEnd.Application.ApiClient;
using Respirar.Authentication.BackEnd.Application.Commands;
using Respirar.Authentication.BackEnd.Application.DTOs;
using System.Text.RegularExpressions;
using Respirar.Authentication.BackEnd.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace Respirar.Authentication.BackEnd.Application.CommandHandlers
{
    public class UserRegisterCommandHandler : IRequestHandler<UserRegisterCommand, ValueResult<UserRegisterResult>>
    {
        private readonly IKeyrockApiClient _keyrockApiClient;
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;

        public UserRegisterCommandHandler(IKeyrockApiClient keyrockApiClient,
            IMailService mailService,
            IConfiguration configuration,
            IDistributedCache cache)
        {
            _keyrockApiClient = keyrockApiClient ?? throw new ArgumentNullException(nameof(_keyrockApiClient));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(_mailService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(_configuration));
            _cache = cache ?? throw new ArgumentNullException(nameof(_cache)); ;
        }

        public async Task<ValueResult<UserRegisterResult>> Handle(UserRegisterCommand command, CancellationToken cancellationToken)
        {
            List<String> validaciones = validarUserRegister(command);
            if (validaciones.FirstOrDefault().ToUpper().Equals("OK"))
            {
                var result = await _keyrockApiClient.UserRegister(command, cancellationToken);

                var mailData = new MailData()
                {
                    EmailToId = command.username,
                    EmailBody = "Bienvenido a la plataforma respirar, clickee el siguiente link para confirmar su registro",
                    EmailSubject = "Bienvenido a la plataforma respirar",
                    EmailToName = command.username, 
                    EmailLink = $"{_configuration["MailSettings:RedirectionDomain"]}/registerconfirmation?id={result.Result.user.id}"
                };

                _mailService.SendMail(mailData);

                var dataToCache = Encoding.UTF8.GetBytes("pending");

                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(1200))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1200));

                await _cache.SetAsync(result.Result.user.id, dataToCache, options);

                return result;
            }
            else
            {
                return ValueResult<UserRegisterResult>.Error(validaciones.FirstOrDefault());
            }
        }

        private List<String> validarUserRegister(UserRegisterCommand command)
        {
            var response = new List<String>();

            var passwordValidation = IsStrongPassword(command.password);
            if (!ValidateEmail(command.username))
            {
                response.Add("El username no debe ser vácio");
            }
            else if (!passwordValidation.IsValid)
            {
                response.AddRange(passwordValidation.Errors);
            }
            else
            {
                response.Add("Ok");
            }

            return response;
        }

        private static bool ValidateEmail(string email)
        {
            // Expresión regular para validar direcciones de correo electrónico
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            // Validar el formato del correo electrónico
            bool isValid = Regex.IsMatch(email, pattern);

            return isValid;
        }


        //TODO Devolver el string de error especifico.
        public static PasswordValidationResult IsStrongPassword(string password)
        {
            PasswordValidationResult result = new PasswordValidationResult()
            {
                Errors = new List<string>(),
                IsValid = true
            };

            // Verificar que la contraseña tenga al menos 8 caracteres
            if (password.Length < 8)
            {
                result.IsValid = false;
                result.Errors.Add("La password debe tener al menos 8 caracteres");
            }
               

            // Verificar que la contraseña contenga al menos una letra mayúscula
            if (!Regex.IsMatch(password, "[A-Z]"))
            {
                result.IsValid = false;
                result.Errors.Add("La password debe contener al menos una mayuscula");
            }

            // Verificar que la contraseña contenga al menos un dígito
            if (!Regex.IsMatch(password, "[0-9]"))
            {
                result.IsValid = false;
                result.Errors.Add("La password debe contener al menos un digito");
            }

            return result;
        }

        public class PasswordValidationResult
        {
            public bool IsValid { get; set; }
            public List<String> Errors { get; set; }
        }
    }
}
