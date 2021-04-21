using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using AWS.Cognito.Api.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AWS.Cognito.Api.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticatitonController : BaseAuthenticationApiController
    {
        private IConfiguration _configuration;

        public AuthenticatitonController(IConfiguration configuration, SignInManager<CognitoUser> signInManager,
            UserManager<CognitoUser> userManager, CognitoUserPool pool) : base(signInManager, userManager, pool)
        {
            _configuration = configuration;
        }

        [Route("token")]
        [HttpGet]
        public async Task<IActionResult> GetTokenAsync(string email, string pwd)
        {
            var token = await GetTokenCognitoAuthenticationAsync(email, pwd);
            //var accessToken = GetToken(_configuration.GetSection("AWS").Get<CognitoConfig>());

            return Ok(new { AccessToken = token.AccessToken, IdToken = token.IdToken });
        }

        private Tuple<string, string> GetTokenHttpRequest(CognitoConfig config)
        {
            var url = config.GetAuthUrlAuthDomain;
            var form = new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"},
                {"scope", config.Scopes}
            };

            using var httpClient = new HttpClient();

            var auth = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes($"{config.ClientId}:{config.ClientSecret}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

            HttpResponseMessage result = httpClient.PostAsync(url, new FormUrlEncodedContent(form)).Result;

            string json = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            result.EnsureSuccessStatusCode();

            var data = Newtonsoft.Json.Linq.JObject.Parse(json);

            return new Tuple<string, string>(data["token_type"].ToString(), data["access_token"].ToString());
        }

        private async Task<AuthenticationResultType> GetTokenCognitoAuthenticationAsync(string email, string pwd)
        {
            var user = await _userManager.FindByEmailAsync(email);
            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
            {
                Password = pwd
            };

            var authResponse = await user.StartWithSrpAuthAsync(authRequest);

            return authResponse.AuthenticationResult;
        }
    }
}
