using HorrorTacticsApi2;
using HorrorTacticsApi2.ApiHelpers;
using HorrorTacticsApi2.Data;
using HorrorTacticsApi2.Domain;
using HorrorTacticsApi2.Domain.IO;
using HorrorTacticsApi2.Game;
using HorrorTacticsApi2.Helpers;
using HorrorTacticsApi2.Hubs;
using HorrorTacticsApi2.Swagger;
using Jonwolfdev.Utils6.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;

bool enableInitLogging = false;
if (args.Length > 0)
{
    foreach(var arg in args)
    {
        if ("init-log".Equals(arg, StringComparison.OrdinalIgnoreCase))
        {
            enableInitLogging = true;
        }
    }
}

var logConfig = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("./logs/ht-errors-init-.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning);

if (enableInitLogging)
    logConfig.WriteTo.File("./logs/ht-log-init.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug);

Log.Logger = logConfig.CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration.AddEnvironmentVariables(prefix: Constants.ENV_PREFIX);
    
    builder.Host.UseSerilog((ctx, lc) =>
    {
        lc
            .WriteTo.Console()
            .ReadFrom.Configuration(ctx.Configuration);
    });

    // Add services to the container.

    builder.Services.AddOptions<AppSettings>()
        .Bind(builder.Configuration.GetSection(Constants.APPSETTINGS_GENERAL_KEY))
        .ValidateDataAnnotations();

    builder.AddJwt();

    builder.Services.AddDbContext<IHorrorDbContext, HorrorDbContext>(options => {
        options.UseNpgsql(builder.Configuration.GetConnectionString(Constants.CONNECTION_STRING_MAIN_KEY));
    });

    // ObjectValidator should only be used for Hub validations
    builder.Services.AddSingleton<IObjectValidator<HubObjectValidator>, HubObjectValidator>();

    builder.Services.AddScoped<ImageModelEntityHandler>();
    builder.Services.AddScoped<AudioModelEntityHandler>();
    builder.Services.AddScoped<StorySceneModelEntityHandler>();
    builder.Services.AddScoped<StoryModelEntityHandler>();
    builder.Services.AddScoped<GameModelStateHandler>();

    builder.Services.AddScoped<ImagesService>();
    builder.Services.AddScoped<AudiosService>();
    builder.Services.AddScoped<StoryScenesService>();
    builder.Services.AddScoped<StoriesService>();
    builder.Services.AddScoped<GamesService>();

    builder.Services.AddSingleton<GameSaver>();

    builder.Services.AddSingleton<FileUploadHandler>();
    builder.Services.AddSingleton<IFileIO, PhysicalFileIO>();
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddSingleton<IFileProvider>(services =>
    {
        var config = services.GetRequiredService<IOptions<AppSettings>>().Value;
        var physicalProvider = new PhysicalFileProvider(config.UploadPath);
        return physicalProvider;
    });

    builder.Services
        .AddSignalR()
        .AddNewtonsoftJsonProtocol();
    builder.Services
        .AddControllers()
        .AddNewtonsoftJson();

    builder.Services.AddApiVersioning(config =>
    {
        config.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
        config.AssumeDefaultVersionWhenUnspecified = true;
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(opt =>
    {
        opt.OperationFilter<FileUploadOperationFilter>();

        var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
        {
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Reference = new OpenApiReference()
            {
                Type = ReferenceType.SecurityScheme,
                Id = JwtBearerDefaults.AuthenticationScheme
            }
        };
        opt.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

        opt.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
        {
            { securityScheme, Array.Empty<string>() }
        });
    });

    builder.Services.AddCors(cors =>
    {
        cors.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin();
            policy.AllowAnyMethod();
            policy.AllowAnyHeader();
        });
    });

    {
        using var app = builder.Build();

        {
            // Start up operations...
            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("HorrorTactics starting up...");

            if (app.Environment.EnvironmentName.StartsWith(Constants.APITESTING_ENV_NAME))
            {
                var executor = scope.ServiceProvider.GetService<IApiTestingExecutor>();
                if (executor != default)
                    await executor.StartAsync(app);
            }

            await scope.ServiceProvider.MigrateDbAsync();
        }

        // TODO: change this to a appsettings bool variable
        if (app.Environment.IsProduction())
        {
            // From https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-apache?view=aspnetcore-3.1
            // For apache2 hosting (reverse proxy)
            // If proxy ip is different from localhost, go to the guide (and add `knownproxies`)
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else if (app.Environment.EnvironmentName.StartsWith(Constants.APITESTING_ENV_NAME))
        {
            // Do not use error handler
        }
        else
        {
            app.UseExceptionHandler("/error");
        }

        app.UseCors();

        // TODO: change this to a appsettings bool variable
        if (!app.Environment.IsProduction())
            app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSerilogRequestLogging();

        app.MapHub<GameHub>(Constants.HUB_PATH);
        app.MapControllers();

        app.Run();
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
    throw;
}
finally
{
    Log.Information("HorrorTactics shutdown.");
    Log.CloseAndFlush();
}


// This has to be added so it can be used within public classes
namespace HorrorTacticsApi2
{
    public partial class Program
    {
    }
}