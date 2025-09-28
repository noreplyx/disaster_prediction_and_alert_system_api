using System.Text.Json;
using System.Text.Json.Serialization;
using Main.Common.Models;
using Main.Common.Persistence.ApiClient;
using Main.Common.Persistence.DatabaseContext;
using Main.Modules.DisasterPredictionModule.Services;
using Main.Modules.DisasterPredictionModule.Services.AlertService;
using Main.Modules.DisasterPredictionModule.Services.RiskCalculator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SendGrid.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<AppSettingModel>().Bind(builder.Configuration);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "{Timestamp:HH:mm} [{Level}] ({ThreadId}) {Message}{NewLine}{Exception}"
    )
    .CreateLogger();

builder.Services.AddLogging(logBuilder =>
{
    logBuilder.ClearProviders();
    logBuilder.AddSerilog(Log.Logger);
});
builder.Services.AddStackExchangeRedisCache((option) =>
{
    option.Configuration = builder.Configuration.GetConnectionString("redis");
    var redisConfiguration = builder.Configuration.GetSection("Redis");
    option.InstanceName = redisConfiguration.GetValue<string>("Prefix");
});
builder.Services.AddSendGrid((serviceProvider, option) =>
{
    var appsettingModel = serviceProvider.GetService<IOptions<AppSettingModel>>();
    option.ApiKey = appsettingModel.Value.SendGridConfiguration.ApiKey;
});
builder.Services.AddDbContext<PostgreSqlDbContext>((serviceProvider,option) =>
{
    // var appsettingModel = serviceProvider.GetService<IOptions<AppSettingModel>>();
    // option.UseNpgsql(appsettingModel.Value.ConnectionStrings.PostgreSQL);
});
builder.Services.AddSingleton<IOpenWeatherClient, OpenWeatherClient>();
builder.Services.AddSingleton<IRiskCalculatorService, RiskCalculatorService>();
builder.Services.AddSingleton<IAlertService, AlertService>();
builder.Services.AddScoped<IMasterDisasterPredictionService, MasterDisasterPredictionService>();

builder.Services.AddControllers().AddJsonOptions(config=>
{
    config.JsonSerializerOptions.WriteIndented = true;
    config.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.MapControllers();

await app.RunAsync();
