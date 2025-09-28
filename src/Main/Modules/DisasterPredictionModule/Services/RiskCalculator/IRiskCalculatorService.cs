using System;
using Main.Modules.DisasterPredictionModule.Enums;
using Main.Modules.DisasterPredictionModule.Models;

namespace Main.Modules.DisasterPredictionModule.Services.RiskCalculator;

public interface IRiskCalculatorService
{
    (
        RiskLevel riskLevel,
        double RiskScore
    ) CalculateFlood(
        FloodRiskCalculateMaterial calculateMaterial
    );
    (
        RiskLevel riskLevel,
        double RiskScore
    ) CalculateEarthquake(
        EarthquakeCalculateMaterial calculateMaterial
    );

    (
        RiskLevel riskLevel,
        double RiskScore
    ) CalculateWildfire(
        WildfireCalculateMaterial calculateMaterial
    );
}
