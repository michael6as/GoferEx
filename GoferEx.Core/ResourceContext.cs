using Newtonsoft.Json;

namespace GoferEx.Core
{
    public class ResourceContext
    {
        public string Token { get; set; }
        public string Provider { get; set; }

        [JsonConstructor]
        public ResourceContext(string token, string provider)
        {
            Token = token;
            Provider = provider;
        }
    }
}