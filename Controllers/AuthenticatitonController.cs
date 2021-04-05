using AWS.Cognito.Api.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AWS.Cognito.Api.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticatitonController : ControllerBase
    {
        private IConfiguration _configuration;

        public AuthenticatitonController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("token")]
        [HttpGet]
        public IActionResult GetTokenAsync()
        {
            var token = GetToken(_configuration.GetSection("AWS").Get<CognitoConfig>());

            return Ok(token.Item2);
        }

        private Tuple<string, string> GetToken(CognitoConfig config)
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
    }
}
