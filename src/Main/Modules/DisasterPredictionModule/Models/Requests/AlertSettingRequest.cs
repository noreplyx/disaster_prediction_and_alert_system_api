using System;
using System.ComponentModel.DataAnnotations;

namespace Main.Modules.DisasterPredictionModule.Models.Requests;

public class AlertSettingRequest
{
    [Required]
    public string RegionId { get; set; }
    [Required]
    public string DisasterType { get; set; }
    [Required]
    public double ThresholdScore { get; set; }
}
