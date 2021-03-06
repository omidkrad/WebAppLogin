﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AspNetCoreIdentity.Data;
using AspNetCoreIdentity.Middlewares;
using AspNetCoreIdentity.Models;
using AspNetCoreIdentity.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Infrastructure;

namespace AspNetCoreIdentity
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public static IConnectionManager ConnectionManager;

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
                options.UseInMemoryDatabase());

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddIdentity<ApplicationUser, IdentityRole>(identityOptions =>
                   // Enables immediate logout, after updating the user's state.
                   identityOptions.SecurityStampValidationInterval = TimeSpan.Zero
                )
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddSignalR(options =>
            {
                options.Hubs.EnableDetailedErrors = true;
            });

            services.AddMvc();
            services.AddDistributedMemoryCache();
            services.AddSession();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            ConnectionManager = serviceProvider.GetService<IConnectionManager>();

            app.UseSurfaceDefense(
                timeSlice: TimeSpan.FromMinutes(10),
                maxHitsAllowedPerTimeSlice: 20
            );

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                LoginPath = new PathString("/Account/Login"),
                Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = context =>
                    {
                        SecurityStampValidator.ValidatePrincipalAsync(context);
                        return Task.FromResult(0);
                    },
                },
                ExpireTimeSpan = TimeSpan.FromSeconds(30),
                SlidingExpiration = true
            });

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseTestData(loggerFactory);

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            app.UseSession();

            app.Map("/session", subApp =>
            {
                subApp.Run(async context =>
                {
                    var visitCount = context.Session.GetInt32("visits") ?? 0;
                    context.Session.SetInt32("visits", ++visitCount);
                    await context.Response.WriteAsync($"You have visited this page {visitCount} times.");
                });
            });

            app.UseSignalR();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}
