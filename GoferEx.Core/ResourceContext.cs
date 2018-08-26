namespace GoferEx.Core
{
    public class ResourceContext
    {
        public string Token { get; set; }
        public string Provider { get; set; }

        public ResourceContext(string token, string provider)
        {
            Token = token;
            Provider = provider;
        }
    }
}