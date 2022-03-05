using System.Text.Json;
using System.Text.Json.Serialization;

using CompanionPlugin.Services;

using StreamCompanion.Services;

namespace StreamCompanion;

public static class Program
{
    private static void ConfigureLogging(WebApplicationBuilder builder)
    {
        builder.Host.ConfigureLogging(logging => {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
    }

    private static void InitConfiguration(ConfigurationManager configuration)
    {
        //configuration.AddJsonFile("settings.json", false, true);
    }

    private static WebApplication BuildApp(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers()
            .AddJsonOptions(options => {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.Configure<JsonSerializerOptions>(options => {
            options.Converters.Add(new JsonStringEnumConverter(null, false));
        });

        ConfigureLogging(builder);
        InitConfiguration(builder.Configuration);

        // Добавление сервисов
        StreamCompanionService companionService = new(builder.Services, builder.Configuration);
        builder.Services.AddSingleton(companionService);

        WebApplication app = builder.Build();

        // Инициализация основного сервиса
        companionService.Init(app.Services.GetService<ServiceResolver>());

        app.Logger.LogInformation("Initialized");

        return app;
    }

    private static void ConfigureApp(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthorization();
        app.MapControllers();
    }

    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        WebApplication app = BuildApp(builder);
        
        ConfigureApp(app);

        app.Run();
    }
}