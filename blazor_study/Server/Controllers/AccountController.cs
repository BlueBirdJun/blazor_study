using blazor_study.Server.Authentication;
using blazor_study.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace blazor_study.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController :ControllerBase
    {
        private UserAccountService _userAccountService;
        public AccountController(UserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }
        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public ActionResult<UserSession> Login([FromBody] LoginRequest loginRequest)
        {
            var jwtAuthenticationManager = new JwtAuthernticationManager(_userAccountService);
            var userSession = jwtAuthenticationManager.GenerateJwtToken(loginRequest.UserName, loginRequest.Password);
            if (userSession is null)
                return Unauthorized();
            else
                return userSession;

        }


    }
}
