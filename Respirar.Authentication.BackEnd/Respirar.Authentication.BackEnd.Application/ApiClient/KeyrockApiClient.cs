using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Respirar.Authentication.BackEnd.Application.Commands;
using Respirar.Authentication.BackEnd.Application.DTOs;
using System.Net.Http.Json;
using System.Text.Json;

namespace Respirar.Authentication.BackEnd.Application.ApiClient
{
    public class KeyrockApiClient : IKeyrockApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public KeyrockApiClient(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ValueResult<LoginResult>> Login(LoginCommand command, CancellationToken cancellationToken)
        {
            ValueResult<LoginResult> result;

            var formData = new FormUrlEncodedContent(new[]
             {
                new KeyValuePair<string, string>("username", command.name),
                new KeyValuePair<string, string>("password", command.password),
                new KeyValuePair<string, string>("grant_type", "password"),
            });
            
            //var response = await _httpClient.PostAsJsonAsync<LoginCommand>("oauth2/token", command, cancellationToken);
            var response = await _httpClient.PostAsync("oauth2/token", formData, cancellationToken);

            if (response.IsSuccessStatusCode) {
                //var res = await response.Content.ReadFromJsonAsync<dynamic>();
                result = ValueResult<LoginResult>.Ok(await response.Content.ReadFromJsonAsync<LoginResult>());
            }
            else
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                result = ValueResult<LoginResult>.Error(errorResponse.error);
            }

            return result;
        }

        public async Task<ValueResult<UserRegisterResult>> UserRegister(UserRegisterCommand command, CancellationToken cancellationToken)
        {
            ValueResult<UserRegisterResult> result;
            var token = await CreateAdminToken(cancellationToken);//"ddcfea33-76e3-4f34-ac78-b3cfdf280a66";
            _httpClient.DefaultRequestHeaders.Add("X-Auth-token", token.Result);
            var jsonContent = new StringContent(JsonSerializer.Serialize(new
            {
                user = new
                {
                    username = command.username,
                    email = command.username,
                    password = command.password
                }
            }), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("v1/users", jsonContent, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                result = ValueResult<UserRegisterResult>.Ok(await response.Content.ReadFromJsonAsync<UserRegisterResult>());
            }
            else
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                result = ValueResult<UserRegisterResult>.Error(errorResponse.error);
            }

            return result;                        
        }

        private async Task<ValueResult<String>>CreateAdminToken(CancellationToken cancellationToken)
        {
            ValueResult<String> result;

            var jsonContent = new StringContent(JsonSerializer.Serialize(new
            {
                name = _configuration["Keyrock:AdminUser"],
                password = _configuration["Keyrock:AdminPass"]

            }), System.Text.Encoding.UTF8, "application/json"); 

            var response = await _httpClient.PostAsync("v1/auth/tokens", jsonContent, cancellationToken);

            if (response.IsSuccessStatusCode && response.Headers.TryGetValues("X-Subject-Token", out var headerValues))
            {
                result = ValueResult<String>.Ok(headerValues.FirstOrDefault());
            }
            else
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                result = ValueResult<String>.Error(errorResponse.error);
            }

            return result;
        }

        public async Task<ValueResult<UpdatePasswordResult>> UpdatePassword(UpdatePasswordCommand command, CancellationToken cancellationToken)
        {
            string userId;
            var token = await CreateAdminToken(cancellationToken);
            ValueResult<UpdatePasswordResult> result;

            if (command.cambioPass)
            {
                string accessToken = _httpContextAccessor.HttpContext.Request.Headers["access-token"];

                ValueResult<GetUserResult> getUserResult = await GetUser(accessToken, cancellationToken);

                if (!getUserResult.IsSuccess)
                {
                    // ESTE TAMBIEN ANDA BIEN ÑERI
                    var errorResponse = getUserResult.Errors.FirstOrDefault();
                    result = ValueResult<UpdatePasswordResult>.Error(errorResponse);
                    return result;
                }
                userId = getUserResult.Result.id;
            }
            else
            {
                var user = await GetUserWithEmail(command.username, cancellationToken);
                userId = user.Result.Id;
            }
           

            //ValueResult<GetTokenResult> getTokenResult = await GetToken(accessToken, cancellationToken) ;
            //string user_id = getTokenResult.Result.User.id;


            Dictionary<string, string> headers = new()
            {
                {
                    "X-Auth-token", token.Result
                }
            };

            HttpRequestMessage request = RequestBuilder(HttpMethod.Patch, $"v1/users/{userId}", headers);

            var jsonContent = new StringContent(JsonSerializer.Serialize(new
            {
                user = new
                {
                    password = command.newPassword
                }
            }), System.Text.Encoding.UTF8, "application/json");

            request.Content = jsonContent;

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                result = ValueResult<UpdatePasswordResult>.Ok(await response.Content.ReadFromJsonAsync<UpdatePasswordResult>());
            }
            else
            {

                // ERROR DE DESERIALIZACION
                var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                result = ValueResult<UpdatePasswordResult>.Error(errorResponse.error);
            }

            return result;                        
        }
        //public async Task<ValueResult<bool>> RecoverPassword(RecoverPasswordCommand command, CancellationToken cancellationToken)
        //{
        //    ValueResult<bool> result;
        //    var token = await CreateAdminToken(cancellationToken);

        //    string accessToken = _httpContextAccessor.HttpContext.Request.Headers["access-token"];

        //    ValueResult<GetUserResult> getUserResult = await GetUser(accessToken, cancellationToken);

        //    if (!getUserResult.IsSuccess)
        //    {
        //        // ESTE TAMBIEN ANDA BIEN ÑERI
        //        var errorResponse = getUserResult.Errors.FirstOrDefault();
        //        result = ValueResult<bool>.Error(errorResponse);
        //        return result;
        //    }
        //    string userId = getUserResult.Result.id;

        //    //ValueResult<GetTokenResult> getTokenResult = await GetToken(accessToken, cancellationToken) ;
        //    //string user_id = getTokenResult.Result.User.id;


        //    Dictionary<string, string> headers = new()
        //    {
        //        {
        //            "X-Auth-token", token.Result
        //        }
        //    };

        //    HttpRequestMessage request = RequestBuilder(HttpMethod.Patch, $"v1/users/{userId}", headers);

        //    var jsonContent = new StringContent(JsonSerializer.Serialize(new
        //    {
        //        user = new
        //        {
        //            password = command.newPassword
        //        }
        //    }), System.Text.Encoding.UTF8, "application/json");

        //    request.Content = jsonContent;

        //    var response = await _httpClient.SendAsync(request, cancellationToken);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        // await response.Content.ReadFromJsonAsync<UpdatePasswordResult>()
        //        result = ValueResult<bool>.Ok(true);
        //    }
        //    else
        //    {

        //        // ERROR DE DESERIALIZACION
        //        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        //        result = ValueResult<bool>.Error(errorResponse.error);
        //    }

        //    return result;
        //}
        public async Task<ValueResult<bool>> UserDelete(UserDeleteCommand command, CancellationToken cancellationToken)
        {
            ValueResult<bool> result;

            var token = await CreateAdminToken(cancellationToken);

            ValueResult<GetUserResult> getUserResult = await GetUser(command.accessToken, cancellationToken);

            if (!getUserResult.IsSuccess)
            {
                // ESTE TAMBIEN ANDA BIEN ÑERI
                var errorResponse = getUserResult.Errors.FirstOrDefault();
                return ValueResult<bool>.Error(errorResponse);
            }
            string userId = getUserResult.Result.id;

            Dictionary<string, string> headers = new()
            {
                {
                    "X-Auth-token", token.Result
                }
            };

            HttpRequestMessage request = RequestBuilder(HttpMethod.Delete, $"v1/users/{userId}", headers);
            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                result = ValueResult<bool>.Ok(true);
            }
            else
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<string>();
                result = ValueResult<bool>.Error(errorResponse);
                result.Result = false;
            }
            return result;
        }
        public async Task<ValueResult<RoleResult>> GetRoles(RolesCommand command, CancellationToken cancellationToken)
        {
            ValueResult<RoleResult> result;
            string url = $"user?access_token={command.accessToken}";

            String authorizationHeader = _httpClient.DefaultRequestHeaders.Authorization.ToString();
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            var response = await _httpClient.GetAsync(url);
            _httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader);

            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content.ReadFromJsonAsync<GetUserResult>();
                RoleResult roles = new RoleResult
                {
                    roles = new List<GetUserResult.Role>()
                };
                roles.roles.AddRange(resultado.roles);
                foreach (var organization in resultado.organizations)
                {
                    roles.roles.AddRange(organization.roles);
                }
                result = ValueResult<RoleResult>.Ok(roles);
            }
            else
            {
                // ESTE ANDA BIEN
                var errorResponse = await response.Content.ReadFromJsonAsync<string>();
                result = ValueResult<RoleResult>.Error(errorResponse);
            }

            return result;
        }
        public async Task<ValueResult<User>> GetUserWithEmail(string email, CancellationToken cancellationToken)
        {
            ValueResult<GetUsersResult> result1 = await GetUsers(cancellationToken);
            ValueResult<User> result2;
            User foundUser = result1.Result.Users.FirstOrDefault(user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (foundUser != null && result1.IsSuccess)
            {
                result2 = ValueResult<User>.Ok(foundUser);
            }
            else
            {
                if (!result1.IsSuccess)
                {
                    var errorResponse = result1.Errors.FirstOrDefault();
                    result2 = ValueResult<User>.Error(errorResponse);
                }
                else
                {
                    result2 = ValueResult<User>.Error("Usuario no encontrado.");
                }
            }

            return result2;
        }
        private async Task<ValueResult<GetUserResult>> GetUser(string accessToken, CancellationToken cancellationToken)
        {
            ValueResult<GetUserResult> result;
            string url = $"user?access_token={accessToken}";

            String authorizationHeader = _httpClient.DefaultRequestHeaders.Authorization.ToString();
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            var response = await _httpClient.GetAsync(url);
            _httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader);

            if (response.IsSuccessStatusCode)
            {
                //result = ValueResult<GetUserResult>.Ok(headerValues.FirstOrDefault());
                result = ValueResult<GetUserResult>.Ok(await response.Content.ReadFromJsonAsync<GetUserResult>());
            }
            else
            {
                // ESTE ANDA BIEN
                var errorResponse = await response.Content.ReadFromJsonAsync<string>();
                result = ValueResult<GetUserResult>.Error(errorResponse);
            }

            return result;
        }
        private async Task<ValueResult<GetUsersResult>> GetUsers(CancellationToken cancellationToken)
        {
            ValueResult<GetUsersResult> result;
            var token = await CreateAdminToken(cancellationToken);
            Dictionary<string, string> headers = new()
            {
                {
                    "X-Auth-token", token.Result
                }
            };
            HttpRequestMessage request = RequestBuilder(HttpMethod.Get, "v1/users", headers);

            //String authorizationHeader = _httpClient.DefaultRequestHeaders.Authorization.ToString();
            //_httpClient.DefaultRequestHeaders.Remove("Authorization");
            var response = await _httpClient.SendAsync(request, cancellationToken);
            //_httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader);

            if (response.IsSuccessStatusCode)
            {
                result = ValueResult<GetUsersResult>.Ok(await response.Content.ReadFromJsonAsync<GetUsersResult>());
            }
            else
            {
                // ESTE ANDA BIEN?
                var errorResponse = await response.Content.ReadFromJsonAsync<string>();
                result = ValueResult<GetUsersResult>.Error(errorResponse);
            }

            return result;
        }
        private HttpRequestMessage RequestBuilder(HttpMethod method, string apiUrl, Dictionary<string, string> headers)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, apiUrl);
            
            foreach (KeyValuePair<string, string> header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            return request;
        }

        public class ErrorResponse
        {
            public string error { get; set; }
        }
    }
}
