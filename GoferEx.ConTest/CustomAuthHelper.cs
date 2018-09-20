using System;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Util.Store;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using GoferEx.ExternalResources;
using GoferEx.ExternalResources.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using GoferEx.Core;
using Newtonsoft.Json;
using System.Text;
using System.Linq;
using Google.GData.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace GoferEx.ConTest
{
    public class CustomAuthHelper
    {
        private IApplicationBuilder _app;
        private IConfiguration _config;
        private IDictionary<string, IResourceHandler<CustomGoogleToken>> _googleResourceHandler;

        public CustomAuthHelper(IApplicationBuilder appBuilder, IDictionary<string, IResourceHandler<CustomGoogleToken>> externalResHandler, IConfiguration config)
        {
            _app = appBuilder;
            _googleResourceHandler = externalResHandler;
            _config = config;
        }

        public void RouteAuthLogin()
        {
            _app.Map("/login", signinApp =>
            {
                signinApp.Run(async context =>
                {
                    var authType = context.Request.Query["authscheme"];
                    if (!string.IsNullOrEmpty(authType))
                    {
                        // By default the client will be redirect back to the URL that issued the challenge (/login?authtype=foo),
                        // send them to the home page instead (/).
                        await context.ChallengeAsync(authType, new AuthenticationProperties() { RedirectUri = "http://localhost:8080" });
                        return;
                    }

                    var response = context.Response;
                    response.ContentType = "application/json";
                    var schemeProvider = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
                    var schemeNames = new List<AuthInfoObject>();
                    foreach (var provider in await schemeProvider.GetAllSchemesAsync())
                    {
                        if (provider.DisplayName != null)
                        {
                            schemeNames.Add(new AuthInfoObject(provider.Name, provider.DisplayName, "?authscheme=" + provider.Name));
                        }
                    }
                    await response.Body.WriteAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(schemeNames)));
                });
            });
        }

        public void RouteAuthSuccess()
        {
            _app.Run(async context =>
            {
                // Setting DefaultAuthenticateScheme causes User to be set
                var user = context.User;

                // This is what [Authorize] calls
                // var user = await context.AuthenticateAsync();

                // This is what [Authorize(ActiveAuthenticationSchemes = MicrosoftAccountDefaults.AuthenticationScheme)] calls
                // var user = await context.AuthenticateAsync(MicrosoftAccountDefaults.AuthenticationScheme);

                // Deny anonymous request beyond this point.
                if (user == null || !user.Identities.Any(identity => identity.IsAuthenticated))
                {
                    // This is what [Authorize] calls
                    // The cookie middleware will handle this and redirect to /login
                    await context.ChallengeAsync();

                    // This is what [Authorize(ActiveAuthenticationSchemes = MicrosoftAccountDefaults.AuthenticationScheme)] calls
                    // await context.ChallengeAsync(MicrosoftAccountDefaults.AuthenticationScheme);

                    return;
                }

                var oauthParams = new OAuth2Parameters()
                {
                    AccessToken = await context.GetTokenAsync("access_token"),
                    AccessCode = await context.GetTokenAsync("code"),
                    AccessType = "offline",
                    RefreshToken = await context.GetTokenAsync("refresh_token"),
                    ClientId = _config["Authentication:Google:ClientId"],
                    ClientSecret = _config["Authentication:Google:ClientSecret"],
                    TokenExpiry = DateTime.Parse(await context.GetTokenAsync("expires_at")),
                    Scope = await context.GetTokenAsync("scope")

                };                
                //// Retrieve Contacts from API - for test only with Google
                var a = _googleResourceHandler["Google"].RetrieveContacts(oauthParams);
                //// Display user information
                //var response = context.Response;
                //response.ContentType = "application/json";
                //await response.WriteAsync("<html><body>");
                //await response.WriteAsync("Hello " + (context.User.Identity.Name ?? "anonymous") + "<br>");
                //foreach (var claim in context.User.Claims)
                //{
                //    await response.WriteAsync(claim.Type + ": " + claim.Value + "<br>");
                //}

                //await response.WriteAsync("Tokens:<br>");

                //await response.WriteAsync("Access Token: " + await context.GetTokenAsync("access_token") + "<br>");
                //await response.WriteAsync("Refresh Token: " + await context.GetTokenAsync("refresh_token") + "<br>");
                //await response.WriteAsync("Token Type: " + await context.GetTokenAsync("token_type") + "<br>");
                //await response.WriteAsync("expires_at: " + await context.GetTokenAsync("expires_at") + "<br>");
                //await response.WriteAsync("<a href=\"/logout\">Logout</a><br>");
                //await response.WriteAsync("<a href=\"/refresh_token\">Refresh Token</a><br>");
                //await response.WriteAsync("</body></html>");
            });
        }

        public void RouteAuthError()
        {

        }

    }
}