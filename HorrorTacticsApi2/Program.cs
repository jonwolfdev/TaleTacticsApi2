using HorrorTacticsApi2;
using HorrorTacticsApi2.Data;
using HorrorTacticsApi2.Domain;
using HorrorTacticsApi2.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

IConfiguration configuration;

{
    var hostForConfig = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, app) =>
        {
            app
                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: false)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables(prefix: Constants.ENV_PREFIX);

        }).ConfigureServices((context, services) =>
        {
            // TODO: duplicated code
            services.AddOptions<AppSettings>()
                .Bind(context.Configuration.GetSection(Constants.APPSETTINGS_GENERAL_KEY))
                .ValidateDataAnnotations();

            services.AddSingleton(context.Configuration);
        });

    using var appForConfig = hostForConfig.Build();
    using var scope = appForConfig.Services.CreateScope();
    configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
}

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("./logs/ht-errors-init-.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning)
    .ReadFrom.Configuration(configuration)
    .CreateBootstrapLogger();

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

    // TODO: duplicated code
    builder.Services.AddOptions<AppSettings>()
        .Bind(builder.Configuration.GetSection(Constants.APPSETTINGS_GENERAL_KEY))
        .ValidateDataAnnotations();

    builder.AddJwt();

    builder.Services.AddDbContext<HorrorDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString(Constants.CONNECTION_STRING_MAIN_KEY))
    );

    builder.Services.AddScoped<ImageModelEntityConverter>();
    builder.Services.AddScoped<ImageService>();

    builder.Services
        .AddControllers()
        .AddNewtonsoftJson();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddCors(cors =>
    {
        cors.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin();
            policy.AllowAnyMethod();
            policy.AllowAnyHeader();
        });
    });

    using var app = builder.Build();

    {
        // Migrate
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Starting HorrorTactics...");
        await scope.ServiceProvider.MigrateDbAsync();
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        // TODO: what is the difference of having devexception page and not having it?
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors();

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseSerilogRequestLogging();

    app.MapControllers();

    app.Run();
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