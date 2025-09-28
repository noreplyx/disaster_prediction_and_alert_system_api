using System;
using System.Threading.Tasks;
using Main.Modules.DisasterPredictionModule.Enums;
using Main.Modules.DisasterPredictionModule.Models;

namespace Main.Modules.DisasterPredictionModule.Services.RiskCalculator;

public class RiskCalculatorService : IRiskCalculatorService
{

    public (RiskLevel riskLevel, double RiskScore) CalculateEarthquake(EarthquakeCalculateMaterial calculateMaterial)
    {
        var score = calculateMaterial.Humidity * calculateMaterial.WindSpeed;

        if (score > 10) return (RiskLevel.High, score);
        if (score > 5) return (RiskLevel.Medium, score);
        return (RiskLevel.Low, score);
    }

    public (RiskLevel riskLevel, double RiskScore) CalculateFlood(FloodRiskCalculateMaterial calculateMaterial)
    {
        var score = calculateMaterial.WindSpeed / calculateMaterial.WindGust;

        if (score > 10) return (RiskLevel.High, score);
        if (score > 5) return (RiskLevel.Medium, score);
        return (RiskLevel.Low, score);
    }

    public (RiskLevel riskLevel, double RiskScore) CalculateWildfire(WildfireCalculateMaterial calculateMaterial)
    {
        var score = calculateMaterial.WindDegree * calculateMaterial.WindSpeed;

        if (score > 10) return (RiskLevel.High, score);
        if (score > 5) return (RiskLevel.Medium, score);
        return (RiskLevel.Low, score);
    }
}
