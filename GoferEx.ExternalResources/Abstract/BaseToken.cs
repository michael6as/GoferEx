namespace GoferEx.ExternalResources
{
    public abstract class BaseToken
    {
        public string Provider { get; private set; }
        public string AuthCode { get; private set; }

        public BaseToken(string provider, string authCode)
        {
            Provider = provider;
            AuthCode = authCode;
        }
    }
}