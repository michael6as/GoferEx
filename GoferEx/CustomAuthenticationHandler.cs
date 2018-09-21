using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoferEx.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SchemeObject = GoferEx.Core.SchemeObject;

namespace GoferEx
{
  public class CustomAuthenticationHandler
  {
    private JsonSerializer _serializer;

    public CustomAuthenticationHandler()
    {
      _serializer = new JsonSerializer();
    }

    public void ConfigureLoginAuthRoutes(IApplicationBuilder app)
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
            await context.ChallengeAsync(authType, new AuthenticationProperties() { RedirectUri = "/" });
            return;
          }

          var response = context.Response;
          response.ContentType = "application/json";
          var schemeProvider = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
          var schemeNames = new List<SchemeObject>();
          foreach (var provider in await schemeProvider.GetAllSchemesAsync())
          {
            if (provider.DisplayName != null)
            {              
              schemeNames.Add(new SchemeObject(provider.Name,provider.DisplayName, "?authscheme=" + provider.Name));
            }
          }          
          await response.Body.WriteAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(schemeNames)));
        });
      });
    }

    public void ConfigureSuccessAuth(IApplicationBuilder app)
    {
      app.Run(async context =>
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

        // Display user information
        var response = context.Response;
        response.ContentType = "text/html";
        await response.WriteAsync("<html><body>");
        await response.WriteAsync("Hello " + (context.User.Identity.Name ?? "anonymous") + "<br>");
        foreach (var claim in context.User.Claims)
        {
          await response.WriteAsync(claim.Type + ": " + claim.Value + "<br>");
        }

        await response.WriteAsync("Tokens:<br>");

        await response.WriteAsync("Access Token: " + await context.GetTokenAsync("access_token") + "<br>");
        await response.WriteAsync("Refresh Token: " + await context.GetTokenAsync("refresh_token") + "<br>");
        await response.WriteAsync("Token Type: " + await context.GetTokenAsync("token_type") + "<br>");
        await response.WriteAsync("expires_at: " + await context.GetTokenAsync("expires_at") + "<br>");
        await response.WriteAsync("<a href=\"/logout\">Logout</a><br>");
        await response.WriteAsync("<a href=\"/refresh_token\">Refresh Token</a><br>");
        await response.WriteAsync("</body></html>");
      });
    }
  }
}
