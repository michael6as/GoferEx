using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoferEx.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace GoferEx.Server.Helpers
{
    public static class StartupAuthExtensions
    {
        public static void AddLoginRoute(this IApplicationBuilder app, IConfiguration config)
        {
            app.Map("/login", signinApp =>
            {
                signinApp.Run(async context =>
                {
                    var authType = context.Request.Query["authscheme"];
                    if (!string.IsNullOrEmpty(authType))
                    {
                        // By default the client will be redirect back to the URL that issued the challenge (/login?authtype=foo),
                        // send them to the home page instead (/).
                        await context.ChallengeAsync(authType,
                            new AuthenticationProperties() {RedirectUri = "http://localhost:8080"});
                        return;
                    }

                    var schemeProvider = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
                    var schemeNames = (from provider in await schemeProvider.GetAllSchemesAsync()
                            where provider.DisplayName != null
                            select new SchemeObject(provider.Name, provider.DisplayName,
                                "?authscheme=" + provider.Name))
                        .ToList();

                    var response = context.Response;
                    response.ContentType = "application/json";
                    await response.Body.WriteAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(schemeNames)));
                });
            });
        }

        public static void AddFailedRoute(this IApplicationBuilder app, IConfiguration config)
        {
            app.Map("/error", errorApp =>
            {
                errorApp.Run(async context =>
                {
                    HttpResponse response = context.Response;
                    response.ContentType = "text/html";
                    await response.WriteAsync("<html><body>");
                    await response.WriteAsync("An remote failure has occurred: " +
                                              context.Request.Query["FailureMessage"] + "<br>");
                    await response.WriteAsync("<a href=\"/\">Home</a>");
                    await response.WriteAsync("</body></html>");
                });
            });
        }

        public static void AddScopes(this OAuthOptions options, IConfiguration config)
        {
            foreach (var scope in JsonConvert.DeserializeObject<List<string>>(config["Authentication:Google:Scopes"]))
            {
                options.Scope.Add(scope);
            }
        }
    }
}
