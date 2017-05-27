using System;
using Microsoft.Extensions.Logging;
using AspNetCoreIdentity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AspNetCoreIdentity
{
    using UserManager = UserManager<ApplicationUser>;

    public class AppSettings
    {
        public ApplicationUser[] TestUsers { get; set; }
    }

    public static class Extensions
    {
        public static void UseTestData(this IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            var appSettings = app.ApplicationServices.GetService<IOptions<AppSettings>>();
            var userManager = app.ApplicationServices.GetService<UserManager>();
            AddTestUsers(appSettings, userManager, loggerFactory);
        }

        private static async void AddTestUsers(IOptions<AppSettings> appSettings, UserManager userManager, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<Startup>();

            foreach (var user in appSettings.Value.TestUsers)
            {
                user.Id = Guid.NewGuid().ToString();
                user.JoinDate = DateTime.UtcNow;
                const string notSoStrongPassword = "Str0ng!";

                var result = await userManager.CreateAsync(user, notSoStrongPassword);
                if (result.Succeeded)
                {
                    logger.LogInformation(0x03,
                        $"System created a new test account. Name: '{user.FullName}', UserName: '{user.UserName}'");
                }
                else foreach (var error in result.Errors)
                {
                    logger.LogError(0xE03, error.Description);
                }
            }
        }

    }
}
