using System;
using System.Collections.Generic;
using System.Text;

namespace GoferEx.ExternalResources.Google
{
    public class GooglePlusAccessToken
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; }

        public GooglePlusAccessToken(string accessToken, string tokenType, int expiresIn, string refreshToken)
        {
            AccessToken = accessToken;
            TokenType = tokenType;
            ExpiresIn = expiresIn;
            RefreshToken = refreshToken;
        }
    }
}
