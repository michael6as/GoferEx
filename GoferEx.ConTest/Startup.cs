using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GoferEx.Core;
using GoferEx.ExternalResources;
using GoferEx.ExternalResources.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GoferEx.ConTest
{
    /* Note all servers must use the same address and port because these are pre-registered with the various providers. */
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            Configuration["Authentication:Google:ClientId"] = "535817455358-tlgkg5jca5u3kjd3jlhqr4u1ef6udt7f.apps.googleusercontent.com";
            Configuration["Authentication:Google:ClientSecret"] = "Mk8mvSz3SmHmi9Enz19vsPbX";
        }

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o => o.LoginPath = new PathString("/login"))
                // You must first create an app with Google and add its ID and Secret to your user-secrets.
                // https://console.developers.google.com/project
                .AddOAuth("Google-AccessToken", "Google AccessToken only", o =>
                {
                    o.ClientId = Configuration["Authentication:Google:ClientId"];
                    o.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                    o.CallbackPath = new PathString("/signin-google-token");
                    o.AuthorizationEndpoint = GoogleDefaults.AuthorizationEndpoint;
                    o.TokenEndpoint = GoogleDefaults.TokenEndpoint;
                    o.Scope.Add("openid");
                    o.Scope.Add("profile");
                    o.Scope.Add("email");
                    o.SaveTokens = true;
                    o.Events = new OAuthEvents()
                    {
                        OnRemoteFailure = HandleOnRemoteFailure
                    };
                })
                // You must first create an app with Google and add its ID and Secret to your user-secrets.
                // https://console.developers.google.com/project
                .AddGoogle(o =>
                {
                    o.ClientId = Configuration["Authentication:Google:ClientId"];
                    o.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                    o.AuthorizationEndpoint += "?prompt=consent"; // Hack so we always get a refresh token, it only comes on the first authorization response
                    o.AccessType = "offline";
                    o.SaveTokens = true;
                    o.Events = new OAuthEvents()
                    {
                        OnRemoteFailure = HandleOnRemoteFailure
                    };
                    o.ClaimActions.MapJsonSubKey("urn:google:image", "image", "url");
                    o.ClaimActions.Remove(ClaimTypes.GivenName);
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
                foreach (var pair in context.Properties.Items)
                {
                    await context.Response.WriteAsync($"-{ UrlEncoder.Default.Encode(pair.Key)}={ UrlEncoder.Default.Encode(pair.Value)}<br>");
                }
            }

            await context.Response.WriteAsync("<a href=\"/\">Home</a>");
            await context.Response.WriteAsync("</body></html>");

            // context.Response.Redirect("/error?FailureMessage=" + UrlEncoder.Default.Encode(context.Failure.Message));

            context.HandleResponse();
        }

        public void Configure(IApplicationBuilder app)
        {
            var d = new Dictionary<string, IResourceHandler<CustomGoogleToken>>();
            d.Add("Google", new ContactResourceHandler("aaaa"));
            var c = new CustomAuthHelper(app, d, Configuration);
            app.UseDeveloperExceptionPage();

            app.UseAuthentication();
            c.RouteAuthLogin();
            // Choose an authentication type
            //app.Map("/login", signinApp =>
            //{
            //    signinApp.Run(async context =>
            //    {
            //        var authType = context.Request.Query["authscheme"];
            //        if (!string.IsNullOrEmpty(authType))
            //        {
            //            // By default the client will be redirect back to the URL that issued the challenge (/login?authtype=foo),
            //            // send them to the home page instead (/).
            //            await context.ChallengeAsync(authType, new AuthenticationProperties() { RedirectUri = "/" });
            //            return;
            //        }

            //        var response = context.Response;
            //        response.ContentType = "application/json";

            //        await response.WriteAsync("Choose an authentication scheme: <br>");
            //        var schemeProvider = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
            //        var authProviders = new List<AuthInfoObject>();
            //        foreach (var provider in await schemeProvider.GetAllSchemesAsync())
            //        {
            //            if (provider.DisplayName != null)
            //            {
            //                authProviders.Add(new AuthInfoObject(provider.Name, provider.DisplayName, "?authscheme = " + provider.Name));
            //            }
            //        }
            //        await response.WriteAsync(JsonConvert.SerializeObject(authProviders));
            //    });
            //});

            // Refresh the access token
            app.Map("/refresh_token", signinApp =>
            {
                signinApp.Run(async context =>
                {
                    var response = context.Response;

                    // Setting DefaultAuthenticateScheme causes User to be set
                    // var user = context.User;

                    // This is what [Authorize] calls
                    var userResult = await context.AuthenticateAsync();
                    var user = userResult.Principal;
                    var authProperties = userResult.Properties;

                    // This is what [Authorize(ActiveAuthenticationSchemes = MicrosoftAccountDefaults.AuthenticationScheme)] calls
                    // var user = await context.AuthenticateAsync(MicrosoftAccountDefaults.AuthenticationScheme);

                    // Deny anonymous request beyond this point.
                    if (!userResult.Succeeded || user == null || !user.Identities.Any(identity => identity.IsAuthenticated))
                    {
                        // This is what [Authorize] calls
                        // The cookie middleware will handle this and redirect to /login
                        await context.ChallengeAsync();

                        // This is what [Authorize(ActiveAuthenticationSchemes = MicrosoftAccountDefaults.AuthenticationScheme)] calls
                        // await context.ChallengeAsync(MicrosoftAccountDefaults.AuthenticationScheme);

                        return;
                    }

                    var currentAuthType = user.Identities.First().AuthenticationType;
                    if (string.Equals(GoogleDefaults.AuthenticationScheme, currentAuthType))
                    {
                        var refreshToken = authProperties.GetTokenValue("refresh_token");

                        if (string.IsNullOrEmpty(refreshToken))
                        {
                            response.ContentType = "text/html";
                            await response.WriteAsync("<html><body>");
                            await response.WriteAsync("No refresh_token is available.<br>");
                            await response.WriteAsync("<a href=\"/\">Home</a>");
                            await response.WriteAsync("</body></html>");
                            return;
                        }

                        var options = await GetOAuthOptionsAsync(context, currentAuthType);

                        var pairs = new Dictionary<string, string>()
                        {
                            { "client_id", options.ClientId },
                            { "client_secret", options.ClientSecret },
                            { "grant_type", "refresh_token" },
                            { "refresh_token", refreshToken }
                        };
                        var content = new FormUrlEncodedContent(pairs);
                        var refreshResponse = await options.Backchannel.PostAsync(options.TokenEndpoint, content, context.RequestAborted);
                        refreshResponse.EnsureSuccessStatusCode();

                        var payload = JObject.Parse(await refreshResponse.Content.ReadAsStringAsync());

                        // Persist the new acess token
                        authProperties.UpdateTokenValue("access_token", payload.Value<string>("access_token"));
                        refreshToken = payload.Value<string>("refresh_token");
                        if (!string.IsNullOrEmpty(refreshToken))
                        {
                            authProperties.UpdateTokenValue("refresh_token", refreshToken);
                        }
                        if (int.TryParse(payload.Value<string>("expires_in"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var seconds))
                        {
                            var expiresAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(seconds);
                            authProperties.UpdateTokenValue("expires_at", expiresAt.ToString("o", CultureInfo.InvariantCulture));
                        }
                        await context.SignInAsync(user, authProperties);

                        await PrintRefreshedTokensAsync(response, payload, authProperties);

                        return;
                    }
                    // https://developers.facebook.com/docs/facebook-login/access-tokens/expiration-and-extension

                    response.ContentType = "text/html";
                    await response.WriteAsync("<html><body>");
                    await response.WriteAsync("Refresh has not been implemented for this provider.<br>");
                    await response.WriteAsync("<a href=\"/\">Home</a>");
                    await response.WriteAsync("</body></html>");
                });
            });

            // Sign-out to remove the user cookie.
            app.Map("/logout", signoutApp =>
            {
                signoutApp.Run(async context =>
                {
                    var response = context.Response;
                    response.ContentType = "text/html";
                    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    await response.WriteAsync("<html><body>");
                    await response.WriteAsync("You have been logged out. Goodbye " + context.User.Identity.Name + "<br>");
                    await response.WriteAsync("<a href=\"/\">Home</a>");
                    await response.WriteAsync("</body></html>");
                });
            });

            // Display the remote error
            app.Map("/error", errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var response = context.Response;
                    response.ContentType = "text/html";
                    await response.WriteAsync("<html><body>");
                    await response.WriteAsync("An remote failure has occurred: " + context.Request.Query["FailureMessage"] + "<br>");
                    await response.WriteAsync("<a href=\"/\">Home</a>");
                    await response.WriteAsync("</body></html>");
                });
            });

            c.RouteAuthSuccess();
            //app.Run(async context =>
            //{
            //    // Setting DefaultAuthenticateScheme causes User to be set
            //    var user = context.User;

            //    // This is what [Authorize] calls
            //    // var user = await context.AuthenticateAsync();

            //    // This is what [Authorize(ActiveAuthenticationSchemes = MicrosoftAccountDefaults.AuthenticationScheme)] calls
            //    // var user = await context.AuthenticateAsync(MicrosoftAccountDefaults.AuthenticationScheme);

            //    // Deny anonymous request beyond this point.
            //    if (user == null || !user.Identities.Any(identity => identity.IsAuthenticated))
            //    {
            //        // This is what [Authorize] calls
            //        // The cookie middleware will handle this and redirect to /login
            //        await context.ChallengeAsync();

            //        // This is what [Authorize(ActiveAuthenticationSchemes = MicrosoftAccountDefaults.AuthenticationScheme)] calls
            //        // await context.ChallengeAsync(MicrosoftAccountDefaults.AuthenticationScheme);

            //        return;
            //    }

            //    // Display user information
            //    var response = context.Response;
            //    response.ContentType = "text/html";
            //    await response.WriteAsync("<html><body>");
            //    await response.WriteAsync("Hello " + (context.User.Identity.Name ?? "anonymous") + "<br>");
            //    foreach (var claim in context.User.Claims)
            //    {
            //        await response.WriteAsync(claim.Type + ": " + claim.Value + "<br>");
            //    }
            //    await response.WriteAsync("Tokens:<br>");

            //    await response.WriteAsync("Access Token: " + await context.GetTokenAsync("access_token") + "<br>");
            //    await response.WriteAsync("Refresh Token: " + await context.GetTokenAsync("refresh_token") + "<br>");
            //    await response.WriteAsync("Token Type: " + await context.GetTokenAsync("token_type") + "<br>");
            //    await response.WriteAsync("expires_at: " + await context.GetTokenAsync("expires_at") + "<br>");
            //    await response.WriteAsync("<a href=\"/logout\">Logout</a><br>");
            //    await response.WriteAsync("<a href=\"/refresh_token\">Refresh Token</a><br>");
            //    await response.WriteAsync("</body></html>");
            //});
        }

        private Task<OAuthOptions> GetOAuthOptionsAsync(HttpContext context, string currentAuthType)
        {
            if (string.Equals(GoogleDefaults.AuthenticationScheme, currentAuthType))
            {
                return Task.FromResult<OAuthOptions>(context.RequestServices.GetRequiredService<IOptionsMonitor<GoogleOptions>>().Get(currentAuthType));
            }
            throw new NotImplementedException(currentAuthType);
        }

        private async Task PrintRefreshedTokensAsync(HttpResponse response, JObject payload, AuthenticationProperties authProperties)
        {
            response.ContentType = "text/html";
            await response.WriteAsync("<html><body>");
            await response.WriteAsync("Refreshed.<br>");
            await response.WriteAsync(HtmlEncoder.Default.Encode(payload.ToString()).Replace(",", ",<br>") + "<br>");

            await response.WriteAsync("<br>Tokens:<br>");

            await response.WriteAsync("Access Token: " + authProperties.GetTokenValue("access_token") + "<br>");
            await response.WriteAsync("Refresh Token: " + authProperties.GetTokenValue("refresh_token") + "<br>");
            await response.WriteAsync("Token Type: " + authProperties.GetTokenValue("token_type") + "<br>");
            await response.WriteAsync("expires_at: " + authProperties.GetTokenValue("expires_at") + "<br>");

            await response.WriteAsync("<a href=\"/\">Home</a><br>");
            await response.WriteAsync("<a href=\"/refresh_token\">Refresh Token</a><br>");
            await response.WriteAsync("</body></html>");
        }
    }
}

