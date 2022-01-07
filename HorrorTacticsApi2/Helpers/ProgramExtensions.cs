﻿using HorrorTacticsApi2.Data;
using Jonwolfdev.Utils6.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HorrorTacticsApi2.Helpers
{
    public static class ProgramExtensions
    {
        public static WebApplicationBuilder AddJwt(this WebApplicationBuilder builder)
        {
            // Jwt setup
            builder.Services.AddOptions<JwtGeneratorStaticOptions>()
                .Bind(builder.Configuration.GetSection(Constants.APPSETTINGS_JWTGENERATOR_KEY))
                .ValidateDataAnnotations();

            builder.Services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

            return builder;
        }

        public static async Task MigrateDbAsync(this IServiceProvider services)
        {
            // Making sure the database file is always updated
            var db = services.GetRequiredService<HorrorDbContext>();
            var logger = services.GetRequiredService<ILogger<Program>>();
            var pendingMigrations = await db.Database.GetPendingMigrationsAsync();
            
            if (pendingMigrations.Any())
            {
                if (File.Exists(Constants.FILE_APPLY_MIGRATIONS))
                {
                    logger.LogInformation("Applying migrations...");
                    await db.Database.MigrateAsync();
                    logger.LogInformation("Finished migrations.");
                    logger.LogInformation($"Deleting file {Constants.FILE_APPLY_MIGRATIONS}");
                    try
                    {
                        File.Delete(Constants.FILE_APPLY_MIGRATIONS);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Couldn't delete {Constants.FILE_APPLY_MIGRATIONS} file", ex);
                    }
                }
                else
                {
                    throw new InvalidOperationException($"There are pending migrations. " +
                    $"Backup the database file then create '{Constants.FILE_APPLY_MIGRATIONS}' file and run HorrorTactics again");
                }
            }
        }
    }
}
