using Main.Common.Models;
using Main.Common.Persistence.ApiClient;
using Main.Common.Persistence.DatabaseContext;
using Main.Modules.DisasterPredictionModule.Services.RiskCalculator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<AppSettingModel>().Bind(builder.Configuration);
builder.Services.AddDbContext<PostgreSqlDbContext>((serviceProvider,option) =>
{
    // var appsettingModel = serviceProvider.GetService<IOptions<AppSettingModel>>();
    // option.UseNpgsql(appsettingModel.Value.ConnectionStrings.PostgreSQL);
});
builder.Services.AddSingleton<IOpenWeatherClient, OpenWeatherClient>();
builder.Services.AddSingleton<IRiskCalculatorService, RiskCalculatorService>();

builder.Services.AddControllers();
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
