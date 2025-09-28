using System;
using Main.Modules.DisasterPredictionModule.Enums;

namespace Main.Modules.DisasterPredictionModule.Models.Responses;

public class DisasterRiskReportResponse
{
    public string RegionId { get; set; }
    public string DisasterType { get; set; }
    public double RiskScore { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public bool AlertTriggered { get; set; }
}
