using GoferEx.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Rewrite.Internal.UrlActions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace GoferEx
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      var builder = new ConfigurationBuilder()        
        .AddJsonFile("appSettings.json",
          optional: false,
          reloadOnChange: true)
        .AddEnvironmentVariables();
        builder.AddUserSecrets<Startup>();     
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();      
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
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
      services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddGoogle(googleOptions =>
      {
        googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
        googleOptions.CallbackPath = "/signin-google";
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseHsts();
      }
      app.UseCors("AllowAll");      
      app.UseHttpsRedirection();      
      app.UseAuthentication();
      app.Build();
      app.UseMvc();
    }
  }
}
