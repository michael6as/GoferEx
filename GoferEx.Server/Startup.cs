using GoferEx.ExternalResources;
using GoferEx.ExternalResources.Google;
using GoferEx.Server.Helpers;
using GoferEx.Server.Helpers.Interfaces;
using GoferEx.Storage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using GoferEx.ExternalResources.Abstract;
using GoferEx.Server.Helpers.TokenFactory;

namespace GoferEx.Server
{
    /// <summary>
    /// Startup class for initializing all components and middleware for your web app
    /// </summary>
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IList<IAuthProvider> AuthProviders { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AuthProviders = new List<IAuthProvider>() { new GoogleAuthProvider(Configuration) };
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var serviceAuthBuilder = services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o => o.LoginPath = new PathString("/login"));
            foreach (var authProvider in AuthProviders)
            {
                authProvider.AddAuthMiddleware(serviceAuthBuilder);
            }
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

            // Initialize all classes instances for the BL and DAL
            IDbProvider dbProvider = new RedisDbProvider(Configuration["DbConnection:Connection"]);
            var defaultImage = File.ReadAllBytes(Configuration["DefaultSettings:DefaultImage"]);
            IDictionary<string, IResourceHandler> resourceHandlers = new Dictionary<string, IResourceHandler>() { { "Google", new GoogleResourceHandler(defaultImage) } };
            services.AddSingleton<IDataHandler>(new BasicDataHandler(resourceHandlers, dbProvider));
            services.AddSingleton<IAuthTokenFactory>(new SingleAuthTokenFactory());

            //services.AddSingleton<IResourceHandler>(new GoogleResourceHandler());
            //services.AddSingleton<IDbProvider>(new RedisDbProvider(Configuration["DbConnection:Connection"])); 
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {            
            app.UseDeveloperExceptionPage();
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.AddLoginRoute(Configuration);
            app.AddFailedRoute(Configuration);

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
