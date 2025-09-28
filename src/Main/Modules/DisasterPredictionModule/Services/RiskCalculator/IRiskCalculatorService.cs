using System;
using Main.Modules.DisasterPredictionModule.Enums;
using Main.Modules.DisasterPredictionModule.Models;

namespace Main.Modules.DisasterPredictionModule.Services.RiskCalculator;

public interface IRiskCalculatorService
{
    Task<RiskCalculateResult> CalculateFloodAsync(
        FloodRiskCalculateMaterial calculateMaterial
    );
    Task<RiskCalculateResult> CalculateEarthquakeAsync(
        EarthquakeCalculateMaterial calculateMaterial
    );

    Task<RiskCalculateResult> CalculateWildfireAsync(
        WildfireCalculateMaterial calculateMaterial
    );
}
