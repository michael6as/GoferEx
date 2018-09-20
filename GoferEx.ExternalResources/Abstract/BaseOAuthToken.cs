using Google.GData.Client;

namespace GoferEx.ExternalResources
{
    public abstract class BaseOAuthToken
    {
        public OAuth2Parameters AuthToken { get; }
        public string ResourceProvider { get; }

        protected BaseOAuthToken(OAuth2Parameters authToken, string resourceProvider)
        {
            AuthToken = authToken;
            ResourceProvider = resourceProvider;
        }
    }
}