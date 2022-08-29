using Amazon.Extensions.CognitoAuthentication;
using AWS.Cognito.Api.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AWS.Cognito.Api.Controllers
{
    [Route("api/validate-access")]
    [ApiController]
    //[Authorize]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ValidateAccessController : BaseAuthenticationApiController
    {
        public ValidateAccessController(SignInManager<CognitoUser> signInManager,
            UserManager<CognitoUser> userManager, CognitoUserPool pool) : base(signInManager, userManager, pool)
        {

        }

        [HttpGet]
        public async Task<bool> Get()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var claims = identity.Claims;
     
                var userName = identity.FindFirst("username").Value;
                var user = await _userManager.FindByIdAsync(userName);
            }

            return true;
        }
    }
}
