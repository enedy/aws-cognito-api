using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AWS.Cognito.Api.Configuration
{
    public abstract class BaseAuthenticationApiController : ControllerBase
    {
        protected readonly SignInManager<CognitoUser> _signInManager;
        protected readonly UserManager<CognitoUser> _userManager;
        protected readonly CognitoUserPool _pool;

        public BaseAuthenticationApiController(SignInManager<CognitoUser> signInManager,
            UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _pool = pool;
        }
    }
}
