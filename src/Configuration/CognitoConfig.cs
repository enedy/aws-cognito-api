namespace AWS.Cognito.Api.Configuration
{
    public class CognitoConfig
    {
        public string Region { get; set; }
        public string AuthDomain { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scopes { get; set; }
        public string PoolId { get; set; }

        public string GetAuthUrlAuthDomain => $"https://{AuthDomain}.auth.{Region}.amazoncognito.com/oauth2/token";

        public string GetAuthUrlJwks => $"https://cognito-idp.{Region}.amazonaws.com/{PoolId}/.well-known/jwks.json";
    }
}
