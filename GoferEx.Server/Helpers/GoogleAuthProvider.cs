using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GoferEx.Server.Helpers.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoferEx.Server.Helpers
{
    public class GoogleAuthProvider:IAuthProvider
    {
        private readonly IConfiguration _config;

        public GoogleAuthProvider(IConfiguration config)
        {
            _config = config;
        }

        public void AddAuthMiddleware(AuthenticationBuilder authBuilder)
        {
            authBuilder.AddGoogle(o =>
            {
                o.ClientId = _config["Authentication:Google:ClientId"];
                o.ClientSecret = _config["Authentication:Google:ClientSecret"];
                o.AuthorizationEndpoint += "?prompt=consent"; // Hack so we always get a refresh token, it only comes on the first authorization response
                o.AccessType = "offline";
                o.SaveTokens = true;
                o.Events = new OAuthEvents()
                {
                    OnRemoteFailure = HandleOnRemoteFailure
                };
                o.ClaimActions.MapJsonSubKey("urn:google:image", "image", "url");
                o.ClaimActions.Remove(ClaimTypes.GivenName);
                o.AddScopes(_config);
                //o.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
                //o.Scope.Add("https://www.googleapis.com/auth/userinfo.email");
                //o.Scope.Add("https://www.googleapis.com/auth/user.phonenumbers.read");
                //o.Scope.Add("https://www.googleapis.com/auth/user.emails.read");
                //o.Scope.Add("https://www.googleapis.com/auth/user.birthday.read");
                //o.Scope.Add("https://www.googleapis.com/auth/user.addresses.read");
                //o.Scope.Add("https://www.googleapis.com/auth/plus.login");
                //o.Scope.Add("https://www.googleapis.com/auth/contacts.readonly");
                //o.Scope.Add("https://www.googleapis.com/auth/contacts");
            });
        }

        private async Task HandleOnRemoteFailure(RemoteFailureContext context)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync("<html><body>");
            await context.Response.WriteAsync("A remote failure has occurred: " + UrlEncoder.Default.Encode(context.Failure.Message) + "<br>");

            if (context.Properties != null)
            {
                await context.Response.WriteAsync("Properties:<br>");
                foreach (KeyValuePair<string, string> pair in context.Properties.Items)
                {
                    await context.Response.WriteAsync($"-{ UrlEncoder.Default.Encode(pair.Key)}={ UrlEncoder.Default.Encode(pair.Value)}<br>");
                }
            }

            await context.Response.WriteAsync("<a href=\"/\">Home</a>");
            await context.Response.WriteAsync("</body></html>");

            context.HandleResponse();
        }
    }
}
