using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoferEx.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using System.Text;
using System.IO;
using System.Globalization;
using Microsoft.AspNetCore.Owin;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.Cookies;

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
      services.AddAuthentication(options => {
        options.DefaultScheme = "Cookies";
      }).AddCookie("Cookies", options => {
        options.Cookie.Name = "auth_cookie";
        options.Cookie.SameSite = SameSiteMode.None;
        options.Events = new CookieAuthenticationEvents
        {
          OnRedirectToLogin = redirectContext =>
          {
            redirectContext.HttpContext.Response.StatusCode = 401;
            return Task.CompletedTask;
          }
        };
      });
      services.AddAuthentication(
        v =>
        {
          v.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
          v.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
        }).AddGoogle(googleOptions =>
      {
        //googleOptions.Backchannel = new HttpClient(new HttpClientHandler());
        googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
        googleOptions.AuthorizationEndpoint = GoogleDefaults.AuthorizationEndpoint;
        googleOptions.TokenEndpoint = GoogleDefaults.TokenEndpoint;
        googleOptions.StateDataFormat = new PropertiesDataFormat(new CustomDataProtector());
        googleOptions.Events = CreateEvent(googleOptions.StateDataFormat);
        googleOptions.Validate();
      });
      var logger = _loggerFactory.CreateLogger<Startup>();

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

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {        
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
      }
      else
      {        
        app.UseExceptionHandler("/Error");
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseCors("AllowAll");
      app.UseAuthentication();
      var c = new CustomAuthenticationHandler();
      c.ConfigureLoginAuthRoutes(app);
      c.ConfigureSuccessAuth(app);      
      //app.UseMvc();      
    }

    private OAuthEvents CreateEvent(ISecureDataFormat<AuthenticationProperties> ppp)
    {
      return new OAuthEvents
      {
        // After OAuth2 has authenticated the user
        OnCreatingTicket = async context =>
        {
          // Create the request message to get user data via the backchannel
          var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
          await Task.FromResult(0);
        },
        OnRemoteFailure = context =>
        {
          var a = context.Request.Query;
          var state = context.Request.Query["state"].FirstOrDefault();
          if (state != null)
          {
            //var options = context.HttpContext.RequestServices.GetRequiredService<AuthenticationProperties>();
            try
            {
              var properties = ppp.Unprotect(state);
            }
            catch (Exception ex)
            {
              context.Response.Redirect("/Account/Login");
              context.HandleResponse();
            }
          }
          return Task.FromResult(0);
        },
        OnRedirectToAuthorizationEndpoint = async context =>
        {
          var a = context.RedirectUri;
          await Task.FromResult(0);
        },
        OnTicketReceived = async context =>
        {
          var a = context.ReturnUri;
          await Task.FromResult(0);
        }
      };
    }
  }
}
