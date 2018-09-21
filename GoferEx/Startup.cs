using GoferEx.ExternalResources;
using GoferEx.ExternalResources.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace GoferEx
{
  public class Startup
  {
    private readonly IHostingEnvironment _env;
    private readonly ILoggerFactory _loggerFactory;

    public Startup(IHostingEnvironment env, IConfiguration config,
      ILoggerFactory loggerFactory)
    {
      _env = env;
      _loggerFactory = loggerFactory;
      Configuration = config;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
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
                  o.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
                  o.Scope.Add("https://www.googleapis.com/auth/userinfo.email");
                  o.Scope.Add("https://www.googleapis.com/auth/user.phonenumbers.read");
                  o.Scope.Add("https://www.googleapis.com/auth/user.emails.read");
                  o.Scope.Add("https://www.googleapis.com/auth/user.birthday.read");
                  o.Scope.Add("https://www.googleapis.com/auth/user.addresses.read");
                  o.Scope.Add("https://www.googleapis.com/auth/plus.login");
                  o.Scope.Add("https://www.googleapis.com/auth/contacts.readonly");
                  o.Scope.Add("https://www.googleapis.com/auth/contacts");
                });
      services.AddCors(options =>
      {
        options.AddPolicy("AllowAll", p =>
        {
          p.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
      });

      //services.AddTransient<IDbProvider, FileSystemProvider>();
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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

      // context.Response.Redirect("/error?FailureMessage=" + UrlEncoder.Default.Encode(context.Failure.Message));

      context.HandleResponse();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      //Dictionary<string, IResourceHandler<CustomGoogleToken>> d = new Dictionary<string, IResourceHandler<CustomGoogleToken>>();
      //d.Add("Google", new ContactResourceHandler("aaaa"));
      //CustomAuthHelper c = new CustomAuthHelper(app, d, Configuration);
      //app.UseDeveloperExceptionPage();
      //app.UseCors("AllowAll");

      //app.UseAuthentication();
      //c.RouteAuthLogin();

      //app.Map("/refresh_token", signinApp =>
      //{
      //  signinApp.Run(async context =>
      //  {
      //    HttpResponse response = context.Response;

      //    // Setting DefaultAuthenticateScheme causes User to be set
      //    // var user = context.User;

      //    // This is what [Authorize] calls
      //    AuthenticateResult userResult = await context.AuthenticateAsync();
      //    ClaimsPrincipal user = userResult.Principal;
      //    AuthenticationProperties authProperties = userResult.Properties;

      //    // This is what [Authorize(ActiveAuthenticationSchemes = MicrosoftAccountDefaults.AuthenticationScheme)] calls
      //    // var user = await context.AuthenticateAsync(MicrosoftAccountDefaults.AuthenticationScheme);

      //    // Deny anonymous request beyond this point.
      //    if (!userResult.Succeeded || user == null || !user.Identities.Any(identity => identity.IsAuthenticated))
      //    {
      //      // This is what [Authorize] calls
      //      // The cookie middleware will handle this and redirect to /login
      //      await context.ChallengeAsync();

      //      // This is what [Authorize(ActiveAuthenticationSchemes = MicrosoftAccountDefaults.AuthenticationScheme)] calls
      //      // await context.ChallengeAsync(MicrosoftAccountDefaults.AuthenticationScheme);

      //      return;
      //    }

      //    string currentAuthType = user.Identities.First().AuthenticationType;
      //    if (string.Equals(GoogleDefaults.AuthenticationScheme, currentAuthType))
      //    {
      //      string refreshToken = authProperties.GetTokenValue("refresh_token");

      //      if (string.IsNullOrEmpty(refreshToken))
      //      {
      //        response.ContentType = "text/html";
      //        await response.WriteAsync("<html><body>");
      //        await response.WriteAsync("No refresh_token is available.<br>");
      //        await response.WriteAsync("<a href=\"/\">Home</a>");
      //        await response.WriteAsync("</body></html>");
      //        return;
      //      }

      //      OAuthOptions options = await GetOAuthOptionsAsync(context, currentAuthType);

      //      Dictionary<string, string> pairs = new Dictionary<string, string>()
      //                  {
      //                      { "client_id", options.ClientId },
      //                      { "client_secret", options.ClientSecret },
      //                      { "grant_type", "refresh_token" },
      //                      { "refresh_token", refreshToken }
      //                  };
      //      FormUrlEncodedContent content = new FormUrlEncodedContent(pairs);
      //      HttpResponseMessage refreshResponse = await options.Backchannel.PostAsync(options.TokenEndpoint, content, context.RequestAborted);
      //      refreshResponse.EnsureSuccessStatusCode();

      //      JObject payload = JObject.Parse(await refreshResponse.Content.ReadAsStringAsync());

      //      // Persist the new acess token
      //      authProperties.UpdateTokenValue("access_token", payload.Value<string>("access_token"));
      //      refreshToken = payload.Value<string>("refresh_token");
      //      if (!string.IsNullOrEmpty(refreshToken))
      //      {
      //        authProperties.UpdateTokenValue("refresh_token", refreshToken);
      //      }
      //      if (int.TryParse(payload.Value<string>("expires_in"), NumberStyles.Integer, CultureInfo.InvariantCulture, out int seconds))
      //      {
      //        DateTimeOffset expiresAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(seconds);
      //        authProperties.UpdateTokenValue("expires_at", expiresAt.ToString("o", CultureInfo.InvariantCulture));
      //      }
      //      await context.SignInAsync(user, authProperties);

      //      await PrintRefreshedTokensAsync(response, payload, authProperties);

      //      return;
      //    }
      //    // https://developers.facebook.com/docs/facebook-login/access-tokens/expiration-and-extension

      //    response.ContentType = "text/html";
      //    await response.WriteAsync("<html><body>");
      //    await response.WriteAsync("Refresh has not been implemented for this provider.<br>");
      //    await response.WriteAsync("<a href=\"/\">Home</a>");
      //    await response.WriteAsync("</body></html>");
      //  });
      //});

      //// Sign-out to remove the user cookie.
      //app.Map("/logout", signoutApp =>
      //{
      //  signoutApp.Run(async context =>
      //  {
      //    HttpResponse response = context.Response;
      //    response.ContentType = "text/html";
      //    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
      //    await response.WriteAsync("<html><body>");
      //    await response.WriteAsync("You have been logged out. Goodbye " + context.User.Identity.Name + "<br>");
      //    await response.WriteAsync("<a href=\"/\">Home</a>");
      //    await response.WriteAsync("</body></html>");
      //  });
      //});

      //// Display the remote error
      //app.Map("/error", errorApp =>
      //{
      //  errorApp.Run(async context =>
      //  {
      //    HttpResponse response = context.Response;
      //    response.ContentType = "text/html";
      //    await response.WriteAsync("<html><body>");
      //    await response.WriteAsync("An remote failure has occurred: " + context.Request.Query["FailureMessage"] + "<br>");
      //    await response.WriteAsync("<a href=\"/\">Home</a>");
      //    await response.WriteAsync("</body></html>");
      //  });
      //});

      ////c.RouteAuthSuccess();

      //app.UseHttpsRedirection();
      ////CustomAuthenticationHandler c = new CustomAuthenticationHandler();
      ////c.ConfigureLoginAuthRoutes(app);
      ////c.ConfigureSuccessAuth(app);
      //app.UseMvc();
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
