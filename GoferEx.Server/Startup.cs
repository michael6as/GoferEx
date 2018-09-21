using GoferEx.ExternalResources;
using GoferEx.ExternalResources.Google;
using GoferEx.Server.Helpers;
using GoferEx.Storage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GoferEx.Server.Helpers.Interfaces;
using GoferEx.Server.Helpers.TokenFactory;

namespace GoferEx.Server
{
    //TODO 1: Documentation here
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //TODO 1: Create Interface for adding Authentications
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o => o.LoginPath = new PathString("/login"))
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
                    o.AddScopes(Configuration);
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

            services.AddSingleton<IResourceHandler>(new GoogleResourceHandler());
            services.AddSingleton<IDbProvider>(new RedisDbProvider(Configuration["DbConnection:Connection"]));
            services.AddSingleton<IAuthTokenFactory>(new SingleAuthTokenFactory());
            //services.AddTransient<IDbProvider, FileSystemProvider>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //Dictionary<string, IResourceHandler<CustomGoogleToken>> d = new Dictionary<string, IResourceHandler<CustomGoogleToken>>();
            //d.Add("Google", new ContactResourceHandler("aaaa"));
            //CustomAuthHelper c = new CustomAuthHelper(app, d, Configuration);
            app.UseDeveloperExceptionPage();
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.AddLoginRoute(Configuration);
            app.AddFailedRoute(Configuration);

            app.UseHttpsRedirection();
            app.UseMvc();
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
    }
}
