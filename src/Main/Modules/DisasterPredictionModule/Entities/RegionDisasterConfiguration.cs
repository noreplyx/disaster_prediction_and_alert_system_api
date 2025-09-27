using System;
using Main.Modules.DisasterPredictionModule.Models;

namespace Main.Modules.DisasterPredictionModule.Entities;

public class RegionDisasterConfiguration
{
    public int Id { get; set; }
    public int RegionId { get; set; }
    public Region Region { get; set; }
    public LocationCoordinate LocationCoordinate { get; set; }
    public int DisasterTypeId { get; set; }
    public DisasterType DisasterType { get; set; }
    public double Threshold { get; set; } = 0;
}
