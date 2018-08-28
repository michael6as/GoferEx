using System;
using System.Collections.Generic;
using System.Text;

namespace GoferEx.ExternalResources.Google
{
    public class CustomGoogleToken : BaseToken
    {
        public CustomGoogleToken(string authCode, string provider) : base(authCode, provider)
        {

        }
    }
}
