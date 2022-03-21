using CompanionPlugin.Services;

using StreamCompanion.Classes;
using StreamCompanion.Services;

namespace StreamCompanion;

public static class Program
{
    private static void ConfigureLogging(WebApplicationBuilder builder)
    {
        builder.Logging
            .ClearProviders()
            .AddConsoleFormatter<ConsoleMessageFormatter, ConsoleMessageFormatterOptions>(opt => {
                opt.Layout = "{category:2}: {time} [{kind}]: {msg}";
            })
            .AddFilter("Microsoft.AspNetCore.Hosting.Diagnostics", LogLevel.Error)
            .AddFilter("Microsoft.AspNetCore.Routing", LogLevel.Error)
            .AddFilter("Microsoft.AspNetCore.Mvc", LogLevel.Error)
            .AddConsole(opt => {
                opt.FormatterName = "ConsoleMessageFormatter";
            })
            .SetMinimumLevel(LogLevel.Information);
    }

    private static void InitConfiguration(ConfigurationManager configuration)
    {
        //configuration.AddJsonFile("settings.json", false, true);
    }

    private static WebApplication BuildApp(WebApplicationBuilder builder)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        ConfigureLogging(builder);
        InitConfiguration(builder.Configuration);

        // Добавление сервисов
        StreamCompanionService companionService = new(builder.Services, builder.Configuration);
        builder.Services.AddSingleton(companionService);

        WebApplication app = builder.Build();

        // Инициализация основного сервиса
        companionService.Init(app.Services.GetService<ServiceResolver>());

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