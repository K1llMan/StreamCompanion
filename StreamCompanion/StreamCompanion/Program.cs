using System.Text.Json;
using System.Text.Json.Serialization;

using CompanionPlugin.Interfaces;
using CompanionPlugin.Services;

using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.Formatters;

using StreamCompanion.Services;

namespace StreamCompanion;

public static class Program
{
    private static void ConfigureLogging(WebApplicationBuilder builder)
    {
        builder.Host.ConfigureLogging(logging => {
            logging.ClearProviders();
            logging.AddConsole();
        });
    }

    private static WebApplication BuildApp(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers()
            .AddJsonOptions(options => {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.Configure<JsonSerializerOptions>(options => {
            options.Converters.Add(new JsonStringEnumConverter(null, false));
        });

        ConfigureLogging(builder);

        // Добавление сервисов
        StreamCompanionService companionService = new(builder.Services);
        builder.Services.AddSingleton(companionService);

        WebApplication app = builder.Build();

        // Инициализация основного сервиса
        companionService.Init(app.Services.GetService<ServiceResolver>());

        return app;
    }

    private static void ConfigureApp(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

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