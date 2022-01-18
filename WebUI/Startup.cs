using System;
using AutoMapper;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using DataBase;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.Telegram.Service;
using WebUI.Mappers;
using WebUI.Services.Identity;
using WebUI.Services.Identity.Impl;
using WebUI.Services.Profile;
using WebUI.Services.Profile.Impl;

namespace WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            // configure Blazorise
            var blazor = services.AddServerSideBlazor();
            blazor.AddCircuitOptions(o => o.DetailedErrors = true);
            blazor.AddHubOptions(o => o.MaximumReceiveMessageSize = 10 * 1024 * 1024 /* 10 Mb */);

            services.Configure<RazorPagesOptions>(options => options.RootDirectory = "/Pages");

            services.AddBlazorise(
                    options =>
                    {
                        options.ChangeTextOnKeyPress = true;
                    }
                )
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new UserStateProfile());
            });

            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddHttpContextAccessor();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.AddControllers();
            services.AddSession(_ =>
            {
                _.IOTimeout = TimeSpan.FromDays(1);
                _.IdleTimeout = TimeSpan.FromMinutes(120);
            });

            services.UsePostgresDatabase(Configuration);
            services.AddTelegramService(Configuration);

            services.AddSingleton<IIdentityManager, IdentityManager>();
            services.AddSingleton<IProfileService, ProfileService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapControllers();
                endpoints.MapFallbackToPage("/_Host");
            });

            serviceProvider.ApplyMigrations();
            serviceProvider.InitializeTelegramService();
        }
    }
}
