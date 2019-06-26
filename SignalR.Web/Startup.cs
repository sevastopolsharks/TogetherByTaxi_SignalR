using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SignalR.Web.Hubs;

namespace SignalR.Web
{
    public class Startup
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// IConfiguration
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddAuthorization();
            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
            var identityServerAuthenticationAuthority = GetStringFromConfig("IdentityServerAuthentication", "Authority");
            var identityServerAuthenticationApiName = GetStringFromConfig("IdentityServerAuthentication", "ApiName");
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = identityServerAuthenticationAuthority;
                    options.ApiName = identityServerAuthenticationApiName;
                    options.TokenRetriever = CustomTokenRetriever.FromHeaderAndQueryString;
                    options.RequireHttpsMetadata = false;
                });

            services
                .InstallConfiguration(Configuration)
                .ConfigureBusManager()
                .ConfigureAutomapper()
                .AddSignalR();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            string GetStringFromConfig(string firstName, string secondName)
            {
                var result = Configuration[$"{firstName}:{secondName}"];
                if (string.IsNullOrEmpty(result))
                {
                    result = Configuration[$"{firstName}_{secondName}"];
                }

                if (string.IsNullOrEmpty(result))
                {
                    throw new Exception($"Configuration setting does not exist. Setting name {firstName}:{secondName}");
                }

                return result;
            }
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
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseAuthentication();

            //app.UseHttpsRedirection();
            //app.UseStaticFiles();
            //app.UseCookiePolicy();

            app.UseCors(b =>
            {
                b.AllowAnyHeader();
                b.AllowAnyMethod();
                b.AllowAnyOrigin();
                b.AllowCredentials();
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<UserHub>("/userhub");
            });
            app.UseMvc();
        }
    }
}
