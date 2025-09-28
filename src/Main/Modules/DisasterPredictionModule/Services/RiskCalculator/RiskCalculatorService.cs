using System;
using System.Text.Json;
using System.Threading.Tasks;
using Main.Common.Models;
using Main.Modules.DisasterPredictionModule.Enums;
using Main.Modules.DisasterPredictionModule.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Main.Modules.DisasterPredictionModule.Services.RiskCalculator;

public class RiskCalculatorService : IRiskCalculatorService
{
    private readonly IDistributedCache _cache;
    private readonly IOptionsMonitor<AppSettingModel> _appSetting;

    public RiskCalculatorService(
        IDistributedCache cache,
        IOptionsMonitor<AppSettingModel> appSetting
    )
    {
        _cache = cache;
        _appSetting = appSetting;
    }

    public async Task<RiskCalculateResult> CalculateEarthquakeAsync(EarthquakeCalculateMaterial calculateMaterial)
    {
        var cacheKey = $"earthquake-humidity-${calculateMaterial.Humidity}-windspeed-{calculateMaterial.WindSpeed}";
        var cacheData = await _cache.GetStringAsync(cacheKey);
        var isInCache = !String.IsNullOrEmpty(cacheData) && !String.IsNullOrWhiteSpace(cacheData);
        if (isInCache)
        {
            return JsonSerializer.Deserialize<RiskCalculateResult>(cacheData);
        }
        var score = calculateMaterial.Humidity * calculateMaterial.WindSpeed;
        var riskLevel = RiskLevel.Low;
        if (score > 10) riskLevel = RiskLevel.High;
        else if (score > 5) riskLevel = RiskLevel.Medium;

        var result = new RiskCalculateResult
        {
            RiskLevel = riskLevel,
            RiskScore = score
        };

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result));
        return result;
    }

    public async Task<RiskCalculateResult> CalculateFloodAsync(FloodRiskCalculateMaterial calculateMaterial)
    {
        var cacheKey = $"flood-windspeed-${calculateMaterial.WindSpeed}-windgust-{calculateMaterial.WindGust}";
        var cacheData = await _cache.GetStringAsync(cacheKey);
        var isInCache = !String.IsNullOrEmpty(cacheData) && !String.IsNullOrWhiteSpace(cacheData);
        if (isInCache)
        {
            return JsonSerializer.Deserialize<RiskCalculateResult>(cacheData);
        }
        var score = calculateMaterial.WindSpeed / calculateMaterial.WindGust;

        var riskLevel = RiskLevel.Low;
        if (score > 10) riskLevel = RiskLevel.High;
        else if (score > 5) riskLevel = RiskLevel.Medium;

        var result = new RiskCalculateResult
        {
            RiskLevel = riskLevel,
            RiskScore = score
        };

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result));
        return result;
    }

    public async Task<RiskCalculateResult> CalculateWildfireAsync(WildfireCalculateMaterial calculateMaterial)
    {
        var cacheKey = $"wildfire-winddegree-${calculateMaterial.WindDegree}-windspeed-{calculateMaterial.WindSpeed}";
        var cacheData = await _cache.GetStringAsync(cacheKey);
        var isInCache = !String.IsNullOrEmpty(cacheData) && !String.IsNullOrWhiteSpace(cacheData);
        if (isInCache)
        {
            return JsonSerializer.Deserialize<RiskCalculateResult>(cacheData);
        }
        var score = calculateMaterial.WindDegree * calculateMaterial.WindSpeed;
        var riskLevel = RiskLevel.Low;
        if (score > 10) riskLevel = RiskLevel.High;
        else if (score > 5) riskLevel = RiskLevel.Medium;

        var result = new RiskCalculateResult
        {
            RiskLevel = riskLevel,
            RiskScore = score
        };

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result));
        return result;
    }
}
