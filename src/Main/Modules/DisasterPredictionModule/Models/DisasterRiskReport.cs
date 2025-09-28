using System;
using Main.Modules.DisasterPredictionModule.Entities;
using Main.Modules.DisasterPredictionModule.Enums;

namespace Main.Modules.DisasterPredictionModule.Models;

public class DisasterRiskReport
{
    public Region Region { get; set; }
    public DisasterType DisasterType { get; set; }
    public double RiskScore { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public double Threshold { get; set; }
    public bool AlertTriggered { get; set; }
}
