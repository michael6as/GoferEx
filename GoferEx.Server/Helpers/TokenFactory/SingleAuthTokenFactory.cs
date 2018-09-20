using System;
using System.Threading.Tasks;
using GoferEx.ExternalResources;
using Google.GData.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using GoferEx.Server.Helpers.Interfaces;

namespace GoferEx.Server.Helpers.TokenFactory
{
    /// <summary>
    /// This is the basic implementation for creating AuthToken for retrieving resources.
    /// If there is more than one provider this ain't the implementation
    /// </summary>
    public class SingleAuthTokenFactory : IAuthTokenFactory
    {

        public async Task<BaseOAuthToken> CreateAuthToken(HttpContext context)
        {
            var accessToken = await context.GetTokenAsync("access_token");
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new UnauthorizedAccessException("The user isn't google authenticated");
            }
            var oauthParams = new OAuth2Parameters()
            {
                AccessToken = accessToken,
                AccessCode = await context.GetTokenAsync("code"),
                AccessType = "offline",
                RefreshToken = await context.GetTokenAsync("refresh_token"),
                TokenExpiry = DateTime.Parse(await context.GetTokenAsync("expires_at")),
                Scope = await context.GetTokenAsync("scope")

            };
            return new GoogleAuthToken(oauthParams);            
        }
    }
}