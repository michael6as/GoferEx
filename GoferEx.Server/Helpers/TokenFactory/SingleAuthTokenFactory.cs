using GoferEx.Core;
using GoferEx.Server.Helpers.Interfaces;
using Google.GData.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace GoferEx.Server.Helpers.TokenFactory
{
    /// <summary>
    /// This is the basic implementation for creating AuthToken for retrieving resources.
    /// If there is more than one provider this ain't the implementation
    /// </summary>
    public class SingleAuthTokenFactory : IAuthTokenFactory
    {

        public async Task<ResourceAuthToken> CreateAuthToken(HttpContext context)
        {
            var cookiesProvider = context.User.Identity.AuthenticationType;
            if (string.IsNullOrEmpty(cookiesProvider))
            {
                return new ResourceAuthToken("Test", "Test");
            }
            var oauthParams = new OAuth2Parameters()
            {
                AccessToken = await context.GetTokenAsync("access_token"),
                AccessCode = await context.GetTokenAsync("code"),
                AccessType = "offline",
                RefreshToken = await context.GetTokenAsync("refresh_token"),
                TokenExpiry = DateTime.Parse(await context.GetTokenAsync("expires_at")),
                Scope = await context.GetTokenAsync("scope")

            };
            return new ResourceAuthToken(context.User.Identity.Name, oauthParams, cookiesProvider);
        }
    }
}