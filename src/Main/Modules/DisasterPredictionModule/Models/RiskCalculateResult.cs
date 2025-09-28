using System;
using Main.Modules.DisasterPredictionModule.Enums;

namespace Main.Modules.DisasterPredictionModule.Models;

public class RiskCalculateResult
{
    public RiskLevel RiskLevel { get; set; }
    public double RiskScore { get; set; }
}
