using Newtonsoft.Json;

namespace GoferEx.Core
{
    public class ResourceContext
    {
        public string Token { get; set; }
        public string RedirectUri { get; set; }
        public string[] Scopes { get; set; }
        public string Provider { get; set; }

        [JsonConstructor]
        public ResourceContext(string token, string redirectUri, string provider, string[] scopes)
        {
            Token = token;
            Provider = provider;
            RedirectUri = redirectUri;
            Scopes = scopes;
        }
    }
}