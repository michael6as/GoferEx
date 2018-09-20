using Google.GData.Client;

namespace GoferEx.ExternalResources
{
    public class GoogleAuthToken : BaseOAuthToken
    {
        public GoogleAuthToken(OAuth2Parameters authParams) : base(authParams, "Google")
        {

        }
    }
}