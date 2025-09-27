using System;
using Main.Modules.DisasterPredictionModule.Enums;

namespace Main.Modules.DisasterPredictionModule.Models;

public class AlertData
{
    public string RegionId { get; set; }
    public string DisasterType { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public string AlertMessage { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}
