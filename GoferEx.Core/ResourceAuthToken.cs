using Google.GData.Client;

namespace GoferEx.Core
{
    /// <summary>
    /// Token created from the cookies in an authorized request
    /// </summary>
    public class ResourceAuthToken
    {
        public string Id { get; set; }
        public OAuth2Parameters AuthToken { get; }
        public string ResourceProvider { get; }

        public ResourceAuthToken(string id, OAuth2Parameters authToken, string resourceProvider)
        {
            Id = id;
            AuthToken = authToken;
            ResourceProvider = resourceProvider;
        }

        /// <summary>
        /// For Anonymous Requests
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resourceProvider"></param>
        public ResourceAuthToken(string id, string resourceProvider)
        {
            Id = id;
            ResourceProvider = resourceProvider;
        }
    }
}