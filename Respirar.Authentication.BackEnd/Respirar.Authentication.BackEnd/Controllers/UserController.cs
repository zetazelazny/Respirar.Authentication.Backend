using MediatR;
using Microsoft.AspNetCore.Mvc;
using Respirar.Authentication.BackEnd.Application.Commands;
using Respirar.Authentication.BackEnd.Application.DTOs;

namespace Respirar.Authentication.BackEnd.Controllers
{
    public class UserController : Controller
    {
        private readonly IMediator mediator;

        public UserController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("/login")]
        public async Task<ActionResult<ValueResult<LoginResult>>> Login([FromBody] LoginCommand loginCommand)
        {
            var result = await mediator.Send(loginCommand);

            return result;
        }

        [HttpPost]
        [Route("/userregister")]
        public async Task<ActionResult<ValueResult<UserRegisterResult>>> UserRegister([FromBody] UserRegisterCommand userRegisterCommand)
        {
            var result = await mediator.Send(userRegisterCommand);

            return result;
        }

        [HttpPost]
        [Route("/recoverPassword")]
        public async Task<ActionResult<ValueResult<bool>>> RecoverPassword([FromBody] RecoverPasswordCommand recoverPasswordCommand)
        {
            var result = await mediator.Send(recoverPasswordCommand);

            return result;
        }

        [HttpGet]
        [Route("/validateUser/{id}")]
        public async Task<ActionResult<ValueResult<bool>>> ValidateUser(string id)
        {
            var command = new ValidateUserCommand()
            {
                Id = id
            };

            var result = await mediator.Send(command);

            return result;
        }

        [HttpPost]
        [Route("/updatepassword")]
        public async Task<ActionResult<ValueResult<UpdatePasswordResult>>> UpdatePassword([FromBody] UpdatePasswordCommand updatePasswordCommand)
        {
            var result = await mediator.Send(updatePasswordCommand);

            return result;
        }

        [HttpDelete]
        [Route("/deleteuser")]
        public async Task<ActionResult<ValueResult<bool>>> UserDelete([FromHeader] UserDeleteCommand userDeleteCommand)
        {
            var result = await mediator.Send(userDeleteCommand);

            return result;
        }

        [HttpGet]
        [Route("/roles")]
        public async Task<ActionResult<ValueResult<RoleResult>>> GetRoles([FromHeader] RolesCommand rolesCommand)
        {
            var result = await mediator.Send(rolesCommand);

            return result;
        }

        [HttpGet]
        [Route("/validateRecoverCode/{code}")]
        public async Task<ActionResult<ValueResult<bool>>> ValidateRecoverCode(string code)
        {
            var command = new ValidateCodeCommand()
            {
                code = code
            };
            var result = await mediator.Send(command);

            return result;
        }
    }
}
