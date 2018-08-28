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

namespace GoferEx
{
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
      services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

      // Add Google Middleware Authentication - Is it?
      services.AddAuthentication().AddGoogle(googleOptions =>
      {
        googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
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

      services.AddTransient<IDbProvider, FileSystemProvider>();
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
      app.UseStaticFiles();
      app.UseCookiePolicy();
      app.UseCors("AllowAll");
      app.UseAuthentication();      
      app.UseMvc();
    }
  }
}
