using System;
using Main.Modules.DisasterPredictionModule.Enums;

namespace Main.Modules.DisasterPredictionModule.Models.Responses;

public class AlertDataResponse
{
    public string RegionId { get; set; }
    public string DisasterType { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public string AlertMessage { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public AlertType Type { get; set; }
}
