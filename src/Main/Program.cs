using Common.Models;
using Common.Persistence.ApiClient;
using Main.Modules.DisasterPredictionModule.Services.RiskCalculator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<AppSettingModel>().Bind(builder.Configuration);
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
